using AmongUs.GameOptions;
using System;

namespace StellarRoles
{
    public static class StellarRoles
    {
        public static Random rnd => new((int)DateTime.Now.Ticks);
        public static NormalGameOptionsV08 NormalOptions => GameOptionsManager.Instance.currentNormalGameOptions;


        public static void ClearAndReloadRoles()
        {
            PlayerGameInfo.ClearAndReload();
            Spectator.ClearAndReload();

            Administrator.ClearAndReload();
            Arsonist.ClearAndReload();
            Beloved.ClearAndReload();
            Bombed.ClearAndReload();
            Bomber.ClearAndReload();
            Camouflager.ClearAndReload();
            Scavenger.ClearAndReload();
            Changeling.ClearAndReload();
            Cultist.ClearAndReload();
            Detective.ClearAndReload();
            Engineer.ClearAndReload();
            Executioner.ClearAndReload();
            Follower.ClearAndReload();
            Guardian.ClearAndReload();
            Assassin.ClearAndReload();
            Vigilante.ClearAndReload();
            HeadHunter.ClearAndReload();
            Impostor.ClearAndReload();
            Investigator.ClearAndReload();
            Jailor.ClearAndReload();
            Janitor.ClearAndReload();
            Jester.ClearAndReload();
            Mayor.ClearAndReload();
            Medic.ClearAndReload();
            Miner.ClearAndReload();
            Morphling.ClearAndReload();
            NeutralKiller.ClearAndReload();
            Nightmare.ClearAndReload();
            ParityCop.ClearAndReload();
            Pyromaniac.ClearAndReload();
            Refugee.ClearAndReload();
            RuthlessRomantic.ClearAndReload();
            Romantic.ClearAndReload();
            Shade.ClearAndReload();
            Parasite.ClearAndReload();
            Sheriff.ClearAndReload();
            Spy.ClearAndReload();
            Tracker.ClearAndReload();
            Trapper.ClearAndReload();
            Undertaker.ClearAndReload();
            Vampire.ClearAndReload();
            VengefulRomantic.ClearAndReload();
            Warlock.ClearAndReload();
            Watcher.ClearAndReload();
            Wraith.ClearAndReload();
            Hacker.ClearAndReload();
            Psychic.ClearAndReload();
            // Modifier
            Giant.ClearAndReload();
            Mini.ClearAndReload();
            Sleepwalker.ClearAndReload();
            Spiteful.ClearAndReload();
            Clutch.ClearAndReload();
            Gopher.ClearAndReload();
            Sniper.ClearAndReload();
            Ascended.ClearAndReload();
            ExtraStats.ClearAndReload();

            //
            RockPaperScissorsGame.ClearAndReload();
        }
    }
}