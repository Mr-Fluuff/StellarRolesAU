using HarmonyLib;
using StellarRoles.Objects;
using StellarRoles.Utilities;
using System;

namespace StellarRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
    public static class WarlockButtons
    {
        private static bool Initialized;
        public static CustomButton CurseButton { get; set; }

        public static void WarlockCurseButton()
        {
            CurseButton = new CustomButton(
                () =>
                {
                    if (Warlock.CurseVictim == null)
                    {
                        // Apply Curse
                        Warlock.CurseVictim = Warlock.CurrentCurseTarget;
                        CurseButton.Timer = 1f;
                        SoundEffectsManager.Play(Sounds.Curse);
                    }
                    else if (Warlock.CurseVictim != null && Warlock.CurseVictimTarget != null)
                    {
                        MurderAttemptResult murder = Helpers.CheckMurderAttemptAndKill(Warlock.Player, Warlock.CurseVictimTarget, showAnimation: false);
                        if (murder == MurderAttemptResult.SuppressKill)
                            return;

                        Helpers.PlayerKilledByAbility(Warlock.CurseVictimTarget);

                        // If blanked or killed
                        if (Warlock.RootTime > 0)
                        {
                            Sleepwalker.LastPosition = PlayerControl.LocalPlayer.transform.position;
                            Helpers.SetMovement(false);
                            FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(Warlock.RootTime, new Action<float>((p) =>
                            { // Delayed action
                                if (p == 1f)
                                    Helpers.SetMovement(true);
                            })));
                        }

                        Warlock.CurseVictim = null;
                        Warlock.CurseVictimTarget = null;
                        CurseButton.Timer = CurseButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer) * Helpers.ClutchMultiplier(PlayerControl.LocalPlayer);
                        Helpers.SetKillerCooldown();
                        Helpers.AddGameInfo(PlayerControl.LocalPlayer.PlayerId, InfoType.AddAbilityKill, InfoType.AddKill);
                    }
                    RPCProcedure.Send(CustomRPC.PsychicAddCount);

                },
                () => { return Warlock.Player == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead && !PlayerControl.LocalPlayer.IsBombedAndActive(); },
                () =>
                {

                    bool canCurse = Warlock.CurseVictim == null && Warlock.CurrentCurseTarget != null;
                    bool canKill = Warlock.CurseVictim != null && Warlock.CurseVictimTarget != null;

                    CurseButton.ActionButton.graphic.sprite = Warlock.CurseVictim != null ? Warlock.GetCurseKillButtonSprite() : Warlock.GetCurseButtonSprite();
                    if (Ascended.IsAscended(Warlock.Player) && Warlock.CurseVictim != null)
                    {
                        Helpers.ShowTargetNameOnButtonExplicit(Warlock.CurseVictimTarget, CurseButton, "Kill");
                    }
                    else
                    {
                        Helpers.ShowTargetNameOnButtonExplicit(null, CurseButton, Warlock.CurseVictim != null ? "Kill" : "CURSE");
                    }


                    CurseButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
                    CurseButton.ActionButton.buttonLabelText.SetOutlineColor(Palette.ImpostorRed);

                    return (canCurse || canKill) && PlayerControl.LocalPlayer.CanMove && !Impostor.IsRoleAblilityBlocked();
                },
                () =>
                {
                    CurseButton.Timer = CurseButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer);
                    CurseButton.Sprite = Warlock.GetCurseButtonSprite();
                    Warlock.CurseVictim = null;
                    Warlock.CurseVictimTarget = null;
                },
                Warlock.GetCurseButtonSprite(),
                CustomButton.ButtonPositions.UpperRow3,
                "ActionQuaternary"
            );

            Initialized = true;
            SetWarlockCooldowns();
        }

        public static void SetWarlockCooldowns()
        {
            if (!Initialized)
            {
                WarlockCurseButton();
            }

            CurseButton.MaxTimer = Warlock.Cooldown;
        }

        public static void Postfix()
        {
            Initialized = false;
            WarlockCurseButton();
        }
    }
}
