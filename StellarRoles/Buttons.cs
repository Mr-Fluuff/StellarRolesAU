using HarmonyLib;
using StellarRoles.Objects;
using StellarRoles.Patches;
using UnityEngine;

namespace StellarRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
    public static class HudManagerStartPatch
    {
        private static bool Initialized;

        public static CustomButton ResetZoomButton { get; set; }
        public static CustomButton HelpButton { get; set; }
        public static CustomButton NeutralKillerSaboButton { get; set; }
        public static CustomButton SpectatorButton { get; set; }
        public static CustomButton ImpKillButton { get; set; }

        public static void SetOtherButtonCooldowns()
        {
            AdministratorButtons.SetAdministratorCooldowns();
            DetectiveButtons.SetDetectiveCooldowns();
            EngineerButtons.SetEngineerCooldowns();
            GuardianButtons.SetGuardianCooldowns();
            MedicButtons.SetMedicCooldowns();
            ParityCopButtons.SetParityCopCooldowns();
            PsychicButtons.SetPsychicCooldowns();
            SheriffButtons.SetSheriffCooldowns();
            TrackerButtons.SetTrackerCooldowns();
            TrapperButtons.SetTrapperCooldowns();
            WatcherButtons.SetWatcherCooldowns();

            //Impostor Cooldowns
            BomberButtons.SetBomberCooldowns();
            CamouflagerButtons.SetCamouflagerCooldowns();
            ChangelingButtons.SetChanglingCooldowns();
            CultistButtons.SetCultistCooldowns();
            JanitorButtons.SetJanitorCooldowns();
            MinerButtons.SetMinerCooldowns();
            MorphlingButtons.SetMorphlingCooldowns();
            ShadeButtons.SetShadeCooldowns();
            HackerButtons.SetHackcerCooldown();
            UndertakerButtons.SetUndertakerCooldowns();
            VampireButtons.SetVampireCooldowns();
            WarlockButtons.SetWarlockCooldowns();
            WraithButtons.SetWraithCooldowns();

            //Neutral Cooldowns
            RomanticButtons.SetRomanticCooldowns();
            ArsonistButtons.SetArsonistCooldowns();
            ScavengerButtons.SetScavengerCooldowns();
            RefugeeButtons.SetRefugeeCooldowns();

            //NK Cooldowns
            RogueButtons.SetRogueCooldowns();
            HeadHunterButtons.SetHeadHunterCooldowns();
            NightmareButtons.SetNightmareCooldowns();
            PyromaniacButtons.SetPyromaniacCooldowns();
            RuthlessButtons.SetRuthlessRomanticCooldowns();
        }

        public static void SetCustomButtonCooldowns()
        {
            if (!Initialized)
            {
                CustomButtonsPostFix(HudManager.Instance);
            }

            HelpButton.Timer = 0f;
            HelpButton.MaxTimer = 0f;
            ResetZoomButton.Timer = 0f;
            ResetZoomButton.MaxTimer = 0f;
            NeutralKillerSaboButton.MaxTimer = 0f;
            NeutralKillerSaboButton.Timer = 0f;
            ImpKillButton.MaxTimer = Helpers.KillCooldown();
            SpectatorButton.Timer = 0f;
            SpectatorButton.MaxTimer = 0f;
        }

        public static void Postfix(HudManager __instance)
        {
            Initialized = false;
            try
            {
                CustomButtonsPostFix(__instance);
            }
            catch { }
        }

        public static void CustomButtonsPostFix(HudManager __instance)
        {
            SpectatorButton = new CustomButton(
                () =>
                {
                    if (Spectator.ToBecomeSpectator.Contains(Spectator.Target.PlayerId))
                    {
                        RPCProcedure.Send(CustomRPC.RemoveSpectator, Spectator.Target.PlayerId);
                        RPCProcedure.RemoveSpectator(Spectator.Target);
                    }
                    else
                    {
                        RPCProcedure.Send(CustomRPC.AddSpectator, Spectator.Target.PlayerId);
                        RPCProcedure.AddSpectator(Spectator.Target);
                    }
                },
                () => { return AmongUsClient.Instance.AmHost && LobbyBehaviour.Instance; },
                () =>
                {
                    SpectatorButton.ActionButton.buttonLabelText.SetOutlineColor(Color.gray);
                    string text = "Assign";
                    if (Spectator.Target == PlayerControl.LocalPlayer)
                        text = "Self Assign";
                    if (Spectator.ToBecomeSpectator.Contains(Spectator.Target.PlayerId))
                    {
                        text = "Remove";
                        if (Spectator.Target == PlayerControl.LocalPlayer)
                            text = "Self Remove";
                    }
                    SpectatorButton.ActionButton.buttonLabelText.text = $"{text}\nSpectator";
                    return Spectator.Target != null;
                },
                () => { }, // noop
                Helpers.LoadSpriteFromResources("StellarRoles.Resources.SpectatorButton.png", 115f),  // Invisible button!
                new Vector3(0, 1, 0),
                null,
                false,
                "Assign\nSpectator"
                );


            HelpButton = new CustomButton(
                    () =>
                    {
                        if (MeetingHudPatch.GuesserUI != null)
                        {
                            MeetingHudPatch.GuesserUIExitButton.OnClick.Invoke();
                        }

                        if (ChangelingUtils.ChangelingUI != null)
                        {
                            Object.Destroy(ChangelingUtils.ChangelingUI);
                        }

                        HelpInfo.RefreshInfo();
                        if (HelpMenu.RolesUI == null)
                            HelpMenu.RoleFactionOnClick();
                        else
                        {
                            Object.Destroy(HelpMenu.RolesUI);
                            HelpMenu.RolesUI = null;
                            MeetingHudPatch.ResetMeetingHud();
                        }
                    },
                    // this will always be true
                    () => PlayerControl.LocalPlayer != null && !Helpers.IsHideAndSeek,
                    () =>
                    {
                        HelpButton.ActionButton.graphic.sprite = HelpMenu.RolesUI == null
                            ? Helpers.LoadSpriteFromResources("StellarRoles.Resources.HelpMenu.HelpButton.png", 150f)
                            : Helpers.LoadSpriteFromResources("StellarRoles.Resources.HelpMenu.HelpButtonExit.png", 150f);
                        return true;
                    },
                    () => { }, // noop
                    Helpers.LoadSpriteFromResources("StellarRoles.Resources.HelpMenu.HelpButton.png", 150f),
                    new Vector3(0.4f, 3.6f, 0),
                    "Help",
                    seeInMeeting: true
                    );

            ResetZoomButton = new CustomButton(
                    () =>
                    {
                        Helpers.ResetZoom();
                    },
                    () => { return PlayerControl.LocalPlayer != null && PlayerControl.LocalPlayer.Data.IsDead && Camera.main.orthographicSize != 3; },
                    () =>
                    {
                        ResetZoomButton.ActionButton.buttonLabelText.SetOutlineColor(Color.white);
                        return true;
                    },
                    () => { }, // noop
                    Helpers.LoadSpriteFromResources("StellarRoles.Resources.ZoomReset.png", 150f),
                    new Vector3(0.4f, 2.9f, 0),
                    null,
                    buttonText: "Reset\nZoom"
                    );

            NeutralKillerSaboButton = new CustomButton(
                    () =>
                    {
                        if (!MapBehaviour.Instance || !MapBehaviour.Instance.isActiveAndEnabled)
                        {
                            __instance.InitMap();
                            MapBehaviour.Instance.ShowSabotageMap();
                        }
                    },
                    () => PlayerControl.LocalPlayer.NeutralKillerCanSabo(),
                    () =>
                    {
                        NeutralKillerSaboButton.ActionButton.buttonLabelText.SetOutlineColor(NeutralKiller.Color);
                        return PlayerControl.LocalPlayer.NeutralKillerCanSabo();
                    },
                    () => { }, // noop
                    __instance.SabotageButton.graphic.sprite,
                    new Vector3(0, 0, 0),
                    null,
                    true,
                    "Sabotage"
                    );

            ImpKillButton = new CustomButton(
               () =>
               {
                   if (Helpers.CheckMurderAttemptAndKill(PlayerControl.LocalPlayer, Impostor.CurrentTarget) == MurderAttemptResult.SuppressKill)
                       return;
                   ImpKillButton.Timer = ImpKillButton.MaxTimer;

                   if (PlayerControl.LocalPlayer == BountyHunter.Player)
                   {
                       if (Impostor.CurrentTarget == BountyHunter.Bounty)
                       {
                           ImpKillButton.Timer = Helpers.KillCooldown() - BountyHunter.BonusTime;
                           BountyHunter.BountyUpdateTimer = 0f; // Force bounty update
                       }
                       else
                       {
                           ImpKillButton.Timer = Helpers.KillCooldown() + BountyHunter.PunishmentTime;
                       }
                   }

                   if (PlayerControl.LocalPlayer == Undertaker.Player)
                   {
                       CustomButton dragButton = UndertakerButtons.UndertakerDragButton;
                       if (dragButton.Timer < Undertaker.DraggingDelayAfterKill * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer) * Helpers.ClutchMultiplier(PlayerControl.LocalPlayer))
                       {
                           dragButton.Timer = Undertaker.DraggingDelayAfterKill * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer) * Helpers.ClutchMultiplier(PlayerControl.LocalPlayer);
                       }
                   }

                   if (PlayerControl.LocalPlayer == Vampire.Player)
                   {
                       CustomButton biteButton = VampireButtons.VampireBiteButton;
                       if (biteButton.Timer < biteButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer) * Helpers.ClutchMultiplier(PlayerControl.LocalPlayer))
                           biteButton.Timer = biteButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer) * Helpers.ClutchMultiplier(PlayerControl.LocalPlayer);
                   }

                   if (PlayerControl.LocalPlayer == Bomber.Player)
                   {
                       CustomButton bombButton = BomberButtons.BomberBombButton;
                       if (bombButton.Timer < bombButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer) * Helpers.ClutchMultiplier(PlayerControl.LocalPlayer))
                       {
                           bombButton.Timer = bombButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer) * Helpers.ClutchMultiplier(PlayerControl.LocalPlayer);
                       }
                   }

                   if (PlayerControl.LocalPlayer == Warlock.Player)
                   {
                       CustomButton curseButton = WarlockButtons.CurseButton;
                       if (curseButton.Timer < curseButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer) * Helpers.ClutchMultiplier(PlayerControl.LocalPlayer))
                       {
                           curseButton.Timer = curseButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer) * Helpers.ClutchMultiplier(PlayerControl.LocalPlayer);
                       }
                   }

                   if (PlayerControl.LocalPlayer == Janitor.Player)
                   {
                       CustomButton cleanButton = JanitorButtons.CleanButton;
                       if (cleanButton.Timer < cleanButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer) * Helpers.ClutchMultiplier(PlayerControl.LocalPlayer))
                       {
                           cleanButton.Timer = cleanButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer) * Helpers.ClutchMultiplier(PlayerControl.LocalPlayer);
                       }
                   }
                   Impostor.CurrentTarget = null;
                   Helpers.AddGameInfo(PlayerControl.LocalPlayer.PlayerId, InfoType.AddKill);
               },
               () =>
               {
                   var localPlayer = PlayerControl.LocalPlayer;
                   if (localPlayer == Vampire.Player && !Vampire.HasKillButton) return false;
                   return !localPlayer.Data.IsDead && localPlayer.Data.Role.IsImpostor;
               },
               () =>
               {
                   bool cultist = PlayerControl.LocalPlayer == Cultist.Player;
                   bool hasconverted = cultist && !Cultist.NeedsFollower;
                   bool canuse = !cultist || hasconverted;

                   return Impostor.CurrentTarget != null && PlayerControl.LocalPlayer.CanMove && canuse && !PlayerControl.LocalPlayer.IsBombedAndActive();
               },
               () => { ImpKillButton.Timer = ImpKillButton.MaxTimer; },
               __instance.KillButton.graphic.sprite,
               CustomButton.ButtonPositions.UpperRow1,
               "ActionSecondary"
               );

            // Set the default (or settings from the previous game) timers / durations when spawning the buttons

            Initialized = true;
            SetCustomButtonCooldowns();
        }
    }
}
