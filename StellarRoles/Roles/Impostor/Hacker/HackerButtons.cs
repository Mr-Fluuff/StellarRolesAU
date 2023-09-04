using HarmonyLib;
using StellarRoles.Objects;
using System.Linq;
using UnityEngine;


namespace StellarRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]

    public static class HackerButtons
    {
        private static bool Initialized;

        public static CustomButton JamButton;
        public static CustomButton VitalsButton;
        public static CustomButton AdminButton;
        public static CustomButton CamerasButton;
        public static CustomButton DownloadButton;

        public static void InitHackerButtons()
        {
            JamButtonInit();
            AdminButtonInit();
            VitalsButtonInit();
            CamerasButtonInit();
            DownloadButtonInit();
            Initialized = true;
        }

        public static void DownloadButtonInit()
        {

            DownloadButton = new CustomButton(
                () =>
                {
                    Hacker.InformationSource = Hacker.CurrentTargetInformationSource;
                    Hacker.IsDownloading = true;
                    DownloadButton.IsEffectActive = true;
                    DownloadButton.Timer = 10f;
                    RPCProcedure.Send(CustomRPC.PsychicAddCount);
                },
                () =>
                {

                    bool isHacker = Hacker.Player == PlayerControl.LocalPlayer;
                    bool noSource = Hacker.InformationSource == InformationSource.None;
                    bool nearChosenSourcce = Hacker.CurrentTargetInformationSource == Hacker.InformationSource;
                    //  Helpers.Log(Hacker.informationSource + "");

                    return isHacker && !PlayerControl.LocalPlayer.Data.IsDead
                    && (noSource || nearChosenSourcce);

                },
                () =>
                {
                    if (DownloadButton.IsEffectActive && Hacker.CurrentTargetInformationSource != Hacker.InformationSource)
                    {
                        //download.Timer = 0f;
                        DownloadButton.IsEffectActive = false;
                    }
                    DownloadButton.ActionButton.buttonLabelText.SetOutlineColor(Hacker.Color);
                    Helpers.ShowTargetNameOnButtonExplicit(null, DownloadButton, $"DOWNLOAD {(int)Hacker.DownloadTime}");
                    DownloadButton.Sprite = Hacker.GetDownloadSprite();
                    return Hacker.RemainingDownloadTime > 0f && Hacker.CurrentTargetInformationSource != InformationSource.None && !Hacker.NoCamsFirstRound();
                },
                () =>
                {
                    DownloadButton.Timer = DownloadButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer);
                    DownloadButton.IsEffectActive = false;
                    Hacker.RoundReset();
                },
                Hacker.GetDownloadSprite(),
                CustomButton.ButtonPositions.UpperRow3,
                "ActionQuaternary",
                true,
                10f,
                () =>
                {
                    DownloadButton.Timer = 0f;
                }
            );


        }


        public static void JamButtonInit()
        {
            JamButton = new CustomButton(
                    () =>
                    {
                        if (!Hacker.LockedOut)
                        {
                            RPCProcedure.Send(CustomRPC.HackerJamToggle);
                            RPCProcedure.HackerJamToggle();
                            Hacker.JamCharges--;
                        }

                        SoundEffectsManager.Play(Sounds.Click);
                        RPCProcedure.Send(CustomRPC.PsychicAddCount);

                    },

                    () => { return Hacker.Player == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead && Hacker.JamChargesPerKill != 0; },
                    () =>
                    {
                        Helpers.ShowTargetNameOnButtonExplicit(null, JamButton, $"Jam - {(int)Hacker.JamCharges}");
                        return PlayerControl.LocalPlayer.CanMove && Hacker.JamCharges > 0;
                    },
                    () =>
                    {
                        JamButton.Timer = JamButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer) * Helpers.ClutchMultiplier(PlayerControl.LocalPlayer);
                        JamButton.IsEffectActive = false;
                        JamButton.ActionButton.graphic.color = Palette.EnabledColor;
                    },
                    Hacker.GetJamSprite(),
                    CustomButton.ButtonPositions.LowerRow4,
                    "SecondAbility",
                    true,
                    Hacker.JamDuration,
                    () =>
                    {
                        JamButton.Timer = JamButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer) * Helpers.ClutchMultiplier(PlayerControl.LocalPlayer);
                        SoundEffectsManager.Play(Sounds.Click);
                    }
                );
        }

        public static void AdminButtonInit()
        {
            AdminButton = new CustomButton(
              () =>
              {
                  if (!Hacker.AdminActive)
                  {
                      if (!MapBehaviour.Instance || !MapBehaviour.Instance.isActiveAndEnabled)
                      {
                          HudManager.Instance.InitMap();
                          MapBehaviour.Instance.ShowCountOverlay(allowedToMove: true, showLivePlayerPosition: true, includeDeadBodies: true);
                      }

                      Hacker.AdminActive = true;
                      Helpers.SetMovement(false);
                      AdminButton.Timer = 5f;
                      RPCProcedure.Send(CustomRPC.PsychicAddCount);

                  }

              },
              () =>
              {
                  bool nearChosenSourcce = Hacker.CurrentTargetInformationSource == Hacker.InformationSource;


                  return Hacker.Player != null
                  && Hacker.Player == PlayerControl.LocalPlayer
                  && !PlayerControl.LocalPlayer.Data.IsDead
                  && Hacker.InformationSource == InformationSource.Admin
                  && !nearChosenSourcce;
              },
              () =>
              {
                  AdminButton.ActionButton.graphic.sprite = Hacker.GetAdminSprite();
                  AdminButton.ActionButton.buttonLabelText.SetOutlineColor(Hacker.Color);
                  Helpers.ShowTargetNameOnButtonExplicit(null, AdminButton, $"ADMIN - {(int)Hacker.DownloadTime}<size=70%>s</size>");
                  AdminButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
                  return PlayerControl.LocalPlayer.CanMove && Hacker.DownloadTime > 0f;
              },
              () =>
              {
                  Hacker.AdminActive = false;
                  Helpers.SetMovement(true);
                  AdminButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
                  AdminButton.Sprite = Hacker.GetAdminSprite();
                  Hacker.RoundReset();
              },
              Hacker.GetAdminSprite(),
              CustomButton.ButtonPositions.UpperRow3,
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
        }

        public static void VitalsButtonInit()
        {
            VitalsButton = new CustomButton(
               () =>
               {
                   if (!Hacker.VitalsActive)
                   {
                       SystemConsole e = Object.FindObjectsOfType<SystemConsole>().FirstOrDefault(x => x.gameObject.name.Contains("panel_vitals"));
                       if (e == null || Camera.main == null)
                           return;
                       Hacker.VitalsActive = true;
                       VitalsButton.EffectDuration = Hacker.DownloadTime;
                       VitalsButton.Timer = Hacker.DownloadTime;
                       VitalsButton.MaxTimer = Hacker.DownloadTime;
                       Hacker.HackedMinigame = Object.Instantiate(e.MinigamePrefab, Camera.main.transform, false);
                       Hacker.HackedMinigame.transform.SetParent(Camera.main.transform, false);

                       Hacker.HackedMinigame.transform.localPosition = new Vector3(0.0f, 0.0f, -50f);
                       Hacker.HackedMinigame.Begin(null);

                       Helpers.SetMovement(false);
                       VitalsButton.Timer = 5f;
                       RPCProcedure.Send(CustomRPC.PsychicAddCount);

                   }
               },
               () =>
               {
                   bool nearChosenSourcce = Hacker.CurrentTargetInformationSource == Hacker.InformationSource;

                   bool playerIsHacker = Hacker.Player == PlayerControl.LocalPlayer;
                   bool isAlive = !PlayerControl.LocalPlayer.Data.IsDead;
                   return isAlive && playerIsHacker && Hacker.InformationSource == InformationSource.Vitals && !nearChosenSourcce;
               },
               () =>
               {
                   Helpers.ShowTargetNameOnButtonExplicit(null, VitalsButton, $"VITALS - {(int)Hacker.DownloadTime}<size=70%>s</size>");
                   VitalsButton.Sprite = Hacker.GetVitalsSprite();
                   VitalsButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
                   VitalsButton.ActionButton.buttonLabelText.SetOutlineColor(Hacker.Color);
                   bool canMove = PlayerControl.LocalPlayer.CanMove;

                   return canMove && Hacker.DownloadTime > 0f;
               },
               () =>
               {
                   Hacker.RoundReset();
                   VitalsButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
                   VitalsButton.Sprite = Hacker.GetVitalsSprite();
                   Helpers.SetMovement(true);
                   Hacker.VitalsActive = false;
               },
                Hacker.GetVitalsSprite(),
                CustomButton.ButtonPositions.UpperRow3,
                "ActionQuaternary",
                false,
                2f,
               () =>
               {
                   VitalsButton.Timer = 3f;
                   Helpers.SetMovement(true);
                   VitalsButton.ActionButton.cooldownTimerText.SetFaceColor(Color.white);
               },
                false,
                "VITALS"
           );
        }

        public static void CamerasButtonInit()
        {
            CamerasButton = new CustomButton(() =>
            {
                if (Hacker.CamerasActive)
                    return;
                if (Helpers.IsMap(Map.Mira))
                {
                    if (Hacker.HackedMinigame == null)
                    {
                        SystemConsole e = Object.FindObjectsOfType<SystemConsole>().FirstOrDefault(x => x.gameObject.name.Contains("SurvLogConsole"));
                        if (e == null || Camera.main == null)
                            return;
                        Hacker.HackedMinigame = Object.Instantiate(e.MinigamePrefab, Camera.main.transform, false);
                    }
                    Hacker.HackedMinigame.transform.SetParent(Camera.main.transform, false);
                    Hacker.HackedMinigame.transform.localPosition = new Vector3(0.0f, 0.0f, -50f);
                    Hacker.HackedMinigame.Begin(null);
                }
                else
                {
                    if (Hacker.HackedMinigame == null)
                    {
                        byte mapId = GameOptionsManager.Instance.currentNormalGameOptions.MapId;
                        SystemConsole e = Object.FindObjectsOfType<SystemConsole>().FirstOrDefault(x => x.gameObject.name.Contains("Surv_Panel"));
                        if (mapId == 0 || mapId == 3)
                            e = Object.FindObjectsOfType<SystemConsole>().FirstOrDefault(x => x.gameObject.name.Contains("SurvConsole"));
                        else if (mapId == 4)
                            e = Object.FindObjectsOfType<SystemConsole>().FirstOrDefault(x => x.gameObject.name.Contains("task_cams"));
                        if (e == null || Camera.main == null)
                            return;
                        Hacker.HackedMinigame = Object.Instantiate(e.MinigamePrefab, Camera.main.transform, false);
                    }
                    Hacker.HackedMinigame.transform.SetParent(Camera.main.transform, false);
                    Hacker.HackedMinigame.transform.localPosition = new Vector3(0.0f, 0.0f, -50f);
                    Hacker.HackedMinigame.Begin(null);
                }

                Hacker.CamerasActive = true;

                Helpers.SetMovement(false);
                CamerasButton.Timer = 5f;
                RPCProcedure.Send(CustomRPC.PsychicAddCount);

            },
            () =>
            {
                bool nearChosenSourcce = Hacker.CurrentTargetInformationSource == Hacker.InformationSource;

                return
                    Hacker.Player == PlayerControl.LocalPlayer &&
                    !PlayerControl.LocalPlayer.Data.IsDead &&
                    !SubmergedCompatibility.IsSubmerged &&
                    Hacker.InformationSource == InformationSource.Cameras &&
                    !nearChosenSourcce;
            },
            () =>
            {
                CamerasButton.ActionButton.graphic.sprite = Helpers.IsMap(Map.Mira) ? Hacker.GetLogSprite() : Hacker.GetCamSprite();
                Helpers.ShowTargetNameOnButtonExplicit(null, CamerasButton, Helpers.IsMap(Map.Mira) ? $"DOORLOG - {(int)Hacker.DownloadTime}<size=70%>s</size>" : $"SECURITY - {(int)Hacker.DownloadTime}<size=70%>s</size>");
                CamerasButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
                CamerasButton.ActionButton.buttonLabelText.SetOutlineColor(Hacker.Color);
                return PlayerControl.LocalPlayer.CanMove && Hacker.DownloadTime > 0f;
            },
            () =>
            {

                Hacker.RoundReset();
                CamerasButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
                Helpers.SetMovement(true);
                Hacker.CamerasActive = false;
            },
            Hacker.GetCamSprite(),
            CustomButton.ButtonPositions.UpperRow3,
            "ActionQuaternary",
            false,
            2f,
            () =>
            {
                CamerasButton.Timer = 3f;
                Helpers.SetMovement(true);
                CamerasButton.ActionButton.cooldownTimerText.SetFaceColor(Color.white);
            },
            false,
            Helpers.IsMap(Map.Mira) ? "DOORLOG" : "SECURITY"
            );
        }

        public static void SetHackcerCooldown()
        {
            if (!Initialized)
            {
                InitHackerButtons();
            }
            AdminButton.Timer = 0f;
            VitalsButton.Timer = 0f;
            CamerasButton.Timer = 0f;
            DownloadButton.MaxTimer = 10f;
            JamButton.EffectDuration = Hacker.JamDuration;
            JamButton.MaxTimer = Hacker.JamDuration;
            JamButton.Timer = 0f;
        }
        public static void Postfix()
        {
            Initialized = false;
            InitHackerButtons();
        }
    }
}
