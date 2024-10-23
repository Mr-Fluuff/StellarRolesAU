using HarmonyLib;
using StellarRoles.Objects;
using UnityEngine;

namespace StellarRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
    internal class AdministratorButtons
    {
        private static bool Initialized;
        public static CustomButton AdminButton;

        public static void AdministratorButton(HudManager __instance)
        {
            AdminButton = new CustomButton(
              () =>
              {
                  if (!Administrator.IsActive)
                  {
                      Administrator.IsActive = true;
                      if (!MapBehaviour.Instance || !MapBehaviour.Instance.isActiveAndEnabled)
                      {
                          HudManager.Instance.InitMap();
                          MapBehaviour.Instance.ShowNormalMap();
                          MapBehaviour.Instance.ShowCountOverlay(allowedToMove: true, showLivePlayerPosition: true, includeDeadBodies: true);
                      }
                      Administrator.BatteryTime--;

                      Helpers.SetMovement(false);
                      AdminButton.Timer = 5f;
                      RPCProcedure.Send(CustomRPC.PsychicAddCount);
                  }

              },
              () => { return Administrator.Player == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead; },
              () =>
              {
                  AdminButton.ActionButton.graphic.sprite = Administrator.GetAdminSprite();
                  AdminButton.ActionButton.buttonLabelText.SetOutlineColor(Color.cyan);
                  Helpers.ShowTargetNameOnButtonExplicit(null, AdminButton, $"ADMIN - {(int)Administrator.BatteryTime}<size=70%>s</size>");
                  AdminButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
                  return PlayerControl.LocalPlayer.CanMove && Administrator.BatteryTime > 1.5 && !(Administrator.DisableRoundOneAccess && MapOptions.IsFirstRound);
              },
              () =>
              {
                  Administrator.IsActive = false;
                  AdminButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
                  AdminButton.Sprite = Administrator.GetAdminSprite();
                  Helpers.SetMovement(true);
              },
              Administrator.GetAdminSprite(),
              CustomButton.ButtonPositions.UpperRow1,
              "ActionQuaternary",
              false,
              2f,
              () =>
              {
                  AdminButton.Timer = 3f;
                  Helpers.SetMovement(true);
                  AdminButton.ActionButton.cooldownTimerText.SetFaceColor(Color.white);
              },
              false,
              "ADMIN"
          );
            Initialized = true;
            SetAdministratorCooldowns();
        }

        public static void SetAdministratorCooldowns()
        {
            if (!Initialized)
            {
                AdministratorButton(HudManager.Instance);
            }

            AdminButton.MaxTimer = 0f;
            AdminButton.Timer = 0f;
        }

        public static void Postfix(HudManager __instance)
        {
            Initialized = false;
            AdministratorButton(__instance);
        }
    }
}
