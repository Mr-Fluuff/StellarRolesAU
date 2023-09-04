using UnityEngine;

namespace StellarRoles
{
    public enum GainsAssassin
    {
        Never,
        ImpWipe,
        GameStart
    }
    public static class Assassin
    {
        public static readonly PlayerList Players = new();
        public static int RemainingShotsAssassin { get; set; } = 2;
        public static bool AssassinLimitOneShotPerMeeting => CustomOptionHolder.AssassinMultipleShotsPerMeeting.GetBool();
        public static bool AssassinGuesserCanGuessSpy => CustomOptionHolder.AssassinCanKillSpy.GetBool();
        public static bool AssassinCanGuessCrew => CustomOptionHolder.AssassinCanGuessCrewmate.GetBool();
        public static bool NeutralKillerAssassinCanGuessCrew => CustomOptionHolder.NeutralKillerAssassinCanGuessCrewmate.GetBool();
        public static bool NeutralKillerAssassinLimitOneShotPerMeeting => CustomOptionHolder.NeutralKillerAssassinMultipleShotsPerMeeting.GetBool();
        public static int RemainingShotsNK { get; set; } = 2;
        public static GainsAssassin NeutralKillerGetsAssassin => (GainsAssassin)CustomOptionHolder.NeutralKillerGainsAssassin.GetSelection();

        private static Sprite _TargetSprite;

        public static Sprite GetTargetSprite()
        {
            return _TargetSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.TargetIcon.png", 150f);
        }

        public static void ClearAndReload()
        {
            Players.Clear();
            RemainingShotsAssassin = CustomOptionHolder.AssassinNumberOfShots.GetSelection();
            RemainingShotsNK = CustomOptionHolder.NeutralKillerAssassinNumberOfShots.GetSelection();
        }

        public static bool NeutralKillersHaveAssassin()
        {
            return NeutralKillerGetsAssassin switch
            {
                GainsAssassin.ImpWipe => MapOptions.Allimpsdead,
                GainsAssassin.Never => false,
                GainsAssassin.GameStart => true,
                _ => throw new System.NotImplementedException()
            };
        }
    }
}