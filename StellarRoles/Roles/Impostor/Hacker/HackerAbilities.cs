using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;



namespace StellarRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HackerAbilities
    {
        static void HackerUpdate()
        {
            if (Hacker.IsDownloading && Hacker.RemainingDownloadTime > 0f && Hacker.CurrentTargetInformationSource != InformationSource.None)
            {
                float dt = Time.deltaTime;
                Hacker.RemainingDownloadTime -= dt;
                Hacker.DownloadTime += dt * Hacker.CalculateHackerMultiplier();
            }
            else
            {
                Hacker.IsDownloading = false;
            }

            if (Hacker.AdminActive || Hacker.CamerasActive || Hacker.VitalsActive) Hacker.DownloadTime -= Time.deltaTime;

            if (Hacker.AdminActive && (MapBehaviour.Instance == null || !MapBehaviour.Instance.isActiveAndEnabled || Hacker.DownloadTime <= 0f))
            {
                Helpers.SetMovement(true);
                MapBehaviour.Instance.Close();
                Hacker.AdminActive = false;
            }
            if (Hacker.CamerasActive && (Minigame.Instance == null || Hacker.DownloadTime <= 0f))
            {
                Helpers.SetMovement(true);
                Hacker.HackedMinigame.Close();
                Hacker.CamerasActive = false;
            }
            if (Hacker.VitalsActive && (Minigame.Instance == null || Hacker.DownloadTime <= 0f))
            {
                Helpers.SetMovement(true);
                Hacker.VitalsActive = false;
                Hacker.HackedMinigame.ForceClose();
            }
        }


        public static void hackerSetTarget()
        {
            bool betterPolusVitals = CustomOptionHolder.MoveVitals.GetBool();
            Vector2 truePosition = PlayerControl.LocalPlayer.GetTruePosition();
            List<InformationSourceDetails> allInformationSources = InformationSourceDetails.BuildAllInformationSources(betterPolusVitals);
            Hacker.CurrentTargetInformationSource = InformationSource.None;

            foreach (InformationSourceDetails details in allInformationSources)
            {
                if (!Helpers.IsMap(details.MapType)) continue;

                float distance = Vector2.Distance(details.Location, truePosition);
                if (distance < details.Range && !PhysicsHelpers.AnythingBetween(details.Location, truePosition, Constants.ShadowMask, false))
                {
                    Hacker.CurrentTargetInformationSource = details.InformationSource;
                    break;
                }
            }
        }


        public static void Postfix(HudManager __instance)
        {
            if (!Helpers.GameStarted || PlayerControl.LocalPlayer != Hacker.Player) return;

            HackerUpdate();
            hackerSetTarget();
        }
    }
}
