using StellarRoles.Objects;
using StellarRoles.Utilities;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace StellarRoles
{
    public static class BountyHunter
    {
        public static PlayerControl Player { get; set; }
        public static PlayerControl CurrentTarget { get; set; }
        public static Color Color => IsNeutralKiller ? NeutralKiller.Color : Palette.ImpostorRed;

        public static Arrow Arrow { get; set; }
        public static float BountyDuration => CustomOptionHolder.BountyHunterBountyDuration.GetFloat();
        public static bool ShowArrow => CustomOptionHolder.BountyHunterShowArrow.GetBool();
        public static float BonusTime => CustomOptionHolder.BountyHunterReducedCooldown.GetFloat();
        public static float PunishmentTime => CustomOptionHolder.BountyHunterPunishmentTime.GetFloat();
        public static float ArrowUpdateIntervall => CustomOptionHolder.BountyHunterArrowUpdateIntervall.GetFloat();

        public static float ArrowUpdateTimer { get; set; } = 0f;
        public static float BountyUpdateTimer { get; set; } = 0f;
        public static PlayerControl Bounty { get; set; }
        public static TextMeshPro CooldownText { get; set; }

        public static bool IsNeutralKiller => CustomOptionHolder.BountyHunterIsNeutral.GetBool();

        public static void GetDescription()
        {
            RoleInfo.BountyHunter.SettingsDescription = RoleInfo.BountyHunterNeutralKiller.SettingsDescription = Helpers.WrapText(
                $"The Bounty Hunter's kill cooldown behaves different from other impostors.\n\n" +
                $"It is given a randomly selected target to kill every {Helpers.ColorString(Color.yellow, BountyDuration.ToString())} seconds. " +
                $"Killing this target will lower the cooldown of their next kill by {Helpers.ColorString(Color.yellow, BonusTime.ToString())}. " +
                $"Killing any other player will instead increase the cooldown by {Helpers.ColorString(Color.yellow, PunishmentTime.ToString())}.\n\n" +
                $"The Bounty Hunter has an arrow pointing to their bounty that updates their location every {Helpers.ColorString(Color.yellow, ArrowUpdateIntervall.ToString())} seconds.");
        }

        public static void ClearAndReload()
        {
            Player = null;
            Bounty = null;
            ArrowUpdateTimer = 0f;
            BountyUpdateTimer = 0f;
            if (Arrow != null) Object.Destroy(Arrow.Object);
            if (CooldownText != null) Object.Destroy(CooldownText.gameObject);
            CooldownText = null;
            Arrow = null;
            foreach (PoolablePlayer p in MapOptions.PlayerIcons.Values)
#pragma warning disable IDE0031 // Use null propagation 
                // suppressed due to Unity being weird and not liking optional chaining here
                if (p.gameObject != null) p.gameObject.SetActive(false);
#pragma warning restore IDE0031 // Use null propagation
        }

        public static List<PlayerControl> PossibleTargets()
        {
            bool isNeutralKiller = IsNeutralKiller;
            PlayerControl partner = Player.GetRomancePartner();
            return PlayerControl.AllPlayerControls.GetFastEnumerator().Where(player =>
            {
                GameData.PlayerInfo data = player.Data;
                return !(
                    data.IsDead || data.Disconnected || player == Player || player == Bounty ||
                    (!isNeutralKiller && (data.Role.IsImpostor || player == Spy.Player)) ||
                    (MapOptions.PlayersAlive >= 8 && data.Role.IsImpostor) ||
                    player == partner ||
                    MapOptions.FirstKillName == data.PlayerName);
            }).ToList();
        }
    }
}
