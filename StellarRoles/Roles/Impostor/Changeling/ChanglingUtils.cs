using HarmonyLib;
using StellarRoles.Patches;
using StellarRoles.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace StellarRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class ChangelingAbilites
    {
        public static void Postfix()
        {
            if (PlayerControl.LocalPlayer != Changeling.Player || !Helpers.GameStarted) return;

            ChangelingUtils.FixedUpdate();
        }
    }

    public static class ChangelingUtils
    {
        public static GameObject ChangelingUI;

        public static void FixedUpdate()
        {
            if (ChangelingUI == null) return;
            if (MeetingHud.Instance != null)
            {
                Object.Destroy(ChangelingUI);
                ChangelingUI = null;
            }
        }
        public static void ChanglingButton()
        {
            if (HelpMenu.RolesUI) return;
            if (ChangelingUI != null) Object.Destroy(ChangelingUI);

            SpriteRenderer container = new GameObject("ChangelingMenu").AddComponent<SpriteRenderer>();
            container.sprite = HelpInfo.getMenuBackground();
            container.transform.SetParent(HudManager.Instance.transform);
            container.transform.localPosition = new Vector3(0, 0f, -75f);
            container.transform.localScale = new Vector3(.7f, .7f, 1f);
            container.gameObject.layer = 5;
            ChangelingUI = container.gameObject;

            Transform buttonTemplate = HudManager.Instance.SettingsButton.transform;
            TextMeshPro textTemplate = HudManager.Instance.TaskPanel.taskText;

            TextMeshPro newtitle = Object.Instantiate(textTemplate, container.transform);
            newtitle.text = "Choose Wisely";
            newtitle.outlineWidth = .25f;
            newtitle.transform.localPosition = new Vector3(0f, 2.7f, -2f);
            newtitle.transform.localScale = Vector3.one * 2.5f;

            List<Transform> buttons = new();
            int count = 0;
            RoleManagerSelectRolesPatch.RoleAssignmentData roleData = RoleManagerSelectRolesPatch.GetRoleAssignmentData();

            foreach (RoleInfo roleInfo in RoleInfo.AllRoleInfos)
            {
                RoleId roleId = roleInfo.RoleId;
                if (roleId == RoleId.Follower && Cultist.FollowerImpRolesEnabled && Cultist.Player != null)
                {

                }
                else if (roleId == RoleId.Changeling
                       || roleId == RoleId.Cultist
                       || !roleData.ImpSettings.ContainsKey(roleId)
                       || roleData.ImpSettings[roleInfo.RoleId] == 0
                       || Changeling.Player.ImpostorPartnersRoles().Any(x => x == roleId)) continue;


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
                    RPCProcedure.Send(CustomRPC.ChangelingChange, (byte)roleId);
                    RPCProcedure.ChangelingChange(roleId);
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
    }

    public static class ChangelingHelpers
    {
        public static List<RoleId> ImpostorPartnersRoles(this PlayerControl changeling)
        {
            List<RoleId> roleList = new List<RoleId>();

            foreach (PlayerControl player in PlayerControl.AllPlayerControls.GetFastEnumerator())
            {
                if (player == changeling || !player.Data.Role.IsImpostor) continue;

                var roleId = RoleInfo.GetRoleInfoForPlayer(player, false).FirstOrDefault().RoleId;

                roleList.Add(roleId);

                switch (roleId)
                {
                    case RoleId.Vampire:
                        roleList.Add(RoleId.Warlock);
                        roleList.Add(RoleId.Shade);
                        break;
                    case RoleId.Shade:
                        roleList.Add(RoleId.Warlock);
                        roleList.Add(RoleId.Vampire);
                        break;
                    case RoleId.Warlock:
                        roleList.Add(RoleId.Shade);
                        roleList.Add(RoleId.Vampire);
                        break;
                }
            }

            return roleList;
        }
    }
}
