using AmongUs.GameOptions;
using HarmonyLib;
using StellarRoles.Utilities;
using System.Linq;
using UnityEngine;

namespace StellarRoles.Patches
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public static class PlayerSizeUpdatePatch
    {
        public static void PlayerSizeUpdate(PlayerControl p)
        {
            // Set default player size
            CircleCollider2D collider = p.Collider.CastFast<CircleCollider2D>();

            p.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
            collider.radius = Mini.DefaultColliderRadius;
            collider.offset = Mini.DefaultColliderOffset * Vector2.down;
            float miniScale = .55f;
            float giantScale = 1f;
            float correctedMiniColliderRadius = Mini.DefaultColliderRadius * 0.7f / miniScale; // scale / 0.7f is the factor by which we decrease the player size, hence we need to increase the collider size by 0.7f / scale
            float correctedGiantColliderRadius = Giant.DefaultColliderRadius * 0.7f / giantScale; // scale / 0.7f is the factor by which we decrease the player size, hence we need to increase the collider size by 0.7f / scale
            bool playerIsMorphed = Morphling.Player != null && p == Morphling.Player && Morphling.MorphTimer > 0;
            bool playerIsMini = Mini.Player != null && p == Mini.Player;
            bool playerIsGiant = Giant.Player != null && p == Giant.Player;

            // Set adapted player size to Mini and Morphling
            if (Camouflager.CamouflageTimer > 0f) return;

            if (playerIsMorphed)
            {
                if (Mini.Player != null && Morphling.MorphTarget == Mini.Player)
                {
                    p.transform.localScale = new Vector3(miniScale, miniScale, 1f);
                    collider.radius = correctedMiniColliderRadius;
                }

                else if (Giant.Player != null && Morphling.MorphTarget == Giant.Player)
                {
                    p.transform.localScale = new Vector3(giantScale, giantScale, 1f);
                    collider.radius = correctedGiantColliderRadius;
                }
            }
            else
            {
                if (playerIsMini)
                {
                    p.transform.localScale = new Vector3(miniScale, miniScale, 1f);
                    collider.radius = correctedMiniColliderRadius;
                }

                if (playerIsGiant)
                {
                    p.transform.localScale = new Vector3(giantScale, giantScale, 1f);
                    collider.radius = correctedGiantColliderRadius;
                }
            }
        }

        public static void Postfix(PlayerControl __instance)
        {
            if (!Helpers.GameStarted || Helpers.IsHideAndSeek) return;
            PlayerSizeUpdate(__instance);
        }
    }
}
