using BepInEx.Configuration;
using Cpp2IL.Core.Extensions;
using HarmonyLib;
using Hazel;
using StellarRoles.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace StellarRoles
{
    public class CustomOption
    {
        public enum CustomOptionType
        {
            General,
            Impostor,
            NeutralK,
            Neutral,
            Crewmate,
            Modifier
        }

        public static readonly List<CustomOption> Options = new();
        public static int Preset { get; set; } = 0;
        public static ConfigEntry<string> VanillaSettings { get; set; }


        public readonly int Id;
        public readonly string Name;
        public readonly object[] Selections;

        public readonly int DefaultSelection;
        public ConfigEntry<int> Entry;
        public int Selection;
        public OptionBehaviour OptionBehaviour;
        public readonly CustomOption Parent;
        public readonly bool IsHeader;
        public readonly CustomOptionType Type;

        // Option creation

        public CustomOption(int id, CustomOptionType type, string name, object[] selections, object defaultValue, CustomOption parent, bool isHeader)
        {
            Id = id;
            Name = parent == null ? name : "- " + name;
            Selections = selections;
            int index = Array.IndexOf(selections, defaultValue);
            DefaultSelection = index >= 0 ? index : 0;
            Parent = parent;
            IsHeader = isHeader;
            Type = type;
            Selection = 0;
            if (id != 0)
            {
                Entry = StellarRolesPlugin.Instance.Config.Bind($"Preset{Preset}", id.ToString(), DefaultSelection);
                Selection = Mathf.Clamp(Entry.Value, 0, selections.Length - 1);
            }
            Options.Add(this);
        }

        public int DefaultSelectionByPreset(int preset)
        {
            object defaultObject = CustomOptionDefaultSettings.GetDefaultPresetValue(Id, preset);
            if (defaultObject == null)
                return DefaultSelection;
            else
            {
                int index = Array.IndexOf(Selections, defaultObject);
                return index >= 0 ? index : 0;
            }
        }


        public static CustomOption Create(int id, CustomOptionType type, string name, string[] selections, CustomOption parent = null, bool isHeader = false)
        {
            return new CustomOption(id, type, name, selections, "", parent, isHeader);
        }

        public static CustomOption Create(int id, CustomOptionType type, string name, float defaultValue, float min, float max, float step, CustomOption parent = null, bool isHeader = false)
        {
            List<object> selections = new();
            for (float s = min; s <= max; s += step)
                selections.Add(s);
            return new CustomOption(id, type, name, selections.ToArray(), defaultValue, parent, isHeader);
        }

        public static CustomOption Create(int id, CustomOptionType type, string name, bool defaultValue, CustomOption parent = null, bool isHeader = false)
        {
            return new CustomOption(id, type, name, new string[] { "Off", "On" }, defaultValue ? "On" : "Off", parent, isHeader);
        }

        // Static behaviour
        private static void SwitchPreset(int newPreset)
        {
            SaveVanillaOptions();
            Preset = newPreset;
            VanillaSettings = StellarRolesPlugin.Instance.Config.Bind($"Preset{Preset}", "GameOptions", "");
            LoadVanillaOptions();

            foreach (CustomOption option in Options)
            {
                if (option.Id == 0) continue;
                option.Entry = StellarRolesPlugin.Instance.Config.Bind($"Preset{Preset}", option.Id.ToString(), option.DefaultSelectionByPreset(Preset));
                option.Selection = Mathf.Clamp(option.Entry.Value, 0, option.Selections.Length - 1);
                if (option.OptionBehaviour is StringOption stringOption)
                {
                    stringOption.oldValue = stringOption.Value = option.Selection;
                    stringOption.ValueText.text = option.Selections[option.Selection].ToString();
                }
            }
        }
        public static void ResetPreset()
        {
            foreach (CustomOption option in Options)
            {
                if (option.Id == 0) continue;
                option.UpdateSelection(option.DefaultSelectionByPreset(Preset));
            }
        }

        public static void SaveVanillaOptions()
        {
            VanillaSettings.Value = Convert.ToBase64String(GameOptionsManager.Instance.gameOptionsFactory.ToBytes(GameManager.Instance.LogicOptions.currentGameOptions));
        }

        private static void LoadVanillaOptions()
        {
            string optionsString = VanillaSettings.Value;
            if (optionsString == "") return;
            GameOptionsManager.Instance.GameHostOptions = GameOptionsManager.Instance.gameOptionsFactory.FromBytes(Convert.FromBase64String(optionsString));
            GameOptionsManager.Instance.CurrentGameOptions = GameOptionsManager.Instance.GameHostOptions;
            GameManager.Instance.LogicOptions.SetGameOptions(GameOptionsManager.Instance.CurrentGameOptions);
            GameManager.Instance.LogicOptions.SyncOptions();
        }

        private static void ShareOptionChange(uint optionId)
        {
            CustomOption option = Options.FirstOrDefault(x => x.Id == optionId);
            if (option == null) return;
            MessageWriter writer = AmongUsClient.Instance!.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.ShareOptions, SendOption.Reliable, -1);
            writer.Write((byte)1);
            writer.WritePacked((uint)option.Id);
            writer.WritePacked(Convert.ToUInt32(option.Selection));
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        public static void ShareOptionSelections()
        {
            if (PlayerControl.AllPlayerControls.Count <= 1 || AmongUsClient.Instance!.AmHost == false) return;
            List<CustomOption> optionsList = Options.Clone();
            while (optionsList.Any())
            {
                byte amount = (byte)Math.Min(optionsList.Count, 200); // takes less than 3 bytes per option on average
                MessageWriter writer = AmongUsClient.Instance!.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.ShareOptions, SendOption.Reliable, -1);
                writer.Write(amount);
                for (int i = 0; i < amount; i++)
                {
                    CustomOption option = optionsList[0];
                    optionsList.RemoveAt(0);
                    writer.WritePacked((uint)option.Id);
                    writer.WritePacked(Convert.ToUInt32(option.Selection));
                }
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
        }

        // Getter

        public int GetSelection()
        {
            return Selection;
        }

        public bool GetBool()
        {
            return Selection > 0;
        }

        public float GetFloat()
        {
            return (float)Selections[Selection];
        }

        public int GetInt()
        {
            return (int)GetFloat();
        }

        public int GetQuantity()
        {
            return Selection + 1;
        }

        // Option changes

        public void UpdateSelection(int newSelection)
        {
            Selection = Mathf.Clamp((newSelection + Selections.Length) % Selections.Length, 0, Selections.Length - 1);
            if (OptionBehaviour != null && OptionBehaviour is StringOption stringOption)
            {
                stringOption.oldValue = stringOption.Value = Selection;
                stringOption.ValueText.text = Selections[Selection].ToString();

                if (AmongUsClient.Instance?.AmHost == true)
                {
                    if (Id == 0 && Selection != Preset)
                    {
                        SwitchPreset(Selection); // Switch presets
                        ShareOptionSelections();
                    }
                    else if (Entry != null)
                    {
                        Entry.Value = Selection; // Save selection to config
                        ShareOptionChange((uint)Id);// Share single selection
                    }
                }
            }
            else if (Id == 0 && AmongUsClient.Instance?.AmHost == true && PlayerControl.LocalPlayer)
            {  // Share the preset switch for random maps, even if the menu isnt open!
                SwitchPreset(Selection);
                ShareOptionSelections();// Share all selections
            }
        }

        public static byte[] SerializeOptions()
        {
            using MemoryStream memoryStream = new();
            using BinaryWriter binaryWriter = new(memoryStream);
            int lastId = -1;
            foreach (CustomOption option in Options.OrderBy(x => x.Id))
            {
                if (option.Id == 0)
                    continue;
                bool consecutive = lastId + 1 == option.Id;
                lastId = option.Id;

                binaryWriter.Write((byte)(option.Selection + (consecutive ? 128 : 0)));
                if (!consecutive)
                    binaryWriter.Write((ushort)option.Id);
            }
            binaryWriter.Flush();
            memoryStream.Position = 0L;
            return memoryStream.ToArray();
        }

        public static void DeserializeOptions(byte[] inputValues)
        {
            BinaryReader reader = new(new MemoryStream(inputValues));
            int lastId = -1;
            while (reader.BaseStream.Position < inputValues.Length)
            {
                try
                {
                    int selection = reader.ReadByte();
                    int id = -1;
                    bool consecutive = selection >= 128;
                    if (consecutive)
                    {
                        selection -= 128;
                        id = lastId + 1;
                    }
                    else
                    {
                        id = reader.ReadUInt16();
                    }
                    if (id == 0) continue;
                    lastId = id;
                    CustomOption option = Options.First(option => option.Id == id);
                    option.UpdateSelection(selection);
                }
                catch
                {
                    // ignore the error here as it is more than likely user error
                }
            }
        }

        // Copy to or paste from clipboard (as string)
        public static void CopyToClipboard()
        {
            GUIUtility.systemCopyBuffer = $"{StellarRolesPlugin.VersionString}!{Convert.ToBase64String(SerializeOptions())}!{VanillaSettings.Value}";
        }

        public static bool PasteFromClipboard()
        {
            try
            {
                string allSettings = GUIUtility.systemCopyBuffer;
                string[] settingsSplit = allSettings.Split("!");
                string versionInfo = settingsSplit[0];
                string torSettings = settingsSplit[1];
                string vanillaSettingsSub = settingsSplit[2];
                DeserializeOptions(Convert.FromBase64String(torSettings));

                VanillaSettings.Value = vanillaSettingsSub;
                LoadVanillaOptions();
                return true;
            }
            catch
            {
                // Ignore the error here as it is more than likely user error
                SoundEffectsManager.Load();
                SoundEffectsManager.Play(Sounds.Fail);
                return false;
            }
        }
    }

    [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Start))]
    class GameOptionsMenuStartPatch
    {
        public static void Postfix(GameOptionsMenu __instance)
        {
            // create copy to clipboard and paste from clipboard buttons.
            GameObject templateButton = UnityEngine.Object.FindObjectsOfType<GameObject>().FirstOrDefault(x => x.name == "CloseButton");
            GameObject resetButton = UnityEngine.Object.Instantiate(templateButton, templateButton.transform.parent);
            resetButton.transform.GetChild(0).gameObject.SetActive(false);
            resetButton.transform.localPosition += Vector3.down * 0.8f;
            PassiveButton resetButtonPassive = resetButton.GetComponent<PassiveButton>();
            SpriteRenderer resetButtonRenderer = resetButton.GetComponent<SpriteRenderer>();
            resetButtonRenderer.sprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.ResetButton.png", 175f);
            resetButtonPassive.OnClick.RemoveAllListeners();
            resetButtonPassive.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
            resetButtonPassive.OnClick.AddListener((Action)(() =>
            {
                CustomOption.ResetPreset();
                resetButtonRenderer.color = Color.green;
                __instance.StartCoroutine(Effects.Lerp(1f, new Action<float>((p) =>
                {
                    if (p > 0.95)
                        resetButtonRenderer.color = Color.white;
                })));
            }));
            GameObject resetButtonOverlay = UnityEngine.Object.Instantiate(templateButton, templateButton.transform.parent);
            SpriteRenderer resetButtonOverlayRenderer = resetButtonOverlay.GetComponent<SpriteRenderer>();
            resetButtonOverlayRenderer.sprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.ResetPresetToolTip.png", 100f);
            resetButtonOverlay.transform.localScale *= .25f;

            resetButtonOverlay.SetActive(false);
            PassiveButton resetButtonOverlayButton = resetButtonOverlay.GetComponent<PassiveButton>();
            resetButtonOverlayButton.OnClick.RemoveAllListeners();
            resetButtonOverlayButton.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();

            resetButtonPassive.OnMouseOver.RemoveAllListeners();
            resetButtonPassive.OnMouseOver = new UnityEngine.Events.UnityEvent();
            resetButtonPassive.OnMouseOver.AddListener((Action)(() =>
            {
                resetButtonOverlay.SetActive(true);
                resetButtonOverlay.transform.localPosition += Vector3.down * 1.2f + Vector3.right * 0.4f;
                resetButtonOverlay.transform.SetLocalZ(-50);
            }));

            resetButtonPassive.OnMouseOut.RemoveAllListeners();
            resetButtonPassive.OnMouseOut = new UnityEngine.Events.UnityEvent();
            resetButtonPassive.OnMouseOut.AddListener((Action)(() =>
            {
                resetButtonOverlay.SetActive(false);
            }));

            GameObject copyButton = UnityEngine.Object.Instantiate(templateButton, templateButton.transform.parent);
            copyButton.transform.GetChild(0).gameObject.SetActive(false);
            copyButton.transform.localPosition += Vector3.down * 1.6f;
            PassiveButton copyButtonPassive = copyButton.GetComponent<PassiveButton>();
            SpriteRenderer copyButtonRenderer = copyButton.GetComponent<SpriteRenderer>();
            copyButtonRenderer.sprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.CopyButton.png", 175f);
            copyButtonPassive.OnClick.RemoveAllListeners();
            copyButtonPassive.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
            copyButtonPassive.OnClick.AddListener((Action)(() =>
            {
                CustomOption.CopyToClipboard();
                copyButtonRenderer.color = Color.green;
                __instance.StartCoroutine(Effects.Lerp(1f, new Action<float>((p) =>
                {
                    if (p > 0.95)
                        copyButtonRenderer.color = Color.white;
                })));
            }));

            GameObject pasteButton = UnityEngine.Object.Instantiate(templateButton, templateButton.transform.parent);
            pasteButton.transform.GetChild(0).gameObject.SetActive(false);
            pasteButton.transform.localPosition += Vector3.down * 2.4f;
            PassiveButton pasteButtonPassive = pasteButton.GetComponent<PassiveButton>();
            SpriteRenderer pasteButtonRenderer = pasteButton.GetComponent<SpriteRenderer>();
            pasteButtonRenderer.sprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.PasteButton.png", 175f);
            pasteButtonPassive.OnClick.RemoveAllListeners();
            pasteButtonPassive.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
            pasteButtonPassive.OnClick.AddListener((Action)(() =>
            {
                pasteButtonRenderer.color = Color.yellow;
                bool success = CustomOption.PasteFromClipboard();
                pasteButtonRenderer.color = success ? Color.green : Color.red;
                __instance.StartCoroutine(Effects.Lerp(1f, new Action<float>((p) =>
                {
                    if (p > 0.95)
                        pasteButtonRenderer.color = Color.white;
                })));
            }));

            // TODI: change these to variables to not duplicate `GameObject.Find` - performance
            if (GameObject.Find("TORSettings") != null)
            { // Settings setup has already been performed, fixing the title of the tab and returning
                GameObject.Find("TORSettings").transform.FindChild("GameGroup").FindChild("Text").GetComponent<TMPro.TextMeshPro>().SetText("Stellar Roles Settings");
                return;
            }
            if (GameObject.Find("ImpostorSettings") != null)
            {
                GameObject.Find("ImpostorSettings").transform.FindChild("GameGroup").FindChild("Text").GetComponent<TMPro.TextMeshPro>().SetText("Impostor Roles Settings");
                return;
            }
            if (GameObject.Find("NeutralKSettings") != null)
            {
                GameObject.Find("NeutralKSettings").transform.FindChild("GameGroup").FindChild("Text").GetComponent<TMPro.TextMeshPro>().SetText("Neutral Killer Roles Settings");
                return;
            }
            if (GameObject.Find("NeutralSettings") != null)
            {
                GameObject.Find("NeutralSettings").transform.FindChild("GameGroup").FindChild("Text").GetComponent<TMPro.TextMeshPro>().SetText("Neutral Roles Settings");
                return;
            }
            if (GameObject.Find("CrewmateSettings") != null)
            {
                GameObject.Find("CrewmateSettings").transform.FindChild("GameGroup").FindChild("Text").GetComponent<TMPro.TextMeshPro>().SetText("Crewmate Roles Settings");
                return;
            }
            if (GameObject.Find("ModifierSettings") != null)
            {
                GameObject.Find("ModifierSettings").transform.FindChild("GameGroup").FindChild("Text").GetComponent<TMPro.TextMeshPro>().SetText("Modifier Settings");
                return;
            }

            // Setup TOR tab
            StringOption template = UnityEngine.Object.FindObjectsOfType<StringOption>().FirstOrDefault();
            if (template == null) return;
            GameObject gameSettings = GameObject.Find("Game Settings");
            GameSettingMenu gameSettingMenu = UnityEngine.Object.FindObjectsOfType<GameSettingMenu>().FirstOrDefault();

            GameObject torSettings = UnityEngine.Object.Instantiate(gameSettings, gameSettings.transform.parent);
            GameOptionsMenu torMenu = torSettings.transform.FindChild("GameGroup").FindChild("SliderInner").GetComponent<GameOptionsMenu>();
            torSettings.name = "TORSettings";

            GameObject impostorSettings = UnityEngine.Object.Instantiate(gameSettings, gameSettings.transform.parent);
            GameOptionsMenu impostorMenu = impostorSettings.transform.FindChild("GameGroup").FindChild("SliderInner").GetComponent<GameOptionsMenu>();
            impostorSettings.name = "ImpostorSettings";

            GameObject neutralKSettings = UnityEngine.Object.Instantiate(gameSettings, gameSettings.transform.parent);
            GameOptionsMenu neutralKMenu = neutralKSettings.transform.FindChild("GameGroup").FindChild("SliderInner").GetComponent<GameOptionsMenu>();
            neutralKSettings.name = "NeutralKSettings";

            GameObject neutralSettings = UnityEngine.Object.Instantiate(gameSettings, gameSettings.transform.parent);
            GameOptionsMenu neutralMenu = neutralSettings.transform.FindChild("GameGroup").FindChild("SliderInner").GetComponent<GameOptionsMenu>();
            neutralSettings.name = "NeutralSettings";

            GameObject crewmateSettings = UnityEngine.Object.Instantiate(gameSettings, gameSettings.transform.parent);
            GameOptionsMenu crewmateMenu = crewmateSettings.transform.FindChild("GameGroup").FindChild("SliderInner").GetComponent<GameOptionsMenu>();
            crewmateSettings.name = "CrewmateSettings";

            GameObject modifierSettings = UnityEngine.Object.Instantiate(gameSettings, gameSettings.transform.parent);
            GameOptionsMenu modifierMenu = modifierSettings.transform.FindChild("GameGroup").FindChild("SliderInner").GetComponent<GameOptionsMenu>();
            modifierSettings.name = "ModifierSettings";

            GameObject roleTab = GameObject.Find("RoleTab");
            GameObject gameTab = GameObject.Find("GameTab");

            GameObject torTab = UnityEngine.Object.Instantiate(roleTab, roleTab.transform.parent);
            SpriteRenderer torTabHighlight = torTab.transform.FindChild("Hat Button").FindChild("Tab Background").GetComponent<SpriteRenderer>();
            torTab.transform.FindChild("Hat Button").FindChild("Icon").GetComponent<SpriteRenderer>().sprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.LobbyMenu.TabIcon.png", 100f);

            GameObject impostorTab = UnityEngine.Object.Instantiate(roleTab, torTab.transform);
            SpriteRenderer impostorTabHighlight = impostorTab.transform.FindChild("Hat Button").FindChild("Tab Background").GetComponent<SpriteRenderer>();
            impostorTab.transform.FindChild("Hat Button").FindChild("Icon").GetComponent<SpriteRenderer>().sprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.LobbyMenu.TabIconImpostor.png", 100f);
            impostorTab.name = "ImpostorTab";

            GameObject neutralKTab = UnityEngine.Object.Instantiate(roleTab, impostorTab.transform);
            SpriteRenderer neutralKTabHighlight = neutralKTab.transform.FindChild("Hat Button").FindChild("Tab Background").GetComponent<SpriteRenderer>();
            neutralKTab.transform.FindChild("Hat Button").FindChild("Icon").GetComponent<SpriteRenderer>().sprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.LobbyMenu.TabIconNeutralK.png", 100f);
            neutralKTab.name = "NeutralKTab";

            GameObject neutralTab = UnityEngine.Object.Instantiate(roleTab, neutralKTab.transform);
            SpriteRenderer neutralTabHighlight = neutralTab.transform.FindChild("Hat Button").FindChild("Tab Background").GetComponent<SpriteRenderer>();
            neutralTab.transform.FindChild("Hat Button").FindChild("Icon").GetComponent<SpriteRenderer>().sprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.LobbyMenu.TabIconNeutral.png", 100f);
            neutralTab.name = "NeutralTab";

            GameObject crewmateTab = UnityEngine.Object.Instantiate(roleTab, neutralTab.transform);
            SpriteRenderer crewmateTabHighlight = crewmateTab.transform.FindChild("Hat Button").FindChild("Tab Background").GetComponent<SpriteRenderer>();
            crewmateTab.transform.FindChild("Hat Button").FindChild("Icon").GetComponent<SpriteRenderer>().sprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.LobbyMenu.TabIconCrewmate.png", 100f);
            crewmateTab.name = "CrewmateTab";

            GameObject modifierTab = UnityEngine.Object.Instantiate(roleTab, crewmateTab.transform);
            SpriteRenderer modifierTabHighlight = modifierTab.transform.FindChild("Hat Button").FindChild("Tab Background").GetComponent<SpriteRenderer>();
            modifierTab.transform.FindChild("Hat Button").FindChild("Icon").GetComponent<SpriteRenderer>().sprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.LobbyMenu.TabIconModifier.png", 100f);
            modifierTab.name = "ModifierTab";

            // Position of Tab Icons
            gameTab.transform.position += Vector3.left * 3.5f;
            roleTab.transform.position += Vector3.left * 3.5f;
            torTab.transform.position += Vector3.left * 2.5f;
            impostorTab.transform.localPosition = Vector3.right * 1f;
            neutralKTab.transform.localPosition = Vector3.right * 1f;
            neutralTab.transform.localPosition = Vector3.right * 1f;
            crewmateTab.transform.localPosition = Vector3.right * 1f;
            modifierTab.transform.localPosition = Vector3.right * 1f;

            GameObject[] tabs = new GameObject[] { gameTab, roleTab, torTab, impostorTab, neutralKTab, neutralTab, crewmateTab, modifierTab };
            for (int i = 0; i < tabs.Length; i++)
            {
                PassiveButton button = tabs[i].GetComponentInChildren<PassiveButton>();
                if (button == null) continue;
                int copiedIndex = i;
                button.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
                button.OnClick.AddListener((Action)(() =>
                {
                    gameSettingMenu.RegularGameSettings.SetActive(false);
                    gameSettingMenu.RolesSettings.gameObject.SetActive(false);
                    torSettings.gameObject.SetActive(false);
                    impostorSettings.gameObject.SetActive(false);
                    neutralKSettings.gameObject.SetActive(false);
                    neutralSettings.gameObject.SetActive(false);
                    crewmateSettings.gameObject.SetActive(false);
                    modifierSettings.gameObject.SetActive(false);
                    gameSettingMenu.GameSettingsHightlight.enabled = false;
                    gameSettingMenu.RolesSettingsHightlight.enabled = false;
                    torTabHighlight.enabled = false;
                    impostorTabHighlight.enabled = false;
                    neutralKTabHighlight.enabled = false;
                    neutralTabHighlight.enabled = false;
                    crewmateTabHighlight.enabled = false;
                    modifierTabHighlight.enabled = false;
                    switch (copiedIndex)
                    {
                        case 0:
                            gameSettingMenu.RegularGameSettings.SetActive(true);
                            gameSettingMenu.GameSettingsHightlight.enabled = true;
                            break;
                        case 1:
                            gameSettingMenu.RolesSettings.gameObject.SetActive(true);
                            gameSettingMenu.RolesSettingsHightlight.enabled = true;
                            break;
                        case 2:
                            torSettings.gameObject.SetActive(true);
                            torTabHighlight.enabled = true;
                            break;
                        case 3:
                            impostorSettings.gameObject.SetActive(true);
                            impostorTabHighlight.enabled = true;
                            break;
                        case 4:
                            neutralKSettings.gameObject.SetActive(true);
                            neutralKTabHighlight.enabled = true;
                            break;
                        case 5:
                            neutralSettings.gameObject.SetActive(true);
                            neutralTabHighlight.enabled = true;
                            break;
                        case 6:
                            crewmateSettings.gameObject.SetActive(true);
                            crewmateTabHighlight.enabled = true;
                            break;
                        case 7:
                            modifierSettings.gameObject.SetActive(true);
                            modifierTabHighlight.enabled = true;
                            break;
                    }
                }));
            }

            foreach (OptionBehaviour option in torMenu.GetComponentsInChildren<OptionBehaviour>())
                UnityEngine.Object.Destroy(option.gameObject);
            List<OptionBehaviour> torOptions = new();

            foreach (OptionBehaviour option in impostorMenu.GetComponentsInChildren<OptionBehaviour>())
                UnityEngine.Object.Destroy(option.gameObject);
            List<OptionBehaviour> impostorOptions = new();

            foreach (OptionBehaviour option in neutralKMenu.GetComponentsInChildren<OptionBehaviour>())
                UnityEngine.Object.Destroy(option.gameObject);
            List<OptionBehaviour> neutralKOptions = new();

            foreach (OptionBehaviour option in neutralMenu.GetComponentsInChildren<OptionBehaviour>())
                UnityEngine.Object.Destroy(option.gameObject);
            List<OptionBehaviour> neutralOptions = new();

            foreach (OptionBehaviour option in crewmateMenu.GetComponentsInChildren<OptionBehaviour>())
                UnityEngine.Object.Destroy(option.gameObject);
            List<OptionBehaviour> crewmateOptions = new();

            foreach (OptionBehaviour option in modifierMenu.GetComponentsInChildren<OptionBehaviour>())
                UnityEngine.Object.Destroy(option.gameObject);
            List<OptionBehaviour> modifierOptions = new();


            List<Transform> menus = new() { torMenu.transform, impostorMenu.transform, neutralKMenu.transform, neutralMenu.transform, crewmateMenu.transform, modifierMenu.transform };
            List<List<OptionBehaviour>> optionBehaviours = new() { torOptions, impostorOptions, neutralKOptions, neutralOptions, crewmateOptions, modifierOptions };

            for (int i = 0; i < CustomOption.Options.Count; i++)
            {
                CustomOption option = CustomOption.Options[i];
                if (option.OptionBehaviour == null)
                {
                    StringOption stringOption = UnityEngine.Object.Instantiate(template, menus[(int)option.Type]);
                    optionBehaviours[(int)option.Type].Add(stringOption);
                    stringOption.OnValueChanged = new Action<OptionBehaviour>((o) => { });
                    stringOption.TitleText.text = option.Name;
                    stringOption.Value = stringOption.oldValue = option.Selection;
                    stringOption.ValueText.text = option.Selections[option.Selection].ToString();

                    option.OptionBehaviour = stringOption;
                }
                option.OptionBehaviour.gameObject.SetActive(true);
            }

            torMenu.Children = torOptions.ToArray();
            torSettings.gameObject.SetActive(false);

            impostorMenu.Children = impostorOptions.ToArray();
            impostorSettings.gameObject.SetActive(false);

            neutralKMenu.Children = neutralOptions.ToArray();
            neutralKSettings.gameObject.SetActive(false);

            neutralMenu.Children = neutralOptions.ToArray();
            neutralSettings.gameObject.SetActive(false);

            crewmateMenu.Children = crewmateOptions.ToArray();
            crewmateSettings.gameObject.SetActive(false);

            modifierMenu.Children = modifierOptions.ToArray();
            modifierSettings.gameObject.SetActive(false);

            // Adapt task count for main options

            NumberOption commonTasksOption = __instance.Children.FirstOrDefault(x => x.name == "NumCommonTasks").TryCast<NumberOption>();
            if (commonTasksOption != null) commonTasksOption.ValidRange = new FloatRange(0f, 4f);

            NumberOption shortTasksOption = __instance.Children.FirstOrDefault(x => x.name == "NumShortTasks").TryCast<NumberOption>();
            if (shortTasksOption != null) shortTasksOption.ValidRange = new FloatRange(0f, 23f);

            NumberOption longTasksOption = __instance.Children.FirstOrDefault(x => x.name == "NumLongTasks").TryCast<NumberOption>();
            if (longTasksOption != null) longTasksOption.ValidRange = new FloatRange(0f, 15f);
        }
    }

    [HarmonyPatch(typeof(StringOption), nameof(StringOption.OnEnable))]
    public class StringOptionEnablePatch
    {
        public static bool Prefix(StringOption __instance)
        {
            CustomOption option = CustomOption.Options.FirstOrDefault(option => option.OptionBehaviour == __instance);
            if (option == null) return true;

            __instance.OnValueChanged = new Action<OptionBehaviour>((o) => { });
            __instance.TitleText.text = option.Name;
            __instance.Value = __instance.oldValue = option.Selection;
            __instance.ValueText.text = option.Selections[option.Selection].ToString();

            return false;
        }
    }

    [HarmonyPatch(typeof(StringOption), nameof(StringOption.Increase))]
    public class StringOptionIncreasePatch
    {
        public static bool Prefix(StringOption __instance)
        {
            CustomOption option = CustomOption.Options.FirstOrDefault(option => option.OptionBehaviour == __instance);
            if (option == null) return true;
            option.UpdateSelection(option.Selection + 1);
            return false;
        }
    }

    [HarmonyPatch(typeof(StringOption), nameof(StringOption.Decrease))]
    public class StringOptionDecreasePatch
    {
        public static bool Prefix(StringOption __instance)
        {
            CustomOption option = CustomOption.Options.FirstOrDefault(option => option.OptionBehaviour == __instance);
            if (option == null) return true;
            option.UpdateSelection(option.Selection - 1);
            return false;
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSyncSettings))]
    public class RpcSyncSettingsPatch
    {
        public static void Postfix()
        {
            CustomOption.ShareOptionSelections();
            CustomOption.SaveVanillaOptions();
        }
    }

    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.CoSpawnPlayer))]
    public class AmongUsClientOnPlayerJoinedPatch
    {
        public static void Postfix()
        {
            if (PlayerControl.LocalPlayer != null && AmongUsClient.Instance.AmHost)
            {
                CustomOption.ShareOptionSelections();
            }

        }
    }


    [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Update))]
    class GameOptionsMenuUpdatePatch
    {
        private static float timer = 1f;
        public static void Postfix(GameOptionsMenu __instance)
        {
            // Return Menu Update if in normal among us settings 
            GameSettingMenu gameSettingMenu = UnityEngine.Object.FindObjectsOfType<GameSettingMenu>().FirstOrDefault();
            if (gameSettingMenu.RegularGameSettings.active || gameSettingMenu.RolesSettings.gameObject.active) return;

            __instance.GetComponentInParent<Scroller>().ContentYBounds.max = -0.5F + __instance.Children.Length * 0.55F;
            timer += Time.deltaTime;
            if (timer < 0.1f) return;
            timer = 0f;

            float offset = 2.75f;
            foreach (CustomOption option in CustomOption.Options)
            {

                if (GameObject.Find("TORSettings") && option.Type != CustomOption.CustomOptionType.General)
                    continue;
                if (GameObject.Find("ImpostorSettings") && option.Type != CustomOption.CustomOptionType.Impostor)
                    continue;
                if (GameObject.Find("NeutralKSettings") && option.Type != CustomOption.CustomOptionType.NeutralK)
                    continue;
                if (GameObject.Find("NeutralSettings") && option.Type != CustomOption.CustomOptionType.Neutral)
                    continue;
                if (GameObject.Find("CrewmateSettings") && option.Type != CustomOption.CustomOptionType.Crewmate)
                    continue;
                if (GameObject.Find("ModifierSettings") && option.Type != CustomOption.CustomOptionType.Modifier)
                    continue;
                if (option?.OptionBehaviour != null && option.OptionBehaviour.gameObject != null)
                {
                    bool enabled = true;
                    CustomOption parent = option.Parent;
                    while (parent != null && enabled)
                    {
                        enabled = parent.Selection != 0;
                        parent = parent.Parent;
                    }
                    option.OptionBehaviour.gameObject.SetActive(enabled);
                    if (enabled)
                    {
                        offset -= option.IsHeader ? 0.75f : 0.5f;
                        option.OptionBehaviour.transform.localPosition = new Vector3(option.OptionBehaviour.transform.localPosition.x, offset, option.OptionBehaviour.transform.localPosition.z);
                    }
                }
            }
        }
    }

    [HarmonyPatch]
    public static class GameOptionsDataPatch
    {
        private static string buildRoleOptions()
        {
            string impRoles = "\n" + CustomOptionHolder.cs(Palette.ImpostorRed, "<size=150%>Impostors</size>") + buildOptionsOfType(CustomOption.CustomOptionType.Impostor, true) + "\n";
            string neutralKRoles = CustomOptionHolder.cs(NeutralKiller.Color, "<size=150%>Neutral Killers</size>") + buildOptionsOfType(CustomOption.CustomOptionType.NeutralK, true) + "\n";
            string neutralRoles = CustomOptionHolder.cs(Color.gray, "<size=150%>Neutrals</size>") + buildOptionsOfType(CustomOption.CustomOptionType.Neutral, true) + "\n";
            string crewRoles = CustomOptionHolder.cs(Color.cyan, "<size=150%>Crewmates</size>") + buildOptionsOfType(CustomOption.CustomOptionType.Crewmate, true) + "\n";
            string modifiers = CustomOptionHolder.cs(Spiteful.Color, "<size=150%>Modifiers</size>") + buildOptionsOfType(CustomOption.CustomOptionType.Modifier, true);
            return impRoles + neutralKRoles + neutralRoles + crewRoles + modifiers;
        }

        private static string buildOptionsOfType(CustomOption.CustomOptionType type, bool headerOnly)
        {
            StringBuilder sb = new("\n");
            IEnumerable<CustomOption> options = CustomOption.Options.Where(o => o.Type == type);

            if (headerOnly)
            {
                foreach (CustomOption option in options)
                {
                    bool morphling = option == CustomOptionHolder.MorphlingIsNeutral && CustomOptionHolder.MorphlingIsNeutral.GetBool();
                    bool bomber = option == CustomOptionHolder.BomberIsNeutral && CustomOptionHolder.BomberIsNeutral.GetBool();
                    bool camouflager = option == CustomOptionHolder.CamouflagerIsNeutral && CustomOptionHolder.CamouflagerIsNeutral.GetBool();
                    bool janitor = option == CustomOptionHolder.JanitorIsNeutral && CustomOptionHolder.JanitorIsNeutral.GetBool();
                    bool miner = option == CustomOptionHolder.MinerIsNeutral && CustomOptionHolder.MinerIsNeutral.GetBool();
                    bool shade = option == CustomOptionHolder.ShadeIsNeutral && CustomOptionHolder.ShadeIsNeutral.GetBool();
                    bool undertaker = option == CustomOptionHolder.UndertakerIsNeutral && CustomOptionHolder.UndertakerIsNeutral.GetBool();
                    bool vampire = option == CustomOptionHolder.VampireIsNeutral && CustomOptionHolder.VampireIsNeutral.GetBool();
                    bool warlock = option == CustomOptionHolder.WarlockIsNeutral && CustomOptionHolder.WarlockIsNeutral.GetBool();
                    bool wraith = option == CustomOptionHolder.WraithIsNeutral && CustomOptionHolder.WraithIsNeutral.GetBool();
                    bool bountyhunter = option == CustomOptionHolder.BountyHunterIsNeutral && CustomOptionHolder.BountyHunterIsNeutral.GetBool();


                    if (morphling || bomber || camouflager || janitor || miner || shade || undertaker || vampire || warlock || wraith || bountyhunter)
                    {
                        sb.AppendLine($"{option.Name}");
                    }

                    else if (option.Parent == null
                        && option != CustomOptionHolder.RefugeeSpawnRate
                        && option != CustomOptionHolder.CrewmateRolesCountMax
                        && option != CustomOptionHolder.CrewmateRolesCountMin
                        && option != CustomOptionHolder.NeutralRolesCountMin
                        && option != CustomOptionHolder.NeutralRolesCountMax
                        && option != CustomOptionHolder.ImpostorRolesCountMax
                        && option != CustomOptionHolder.ImpostorRolesCountMin
                        && option != CustomOptionHolder.NeutralKillerRolesCountMax
                        && option != CustomOptionHolder.NeutralKillerRolesCountMin
                        && option != CustomOptionHolder.ModifiersMiscCountMax
                        && option != CustomOptionHolder.ModifiersMiscCountMin
                        && option != CustomOptionHolder.ModifierCosmeticMin
                        && option != CustomOptionHolder.ModifierCosmeticMax
                        && option != CustomOptionHolder.ModifiersImpCountMax
                        && option != CustomOptionHolder.NeutralKillersGetNonCritSabo
                        && option != CustomOptionHolder.EnableRogueImpostors
                        && option != CustomOptionHolder.ImpsLooseCritSabo
                        && option != CustomOptionHolder.ImpsLoseDoors
                        && option != CustomOptionHolder.NeutralKillerGainsAssassin
                        && option != CustomOptionHolder.ModifiersImpCountMin)
                    {
                        sb.AppendLine($"{option.Name}: {option.Selections[option.Selection].ToString()}");
                    }
                }
                return sb.ToString();
            }

            foreach (CustomOption option in options)
            {
                if (option.Parent != null)
                {
                    bool isIrrelevant = option.Parent.GetSelection() == 0 || (option.Parent.Parent != null && option.Parent.Parent.GetSelection() == 0);

                    Color c = isIrrelevant ? Color.grey : Color.white;  // No use for now
                    if (isIrrelevant) continue;
                    sb.AppendLine(Helpers.ColorString(c, $"{option.Name}: {option.Selections[option.Selection]}"));
                }
                else
                {
                    if (option == CustomOptionHolder.CrewmateRolesCountMin)
                    {
                        string optionName = CustomOptionHolder.cs(Palette.CrewmateBlue, "Crewmate Roles");
                        int min = CustomOptionHolder.CrewmateRolesCountMin.GetSelection();
                        int max = CustomOptionHolder.CrewmateRolesCountMax.GetSelection();
                        if (min > max) min = max;
                        string optionValue = (min == max) ? $"{max}" : $"{min} - {max}";
                        sb.AppendLine($"<u>{optionName}: {optionValue}</u>");
                    }
                    else if (option == CustomOptionHolder.NeutralRolesCountMin)
                    {
                        string optionName = CustomOptionHolder.cs(Color.gray, "Neutral Roles");
                        int min = CustomOptionHolder.NeutralRolesCountMin.GetSelection();
                        int max = CustomOptionHolder.NeutralRolesCountMax.GetSelection();
                        if (min > max) min = max;
                        string optionValue = (min == max) ? $"{max}" : $"{min} - {max}";
                        sb.AppendLine($"<u>{optionName}: {optionValue}</u>");
                    }
                    else if (option == CustomOptionHolder.NeutralKillerRolesCountMin)
                    {
                        string optionName = CustomOptionHolder.cs(NeutralKiller.Color, "Neutral Killer Roles");
                        int min = CustomOptionHolder.NeutralKillerRolesCountMin.GetSelection();
                        int max = CustomOptionHolder.NeutralKillerRolesCountMax.GetSelection();
                        if (min > max) min = max;
                        string optionValue = (min == max) ? $"{max}" : $"{min} - {max}";
                        sb.AppendLine($"<u>{optionName}: {optionValue}</u>");
                    }
                    else if (option == CustomOptionHolder.ImpostorRolesCountMin)
                    {
                        string optionName = CustomOptionHolder.cs(Palette.ImpostorRed, "Impostor Roles");
                        int min = CustomOptionHolder.ImpostorRolesCountMin.GetSelection();
                        int max = CustomOptionHolder.ImpostorRolesCountMax.GetSelection();
                        if (min > max) min = max;
                        string optionValue = (min == max) ? $"{max}" : $"{min} - {max}";
                        sb.AppendLine($"<u>{optionName}: {optionValue}</u>");
                    }
                    else if (option == CustomOptionHolder.ModifierCosmeticMin)
                    {
                        string optionName = CustomOptionHolder.cs(Spiteful.Color, "Cosmetic Modifiers");
                        int min = CustomOptionHolder.ModifierCosmeticMin.GetSelection();
                        int max = CustomOptionHolder.ModifierCosmeticMax.GetSelection();
                        if (min > max) min = max;
                        string optionValue = (min == max) ? $"{max}" : $"{min} - {max}";
                        sb.AppendLine();
                        sb.AppendLine($"<u>{optionName}: {optionValue}</u>");
                    }
                    else if (option == CustomOptionHolder.ModifiersImpCountMin)
                    {
                        string optionName = CustomOptionHolder.cs(Palette.ImpostorRed, "Impostor Modifiers");
                        int min = CustomOptionHolder.ModifiersImpCountMin.GetSelection();
                        int max = CustomOptionHolder.ModifiersImpCountMax.GetSelection();
                        if (min > max) min = max;
                        string optionValue = (min == max) ? $"{max}" : $"{min} - {max}";
                        sb.AppendLine();
                        sb.AppendLine($"<u>{optionName}: {optionValue}</u>");
                    }
                    else if (option == CustomOptionHolder.ModifiersMiscCountMin)
                    {
                        string optionName = CustomOptionHolder.cs(Spiteful.Color, "Misc Modifiers");
                        int min = CustomOptionHolder.ModifiersMiscCountMin.GetSelection();
                        int max = CustomOptionHolder.ModifiersMiscCountMax.GetSelection();
                        if (min > max) min = max;
                        string optionValue = (min == max) ? $"{max}" : $"{min} - {max}";
                        sb.AppendLine();
                        sb.AppendLine($"<u>{optionName}: {optionValue}</u>");
                    }
                    else if (option == CustomOptionHolder.CrewmateRolesCountMax
                        || option == CustomOptionHolder.RefugeeSpawnRate
                        || option == CustomOptionHolder.NeutralRolesCountMax
                        || option == CustomOptionHolder.NeutralKillerRolesCountMax
                        || option == CustomOptionHolder.ImpostorRolesCountMax
                        || option == CustomOptionHolder.ModifiersMiscCountMax
                        || option == CustomOptionHolder.ModifiersImpCountMax
                        || option == CustomOptionHolder.ModifierCosmeticMax
                        ) continue;
                    else
                        sb.AppendLine($"\n{option.Name}: {option.Selections[option.Selection].ToString()}");
                }
            }
            return sb.ToString();
        }

        [HarmonyPatch(typeof(IGameOptionsExtensions), nameof(IGameOptionsExtensions.ToHudString))]
        private static void Postfix(ref string __result)
        {
            if (GameOptionsManager.Instance.currentGameOptions.GameMode == AmongUs.GameOptions.GameModes.HideNSeek) return; // Allow Vanilla Hide N Seek

            int counter = StellarRolesPlugin.optionsPage;
            string hudString = counter != 0 ? Helpers.ColorString(DateTime.Now.Second % 2 == 0 ? Color.white : Color.red, "(Use scroll wheel if necessary)\n\n") : "";

            switch (counter)
            {
                case 0:
                    hudString += "Page 1: Vanilla Settings \n\n" + __result;
                    break;
                case 1:
                    hudString += "Page 2: Stellar Roles Settings \n" + buildOptionsOfType(CustomOption.CustomOptionType.General, false);
                    break;
                case 2:
                    hudString += "Page 3: Role and Modifier Rates \n" + buildRoleOptions();
                    break;
                case 3:
                    hudString += "Page 4: Impostor Role Settings \n" + buildOptionsOfType(CustomOption.CustomOptionType.Impostor, false);
                    break;
                case 4:
                    hudString += "Page 5: Neutral Killer Role Settings \n" + buildOptionsOfType(CustomOption.CustomOptionType.NeutralK, false);
                    break;
                case 5:
                    hudString += "Page 6: Neutral Role Settings \n" + buildOptionsOfType(CustomOption.CustomOptionType.Neutral, false);
                    break;
                case 6:
                    hudString += "Page 7: Crewmate Role Settings \n" + buildOptionsOfType(CustomOption.CustomOptionType.Crewmate, false);
                    break;
                case 7:
                    hudString += "Page 8: Modifier Settings \n" + buildOptionsOfType(CustomOption.CustomOptionType.Modifier, false);
                    break;
            }

            hudString += $"\n Press TAB or Page Number for more... ({counter + 1}/8)";
            __result = hudString;
        }
    }

    [HarmonyPatch(typeof(KeyboardJoystick), nameof(KeyboardJoystick.Update))]
    public static class GameOptionsNextPagePatch
    {
        public static void Postfix()
        {
            int page = StellarRolesPlugin.optionsPage;
            if (Input.GetKeyDown(KeyCode.Tab))
                StellarRolesPlugin.optionsPage = (StellarRolesPlugin.optionsPage + 1) % 8;
            if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
                StellarRolesPlugin.optionsPage = 0;
            if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
                StellarRolesPlugin.optionsPage = 1;
            if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
                StellarRolesPlugin.optionsPage = 2;
            if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
                StellarRolesPlugin.optionsPage = 3;
            if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5))
                StellarRolesPlugin.optionsPage = 4;
            if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6))
                StellarRolesPlugin.optionsPage = 5;
            if (Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Keypad7))
                StellarRolesPlugin.optionsPage = 6;
            if (Input.GetKeyDown(KeyCode.Alpha8) || Input.GetKeyDown(KeyCode.Keypad8))
                StellarRolesPlugin.optionsPage = 7;

            if (page != StellarRolesPlugin.optionsPage)
            {
                Vector3 position = (Vector3)FastDestroyableSingleton<HudManager>.Instance?.GameSettings?.transform.localPosition;
                FastDestroyableSingleton<HudManager>.Instance.GameSettings.transform.localPosition = new Vector3(position.x, 2.9f, position.z);
            }
        }
    }


    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class GameSettingsScalePatch
    {
        public static void Prefix(HudManager __instance)
        {
            if (__instance.GameSettings != null) __instance.GameSettings.fontSize = 1.2f;
        }
    }


    // This class is taken from Town of Us Reactivated, https://github.com/eDonnes124/Town-Of-Us-R/blob/master/source/Patches/CustomOption/Patches.cs, Licensed under GPLv3
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate
    {
        private static float MinX;
        private const float MinY = 2.9F;


        public static Scroller Scroller;
        private static Vector3 LastPosition;
        private static float lastAspect;
        private static bool setLastPosition = false;

        public static void Prefix(HudManager __instance)
        {
            if (__instance.GameSettings?.transform == null) return;

            // Sets the MinX position to the left edge of the screen + 0.1 units
            Rect safeArea = Screen.safeArea;
            float aspect = Mathf.Min(Camera.main.aspect, safeArea.width / safeArea.height);
            float safeOrthographicSize = CameraSafeArea.GetSafeOrthographicSize(Camera.main);
            MinX = 0.1f - safeOrthographicSize * aspect;

            if (!setLastPosition || aspect != lastAspect)
            {
                LastPosition = new Vector3(MinX, MinY);
                lastAspect = aspect;
                setLastPosition = true;
                if (Scroller != null) Scroller.ContentXBounds = new FloatRange(MinX, MinX);
            }

            CreateScroller(__instance);

            Scroller.gameObject.SetActive(__instance.GameSettings.gameObject.activeSelf);

            if (!Scroller.gameObject.active) return;

            int rows = __instance.GameSettings.text.Count(c => c == '\n');
            float LobbyTextRowHeight = 0.06F;
            float maxY = Mathf.Max(MinY, rows * LobbyTextRowHeight + (rows - 38) * LobbyTextRowHeight);

            Scroller.ContentYBounds = new FloatRange(MinY, maxY);

            // Prevent scrolling when the player is interacting with a menu
            if (!PlayerControl.LocalPlayer.CanMove)
            {
                __instance.GameSettings.transform.localPosition = LastPosition;
                return;
            }

            if (__instance.GameSettings.transform.localPosition.x != MinX ||
                __instance.GameSettings.transform.localPosition.y < MinY) return;

            LastPosition = __instance.GameSettings.transform.localPosition;
        }

        private static void CreateScroller(HudManager __instance)
        {
            if (Scroller != null) return;

            Scroller = new GameObject("SettingsScroller").AddComponent<Scroller>();
            Scroller.transform.SetParent(__instance.GameSettings.transform.parent);
            Scroller.gameObject.layer = 5;

            Scroller.transform.localScale = Vector3.one;
            Scroller.allowX = false;
            Scroller.allowY = true;
            Scroller.active = true;
            Scroller.velocity = new Vector2(0, 0);
            Scroller.ScrollbarYBounds = new FloatRange(0, 0);
            Scroller.ContentXBounds = new FloatRange(MinX, MinX);
            Scroller.enabled = true;

            Scroller.Inner = __instance.GameSettings.transform;
            __instance.GameSettings.transform.SetParent(Scroller.transform);
        }
    }
}
