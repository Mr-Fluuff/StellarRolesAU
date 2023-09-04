using HarmonyLib;
using StellarRoles.Objects;
using StellarRoles.Utilities;
using System;
using UnityEngine;

namespace StellarRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
    internal class DetectiveButtons
    {
        public static bool Initialized;
        public static CustomButton DetectiveButton;

        public static void DetectiveIvestigateButton()
        {
            DetectiveButton = new CustomButton(
                () =>
                {
                    if (Detective.Target != null)
                    {
                        Detective.InspectTarget = Detective.Target;
                        DetectiveButton.HasEffect = true;
                    }
                },
                () =>
                {

                    bool isDetective = Detective.Player == PlayerControl.LocalPlayer;
                    return isDetective && !PlayerControl.LocalPlayer.Data.IsDead && Detective.InspectsPerRound > 0;
                },
                () =>
                {
                    if (DetectiveButton.IsEffectActive && Detective.Target != Detective.InspectTarget)
                    {
                        Detective.InspectTarget = null;
                        DetectiveButton.Timer = 0f;
                        DetectiveButton.IsEffectActive = false;
                    }
                    DetectiveButton.ActionButton.buttonLabelText.SetOutlineColor(Color.cyan);
                    Helpers.ShowTargetNameOnButtonExplicit(null, DetectiveButton, $"INSPECT - {Detective.InspectsPerRound}");


                    bool canInspectFresh = Detective.TargetIsFresh && Detective.InspectsPerRound > 0;
                    return Detective.Target != null && PlayerControl.LocalPlayer.CanMove
                    && (!Detective.TargetIsFresh || canInspectFresh)
                    && !DetectiveAbilities.IsRoleBlocked();
                },
                () =>
                {
                    DetectiveButton.Timer = DetectiveButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer);
                    DetectiveButton.IsEffectActive = false;
                    Detective.InspectTarget = null;
                    Detective.InspectsPerRound = Detective.InspectsPerRoundDefault;
                    Detective.QuestionsForPlayer.Clear();
                },
                Detective.GetQuestionSprite(),
                CustomButton.ButtonPositions.UpperRow1,
                "ActionQuaternary",
                true,
                Detective.CalculateDetectiveDuration(),
                () =>
                {
                    RPCProcedure.Send(CustomRPC.PsychicAddCount);
                    DetectiveButton.Timer = DetectiveButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer);
                    if (Detective.Target?.Player == null)
                        return;
                    PlayerControl targetPlayer = Detective.Target.Player;
                    if (Detective.TargetIsFresh)
                    {
                        if (!Detective.BodyToInspects.TryGetValue(targetPlayer.PlayerId, out int inspects))
                            Detective.BodyToInspects[targetPlayer.PlayerId] = inspects = 4;
                        if (inspects > 0)
                            //We can't erase dead bodies so this prevents repeat wasted charges for no info
                            Detective.InspectsPerRound--;
                        Detective.BodyToInspects[targetPlayer.PlayerId]--;
                    }
                    else
                    {
                        if (!Detective.CrimeSceneToInspect.ContainsKey(targetPlayer.PlayerId))
                            Detective.CrimeSceneToInspect[targetPlayer.PlayerId] = 3;
                        else
                            Detective.CrimeSceneToInspect[targetPlayer.PlayerId]--;

                        Detective.InspectsPerRound--;
                    }

                    FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, Detective.DetectiveQuestion());

                    // Remove soul
                    if (Detective.IsCrimeSceneEnabled && Detective.CrimeSceneToInspect.ContainsKey(targetPlayer.PlayerId) && Detective.CrimeSceneToInspect[targetPlayer.PlayerId] == 0f)
                    {
                        float closestDistance = float.MaxValue;
                        SpriteRenderer target = null;

                        Detective.DeadBodies.RemoveAll((tuple) => tuple.Item1 == Detective.Target);

                        foreach (SpriteRenderer rend in Detective.CrimeScenes)
                        {
                            float distance = Vector2.Distance(rend.transform.position, PlayerControl.LocalPlayer.GetTruePosition());
                            if (distance < closestDistance)
                            {
                                closestDistance = distance;
                                target = rend;
                            }
                        }

                        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(5f, new Action<float>((p) =>
                        {
                            if (target != null)
                            {
                                Color tmp = target.color;
                                tmp.a = Mathf.Clamp01(1 - p);
                                target.color = tmp;
                            }
                            if (p == 1f && target != null && target.gameObject != null)
                                UnityEngine.Object.Destroy(target.gameObject);
                        })));

                        Detective.CrimeScenes.Remove(target);
                    }
                }
            );
            Initialized = true;
            SetDetectiveCooldowns();
        }

        public static void SetDetectiveCooldowns()
        {
            if (!Initialized)
            {
                DetectiveIvestigateButton();
            }

            DetectiveButton.EffectDuration = Detective.CalculateDetectiveDuration();
            DetectiveButton.MaxTimer = Detective.Cooldown * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer);
        }

        public static void Postfix()
        {
            Initialized = false;
            DetectiveIvestigateButton();
        }
    }
}
