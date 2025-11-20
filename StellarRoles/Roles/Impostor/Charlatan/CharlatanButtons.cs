using HarmonyLib;
using StellarRoles.Objects;
using UnityEngine;

namespace StellarRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
    public class CharlatanButtons
    {
        public static bool initialized;
        public static CustomButton DeceiveButton;
        public static CustomButton ConcealButton;



        public static void DeceiveButtonInt()
        {
            DeceiveButton = new CustomButton(
                () =>
                {
                    CharlatanAbilities.DeceiveAbility();
                },
                () => { return Charlatan.Player == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead; },
                () =>
                {
                    string deceive = Charlatan.DeceiveTimer <= 0 ? "Deceive" : $"Deceive - {(int)Charlatan.DeceiveTimer}";
                    Helpers.ShowTargetNameOnButtonExplicit(null, DeceiveButton, deceive);
                    return PlayerControl.LocalPlayer.CanMove && !Impostor.IsRoleAblilityBlocked() && Charlatan.ReportTarget != byte.MaxValue;
                },
                () =>
                {
                    DeceiveButton.Timer = DeceiveButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer) * Helpers.ClutchMultiplier(PlayerControl.LocalPlayer);
                    DeceiveButton.IsEffectActive = false;
                    DeceiveButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
                },
                Charlatan.GetDeceiveButtonSprite(),
                CustomButton.ButtonPositions.UpperRow3,
                "ActionQuaternary",
                false,
                0f,
                () =>
                {
                }
            );
        }

        public static void ConcealButtonInt()
        {
            ConcealButton = new CustomButton(
                () =>
                {
                    ConcealButton.HasEffect = true;
                },
                () => { return Charlatan.Player == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead; },
                () =>
                {
                    if (ConcealButton.IsEffectActive && Charlatan.DeadBodies.Count <= 0)
                    {
                        ConcealButton.IsEffectActive = false;
                        ConcealButton.Timer = 0f;
                    }
                    string conceal = Charlatan.ConcealCharges > 0 ? $"Conceal - {(int)Charlatan.ConcealCharges}" : "Conceal";
                    if (ConcealButton.IsEffectActive) conceal = "Concealing";
                    Helpers.ShowTargetNameOnButtonExplicit(null, ConcealButton, conceal);
                    return PlayerControl.LocalPlayer.CanMove && !Impostor.IsRoleAblilityBlocked() && Charlatan.ConcealCharges > 0 && Charlatan.DeadBodies.Count > 0;
                },
                () =>
                {
                    ConcealButton.Timer = ConcealButton.MaxTimer;
                    ConcealButton.IsEffectActive = false;
                },
                Charlatan.GetConcealButtonSprite(),
                CustomButton.ButtonPositions.LowerRow4,
                "SecondAbility",
                true,
                Charlatan.ConcealChannelDuration,
                () =>
                {
                    CharlatanAbilities.ConcealBodiesInRange();
                    Charlatan.ConcealCharges--;
                    RPCProcedure.Send(CustomRPC.PsychicAddCount);
                    ConcealButton.Timer = Helpers.KillCooldown();
                }
            );
        }

        public static void SetCharlatanCooldowns()
        {
            if (!initialized)
            {
                CharlatanButtonsInit();
            }

            DeceiveButton.MaxTimer = 0f;
            ConcealButton.MaxTimer = Helpers.KillCooldown();
        }

        public static void CharlatanButtonsInit()
        {
            DeceiveButtonInt();
            ConcealButtonInt();

            initialized = true;
            SetCharlatanCooldowns();
        }

        public static void Postfix()
        {
            initialized = false;
            CharlatanButtonsInit();
        }
    }
}
