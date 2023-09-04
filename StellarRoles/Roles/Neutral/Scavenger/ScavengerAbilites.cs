using HarmonyLib;
using StellarRoles.Objects;

namespace StellarRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class ScavengerAbilites
    {
        public static void Postfix(HudManager __instance)
        {
            if (!Helpers.GameStarted || PlayerControl.LocalPlayer != Scavenger.Player) return;

            scavengerDeadBodiesUpdate();
        }

        public static void scavengerDeadBodiesUpdate()
        {
            // Handle corpses tracking
            if (Scavenger.CorpsesTrackingTimer > 0f)
            {
                DeadBody[] deadBodies = UnityEngine.Object.FindObjectsOfType<DeadBody>();
                bool arrowsCountChanged = Scavenger.LocalArrows.Count != deadBodies.Length;
                int index = 0;

                if (arrowsCountChanged)
                {
                    foreach (Arrow arrow in Scavenger.LocalArrows) UnityEngine.Object.Destroy(arrow.Object);
                    Scavenger.LocalArrows.Clear();
                }
                foreach (DeadBody position in deadBodies)
                {
                    if (arrowsCountChanged)
                    {
                        Scavenger.LocalArrows.Add(new Arrow(Palette.ImpostorRed));
                        Scavenger.LocalArrows[index].Object.SetActive(true);
                    }
                    Scavenger.LocalArrows[index]?.Update(position.transform.position);
                    index++;
                }
            }
            else if (Scavenger.LocalArrows.Count > 0)
            {
                foreach (Arrow arrow in Scavenger.LocalArrows) UnityEngine.Object.Destroy(arrow.Object);
                Scavenger.LocalArrows.Clear();
            }

        }
    }
}
