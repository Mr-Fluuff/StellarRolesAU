using HarmonyLib;
using Reactor.Utilities.Extensions;
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


                Transform RoleButton = Object.Instantiate(buttonTemplate, container.transform);
                RoleButton.GetComponent<AspectPosition>().Destroy();
                RoleButton.name = Helpers.ColorString(roleInfo.Color, roleInfo.Name) + " Button";
                RoleButton.GetComponent<BoxCollider2D>().size = new Vector2(2.5f, 0.55f);
                TextMeshPro ButtonLabel = Object.Instantiate(textTemplate, RoleButton);
                var ActiveSprite = RoleButton.FindChild("Active").GetComponent<SpriteRenderer>();
                var InactiveSprite = RoleButton.FindChild("Inactive").GetComponent<SpriteRenderer>();
                RoleButton.FindChild("Background").gameObject.active = false;
                ActiveSprite.sprite = HelpMenu.GetPlateSprite();
                InactiveSprite.sprite = HelpMenu.GetPlateSprite();
                ActiveSprite.color = Color.green;
                buttons.Add(RoleButton);
                int row = count / 3, col = count % 3;
                RoleButton.localPosition = new Vector3(-3.205f + col * 3.2f, 2.4f - row * 0.75f, -5);
                RoleButton.localScale = new Vector3(1.125f, 1.125f, 1f);
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
                    RPCProcedure.Send(CustomRPC.ChangelingChange, (byte)roleId);
                    RPCProcedure.ChangelingChange(roleId);
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
