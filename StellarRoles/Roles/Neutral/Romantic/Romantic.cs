using StellarRoles.Objects;
using UnityEngine;

namespace StellarRoles
{
    public static class Romantic
    {
        public static readonly Color Color = new Color32(210, 60, 210, byte.MaxValue);

        public static PlayerControl Player { get; set; }
        public static PlayerControl Lover { get; set; }
        // TODO: make this a getter
        public static bool HasLover { get; set; } = false;
        public static PlayerControl CurrentTarget { get; set; }
        public static Arrow Arrow { get; set; } = new(Color);
        public static float Cooldown => CustomOptionHolder.RomanticProtectCooldown.GetFloat();
        public static float VestDuration => CustomOptionHolder.RomanticProtectDuration.GetFloat();
        public static bool NeutralSided { get; set; } = false;
        public static bool RomanticKnowsRole { get; set; } = false;
        public static bool KnowsRoleInfoImmediately => CustomOptionHolder.RomanticKnowsTargetRoleWhen.GetSelection() == 0;
        public static bool IsVestActive { get; set; } = false;
        public static bool IsArsonist { get; set; } = false;
        public static bool IsJester { get; set; } = false;
        public static bool IsScavenger { get; set; } = false;
        public static bool IsExecutioner { get; set; } = false;
        public static bool IsHeadHunter { get; set; } = false;
        public static bool IsPyromaniac { get; set; } = false;
        public static bool IsCrewmate { get; set; } = false;
        public static bool IsImpostor { get; set; } = false;
        public static bool PartnerSeesLoveInstantly => CustomOptionHolder.RomanticLoverSeesLove.GetSelection() == 2;
        public static bool PartnerSeesLoveAfterMeeting => CustomOptionHolder.RomanticLoverSeesLove.GetSelection() == 1;
        public static bool SetNameFirstMeeting { get; set; } = false;
        public static bool DieOnAllImpsDead => CustomOptionHolder.RomanticOnAllImpsDead.GetSelection() == 0;
        public static bool TurnOffRomanticToRefugee => CustomOptionHolder.TurnOffRomanticToRefugee.GetBool();
        public static bool PairIsDead { get; set; } = false;

        private static Sprite _ProtectButtonSprite;
        private static Sprite _AliveMeetingOverlay;
        private static Sprite _DeadMeetingOverlay;
        private static Sprite _RomanceButtonSprite;
        private static Sprite _AliveToolTip;
        private static Sprite _DeadToolTip;
        private static Sprite _KillButtonSprite;

        public static void GetDescription()
        {
            string settingsDescription =
                $"The goal of the {nameof(Romantic)} is to choose a player during a round and help that player win the game at all costs.\n\n" +
                $"Choose someone quickly! An undecided {nameof(Romantic)} will turn into a {nameof(Refugee)} when 7 players remain alive.\n\n" +
                $"After you choose a player you gain the ability to protect them from attacks for {Helpers.ColorString(Color.yellow, VestDuration.ToString())} seconds on a {Helpers.ColorString(Color.yellow, Cooldown.ToString())} second cooldown.";

            if (PartnerSeesLoveInstantly)
                settingsDescription += "\n\nYour target will see they are chosen immediately. ";
            else if (PartnerSeesLoveAfterMeeting)
                settingsDescription += "\n\nBe careful! Your target will not know that they were the one who was selected until the next meeting. ";

            settingsDescription +=
                $"\n\nIn the event your love dies, your role will change based on what their role was. " +
                $"If they were a crewmate or impostor, you become the Vengeful {nameof(Romantic)}, if they were a Neutral, you will take their role " +
                $"to finish their objective! If they were a Neutral Killer, you become the Ruthless {nameof(Romantic)}!";

            RoleInfo.Romantic.SettingsDescription = Helpers.WrapText(settingsDescription);
        }
        public static Sprite GetRomanceButtonSprite()
        {
            return _RomanceButtonSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.Romantic.Romance.png", 115f);
        }

        public static Sprite GetKillButtonSprite()
        {
            return _KillButtonSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.PinkKillButton.png", 115f);
        }

        public static Sprite GetProtectButtonSprite()
        {
            return _ProtectButtonSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.Romantic.RomanticProtect.png", 115f);
        }

        public static Sprite GetAliveToolTip()
        {
            return _AliveToolTip ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.Romantic.RomanticAliveTip.png", 350f);
        }

        public static Sprite GetDeadToolTip()
        {
            return _DeadToolTip ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.Romantic.RomanticDeadTip.png", 350f);
        }

        public static Sprite GetRomanticAliveMeetingOverlay()
        {
            return _AliveMeetingOverlay ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.Romantic.RomanticHeart.png", 250f);
        }

        public static Sprite GetRomanticDeadMeetingOverlay()
        {
            return _DeadMeetingOverlay ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.Romantic.RomanticBrokenHeart.png", 250f);
        }

        public static bool CanBeKilledBySheriff()
        {
            return Lover != null && Sheriff.CanBeKilledBySheriff(Lover);
        }

        public static void ClearAndReload()
        {
            Player = null;
            Lover = null;
            Object.Destroy(Arrow.Object);
            Arrow = new Arrow(Color);
            Arrow.Object.SetActive(false);
            HasLover = false;
            CurrentTarget = null;

            RomanticKnowsRole = true;
            IsVestActive = false;
            PairIsDead = false;

            ResetAlignment();
        }

        public static void ResetAlignment()
        {
            IsArsonist = false;
            IsJester = false;
            IsScavenger = false;
            IsExecutioner = false;
            IsHeadHunter = false;
            IsCrewmate = false;
            IsImpostor = false;
            SetNameFirstMeeting = false;
            IsPyromaniac = false;
            NeutralSided = false;
        }
    }

    public static class VengefulRomantic
    {
        public static readonly Color Color = new Color32(210, 30, 70, byte.MaxValue);
        public static readonly Color RevengeColor = new Color32(153, 3, 36, byte.MaxValue);

        public static PlayerControl Player { get; set; }
        public static PlayerControl Target { get; set; }
        public static PlayerControl Lover { get; set; }
        public static PlayerControl CurrentTarget { get; set; }
        public static bool AvengedLover { get; set; } = false;
        public static bool IsCrewmate { get; set; } = false;
        public static bool IsImpostor { get; set; } = false;
        public static bool IsDisconnected { get; set; } = false;

        public static void GetDescription()
        {
            RoleInfo.VengefulRomantic.SettingsDescription = Helpers.WrapText(
                $"The Vengeful {nameof(Romantic)} was once a normal {nameof(Romantic)}, but their love has perished!\n\n" +
                $"The objective of the Vengeful {nameof(Romantic)} is the same as the normal {nameof(Romantic)}. " +
                $"Do whatever you can to make sure your fallen love still wins the game!\n\n" +
                $"If your fallen love was murdered or assassinated, you will be given a kill button! " +
                $"The kill button can only be used on the exact player that assassinated or murdered your fallen love. " +
                $"If you attempt to use it on any other player, you will die instead!");
        }

        public static bool CanBeKilledBySheriff()
        {
            return Lover != null && Sheriff.CanBeKilledBySheriff(Lover);
        }

        public static void ClearAndReload()
        {
            Player = null;
            Target = null;
            Lover = null;
            AvengedLover = false;
            IsCrewmate = false;
            IsImpostor = false;
            IsDisconnected = false;
        }
    }


    public static class Beloved
    {
        public static readonly Color Color = new Color32(210, 60, 210, byte.MaxValue);
        public static PlayerControl Player { get; set; }
        public static PlayerControl Romantic { get; set; }
        public static bool WasArsonist { get; set; } = false;
        public static bool EasExecutioner { get; set; } = false;
        public static bool WasJester { get; set; } = false;
        public static bool WasScavenger { get; set; } = false;
        public static bool WasRefugee { get; set; } = false;
        public static bool WasNK { get; set; } = false;

        /// <summary>
        /// Unused function but kept for information anyway
        /// </summary>
        public static void GetDescription()
        {
            RoleInfo.Beloved.SettingsDescription = Helpers.WrapText(
                "You have perished, but your Romantic partner might be able to finish your objective!");
        }

        public static void ClearAndReload()
        {
            Player = null;
            Romantic = null;
            WasArsonist = false;
            EasExecutioner = false;
            WasJester = false;
            WasScavenger = false;
            WasRefugee = false;
            WasNK = false;
        }
    }
}
