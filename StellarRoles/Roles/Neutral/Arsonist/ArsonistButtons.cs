using HarmonyLib;
using StellarRoles.Objects;

namespace StellarRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
    public class ArsonistButtons
    {
        public static bool initialized;
        public static CustomButton ArsonistButton;

        public static void ArsonistDouseButton()
        {
            ArsonistButton = new CustomButton(
                () =>
                {
                    if (Arsonist.DousedEveryoneAlive())
                    {
                        RPCProcedure.Send(CustomRPC.ArsonistWin);
                        RPCProcedure.ArsonistWin();
                        ArsonistButton.HasEffect = false;
                    }
                    else if (Arsonist.CurrentTarget != null)
                    {
                        Arsonist.DouseTarget = Arsonist.CurrentTarget;
                        ArsonistButton.HasEffect = true;
                        SoundEffectsManager.Play(Sounds.Douse);
                    }
                },
                () => { return Arsonist.Player == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead; },
                () =>
                {
                    bool dousedEveryoneAlive = Arsonist.DousedEveryoneAlive();
                    ArsonistButton.ActionButton.buttonLabelText.SetOutlineColor(Arsonist.Color);

                    if (dousedEveryoneAlive)
                    {
                        Helpers.ShowTargetNameOnButtonExplicit(null, ArsonistButton, "IGNITE");
                        ArsonistButton.ActionButton.graphic.sprite = Arsonist.GetIgniteSprite();
                    }
                    else
                    {
                        Helpers.ShowTargetNameOnButtonExplicit(null, ArsonistButton, "DOUSE");
                        ArsonistButton.ActionButton.graphic.sprite = Arsonist.GetDouseSprite();
                    }

                    if (ArsonistButton.IsEffectActive && Arsonist.DouseTarget != Arsonist.CurrentTarget)
                    {
                        Arsonist.DouseTarget = null;
                        ArsonistButton.Timer = 0f;
                        ArsonistButton.IsEffectActive = false;
                    }

                    return PlayerControl.LocalPlayer.CanMove && (dousedEveryoneAlive || Arsonist.CurrentTarget != null);
                },
                () =>
                {
                    ArsonistButton.Timer = Arsonist.RoundCooldown;
                    Arsonist.DouseTarget = null;
                    ArsonistButton.IsEffectActive = false;
                },
                Arsonist.GetDouseSprite(),
                CustomButton.ButtonPositions.UpperRow1,
                "ActionQuaternary",
                true,
                Arsonist.Duration,
                () =>
                {
                    if (Arsonist.DouseTarget != null)
                    {
                        RPCProcedure.Send(CustomRPC.ArsonistDouse, Arsonist.DouseTarget);
                        RPCProcedure.ArsonistDouse(Arsonist.DouseTarget);
                        RPCProcedure.Send(CustomRPC.PsychicAddCount);
                    }
                    Arsonist.DouseTarget = null;
                    ArsonistButton.Timer = Arsonist.DousedEveryoneAlive() ? 0 : ArsonistButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer);

                    foreach (PlayerControl player in Arsonist.DousedPlayers.GetPlayerEnumerator())
                        if (MapOptions.PlayerIcons.TryGetValue(player.PlayerId, out PoolablePlayer icon))
                            icon.SetSemiTransparent(false);
                }
            );

            initialized = true;
            SetArsonistCooldowns();
        }

        public static void SetArsonistCooldowns()
        {
            if (!initialized)
            {
                ArsonistDouseButton();
            }

            ArsonistButton.EffectDuration = Arsonist.Duration;
            ArsonistButton.MaxTimer = Arsonist.Cooldown;
        }

        public static void Postfix()
        {
            initialized = false;
            ArsonistDouseButton();
        }
    }
}
