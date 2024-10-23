using StellarRoles.Utilities;
using System.Linq;
using UnityEngine;

namespace StellarRoles
{
    public static class Arsonist
    {
        public static PlayerControl Player { get; set; } = null;
        public static readonly Color Color = new Color32(238, 112, 46, byte.MaxValue);
        public static float Cooldown => CustomOptionHolder.ArsonistCooldown.GetFloat();
        public static float Duration => CustomOptionHolder.ArsonistDuration.GetFloat();
        public static float RoundCooldown => CustomOptionHolder.ArsonistDouseIgniteRoundCooldown.GetFloat();
        public static bool TriggerArsonistWin { get; set; } = false;

        public static PlayerControl CurrentTarget { get; set; } = null;
        public static PlayerControl DouseTarget { get; set; } = null;
        public static readonly PlayerList DousedPlayers = new();

        private static Sprite _DouseSprite;

        public static void GetDescription()
        {
            string settingsDescription =
                $"The goal of the {nameof(Arsonist)} is to tag every living player with a douse, then burn them all at once with an ignite!\n\n" +
                $"The {nameof(Arsonist)}'s first douse in a round starts at {Helpers.ColorString(Color.yellow, RoundCooldown.ToString())} seconds, but they may douse every {Helpers.ColorString(Color.yellow, Cooldown.ToString())} seconds after that.\n\n" +
                $"The {nameof(Arsonist)} must stay near a player for {Helpers.ColorString(Color.yellow, Duration.ToString())} second(s) to douse them.";

            if (IsPyromaniacEnabled() || !Helpers.GameStarted)
                settingsDescription +=
                    $"\n\nIf a {nameof(Pyromaniac)} Neutral-Killing role has spawned, the {nameof(Arsonist)} can attempt to assist them and win with them by being the final two players alive. " +
                    $"Be careful when trying to make an alliance! This is a ruthless killing role with their own goals to achieve!";

            RoleInfo.Arsonist.SettingsDescription = Helpers.WrapText(settingsDescription);
        }
        public static Sprite GetDouseSprite()
        {
            return _DouseSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.DouseButton.png", 115f);
        }

        private static Sprite _IgniteSprite;
        public static Sprite GetIgniteSprite()
        {
            return _IgniteSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.IgniteButton.png", 115f);
        }

        public static bool DousedEveryoneAlive()
        {
            return PlayerControl.AllPlayerControls.GetFastEnumerator().All(
                player => player == Player || DousedPlayers.Contains(player) || player.Data.IsDead || player.Data.Disconnected
            );
        }

        public static bool IsPyromaniacEnabled()
        {
            return CustomOptionHolder.PyromaniacSpawnRate.GetSelection() > 0 && CustomOptionHolder.NeutralKillerRolesCountMin.GetFloat() > 0;
        }

        public static void ClearAndReload()
        {
            Player = null;
            CurrentTarget = null;
            DouseTarget = null;
            TriggerArsonistWin = false;
            DousedPlayers.Clear();
            foreach (PoolablePlayer p in MapOptions.PlayerIcons.Values)
#pragma warning disable IDE0031 // Use null propagation
                // Unity is weird here and doesn't like null propagation on GameObjects
                if (p.gameObject != null) p.gameObject.SetActive(false);
#pragma warning restore IDE0031 // Use null propagation
        }
    }
}
