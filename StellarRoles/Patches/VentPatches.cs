using HarmonyLib;
using StellarRoles.Objects;
using StellarRoles.Utilities;
using System;
using UnityEngine;
using static StellarRoles.MapOptions;

namespace StellarRoles.Patches
{

    [HarmonyPatch(typeof(Vent))]
    public static class VentCanUsePatch
    {
        [HarmonyPrefix, HarmonyPatch(nameof(Vent.SetButtons))]
        public static bool SetButtonsPrefix(Vent __instance, [HarmonyArgument(0)] ref bool enabled)
        {
            bool canSwitch = true;

            PlayerControl player = PlayerControl.LocalPlayer;

            if (player.IsJester(out _) || Spy.Player == player)
                canSwitch = false;
            if (Ascended.AscendedJester(player))
            {
                canSwitch = true;
            }

            if (enabled && !canSwitch)
            {
                for (int i = 0; i < __instance.Buttons.Length; i++)
                {
                    __instance.Buttons[i].gameObject.SetActive(false);
                }
                return false;
            }
            return true;
        }

        [HarmonyPrefix, HarmonyPatch(nameof(Vent.CanUse))]
        public static bool CanUsePrefix(Vent __instance, ref float __result, [HarmonyArgument(0)] NetworkedPlayerInfo pc, [HarmonyArgument(1)] ref bool canUse, [HarmonyArgument(2)] ref bool couldUse)
        {
            if (Helpers.IsHideAndSeek) return true;
            float num = float.MaxValue;
            PlayerControl @object = pc.Object;

            bool roleCouldUse = @object.RoleCanUseVents();

            if (__instance.name.StartsWith("SealedVent_"))
            {
                canUse = couldUse = false;
                __result = num;
                return false;
            }

            // Submerged Compatability if needed:
            if (SubmergedCompatibility.IsSubmerged)
            {
                if (SubmergedCompatibility.getInTransition())
                {
                    __result = float.MaxValue;
                    return canUse = couldUse = false;
                }
                // as submerged does, only change stuff for vents 9 and 14 of submerged. Code partially provided by AlexejheroYTB
                switch (__instance.Id)
                {
                    case 9:  // Cannot enter vent 9 (Engine Room Exit Only Vent)!
                        if (PlayerControl.LocalPlayer.inVent) break;
                        __result = float.MaxValue;
                        return canUse = couldUse = false;
                    case 14: // Lower Central
                        __result = float.MaxValue;
                        couldUse = roleCouldUse && !pc.IsDead && (@object.CanMove || @object.inVent);
                        canUse = couldUse;
                        if (canUse)
                        {
                            Vector3 center = @object.Collider.bounds.center;
                            Vector3 position = __instance.transform.position;
                            __result = Vector2.Distance(center, position);
                            canUse &= __result <= __instance.UsableDistance;
                        }
                        return false;
                }
            }

            float usableDistance = __instance.UsableDistance;

            couldUse = (@object.inVent || roleCouldUse) && !pc.IsDead && (@object.CanMove || @object.inVent);
            canUse = couldUse;
            if (canUse)
            {
                Vector3 center = @object.Collider.bounds.center;
                Vector3 position = __instance.transform.position;
                num = Vector2.Distance(center, position);
                canUse &= num <= usableDistance && !PhysicsHelpers.AnythingBetween(@object.Collider, center, position, Constants.ShipOnlyMask, false);
            }
            __result = num;
            return false;
        }

        [HarmonyPrefix, HarmonyPatch(nameof(Vent.Use))]
        public static bool UsePrefix(Vent __instance)
        {
            if (Helpers.IsHideAndSeek) return true;

            __instance.CanUse(PlayerControl.LocalPlayer.Data, out bool canUse, out bool couldUse);
            if (!canUse) return false; // No need to execute the native method as using is disallowed anyways

            bool isExit = PlayerControl.LocalPlayer.inVent;

            if (VentTrap.VentTrapMap.ContainsKey(__instance.Id))
            {
                if (Trapper.TrapRootDuration > 0f)
                {
                    if (isExit)
                    {
                        RPCProcedure.Send(CustomRPC.ExitAllVents, PlayerControl.LocalPlayer);
                        PlayerControl.LocalPlayer.MyPhysics.ExitAllVents();
                    }

                    Helpers.SetMovement(false);

                    Trapper.TrapRootDuration.DelayedAction(()=> { Helpers.SetMovement(true); });
                    if (Constants.ShouldPlaySfx())
                    {
                        SoundManager.Instance.PlaySound(PlayerControl.LocalPlayer.MyPhysics.ImpostorDiscoveredSound, false, 0.8f, null);
                    }
                }

                RPCProcedure.Send(CustomRPC.TriggerVentTrap, PlayerControl.LocalPlayer, __instance);
                RPCProcedure.TriggerVentTrap(PlayerControl.LocalPlayer, __instance.Id);
                return false;
            }
            else if (isExit)
            {
                PlayerControl.LocalPlayer.MyPhysics.RpcExitVent(__instance.Id);
            }
            else
            {
                PlayerControl.LocalPlayer.MyPhysics.RpcEnterVent(__instance.Id);
            }

            __instance.SetButtons(!isExit);
            return false;
        }

        [HarmonyPrefix, HarmonyPatch(nameof(Vent.EnterVent))]
        public static bool EnterVentPrefix(Vent __instance, PlayerControl pc)
        {
            if (Helpers.IsHideAndSeek) return true;

            if (pc.AmOwner)
            {
                Vent.currentVent = __instance;
                ConsoleJoystick.SetMode_Vent();
            }
            if (!__instance.EnterVentAnim)
                return false;

            Vector2 truePosition = PlayerControl.LocalPlayer.GetTruePosition();
            Vector2 vector = pc.GetTruePosition() - truePosition;
            float magnitude = vector.magnitude;

            if (pc != null && !HideVentInFog || HideVentInFog && magnitude < PlayerControl.LocalPlayer.lightSource.viewDistance &&
                !PhysicsHelpers.AnyNonTriggersBetween(truePosition, vector.normalized, magnitude, Constants.ShipAndObjectsMask))
                __instance.myAnim.Play(__instance.EnterVentAnim, 1f);

            if (pc.AmOwner && Constants.ShouldPlaySfx())
            {
                SoundManager.Instance.StopSound(ShipStatus.Instance.VentEnterSound);
                SoundManager.Instance.PlaySound(ShipStatus.Instance.VentEnterSound, false, 1f).pitch = FloatRange.Next(0.8f, 1.2f);
            }

            return false;
        }

        [HarmonyPrefix, HarmonyPatch(nameof(Vent.ExitVent))]
        public static bool ExitVentPrefix(Vent __instance, PlayerControl pc)
        {
            if (Helpers.IsHideAndSeek) return true;

            if (pc.AmOwner)
            {
                Vent.currentVent = null;
            }
            if (!__instance.ExitVentAnim)
                return false;

            Vector2 truePosition = PlayerControl.LocalPlayer.GetTruePosition();
            Vector2 vector = pc.GetTruePosition() - truePosition;
            float magnitude = vector.magnitude;

            if (pc != null && !HideVentInFog || HideVentInFog && magnitude < PlayerControl.LocalPlayer.lightSource.viewDistance &&
                !PhysicsHelpers.AnyNonTriggersBetween(truePosition, vector.normalized, magnitude, Constants.ShipAndObjectsMask))
                __instance.myAnim.Play(__instance.ExitVentAnim, 1f);

            if (pc.AmOwner && Constants.ShouldPlaySfx())
            {
                SoundManager.Instance.StopSound(ShipStatus.Instance.VentEnterSound);
                SoundManager.Instance.PlaySound(ShipStatus.Instance.VentEnterSound, false, 1f).pitch = FloatRange.Next(0.8f, 1.2f);
            }

            return false;
        }

        [HarmonyPostfix, HarmonyPatch(nameof(Vent.ExitVent))]

        public static void ExitVentPostfix(Vent __instance, PlayerControl pc)
        {
            if (pc.IsMorphed())
            {
                var outfit = Morphling.MorphTarget.Data.DefaultOutfit;
                pc.SetLook(Morphling.MorphTarget.Data.PlayerName, outfit.ColorId, outfit.HatId, outfit.VisorId, outfit.SkinId, outfit.PetId);
            }
        }
    }

    [HarmonyPatch(typeof(Vent), nameof(Vent.SetOutline))]
    public static class VentOutlinePatch
    {
        public static bool Prefix(Vent __instance, [HarmonyArgument(0)] bool on, [HarmonyArgument(1)] bool mainTarget)
        {
            if (Helpers.IsHideAndSeek) return true;

            var localplayer = PlayerControl.LocalPlayer;
            Color color =
                localplayer.Data.Role.IsImpostor || localplayer.IsNeutralKiller() || localplayer == Spy.Player ? Palette.ImpostorRed :
                localplayer.IsJester(out _) ? Jester.Color :
                localplayer == Scavenger.Player ? Scavenger.Color :
                Palette.CrewmateBlue;
            __instance.myRend.material.SetFloat("_Outline", on ? 1 : 0);
            __instance.myRend.material.SetColor("_OutlineColor", color);
            __instance.myRend.material.SetColor("_AddColor", mainTarget ? color : Color.clear);
            return false;
        }
    }

    [HarmonyPatch(typeof(VentButton), nameof(VentButton.DoClick))]
    class VentButtonDoClickPatch
    {
        static bool Prefix(VentButton __instance)
        {
            // Manually modifying the VentButton to use Vent.Use again in order to trigger the Vent.Use prefix patch
            // not sure this is required? lol
            if (__instance.currentTarget != null) __instance.currentTarget.Use();
            return false;

        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    class VentButtonVisibilityPatch
    {
        static void Postfix(HudManager __instance)
        {
            if (Helpers.IsHideAndSeek) return;
            if (!Helpers.GameStarted)
            {
                __instance.ImpostorVentButton.Hide();
                __instance.SabotageButton.Hide();
                return;
            }

            if (Helpers.CanShowButtons)
            {
                __instance.ImpostorVentButton.Hide();
                __instance.SabotageButton.Hide();

                if (PlayerControl.LocalPlayer.RoleCanUseVents() && PlayerControl.LocalPlayer != Engineer.Player)
                {
                    __instance.ImpostorVentButton.Show();
                    __instance.ImpostorVentButton.gameObject.SetActive(true);
                }

                if (PlayerControl.LocalPlayer.RoleCanSabotage())
                {
                    __instance.SabotageButton.Show();
                    __instance.SabotageButton.gameObject.SetActive(true);
                }
            }
        }
    }

    [HarmonyPatch(typeof(VentButton), nameof(VentButton.SetTarget))]
    public static class VentButtonSetTargetPatch
    {
        public static Sprite defaultVentSprite = null;

        public static bool Prefix(VentButton __instance)
        {
            if (defaultVentSprite == null)
            {
                defaultVentSprite = __instance.graphic.sprite;
            }
            return true;
        }

        static void Postfix(VentButton __instance)
        {
            if (!Helpers.GameStarted || !__instance.isActiveAndEnabled) return;

            if (Engineer.Player == PlayerControl.LocalPlayer)
            {
                __instance.graphic.sprite = Engineer.GetVentButtonSprite();
                __instance.buttonLabelText.SetOutlineColor(Color.cyan);

                if (Minigame.Instance)
                {
                    //__instance.graphic.enabled = false;
                    __instance.currentTarget = null;
                    __instance.SetDisabled();
                }
            }
            else if (Follower.Player == PlayerControl.LocalPlayer)
            {
                __instance.graphic.sprite = defaultVentSprite;
                __instance.buttonLabelText.SetOutlineColor(Palette.ImpostorRed);
                __instance.buttonLabelText.text = "Vent";
            }


            else if (Jester.CanEnterVents && PlayerControl.LocalPlayer.IsJester(out _))
            {
                __instance.graphic.sprite = Jester.GetVentButtonSprite();
                __instance.buttonLabelText.SetOutlineColor(Jester.Color);
                __instance.buttonLabelText.text = "Hide";
            }

            else if (Scavenger.Player == PlayerControl.LocalPlayer)
            {
                __instance.graphic.sprite = Scavenger.GetVentButtonSprite();
                __instance.buttonLabelText.SetOutlineColor(Scavenger.Color);
                __instance.buttonLabelText.text = "Vent";
            }

            else if (Spy.Player == PlayerControl.LocalPlayer)
            {
                __instance.graphic.sprite = Spy.GetVentButtonSprite();
                __instance.buttonLabelText.SetOutlineColor(Spy.Color);

                if (Minigame.Instance)
                {
                    //__instance.graphic.enabled = false;
                    __instance.currentTarget = null;
                    __instance.SetDisabled();
                }
            }
        }
    }

    [HarmonyPatch]
    public static class VentCleaningMinigamePatch
    {
        [HarmonyPatch(typeof(VentilationSystem), nameof(VentilationSystem.BootImpostorFromVent))]
        static class BootImpostorFromVentPatch
        {
            static bool Prefix()
            {
                return !DisableVentCleanEjections;
            }
        }

        [HarmonyPatch(typeof(VentilationSystem), nameof(VentilationSystem.IsImpostorInsideVent))]
        static class IsImpostorInsideVentPatch
        {
            static bool Prefix()
            {
                return !DisableVentCleanEjections;
            }
        }

        [HarmonyPatch(typeof(VentilationSystem), nameof(VentilationSystem.IsVentCurrentlyBeingCleaned))]
        static class IsVentCurrentlyBeingCleanedPatch
        {
            static bool Prefix()
            {
                return !DisableVentCleanEjections;
            }
        }
    }
}
