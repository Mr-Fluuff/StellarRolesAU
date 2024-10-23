using System.Collections.Generic;
using UnityEngine;


namespace StellarRoles
{
    public enum GainSabo
    {
        ImpWipe,
        Never,
        GameStart
    }
    public static class NeutralKiller
    {
        public static readonly PlayerList Players = new();
        public static readonly PlayerList RogueImps = new();
        public static readonly Color Color = new Color32(69, 69, 69, byte.MaxValue);
        public static bool LoseCritSabo => !CustomOptionHolder.ImpsLooseCritSabo.GetBool();
        public static bool LoseDoorSabo => !CustomOptionHolder.ImpsLoseDoors.GetBool();
        public static GainSabo GainsSabo => (GainSabo)CustomOptionHolder.NeutralKillersGetNonCritSabo.GetSelection();
        public static PlayerControl CurrentTarget { get; set; }

        public static void GetDescription()
        {
            List<string> rogueImpsList = new();
            if (CustomOptionHolder.BomberIsNeutral.GetBool() && CustomOptionHolder.BomberSpawnRate.GetSelection() > 0)
                rogueImpsList.Add(nameof(Bomber));
            if (CustomOptionHolder.CamouflagerIsNeutral.GetBool() && CustomOptionHolder.CamouflagerSpawnRate.GetSelection() > 0)
                rogueImpsList.Add(nameof(Camouflager));
            if (CustomOptionHolder.JanitorIsNeutral.GetBool() && CustomOptionHolder.JanitorSpawnRate.GetSelection() > 0)
                rogueImpsList.Add(nameof(Janitor));
            if (CustomOptionHolder.MinerIsNeutral.GetBool() && CustomOptionHolder.MinerSpawnRate.GetSelection() > 0)
                rogueImpsList.Add(nameof(Miner));
            if (CustomOptionHolder.MorphlingIsNeutral.GetBool() && CustomOptionHolder.MorphlingSpawnRate.GetSelection() > 0)
                rogueImpsList.Add(nameof(Morphling));
            if (CustomOptionHolder.ParasiteIsNeutral.GetBool() && CustomOptionHolder.ParasiteSpawnRate.GetSelection() > 0)
                rogueImpsList.Add(nameof(Parasite));
            if (CustomOptionHolder.ShadeIsNeutral.GetBool() && CustomOptionHolder.ShadeSpawnRate.GetSelection() > 0)
                rogueImpsList.Add(nameof(Shade));
            if (CustomOptionHolder.UndertakerIsNeutral.GetBool() && CustomOptionHolder.UndertakerSpawnRate.GetSelection() > 0)
                rogueImpsList.Add(nameof(Undertaker));
            if (CustomOptionHolder.VampireIsNeutral.GetBool() && CustomOptionHolder.VampireSpawnRate.GetSelection() > 0)
                rogueImpsList.Add(nameof(Vampire));
            if (CustomOptionHolder.WarlockIsNeutral.GetBool() && CustomOptionHolder.WarlockSpawnRate.GetSelection() > 0)
                rogueImpsList.Add(nameof(Warlock));
            if (CustomOptionHolder.WraithIsNeutral.GetBool() && CustomOptionHolder.WraithSpawnRate.GetSelection() > 0)
                rogueImpsList.Add(nameof(Warlock));
            string settingsinfo = $"A {Helpers.ColorString(Color, "Rogue-Impostor")} is an {Helpers.ColorString(Palette.ImpostorRed, "Impostor")} role that has been toggled by your host to spawn as a Neutral Killer instead!\n\n";

            if (rogueImpsList.Count > 0)
                settingsinfo += $"The Following Roles are set to spawn this way:\n{string.Join(", ", rogueImpsList)}";
            else
                settingsinfo += $"The host does not have a {Helpers.ColorString(Color, "Rogue-Impostor")} toggled on";

            RoleInfo.RogueImpostor.SettingsDescription = Helpers.WrapText(settingsinfo);
        }

        public static void ClearAndReload()
        {
            Players.Clear();
            RogueImps.Clear();
            CurrentTarget = null;
        }
    }
}
