using HarmonyLib;
using StellarRoles.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace StellarRoles
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    internal class PlayerOutlines
    {
        public static void Postfix(PlayerControl __instance)
        {
            if (!Helpers.GameStarted || !__instance.AmOwner)
                return;

            BaseOutlines();
            NeutralKillerOutlines();
            GuardianOutlines();
            MedicOutlines();
            ParityCopOutlines();
            SheriffOutlines();
            TrackerOutlines();
            ArsonistOutlines();
            RomanticOutlines();
            VengefulRomanticOutlines();
            HeadHunterOutlines();
            NightmareOutlines();
            PyromaniacOutlines();
            RuthlessRomanticOutlines();
            RefugeeOutlines();
            ImpostorSetTarget();
            BomberSetTarget();
            BombedSetTarget();
        }

        static void BaseOutlines()
        {
            foreach (PlayerControl target in PlayerControl.AllPlayerControls.GetFastEnumerator())
            {
                if (target.cosmetics?.currentBodySprite?.BodySprite == null) continue;

                bool isMorphedMorphling = target == Morphling.Player && Morphling.MorphTarget != null && Morphling.MorphTimer > 0f;
                bool isParasiteInfectedPlayer = target == Parasite.Controlled;
                bool hasVisibleShield = false;
                Color color = Guardian.ShieldedColor;
                if (Camouflager.CamouflageTimer <= 0f)
                {
                    if ((isMorphedMorphling ? Morphling.MorphTarget : isParasiteInfectedPlayer ? Parasite.Player : target) == Guardian.Shielded)
                    {
                        bool isDelayed = Guardian.ShieldVisibilityTimer < Guardian.ShieldVisibilityDelay;
                        hasVisibleShield = Guardian.CanSeeShield(PlayerControl.LocalPlayer);
                        bool inRange = true;
                        if (Guardian.LimitedVisionShield)
                            inRange = Helpers.PlayerIsClose(target, Guardian.ShieldVisionRadius);

                        hasVisibleShield = (hasVisibleShield && !isDelayed) || PlayerControl.LocalPlayer == Guardian.Player;
                        hasVisibleShield = hasVisibleShield && !GuardianAbilities.IsRoleBlocked() && !Guardian.Shielded.Data.IsDead && inRange && !Guardian.Shielded.IsMushroomMixupActive();
                    }

                    if (MapOptions.ShieldFirstKill && Helpers.IsFirstKilled(isMorphedMorphling ? Morphling.MorphTarget : target))
                    {
                        hasVisibleShield = true;
                        color = Color.blue;
                    }

                    if (PlayerControl.LocalPlayer == Romantic.Player && (target == Romantic.Lover || (isMorphedMorphling && Morphling.MorphTarget == Romantic.Lover)) && Romantic.IsVestActive)
                    {
                        hasVisibleShield = true;
                        color = Romantic.Color;
                    }

                }
                if (hasVisibleShield)
                {
                    target.cosmetics.currentBodySprite.BodySprite.material.SetFloat("_Outline", 1f);
                    target.cosmetics.currentBodySprite.BodySprite.material.SetColor("_OutlineColor", color);
                }
                else
                {
                    target.cosmetics.currentBodySprite.BodySprite.material.SetFloat("_Outline", 0f);
                }
            }
        }

        static void ImpostorSetTarget()
        {
            if (!PlayerControl.LocalPlayer.Data.Role.IsImpostor || !PlayerControl.LocalPlayer.CanMove || PlayerControl.LocalPlayer.Data.IsDead)
            { // !isImpostor || !canMove || isDead
                HudManager.Instance.KillButton.SetTarget(null);
                return;
            }

            PlayerControl target;
            if (Spy.Player == null)
                target = Helpers.SetTarget(true, true, canIncrease: true);
            else if (Spy.ImpostorsCanKillAnyone)
                target = Helpers.SetTarget(false, true, canIncrease: true);
            else
                target = Helpers.SetTarget(true, true, new List<PlayerControl>() { Spy.Player }, canIncrease: true);

            Impostor.CurrentTarget = target;
            Helpers.SetPlayerOutline(Impostor.CurrentTarget, Palette.ImpostorRed);
        }

        static void GuardianOutlines()
        {
            if (PlayerControl.LocalPlayer == Guardian.Player && !Guardian.UsedShield)
                Helpers.SetPlayerOutline(Guardian.CurrentTarget, Guardian.ShieldedColor);
        }

        static void SheriffOutlines()
        {
            if (PlayerControl.LocalPlayer == Sheriff.Player)
                Helpers.SetPlayerOutline(Sheriff.CurrentTarget, Sheriff.Color);
        }

        static void RuthlessRomanticOutlines()
        {
            if (PlayerControl.LocalPlayer.IsRuthlessRomanticAndVestActive())
                Helpers.SetPlayerOutline(PlayerControl.LocalPlayer, RuthlessRomantic.Color);
        }

        static void RefugeeOutlines()
        {
            if (PlayerControl.LocalPlayer.IsRefugeeAndVestActive())
                Helpers.SetPlayerOutline(PlayerControl.LocalPlayer, Refugee.Color);
        }

        static void PyromaniacOutlines()
        {
            if (PlayerControl.LocalPlayer.IsPyromaniac(out Pyromaniac pyromaniac))
                Helpers.SetPlayerOutline(pyromaniac.CurrentTarget, Pyromaniac.Color);
        }

        static void NightmareOutlines()
        {
            if (PlayerControl.LocalPlayer.IsNightmare(out Nightmare nightmare))
                Helpers.SetPlayerOutline(nightmare.AbilityCurrentTarget, Nightmare.Color);
        }

        static void HeadHunterOutlines()
        {
            if (PlayerControl.LocalPlayer == HeadHunter.Player)
                Helpers.SetPlayerOutline(HeadHunter.CurrentTarget, Palette.ImpostorRed);
        }

        static void RomanticOutlines()
        {
            if (PlayerControl.LocalPlayer == Romantic.Player)
                Helpers.SetPlayerOutline(Romantic.CurrentTarget, Romantic.Color);
        }

        static void VengefulRomanticOutlines()
        {
            if (PlayerControl.LocalPlayer == VengefulRomantic.Player)
                Helpers.SetPlayerOutline(VengefulRomantic.CurrentTarget, VengefulRomantic.Color);
        }

        static void ArsonistOutlines()
        {
            if (PlayerControl.LocalPlayer == Arsonist.Player)
                Helpers.SetPlayerOutline(Arsonist.CurrentTarget, Arsonist.Color);
        }

        static void TrackerOutlines()
        {
            if (PlayerControl.LocalPlayer == Tracker.Player)
                Helpers.SetPlayerOutline(Tracker.CurrentTarget, Tracker.Color);
        }

        static void ParityCopOutlines()
        {
            if (PlayerControl.LocalPlayer.IsParityCop(out ParityCop parityCop))
                Helpers.SetPlayerOutline(parityCop.CurrentTarget, ParityCop.Color);
        }

        static void MedicOutlines()
        {
            if (PlayerControl.LocalPlayer == Medic.Player)
                Helpers.SetPlayerOutline(Medic.CurrentTarget, Medic.Color);
        }

        static void NeutralKillerOutlines()
        {
            if (NeutralKiller.Players.Contains(PlayerControl.LocalPlayer))
                Helpers.SetPlayerOutline(NeutralKiller.CurrentTarget, NeutralKiller.Color);
        }

        static void BomberSetTarget()
        {
            if (Bomber.Player != PlayerControl.LocalPlayer) return;

            List<PlayerControl> players = new();
            foreach (PlayerControl player in PlayerControl.AllPlayerControls.GetFastEnumerator())
            {
                if (player.IsBombed() || player == MapOptions.FirstKillPlayer)
                {
                    players.Add(player);
                }
            }

            Bomber.AbilityCurrentTarget = Helpers.SetTarget(untargetablePlayers: players);
            Helpers.SetPlayerOutline(Bomber.AbilityCurrentTarget, Bomber.Color);
        }

        static void BombedSetTarget()
        {
            if (!PlayerControl.LocalPlayer.IsBombed(out Bombed bombed)) return;

            List<PlayerControl> players = new();
            foreach (PlayerControl player in PlayerControl.AllPlayerControls.GetFastEnumerator())
            {
                if (player.IsBombed() || player == MapOptions.FirstKillPlayer || player == bombed.LastBombed)
                {
                    players.Add(player);
                }
            }

            bombed.CurrentTarget = Helpers.SetTarget(untargetablePlayers: players);
            if (bombed.BombActive)
                Helpers.SetPlayerOutline(bombed.CurrentTarget, Palette.ImpostorRed);
        }
    }
}
