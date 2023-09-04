using System.Collections.Generic;
using UnityEngine;

namespace StellarRoles
{
    public class Jester
    {
        public static readonly Dictionary<byte, Jester> JesterDictionary = new();
        public static readonly Color Color = new Color32(236, 98, 165, byte.MaxValue);

        public static float LightsOnVision => CustomOptionHolder.JesterLightsOnVision.GetFloat();
        public static float LightsOffVision => CustomOptionHolder.JesterLightsOffVision.GetFloat();
        public static bool CanCallEmergency => CustomOptionHolder.JesterCanCallEmergency.GetBool();
        public static bool CanEnterVents => CustomOptionHolder.JesterCanEnterVents.GetBool();
        public static PlayerControl WinningJesterPlayer { get; set; }
        public static bool TriggerJesterWin { get; set; } = false;

        private static Sprite _VentButtonSprite;


        public readonly PlayerControl Player;
        public bool WasExecutioner = false;
        public static Sprite GetVentButtonSprite()
        {
            return _VentButtonSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.VentButtons.JesterVent.png", 115f);
        }

        public static void GetDescription()
        {
            string settingsDescription =
                $"The {nameof(Jester)}'s goal is to be voted out. Be careful how you play! " +
                $"An {nameof(Assassin)} or {nameof(Vigilante)} may catch on and can kill you in meeting. " +
                $"The crew may also begin to trust you too much if you oppose the same players as they do!";

            if (!CanCallEmergency)
                settingsDescription += $"\n\nThe {nameof(Jester)} is unable to call an Emergency Meeting.";

            settingsDescription +=
                $"\n\nThe {nameof(Jester)} has unique vision settings. During a lights sabotage, they have regular {Helpers.ColorString(Color.yellow, LightsOffVision.ToString())}x vision. " +
                $"Their vision is {Helpers.ColorString(Color.yellow, LightsOnVision.ToString())}x otherwise.";

            if (CanEnterVents)
                settingsDescription += "\n\nThe Jester may enter vents, but not travel through them.";

            RoleInfo.Jester.SettingsDescription = Helpers.WrapText(settingsDescription);
        }

        public Jester(PlayerControl player)
        {
            Player = player;
            JesterDictionary.Add(player.PlayerId, this);
        }

        public static bool IsJester(byte playerId, out Jester jester)
        {
            return JesterDictionary.TryGetValue(playerId, out jester);
        }

        public static bool IsJester(PlayerControl player)
        {
            return Jester.JesterDictionary.ContainsKey(player.PlayerId);
        }


        public static void RemoveJester(byte playerId)
        {
            JesterDictionary.Remove(playerId);
        }

        public static void ClearAndReload()
        {
            JesterDictionary.Clear();
            WinningJesterPlayer = null;
            TriggerJesterWin = false;
        }
    }

    public static class JesterExtenstions
    {
        public static bool IsJester(this PlayerControl player, out Jester jester) => Jester.IsJester(player.PlayerId, out jester);
    }
}
