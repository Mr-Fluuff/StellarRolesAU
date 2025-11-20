using HarmonyLib;
using Reactor.Utilities.Extensions;
using UnityEngine;

namespace StellarRoles
{
    [HarmonyPatch]
    public static class Parasite
    {
        public static PlayerControl Player { get; set; } = null;
        public static PlayerControl CurrentTarget { get; set; } = null;
        public static PlayerControl Controlled { get; set; } = null;
        public static Vector2 Position { get; set; } = new Vector2();


        public static float InfestCooldown => CustomOptionHolder.ParasiteInfestCooldown.GetFloat();
        public static float ControlDuration => CustomOptionHolder.ParasiteControlDuration.GetFloat();
        public static bool CanSaveInfested => CustomOptionHolder.ParasiteSaveInfested.GetBool();
        public static bool NormalKillButton => CustomOptionHolder.ParasiteNormalKillButton.GetBool();
        public static bool Unlimited => !CustomOptionHolder.ParasiteControlDuration.GetBool();


        public static Color Color => IsNeutralKiller ? NeutralKiller.Color : Palette.ImpostorRed;
        public static bool IsNeutralKiller => CustomOptionHolder.ParasiteIsNeutral.GetBool();
        public static float ControlTimer = 10f;
        public static float InfestedTimer = 10f;

        private static Sprite _ControlButtonSprite;
        private static Sprite _KillInfestButtonSprite;

        public static void GetDescription()
        {
            string save = !CanSaveInfested ? ", the Parasite dies," : "";
            string timer = Unlimited ? "∞" : ControlDuration.ToString();
            string notsave = CanSaveInfested ? " The target is saved if the Parasite dies." : "";
            string normalKill = NormalKillButton ? "Using Infest will put kill on 75% of its normal cooldown, and if the target player dies for any reason kill will be put on 75% of its normal cooldown again." : "The Parasite cannot kill normally.";
            string info = $"The Parasite is a role that can take control of another player to kill them in another location. " +
                $"\n\nInfest is the first ability. While infested a player will look like the Parasite, cannot perform any actions, and their movement is controlled by the Parasite. " +
                $"Default keys for the victim's movement are I, J, K, and L but these can be changed in key bind settings." +
                $"\n\nOnce infested, Decay can be used to kill the targeted player. If infest duration expires{save} or a meeting is called the infested player will die.{notsave}" +
                $"\n\nInfest has a {Helpers.ColorString(Color.yellow, InfestCooldown.ToString())} second cooldown and lasts {Helpers.ColorString(Color.yellow, timer)} seconds. " +
                $"The first cooldown per round cannot be less than standard kill cooldown. {normalKill}";
        
            RoleInfo.Parasite.SettingsDescription = RoleInfo.ParasiteNeutralKiller.SettingsDescription = Helpers.WrapText(info);
        }

        public static Sprite GetControlSprite()
        {
            return _ControlButtonSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.ParasiteControl.png", 115f);
        }

        public static Sprite GetKillInfestSprite()
        {
            return _KillInfestButtonSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.ParasiteDecay.png", 115f);
        }

        public static void ClearAndReload()
        {
            Player = null;
            CurrentTarget = null;
            Controlled = null;
            ControlTimer = ControlDuration;
            ParasiteAbilites.DestroyAssets();
        }
    }
}
