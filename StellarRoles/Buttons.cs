using HarmonyLib;
using StellarRoles.Modules;
using StellarRoles.Objects;
using StellarRoles.Patches;
using UnityEngine;
using static StellarRoles.Objects.CustomButton;

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
        public static CustomButton RefreshCosmeticsButton { get; set; }
        public static CustomButton ImpKillButton { get; set; }
        public static CustomButton ImpChatButton { get; set; }
        public static CustomButton HistoryButton { get; set; }



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
            RefreshCosmeticsButton.Timer = 0f;
            RefreshCosmeticsButton.MaxTimer = 10f;
            ImpChatButton.Timer = 0f;
            ImpChatButton.MaxTimer = 0f;
            HistoryButton.MaxTimer = 0f;
            HistoryButton.Timer = 0f;
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
                        RPCProcedure.Send(CustomRPC.RemoveSpectator, Spectator.Target);
                        RPCProcedure.RemoveSpectator(Spectator.Target);
                    }
                    else
                    {
                        RPCProcedure.Send(CustomRPC.AddSpectator, Spectator.Target);
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
                    if (Spectator.ToBecomeSpectator.Contains(Spectator.Target))
                    {
                        text = "Remove";
                        if (Spectator.Target == PlayerControl.LocalPlayer)
                            text = "Self Remove";
                    }
                    SpectatorButton.ActionButton.buttonLabelText.text = $"{text}\nSpectator";
                    SpectatorButton.ActionButton.buttonLabelText.transform.localScale = Vector3.one * 1.3f;

                    return Spectator.Target != null;
                },
                () => { }, // noop
                Helpers.LoadSpriteFromResources("StellarRoles.Resources.SpectatorButton.png", 115f),
                new(-0.25f, 0f, 0f),
                null,
                true,
                "Assign\nSpectator"
                );

            RefreshCosmeticsButton = new CustomButton(
                () =>
                {
                    if (CustomHatLoader.IsRunning || CustomVisorLoader.IsRunning) return;

                    RefreshCosmeticsButton.IsEffectActive = true;
                    RefreshCosmeticsButton.Timer = 5;

                    Helpers.LoadCosmetics(HatManager.Instance);
                },
                () => { return PlayerControl.LocalPlayer && LobbyBehaviour.Instance; },
                () =>
                {
                    var CosmeticsTotal = CustomHatLoader.TotalHatsToDownload + CustomVisorLoader.TotalVisorsToDownload;
                    var CosmeticsDownloaded = CustomHatLoader.TotalHatsDownloaded + CustomVisorLoader.TotalVisorsDownloaded;
                    double CosmeticsLeft = ((double)CosmeticsDownloaded / CosmeticsTotal);
                    string Percent = CosmeticsLeft.ToString("p1");

                    string text = "Cosmetics\nRefresh";
                    if (CustomHatLoader.IsRunning || CustomVisorLoader.IsRunning)
                    {
                        text = "Cosmetics\nDownloading\n" + Percent;
                    }

                    RefreshCosmeticsButton.ActionButton.buttonLabelText.text = text;
                    RefreshCosmeticsButton.ActionButton.buttonLabelText.transform.localPosition = new Vector3(0, -0.65f);
                    RefreshCosmeticsButton.ActionButton.buttonLabelText.transform.localScale = Vector3.one * 1.2f;

                    return true;
                },
                () => { }, // noop
                Helpers.LoadSpriteFromResources("StellarRoles.Resources.CosmeticRefreshButton.png", 115f),
                new Vector3(0.75f, .1f, 0),
                null,
                true,
                "Refresh"
                );
            HistoryButton = new CustomButton(
                () =>
                {
                    PreviousGameHistory.ToggleHistoryScreen();
                },
                () => { return PlayerControl.LocalPlayer && LobbyBehaviour.Instance && PreviousGameHistory.PreviousGameList != null && PreviousGameHistory.PreviousGameList.Count > 0; },
                () =>
                {
                    if (HistoryButton.Timer > 0) HistoryButton.Timer = 0f;
                    if (PreviousGameHistory.HistoryUI != null && PreviousGameHistory.HistoryUI.active)
                    {
                        HistoryButton.ActionButton.buttonLabelText.text = "Close";
                        HistoryButton.ActionButton.buttonLabelText.transform.localScale = Vector3.one;
                        if (HudManager.Instance.GameMenu.isActiveAndEnabled)
                        {
                            PreviousGameHistory.ToggleHistoryScreen();
                        }
                    }
                    else
                    {
                        HistoryButton.ActionButton.buttonLabelText.text = "View\nHistory";
                        HistoryButton.ActionButton.buttonLabelText.transform.localScale = Vector3.one * 1.3f;
                    }

                    return true;
                },
                () => { }, // noop
                Helpers.LoadSpriteFromResources("StellarRoles.Resources.GameHistory.GameHistoryButton.png", 150f),
                new Vector3(-1f, 0f, -12),
                null,
                false,
                "History"
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
                    () => { return PlayerControl.LocalPlayer && !Helpers.IsHideAndSeek; },
                    () =>
                    {
                        if (HelpButton.Timer > 0) HelpButton.Timer = 0f;
                        if (Helpers.GameStarted || Helpers.TutorialActive)
                        {
                            HelpButton.ActionButton.transform.localPosition += new Vector3(2.02f, -1.36f);
                        }
                        HelpButton.ActionButton.graphic.sprite = HelpMenu.RolesUI == null
                            ? HelpMenu.GetHelpButtonSprite()
                            : HelpMenu.GetHelpButtonCloseSprite();
                        return true;
                    },
                    () => { }, // noop
                    HelpMenu.GetHelpButtonSprite(),
                    new Vector3(-1.6f, 4.9f, -20),
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

            ImpChatButton = new CustomButton(
                    () =>
                    {
                        if (HudManager.Instance.Chat.IsOpenOrOpening)
                        {
                            if (!Impostor.ImpChatOn)
                            {
                                HudManager.Instance.Chat.Toggle();
                                Impostor.ImpChatOn = true;
                                HudManager.Instance.Chat.Toggle();
                            }
                            else
                            {
                                HudManager.Instance.Chat.Toggle();
                            }
                        }
                        else
                        {
                            Impostor.ImpChatOn = true;
                            HudManager.Instance.Chat.Toggle();
                        }
                    },
                    () =>
                    {
                        var enabled = Impostor.ChatEnabled || (PlayerControl.LocalPlayer.IsTeamCultist() && Cultist.CultistMeetingChat);
                        return PlayerControl.LocalPlayer != null && PlayerControl.LocalPlayer.Data.Role.IsImpostor && Spy.Player == null && enabled && MeetingHud.Instance;
                    },
                    () =>
                    {
                        return true;
                    },
                    () => { }, // noop
                    Impostor.GetImpChatSprite(),
                    new Vector3(-0.83f, 4.9f, -40),
                    null,
                    buttonText: "",
                    seeInMeeting: true
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
                    () =>
                    {
                        return PlayerControl.LocalPlayer && PlayerControl.LocalPlayer.NeutralKillerCanSabo();
                    },
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
                   Helpers.ResetAbilityCooldown(true);
                   Impostor.CurrentTarget = null;
                   PlayerControl.LocalPlayer.RPCAddGameInfo(InfoType.AddKill);
               },
               () =>
               {
                   var localPlayer = PlayerControl.LocalPlayer;
                   if (localPlayer == Vampire.Player && !Vampire.HasKillButton) return false;
                   if (localPlayer == Parasite.Player && !Parasite.NormalKillButton) return false;
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
