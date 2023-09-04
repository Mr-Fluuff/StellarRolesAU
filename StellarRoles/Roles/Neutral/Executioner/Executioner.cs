using UnityEngine;

namespace StellarRoles
{
    public enum ExePromotes
    {
        Jester,
        Refugee
    }
    public static class Executioner
    {
        public static readonly Color Color = new Color32(201, 204, 63, byte.MaxValue);
        public static readonly Color TargetColor = new Color32(0, 0, 0, byte.MaxValue);

        public static PlayerControl Player { get; set; }
        public static PlayerControl Target { get; set; }
        public static ExePromotes PromotesTo => (ExePromotes)CustomOptionHolder.ExecutionerPromotesTo.GetSelection(); // 0 = Jester, 1 = Refugee
        public static bool ConvertsImmediately => CustomOptionHolder.ExecutionerConvertsImmediately.GetSelection() == 0;

        public static bool TriggerExecutionerWin { get; set; } = false;

        public static void GetDescription()
        {
            RoleInfo.Executioner.SettingsDescription = Helpers.WrapText(
                $"The goal of the {nameof(Executioner)} is to get a randomly selected crewmate voted out. " +
                $"Be wary of how you do this! The {nameof(Assassin)} or {nameof(Vigilante)} might catch on and kill you in the meeting!\n\n" +
                $"The {nameof(Executioner)} cannot have the {nameof(Mayor)}, {nameof(Sheriff)}, Impostors, Neutrals, or Neutral Killers as its target.\n\n" +
                $"If the target dies, the {nameof(Executioner)} will turn into a {(PromotesTo == 0 ? nameof(Jester) : nameof(Refugee))} {(ConvertsImmediately ? "Immediately" : "at the Next Meeting")}.");
        }

        public static void ExecutionerCheckPromotion()
        {
            if (Player == null || !AmongUsClient.Instance.AmHost)
                return;

            if (Target == null || Target.Data.Disconnected || Target.Data.IsDead)
            {
                RPCProcedure.Send(CustomRPC.ExecutionerChangeRole);
                RPCProcedure.ExecutionerChangeRole();
            }
        }


        public static void ClearAndReload()
        {
            Player = null;
            TriggerExecutionerWin = false;
            Target = null;
        }
    }
}
