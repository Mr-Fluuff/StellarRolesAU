using AmongUs.GameOptions;
using HarmonyLib;
using UnityEngine;

namespace StellarRoles.Patches
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public static class PlayerPhysicsFixedUpdatePatch
    {
        public static void Postfix(PlayerControl __instance)
        {
            if (!Helpers.GameStarted || GameOptionsManager.Instance.currentGameOptions.GameMode == GameModes.HideNSeek)
                return;
            if (PlayerControl.LocalPlayer != __instance || GameData.Instance == null || !__instance.CanMove)
                return;
            float speed = GameOptionsManager.Instance.CurrentGameOptions.GetFloat(FloatOptionNames.PlayerSpeedMod) * 2f;

            bool isMorphed = Morphling.Player == __instance && Morphling.MorphTimer > 0;

            if (!isMorphed)
            {
                if (Giant.Player == __instance)
                {
                    speed *= Giant.SpeedMultiplier;
                }
                else if (Mini.Player == __instance)
                {
                    speed *= Mini.SpeedMultiplier;
                }
            }

            if (isMorphed)
            {
                if (Morphling.MorphTarget == Giant.Player)
                    speed *= Giant.SpeedMultiplier;
                else if (Morphling.MorphTarget == Mini.Player)
                    speed *= Mini.SpeedMultiplier;
            }
            else if (Undertaker.Player == __instance && Undertaker.DeadBodyDragged != null)
            {
                if (Undertaker.DeadBodyDragged.IsMiniBody())
                    speed *= 0.6f;
                else if (Undertaker.DeadBodyDragged.IsGiantBody())
                    speed *= 0.3f;
                else
                    speed *= 0.5f;
                if (Ascended.IsAscended(Undertaker.Player))
                {
                    speed *= 1.2f;
                }
            }
            else if (Wraith.Player == __instance && Wraith.PhaseOn)
            {
                speed *= Wraith.SpeedMultiplier;
            }

            __instance.MyPhysics.Speed = speed;
        }
    }

    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.Awake))]
    public static class PlayerPhysiscs_Awake_Patch
    {
        public static void Postfix(PlayerPhysics __instance)
        {
            if (!__instance.body)
                return;
            __instance.body.interpolation = RigidbodyInterpolation2D.Interpolate;
        }
    }

    [HarmonyPatch(typeof(CustomNetworkTransform), nameof(CustomNetworkTransform.FixedUpdate))]
    public static class SpeedPatch
    {
        public static void Postfix(CustomNetworkTransform __instance)
        {
            if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started || GameOptionsManager.Instance.currentGameOptions.GameMode == GameModes.HideNSeek)
                return;

            giantSpeed(__instance);
            miniSpeed(__instance);
            wraithSpeed(__instance);
            morphlingSpeed(__instance);
        }


        static void giantSpeed(CustomNetworkTransform __instance)
        {
            if (Giant.Player == __instance.gameObject.GetComponent<PlayerControl>() && !__instance.AmOwner && __instance.interpolateMovement != 0.0f && !Giant.Player.IsMorphed())
                __instance.body.velocity *= Giant.SpeedMultiplier;
        }

        static void miniSpeed(CustomNetworkTransform __instance)
        {
            if (Mini.Player == __instance.gameObject.GetComponent<PlayerControl>() && !__instance.AmOwner && __instance.interpolateMovement != 0.0f && !Mini.Player.IsMorphed())
                __instance.body.velocity *= Mini.SpeedMultiplier;
        }

        static void wraithSpeed(CustomNetworkTransform __instance)
        {
            if (Wraith.Player == __instance.gameObject.GetComponent<PlayerControl>() && !__instance.AmOwner && __instance.interpolateMovement != 0.0f)
                __instance.body.velocity *= Wraith.PhaseOn ? Wraith.SpeedMultiplier : 1;
        }

        static void morphlingSpeed(CustomNetworkTransform __instance)
        {
            if (Morphling.Player == __instance.gameObject.GetComponent<PlayerControl>() && !__instance.AmOwner && __instance.interpolateMovement != 0.0f)
            {
                if (Morphling.MorphTimer > 0)
                {
                    if (Mini.Player != null && Morphling.MorphTarget == Mini.Player)
                        __instance.body.velocity *= Mini.SpeedMultiplier;
                    else if (Giant.Player != null && Morphling.MorphTarget == Giant.Player)
                        __instance.body.velocity *= Giant.SpeedMultiplier;
                    else
                        __instance.body.velocity *= 1f;
                }
                else
                    __instance.body.velocity *= 1f;
            }
        }
    }

    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.WalkPlayerTo))]
    public class PlayerPhysicsWalkPlayerToPatch
    {
        public static void Prefix(PlayerPhysics __instance)
        {
            bool MorphedAs(PlayerControl player) => __instance.myPlayer == Morphling.Player && Morphling.MorphTarget == player && Morphling.MorphTimer > 0f;
            if (Camouflager.CamouflageTimer > 0f)
                return;

            if (
                (__instance.myPlayer == Mini.Player || MorphedAs(Mini.Player)) && !Mini.Player.IsMorphed()
            )
                __instance.myPlayer.Collider.offset = Mini.DefaultColliderOffset * Vector2.down;

            if (
                (__instance.myPlayer == Giant.Player || MorphedAs(Giant.Player)) && !Giant.Player.IsMorphed()
            )
                __instance.myPlayer.Collider.offset = Giant.DefaultColliderOffset * Vector2.down;
        }
    }
}