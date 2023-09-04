using HarmonyLib;
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

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        public static class HelpButtonHudUpdate
        {
            public static void Postfix()
            {
                if (!LobbyBehaviour.Instance || Helpers.GameStarted) return;
                try
                {
                    HudManagerStartPatch.HelpButton.Update();
                }
                catch { }
            }
        }

        public static void RoleFactionOnClick()
        {
            if (RolesUI != null) return;

            SpriteRenderer container = new GameObject("RoleFactionMenuContainer").AddComponent<SpriteRenderer>();
            container.sprite = HelpInfo.getFactionBackground();
            container.transform.SetParent(HudManager.Instance.transform);
            container.gameObject.transform.SetLocalZ(-200);
            container.transform.localPosition = new Vector3(0, 0, -50f);
            container.transform.localScale = new Vector3(.75f, .7f, 1f);
            container.gameObject.layer = 5;

            RolesUI = container.gameObject;

            Transform buttonTemplate = HudManager.Instance.SettingsButton.transform;
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

                Transform buttonTransform = Object.Instantiate(buttonTemplate, container.transform);
                buttonTransform.name = faction + " Button";
                buttonTransform.GetComponent<BoxCollider2D>().size = new Vector2(2.5f, 0.55f);
                buttonTransform.GetComponent<SpriteRenderer>().sprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.BlankPlate.png", 215f);
                buttons.Add(buttonTransform);
                buttonTransform.localPosition = new Vector3(0, 1.8f - i * 1f, -5);
                buttonTransform.localScale = new Vector3(2f, 1.5f, 1f);

                TextMeshPro label = Object.Instantiate(textTemplate, buttonTransform);
                label.text = faction;
                label.alignment = TextAlignmentOptions.Center;
                label.transform.localPosition = new Vector3(0, 0, label.transform.localPosition.z);
                label.transform.localScale = new Vector3(1.6f, 2.3f, 1f);

                PassiveButton button = buttonTransform.GetComponent<PassiveButton>();
                button.OnClick.RemoveAllListeners();
                Button.ButtonClickedEvent onClick = button.OnClick = new Button.ButtonClickedEvent();
                onClick.AddListener((Action)(() =>
                {
                    Object.Destroy(container.gameObject);
                    roleInfosOnclick(faction, factionid);
                }));

                button.OnMouseOver.RemoveAllListeners();
                button.OnMouseOver.AddListener((Action)(() =>
                {
                    buttonTransform.GetComponent<SpriteRenderer>().color = Color.yellow;
                }));

                button.OnMouseOut.RemoveAllListeners();
                button.OnMouseOut.AddListener((Action)(() =>
                {
                    buttonTransform.GetComponent<SpriteRenderer>().color = Color.white;
                }));
            }
        }

        public static void roleInfosOnclick(string faction, Faction factionId)
        {
            List<RoleId> NKIds = new() { RoleId.HeadHunter, RoleId.Pyromaniac, RoleId.Nightmare, RoleId.RuthlessRomantic, RoleId.RogueImpostor };
            SpriteRenderer container = new GameObject("RoleListMenuContainer").AddComponent<SpriteRenderer>();
            container.sprite = HelpInfo.getMenuBackground();
            container.transform.SetParent(HudManager.Instance.transform);
            container.transform.localPosition = new Vector3(0, 0f, -75f);
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

                Transform buttonTransform = Object.Instantiate(buttonTemplate, container.transform);
                buttonTransform.name = Helpers.ColorString(roleInfo.Color, roleInfo.Name) + " Button";
                buttonTransform.GetComponent<BoxCollider2D>().size = new Vector2(2.5f, 0.55f);
                TextMeshPro label = Object.Instantiate(textTemplate, buttonTransform);
                buttonTransform.GetComponent<SpriteRenderer>().sprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.BlankPlate.png", 215f);
                buttons.Add(buttonTransform);
                int row = count / 3, col = count % 3;
                buttonTransform.localPosition = new Vector3(-3.205f + col * 3.2f, 2.4f - row * 0.75f, -5);
                buttonTransform.localScale = new Vector3(1.125f, 1.125f, 1f);
                label.text = Helpers.ColorString(roleInfo.Color, roleInfo.Name);
                label.alignment = TextAlignmentOptions.Center;
                label.transform.localPosition = new Vector3(0, 0, label.transform.localPosition.z);
                label.transform.localScale *= 1.55f;
                PassiveButton button = buttonTransform.GetComponent<PassiveButton>();
                button.OnClick.RemoveAllListeners();
                Button.ButtonClickedEvent onClick = button.OnClick = new Button.ButtonClickedEvent();
                onClick.AddListener((Action)(() =>
                {
                    Object.Destroy(container.gameObject);
                    AddInfoCard(roleInfo);
                }));
                button.OnMouseOut.RemoveAllListeners();
                button.OnMouseOver.AddListener((Action)(() =>
                {
                    buttonTransform.GetComponent<SpriteRenderer>().color = Color.yellow;
                }));
                button.OnMouseOut.RemoveAllListeners();
                button.OnMouseOut.AddListener((Action)(() =>
                {
                    buttonTransform.GetComponent<SpriteRenderer>().color = Color.white;
                }));
                count++;
            }
        }
        static void AddInfoCard(RoleInfo roleInfo)
        {
            string roleSettingDescription = HelpInfo.helpText(roleInfo);
            string coloredHelp = Helpers.ReplaceWithColors(roleSettingDescription);

            GameObject roleCard = Object.Instantiate(new GameObject("RoleCard"), HudManager.Instance.transform);
            SpriteRenderer roleCardRend = roleCard.AddComponent<SpriteRenderer>();
            roleCard.layer = 5;
            roleCard.transform.localPosition = new Vector3(0f, 0f, -150f);
            roleCard.transform.localScale = new Vector3(0.68f, 0.68f, 1f);
            RolesUI = roleCard.gameObject;

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
