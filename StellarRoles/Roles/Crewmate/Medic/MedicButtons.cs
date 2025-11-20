using HarmonyLib;
using StellarRoles.Objects;
using System.Linq;
using UnityEngine;

namespace StellarRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
    public static class MedicButtons
    {
        private static bool Initialized;

        public static CustomButton MedicVitals { get; set; }
        public static CustomButton MedicHeartMonitor { get; set; }

        public static void InitMedicButtons()
        {
            MedicVitalsButton();
            MedicHeartMonitorButton();

            Initialized = true;
            SetMedicCooldowns();
        }
        private static void MedicHeartMonitorButton()
        {
            MedicHeartMonitor = new CustomButton(
               () =>
               {
                   RPCProcedure.Send(CustomRPC.MedicSetHearMonitor, Medic.CurrentTarget);
                   RPCProcedure.MedicSetHearMonitor(Medic.CurrentTarget);
                   SoundEffectsManager.Play(Sounds.Monitor);
                   MedicHeartMonitor.Timer = MedicHeartMonitor.MaxTimer;
                   RPCProcedure.Send(CustomRPC.PsychicAddCount);

               },
               () =>
               {
                   return Medic.Player == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead && !Medic.UsedHeartMonitor;
               },
               () =>
               {

                   MedicHeartMonitor.ActionButton.graphic.sprite = Medic.GetHeartMonitorSprite();
                   Helpers.ShowTargetNameOnButtonExplicit(null, MedicHeartMonitor, "MONITOR");
                   MedicHeartMonitor.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
                   MedicHeartMonitor.ActionButton.buttonLabelText.SetOutlineColor(Color.cyan);

                   return PlayerControl.LocalPlayer.CanMove && Medic.CurrentTarget != null && !Medic.UsedHeartMonitor && !MedicAbilities.isRoleBlocked();
               },
               () =>
               {
                   Medic.ResetHeartMonitor();
                   MedicHeartMonitor.Timer = 5f;
               },
               Medic.GetHeartMonitorSprite(),
               CustomButton.ButtonPositions.UpperRow2,
               "SecondAbility",
               false,
               0f,
               () => MedicHeartMonitor.Timer = MedicHeartMonitor.MaxTimer,
               false,
               "Monitor"
           );
        }

        private static void MedicVitalsButton()
        {
            MedicVitals = new CustomButton(
               () =>
               {
                   if (!Medic.IsActive)
                   {
                       SystemConsole e = Object.FindObjectsOfType<SystemConsole>().FirstOrDefault(x => x.gameObject.name.Contains("panel_vitals"));
                       if (e == null)
                           e = Object.FindObjectsOfType<SystemConsole>().FirstOrDefault(x => x.gameObject.name.Contains("VitalsConsole"));
                       if (e == null || Camera.main == null)
                           return;
                       Medic.IsActive = true;
                       Medic.Battery--;
                       MedicVitals.EffectDuration = Medic.Battery;
                       MedicVitals.Timer = Medic.Battery;
                       MedicVitals.MaxTimer = Medic.Battery;
                       Medic.VitalsMinigame = Object.Instantiate(e.MinigamePrefab, Camera.main.transform, false);
                       Medic.VitalsMinigame.transform.SetParent(Camera.main.transform, false);

                       Medic.VitalsMinigame.transform.localPosition = new Vector3(0.0f, 0.0f, -50f);
                       Medic.VitalsMinigame.Begin(null);

                       Helpers.SetMovement(false);
                       SoundEffectsManager.Play(Sounds.Vitals);
                       MedicVitals.Timer = 5f;
                       RPCProcedure.Send(CustomRPC.PsychicAddCount);
                   }

               },
               () =>
               {
                   return Medic.Player == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead;
               },
               () =>
               {
                   Helpers.ShowTargetNameOnButtonExplicit(null, MedicVitals, $"VITALS - {(int)Medic.Battery}<size=70%>s</size>");
                   MedicVitals.Sprite = Medic.GetVitalsSprite();
                   MedicVitals.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
                   MedicVitals.ActionButton.buttonLabelText.SetOutlineColor(Color.cyan);
                   return PlayerControl.LocalPlayer.CanMove && !(Medic.DisableRoundOneAccess && MapOptions.IsFirstRound) && Medic.Battery > 1.5;
               },
               () =>
               {
                   Medic.IsActive = false;
                   MedicVitals.Timer = 0f;
                   MedicVitals.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
                   MedicVitals.Sprite = Medic.GetVitalsSprite();
                   Helpers.SetMovement(true);
               },
                Medic.GetVitalsSprite(),
                CustomButton.ButtonPositions.UpperRow1,
                "ActionQuaternary",
                false,
                2f,
               () =>
               {
                   MedicVitals.Timer = 3f;
                   Helpers.SetMovement(true);
                   MedicVitals.ActionButton.cooldownTimerText.SetFaceColor(Color.white);
               },
                false,
                "VITALS"
           );
        }

        public static void SetMedicCooldowns()
        {
            if (!Initialized)
            {
                InitMedicButtons();
            }

            MedicVitals.Timer = 0f;
            MedicHeartMonitor.Timer = 0f;
            MedicHeartMonitor.MaxTimer = 0f;
            MedicVitals.MaxTimer = 0f;
        }

        public static void Postfix()
        {
            Initialized = false;
            InitMedicButtons();
        }
    }
}
