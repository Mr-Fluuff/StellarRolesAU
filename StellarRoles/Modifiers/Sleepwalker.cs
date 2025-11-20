using UnityEngine;

namespace StellarRoles
{
    public static class Sleepwalker
    {
        public static readonly PlayerList Players = new();
        public static Vector3 LastPosition { get; set; }

        public static void GetDescription()
        {
            RoleInfo.Sleepwalker.SettingsDescription = Helpers.WrapText(
                $"The {nameof(Sleepwalker)} modifier prevents a player from being sent to the meeting table when a meeting is called or a body is reported. " +
                $"After the meeting ends, they will continue where they left off the previous round.");
        }

        public static void ClearAndReload()
        {
            Players.Clear();
            LastPosition = Vector3.zero;
        }

        public static void RpcSleepwalkToPosition()
        {
            if (LastPosition != Vector3.zero && Players.Contains(PlayerControl.LocalPlayer))
            {
                PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(LastPosition);
                if (SubmergedCompatibility.IsSubmerged)
                    SubmergedCompatibility.ChangeFloor(LastPosition.y > -7);

                LastPosition = Vector3.zero;
            }
        }
    }
}
