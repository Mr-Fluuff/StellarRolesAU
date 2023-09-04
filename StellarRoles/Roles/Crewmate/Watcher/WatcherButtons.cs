using HarmonyLib;
using StellarRoles.Objects;
using System;
using System.Linq;
using UnityEngine;

namespace StellarRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
    public static class WatcherButtons
    {
        private static bool Initialized;

        public static CustomButton WatcherCamsButton { get; set; }
        public static CustomButton WatcherSensorButton { get; set; }

        public static void InitWatcherButtons()
        {
            CamsButton();
            SensorButton();

            Initialized = true;
            SetWatcherCooldowns();
        }

        public static void CamsButton()
        {
            WatcherCamsButton = new CustomButton(
            () =>
            {
                if (!Watcher.IsActive)
                {
                    if (Watcher.CameraMinigame == null)
                    {
                        SystemConsole e = null;
                        if (Helpers.IsMap(Map.Polus))
                            e = UnityEngine.Object.FindObjectsOfType<SystemConsole>().FirstOrDefault(x => x.gameObject.name.Contains("Surv_Panel"));
                        else if (Helpers.IsMap(Map.Skeld) || Helpers.IsMap(Map.Dleks))
                            e = UnityEngine.Object.FindObjectsOfType<SystemConsole>().FirstOrDefault(x => x.gameObject.name.Contains("SurvConsole"));
                        else if (Helpers.IsMap(Map.Airship))
                            e = UnityEngine.Object.FindObjectsOfType<SystemConsole>().FirstOrDefault(x => x.gameObject.name.Contains("task_cams"));
                        else if (Helpers.IsMap(Map.Mira))
                            e = UnityEngine.Object.FindObjectsOfType<SystemConsole>().FirstOrDefault(x => x.gameObject.name.Contains("SurvLogConsole"));
                        else if (Helpers.IsMap(Map.Submerged))
                            e = UnityEngine.Object.FindObjectsOfType<SystemConsole>().FirstOrDefault(x => x.gameObject.name.Contains("SecurityConsole"));
                        if (e == null || Camera.main == null)
                            return;
                        Watcher.CameraMinigame = UnityEngine.Object.Instantiate(e.MinigamePrefab, Camera.main.transform, false);
                    }
                    Watcher.CameraMinigame.transform.SetParent(Camera.main.transform, false);
                    Watcher.CameraMinigame.transform.localPosition = new Vector3(0.0f, 0.0f, -50f);
                    Watcher.CameraMinigame.Begin(null);

                    Watcher.IsActive = true;
                    Watcher.BatteryTime--;

                    Helpers.SetMovement(false);
                    WatcherCamsButton.Timer = 5f;
                    RPCProcedure.Send(CustomRPC.PsychicAddCount);
                }
            },
            () =>
            {
                return Watcher.Player == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                WatcherCamsButton.ActionButton.graphic.sprite = Helpers.IsMap(Map.Mira) ? Watcher.GetLogSprite() : Watcher.GetCamSprite();
                Helpers.ShowTargetNameOnButtonExplicit(null, WatcherCamsButton, Helpers.IsMap(Map.Mira) ? $"DOORLOG - {(int)Watcher.BatteryTime}<size=70%>s</size>" : $"SECURITY - {(int)Watcher.BatteryTime}<size=70%>s</size>");
                WatcherCamsButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
                WatcherCamsButton.ActionButton.buttonLabelText.SetOutlineColor(Color.cyan);

                return PlayerControl.LocalPlayer.CanMove && !(Watcher.DisableRoundOneAccess && MapOptions.IsFirstRound) && Watcher.BatteryTime > 1.5;
            },
            () =>
            {
                Watcher.IsActive = false;
                WatcherCamsButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
                Helpers.SetMovement(true);
            },
            Watcher.GetCamSprite(),
            CustomButton.ButtonPositions.UpperRow1,
            "ActionQuaternary",
            false,
            2f,
            () =>
            {
                WatcherCamsButton.Timer = 3f;
                Helpers.SetMovement(true);
                WatcherCamsButton.ActionButton.cooldownTimerText.SetFaceColor(Color.white);
            },
            false,
            Helpers.IsMap(Map.Mira) ? "DOORLOG" : "SECURITY"
        );
        }

        public static void SensorButton()
        {
            WatcherSensorButton = new CustomButton(
               () =>
               {
                   Vector3 pos = PlayerControl.LocalPlayer.transform.position;
                   byte[] buff = new byte[sizeof(float) * 2];
                   Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0 * sizeof(float), sizeof(float));
                   Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1 * sizeof(float), sizeof(float));
                   RPCProcedure.PlaceSensor(buff);
                   SoundEffectsManager.Play(Sounds.Hammer);
                   WatcherSensorButton.Timer = 0.5f;
                   Watcher.SensorCount--;
                   RPCProcedure.Send(CustomRPC.PsychicAddCount);
               },
               () => Watcher.Player == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead && Watcher.SensorCount >= 1,
               () =>
               {
                   WatcherSensorButton.ActionButton.buttonLabelText.SetOutlineColor(Color.cyan);
                   Helpers.ShowTargetNameOnButtonExplicit(null, WatcherSensorButton, $"Sensors - {(int)Watcher.SensorCount}");
                   return PlayerControl.LocalPlayer.CanMove && !TrapperAbilities.IsRoleBlocked();
               },
               () =>
               {
                   RPCProcedure.Send(CustomRPC.ResetSensors);
                   Watcher.ResetSensors();
               },
               Watcher.GetTrapWireSprite(),
               CustomButton.ButtonPositions.UpperRow2,
               "SecondAbility"
           );
        }

        public static void SetWatcherCooldowns()
        {
            if (!Initialized)
            {
                InitWatcherButtons();
            }

            WatcherCamsButton.MaxTimer = 0f;
            WatcherSensorButton.MaxTimer = 0f;
        }

        public static void Postfix()
        {
            Initialized = false;
            InitWatcherButtons();
        }
    }
}
