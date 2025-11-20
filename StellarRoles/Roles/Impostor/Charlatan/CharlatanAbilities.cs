using HarmonyLib;
using StellarRoles.Objects;
using StellarRoles.Utilities;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using static UnityEngine.GraphicsBuffer;

namespace StellarRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class CharlatanAbilities
    {
        public static void Postfix(HudManager __instance)
        {
            if (!Helpers.GameStarted || PlayerControl.LocalPlayer != Charlatan.Player) return;
            CharlatanSetTarget();
            CharlatanUpdate();
        }

        public static void CharlatanSetTarget()
        {
            Charlatan.DeadBodies = Helpers.DeadBodies(Charlatan.ConcealRange, Charlatan.ConcealedBodies);
        }

        public static void CharlatanUpdate() 
        {
            Charlatan.DeceiveTimer -= Time.deltaTime;

            if (Charlatan.DeceiveTimer <= 0 && Charlatan.ReportTarget != byte.MaxValue)
            {
                Charlatan.ReportTarget = byte.MaxValue;
            }
        }

        public static void CharlatanSetDeceiveTarget(this PlayerControl target)
        {
            Charlatan.ReportTarget = target.PlayerId;
            Helpers.Log("SetDeceiveTarget");
        }

        public static void ConcealBodiesInRange()
        {
            foreach(var body in Charlatan.DeadBodies)
            {
                RPCConcealBody(body);
            }
        }

        public static void RPCConcealBody(DeadBody body)
        {
            RPCProcedure.ConcealBody(body.ParentId);
            RPCProcedure.Send(CustomRPC.ConcealBody, body);
        }

        public static void CheckKill(PlayerControl target) 
        {
            if (PlayerControl.LocalPlayer != Charlatan.Player) return;

            Charlatan.ConcealCharges += Charlatan.ConcealChargesPerKill;
            Charlatan.DeceiveTimer = Charlatan.MaxDeceiveTimer;
            Charlatan.MaxDeceiveTimer += Charlatan.DeceiveIncPerKill;
            target.CharlatanSetDeceiveTarget();

            CharlatanButtons.ConcealButton.Timer = 5f;
        }

        public static void DeceiveAbility() 
        {
            if (Charlatan.ReportTarget == byte.MaxValue) return;

            var bodies = Object.FindObjectsOfType<DeadBody>().Where(x => x.ParentId == Charlatan.ReportTarget);
            if (bodies.Any())
            {
                foreach (var body in bodies)
                {
                    body.Reported = true;
                }
            }
            else
            {
                Charlatan.ReportTarget = byte.MaxValue;
                Charlatan.DeceiveTimer = 0;
                return;
            }
            NetworkedPlayerInfo playerById = GameData.Instance.GetPlayerById(Charlatan.ReportTarget);
            PlayerControl.LocalPlayer.CmdReportDeadBody(playerById);
        }
    }
}
