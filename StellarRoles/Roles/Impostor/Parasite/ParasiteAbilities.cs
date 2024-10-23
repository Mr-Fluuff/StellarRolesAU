using HarmonyLib;
using InnerNet;
using Reactor.Utilities.Extensions;
using StellarRoles.Objects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StellarRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class ParasiteAbilites
    {
        public static Camera Camera { get; set; } = null;
        public static SpriteRenderer Border { get; set; } = null;
        public static void Postfix(HudManager __instance)
        {
            if (!Helpers.GameStarted || Parasite.Player == null) return;
            if (Parasite.Player == PlayerControl.LocalPlayer)
            {
                ParasiteSetTarget();
            }
            UpdateParasite();
        }

        static void ParasiteSetTarget()
        {
            PlayerControl target;
            if (Parasite.IsNeutralKiller)
            {
                target = Helpers.SetTarget(false, true);
            }

            else if (Spy.Player != null)
            {
                if (Spy.ImpostorsCanKillAnyone)
                {
                    target = Helpers.SetTarget(false, true);
                }
                else
                {
                    target = Helpers.SetTarget(true, true, new List<PlayerControl>() { Spy.Player });
                }
            }

            else
            {
                target = Helpers.SetTarget(true, true);
            }

            Parasite.CurrentTarget = Parasite.Controlled == null ? target : null;
            Helpers.SetPlayerOutline(Parasite.CurrentTarget, Parasite.Color);
        }

        public static void ParasiteCreate()
        {
            Camera = UnityEngine.Object.Instantiate(Camera.main);
            Camera.name = "ParasiteCam";
            Camera.orthographicSize = 1.5f;
            var aspect = (float)Screen.height / Screen.width;
            var width = aspect * 0.3f;
            var posw = aspect * 0.04f;
            Camera.rect = new Rect(posw, 0.04f, width, 0.3f);
            Camera.transform.DestroyChildren();
            Camera.GetComponent<FollowerCamera>().Destroy();
            Camera.nearClipPlane = -1;
            Camera.depth = Camera.main.depth + 1;
            Camera.gameObject.SetActive(false);

            Border = UnityEngine.Object.Instantiate(new GameObject("ParasiteBorder")).AddComponent<SpriteRenderer>();
            Border.sprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.ParasiteBorder.png", 144f);
            Border.transform.SetParent(HudManager.Instance.transform);
            Border.gameObject.layer = 5;
            var aspectpos = Border.gameObject.AddComponent<AspectPosition>();
            aspectpos.Alignment = AspectPosition.EdgeAlignments.LeftBottom;
            aspectpos.DistanceFromEdge = new Vector3(1.14f, 1.14f);
            Border.gameObject.SetActive(false);
        }

        public static float timer = 0;
        public static void UpdateParasite()
        {
            if (Parasite.Controlled != null)
            {
                Parasite.ControlTimer -= Time.deltaTime;
                if (Parasite.Controlled.AmOwner)
                {
                    timer += Time.deltaTime;
                    if ((int)timer == 1)
                    {
                        timer = 0;
                        new CustomMessage("You are being Controlled", 1f, true, Palette.ImpostorRed);
                    }
                }

                if (AmongUsClient.Instance.AmHost)
                {
                    if (Parasite.Controlled.Data.IsDead || Parasite.Controlled.Data.Disconnected)
                    {
                        RPCResetInfected();
                    }
                    if (Parasite.ControlTimer <= 0 && !Parasite.Unlimited)
                    {
                        RPCKillInfected();
                    }
                    if (Parasite.Player.Data.IsDead)
                    {
                        if (Parasite.CanSaveInfested)
                        {
                            RPCResetInfected();
                        }
                        else
                        {
                            RPCKillInfected();
                        }
                    }
                }
                if (Camera != null && Parasite.Player.AmOwner)
                {
                    Camera.transform.position = Parasite.Controlled.transform.position;
                }
            }
        }

        public static void ParasiteControl()
        {
            RPCControlPlayer();
            Parasite.CurrentTarget = null;

            if (Camera == null)
                ParasiteCreate();

            Camera.gameObject.SetActive(true);
            Border.gameObject.SetActive(true);
        }

        public static void RPCControlPlayer()
        {
            RPCProcedure.Send(CustomRPC.ControlPlayer, Parasite.CurrentTarget);
            RPCProcedure.ControlPlayer(Parasite.CurrentTarget);
            RPCProcedure.Send(CustomRPC.PsychicAddCount);
        }

        public static void RPCKillInfected()
        {
            RPCProcedure.Send(CustomRPC.KillInfected, true);
            RPCProcedure.KillInfected(true);
        }

        public static void RPCResetInfected()
        {
            RPCProcedure.Send(CustomRPC.KillInfected, false);
            RPCProcedure.KillInfected(false);
        }

        public static void DestroyAssets()
        {
            if (Camera?.gameObject != null)
            {
                Camera.Destroy();
                Camera = null;
            }
            if (Border?.gameObject != null)
            {
                Border.Destroy();
                Border = null;
            }
        }
    }

    public static class ParasiteMovementPatches
    {
        public static Vector2 parasiteVector = Vector2.zero;

        [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.FixedUpdate))]
        class InfectedPlayerPhysics_FixedUpdate
        {
            static bool Prefix(PlayerPhysics __instance)
            {
                if (Parasite.Controlled?.AmOwner == true && __instance.myPlayer.AmOwner)
                {
                    __instance.HandleAnimation(PlayerControl.LocalPlayer.Data.IsDead);
                    return false;
                }

                if (PlayerControl.LocalPlayer != Parasite.Player || Parasite.Controlled == null) return true;
                if (__instance.myPlayer == Parasite.Controlled)
                {
                    __instance.HandleAnimation(__instance.myPlayer.Data.IsDead);
                    return false;
                }
                return true;
            }

        }
        [HarmonyPatch(typeof(KeyboardJoystick), nameof(KeyboardJoystick.Update))]
        class InfectedPlayerKeyboardUpdate
        {
            static void Postfix()
            {
                if (PlayerControl.LocalPlayer != Parasite.Player) return;

                parasiteVector.x = parasiteVector.y = 0f;
                if (parasiteVector == Vector2.zero)
                {
                    if (KeyboardJoystick.player.GetButton("ParasiteRight"))
                    {
                        parasiteVector.x = parasiteVector.x + 1;
                    }
                    if (KeyboardJoystick.player.GetButton("ParasiteLeft"))
                    {
                        parasiteVector.x = parasiteVector.x - 1;
                    }
                    if (KeyboardJoystick.player.GetButton("ParasiteUp"))
                    {
                        parasiteVector.y = parasiteVector.y + 1;
                    }
                    if (KeyboardJoystick.player.GetButton("ParasiteDown"))
                    {
                        parasiteVector.y = parasiteVector.y - 1;
                    }
                }
                parasiteVector.Normalize();

                if (Parasite.Controlled != null)
                {
                    var vel = parasiteVector * Parasite.Controlled.MyPhysics.TrueSpeed;
                    Parasite.Controlled.MyPhysics.body.velocity = vel;
                    var pos = Parasite.Controlled.transform.position;
                    RPCProcedure.Send(CustomRPC.MoveControlledPlayer, vel.x, vel.y);
                    //RPCProcedure.Send(CustomRPC.MoveControlledPlayer, pos.x, pos.y);
                }

            }
        }
    }

}
