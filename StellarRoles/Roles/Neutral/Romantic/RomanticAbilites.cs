using HarmonyLib;
using StellarRoles.Patches;
using System.Linq;
using UnityEngine;

namespace StellarRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class RomanticAbilites
    {
        public static void Postfix(HudManager __instance)
        {
            if (!Helpers.GameStarted || Helpers.IsHideAndSeek) return;

            RomanticCheck();
            VengfulRomantic();
        }

        static void RomanticCheck()
        {
            if (PlayerControl.LocalPlayer != Romantic.Player) return;

            Romantic.CurrentTarget = Helpers.SetTarget();
            RomanticUpdate();
        }

        static void VengfulRomantic()
        {
            if (PlayerControl.LocalPlayer != VengefulRomantic.Player) return;
            VengefulRomanticSetTarget();
        }

        static void RomanticUpdate()
        {
            if (Romantic.Lover == null || Romantic.Player.Data.IsDead || (MapOptions.NeutralRoleBlock && Helpers.IsCommsActive()))
            {
                Romantic.Arrow.Object.SetActive(false);
                return;
            }

            bool trackedOnMap = !Romantic.Lover.Data.IsDead;
            Vector3 position = Romantic.Lover.transform.position;
            if (!trackedOnMap)
            {
                DeadBody body = Object.FindObjectsOfType<DeadBody>().FirstOrDefault(b => b.ParentId == Romantic.Lover.PlayerId);
                if (body != null)
                {
                    trackedOnMap = true;
                    position = body.transform.position;
                }
            }

            Romantic.Arrow.Update(position);
            Romantic.Arrow.Object.SetActive(trackedOnMap);
        }

        public static bool RomanticRoleUpdate(bool afterMeeting)
        {
            if (AmongUsClient.Instance.AmHost && !Romantic.PairIsDead && Romantic.Lover != null && (Romantic.Player == null || Romantic.Player.Data.Disconnected))
            {
                Helpers.LoverPairDead();
            }

            if (PlayerControl.LocalPlayer != Romantic.Player) return false;

            if (MapOptions.PlayersAlive <= 7 && !Romantic.HasLover && !Romantic.TurnOffRomanticToRefugee)
            {
                RPCProcedure.Send(CustomRPC.RomanticTurnToRefugee);
                RPCProcedure.RomanticToRefugee();
                return false;
            }

            bool vengful = false;
            if (Romantic.HasLover)
            {
                if (Romantic.Player.Data.IsDead && !Romantic.PairIsDead)
                {
                    Helpers.LoverPairDead();
                }
                if ((Romantic.Lover.Data.IsDead || Romantic.Lover.Data.Disconnected) && !Romantic.Player.Data.IsDead)
                {
                    if (Romantic.IsImpostor && Romantic.DieOnAllImpsDead && MapOptions.Allimpsdead)
                    {
                        if (afterMeeting)
                            Helpers.ExilePlayer(Romantic.Player);
                        else
                            Helpers.UncheckedMurderPlayer(Romantic.Player, Romantic.Player, false);
                    }
                    else if (Romantic.NeutralSided && !Jester.TriggerJesterWin)
                    {
                        RPCProcedure.Send(CustomRPC.BecomeLoversRole);
                        RPCProcedure.BecomeLoversRole();
                    }
                    else if (!afterMeeting)
                    {
                        RPCProcedure.Send(CustomRPC.BecomeVengefulLover);
                        RPCProcedure.BecomeVengefulLover();
                        vengful = true;
                    }
                    Helpers.LoverPairDead();
                }
            }
            return vengful;
        }

        public static void VengefulRomanticSetTarget()
        {
            if (VengefulRomantic.Player == null) return;
            VengefulRomantic.CurrentTarget = Helpers.SetTarget(canIncrease: true);
        }

        public static void VengefulRoleUpdate(bool afterMeeting)
        {
            if (PlayerControl.LocalPlayer != VengefulRomantic.Player || PlayerControl.LocalPlayer.Data.IsDead) return;
            if (VengefulRomantic.Lover.Data.Disconnected) VengefulRomantic.IsDisconnected = true;
            if (!VengefulRomantic.IsImpostor) return;

            if (!MapOptions.Allimpsdead) return;

            if (Romantic.DieOnAllImpsDead)
            {
                if (afterMeeting)
                    Helpers.ExilePlayer(VengefulRomantic.Player);
                else
                    Helpers.UncheckedMurderPlayer(VengefulRomantic.Player, VengefulRomantic.Player, false);
            }
            else
            {
                RPCProcedure.Send(CustomRPC.VengefulRomanticToRefugee);
                RPCProcedure.VengefulRomanticToRefugee();
            }
        }
    }
}
