using HarmonyLib;
using UnityEngine;

namespace StellarRoles.Patches
{
    [HarmonyPatch(typeof(KeyboardJoystick), nameof(KeyboardJoystick.HandleHud))]
    public class KeyboardJoystickPatch
    {
        public static bool Prefix()
        {
            if (KeyboardJoystick.player.GetButtonDown(4))
            {
                HudManager.Instance.MapButton.OnClick.Invoke();
                return false;
            }
            return true;
        }
        public static void Postfix()
        {
            if (HudManager.Instance != null
                && HudManager.Instance.ImpostorVentButton != null
                && HudManager.Instance.ImpostorVentButton.isActiveAndEnabled
                && ConsoleJoystick.player.GetButtonDown(50))
            {
                HudManager.Instance.ImpostorVentButton.DoClick();
                if (Engineer.Player == PlayerControl.LocalPlayer)
                {
                    EngineerButtons.EngineerVent.ActionButton.DoClick();
                }
            }

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                MapOptions.ShowRoles = !MapOptions.ShowRoles;
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (HelpMenu.RolesUI != null)
                {
                    Object.Destroy(HelpMenu.RolesUI);
                    HelpMenu.RolesUI = null;
                }

                if (ChangelingUtils.ChangelingUI != null)
                {
                    Object.Destroy(ChangelingUtils.ChangelingUI);
                    ChangelingUtils.ChangelingUI = null;
                }

                if (PreviousGameHistory.HistoryUI != null)
                {
                    PreviousGameHistory.HistoryUI.SetActive(false);
                }
            }
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.R) && LobbyBehaviour.Instance)
            {
                PlayerControl.LocalPlayer.MyPhysics.RpcCancelPet();
            }
        }
    }
}
