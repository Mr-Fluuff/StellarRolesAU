using System.Collections.Generic;
using UnityEngine;
//TODO: USE THIS CLASS
namespace StellarRoles
{
    public abstract class Role
    {
        public static readonly Dictionary<byte, Role> RoleDictionary = new();
        public static readonly List<KeyValuePair<byte, RoleId>> RoleHistory = new();


        public PlayerControl player;
        public Color color;
        public RoleId roleId;


        protected Role(PlayerControl p)
        {
            player = p;
            RoleDictionary.Add(p.PlayerId, this);
        }

        public void AddToRoleHistory(RoleId role)
        {
            RoleHistory.Add(KeyValuePair.Create(player.PlayerId, role));
        }

        public static Role GetRole(PlayerControl player)
        {
            if (player == null) return null;
            if (RoleDictionary.TryGetValue(player.PlayerId, out Role role))
                return role;

            return null;
        }

        public static T GetRole<T>(PlayerControl player) where T : Role
        {
            return GetRole(player) as T;
        }
    }
}
