using HarmonyLib;
using StellarRoles.Objects;
using StellarRoles.Utilities;
using UnityEngine;

namespace StellarRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class BountyHunterAbilities
    {
        public static void Postfix(HudManager __instance)
        {
            if (!Helpers.GameStarted || PlayerControl.LocalPlayer != BountyHunter.Player) return;
            bountyHunterUpdate();
        }

        static void bountyHunterUpdate()
        {
            if (BountyHunter.Player.Data.IsDead)
            {
                if (BountyHunter.Arrow != null)
                    Object.Destroy(BountyHunter.Arrow.Object);
                BountyHunter.Arrow = null;
                if (BountyHunter.CooldownText != null && BountyHunter.CooldownText.gameObject != null)
                    Object.Destroy(BountyHunter.CooldownText.gameObject);
                BountyHunter.CooldownText = null;
                BountyHunter.Bounty = null;
                foreach (PoolablePlayer p in MapOptions.PlayerIcons.Values)
#pragma warning disable IDE0031 // Use null propagation
                    // suppressed due to Unity being weird and not liking optional chaining here
                    if (p.gameObject != null) p.gameObject.SetActive(false);
#pragma warning restore IDE0031 // Use null propagation
                return;
            }

            if (Helpers.IsCommsActive() && MapOptions.ImposterAbiltiesRoleBlock)
            {
                BountyHunter.Arrow?.Object.SetActive(false);
                return;
            }

            BountyHunter.ArrowUpdateTimer -= Time.deltaTime;
            BountyHunter.BountyUpdateTimer -= Time.deltaTime;

            if (BountyHunter.BountyUpdateTimer <= 0f)
            {
                // Set new bounty
                BountyHunter.Bounty = null;
                BountyHunter.ArrowUpdateTimer = 0f; // Force arrow to update
                BountyHunter.BountyUpdateTimer = BountyHunter.BountyDuration;
                System.Collections.Generic.List<PlayerControl> possibleTargets = BountyHunter.PossibleTargets();

                BountyHunter.Bounty = possibleTargets[StellarRoles.rnd.Next(0, possibleTargets.Count)];
                if (BountyHunter.Bounty == null) return;

                // Show poolable player
                if (FastDestroyableSingleton<HudManager>.Instance != null && FastDestroyableSingleton<HudManager>.Instance.UseButton != null)
                {
                    foreach (PoolablePlayer pp in MapOptions.PlayerIcons.Values) pp.gameObject.SetActive(false);
                    if (MapOptions.PlayerIcons.ContainsKey(BountyHunter.Bounty.PlayerId) && MapOptions.PlayerIcons[BountyHunter.Bounty.PlayerId].gameObject != null)
                        MapOptions.PlayerIcons[BountyHunter.Bounty.PlayerId].gameObject.SetActive(true);
                }
            }

            if (MapOptions.FirstKillName == BountyHunter.Bounty.Data.PlayerName || MapOptions.FirstKillPlayer == BountyHunter.Bounty) BountyHunter.BountyUpdateTimer = 0f;

            // Hide in meeting
            if ((MeetingHud.Instance || Impostor.IsRoleAblilityBlocked()) && MapOptions.PlayerIcons.ContainsKey(BountyHunter.Bounty.PlayerId) && MapOptions.PlayerIcons[BountyHunter.Bounty.PlayerId].gameObject != null)
                MapOptions.PlayerIcons[BountyHunter.Bounty.PlayerId].gameObject.SetActive(false);

            // Update Cooldown Text
            if (BountyHunter.CooldownText != null)
            {
                BountyHunter.CooldownText.text = Mathf.CeilToInt(Mathf.Clamp(BountyHunter.BountyUpdateTimer, 0, BountyHunter.BountyDuration)).ToString();
                BountyHunter.CooldownText.gameObject.SetActive(!MeetingHud.Instance && !Impostor.IsRoleAblilityBlocked());  // Show if not in meeting
            }

            // Update Arrow
            if (BountyHunter.ShowArrow && BountyHunter.Bounty != null)
            {
                BountyHunter.Arrow ??= new Arrow(Color.red);
                if (BountyHunter.ArrowUpdateTimer <= 0f)
                {
                    BountyHunter.Arrow.Update(BountyHunter.Bounty.transform.position);
                    BountyHunter.ArrowUpdateTimer = BountyHunter.ArrowUpdateIntervall;
                }
                BountyHunter.Arrow.Object.SetActive(!Impostor.IsRoleAblilityBlocked());
                BountyHunter.Arrow.Update();
            }
        }
    }
}
