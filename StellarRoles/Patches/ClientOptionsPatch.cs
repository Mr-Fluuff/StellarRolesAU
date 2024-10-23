using HarmonyLib;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.UI.Button;
using Object = UnityEngine.Object;

namespace StellarRoles.Patches
{
    [HarmonyPatch]
    public static class ClientOptionsPatch
    {
        private static SelectionBehaviour[] AllOptions = {
            new SelectionBehaviour("Ghosts Can See Votes", () => MapOptions.GhostsSeeVotes = StellarRolesPlugin.GhostsSeeVotes.Value = !StellarRolesPlugin.GhostsSeeVotes.Value, StellarRolesPlugin.GhostsSeeVotes.Value),
            new SelectionBehaviour("Show Role Summary", () => MapOptions.ShowRoleSummary = StellarRolesPlugin.ShowRoleSummary.Value = !StellarRolesPlugin.ShowRoleSummary.Value, StellarRolesPlugin.ShowRoleSummary.Value),
            new SelectionBehaviour("Enable Sound Effects", () => MapOptions.EnableSoundEffects = StellarRolesPlugin.EnableSoundEffects.Value = !StellarRolesPlugin.EnableSoundEffects.Value, StellarRolesPlugin.EnableSoundEffects.Value),
            new SelectionBehaviour("Hide Pet From Others", () => MapOptions.HidePetFromOthers = StellarRolesPlugin.HidePetFromOthers.Value = !StellarRolesPlugin.HidePetFromOthers.Value, StellarRolesPlugin.HidePetFromOthers.Value),
            //new SelectionBehaviour("Debug Mode", () => StellarRolesPlugin.DebugMode.Value = !StellarRolesPlugin.DebugMode.Value, StellarRolesPlugin.DebugMode.Value),
        };

        private static GameObject popUp;
        private static TextMeshPro titleText;

        private static ToggleButtonBehaviour buttonPrefab;
        private static Vector3? _origin;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
        public static void MainMenuManager_StartPostfix(MainMenuManager __instance)
        {
            // Prefab for the title
            GameObject go = new("TitleTextSR");
            TextMeshPro tmp = go.AddComponent<TextMeshPro>();
            tmp.fontSize = 4;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.transform.localPosition += Vector3.left * 0.2f;
            titleText = Object.Instantiate(tmp);
            titleText.gameObject.SetActive(false);
            Object.DontDestroyOnLoad(titleText);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(OptionsMenuBehaviour), nameof(OptionsMenuBehaviour.Start))]
        public static void OptionsMenuBehaviour_StartPostfix(OptionsMenuBehaviour __instance)
        {
            if (!__instance.CensorChatButton) return;

            if (!popUp)
            {
                CreateCustom(__instance);
            }

            if (!buttonPrefab)
            {
                buttonPrefab = Object.Instantiate(__instance.CensorChatButton);
                Object.DontDestroyOnLoad(buttonPrefab);
                buttonPrefab.name = "CensorChatPrefab";
                buttonPrefab.gameObject.SetActive(false);
            }

            SetUpOptions();
            InitializeMoreButton(__instance);
        }

        private static void CreateCustom(OptionsMenuBehaviour prefab)
        {
            popUp = Object.Instantiate(prefab.gameObject);
            Object.DontDestroyOnLoad(popUp);
            Transform transform = popUp.transform;
            Vector3 pos = transform.localPosition;
            pos.z = -920f;
            transform.localPosition = pos;

            Object.Destroy(popUp.GetComponent<OptionsMenuBehaviour>());
            foreach (GameObject gObj in popUp.gameObject.GetAllChilds())
            {
                if (gObj.name != "Background" && gObj.name != "CloseButton")
                    Object.Destroy(gObj);
            }

            popUp.SetActive(false);
        }

        private static void InitializeMoreButton(OptionsMenuBehaviour __instance)
        {
            ToggleButtonBehaviour moreOptions = Object.Instantiate(buttonPrefab, __instance.CensorChatButton.transform.parent);
            Transform transform = __instance.CensorChatButton.transform;
            __instance.CensorChatButton.Text.transform.localScale = new Vector3(1 / 0.66f, 1, 1);
            _origin ??= transform.localPosition;

            transform.localPosition = _origin.Value + Vector3.left * 0.45f;
            transform.localScale = new Vector3(0.66f, 1, 1);
            __instance.EnableFriendInvitesButton.transform.localScale = new Vector3(0.66f, 1, 1);
            __instance.EnableFriendInvitesButton.transform.localPosition += Vector3.right * 0.5f;
            __instance.EnableFriendInvitesButton.Text.transform.localScale = new Vector3(1.2f, 1, 1);

            moreOptions.transform.localPosition = _origin.Value + Vector3.right * 4f / 3f;
            moreOptions.transform.localScale = new Vector3(0.66f, 1, 1);

            moreOptions.gameObject.SetActive(true);
            moreOptions.Text.text = "Mod Options...";
            moreOptions.Text.transform.localScale = new Vector3(1 / 0.66f, 1, 1);
            PassiveButton moreOptionsButton = moreOptions.GetComponent<PassiveButton>();
            moreOptionsButton.OnClick = new ButtonClickedEvent();
            moreOptionsButton.OnClick.AddListener((Action)(() =>
            {
                if (!popUp) return;

                if (__instance.transform.parent && __instance.transform.parent == HudManager.Instance.transform)
                {
                    popUp.transform.SetParent(HudManager.Instance.transform);
                    popUp.transform.localPosition = new Vector3(0, 0, -920f);
                }
                else
                {
                    popUp.transform.SetParent(null);
                    Object.DontDestroyOnLoad(popUp);
                }

                CheckSetTitle();
                RefreshOpen();
            }));
        }

        private static void RefreshOpen()
        {
            popUp.gameObject.SetActive(false);
            popUp.gameObject.SetActive(true);
            SetUpOptions();
        }

        private static void CheckSetTitle()
        {
            if (!popUp || popUp.GetComponentInChildren<TextMeshPro>() || !titleText) return;

            TextMeshPro title = Object.Instantiate(titleText, popUp.transform);
            title.GetComponent<RectTransform>().localPosition = Vector3.up * 2.3f;
            title.gameObject.SetActive(true);
            title.text = "More Options...";
            title.name = "TitleText";
        }

        private static void SetUpOptions()
        {
            if (popUp.transform.GetComponentInChildren<ToggleButtonBehaviour>()) return;

            for (int i = 0; i < AllOptions.Length; i++)
            {
                SelectionBehaviour info = AllOptions[i];

                ToggleButtonBehaviour button = Object.Instantiate(buttonPrefab, popUp.transform);
                Vector3 pos = new(i % 2 == 0 ? -1.17f : 1.17f, 1.3f - i / 2 * 0.8f, -.5f);

                Transform transform = button.transform;
                transform.localPosition = pos;

                button.onState = info.DefaultValue;
                button.Background.color = button.onState ? Color.green : Palette.ImpostorRed;

                button.Text.text = info.Title;
                button.Text.fontSizeMin = button.Text.fontSizeMax = 1.8f;
                button.Text.font = Object.Instantiate(titleText.font);
                button.Text.GetComponent<RectTransform>().sizeDelta = new Vector2(2, 2);

                button.name = info.Title.Replace(" ", "") + "Toggle";
                button.gameObject.SetActive(true);

                PassiveButton passiveButton = button.GetComponent<PassiveButton>();
                BoxCollider2D colliderButton = button.GetComponent<BoxCollider2D>();

                colliderButton.size = new Vector2(2.2f, .7f);

                passiveButton.OnClick = new ButtonClickedEvent();
                passiveButton.OnMouseOut = new UnityEvent();
                passiveButton.OnMouseOver = new UnityEvent();

                passiveButton.OnClick.AddListener((Action)(() =>
                {
                    button.onState = info.OnClick();
                    button.Background.color = button.onState ? Color.green : Palette.ImpostorRed;
                }));

                passiveButton.OnMouseOver.AddListener((Action)(() => button.Background.color = new Color32(34, 139, 34, byte.MaxValue)));
                passiveButton.OnMouseOut.AddListener((Action)(() => button.Background.color = button.onState ? Color.green : Palette.ImpostorRed));

                foreach (SpriteRenderer spr in button.gameObject.GetComponentsInChildren<SpriteRenderer>())
                    spr.size = new Vector2(2.2f, .7f);
            }
        }

        private static IEnumerable<GameObject> GetAllChilds(this GameObject Go)
        {
            for (int i = 0; i < Go.transform.childCount; i++)
            {
                yield return Go.transform.GetChild(i).gameObject;
            }
        }

        public class SelectionBehaviour
        {
            public string Title;
            public Func<bool> OnClick;
            public bool DefaultValue;

            public SelectionBehaviour(string title, Func<bool> onClick, bool defaultValue)
            {
                Title = title;
                OnClick = onClick;
                DefaultValue = defaultValue;
            }
        }
    }
}
