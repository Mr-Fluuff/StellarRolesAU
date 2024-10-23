using HarmonyLib;
using Reactor.Utilities.Extensions;
using Steamworks;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace StellarRoles.Patches
{
    public static class HelpMenu
    {
        public static GameObject RolesUI { get; set; }
        public static readonly List<string> Factions = new() { "Impostors", "Crewmates", "Neutrals", "Neutral Killers", "Modifiers" };
        private static TextMeshPro infoButtonText;
        private static Sprite _BlankPlateSprite;
        private static Sprite _HelpButtonSprite;
        private static Sprite _HelpButtonCloseSprite;
        public static GameObject Reference = null;

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        [HarmonyPriority(Priority.Last)]
        public static class HelpButtonHudUpdate
        {
            public static void Postfix()
            {
                if (LobbyBehaviour.Instance || Helpers.TutorialActive)
                {
                    try
                    {
                        HudManagerStartPatch.HelpButton.Update();
                        HudManagerStartPatch.ImpChatButton.Update();
                        HudManagerStartPatch.HistoryButton.Update();
                    }
                    catch { }
                }
                if (Reference == null)
                {
                    Reference = new GameObject("ButtonReference");
                    Reference.transform.localPosition = new Vector3(0f, 0f);
                }
            }
        }

        public static Sprite GetPlateSprite()
        {
            return _BlankPlateSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.BlankPlate.png", 100f);
        }
        public static Sprite GetHelpButtonSprite()
        {
            return _HelpButtonSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.HelpMenu.HelpButton.png", 150f);
        }
        public static Sprite GetHelpButtonCloseSprite()
        {
            return _HelpButtonCloseSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.HelpMenu.HelpButtonExit.png", 150f);
        }

        public static void RoleFactionOnClick()
        {
            if (RolesUI != null) return;

            SpriteRenderer container = new GameObject("RoleFactionMenuContainer").AddComponent<SpriteRenderer>();
            container.sprite = HelpInfo.getFactionBackground();
            container.transform.SetParent(HudManager.Instance.transform);
            container.gameObject.transform.SetLocalZ(-200);
            container.transform.localPosition = new Vector3(0, 0, -20f);
            container.transform.localScale = new Vector3(.75f, .7f, 1f);
            container.gameObject.layer = 5;

            RolesUI = container.gameObject;

            var buttonTemplate = HudManager.Instance.SettingsButton.transform;
            TextMeshPro textTemplate = HudManager.Instance.TaskPanel.taskText;

            TextMeshPro newtitle = Object.Instantiate(textTemplate, container.transform);
            newtitle.text = "Role Info Menu";
            newtitle.color = Color.white;
            newtitle.outlineWidth = .25f;
            newtitle.transform.localPosition = new Vector3(1f, -.25f, -2f);
            newtitle.transform.localScale = Vector3.one * 2.5f;

            List<Transform> buttons = new();

            for (int i = 0; i < Factions.Count; i++)
            {
                string faction = "";
                Faction factionid = Faction.Other;
                switch (Factions[i])
                {
                    case "Impostors":
                        faction = Helpers.ColorString(Palette.ImpostorRed, "Impostors");
                        factionid = Faction.Impostor;
                        break;
                    case "Crewmates":
                        faction = Helpers.ColorString(Palette.CrewmateBlue, "Crewmates");
                        factionid = Faction.Crewmate;
                        break;
                    case "Neutrals":
                        faction = Helpers.ColorString(Color.gray, "Neutrals");
                        factionid = Faction.Neutral;
                        break;
                    case "Neutral Killers":
                        faction = Helpers.ColorString(NeutralKiller.Color, "Neutral Killers");
                        factionid = Faction.NK;
                        break;
                    case "Modifiers":
                        faction = Helpers.ColorString(Spiteful.Color, "Modifiers");
                        factionid = Faction.Modifier;
                        break;
                }

                var RoleButton = Object.Instantiate(buttonTemplate, container.transform);
                RoleButton.GetComponent<AspectPosition>().Destroy();
                RoleButton.transform.position = Vector3.zero;
                RoleButton.name = faction + " Button";
                RoleButton.GetComponent<BoxCollider2D>().size = new Vector2(2.5f, 0.55f);
                var ActiveSprite = RoleButton.FindChild("Active").GetComponent<SpriteRenderer>();
                var InactiveSprite = RoleButton.FindChild("Inactive").GetComponent<SpriteRenderer>();
                RoleButton.FindChild("Background").gameObject.active = false;
                ActiveSprite.sprite = GetPlateSprite();
                InactiveSprite.sprite = GetPlateSprite();
                ActiveSprite.color = Color.green;
                buttons.Add(RoleButton.transform);
                RoleButton.transform.localPosition = new Vector3(0, 1.8f - i * 1f, -10);
                RoleButton.transform.localScale = new Vector3(2f, 1.5f, 1f);

                TextMeshPro ButtonLabel = Object.Instantiate(textTemplate, RoleButton.transform);
                ButtonLabel.text = faction;
                ButtonLabel.alignment = TextAlignmentOptions.Center;
                ButtonLabel.transform.localPosition = new Vector3(0, 0, ButtonLabel.transform.localPosition.z);
                ButtonLabel.transform.localScale = new Vector3(1.6f, 2.3f, 1f);

                PassiveButton button = RoleButton.GetComponent<PassiveButton>();

                button.OnClick.RemoveAllListeners();
                button.OnClick = new Button.ButtonClickedEvent();
                button.OnClick.AddListener((Action)(() =>
                {
                    Object.Destroy(container.gameObject);
                    roleInfosOnclick(faction, factionid);
                }));
            }
        }

        public static void roleInfosOnclick(string faction, Faction factionId)
        {
            List<RoleId> NKIds = new() { RoleId.HeadHunter, RoleId.Pyromaniac, RoleId.Nightmare, RoleId.RuthlessRomantic, RoleId.RogueImpostor };
            SpriteRenderer container = new GameObject("RoleListMenuContainer").AddComponent<SpriteRenderer>();
            container.sprite = HelpInfo.getMenuBackground();
            container.transform.SetParent(HudManager.Instance.transform);
            container.transform.localPosition = new Vector3(0, 0f, -20f);
            container.transform.localScale = new Vector3(.7f, .7f, 1f);
            container.gameObject.layer = 5;
            RolesUI = container.gameObject;

            Transform buttonTemplate = HudManager.Instance.SettingsButton.transform;
            TextMeshPro textTemplate = HudManager.Instance.TaskPanel.taskText;

            TextMeshPro newtitle = Object.Instantiate(textTemplate, container.transform);
            newtitle.text = faction;
            newtitle.outlineWidth = .25f;
            newtitle.transform.localPosition = new Vector3(0f, 2.7f, -2f);
            newtitle.transform.localScale = Vector3.one * 2.5f;

            List<Transform> buttons = new();
            int count = 0;
            bool gameStarted = Helpers.GameStarted;
            foreach (RoleInfo roleInfo in RoleInfo.AllRoleInfos)
            {
                if (roleInfo.FactionId != factionId) continue;
                if (!gameStarted && factionId == Faction.NK && !NKIds.Contains(roleInfo.RoleId)) continue;
                if (gameStarted && Helpers.GetNotActiveRoles(roleInfo)) continue;

                var RoleButton = Object.Instantiate(buttonTemplate, container.transform);
                RoleButton.GetComponent<AspectPosition>().Destroy();
                RoleButton.transform.position = Vector3.zero;
                RoleButton.name = Helpers.ColorString(roleInfo.Color, roleInfo.Name) + " Button";
                RoleButton.GetComponent<BoxCollider2D>().size = new Vector2(2.5f, 0.55f);

                var ActiveSprite = RoleButton.FindChild("Active").GetComponent<SpriteRenderer>();
                var InactiveSprite = RoleButton.FindChild("Inactive").GetComponent<SpriteRenderer>();
                RoleButton.FindChild("Background").gameObject.active = false;
                ActiveSprite.sprite = GetPlateSprite();
                InactiveSprite.sprite = GetPlateSprite();
                ActiveSprite.color = Color.green;

                int row = count / 3, col = count % 3;
                RoleButton.transform.localPosition = new Vector3(-3.205f + col * 3.2f, 2.4f - row * 0.75f, -5);
                RoleButton.transform.localScale = new Vector3(1.125f, 1.125f, 1f);

                buttons.Add(RoleButton);

                TextMeshPro ButtonLabel = Object.Instantiate(textTemplate, RoleButton.transform);
                ButtonLabel.text = Helpers.ColorString(roleInfo.Color, roleInfo.Name);
                ButtonLabel.alignment = TextAlignmentOptions.Center;
                ButtonLabel.transform.localPosition = new Vector3(0, 0, ButtonLabel.transform.localPosition.z);
                ButtonLabel.transform.localScale *= 1.55f;

                PassiveButton button = RoleButton.GetComponent<PassiveButton>();
                button.OnClick.RemoveAllListeners();
                Button.ButtonClickedEvent onClick = button.OnClick = new Button.ButtonClickedEvent();
                onClick.AddListener((Action)(() =>
                {
                    Object.Destroy(container.gameObject);
                    AddInfoCard(roleInfo);
                }));
                count++;
            }
        }
        static void AddInfoCard(RoleInfo roleInfo)
        {
            string roleSettingDescription = HelpInfo.helpText(roleInfo);
            string coloredHelp = Helpers.ReplaceWithColors(roleSettingDescription);

            GameObject roleCardBackground = Object.Instantiate(new GameObject("RoleCardBackground"), HudManager.Instance.transform);
            SpriteRenderer roleCardBackgroundRend = roleCardBackground.AddComponent<SpriteRenderer>();
            roleCardBackgroundRend.sprite = HelpInfo.getHelpMenuBackground();
            roleCardBackground.layer = 5;
            roleCardBackground.transform.localPosition = new Vector3(0f, 0f, -20f);
            roleCardBackground.transform.localScale = new Vector3(0.68f, 0.68f, 1f);

            RolesUI = roleCardBackground.gameObject;

            GameObject roleCard = Object.Instantiate(new GameObject("RoleCard"), roleCardBackground.transform);
            SpriteRenderer roleCardRend = roleCard.AddComponent<SpriteRenderer>();
            roleCard.layer = 5;
            roleCard.transform.localPosition = new Vector3(0, 0, -1);
            roleCardRend.sprite = HelpInfo.helpButtonGetSprite(roleInfo.RoleId);

            infoButtonText = Object.Instantiate(HudManager.Instance.TaskPanel.taskText, roleCard.transform);
            infoButtonText.color = Color.white;
            infoButtonText.text = coloredHelp;
            infoButtonText.enableWordWrapping = false;
            infoButtonText.transform.localScale = Vector3.one * 1.25f;
            infoButtonText.transform.localPosition = new Vector3(-3f, 0f, -50f);
            infoButtonText.alignment = TextAlignmentOptions.TopLeft;
            infoButtonText.fontStyle = FontStyles.Bold;
        }
    }
}
