using System.Collections.Generic;
using UnityEngine;
using BepInEx.Configuration;
using System;
using System.IO;
using System.Linq;
using HarmonyLib;
using Hazel;
using System.Text;
using StellarRoles.Utilities;
using static StellarRoles.StellarRoles;
using static StellarRoles.CustomOption;
using Reactor.Utilities.Extensions;
using AmongUs.GameOptions;
using TMPro;
using BepInEx.Unity.IL2CPP;
using BepInEx;

namespace StellarRoles
{
    public class CustomOption
    {
        public static readonly Color DefaultColor = new Color32(74, 127, 121, byte.MaxValue);
        private static Sprite _HeaderBackground;
        private static Sprite _HeaderDivider;

        public static Sprite GetHeaderSprite()
        {
            return _HeaderBackground ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.HeaderBackground.png", 100f);
        }
        public static Sprite GetDividerSprite()
        {
            return _HeaderDivider ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.HeaderDivider.png", 100f);
        }


        public enum CustomOptionType
        {
            General,
            Impostor,
            Neutral,
            NeutralK,
            Crewmate,
            Modifier,
            Map,
        }

        public static List<CustomOption> Options = new List<CustomOption>();
        public static int Preset = 0;
        public static ConfigEntry<string> VanillaSettings;

        public int Id;
        public string name;
        public System.Object[] Selections;

        public int DefaultSelection;
        public ConfigEntry<int> Entry;
        public int Selection;
        public OptionBehaviour OptionBehaviour;
        public CustomOption Parent;
        public bool isHeader;
        public CustomOptionType type;
        public Action onChange = null;
        public string heading = "";
        public Color HeaderColor;

        // Option creation

        public CustomOption(int id, CustomOptionType type, string name, System.Object[] selections, System.Object defaultValue, CustomOption parent, bool isHeader, Action onChange = null, string heading = "", Color headerColor = default)
        {
            this.Id = id;
            this.name = name;
            this.Selections = selections;
            int index = Array.IndexOf(selections, defaultValue);
            this.DefaultSelection = index >= 0 ? index : 0;
            this.Parent = parent;
            this.isHeader = isHeader;
            this.type = type;
            this.onChange = onChange;
            this.heading = heading;
            this.HeaderColor = Color.white;
            Selection = 0;
            if (id != 0)
            {
                Entry = StellarRolesPlugin.Instance.Config.Bind($"Preset{Preset}", id.ToString(), DefaultSelection);
                Selection = Mathf.Clamp(Entry.Value, 0, selections.Length - 1);
            }
            Options.Add(this);
            HeaderColor = headerColor;
        }

        public static CustomOption Create(int id, CustomOptionType type, string name, string[] selections, CustomOption parent = null, bool isHeader = false, Action onChange = null, string heading = "", Color headerColor = default)
        {
            return new CustomOption(id, type, name, selections, "", parent, isHeader, onChange, heading, headerColor);
        }

        public static CustomOption Create(int id, CustomOptionType type, string name, object[] selections, CustomOption parent = null, bool isHeader = false, Action onChange = null, string heading = "")
        {
            return new CustomOption(id, type, name, selections, "", parent, isHeader, onChange, heading);
        }

        public static CustomOption Create(int id, CustomOptionType type, string name, float defaultValue, float min, float max, float step, CustomOption parent = null, bool isHeader = false, Action onChange = null, string heading = "")
        {
            List<object> selections = new();
            for (float s = min; s <= max; s += step)
                selections.Add(s);
            return new CustomOption(id, type, name, selections.ToArray(), defaultValue, parent, isHeader, onChange, heading);
        }

        public static CustomOption Create(int id, CustomOptionType type, string name, bool defaultValue, CustomOption parent = null, bool isHeader = false, Action onChange = null, string heading = "", Color headerColor = default)
        {
            return new CustomOption(id, type, name, new string[] { "Off", "On" }, defaultValue ? "On" : "Off", parent, isHeader, onChange, heading, headerColor);
        }

        public static CustomOption CreateHeader(int id, CustomOptionType type, string header, Color headerColor = default)
        {
            return new CustomOption(id, type, "", new string[] { "HeaderOnly" }, "HeaderOnly", null, true, null, header, headerColor);
        }

        // Static behaviour

        public static void SwitchPreset(int newPreset)
        {
            SaveVanillaOptions();
            CustomOption.Preset = newPreset;
            VanillaSettings = StellarRolesPlugin.Instance.Config.Bind($"Preset{Preset}", "GameOptions", "");
            LoadVanillaOptions();

            var value = CustomOptionDefaultSettings.Presets((Preset)Preset);
            foreach (CustomOption option in CustomOption.Options)
            {
                if (option.Id == 0) continue;
                var optiondefault = option.DefaultSelection;
                if (value != null && value.TryGetValue(option.Id, out int newvalue))
                {
                    optiondefault = newvalue;
                }
                option.Entry = StellarRolesPlugin.Instance.Config.Bind($"Preset{Preset}", option.Id.ToString(), optiondefault);
                option.Selection = Mathf.Clamp(option.Entry.Value, 0, option.Selections.Length - 1);
                if (option.OptionBehaviour != null && option.OptionBehaviour is StringOption stringOption)
                {
                    stringOption.oldValue = stringOption.Value = option.Selection;
                    stringOption.ValueText.text = option.Selections[option.Selection].ToString();
                }
            }
        }

        public static void ResetPreset()
        {
            if (SetPreset((Preset)Preset)) return;

            foreach (CustomOption option in Options)
            {
                if (option.Id == 0) continue;
                option.UpdateSelection(option.DefaultSelection, false);
            }
        }

        public static bool SetPreset(Preset preset)
        {
            var pasteKey = CustomOptionDefaultSettings.GetDefaultPresetPasteKey(preset);
            if (pasteKey != "")
            {
                pasteFromClipboard(pasteKey);
                return true;
            }
            return false;
        }

        public static void SaveVanillaOptions()
        {
            VanillaSettings.Value = Convert.ToBase64String(GameOptionsManager.Instance.gameOptionsFactory.ToBytes(GameManager.Instance.LogicOptions.currentGameOptions, false));
        }

        public static bool LoadVanillaOptions()
        {
            string optionsString = VanillaSettings.Value;
            if (optionsString == "") return false;
            IGameOptions gameOptions = GameOptionsManager.Instance.gameOptionsFactory.FromBytes(Convert.FromBase64String(optionsString));
            if (gameOptions.Version < 8)
            {
                StellarRolesPlugin.Logger.LogMessage("tried to paste old settings, not doing this!");
                return false;
            }
            GameOptionsManager.Instance.GameHostOptions = gameOptions;
            GameOptionsManager.Instance.CurrentGameOptions = GameOptionsManager.Instance.GameHostOptions;
            GameManager.Instance.LogicOptions.SetGameOptions(GameOptionsManager.Instance.CurrentGameOptions);
            GameManager.Instance.LogicOptions.SyncOptions();
            return true;
        }

        public static void ShareOptionChange(uint optionId)
        {
            var option = Options.FirstOrDefault(x => x.Id == optionId);
            if (option == null) return;
            var writer = AmongUsClient.Instance!.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)254, SendOption.Reliable, -1);
            writer.Write((byte)CustomRPC.ShareOptions);
            writer.Write((byte)1);
            writer.WritePacked((uint)option.Id);
            writer.WritePacked(Convert.ToUInt32(option.Selection));
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        public static void ShareOptionSelections()
        {
            if (PlayerControl.AllPlayerControls.Count <= 1 || AmongUsClient.Instance!.AmHost == false && PlayerControl.LocalPlayer == null) return;
            var optionsList = new List<CustomOption>(CustomOption.Options);
            while (optionsList.Any())
            {
                byte amount = (byte)Math.Min(optionsList.Count, 200); // takes less than 3 bytes per option on average
                var writer = AmongUsClient.Instance!.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)254, SendOption.Reliable, -1);
                writer.Write((byte)CustomRPC.ShareOptions);
                writer.Write(amount);
                for (int i = 0; i < amount; i++)
                {
                    var option = optionsList[0];
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
            if (Selections[Selection] is float v)
            {
                return v;
            }
            return 0f;
        }

        public int GetInt()
        {
            return (int)GetFloat();
        }

        public int GetQuantity()
        {
            return Selection + 1;
        }


        public void UpdateSelection(int newSelection, bool notifyUsers = true)
        {
            newSelection = Mathf.Clamp((newSelection + Selections.Length) % Selections.Length, 0, Selections.Length - 1);
            if (AmongUsClient.Instance?.AmClient == true && notifyUsers && Selection != newSelection)
            {
                DestroyableSingleton<HudManager>.Instance.Notifier.AddSettingsChangeMessage((StringNames)(this.Id + 6000), Selections[newSelection].ToString(), false);
                try
                {
                    if (GameStartManager.Instance != null && GameStartManager.Instance.LobbyInfoPane != null && GameStartManager.Instance.LobbyInfoPane.LobbyViewSettingsPane != null && GameStartManager.Instance.LobbyInfoPane.LobbyViewSettingsPane.gameObject.activeSelf)
                    {
                        LobbyViewSettingsPaneChangeTabPatch.Postfix(GameStartManager.Instance.LobbyInfoPane.LobbyViewSettingsPane, GameStartManager.Instance.LobbyInfoPane.LobbyViewSettingsPane.currentTab);
                    }
                }
                catch { }
            }
            Selection = newSelection;
            try
            {
                if (onChange != null) onChange();
            }
            catch { }
            if (OptionBehaviour != null && OptionBehaviour is StringOption stringOption)
            {
                stringOption.oldValue = stringOption.Value = Selection;
                stringOption.ValueText.text = Selections[Selection].ToString();
                if (AmongUsClient.Instance?.AmHost == true && PlayerControl.LocalPlayer)
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
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
                {
                    int lastId = -1;
                    foreach (var option in CustomOption.Options.OrderBy(x => x.Id))
                    {
                        if (option.Id == 0) continue;
                        bool consecutive = lastId + 1 == option.Id;
                        lastId = option.Id;

                        binaryWriter.Write((byte)(option.Selection + (consecutive ? 128 : 0)));
                        if (!consecutive) binaryWriter.Write((ushort)option.Id);
                    }
                    binaryWriter.Flush();
                    memoryStream.Position = 0L;
                    return memoryStream.ToArray();
                }
            }
        }

        public static int DeserializeOptions(byte[] inputValues)
        {
            BinaryReader reader = new BinaryReader(new MemoryStream(inputValues));
            int lastId = -1;
            bool somethingApplied = false;
            int errors = 0;
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
                    option.Entry = StellarRolesPlugin.Instance.Config.Bind($"Preset{Preset}", option.Id.ToString(), option.DefaultSelection);
                    option.Selection = selection;
                    if (option.OptionBehaviour != null && option.OptionBehaviour is StringOption stringOption)
                    {
                        stringOption.oldValue = stringOption.Value = option.Selection;
                        stringOption.ValueText.text = option.Selections[option.Selection].ToString();
                    }
                    somethingApplied = true;
                }
                catch (Exception e)
                {
                    StellarRolesPlugin.Logger.LogWarning($"id:{lastId}:{e}: while deserializing - tried to paste invalid settings!");
                    errors++;
                }
            }
            return Convert.ToInt32(somethingApplied) + (errors > 0 ? 0 : 1);
        }

        // Copy to or paste from clipboard (as string)
        public static void copyToClipboard()
        {
            GUIUtility.systemCopyBuffer = $"{StellarRolesPlugin.VersionString}!{Convert.ToBase64String(SerializeOptions())}!{VanillaSettings.Value}";
        }

        public static int pasteFromClipboard(string preset = "")
        {
            string allSettings;
            if (preset != "") allSettings = preset;
            else allSettings = GUIUtility.systemCopyBuffer;
            int ModdedOptionsFine = 0;
            bool vanillaOptionsFine = false;
            try
            {
                var settingsSplit = allSettings.Split("!");
                Version versionInfo = Version.Parse(settingsSplit[0]);
                string ModdedSettings = settingsSplit[1];
                string vanillaSettingsSub = settingsSplit[2];
                ModdedOptionsFine = DeserializeOptions(Convert.FromBase64String(ModdedSettings));
                ShareOptionSelections();
                if (StellarRolesPlugin.Version > versionInfo && versionInfo < Version.Parse("24.8.7"))
                {
                    vanillaOptionsFine = false;
                    HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, "Host Info: Pasting vanilla settings failed, Modded Options applied!");
                }
                else
                {
                    VanillaSettings.Value = vanillaSettingsSub;
                    vanillaOptionsFine = LoadVanillaOptions();
                }
            }
            catch (Exception e)
            {
                StellarRolesPlugin.Logger.LogWarning($"{e}: tried to paste invalid settings!\n{allSettings}");
                string errorStr = allSettings.Length > 2 ? allSettings.Substring(0, 3) : "(empty clipboard) ";
                HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, $"Host Info: You tried to paste invalid settings: \"{errorStr}...\"");
                SoundEffectsManager.Load();
                SoundEffectsManager.Play("fail");
            }
            return Convert.ToInt32(vanillaOptionsFine) + ModdedOptionsFine;
        }
    }




    [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.ChangeTab))]
    class GameOptionsMenuChangeTabPatch
    {
        public static void Postfix(GameSettingMenu __instance, int tabNum, bool previewOnly)
        {
            if (previewOnly) return;
            foreach (var tab in GameOptionsMenuStartPatch.currentTabs)
            {
                if (tab != null)
                    tab.SetActive(false);
            }
            foreach (var pbutton in GameOptionsMenuStartPatch.currentButtons)
            {
                pbutton.SelectButton(false);
            }
            if (tabNum == 1)
            {
                var GameSettingsText = GameObject.Find("GameSettingsLabel");
                GameSettingsText.GetComponent<TextMeshPro>().text = "Vanilla Settings";
            }
            if (tabNum > 2)
            {
                tabNum -= 3;
                GameOptionsMenuStartPatch.currentTabs[tabNum].SetActive(true);
                GameOptionsMenuStartPatch.currentButtons[tabNum].SelectButton(true);
            }
        }
    }

    [HarmonyPatch(typeof(LobbyViewSettingsPane), nameof(LobbyViewSettingsPane.SetTab))]
    class LobbyViewSettingsPaneRefreshTabPatch
    {
        public static bool Prefix(LobbyViewSettingsPane __instance)
        {
            if ((int)__instance.currentTab < 15)
            {
                LobbyViewSettingsPaneChangeTabPatch.Postfix(__instance, __instance.currentTab);
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(LobbyViewSettingsPane), nameof(LobbyViewSettingsPane.ChangeTab))]
    class LobbyViewSettingsPaneChangeTabPatch
    {
        public static void Postfix(LobbyViewSettingsPane __instance, StringNames category)
        {
            int tabNum = (int)category;

            foreach (var pbutton in LobbyViewSettingsPatch.currentButtons)
            {
                pbutton.SelectButton(false);
            }
            if (tabNum > 20) // StringNames are in the range of 3000+ 
                return;
            __instance.taskTabButton.SelectButton(false);

            if (tabNum > 2)
            {
                tabNum -= 3;
                //GameOptionsMenuStartPatch.currentTabs[tabNum].SetActive(true);
                LobbyViewSettingsPatch.currentButtons[tabNum].SelectButton(true);
                LobbyViewSettingsPatch.drawTab(__instance, LobbyViewSettingsPatch.currentButtonTypes[tabNum]);
            }
        }
    }

    [HarmonyPatch(typeof(LobbyViewSettingsPane), nameof(LobbyViewSettingsPane.Update))]
    class LobbyViewSettingsPaneUpdatePatch
    {
        public static void Postfix(LobbyViewSettingsPane __instance)
        {
            if (LobbyViewSettingsPatch.currentButtons.Count == 0)
            {
                LobbyViewSettingsPatch.gameModeChangedFlag = true;
                LobbyViewSettingsPatch.Postfix(__instance);

            }
        }
    }


    [HarmonyPatch(typeof(LobbyViewSettingsPane), nameof(LobbyViewSettingsPane.Awake))]
    class LobbyViewSettingsPatch
    {
        public static List<PassiveButton> currentButtons = new();
        public static List<CustomOptionType> currentButtonTypes = new();
        public static bool gameModeChangedFlag = false;

        public static void createCustomButton(LobbyViewSettingsPane __instance, int targetMenu, string buttonName, string buttonText, CustomOptionType optionType)
        {
            buttonName = "View" + buttonName;
            var buttonTemplate = GameObject.Find("OverviewTab");
            var ModdedSettingsButton = GameObject.Find(buttonName);
            if (ModdedSettingsButton == null)
            {
                ModdedSettingsButton = GameObject.Instantiate(buttonTemplate, buttonTemplate.transform.parent);
                ModdedSettingsButton.transform.localPosition += Vector3.right * 1.3f * (targetMenu - 2);
                ModdedSettingsButton.name = buttonName;
                __instance.StartCoroutine(Effects.Lerp(2f, new Action<float>(p => { ModdedSettingsButton.transform.FindChild("FontPlacer").GetComponentInChildren<TextMeshPro>().text = buttonText; })));
                var ModdedSettingsPassiveButton = ModdedSettingsButton.GetComponent<PassiveButton>();
                ModdedSettingsPassiveButton.OnClick.RemoveAllListeners();
                ModdedSettingsPassiveButton.OnClick.AddListener((System.Action)(() => {
                    __instance.ChangeTab((StringNames)targetMenu);
                }));
                ModdedSettingsPassiveButton.OnMouseOut.RemoveAllListeners();
                ModdedSettingsPassiveButton.OnMouseOver.RemoveAllListeners();
                ModdedSettingsPassiveButton.SelectButton(false);
                currentButtons.Add(ModdedSettingsPassiveButton);
                currentButtonTypes.Add(optionType);
            }
        }

        public static void Postfix(LobbyViewSettingsPane __instance)
        {
            currentButtons.ForEach(x => x?.Destroy());
            currentButtons.Clear();
            currentButtonTypes.Clear();

            removeVanillaTabs(__instance);

            createSettingTabs(__instance);

        }

        public static void removeVanillaTabs(LobbyViewSettingsPane __instance)
        {
            GameObject.Find("RolesTabs")?.Destroy();
            var overview = GameObject.Find("OverviewTab");
            if (!gameModeChangedFlag)
            {
                overview.transform.localScale = new Vector3(0.375f * overview.transform.localScale.x, overview.transform.localScale.y, overview.transform.localScale.z);
                overview.transform.localPosition += new Vector3(-1.2f, 0f, 0f);

            }
            overview.transform.Find("FontPlacer").transform.localScale = new Vector3(1.5f, 1f, 1f);
            overview.transform.Find("FontPlacer").transform.localPosition = new Vector3(-0.6f, -0.1f, 0f);
            gameModeChangedFlag = false;
        }

        public static void drawTab(LobbyViewSettingsPane __instance, CustomOptionType optionType)
        {

            var relevantOptions = Options.Where(x => x.type == optionType).ToList();

            if ((int)optionType == 99)
            {
                // Create 4 Groups with Role settings only
                relevantOptions.Clear();
                relevantOptions.AddRange(Options.Where(x => x.type == CustomOptionType.Impostor && x.isHeader));
                relevantOptions.AddRange(Options.Where(x => x.type == CustomOptionType.Neutral && x.isHeader));
                relevantOptions.AddRange(Options.Where(x => x.type == CustomOptionType.NeutralK && x.isHeader));
                relevantOptions.AddRange(Options.Where(x => x.type == CustomOptionType.Crewmate && x.isHeader));
                relevantOptions.AddRange(Options.Where(x => x.type == CustomOptionType.Modifier && x.isHeader));
            }

            for (int j = 0; j < __instance.settingsInfo.Count; j++)
            {
                __instance.settingsInfo[j].gameObject.Destroy();
            }
            __instance.settingsInfo.Clear();

            float num = 1.44f;
            int i = 0;
            int singles = 0;
            int headers = 0;
            int lines = 0;
            var curType = CustomOptionType.Modifier;

            foreach (var option in relevantOptions)
            {
                if (option.isHeader && (int)optionType != 99 || (int)optionType == 99 && curType != option.type)
                {
                    curType = option.type;
                    if (i != 0) num -= 0.59f;
                    if (i % 2 != 0) singles++;
                    headers++; // for header
                    CategoryHeaderMasked categoryHeaderMasked = UnityEngine.Object.Instantiate<CategoryHeaderMasked>(__instance.categoryHeaderOrigin);
                    categoryHeaderMasked.SetHeader(StringNames.ImpostorsCategory, 61);
                    categoryHeaderMasked.Title.text = option.heading != "" ? option.heading : option.name;
                    if ((int)optionType == 99)
                        categoryHeaderMasked.Title.text = new Dictionary<CustomOptionType, string>() 
                        { 
                            { CustomOptionType.Impostor, "Impostor Roles" }, 
                            { CustomOptionType.Neutral, "Neutral Roles" },
                            { CustomOptionType.NeutralK, "NeutralK Roles" },
                            { CustomOptionType.Crewmate, "Crewmate Roles" }, 
                            { CustomOptionType.Modifier, "Modifiers" } }[curType];
                    categoryHeaderMasked.Title.outlineColor = Color.white;
                    categoryHeaderMasked.Title.outlineWidth = 0.2f;
                    categoryHeaderMasked.transform.SetParent(__instance.settingsContainer);
                    categoryHeaderMasked.transform.localScale = Vector3.one;
                    categoryHeaderMasked.transform.localPosition = new Vector3(-9.77f, num, -2f);
                    __instance.settingsInfo.Add(categoryHeaderMasked.gameObject);
                    num -= 0.85f;
                    i = 0;
                }

                ViewSettingsInfoPanel viewSettingsInfoPanel = UnityEngine.Object.Instantiate<ViewSettingsInfoPanel>(__instance.infoPanelOrigin);
                viewSettingsInfoPanel.transform.SetParent(__instance.settingsContainer);
                viewSettingsInfoPanel.transform.localScale = Vector3.one;
                float num2;
                if (i % 2 == 0)
                {
                    lines++;
                    num2 = -8.95f;
                    if (i > 0)
                    {
                        num -= 0.59f;
                    }
                }
                else
                {
                    num2 = -3f;
                }
                viewSettingsInfoPanel.transform.localPosition = new Vector3(num2, num, -2f);
                int value = option.GetSelection();
                viewSettingsInfoPanel.SetInfo(StringNames.ImpostorsCategory, option.Selections[value].ToString(), 61);
                viewSettingsInfoPanel.titleText.text = option.name;
                if (option.isHeader && (int)optionType != 99 && option.heading == "" && (option.type == CustomOptionType.Neutral || option.type == CustomOptionType.NeutralK || option.type == CustomOptionType.Crewmate || option.type == CustomOptionType.Impostor || option.type == CustomOptionType.Modifier))
                {
                    viewSettingsInfoPanel.titleText.text = "Spawn Chance";
                }
                if ((int)optionType == 99)
                {
                    viewSettingsInfoPanel.titleText.outlineColor = Color.white;
                    viewSettingsInfoPanel.titleText.outlineWidth = 0.2f;
                    if (option.type == CustomOptionType.Modifier)
                        viewSettingsInfoPanel.settingText.text = viewSettingsInfoPanel.settingText.text + GameOptionsDataPatch.buildModifierExtras(option);
                }
                __instance.settingsInfo.Add(viewSettingsInfoPanel.gameObject);

                i++;
            }
            float actual_spacing = (headers * 0.85f + lines * 0.59f) / (headers + lines);
            __instance.scrollBar.CalculateAndSetYBounds((float)(__instance.settingsInfo.Count + singles * 2 + headers), 2f, 6f, actual_spacing);

        }

        public static void createSettingTabs(LobbyViewSettingsPane __instance)
        {
            // Handle different gamemodes and tabs needed therein.
            int next = 3;
            // create SR settings
            createCustomButton(__instance, next++, "SRSettings", "SR Settings", CustomOptionType.General);
            // create Map settings
            createCustomButton(__instance, next++, "MapSettings", "Map Settings", CustomOptionType.Map);
            // create Role Overview
            createCustomButton(__instance, next++, "RoleOverview", "Role Overview", (CustomOptionType)99);
            // IMp
            createCustomButton(__instance, next++, "ImpostorSettings", "Impostor Roles", CustomOptionType.Impostor);
            // Neutral
            createCustomButton(__instance, next++, "NeutralSettings", "Neutral Roles", CustomOptionType.Neutral);
            // NeutralK
            createCustomButton(__instance, next++, "NeutralKSettings", "NeutralK Roles", CustomOptionType.NeutralK);
            // Crew
            createCustomButton(__instance, next++, "CrewmateSettings", "Crewmate Roles", CustomOptionType.Crewmate);
            // Modifier
            createCustomButton(__instance, next++, "ModifierSettings", "Modifiers", CustomOptionType.Modifier);
        }
    }

    [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.CreateSettings))]
    class GameOptionsMenuCreateSettingsPatch
    {
        public static void Postfix(GameOptionsMenu __instance)
        {
            if (__instance.gameObject.name == "GAME SETTINGS TAB")
                adaptTaskCount(__instance);
        }

        private static void adaptTaskCount(GameOptionsMenu __instance)
        {
            // Adapt task count for main options
            var commonTasksOption = __instance.Children.ToArray().FirstOrDefault(x => x.TryCast<NumberOption>()?.intOptionName == Int32OptionNames.NumCommonTasks).Cast<NumberOption>();
            if (commonTasksOption != null) commonTasksOption.ValidRange = new FloatRange(0f, 4f);
            var shortTasksOption = __instance.Children.ToArray().FirstOrDefault(x => x.TryCast<NumberOption>()?.intOptionName == Int32OptionNames.NumShortTasks).TryCast<NumberOption>();
            if (shortTasksOption != null) shortTasksOption.ValidRange = new FloatRange(0f, 23f);
            var longTasksOption = __instance.Children.ToArray().FirstOrDefault(x => x.TryCast<NumberOption>()?.intOptionName == Int32OptionNames.NumLongTasks).TryCast<NumberOption>();
            if (longTasksOption != null) longTasksOption.ValidRange = new FloatRange(0f, 15f);
        }
    }


    [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.Start))]
    class GameOptionsMenuStartPatch
    {
        public static List<GameObject> currentTabs = new();
        public static List<PassiveButton> currentButtons = new();
        public static Sprite _BackGroundSprite = null;
        public static Sprite _CopyInactiveSprite = null;
        public static Sprite _CopyActiveSprite = null;
        public static Sprite _PasteInactiveSprite = null;
        public static Sprite _PasteActiveSprite = null;
        public static Sprite _ResetInactiveSprite = null;
        public static Sprite _ResetActiveSprite = null;
        public static TMP_FontAsset _fontAsset = null;

        public static bool loaded = false;


        public static void Postfix(GameSettingMenu __instance)
        {
            currentTabs.ForEach(x => x?.Destroy());
            currentButtons.ForEach(x => x?.Destroy());
            currentTabs = new();
            currentButtons = new();

            if (GameOptionsManager.Instance.currentGameOptions.GameMode == GameModes.HideNSeek) return;
            var GameSettingsText = GameObject.Find("GameSettingsLabel");
            _fontAsset = GameSettingsText.GetComponent<TextMeshPro>().font;

            removeVanillaTabs(__instance);
            createSettingTabs(__instance);

            if (loaded == false)
            {
                _BackGroundSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.CopyPasteBG.png", 175f);
                _CopyInactiveSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.Copy.png", 100f);
                _CopyActiveSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.CopyActive.png", 100f);
                _PasteInactiveSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.Paste.png", 100f);
                _PasteActiveSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.PasteActive.png", 100f);
                _ResetInactiveSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.Reset.png", 100f);
                _ResetActiveSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.ResetActive.png", 100f);
                loaded = true;
            }

            var GOMGameObject = GameObject.Find("GameSettingsButton");
            GameSettingsText.transform.localPosition = new Vector3(-3.329f, 2.554f, -3f);
            __instance.StartCoroutine(Effects.Lerp(2f, new Action<float>(p => 
            {
                GOMGameObject.transform.FindChild("FontPlacer").GetComponentInChildren<TextMeshPro>().text = "Vanilla Settings";
                GameSettingsText.GetComponent<TextMeshPro>().text = "Vanilla Settings";
            })));

            createResetPresetButton(__instance);
            // create copy to clipboard and paste from clipboard buttons.
            var Template = GameObject.Find("PlayerOptionsMenu(Clone)").transform.Find("CloseButton").gameObject;
            var holderGO = new GameObject("copyPasteButtonParent");
            var bgrenderer = holderGO.AddComponent<SpriteRenderer>();
            bgrenderer.sprite = _BackGroundSprite;
            holderGO.transform.SetParent(Template.transform.parent, false);
            holderGO.transform.localPosition = new Vector3(-1.19f, 2.69f, -27f);
            holderGO.layer = Template.layer;
            holderGO.SetActive(true);

            createCopyButton(__instance, holderGO, Template);
            createPasteButton(__instance, holderGO, Template);
            CustomOptionDefaultSettings.CreatePresets();
        }
        private static void createCopyButton(GameSettingMenu __instance, GameObject holderGO, GameObject Template)
        {
            var copyButton = GameObject.Instantiate(Template, holderGO.transform);
            copyButton.transform.localPosition = new Vector3(-0.3f, 0.02f, -2f);
            var copyButtonPassive = copyButton.GetComponent<PassiveButton>();
            var copyButtonRenderer = copyButton.GetComponentInChildren<SpriteRenderer>();
            var copyButtonActiveRenderer = copyButton.transform.GetChild(1).GetComponent<SpriteRenderer>();
            copyButtonRenderer.sprite = _CopyInactiveSprite;
            copyButton.transform.GetChild(1).transform.localPosition = Vector3.zero;
            copyButtonActiveRenderer.sprite = _CopyActiveSprite;
            copyButtonPassive.OnClick.RemoveAllListeners();
            copyButtonPassive.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
            copyButtonPassive.OnClick.AddListener((System.Action)(() => {
                copyToClipboard();
                copyButtonRenderer.color = Color.green;
                copyButtonActiveRenderer.color = Color.green;
                __instance.StartCoroutine(Effects.Lerp(1f, new System.Action<float>((p) => {
                    if (p > 0.95)
                    {
                        copyButtonRenderer.color = Color.white;
                        copyButtonActiveRenderer.color = Color.white;
                    }
                })));
            }));
        }
        private static void createPasteButton(GameSettingMenu __instance, GameObject holderGO, GameObject Template)
        {
            var pasteButton = GameObject.Instantiate(Template, holderGO.transform);
            pasteButton.transform.localPosition = new Vector3(0.3f, 0.02f, -2f);
            var pasteButtonPassive = pasteButton.GetComponent<PassiveButton>();
            var pasteButtonRenderer = pasteButton.GetComponentInChildren<SpriteRenderer>();
            var pasteButtonActiveRenderer = pasteButton.transform.GetChild(1).GetComponent<SpriteRenderer>();
            pasteButtonRenderer.sprite = _PasteInactiveSprite;
            pasteButtonActiveRenderer.sprite = _PasteActiveSprite;
            pasteButtonPassive.OnClick.RemoveAllListeners();
            pasteButtonPassive.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
            pasteButtonPassive.OnClick.AddListener((System.Action)(() => {
                pasteButtonRenderer.color = Color.yellow;
                int success = pasteFromClipboard();
                pasteButtonRenderer.color = success == 3 ? Color.green : success == 0 ? Color.red : Color.yellow;
                pasteButtonActiveRenderer.color = success == 3 ? Color.green : success == 0 ? Color.red : Color.yellow;
                __instance.StartCoroutine(Effects.Lerp(1f, new System.Action<float>((p) => {
                    if (p > 0.95)
                    {
                        pasteButtonRenderer.color = Color.white;
                        pasteButtonActiveRenderer.color = Color.white;
                    }
                })));
            }));
        }

        private static void createResetPresetButton(GameSettingMenu __instance)
        {
            var closebutton = GameObject.Find("PlayerOptionsMenu(Clone)").transform.Find("CloseButton").gameObject;

            var template = HudManager.Instance.SettingsButton.transform;
            var ResetButton = UnityEngine.Object.Instantiate(template, closebutton.transform.parent);
            ResetButton.name = "ResetPreset Button";
            ResetButton.localPosition = new Vector3(-1.19f, 2.69f, -27f);
            ResetButton.GetComponent<AspectPosition>().DistanceFromEdge = new Vector3(3, 0.3f, -400f);
            var resetButtonPassive = ResetButton.GetComponent<PassiveButton>();
            var ActiveSprite = ResetButton.FindChild("Active").GetComponent<SpriteRenderer>();
            var InactiveSprite = ResetButton.FindChild("Inactive").GetComponent<SpriteRenderer>();
            //var BackgroundSprite = ResetButton.transform.FindChild("Background");

            InactiveSprite.sprite = _ResetInactiveSprite;
            ActiveSprite.sprite = _ResetActiveSprite;
            resetButtonPassive.OnClick.RemoveAllListeners();
            resetButtonPassive.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
            resetButtonPassive.OnClick.AddListener((System.Action)(() => {
                ResetPreset();
                InactiveSprite.color = Color.green;
                ActiveSprite.color = Color.green;
                __instance.StartCoroutine(Effects.Lerp(1f, new System.Action<float>((p) => {
                    if (p > 0.95)
                    {
                        InactiveSprite.color = Color.white;
                        ActiveSprite.color = Color.white;
                    }
                })));
            }));
        }

        private static void createSettings(GameOptionsMenu menu, List<CustomOption> options)
        {
            float num = 1.5f;
            foreach (CustomOption option in options)
            {
                if (option.isHeader)
                {
                    CategoryHeaderMasked categoryHeaderMasked = UnityEngine.Object.Instantiate<CategoryHeaderMasked>(menu.categoryHeaderOrigin, Vector3.zero, Quaternion.identity, menu.settingsContainer);
                    categoryHeaderMasked.Title.font = _fontAsset;
                    categoryHeaderMasked.SetHeader(StringNames.ImpostorsCategory, 20);
                    categoryHeaderMasked.transform.FindChild("HeaderText").GetComponent<RectTransform>().sizeDelta = new Vector2(3, 0.5f);
                    categoryHeaderMasked.Title.text = option.heading != "" ? option.heading : option.name;
                    categoryHeaderMasked.Title.outlineColor = Color.white;
                    categoryHeaderMasked.Title.outlineWidth = 0.035f;
                    categoryHeaderMasked.Title.fontSizeMax = 4;
                    categoryHeaderMasked.transform.localScale = new Vector3(0.63f ,0.63f);
                    categoryHeaderMasked.transform.localPosition = new Vector3(-0.903f, num, -2f);
                    categoryHeaderMasked.Background.sprite = GetHeaderSprite();
                    categoryHeaderMasked.Divider.sprite = GetDividerSprite();
                    categoryHeaderMasked.Background.transform.localPosition = new Vector3(-0.2446f, -0.1923f);
                    if (option.HeaderColor != default)
                    {
                        categoryHeaderMasked.Background.color = option.HeaderColor;
                        categoryHeaderMasked.Divider.color = option.HeaderColor;
                    }
                    else
                    {
                        categoryHeaderMasked.Background.color = DefaultColor;
                        categoryHeaderMasked.Divider.color = DefaultColor;
                    }
                    num -= 0.63f;
                }
                if (option.Selections[0].ToString() == "HeaderOnly")
                {
                    menu.scrollBar.SetYBoundsMax(-num - 1.65f);
                    continue;
                }
                OptionBehaviour optionBehaviour = UnityEngine.Object.Instantiate<StringOption>(menu.stringOptionOrigin, Vector3.zero, Quaternion.identity, menu.settingsContainer);
                optionBehaviour.transform.localPosition = new Vector3(0.952f, num, -2f);
                var backgroundlabel = optionBehaviour.transform.FindChild("LabelBackground");
                backgroundlabel.localScale = new Vector3(1.8f, 1f);
                backgroundlabel.localPosition = new Vector3(-2.16f, -0.0619f);
                optionBehaviour.SetClickMask(menu.ButtonClickMask);
                var stringOption = optionBehaviour as StringOption;
                var TitleText = stringOption.TitleText;
                TitleText.font = _fontAsset;


                // "SetUpFromData"
                SpriteRenderer[] componentsInChildren = optionBehaviour.GetComponentsInChildren<SpriteRenderer>(true);
                stringOption.OnValueChanged = new Action<OptionBehaviour>((o) => { });
                TitleText.text = option.name;
                TitleText.transform.localPosition += new Vector3(-2.7f, 0);
                var RectTrans = TitleText.transform.GetComponent<RectTransform>();
                RectTrans.sizeDelta = new Vector2(5, 0.5f);
                RectTrans.localScale = new Vector3(1.75f, 1.25f);


                if (option.isHeader && option.heading == "" && (option.type == CustomOptionType.Neutral || option.type == CustomOptionType.NeutralK || option.type == CustomOptionType.Crewmate || option.type == CustomOptionType.Impostor || option.type == CustomOptionType.Modifier))
                {
                    TitleText.text = "Spawn Chance";
                }
                if (TitleText.text.Length > 40)
                    TitleText.fontSize = 2.2f;
                if (TitleText.text.Length > 60)
                    TitleText.fontSize = 2f;
                stringOption.Value = stringOption.oldValue = option.Selection;
                stringOption.ValueText.text = option.Selections[option.Selection].ToString();
                stringOption.ValueText.font = _fontAsset;
                stringOption.ValueText.outlineWidth = 0.1f;
                stringOption.ValueText.outlineColor = Color.white;
                option.OptionBehaviour = stringOption;
                for (int i = 0; i < componentsInChildren.Length; i++)
                {
                    componentsInChildren[i].material.SetInt(PlayerMaterial.MaskLayer, 20);
                }
                foreach (TextMeshPro textMeshPro in optionBehaviour.GetComponentsInChildren<TextMeshPro>(true))
                {
                    textMeshPro.fontMaterial.SetFloat("_StencilComp", 3f);
                    textMeshPro.fontMaterial.SetFloat("_Stencil", (float)20);
                }


                menu.Children.Add(optionBehaviour);
                num -= 0.45f;
                menu.scrollBar.SetYBoundsMax(-num - 1.65f);
            }

            for (int i = 0; i < menu.Children.Count; i++)
            {
                OptionBehaviour optionBehaviour = menu.Children[i];
                if (AmongUsClient.Instance && !AmongUsClient.Instance.AmHost)
                {
                    optionBehaviour.SetAsPlayer();
                }
            }
        }

        private static void removeVanillaTabs(GameSettingMenu __instance)
        {
            GameObject.Find("What Is This?")?.Destroy();
            GameObject.Find("GamePresetButton")?.Destroy();
            GameObject.Find("RoleSettingsButton")?.Destroy();
            __instance.ChangeTab(1, false);
        }

        public static void createCustomButton(GameSettingMenu __instance, int targetMenu, string buttonName, string buttonText)
        {
            var leftPanel = GameObject.Find("LeftPanel");
            var buttonTemplate = GameObject.Find("GameSettingsButton");
            var GameSettingsText = GameObject.Find("GameSettingsLabel");
            if (targetMenu == 3)
            {
                buttonTemplate.transform.localPosition -= Vector3.up * 0.85f;
                buttonTemplate.transform.localScale *= Vector2.one * 0.75f;
            }
            var ModdedSettingsButton = GameObject.Find(buttonName);
            if (ModdedSettingsButton == null)
            {
                ModdedSettingsButton = GameObject.Instantiate(buttonTemplate, leftPanel.transform);
                ModdedSettingsButton.transform.localPosition += Vector3.up * 0.5f * (targetMenu - 2);
                ModdedSettingsButton.name = buttonName;
                __instance.StartCoroutine(Effects.Lerp(2f, new Action<float>(p => { ModdedSettingsButton.transform.FindChild("FontPlacer").GetComponentInChildren<TextMeshPro>().text = buttonText; })));
                var ModdedSettingsPassiveButton = ModdedSettingsButton.GetComponent<PassiveButton>();
                ModdedSettingsPassiveButton.OnClick.RemoveAllListeners();
                ModdedSettingsPassiveButton.OnClick.AddListener((System.Action)(() => {
                    __instance.ChangeTab(targetMenu, false);
                    GameSettingsText.GetComponent<TextMeshPro>().text = buttonText;
                }));
                ModdedSettingsPassiveButton.OnMouseOut.RemoveAllListeners();
                ModdedSettingsPassiveButton.OnMouseOver.RemoveAllListeners();
                ModdedSettingsPassiveButton.SelectButton(false);
                currentButtons.Add(ModdedSettingsPassiveButton);
            }
        }

        public static void createGameOptionsMenu(GameSettingMenu __instance, CustomOptionType optionType, string settingName)
        {
            var tabTemplate = GameObject.Find("GAME SETTINGS TAB");
            currentTabs.RemoveAll(x => x == null);

            var ModdedSettingsTab = GameObject.Instantiate(tabTemplate, tabTemplate.transform.parent);
            ModdedSettingsTab.name = settingName;

            var ModdedSettingsGOM = ModdedSettingsTab.GetComponent<GameOptionsMenu>();
            foreach (var child in ModdedSettingsGOM.Children)
            {
                child.Destroy();
            }
            ModdedSettingsGOM.scrollBar.transform.FindChild("SliderInner").DestroyChildren();
            ModdedSettingsGOM.Children.Clear();
            var relevantOptions = Options.Where(x => x.type == optionType).ToList();
            createSettings(ModdedSettingsGOM, relevantOptions);

            currentTabs.Add(ModdedSettingsTab);
            ModdedSettingsTab.SetActive(false);
        }

        private static void createSettingTabs(GameSettingMenu __instance)
        {
            // Handle different gamemodes and tabs needed therein.
            int next = 3;
            // Modifier
            createCustomButton(__instance, next++, "ModifierSettings", "Modifiers");
            createGameOptionsMenu(__instance, CustomOptionType.Modifier, "ModifierSettings");
            // Crew
            createCustomButton(__instance, next++, "CrewmateSettings", "Crewmate Roles");
            createGameOptionsMenu(__instance, CustomOptionType.Crewmate, "CrewmateSettings");
            // Neutral
            createCustomButton(__instance, next++, "NeutralKSettings", "Neutral Killing Roles");
            createGameOptionsMenu(__instance, CustomOptionType.NeutralK, "NeutralKSettings");
            // Neutral
            createCustomButton(__instance, next++, "NeutralSettings", "Neutral Roles");
            createGameOptionsMenu(__instance, CustomOptionType.Neutral, "NeutralSettings");
            // IMp
            createCustomButton(__instance, next++, "ImpostorSettings", "Impostor Roles");
            createGameOptionsMenu(__instance, CustomOptionType.Impostor, "ImpostorSettings");
            // Map
            createCustomButton(__instance, next++, "MapSettings", "Map Settings");
            createGameOptionsMenu(__instance, CustomOptionType.Map, "MapSettings");
            // create SR settings
            createCustomButton(__instance, next++, "SRSettings", "Stellar Settings");
            createGameOptionsMenu(__instance, CustomOptionType.General, "SRSettings");
        }
    }

    [HarmonyPatch(typeof(StringOption), nameof(StringOption.Initialize))]
    public class StringOptionEnablePatch
    {
        public static bool Prefix(StringOption __instance)
        {
            CustomOption option = CustomOption.Options.FirstOrDefault(option => option.OptionBehaviour == __instance);
            if (option == null) return true;

            __instance.OnValueChanged = new Action<OptionBehaviour>((o) => { });
            //__instance.TitleText.text = option.name;
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

    [HarmonyPatch(typeof(StringOption), nameof(StringOption.FixedUpdate))]
    public class StringOptionFixedUpdate
    {
        public static void Postfix(StringOption __instance)
        {
            if (!IL2CPPChainloader.Instance.Plugins.TryGetValue("com.DigiWorm.LevelImposter", out PluginInfo _)) return;
            CustomOption option = CustomOption.Options.FirstOrDefault(option => option.OptionBehaviour == __instance);
            if (option == null) return;
            if (GameOptionsManager.Instance.CurrentGameOptions.MapId == 6)
            {
                if (option.OptionBehaviour != null && option.OptionBehaviour is StringOption stringOption)
                {
                    stringOption.ValueText.text = option.Selections[option.Selection].ToString();
                }
                else if (option.OptionBehaviour != null && option.OptionBehaviour is StringOption stringOptionToo)
                {
                    stringOptionToo.oldValue = stringOptionToo.Value = option.Selection;
                    stringOptionToo.ValueText.text = option.Selections[option.Selection].ToString();
                }
            }
        }
    }


    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSyncSettings))]
    public class RpcSyncSettingsPatch
    {
        public static void Postfix()
        {
            //CustomOption.ShareOptionSelections();
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
                GameManager.Instance.LogicOptions.SyncOptions();
                CustomOption.ShareOptionSelections();
            }
        }
    }


    [HarmonyPatch]
    class GameOptionsDataPatch
    {
        private static string buildRoleOptions()
        {
            var impRoles = buildOptionsOfType(CustomOption.CustomOptionType.Impostor, true) + "\n";
            var neutralRoles = buildOptionsOfType(CustomOption.CustomOptionType.Neutral, true) + "\n";
            var nKRoles = buildOptionsOfType(CustomOption.CustomOptionType.NeutralK, true) + "\n";
            var crewRoles = buildOptionsOfType(CustomOption.CustomOptionType.Crewmate, true) + "\n";
            var modifiers = buildOptionsOfType(CustomOption.CustomOptionType.Modifier, true);
            return impRoles + neutralRoles + nKRoles + crewRoles + modifiers;
        }
        public static string buildModifierExtras(CustomOption customOption)
        {
            // find options children with quantity
            var children = CustomOption.Options.Where(o => o.Parent == customOption);
            var quantity = children.Where(o => o.name.Contains("Quantity")).ToList();
            if (customOption.GetSelection() == 0) return "";
            if (quantity.Count == 1) return $" ({quantity[0].GetQuantity()})";
            return "";
        }

        private static string buildOptionsOfType(CustomOption.CustomOptionType type, bool headerOnly)
        {
            StringBuilder sb = new StringBuilder("\n");
            var options = CustomOption.Options.Where(o => o.type == type);
            foreach (var option in options)
            {
                if (option.Parent == null)
                {
                    string line = $"{option.name}: {option.Selections[option.Selection].ToString()}";
                    if (type == CustomOption.CustomOptionType.Modifier) line += buildModifierExtras(option);
                    sb.AppendLine(line);
                }
            }
            if (headerOnly) return sb.ToString();
            else sb = new StringBuilder();

            foreach (CustomOption option in options)
            {
                if (option.Parent != null)
                {
                    bool isIrrelevant = option.Parent.GetSelection() == 0 || (option.Parent.Parent != null && option.Parent.Parent.GetSelection() == 0);

                    Color c = isIrrelevant ? Color.grey : Color.white;  // No use for now
                    if (isIrrelevant) continue;
                    sb.AppendLine(Helpers.ColorString(c, $"{option.name}: {option.Selections[option.Selection].ToString()}"));
                }
                else
                {
                    if (option == CustomOptionHolder.CrewmateRolesCountMin)
                    {
                        var optionName = CustomOptionHolder.cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "Crewmate Roles");
                        var min = CustomOptionHolder.CrewmateRolesCountMin.GetSelection();
                        var max = CustomOptionHolder.CrewmateRolesCountMax.GetSelection();
                        string optionValue = "";
                        if (min > max) min = max;
                        optionValue += (min == max) ? $"{max}" : $"{min} - {max}";
                        sb.AppendLine($"{optionName}: {optionValue}");
                    }
                    else if (option == CustomOptionHolder.NeutralRolesCountMin)
                    {
                        var optionName = CustomOptionHolder.cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "Neutral Roles");
                        var min = CustomOptionHolder.NeutralRolesCountMin.GetSelection();
                        var max = CustomOptionHolder.NeutralRolesCountMax.GetSelection();
                        if (min > max) min = max;
                        var optionValue = (min == max) ? $"{max}" : $"{min} - {max}";
                        sb.AppendLine($"{optionName}: {optionValue}");
                    }
                    else if (option == CustomOptionHolder.ImpostorRolesCountMin)
                    {
                        var optionName = CustomOptionHolder.cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "Impostor Roles");
                        var min = CustomOptionHolder.ImpostorRolesCountMin.GetSelection();
                        var max = CustomOptionHolder.ImpostorRolesCountMax.GetSelection();
                        if (max > GameOptionsManager.Instance.currentGameOptions.NumImpostors) max = GameOptionsManager.Instance.currentGameOptions.NumImpostors;
                        if (min > max) min = max;
                        var optionValue = (min == max) ? $"{max}" : $"{min} - {max}";
                        sb.AppendLine($"{optionName}: {optionValue}");
                    }

                    else if ((option == CustomOptionHolder.CrewmateRolesCountMax) || (option == CustomOptionHolder.NeutralRolesCountMax) || (option == CustomOptionHolder.ImpostorRolesCountMax))
                    {
                        continue;
                    }
                    else
                    {
                        sb.AppendLine($"\n{option.name}: {option.Selections[option.Selection].ToString()}");
                    }
                }
            }
            return sb.ToString();
        }


        public static int maxPage = 7;
        public static string buildAllOptions(string vanillaSettings = "", bool hideExtras = false)
        {
            if (vanillaSettings == "")
                vanillaSettings = GameOptionsManager.Instance.CurrentGameOptions.ToHudString(PlayerControl.AllPlayerControls.Count);
            int counter = StellarRolesPlugin.optionsPage;
            string hudString = counter != 0 && !hideExtras ? Helpers.ColorString(DateTime.Now.Second % 2 == 0 ? Color.white : Color.red, "(Use scroll wheel if necessary)\n\n") : "";

            maxPage = 7;
            switch (counter)
            {
                case 0:
                    hudString += (!hideExtras ? "" : "Page 1: Vanilla Settings \n\n") + vanillaSettings;
                    break;
                case 1:
                    hudString += "Page 2: StellarRoles Settings \n" + buildOptionsOfType(CustomOption.CustomOptionType.General, false);
                    break;
                case 2:
                    hudString += "Page 3: Role and Modifier Rates \n" + buildRoleOptions();
                    break;
                case 3:
                    hudString += "Page 4: Impostor Role Settings \n" + buildOptionsOfType(CustomOption.CustomOptionType.Impostor, false);
                    break;
                case 4:
                    hudString += "Page 5: Neutral Role Settings \n" + buildOptionsOfType(CustomOption.CustomOptionType.Neutral, false);
                    break;
                case 5:
                    hudString += "Page 6: Crewmate Role Settings \n" + buildOptionsOfType(CustomOption.CustomOptionType.Crewmate, false);
                    break;
                case 6:
                    hudString += "Page 7: Modifier Settings \n" + buildOptionsOfType(CustomOption.CustomOptionType.Modifier, false);
                    break;
            }

            if (!hideExtras || counter != 0) hudString += $"\n Press TAB or Page Number for more... ({counter + 1}/{maxPage})";
            return hudString;
        }


        [HarmonyPatch(typeof(IGameOptionsExtensions), nameof(IGameOptionsExtensions.ToHudString))]
        private static void Postfix(ref string __result)
        {
            if (GameOptionsManager.Instance.currentGameOptions.GameMode == AmongUs.GameOptions.GameModes.HideNSeek) return; // Allow Vanilla Hide N Seek
            __result = buildAllOptions(vanillaSettings: __result);
        }
    }

    [HarmonyPatch]
    public class AddToKillDistanceSetting
    {
        [HarmonyPatch(typeof(GameOptionsData), nameof(GameOptionsData.AreInvalid))]
        [HarmonyPrefix]

        public static bool Prefix(GameOptionsData __instance, ref int maxExpectedPlayers, ref bool __result)
        {
            //making the killdistances bound check higher since extra short is added
            if (__instance.MaxPlayers > maxExpectedPlayers || __instance.NumImpostors < 1
                    || __instance.NumImpostors > 3 || __instance.KillDistance < 0
                    || __instance.KillDistance >= GameOptionsData.KillDistances.Count
                    || __instance.PlayerSpeedMod <= 0f || __instance.PlayerSpeedMod > 3f)
            {
                __result = true;
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(NormalGameOptionsV07), nameof(NormalGameOptionsV07.AreInvalid))]
        [HarmonyPrefix]

        public static bool Prefix(NormalGameOptionsV07 __instance, ref int maxExpectedPlayers, ref bool __result)
        {
            if (__instance.MaxPlayers > maxExpectedPlayers || __instance.NumImpostors < 1
                    || __instance.NumImpostors > 3 || __instance.KillDistance < 0
                    || __instance.KillDistance >= GameOptionsData.KillDistances.Count
                    || __instance.PlayerSpeedMod <= 0f || __instance.PlayerSpeedMod > 3f)
            {
                __result=true;
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(NormalGameOptionsV08), nameof(NormalGameOptionsV08.AreInvalid))]
        [HarmonyPrefix]

        public static bool Prefix(NormalGameOptionsV08 __instance, ref int maxExpectedPlayers, ref bool __result)
        {
            if (__instance.MaxPlayers > maxExpectedPlayers || __instance.NumImpostors < 1
                    || __instance.NumImpostors > 3 || __instance.KillDistance < 0
                    || __instance.KillDistance >= GameOptionsData.KillDistances.Count
                    || __instance.PlayerSpeedMod <= 0f || __instance.PlayerSpeedMod > 3f)
            {
                __result = true;
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(StringOption), nameof(StringOption.Initialize))]
        [HarmonyPrefix]

        public static void Prefix(StringOption __instance)
        {
            if (__instance.Title == StringNames.GameKillDistance)
            {
                if (__instance.Values.Count == 3)
                {
                    __instance.Values = new([(StringNames)49999, StringNames.SettingShort, StringNames.SettingMedium, StringNames.SettingLong]);
                }
            }
        }

        [HarmonyPatch(typeof(OptionBehaviour), nameof(OptionBehaviour.GetValueString))]
        [HarmonyPrefix]

        public static bool OptionBPrefix(OptionBehaviour __instance, ref float value, ref string __result)
        {
            if (__instance.Title == StringNames.GameKillDistance && Helpers.IsNormal)
            {
                var index = GameOptionsManager.Instance.currentNormalGameOptions.KillDistance;
                var stringname = GameOptionsData.KillDistanceStrings[index];
                __result = stringname;
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(StringGameSetting), nameof(StringGameSetting.GetValueString))]
        [HarmonyPrefix]

        public static bool StringOPrefix(StringGameSetting __instance, ref float value, ref string __result)
        {
            if (__instance.Title == StringNames.GameKillDistance && Helpers.IsNormal)
            {
                var index = GameOptionsManager.Instance.currentNormalGameOptions.KillDistance;
                var stringname = GameOptionsData.KillDistanceStrings[index];
                __result = stringname;
                return false;
            }
            return true;
        }


        [HarmonyPatch(typeof(IGameOptionsExtensions), nameof(IGameOptionsExtensions.AppendItem),
            new Type[] { typeof(Il2CppSystem.Text.StringBuilder), typeof(StringNames), typeof(string) })]
        [HarmonyPrefix]

        public static void Prefix(ref StringNames stringName, ref string value)
        {
            if (stringName == StringNames.GameKillDistance)
            {
                int index;
                if (GameOptionsManager.Instance.currentGameMode == GameModes.Normal)
                {
                    index = GameOptionsManager.Instance.currentNormalGameOptions.KillDistance;
                }
                else
                {
                    index = GameOptionsManager.Instance.currentHideNSeekGameOptions.KillDistance;
                }
                value = GameOptionsData.KillDistanceStrings[index];
            }
        }


        [HarmonyPatch(typeof(TranslationController), nameof(TranslationController.GetString), new[] {
                typeof(StringNames),
                typeof(Il2CppReferenceArray<Il2CppSystem.Object>)
            })]
        [HarmonyPriority(Priority.Last)]
        [HarmonyPrefix]

        public static bool VeryShortPrefix(ref string __result, [HarmonyArgument(0)] StringNames id)
        {
            if ((int)id == 49999)
            {
                __result = "Very Short";
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Awake))]
    public static class SetKillDistancePatches
    {
        public static void Prefix()
        {
            addKillDistance();
        }
        public static void addKillDistance()
        {
            GameOptionsData.KillDistances = new([0.6f, 1f, 1.8f, 2.5f]);
            GameOptionsData.KillDistanceStrings = new(["Very Short", "Short", "Medium", "Long"]);
        }
    }

    [HarmonyPatch(typeof(KeyboardJoystick), nameof(KeyboardJoystick.Update))]
    public static class GameOptionsNextPagePatch
    {
        public static void Postfix(KeyboardJoystick __instance)
        {
            int page = StellarRolesPlugin.optionsPage;
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                StellarRolesPlugin.optionsPage = (StellarRolesPlugin.optionsPage + 1) % 7;
            }
            if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
            {
                StellarRolesPlugin.optionsPage = 0;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
            {
                StellarRolesPlugin.optionsPage = 1;
            }
            if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
            {
                StellarRolesPlugin.optionsPage = 2;
            }
            if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
            {
                StellarRolesPlugin.optionsPage = 3;
            }
            if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5))
            {
                StellarRolesPlugin.optionsPage = 4;
            }
            if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6))
            {
                StellarRolesPlugin.optionsPage = 5;
            }
            if (Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Keypad7))
            {
                StellarRolesPlugin.optionsPage = 6;
            }
            if (StellarRolesPlugin.optionsPage >= GameOptionsDataPatch.maxPage) StellarRolesPlugin.optionsPage = 0;
        }
    }


    /*//This class is taken and adapted from Town of Us Reactivated, https://github.com/eDonnes124/Town-Of-Us-R/blob/master/source/Patches/CustomOption/Patches.cs, Licensed under GPLv3
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate
    {
        private static GameObject GameSettingsObject;
        private static TextMeshPro GameSettings;
        public static float
            MinX,*//*-5.3F*//*
            OriginalY = 2.9F,
            MinY = 2.9F;

        public static Scroller Scroller;
        private static Vector3 LastPosition;
        private static float lastAspect;
        private static bool setLastPosition = false;
        private static TMPro.TextMeshPro[] settingsTMPs = new TMPro.TextMeshPro[4];
        private static GameObject settingsBackground;

        public static void Prefix(HudManager __instance)
        {
            if (GameSettings?.transform == null) return;

            // Sets the MinX position to the left edge of the screen + 0.1 units
            Rect safeArea = Screen.safeArea;
            float aspect = Mathf.Min((Camera.main).aspect, safeArea.width / safeArea.height);
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

            Scroller.gameObject.SetActive(GameSettings.gameObject.activeSelf);

            if (!Scroller.gameObject.active) return;

            var rows = GameSettings.text.Count(c => c == '\n');
            float LobbyTextRowHeight = 0.06F;
            var maxY = Mathf.Max(MinY, rows * LobbyTextRowHeight + (rows - 38) * LobbyTextRowHeight);

            Scroller.ContentYBounds = new FloatRange(MinY, maxY);

            // Prevent scrolling when the player is interacting with a menu
            if (PlayerControl.LocalPlayer?.CanMove != true)
            {
                GameSettings.transform.localPosition = LastPosition;

                return;
            }

            if (GameSettings.transform.localPosition.x != MinX ||
                GameSettings.transform.localPosition.y < MinY) return;

            LastPosition = GameSettings.transform.localPosition;
        }

        private static void CreateScroller(HudManager __instance)
        {
            if (Scroller != null) return;

            Transform target = GameSettings.transform;

            Scroller = new GameObject("SettingsScroller").AddComponent<Scroller>();
            Scroller.transform.SetParent(GameSettings.transform.parent);
            Scroller.gameObject.layer = 5;

            Scroller.transform.localScale = Vector3.one;
            Scroller.allowX = false;
            Scroller.allowY = true;
            Scroller.active = true;
            Scroller.velocity = new Vector2(0, 0);
            Scroller.ScrollbarYBounds = new FloatRange(0, 0);
            Scroller.ContentXBounds = new FloatRange(MinX, MinX);
            Scroller.enabled = true;

            Scroller.Inner = target;
            target.SetParent(Scroller.transform);
        }

        [HarmonyPrefix]
        public static void Prefix2(HudManager __instance)
        {
            if (!settingsTMPs[0]) return;
            foreach (var tmp in settingsTMPs) tmp.text = "";
            var settingsString = GameOptionsDataPatch.buildAllOptions(hideExtras: true);
            var blocks = settingsString.Split("\n\n", StringSplitOptions.RemoveEmptyEntries); ;
            string curString = "";
            string curBlock;
            int j = 0;
            for (int i = 0; i < blocks.Length; i++)
            {
                curBlock = blocks[i];
                if (Helpers.lineCount(curBlock) + Helpers.lineCount(curString) < 43)
                {
                    curString += curBlock + "\n\n";
                }
                else
                {
                    settingsTMPs[j].text = curString;
                    j++;

                    curString = "\n" + curBlock + "\n\n";
                    if (curString.Substring(0, 2) != "\n\n") curString = "\n" + curString;
                }
            }
            if (j < settingsTMPs.Length) settingsTMPs[j].text = curString;
            int blockCount = 0;
            foreach (var tmp in settingsTMPs)
            {
                if (tmp.text != "")
                    blockCount++;
            }
            for (int i = 0; i < blockCount; i++)
            {
                settingsTMPs[i].transform.localPosition = new Vector3(-blockCount * 1.2f + 2.7f * i, 2.2f, -500f);
            }
        }
    }*/
}