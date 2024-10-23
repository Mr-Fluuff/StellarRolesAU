using HarmonyLib;
using StellarRoles.Objects;
using StellarRoles.Utilities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StellarRoles.Patches
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    class HudManagerUpdatePatch
    {
        static void NeutralKillerSetTarget()
        {
            if (NeutralKiller.Players.Contains(PlayerControl.LocalPlayer))
                NeutralKiller.CurrentTarget = Helpers.SetTarget(false, true, canIncrease: true);
        }

        static void MorphlingAndCamouflagerUpdate()
        {
            float oldCamouflageTimer = Camouflager.CamouflageTimer;
            float oldMorphTimer = Morphling.MorphTimer;
            float oldParasiteTimer = Parasite.ControlTimer;

            Camouflager.CamouflageTimer -= Time.deltaTime;
            Morphling.MorphTimer -= Time.deltaTime;

            // Camouflage reset and set Morphling look if necessary
            if (oldCamouflageTimer > 0f && Camouflager.CamouflageTimer <= 0f && !PlayerControl.LocalPlayer.IsMushroomMixupActive())
            {
                Camouflager.ResetCamouflage();
                if (Morphling.MorphTimer > 0f && Morphling.Player != null && Morphling.MorphTarget != null)
                {
                    RPCProcedure.SetLook(Morphling.Player, Morphling.MorphTarget);
                }
                if (Parasite.Player != null && Parasite.Controlled != null)
                {
                    RPCProcedure.SetLook(Parasite.Controlled, Parasite.Player);
                }
            }

            // Morphling reset (only if camouflage is inactive)
            if (Camouflager.CamouflageTimer <= 0f && oldMorphTimer > 0f && Morphling.MorphTimer <= 0f && Morphling.Player != null)
                Morphling.ResetMorph();
        }

        public static void RefreshRoleDescription()
        {
            var player = PlayerControl.LocalPlayer;
            List<string> taskTexts = new();

            if (MapOptions.DeadCrewPreventTaskWin && player.IsCrew() && player.Data.Role.IsDead && MapOptions.CrewAlive <= 0)
                taskTexts.Add(Helpers.ColorString(Palette.ImpostorRed, "Tasks will no longer save your kind!"));

            foreach (RoleInfo roleInfo in RoleInfo.GetRoleInfoForPlayer(player))
                if (roleInfo != RoleInfo.WasRomantic)
                    taskTexts.Add(Helpers.GetRoleString(roleInfo));

            List<PlayerTask> toRemove = new();
            foreach (PlayerTask t in player.myTasks.GetFastEnumerator())
            {
                ImportantTextTask textTask = t.TryCast<ImportantTextTask>();
                if (textTask == null)
                    continue;

                string currentText = textTask.Text;

                if (taskTexts.Contains(currentText))
                    taskTexts.Remove(currentText); // TextTask for this RoleInfo does not have to be added, as it already exists
                else
                    toRemove.Add(t); // TextTask does not have a corresponding RoleInfo and will hence be deleted
            }

            foreach (PlayerTask t in toRemove)
            {
                t.OnRemove();
                player.myTasks.Remove(t);
                Object.Destroy(t.gameObject);
            }

            // Add TextTask for remaining RoleInfos
            foreach (string title in taskTexts)
            {
                ImportantTextTask task = new GameObject("RoleTask").AddComponent<ImportantTextTask>();
                task.transform.SetParent(player.transform, false);
                task.Text = title;
                player.myTasks.Insert(0, task);
            }
        }

        public static Dictionary<byte, (string name, Color color)> TagColorDict = new();
        static void ResetNameTagsAndColors()
        {
            PlayerControl localPlayer = PlayerControl.LocalPlayer;
            var myData = localPlayer.Data;
            var amImpostor = myData.Role.IsImpostor;

            TagColorDict.Clear();

            try
            {
                foreach (var data in GameData.Instance.AllPlayers.GetFastEnumerator())
                {
                    PlayerControl player = null;
                    if (data != null && data.Object != null)
                    {
                        player = data.Object;
                    }
                    bool playerIsImpostor = data.Role.IsImpostor;
                    string text = data.PlayerName;
                    Color color;

                    if (player != null)
                    {
                        color = amImpostor && playerIsImpostor ? Palette.ImpostorRed : Color.white;
                        string playerName = text;
                        string colorName = data.ColorName;

                        if (player.IsMorphed())
                        {
                            playerName = Morphling.MorphTarget.Data.PlayerName;
                            colorName = Morphling.MorphTarget.Data.ColorName;
                        }
                        if (player == Parasite.Controlled)
                        {
                            playerName = Parasite.Player.Data.PlayerName;
                            colorName = Parasite.Player.Data.ColorName;
                        }

                        TMPro.TextMeshPro nameText = player.cosmetics.nameText;
                        TMPro.TextMeshPro colorBlindText = player.cosmetics.colorBlindText;

                        nameText.text = Helpers.ShouldHidePlayerName(player) ? "" : playerName;
                        nameText.color = color;

                        colorBlindText.text = Helpers.ShouldHidePlayerName(player) || localPlayer == player ? "" : colorName.Replace(")", "").Replace("(", "");
                    }
                    else
                    {
                        color = Color.white;
                    }

                    TagColorDict.Add(data.PlayerId, (text, color));
                }

                MeetingHud.Instance?.playerStates?.ToList().ForEach(playerVoteArea =>
                {
                    (string name, Color color) = TagColorDict[playerVoteArea.TargetPlayerId];
                    TMPro.TextMeshPro platetext = playerVoteArea.NameText;
                    platetext.text = name;
                    platetext.color = color;
                });
            }
            catch { }
        }

        static void SetPlayerNameColor(PlayerControl player, Color color)
        {
            player.cosmetics.nameText.color = color;
            if (MeetingHud.Instance != null)
                foreach (PlayerVoteArea voteArea in MeetingHud.Instance.playerStates)
                    if (player.PlayerId == voteArea.TargetPlayerId)
                        voteArea.NameText.color = color;
        }

        static void SetNameColors()
        {
            PlayerControl localPlayer = PlayerControl.LocalPlayer;
            RoleInfo localRole = RoleInfo.GetRoleInfoForPlayer(localPlayer, false).FirstOrDefault();

            if (!MapOptions.ToggleRoles || MapOptions.ShowRoles)
                SetPlayerNameColor(localPlayer, localRole.Color);
            else if (!localPlayer.Data.Role.IsImpostor)
                SetPlayerNameColor(localPlayer, Color.white);

            if (localPlayer.Data.Role.IsImpostor)
            {
                if (Morphling.Player != null && Morphling.Player.IsMorphed() && Morphling.MorphTarget.Data.Role.IsImpostor)
                    SetPlayerNameColor(Morphling.Player, Palette.ImpostorRed);

                if (Spy.Player != null)
                    SetPlayerNameColor(Spy.Player, Spy.Color);
            }

            if (localPlayer.Data.IsDead && MapOptions.GhostsSeeRoles)
                foreach (PlayerControl player in PlayerControl.AllPlayerControls.GetFastEnumerator())
                    if (!player.AmOwner)
                        SetPlayerNameColor(player, RoleInfo.GetRoleInfoForPlayer(player, false).FirstOrDefault().Color);
        }

        static void PetsUpdate()
        {
            if (MeetingHud.Instance) return;

            foreach (var data in GameData.Instance.AllPlayers.GetFastEnumerator())
            {
                if (data == null || data.Disconnected) continue;
                var player = data.Object;
                if (player.AmOwner || player == null) continue;
                PetBehaviour pet = player.GetPet();

                bool isMorphed = player.IsMorphed() && MapOptions.PlayerPetsToHide.Contains(Morphling.MorphTarget.PlayerId);
                bool optedOut = MapOptions.PlayerPetsToHide.Contains(player.PlayerId);
                bool cantsee = !PlayerControl.LocalPlayer.Data.IsDead
                    && !player.petting
                    && (isMorphed
                    || optedOut
                    || player.Data.IsDead
                    || player.inVent);

                if (pet != null)
                    pet.Visible = !cantsee;
            }
        }

        static void UpdateMiniMap()
        {
            PlayerControl player = PlayerControl.LocalPlayer;
            bool isImp = player.Data.Role.IsImpostor;
            bool nK = player.IsNeutralKiller();
            if (!isImp && !nK) return;
            bool impsLoseCritical = isImp && NeutralKiller.LoseCritSabo && MapOptions.Allimpsdead;
            bool nkCanSabo = nK && player.NeutralKillerCanSabo();
            bool loseCritical = impsLoseCritical || nkCanSabo;

            bool impsLoseDoors = isImp && NeutralKiller.LoseDoorSabo && MapOptions.Allimpsdead;

            if (MapBehaviour.Instance != null && MapBehaviour.Instance.IsOpen)
            {
                switch (Helpers.CurrentMap())
                {
                    case Map.Skeld:
                    case Map.Dleks:
                        // Map sabotage
                        GameObject minimapSabotage = GameObject.Find("Main Camera/Hud/ShipMap(Clone)/InfectedOverlay"); // Skeld
                        if (loseCritical)
                        {
                            minimapSabotage.transform.GetChild(4).gameObject.SetActive(false); // Sabotage o2
                            minimapSabotage.transform.GetChild(8).gameObject.SetActive(false); // Sabotage reactor
                        }
                        if (impsLoseDoors)
                        {
                            minimapSabotage.transform.GetChild(0).gameObject.SetActive(false); // Cafeteria Doors
                            minimapSabotage.transform.GetChild(2).gameObject.SetActive(false); // Medbay Doors
                            minimapSabotage.transform.GetChild(3).GetChild(0).gameObject.SetActive(false); // Electrical Doors
                            minimapSabotage.transform.GetChild(5).gameObject.SetActive(false); // Left Engine Doors
                            minimapSabotage.transform.GetChild(6).gameObject.SetActive(false); // Right Engine Doors
                            minimapSabotage.transform.GetChild(7).gameObject.SetActive(false); // Storage Doors
                            minimapSabotage.transform.GetChild(9).gameObject.SetActive(false); // Security Doors
                        }
                        break;
                    case Map.Mira:
                        GameObject minimapSabotageMira = GameObject.Find("Main Camera/Hud/HqMap(Clone)/InfectedOverlay"); // Mira
                        if (loseCritical)
                        {
                            minimapSabotageMira.transform.GetChild(2).gameObject.SetActive(false);
                            minimapSabotageMira.transform.GetChild(3).gameObject.SetActive(false);
                        }
                        break;
                    case Map.Polus:
                        GameObject minimapSabotagePolus = GameObject.Find("Main Camera/Hud/PbMap(Clone)/InfectedOverlay"); // Polus
                        if (loseCritical)
                        {
                            minimapSabotagePolus.transform.GetChild(6).GetChild(0).gameObject.SetActive(false);
                        }
                        if (impsLoseDoors)
                        {
                            minimapSabotagePolus.transform.GetChild(0).GetChild(1).gameObject.SetActive(false); // Sabotage reactor
                            minimapSabotagePolus.transform.GetChild(1).GetChild(1).gameObject.SetActive(false); // Sabotage reactor
                            minimapSabotagePolus.transform.GetChild(2).gameObject.SetActive(false); // Sabotage reactor
                            minimapSabotagePolus.transform.GetChild(3).gameObject.SetActive(false); // Sabotage reactor
                            minimapSabotagePolus.transform.GetChild(4).gameObject.SetActive(false); // Sabotage reactor
                            minimapSabotagePolus.transform.GetChild(5).gameObject.SetActive(false); // Sabotage reactor
                            minimapSabotagePolus.transform.GetChild(6).GetChild(1).gameObject.SetActive(false); // Sabotage reactor
                        }
                        break;
                    case Map.Airship:
                        GameObject minimapSabotageAirship = GameObject.Find("Main Camera/Hud/AirshipMap(Clone)/InfectedOverlay"); // Airship
                        if (loseCritical)
                        {
                            minimapSabotageAirship.transform.GetChild(3).gameObject.SetActive(false);
                        }
                        if (impsLoseDoors)
                        {
                            minimapSabotageAirship.transform.GetChild(0).GetChild(1).gameObject.SetActive(false); // Comms Doors
                            minimapSabotageAirship.transform.GetChild(2).gameObject.SetActive(false); // MainHall Doors
                            minimapSabotageAirship.transform.GetChild(4).gameObject.SetActive(false); // Records Doors
                            minimapSabotageAirship.transform.GetChild(5).gameObject.SetActive(false); // Brig Doors
                            minimapSabotageAirship.transform.GetChild(6).gameObject.SetActive(false); // Kitchen Doors
                            minimapSabotageAirship.transform.GetChild(7).gameObject.SetActive(false); // Medbay Doors
                        }
                        break;
                    case Map.Fungal:
                        GameObject minimapSabotageFungal = GameObject.Find("Main Camera/Hud/FungleMap(Clone)/InfectedOverlay"); // Airship
                        if (loseCritical)
                        {
                            minimapSabotageFungal.transform.GetChild(3).GetChild(0).gameObject.SetActive(false);
                        }
                        if (impsLoseDoors)
                        {
                            minimapSabotageFungal.transform.GetChild(0).GetChild(1).gameObject.SetActive(false); // Comms Doors
                            minimapSabotageFungal.transform.GetChild(1).GetChild(0).gameObject.SetActive(false); // Lab Doors
                            minimapSabotageFungal.transform.GetChild(2).GetChild(0).gameObject.SetActive(false); // Storage Doors
                            minimapSabotageFungal.transform.GetChild(3).GetChild(1).gameObject.SetActive(false); // Reactor Doors
                            minimapSabotageFungal.transform.GetChild(4).GetChild(0).gameObject.SetActive(false); // Lookout Doors
                            minimapSabotageFungal.transform.GetChild(5).GetChild(0).gameObject.SetActive(false); // Kitchen Doors
                            minimapSabotageFungal.transform.GetChild(7).GetChild(0).gameObject.SetActive(false); // MiningPit Doors

                        }
                        break;
                }
            }
        }

        static Color R1Color => new Color(255, 50, 0); //Red-Orangeish
        static void SetFirstKillPlayersTag()
        {
            PlayerControl localplayer = PlayerControl.LocalPlayer;
            bool CanSeeTag = !localplayer.IsCrew() || localplayer == Sheriff.Player || localplayer == Guardian.Player;

            if (!MapOptions.IsFirstRound || !MapOptions.ShowRoundOneKillIndicators || MapOptions.FirstKillPlayers.Count == 0) return;
            if (!CanSeeTag) return;

            string suffix = Helpers.ColorString(R1Color, "\n(Died R1)");

            foreach (PlayerControl player in MapOptions.FirstKillPlayers)
            {
                if (player == localplayer || player == null) continue;

                if (!Helpers.ShouldHidePlayerName(player) && !Helpers.IsMorphed(player) && player.IsAlive())
                    player.cosmetics.nameText.text += suffix;

                if (Helpers.IsMorphlingTargetAndMorphed(player) && !Helpers.ShouldHidePlayerName(Morphling.Player))
                    Morphling.Player.cosmetics.nameText.text += suffix;
            }
        }

        static void SetExecutionerNameTag()
        {
            PlayerControl localplayer = PlayerControl.LocalPlayer;

            if (Executioner.Target != null && (Executioner.Player == localplayer || localplayer.Data.IsDead))
            {
                string suffix = Helpers.ColorString(Executioner.Color, " ⁂");
                if (!Helpers.ShouldHidePlayerName(Executioner.Target) && !PhysicsHelpers.AnythingBetween(localplayer.GetTruePosition(), Executioner.Target.GetTruePosition(), Constants.ShadowMask, false))
                    Executioner.Target.cosmetics.nameText.text += suffix;

                if (MeetingHud.Instance != null)
                    MeetingHud.Instance.playerStates.First(voteArea => voteArea.TargetPlayerId == Executioner.Target.PlayerId).NameText.text += suffix;
            }
        }

        static void SetRomanticNameTag()
        {
            if (Romantic.Player == null || Romantic.Lover == null) return;
            PlayerControl localplayer = PlayerControl.LocalPlayer;
            string suffix = Helpers.ColorString(Romantic.Color, " ♥");

            if (Romantic.Player == localplayer || (MapOptions.GhostsSeeRomanticTarget && localplayer.Data.IsDead))
            {
                if (!Helpers.ShouldHidePlayerName(Romantic.Lover) && !Romantic.Lover.Data.Disconnected)
                    Romantic.Lover.cosmetics.nameText.text += suffix;

                if (MeetingHud.Instance != null)
                    MeetingHud.Instance.playerStates.First(voteArea => voteArea.TargetPlayerId == Romantic.Lover.PlayerId).NameText.text += suffix;
            }
            else if (Romantic.Lover == localplayer && (Romantic.PartnerSeesLoveInstantly || (Romantic.PartnerSeesLoveAfterMeeting && Romantic.SetNameFirstMeeting)))
            {
                if (!Helpers.IsInvisible(localplayer))
                    localplayer.cosmetics.nameText.text += suffix;

                if (MeetingHud.Instance != null)
                    MeetingHud.Instance.playerStates.First(voteArea => voteArea.TargetPlayerId == localplayer.PlayerId).NameText.text += suffix;
            }
        }

        static void SetVengefulRomanticNameTag()
        {
            if (VengefulRomantic.Player == null) return;
            PlayerControl localplayer = PlayerControl.LocalPlayer;
            string revengesuffix = Helpers.ColorString(VengefulRomantic.RevengeColor, " ♥");
            string loversuffix = Helpers.ColorString(Romantic.Color, " ♥");

            if (VengefulRomantic.Player == localplayer || localplayer.Data.IsDead)
            {
                if (!VengefulRomantic.Lover.Data.Disconnected)
                    VengefulRomantic.Lover.cosmetics.nameText.text += loversuffix;

                if (MeetingHud.Instance != null)
                    MeetingHud.Instance.playerStates.First(voteArea => voteArea.TargetPlayerId == VengefulRomantic.Lover.PlayerId).NameText.text += loversuffix;
            }

            if (MapOptions.GhostsSeeRomanticTarget && localplayer.Data.IsDead && VengefulRomantic.Target != null)
            {
                if (!VengefulRomantic.Target.Data.Disconnected)
                    VengefulRomantic.Target.cosmetics.nameText.text += revengesuffix;

                if (MeetingHud.Instance != null)
                    MeetingHud.Instance.playerStates.First(voteArea => voteArea.TargetPlayerId == VengefulRomantic.Target.PlayerId).NameText.text += revengesuffix;
            }
        }

        static void SetParityCop()
        {
            PlayerControl localplayer = PlayerControl.LocalPlayer;
            string suffix = Helpers.ColorString(ParityCop.Color, " <size=120%>⇋</size>");

            foreach (ParityCop parityCop in ParityCop.ParityCopDictionary.Values)
                if ((parityCop.Player.AmOwner || localplayer.Data.IsDead) && !parityCop.Player.Data.IsDead)
                    foreach (PlayerControl player in parityCop.ComparedPlayers.GetPlayerEnumerator())
                    {
                        if (player == null) continue;

                        if (!Helpers.ShouldHidePlayerName(player) && player.IsAlive() && !player.IsMorphed())
                            player.cosmetics.nameText.text += suffix;

                        if (Helpers.IsMorphlingTargetAndMorphed(player) && !Helpers.ShouldHidePlayerName(Morphling.Player))
                            Morphling.Player.cosmetics.nameText.text += suffix;
                    }
        }


        static void SetBomberTags()
        {
            PlayerControl localplayer = PlayerControl.LocalPlayer;
            string suffix = Helpers.ColorString(Palette.ImpostorRed, " <size=120%>※</size>");

            if (Bombed.BombedDictionary.Count <= 0) return;

            foreach (Bombed bombed in Bombed.BombedDictionary.Values)
            {
                if (bombed.Player == null) continue;
                // Bomber
                if (Bomber.SeeBombTarget && ((localplayer.Data.Role.IsImpostor && !Bomber.IsNeutralKiller) || bombed.Bomber == localplayer || localplayer.Data.IsDead))
                {
                    if (!Helpers.ShouldHidePlayerName(bombed.Player) && bombed.Player.IsAlive() && !bombed.Player.IsMorphed())
                        bombed.Player.cosmetics.nameText.text += suffix;

                    if (Helpers.IsMorphlingTargetAndMorphed(bombed.Player) && !Helpers.ShouldHidePlayerName(Morphling.Player))
                        Morphling.Player.cosmetics.nameText.text += suffix;
                }
            }
        }

        static void SetParasiteTags()
        {
            PlayerControl localplayer = PlayerControl.LocalPlayer;
            
            string suffix = Helpers.ColorString(Parasite.Color, " <size=100%>■</size>");

            if (Parasite.Controlled == null) return;

            if ((localplayer.Data.Role.IsImpostor && !Parasite.IsNeutralKiller) || Parasite.Player == localplayer || localplayer.Data.IsDead)
            {
                if (!Helpers.ShouldHidePlayerName(Parasite.Controlled) && Parasite.Controlled.IsAlive() && !Parasite.Controlled.IsMorphed())
                    Parasite.Controlled.cosmetics.nameText.text += suffix;

                if (Helpers.IsMorphlingTargetAndMorphed(Parasite.Controlled) && !Helpers.ShouldHidePlayerName(Morphling.Player))
                    Morphling.Player.cosmetics.nameText.text += suffix;
            }
        }

        static void SetNightmareTags()
        {
            PlayerControl localplayer = PlayerControl.LocalPlayer;
            if (Nightmare.PlayerToNightmare.Count <= 0) return;
            string suffix = Helpers.ColorString(HeadHunter.TargetColor, " <size=120%>※ </size>");

            foreach (Nightmare nightmare in Nightmare.PlayerToNightmare.Values)
            {
                if (localplayer.Data.IsDead || (localplayer == nightmare.Player && nightmare.BlindedPlayers.Count > 0))
                {
                    foreach (PlayerControl player in nightmare.BlindedPlayers.GetPlayerEnumerator())
                    {
                        if (player == null) continue;

                        if (!Helpers.ShouldHidePlayerName(player) && !player.IsMorphed() && player.IsAlive())
                        {
                            player.cosmetics.nameText.text += suffix;
                        }

                        if (Helpers.IsMorphlingTargetAndMorphed(player) && !Helpers.ShouldHidePlayerName(Morphling.Player))
                            Morphling.Player.cosmetics.nameText.text += suffix;
                    }
                }
            }
        }

        static void SetShadeTags()
        {
            PlayerControl localplayer = PlayerControl.LocalPlayer;
            if (Shade.Player == null || Shade.BlindedPlayers.Count <= 0) return;
            if (!Shade.Player.AmOwner && !localplayer.Data.IsDead) return;

            string suffix = Helpers.ColorString(HeadHunter.TargetColor, " <size=120%>※ </size>");

            foreach (PlayerControl player in Shade.BlindedPlayers.GetPlayerEnumerator())
            {
                if (player == null) continue;
                if (Shade.Player.Data.Role.IsImpostor && (player.Data.Role.IsImpostor || player == Spy.Player)) continue;

                if (!Helpers.ShouldHidePlayerName(player) && player.IsAlive() && !player.IsMorphed())
                {
                    player.cosmetics.nameText.text += suffix;
                }

                if (Helpers.IsMorphlingTargetAndMorphed(player) && !Helpers.ShouldHidePlayerName(Morphling.Player))
                {
                    Morphling.Player.cosmetics.nameText.text += suffix;
                }
            }
        }


        static void SetNightmareParalyzedPlayerTag()
        {
            string suffix = Helpers.ColorString(Nightmare.Color, " <size=120%>※ </size>");

            foreach (Nightmare nightmare in Nightmare.PlayerToNightmare.Values)
            {
                PlayerControl paralyzedPlayer = nightmare.ParalyzedPlayer;
                if (paralyzedPlayer != null && paralyzedPlayer.IsAlive() && (nightmare.Player.AmOwner || PlayerControl.LocalPlayer.Data.IsDead))
                {
                    if (!Helpers.ShouldHidePlayerName(paralyzedPlayer) && paralyzedPlayer.IsAlive() && !paralyzedPlayer.IsMorphed())
                        paralyzedPlayer.cosmetics.nameText.text += suffix;

                    if (Helpers.IsMorphlingTargetAndMorphed(paralyzedPlayer) && !Helpers.ShouldHidePlayerName(Morphling.Player))
                        Morphling.Player.cosmetics.nameText.text += suffix;
                }
            }
        }

        static void SetArsonistDouseTags()
        {
            PlayerControl localplayer = PlayerControl.LocalPlayer;
            string suffix = Helpers.ColorString(Arsonist.Color, " ❖");

            if (Arsonist.Player != null && !Arsonist.Player.Data.IsDead && (localplayer.Data.IsDead || localplayer == Arsonist.Player))
            {
                foreach (PlayerControl player in Arsonist.DousedPlayers.GetPlayerEnumerator())
                {
                    if (player == null || player.Data.Disconnected) continue;
                    if (!Helpers.ShouldHidePlayerName(player) && player.IsAlive() && !player.IsMorphed())
                        player.cosmetics.nameText.text += suffix;

                    if (Helpers.IsMorphlingTargetAndMorphed(player) && !Helpers.ShouldHidePlayerName(Morphling.Player))
                        Morphling.Player.cosmetics.nameText.text += suffix;
                }

                if (MeetingHud.Instance != null)
                    foreach (PlayerVoteArea voteArea in MeetingHud.Instance.playerStates)
                        if (Arsonist.DousedPlayers.Any(player => player == voteArea.TargetPlayerId))
                            voteArea.NameText.text += suffix;
            }
        }

        static void SetPyromaniacDouseTag()
        {
            if (Pyromaniac.PyromaniacDictionary.Count <= 0) return;
            PlayerControl localplayer = PlayerControl.LocalPlayer;

            string suffix = Helpers.ColorString(Pyromaniac.Color, " <size=120%>¤</size>");

            foreach (Pyromaniac pyro in Pyromaniac.PyromaniacDictionary.Values)
            {
                if ((localplayer.Data.IsDead || localplayer == pyro.Player) && !pyro.Player.Data.IsDead)
                {
                    foreach (PlayerControl dousedPlayer in pyro.DousedPlayers.GetPlayerEnumerator())
                    {
                        if (dousedPlayer == null) continue;
                        if (!Helpers.ShouldHidePlayerName(dousedPlayer) && dousedPlayer.IsAlive() && !dousedPlayer.IsMorphed())
                            dousedPlayer.cosmetics.nameText.text += suffix;

                        if (Helpers.IsMorphlingTargetAndMorphed(dousedPlayer) && !Helpers.ShouldHidePlayerName(Morphling.Player))
                        {
                            Morphling.Player.cosmetics.nameText.text += suffix;
                        }
                    }
                }
            }
        }

        static void SetMedicTag()
        {
            PlayerControl localplayer = PlayerControl.LocalPlayer;
            string suffix = Helpers.ColorString(Medic.Color, " ♥");

            if (Medic.Player != null && !Medic.Player.Data.IsDead && Medic.Target != null)
            {
                if (localplayer.Data.IsDead || localplayer == Medic.Player)
                {
                    if (Medic.Target.IsAlive() && !Helpers.ShouldHidePlayerName(Medic.Target) && !Medic.Target.IsMorphed())
                    {
                        Medic.Target.cosmetics.nameText.text += suffix;
                    }

                    if (Helpers.IsMorphlingTargetAndMorphed(Medic.Target) && !Helpers.ShouldHidePlayerName(Morphling.Player))
                    {
                        Morphling.Player.cosmetics.nameText.text += suffix;
                    }
                }
            }
        }

        static void SetHeadHunterTag()
        {
            PlayerControl localplayer = PlayerControl.LocalPlayer;

            if (HeadHunter.Player != null && (localplayer == HeadHunter.Player || localplayer.Data.IsDead) && !HeadHunter.Player.Data.IsDead)
            {
                string suffix = Helpers.ColorString(HeadHunter.Color, " <size=120%>⊗</size>");

                foreach (PlayerControl player in HeadHunter.Bounties.GetPlayerEnumerator())
                {
                    if (player == null) continue;
                    if (player.IsAlive() && !Helpers.ShouldHidePlayerName(player) && !player.IsMorphed())
                        player.cosmetics.nameText.text += suffix;

                    if (Helpers.IsMorphlingTargetAndMorphed(player) && !Helpers.ShouldHidePlayerName(Morphling.Player))
                        Morphling.Player.cosmetics.nameText.text += suffix;

                    if (MeetingHud.Instance != null)
                        foreach (PlayerVoteArea p in MeetingHud.Instance.playerStates)
                            if (player.PlayerId == p.TargetPlayerId)
                                p.NameText.text += suffix;
                }
            }
        }


        static void SetNameTags()
        {
            SetFirstKillPlayersTag();
            SetExecutionerNameTag();
            SetRomanticNameTag();
            SetVengefulRomanticNameTag();
            SetParityCop();
            SetBomberTags();
            SetNightmareTags();
            SetNightmareParalyzedPlayerTag();
            SetShadeTags();
            SetArsonistDouseTags();
            SetMedicTag();
            SetHeadHunterTag();
            SetPyromaniacDouseTag();
            SetParasiteTags();
        }

        static void UpdateShielded()
        {
            if (Guardian.Player == null || Guardian.Player.Data.IsDead && Guardian.ShieldFadesOnDeath)
            {
                Guardian.Shielded = null;
            }
        }

        static void TimerUpdate()
        {
            float dt = Time.deltaTime;
            MapOptions.Meetingtime -= dt;
            Scavenger.CorpsesTrackingTimer -= dt;
            Guardian.ShieldVisibilityTimer += dt;
        }

        static void UpdateImpostorKillButton(HudManager __instance)
        {
            if (PlayerControl.LocalPlayer.Data.Role.IsImpostor)
                __instance.KillButton.Hide();
        }

        static void UpdateImpostorVentButton(HudManager __instance)
        {
            if (!PlayerControl.LocalPlayer.Data.Role.IsImpostor || (__instance.UseButton == null && __instance.PetButton == null)) return;

            __instance.ImpostorVentButton.transform.localPosition = __instance.UseButton.transform.localPosition + CustomButton.ButtonPositions.UpperRow2;
        }

        static void UpdateUseButton(HudManager __instance)
        {
            if (MeetingHud.Instance) __instance.UseButton.Hide();
        }

        static void UpdateSabotageButton(HudManager __instance)
        {
            if (MeetingHud.Instance) __instance.SabotageButton.Hide();
        }

        static void UpdateHauntButton(HudManager __instance)
        {
            if (MeetingHud.Instance) __instance.AbilityButton.Hide();
        }

        static void UpdateReportButton(HudManager __instance)
        {
            var localPlayer = PlayerControl.LocalPlayer;
            if (localPlayer.Data.IsDead) return;

            foreach (var collider2D in Physics2D.OverlapCircleAll(localPlayer.GetTruePosition(), localPlayer.MaxReportDistance, Constants.PlayersOnlyMask))
            {
                if (collider2D.tag == "DeadBody")
                {
                    var component = collider2D.GetComponent<DeadBody>();
                    if (component?.Reported == false)
                    {
                        if (!PhysicsHelpers.AnythingBetween(localPlayer.GetTruePosition(), component.TruePosition, Constants.ShipAndAllObjectsMask, false) && !Minigame.Instance)
                        {
                            __instance.ReportButton.graphic.color = __instance.ReportButton.buttonLabelText.color = Palette.EnabledColor;
                            __instance.ReportButton.graphic.material.SetFloat(Shader.PropertyToID("_Desat"), 0f);
                        }
                        else
                        {
                            __instance.ReportButton.graphic.color = __instance.ReportButton.buttonLabelText.color = Palette.DisabledClear;
                            __instance.ReportButton.graphic.material.SetFloat(Shader.PropertyToID("_Desat"), 1f);
                        }
                        if (component.Reported) break;
                    }
                }
            }
        }


        static void Postfix(HudManager __instance)
        {
            if (LobbyBehaviour.Instance)
                __instance.ImpostorVentButton.gameObject.SetActive(false);

            if (!Helpers.GameStarted || Helpers.IsHideAndSeek) return;
            __instance.MapButton.gameObject.SetActive(true);

            CustomButton.HudUpdate();
            ResetNameTagsAndColors();
            SetNameColors();
            UpdateShielded();
            UpdateImpostorKillButton(__instance);
            UpdateImpostorVentButton(__instance);
            UpdateReportButton(__instance);
            TimerUpdate();
            VentTrap.UpdateVentTrapPerPlayer();

            // Meeting hide buttons if needed (used for the map usage, because closing the map would show buttons)
            UpdateSabotageButton(__instance);
            UpdateUseButton(__instance);
            UpdateHauntButton(__instance);
            UpdateMiniMap();

            // Update Role Description
            RefreshRoleDescription();

            //NK
            NeutralKillerSetTarget();

            // Executioner
            if (Executioner.ConvertsImmediately)
                Executioner.ExecutionerCheckPromotion();

            // Morphling and Camouflager
            MorphlingAndCamouflagerUpdate();

            // Shade Traces
            ShadeTrace.UpdateAll();

            // Hide Pets Update
            PetsUpdate();

            SetNameTags();

        }
    }
}
