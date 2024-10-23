using HarmonyLib;
using StellarRoles.Objects;
using UnityEngine;

namespace StellarRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
    public static class ParityCopButtons
    {
        private static bool Initialized;

        public static CustomButton ParityCopCompareButton { get; set; }
        public static CustomButton ParityCopFakeCompareButton { get; set; }

        public static void InitParityCopButtons()
        {
            CompareButton();
            FakeCompareButton();

            Initialized = true;
            SetParityCopCooldowns();
        }

        public static void CompareButton()
        {
            ParityCopCompareButton = new CustomButton(
                () =>
                {
                    PlayerControl.LocalPlayer.IsParityCop(out ParityCop parityCop);
                    RPCProcedure.Send(CustomRPC.ParityCopCompareAddition, PlayerControl.LocalPlayer, parityCop.CurrentTarget);
                    parityCop.ComparedPlayers.Add(parityCop.CurrentTarget);
                    SoundEffectsManager.Play(Sounds.TrackPlayer); // uses the same sound as the tracker
                    ParityCopCompareButton.Timer = ParityCopCompareButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer);
                    RPCProcedure.Send(CustomRPC.PsychicAddCount);
                },
                () =>
                {
                    return PlayerControl.LocalPlayer.IsParityCop(out ParityCop parityCop) && !parityCop.Player.Data.IsDead;
                },
                () =>
                {
                    PlayerControl.LocalPlayer.IsParityCop(out ParityCop parityCop);
                    ParityCopCompareButton.ActionButton.buttonLabelText.SetOutlineColor(Color.cyan);
                    Helpers.ShowTargetNameOnButtonExplicit(null, ParityCopCompareButton, "COMPARE");
                    return PlayerControl.LocalPlayer.CanMove && parityCop.CurrentTarget != null && !ParityCopAbilities.IsRoleBlocked();
                },
                () =>
                {
                    ParityCop.ClearParityCopLists();
                    ParityCopCompareButton.Timer = ParityCopCompareButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer);
                },
                ParityCop.GetButtonSprite(),
                CustomButton.ButtonPositions.UpperRow1,
                "ActionQuaternary",
                false
            );
        }

        public static void FakeCompareButton()
        {
            ParityCopFakeCompareButton = new CustomButton(
                () =>
                {
                    RPCProcedure.Send(CustomRPC.FakeCompare, PlayerControl.LocalPlayer, true);
                    PlayerControl.LocalPlayer.IsParityCop(out ParityCop parityCop);
                    parityCop.PressedFakeCompare = true;
                },
                () =>
                {
                    return
                        ParityCop.FakeCompare &&
                        !PlayerControl.LocalPlayer.Data.IsDead &&
                        PlayerControl.LocalPlayer.IsParityCop(out ParityCop parityCop) &&
                        !parityCop.PressedFakeCompare &&
                        parityCop.FakeCompareCharges > 0f;
                },
                () =>
                {
                    ParityCopFakeCompareButton.ActionButton.buttonLabelText.SetOutlineColor(Color.cyan);
                    Helpers.ShowTargetNameOnButtonExplicit(null, ParityCopFakeCompareButton, "FAKE OUT");
                    return !ParityCopAbilities.IsRoleBlocked();
                },
                () =>
                {
                    if (PlayerControl.LocalPlayer.IsParityCop(out ParityCop parityCop))
                    {
                        RPCProcedure.Send(CustomRPC.FakeCompare, PlayerControl.LocalPlayer, false);
                        parityCop.PressedFakeCompare = false;
                    }
                },
                ParityCop.GetFakeCompareSprite(),
                CustomButton.ButtonPositions.UpperRow2,
                "SecondAbility"
            );
        }

        public static void SetParityCopCooldowns()
        {
            if (!Initialized)
            {
                InitParityCopButtons();
            }

            ParityCopFakeCompareButton.MaxTimer = 0f;
            ParityCopCompareButton.MaxTimer = ParityCop.CalculateParityCopCooldown(PlayerControl.LocalPlayer);

        }

        public static void Postfix()
        {
            Initialized = false;
            InitParityCopButtons();
        }

    }
}
