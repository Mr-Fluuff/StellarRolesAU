using HarmonyLib;
using StellarRoles.Modules;
using StellarRoles.Objects;
using StellarRoles.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace StellarRoles.Patches
{
    [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.OnDestroy))]
    class IntroCutsceneOnDestroyPatch
    {
        public static PoolablePlayer playerPrefab;
        public static Vector3 BottomLeft;

        public static void Prefix(IntroCutscene __instance)
        {
            if (PlayerControl.LocalPlayer != null)
            {
                // Generate and initialize player icons
                int playerCounter = 0;
                float safeOrthographicSize = CameraSafeArea.GetSafeOrthographicSize(Camera.main);
                BottomLeft = new Vector3((1.75f - safeOrthographicSize * Camera.main.aspect * 1.70f) / 2, (0.15f - safeOrthographicSize * 1.7f) / 2, -61f);
                foreach (PlayerControl player in PlayerControl.AllPlayerControls.GetFastEnumerator())
                {
                    NetworkedPlayerInfo data = player.Data;
                    PoolablePlayer poolablePlayer = UnityEngine.Object.Instantiate(__instance.PlayerPrefab, HudManager.Instance.transform);
                    playerPrefab = __instance.PlayerPrefab;
                    poolablePlayer.SetFlipX(false);
                    player.SetPlayerMaterialColors(poolablePlayer.cosmetics.currentBodySprite.BodySprite);
                    poolablePlayer.SetSkin(data.DefaultOutfit.SkinId, data.DefaultOutfit.ColorId);
                    poolablePlayer.cosmetics.SetHat(data.DefaultOutfit.HatId, data.DefaultOutfit.ColorId);
                    poolablePlayer.cosmetics.nameText.text = data.PlayerName;
                    MapOptions.PlayerIcons[player.PlayerId] = poolablePlayer;

                    if (PlayerControl.LocalPlayer == Arsonist.Player && !player.AmOwner)
                    {
                        poolablePlayer.transform.localPosition = BottomLeft + new Vector3(-0.25f, -0.25f, 0) + Vector3.right * playerCounter++ * 0.35f;
                        poolablePlayer.transform.localScale = Vector3.one * 0.2f;
                        poolablePlayer.SetSemiTransparent(true);
                        poolablePlayer.gameObject.SetActive(true);
                    }
                    else
                    {
                        poolablePlayer.transform.localPosition = BottomLeft;
                        poolablePlayer.transform.localScale = Vector3.one * 0.4f;
                        poolablePlayer.gameObject.SetActive(false);
                    }
                }
            }

            HeadHunter.HeadHunterUpdate();

            //Goopy Spawn
            if (Helpers.IsMap(Map.Polus))
                Goopy.CreateGoopy();

            // Force Reload of SoundEffectHolder
            //SoundEffectsManager.Load();

            // First kill
            if (AmongUsClient.Instance.AmHost)
            {
                if (MapOptions.ShieldFirstKill && MapOptions.FirstKillName != "")
                {
                    PlayerControl target = PlayerControl.AllPlayerControls.GetFastEnumerator().FirstOrDefault(x => x.Data.PlayerName.Equals(MapOptions.FirstKillName));
                    if (target != null)
                    {
                        RPCProcedure.Send(CustomRPC.SetFirstKill, target);
                        MapOptions.FirstKillPlayer = target;
                    }
                }
                if (MapOptions.FirstKillPlayersNames?.Count != 0)
                    foreach (PlayerControl player in PlayerControl.AllPlayerControls.GetFastEnumerator())
                    {
                        if (MapOptions.FirstKillPlayersNames.Contains(player.Data.PlayerName))
                        {
                            RPCProcedure.Send(CustomRPC.SetFirstKillPlayers, player);
                            MapOptions.FirstKillPlayers.Add(player);
                        }
                    }
                MapOptions.FirstKillName = "";
                MapOptions.FirstKillPlayersNames.Clear();

                string GameID = Helpers.RandomId();
                RPCProcedure.Send(CustomRPC.SetRandomID, GameID);
                RPCProcedure.SetRandomId(GameID);
            }

            MapOptions.ReloadPluginOptions();
            Helpers.MoveTrash();
            Helpers.AdjustFungalLadder();
            Helpers.CheckPlayersAlive();
            //ExtraStats.UpdateSurvivability();


            if (Ascended.IsAscended(PlayerControl.LocalPlayer))
            {

                Miner.ChargesRemaining += 2;
                Vigilante.RemainingShotsVigilante++;
                if (PlayerControl.LocalPlayer.IsParityCop(out ParityCop parityCop))
                {
                    parityCop.FakeCompareCharges++;
                }

            }

            if (MapOptions.HidePetFromOthers)
            {
                RPCProcedure.Send(CustomRPC.AddPet, PlayerControl.LocalPlayer);
                MapOptions.PlayerPetsToHide.Add(PlayerControl.LocalPlayer);
            }

            Color color = Color.clear;
            color.a = 0.1f;
            foreach (PlayerControl player in Spectator.Players.GetPlayerEnumerator())
            {
                player.SetLook("Spectator", 6, "", "", "", "");
                player.cosmetics.currentBodySprite.BodySprite.color = color;
                player.Data.IsDead = true;
                player.Exiled();
                player.ClearAllTasks();
            }

            foreach (PlayerControl player in PlayerControl.AllPlayerControls.GetFastEnumerator())
            {
                player.SetPlayerSize();
            }

            Helpers.SetStartOfRoundCooldowns();
            Helpers.GameStartKillCD();

        }
    }

    [HarmonyPatch]
    class IntroPatch
    {
        public static void SetupIntroTeamIcons(ref Il2CppSystem.Collections.Generic.List<PlayerControl> yourTeam)
        {
            if (RoleDraft.isEnabled)
            {
                Il2CppSystem.Collections.Generic.List<PlayerControl> soloTeam = new();
                soloTeam.Add(PlayerControl.LocalPlayer);
                yourTeam = soloTeam;
            }
            else
            {
                // Intro solo teams
                if (Helpers.IsNeutral(PlayerControl.LocalPlayer))
                {
                    Il2CppSystem.Collections.Generic.List<PlayerControl> soloTeam = new();
                    soloTeam.Add(PlayerControl.LocalPlayer);
                    yourTeam = soloTeam;
                }

                // Intro Exe and Target
                if (PlayerControl.LocalPlayer == Executioner.Player)
                {
                    Il2CppSystem.Collections.Generic.List<PlayerControl> soloTeam = new();
                    soloTeam.Add(PlayerControl.LocalPlayer);
                    soloTeam.Add(Executioner.Target);
                    yourTeam = soloTeam;
                }

                // Add the Spy to the Impostor team (for the Impostors)
                if (Spy.Player != null && PlayerControl.LocalPlayer.Data.Role.IsImpostor)
                {
                    List<PlayerControl> players = PlayerControl.AllPlayerControls.GetFastEnumerator().ToList().OrderBy(x => Guid.NewGuid()).ToList();
                    Il2CppSystem.Collections.Generic.List<PlayerControl> fakeImpostorTeam = new(); // The local player always has to be the first one in the list (to be displayed in the center)
                    fakeImpostorTeam.Add(PlayerControl.LocalPlayer);
                    foreach (PlayerControl p in players)
                    {
                        if (PlayerControl.LocalPlayer != p && (p == Spy.Player || p.Data.Role.IsImpostor))
                            fakeImpostorTeam.Add(p);
                    }
                    yourTeam = fakeImpostorTeam;
                }
            }
        }

        public static void SetupIntroTeam(IntroCutscene __instance)
        {
            if (RoleDraft.isEnabled)
            {
                __instance.TeamTitle.text = "";
                __instance.BackgroundBar.material.color = Color.clear;
                __instance.ImpostorText.text = "";
                __instance.TeamTitle.color = Palette.CrewmateBlue;
            }
            else
            {
                List<RoleInfo> infos = RoleInfo.GetRoleInfoForPlayer(PlayerControl.LocalPlayer);
                RoleInfo roleInfo = infos.Where(info => info.FactionId != Faction.Modifier).FirstOrDefault();
                if (roleInfo == null) return;
                if (roleInfo == RoleInfo.Spectator)
                {
                    __instance.TeamTitle.text = "Spectator";
                }
                else if (roleInfo.FactionId == Faction.Neutral || (roleInfo.FactionId == Faction.NK))
                {
                    __instance.TeamTitle.text = "Neutral";
                }
                __instance.TeamTitle.color = roleInfo.Color;
                __instance.BackgroundBar.material.color = roleInfo.Color;
            }
        }

        public static IEnumerator<WaitForSeconds> EndShowRole(IntroCutscene __instance)
        {
            yield return new WaitForSeconds(5f);
            __instance.YouAreText?.gameObject.SetActive(false);
            __instance.RoleText?.gameObject.SetActive(false);
            __instance.RoleBlurbText?.gameObject.SetActive(false);
            __instance.ourCrewmate?.gameObject.SetActive(false);

        }

        [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.CreatePlayer))]
        class CreatePlayerPatch
        {
            public static void Postfix(bool impostorPositioning, ref PoolablePlayer __result)
            {
                if (RoleDraft.isEnabled) 
                {
                    __result.ToggleName(false);
                }
                if (impostorPositioning) __result.SetNameColor(Palette.ImpostorRed);
            }
        }

        [HarmonyPatch]
        public static class SetUpRoleTextPatch
        {
            public static MethodBase TargetMethod()
            {
                return EnumerationHelpers.GetMoveNext<IntroCutscene>(nameof(IntroCutscene.ShowRole))!;
            }
            static public void SetRoleTexts(IntroCutscene __instance)
            {
                // Don't override the intro of the vanilla roles
                List<RoleInfo> infos = RoleInfo.GetRoleInfoForPlayer(PlayerControl.LocalPlayer);
                RoleInfo roleInfo = infos.Where(info => info.FactionId != Faction.Modifier).FirstOrDefault();
                __instance.RoleBlurbText.text = "";
                if (roleInfo != null)
                {
                    __instance.RoleText.text = roleInfo.Name;
                    __instance.RoleText.color = roleInfo.Color;
                    __instance.YouAreText.color = roleInfo.Color;
                    __instance.RoleBlurbText.text = roleInfo.IntroDescription;
                    __instance.RoleBlurbText.color = roleInfo.Color;
                }
                if (infos.Any(info => info.RoleId == RoleId.Executioner))
                {
                    PlayerControl target = Executioner.Target;
                    __instance.RoleBlurbText.text += Helpers.ColorString(Executioner.Color, $"\nVote out {target?.Data?.PlayerName ?? ""} ");
                }
            }

            public static void Postfix(Il2CppObjectBase __instance)
            {
                var wrapper = new StateMachineWrapper<IntroCutscene>(__instance);
                // run before the first yield
                if (wrapper.GetState() != 1)
                {
                    return;
                }

                var introCutscene = wrapper.Instance;

                RandomSeed.GenerateSeed();
                if (HelpMenu.RolesUI != null) UnityEngine.Object.Destroy(HelpMenu.RolesUI);
                if (PreviousGameHistory.HistoryUI != null) PreviousGameHistory.HistoryUI.SetActive(false);
                SetRoleTexts(introCutscene);
            }
        }

        [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginCrewmate))]
        class BeginCrewmatePatch
        {
            public static void Prefix(ref Il2CppSystem.Collections.Generic.List<PlayerControl> teamToDisplay)
            {
                SetupIntroTeamIcons(ref teamToDisplay);
            }

            public static void Postfix(IntroCutscene __instance)
            {
                SetupIntroTeam(__instance);
            }
        }

        [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginImpostor))]
        class BeginImpostorPatch
        {
            public static void Prefix(ref Il2CppSystem.Collections.Generic.List<PlayerControl> yourTeam)
            {
                SetupIntroTeamIcons(ref yourTeam);
            }

            public static void Postfix(IntroCutscene __instance)
            {
                SetupIntroTeam(__instance);
            }
        }
    }
}

