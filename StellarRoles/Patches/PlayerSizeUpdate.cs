using HarmonyLib;
using UnityEngine;

namespace StellarRoles.Patches
{
    public static class PlayerSizeUpdatePatch
    {
        static readonly float miniScale = .55f;
        static readonly float giantScale = 1f;
        public const float DefaultColliderRadius = 0.2233912f;
        public const float DefaultColliderOffset = 0.365f;

        static float CorrectedMiniColliderRadius => DefaultColliderRadius * (0.71f / miniScale);
        static float CorrectedMiniColliderOffset => DefaultColliderOffset * (0.71f / miniScale);

        static float CorrectedGiantColliderRadius => DefaultColliderRadius * (0.71f / giantScale);
        static float CorrectedGiantColliderOffset => DefaultColliderOffset * (0.71f / giantScale);

        // (scale / 0.7f) is the factor by which we decrease the player size, hence we need to increase the collider size by (0.7f / scale)

        public static void SetPlayerSize(this PlayerControl p)
        {
            // Set default player size
            CircleCollider2D collider = p.Collider.CastFast<CircleCollider2D>();

            p.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
            collider.radius = DefaultColliderRadius;
            collider.offset = DefaultColliderOffset * Vector2.down;

            // Set adapted player size to Mini and Morphling
            if (Camouflager.CamouflageTimer > 0f || p.IsMushroomMixupActive()) return;

            bool playerIsMorphed = Morphling.Player != null && p == Morphling.Player && Morphling.MorphTimer > 0;
            bool playerIsInfested = Parasite.Player != null && p == Parasite.Controlled;

            bool playerIsMini = Mini.Player != null && p == Mini.Player;
            bool playerIsGiant = Giant.Player != null && p == Giant.Player;


            if (playerIsMorphed)
            {
                if (Mini.Player != null && Morphling.MorphTarget == Mini.Player)
                {
                    p.transform.localScale = new Vector3(miniScale, miniScale, 1f);
                    collider.radius = 0.32f;
                    collider.offset = 0.2f * Vector2.down;
                }
                else if (Giant.Player != null && Morphling.MorphTarget == Giant.Player)
                {
                    p.transform.localScale = new Vector3(giantScale, giantScale, 1f);
                    collider.radius = CorrectedGiantColliderRadius;
                    collider.offset = CorrectedGiantColliderOffset * Vector2.down;
                }
            }
            else if (playerIsInfested)
            {
                if (Mini.Player != null && Parasite.Player == Mini.Player)
                {
                    p.transform.localScale = new Vector3(miniScale, miniScale, 1f);
                    collider.radius = 0.32f;
                    collider.offset = 0.2f * Vector2.down;
                }
                else if (Giant.Player != null && Parasite.Player == Giant.Player)
                {
                    p.transform.localScale = new Vector3(giantScale, giantScale, 1f);
                    collider.radius = CorrectedGiantColliderRadius;
                    collider.offset = CorrectedGiantColliderOffset * Vector2.down;
                }
            }
            else
            {
                if (playerIsMini)
                {
                    p.transform.localScale = new Vector3(miniScale, miniScale, 1f);
                    collider.radius = 0.32f;
                    collider.offset = 0.2f * Vector2.down;
                }
                if (playerIsGiant)
                {
                    p.transform.localScale = new Vector3(giantScale, giantScale, 1f);
                    collider.radius = CorrectedGiantColliderRadius;
                    collider.offset = CorrectedGiantColliderOffset * Vector2.down;
                }
            }
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MixUpOutfit))]
    public static class MixUpOutfitPatch
    {
        static void Postfix(PlayerControl __instance)
        {
            __instance.SetPlayerSize();
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixMixedUpOutfit))]
    public static class FixMixedUpOutfitPatch
    {
        static void Postfix(PlayerControl __instance)
        {
            __instance.SetPlayerSize();
        }
    }
}
