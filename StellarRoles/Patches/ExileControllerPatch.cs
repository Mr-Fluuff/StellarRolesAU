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
        public static GameData.PlayerInfo lastExiled;
        public static void Prefix([HarmonyArgument(0)] GameData.PlayerInfo exiled)
        {
            lastExiled = exiled;

            // Guardian shield
            if (Guardian.Shielded != null && AmongUsClient.Instance.AmHost)
            {
                // We need to send the RPC from the host here, to make sure that the order of shifting and setting the shield is correct(for that reason the futureShifted and futureShielded are being synced)
                RPCProcedure.Send(CustomRPC.GuardianResetShielded);
                RPCProcedure.GuardianResetShield();
            }

            // Miner Vents
            if (Miner.Player != null)
            {
                MinerVent.ConvertToVents();
            }

            if (AmongUsClient.Instance.AmHost)
                foreach (Jailor jailor in Jailor.PlayerIdToJailor.Values)
                {
                    RPCProcedure.Send(CustomRPC.JailBreak, jailor.Player.PlayerId);
                    RPCProcedure.JailBreak(jailor);
                }

            // SecurityGuard vents and cameras
            List<SurvCamera> allCameras = MapUtilities.CachedShipStatus.AllCameras.ToList();
            foreach (SurvCamera camera in MapOptions.CamerasToAdd)
            {
                camera.gameObject.SetActive(true);
                camera.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
                allCameras.Add(camera);
            };
            MapUtilities.CachedShipStatus.AllCameras = allCameras.ToArray();
            MapOptions.CamerasToAdd.Clear();

            foreach (Vent vent in MapOptions.VentsToSeal)
            {
                PowerTools.SpriteAnim animator = vent.GetComponent<PowerTools.SpriteAnim>();
                animator?.Stop();
                vent.EnterVentAnim = vent.ExitVentAnim = null;
                vent.myRend.sprite = animator == null ? Trapper.GetStaticVentSealedSprite() : Trapper.GetAnimatedVentSealedSprite();
                if (vent.name.StartsWith("FutureSealedVent_MinerVent_")) Trapper.GetStaticVentSealedSprite();
                if (SubmergedCompatibility.IsSubmerged && vent.Id == 0) vent.myRend.sprite = Trapper.GetSubmergedCentralUpperSealedSprite();
                if (SubmergedCompatibility.IsSubmerged && vent.Id == 14) vent.myRend.sprite = Trapper.GetSubmergedCentralLowerSealedSprite();
                vent.myRend.color = Color.white;
                vent.name = "SealedVent_" + vent.name;
            }
            MapOptions.VentsToSeal.Clear();
            MapOptions.VentsInUse.Clear();
        }
    }

    [HarmonyPatch]
    class ExileControllerWrapUpPatch
    {

        [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
        class BaseExileControllerPatch
        {
            public static void Postfix(ExileController __instance)
            {
                WrapUpPostfix(__instance.exiled);
            }
        }

        [HarmonyPatch(typeof(AirshipExileController), nameof(AirshipExileController.WrapUpAndSpawn))]
        class AirshipExileControllerPatch
        {
            public static void Postfix(AirshipExileController __instance)
            {
                WrapUpPostfix(__instance.exiled);
            }
        }

        // Workaround to add a "postfix" to the destroying of the exile controller (i.e. cutscene) and SpwanInMinigame of submerged
        [HarmonyPatch(typeof(UnityEngine.Object), nameof(UnityEngine.Object.Destroy), new Type[] { typeof(GameObject) })]
        public static void Prefix(GameObject obj)
        {
            if (!SubmergedCompatibility.IsSubmerged) return;
            if (obj.name.Contains("ExileCutscene"))
            {
                WrapUpPostfix(ExileControllerBeginPatch.lastExiled);
            }
            else if (obj.name.Contains("SpawnInMinigame"))
                Sleepwalker.SetPosition();
        }

        static void WrapUpPostfix(GameData.PlayerInfo exiled)
        {
            // I don't think exiled will be null here but just to be sure
            if (exiled != null)
            {
                // Jester win condition
                if (Jester.IsJester(exiled.PlayerId, out Jester jester))
                {
                    RPCProcedure.Send(CustomRPC.SetJesterWinner, exiled.PlayerId);
                    Jester.WinningJesterPlayer = jester.Player;
                    Jester.TriggerJesterWin = true;
                }

                // Executioner win condition
                if (Executioner.Player != null && Executioner.Target.PlayerId == exiled.PlayerId && !Executioner.Player.Data.IsDead)
                {
                    RPCProcedure.Send(CustomRPC.SetExecutionerWin);
                    Executioner.TriggerExecutionerWin = true;
                }

                foreach (Spiteful player in Spiteful.SpitefulRoles)
                    if (player.Player.PlayerId == exiled.PlayerId)
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

            Helpers.CheckImpsAlive();
            Helpers.CheckPlayersAlive();

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

            // Force Bounty Hunter Bounty Update
            if (BountyHunter.Player == PlayerControl.LocalPlayer)
                BountyHunter.BountyUpdateTimer = 0f;

            // Medium spawn souls
            if (Detective.Player == PlayerControl.LocalPlayer)
            {
                foreach (SpriteRenderer sr in Detective.CrimeScenes)
                    UnityEngine.Object.Destroy(sr.gameObject);
                Detective.CrimeScenes.Clear();

                foreach ((DeadPlayer deadBody, Vector3 ps) in Detective.FeatureDeadBodies)
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
                Detective.DeadBodies.Clear();
                Detective.DeadBodies.AddRange(Detective.FeatureDeadBodies);
                Detective.FeatureDeadBodies.Clear();
            }

            // Sleepwalker set position
            Sleepwalker.SetPosition();

            if (PlayerControl.LocalPlayer.RoleCanUseVents())
            {
                Helpers.ResetVentBug();
            }
        }
    }

    [HarmonyPatch(typeof(SpawnInMinigame), nameof(SpawnInMinigame.Close))]  // Set position of AntiTp players AFTER they have selected a spawn.
    class AirshipSpawnInPatch
    {
        static void Postfix()
        {
            Sleepwalker.SetPosition();
        }
    }

    [HarmonyPatch(typeof(TranslationController), nameof(TranslationController.GetString), new Type[] { typeof(StringNames), typeof(Il2CppReferenceArray<Il2CppSystem.Object>) })]
    class ExileControllerMessagePatch
    {
        static void Postfix(ref string __result, [HarmonyArgument(0)] StringNames id)
        {
            try
            {
                if (ExileController.Instance != null && ExileController.Instance.exiled != null)
                {
                    PlayerControl player = Helpers.PlayerById(ExileController.Instance.exiled.Object.PlayerId);
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
                            __result = "";
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
            DeadPlayer deadPlayer = new(__instance, DateTime.UtcNow, DeathReason.Exile, null);
            GameHistory.DeadPlayers.Add(deadPlayer);

            // Remove fake tasks when player dies
            if (__instance.HasFakeTasks())
                __instance.ClearAllTasks();
        }
    }
}