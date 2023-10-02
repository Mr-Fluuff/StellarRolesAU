using UnityEngine;

namespace StellarRoles.Patches
{
    public static class PlayerSizeUpdatePatch
    {
        static readonly float miniScale = .55f;
        static readonly float giantScale = 1f;
        static float CorrectedMiniColliderRadius => Mini.DefaultColliderRadius * 0.7f / miniScale; // scale / 0.7f is the factor by which we decrease the player size, hence we need to increase the collider size by 0.7f / scale
        static float CorrectedGiantColliderRadius => Giant.DefaultColliderRadius * 0.7f / giantScale; // scale / 0.7f is the factor by which we decrease the player size, hence we need to increase the collider size by 0.7f / scale

        public static void SetPlayerSize(this PlayerControl p)
        {
            // Set default player size
            CircleCollider2D collider = p.Collider.CastFast<CircleCollider2D>();

            p.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
            collider.radius = Mini.DefaultColliderRadius;
            collider.offset = Mini.DefaultColliderOffset * Vector2.down;

            // Set adapted player size to Mini and Morphling
            if (Camouflager.CamouflageTimer > 0f) return;

            bool playerIsMorphed = Morphling.Player != null && p == Morphling.Player && Morphling.MorphTimer > 0;
            bool playerIsMini = Mini.Player != null && p == Mini.Player;
            bool playerIsGiant = Giant.Player != null && p == Giant.Player;


            if (playerIsMorphed)
            {
                if (Mini.Player != null && Morphling.MorphTarget == Mini.Player)
                {
                    p.transform.localScale = new Vector3(miniScale, miniScale, 1f);
                    collider.radius = CorrectedMiniColliderRadius;
                }

                else if (Giant.Player != null && Morphling.MorphTarget == Giant.Player)
                {
                    p.transform.localScale = new Vector3(giantScale, giantScale, 1f);
                    collider.radius = CorrectedGiantColliderRadius;
                }
            }
            else
            {
                if (playerIsMini)
                {
                    p.transform.localScale = new Vector3(miniScale, miniScale, 1f);
                    collider.radius = CorrectedMiniColliderRadius;
                }

                if (playerIsGiant)
                {
                    p.transform.localScale = new Vector3(giantScale, giantScale, 1f);
                    collider.radius = CorrectedGiantColliderRadius;
                }
            }
        }
    }
}
