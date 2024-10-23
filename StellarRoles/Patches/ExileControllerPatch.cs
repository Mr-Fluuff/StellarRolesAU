using HarmonyLib;
using StellarRoles.Objects;
using StellarRoles.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StellarRoles.Patches
{
    [HarmonyPatch(typeof(ExileController), nameof(ExileController.Begin))]
    [HarmonyPriority(Priority.First)]
    class ExileControllerBeginPatch
    {
        public static ExileController.InitProperties Init;
        public static void Prefix(ExileController __instance, [HarmonyArgument(0)] ExileController.InitProperties init)
        {
            Init = init;
        }
    }

    [HarmonyPatch]
    class ExileControllerWrapUpPatch
    {

        [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
        class BaseExileControllerPatch
        {
            public static void Prefix(ExileController __instance)
            {
                //WrapUpPrefix(__instance.initData);
            }
            public static void Postfix(ExileController __instance)
            {
                WrapUpPostfix(__instance.initData);
            }
        }

        [HarmonyPatch(typeof(AirshipExileController), nameof(AirshipExileController.WrapUpAndSpawn))]
        class AirshipExileControllerPatch
        {
            public static void Prefix(ExileController __instance)
            {
                //WrapUpPrefix(__instance.initData);
            }
            public static void Postfix(AirshipExileController __instance)
            {
                WrapUpPostfix(__instance.initData);
            }
        }

        // Workaround to add a "postfix" to the destroying of the exile controller (i.e. cutscene) and SpwanInMinigame of submerged
        [HarmonyPatch(typeof(UnityEngine.Object), nameof(UnityEngine.Object.Destroy), [typeof(GameObject)])]
        class SubmergedExileControllerPatch
        {
            //public static bool Destroyed = false;
            //public static bool Spawn = false;
            public static void Prefix(GameObject obj, ref (bool Destroyed, bool Spawn) __state)
            {
                if (!SubmergedCompatibility.IsSubmerged || !Helpers.IsMap(Map.Submerged)) return;
                try
                {
                    if (obj?.name?.Contains("ExileCutscene") == true)
                    {
                        __state.Destroyed = true;
                        //WrapUpPrefix(ExileControllerBeginPatch.Init);
                    }
                    else if (obj?.name?.Contains("SpawnInMinigame") == true)
                    {
                        __state.Spawn = true;
                    }
                }
                catch { }
            }
            public static void Postfix((bool Destroyed, bool Spawn)__state)
            {
                if (__state.Destroyed)
                {
                    try
                    {
                        WrapUpPostfix(ExileControllerBeginPatch.Init);
                    }
                    catch { }
                }
                else if (__state.Spawn)
                {
                    Sleepwalker.SetPosition();
                    //Spawn = false;
                }
            }
        }

/*        static void WrapUpPrefix(ExileController.InitProperties init)
        {
            PlayerControl exiledPlayer = null;
            if (init.networkedPlayer != null)
            {
                exiledPlayer = init.networkedPlayer.Object;
            }
            try
            {
                ExtraStats.ExileStats(exiledPlayer);
            }
            catch { }
        }
*/
        static void WrapUpPostfix(ExileController.InitProperties init)
        {
            PlayerControl exiledPlayer = null;
            Helpers.CheckImpsAlive();
            Helpers.CheckPlayersAlive();
            ExtraStats.UpdateSurvivability();


            if (init.networkedPlayer != null)
            {
                exiledPlayer = init.networkedPlayer.Object;
            }

            if (AmongUsClient.Instance.AmHost)
            {
                // Guardian shield
                if (Guardian.Shielded != null)
                {
                    // We need to send the RPC from the host here, to make sure that the order of shifting and setting the shield is correct(for that reason the futureShifted and futureShielded are being synced)
                    RPCProcedure.Send(CustomRPC.GuardianResetShielded);
                    RPCProcedure.GuardianResetShield();
                }

                foreach (Jailor jailor in Jailor.PlayerIdToJailor.Values)
                {
                    RPCProcedure.Send(CustomRPC.JailBreak, jailor.Player);
                    RPCProcedure.JailBreak(jailor);
                }

                if (exiledPlayer != null)
                {
                    // Jester win condition
                    if (Jester.IsJester(exiledPlayer.PlayerId, out Jester jester))
                    {
                        RPCProcedure.Send(CustomRPC.SetJesterWinner, exiledPlayer);
                        Jester.WinningJesterPlayer = jester.Player;
                        Jester.TriggerJesterWin = true;
                    }

                    // Executioner win condition
                    if (Executioner.Player != null && Executioner.Target.PlayerId == exiledPlayer.PlayerId && !Executioner.Player.Data.IsDead)
                    {
                        RPCProcedure.Send(CustomRPC.SetExecutionerWin);
                        Executioner.TriggerExecutionerWin = true;
                    }
                }

                // Guardian shield
                if (Guardian.Shielded != null)
                {
                    // We need to send the RPC from the host here, to make sure that the order of shifting and setting the shield is correct(for that reason the futureShifted and futureShielded are being synced)
                    RPCProcedure.Send(CustomRPC.GuardianResetShielded);
                    RPCProcedure.GuardianResetShield();
                }
            }

            // Miner Vents
            if (Miner.Player != null)
            {
                MinerVent.ConvertToVents();
            }

            // SecurityGuard vents and cameras
            List<SurvCamera> allCameras = ShipStatus.Instance.AllCameras.ToList();
            foreach (SurvCamera camera in MapOptions.CamerasToAdd)
            {
                camera.gameObject.SetActive(true);
                camera.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
                allCameras.Add(camera);
            };
            ShipStatus.Instance.AllCameras = allCameras.ToArray();
            MapOptions.CamerasToAdd.Clear();

            foreach (Vent vent in MapOptions.VentsToSeal)
            {
                PowerTools.SpriteAnim animator = vent.myAnim;
                vent.EnterVentAnim = vent.ExitVentAnim = null;
                vent.myRend.sprite = animator == null ? Trapper.GetStaticVentSealedSprite() : Trapper.GetAnimatedVentSealedSprite();
                animator?.Stop();
                if (Helpers.IsMap(Map.Fungal))
                {
                    vent.myRend.sprite = Trapper.GetFungalSealedSprite();
                    vent.myRend.transform.localPosition = new Vector3(0, -.01f);
                }
                if (vent.name.StartsWith("FutureSealedVent_MinerVent_")) vent.myRend.sprite = Trapper.GetStaticVentSealedSprite();
                if (SubmergedCompatibility.IsSubmerged && vent.Id == 0) vent.myRend.sprite = Trapper.GetSubmergedCentralUpperSealedSprite();
                if (SubmergedCompatibility.IsSubmerged && vent.Id == 14) vent.myRend.sprite = Trapper.GetSubmergedCentralLowerSealedSprite();
                vent.myRend.color = Color.white;
                vent.name = "SealedVent_" + vent.name;
            }
            MapOptions.VentsToSeal.Clear();
            MapOptions.VentsInUse.Clear();

            foreach (Spiteful player in Spiteful.SpitefulRoles)
            {
                if (exiledPlayer != null && player.Player.PlayerId == exiledPlayer.PlayerId)
                    player.IsExiled = true;
                else if (!player.IsExiled)
                    player.VotedBy.Clear();
            }

            Scavenger.ScavengerToRefugeeCheck();

            // Reset custom button timers where necessary
            CustomButton.MeetingEndedUpdate();

            // Headhunter deactive dead players and add new targets if needed
            HeadHunter.HeadHunterUpdate();

            //Reset Tracked Players
            Tracker.ResetTracked();

            Goopy.StartAnimation();

            RomanticAbilites.RomanticRoleUpdate(true);
            RomanticAbilites.VengefulRoleUpdate(true);

            Bombed.KillerDictionary.Clear();

            // Arsonist deactivate dead poolable players
            if (Arsonist.Player == PlayerControl.LocalPlayer)
            {
                int visibleCounter = 0;
                Vector3 newBottomLeft = IntroCutsceneOnDestroyPatch.BottomLeft;
                foreach (PlayerControl player in PlayerControl.AllPlayerControls.GetFastEnumerator())
                {
                    if (!MapOptions.PlayerIcons.ContainsKey(player.PlayerId)) continue;
                    if (player.Data.IsDead || player.Data.Disconnected)
                    {
                        MapOptions.PlayerIcons[player.PlayerId].gameObject.SetActive(false);
                    }
                    else
                    {
                        MapOptions.PlayerIcons[player.PlayerId].transform.localPosition = newBottomLeft + Vector3.right * visibleCounter * 0.35f;
                        visibleCounter++;
                    }
                }
            }

            // Medium spawn souls
            if (Detective.Player == PlayerControl.LocalPlayer)
            {
                foreach (SpriteRenderer sr in Detective.CrimeScenes)
                    UnityEngine.Object.Destroy(sr.gameObject);
                Detective.CrimeScenes.Clear();

                foreach ((DeadPlayer deadBody, Vector3 ps) in Detective.FreshDeadBodies)
                {
                    GameObject s = new();
                    //s.transform.position = ps;
                    s.transform.position = new Vector3(ps.x, ps.y, ps.y / 1000 + 0.001f);
                    s.layer = 2;
                    SpriteRenderer rend = s.AddComponent<SpriteRenderer>();
                    s.AddSubmergedComponent(SubmergedCompatibility.Classes.ElevatorMover);
                    rend.sprite = Detective.GetCrimeSceneSprite();
                    Detective.CrimeScenes.Add(rend);
                    byte playerId = deadBody.Player.PlayerId;
                    PlayerControl killer = deadBody.KillerIfExisting;
                    if (killer != null && !Detective.PlayerIdToKillerCountQuestion.ContainsKey(playerId) && Detective.KillersLinkToKills.TryGetValue(killer.PlayerId, out PlayerList kills))
                    {
                        Detective.PlayerIdToKillerCountQuestion.Add(playerId, kills.Count);
                        Detective.PlayerIdToKillerKilledQuestion.Add(playerId, Detective.FindOtherPlayer(playerId));
                    }

                }
                Detective.OldDeadBodies.Clear();
                Detective.OldDeadBodies.AddRange(Detective.FreshDeadBodies);
                Detective.FreshDeadBodies.Clear();
            }

            // Sleepwalker set position
            if (!Helpers.IsMap(Map.Airship) && !Helpers.IsMap(Map.Submerged))
            {
                Sleepwalker.SetPosition();
            }

            if (PlayerControl.LocalPlayer.AmOwner)
            {
                Helpers.ResetVentBug();
                RPCProcedure.Send(CustomRPC.ResetAnimation, PlayerControl.LocalPlayer);
                RPCProcedure.ResetAnimation(PlayerControl.LocalPlayer);
            }

            if (GameTimer.Enabletimer) GameTimer._isCountingDown = true;
        }
    }

    [HarmonyPatch(typeof(TranslationController), nameof(TranslationController.GetString), new Type[] { typeof(StringNames), typeof(Il2CppReferenceArray<Il2CppSystem.Object>) })]
    class ExileControllerMessagePatch
    {
        static void Postfix(ref string __result, [HarmonyArgument(0)] StringNames id)
        {
            try
            {
                if (ExileController.Instance != null && ExileController.Instance.initData.networkedPlayer != null)
                {
                    PlayerControl player = Helpers.PlayerById(ExileController.Instance.initData.networkedPlayer.Object.PlayerId);
                    if (player == null) return;

                    bool isSpiteful = player.IsSpiteful(out Spiteful spiteful);
                    if (isSpiteful && (id == StringNames.ExileTextNonConfirm))
                    {
                        __result = player.Data.PlayerName + " was Spiteful!";
                    }
                    // Exile role text
                    if (id == StringNames.ExileTextPN || id == StringNames.ExileTextSN || id == StringNames.ExileTextPP || id == StringNames.ExileTextSP)
                    {
                        __result = $"{player.Data.PlayerName} was The {string.Join(" ", RoleInfo.GetRoleInfoForPlayer(player, false).Select(x => x.Name))}";
                        if (isSpiteful)
                            __result += "\n And was Spiteful!";
                    }
                    // Hide number of remaining impostors on Jester win
                    if (id == StringNames.ImpostorsRemainP || id == StringNames.ImpostorsRemainS)
                    {
                        if (player.IsJester(out _))
                            __result = "Meow";
                        __result = "\n" + __result;
                    }
                }
            }
            catch
            {
                // pass - Hopefully prevent leaving while exiling to softlock game
            }
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Exiled))]
    public static class ExilePlayerPatch
    {
        public static void Postfix(PlayerControl __instance)
        {
            // Collect dead player info
            new DeadPlayer(__instance, DateTime.UtcNow, DeathReason.Exile, null);

            // Remove fake tasks when player dies
            if (__instance.HasFakeTasks())
                __instance.ClearAllTasks();
        }
    }
}