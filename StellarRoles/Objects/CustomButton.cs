using StellarRoles.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StellarRoles.Objects
{
    public class CustomButton
    {
        public static readonly List<CustomButton> Buttons = new();
        public readonly ActionButton ActionButton;

        public float MaxTimer { get; set; } = float.MaxValue;
        public float Timer { get; set; } = 0f;
        public bool HasEffect { get; set; }
        public bool IsEffectActive { get; set; } = false;
        public bool ShowButtonText { get; set; } = false;
        public float EffectDuration { get; set; }
        public Sprite Sprite { get; set; }


        private readonly Vector3 PositionOffset;
        private readonly Action OnClickHandler;
        private readonly Action OnMeetingEndsHandler;
        private readonly Func<bool> ShouldShowButton;
        private readonly Func<bool> CouldUse;
        private readonly Action OnEffectEnds;
        private readonly bool Mirror;
        private readonly string ActionName;
        private readonly string ButtonText;
        private readonly bool SeeInMeeting = false;

        private static readonly int Desat = Shader.PropertyToID("_Desat");

        public static class ButtonPositions
        {
            public static readonly Vector3 LowerRow3 = new(-2f, 0f, 0);  // Not usable for imps beacuse of new button positions!
            public static readonly Vector3 LowerRow4 = new(-3f, 0f, 0);
            public static readonly Vector3 LowerRow5 = new(-4f, 0f, 0);

            public static readonly Vector3 UpperRow1 = new(0f, 1f, 0f);  // Not usable for imps beacuse of new button positions!
            public static readonly Vector3 UpperRow2 = new(-1f, 1f, 0f);  // Not usable for imps beacuse of new button positions!
            public static readonly Vector3 UpperRow3 = new(-2f, 1f, 0f);
            public static readonly Vector3 UpperRow4 = new(-3f, 1f, 0f);
        }

        private CustomButton(
            Action onClick,
            Func<bool> hasButton,
            Func<bool> couldUse,
            Action onMeetingEnds,
            Sprite sprite,
            Vector3 positionOffset,
            bool hasEffect,
            float effectDuration,
            Action onEffectEnds,
            bool mirror = false,
            string buttonText = "",
            bool seeInMeeting = false
        )
        {
            OnClickHandler = onClick;
            ShouldShowButton = hasButton;
            CouldUse = couldUse;
            PositionOffset = positionOffset;
            OnMeetingEndsHandler = onMeetingEnds;
            HasEffect = hasEffect;
            EffectDuration = effectDuration;
            OnEffectEnds = onEffectEnds;
            Sprite = sprite;
            Mirror = mirror;
            ButtonText = buttonText;
            SeeInMeeting = seeInMeeting;
            Timer = 16.2f;
            Buttons.Add(this);
            ActionButton = UnityEngine.Object.Instantiate(HudManager.Instance.KillButton, HudManager.Instance.KillButton.transform.parent);
            PassiveButton button = ActionButton.GetComponent<PassiveButton>();
            ShowButtonText = ActionButton.graphic.sprite == sprite || buttonText != "";
            button.OnClick = new Button.ButtonClickedEvent();
            button.OnClick.AddListener((UnityEngine.Events.UnityAction)OnClickEvent);

            SetActive(false);
        }

        public CustomButton(
            Action onClick,
            Func<bool> hasButton,
            Func<bool> couldUse,
            Action onMeetingEnds,
            Sprite sprite,
            Vector3 positionOffset,
            string actionName,
            bool hasEffect,
            float effectDuration,
            Action onEffectEnds,
            bool mirror = false,
            string buttonText = "",
            bool seeInMeeting = false
        ) : this(onClick, hasButton, couldUse, onMeetingEnds, sprite, positionOffset, hasEffect, effectDuration, onEffectEnds, mirror, buttonText, seeInMeeting)
        {
            ActionName = actionName;
        }

        public CustomButton(
            Action onClick,
            Func<bool> hasButton,
            Func<bool> couldUse,
            Action onMeetingEnds,
            Sprite sprite,
            Vector3 positionOffset,
            string actionName,
            bool mirror = false,
            string buttonText = "",
            bool seeInMeeting = false
        ) : this(onClick, hasButton, couldUse, onMeetingEnds, sprite, positionOffset, actionName, false, 0f, () => { }, mirror, buttonText, seeInMeeting) { }

        public void OnClickEvent()
        {
            if (Timer < 0f && ShouldShowButton() && CouldUse())
            {
                ActionButton.graphic.color = new Color(1f, 1f, 1f, 0.3f);
                OnClickHandler();

                if (HasEffect && !IsEffectActive)
                {
                    Timer = EffectDuration;
                    ActionButton.cooldownTimerText.color = new Color(0F, 0.8F, 0F);
                    IsEffectActive = true;
                }
            }
        }

        public static void HudUpdate()
        {
            Buttons.RemoveAll(item => item.ActionButton == null);
            foreach (CustomButton button in Buttons)
                try
                {
                    button.Update();
                }
                catch { }
        }

        public static void MeetingEndedUpdate()
        {
            Buttons.RemoveAll(item => item.ActionButton == null);
            foreach (CustomButton button in Buttons)
            {
                try
                {
                    button.OnMeetingEndsHandler();
                    button.Update();
                }
                catch { }
            }
        }

        public void SetActive(bool isActive)
        {
            ActionButton.gameObject.SetActive(isActive);
            ActionButton.graphic.enabled = isActive;
        }

        public void Update()
        {
            PlayerControl localPlayer = PlayerControl.LocalPlayer;
            bool moveable = localPlayer.moveable;
            bool nightmare = localPlayer.IsNightmare(out _) && (NightmareButtons.NightmareBlindButton.ActionButton == ActionButton || NightmareButtons.NightmareParalyzeButton.ActionButton == ActionButton);
            bool gopher = Gopher.Players.Contains(localPlayer.PlayerId) && localPlayer.inVent;
            bool undertaker = localPlayer == Undertaker.Player && (HudManagerStartPatch.ImpKillButton.ActionButton == ActionButton || RogueButtons.RogueKillButton.ActionButton == ActionButton);

            if (localPlayer.Data == null || (MeetingHud.Instance && !SeeInMeeting) || ExileController.Instance || !ShouldShowButton())
            {
                SetActive(false);
                return;
            }
            HudManager hudManager = HudManager.Instance;
            SetActive(hudManager.UseButton.isActiveAndEnabled || hudManager.PetButton.isActiveAndEnabled || (MeetingHud.Instance && SeeInMeeting));
            ActionButton.graphic.sprite = Sprite;
            if (ShowButtonText && ButtonText != "")
                ActionButton.OverrideText(ButtonText);
            ActionButton.buttonLabelText.enabled = ShowButtonText; // Only show the text if it's a kill button
            if (hudManager.UseButton != null)
            {
                Vector3 pos = hudManager.UseButton.transform.localPosition;
                if (Mirror)
                {
                    float aspect = Camera.main.aspect;
                    float safeOrthographicSize = CameraSafeArea.GetSafeOrthographicSize(Camera.main);
                    float xpos = 0.05f - safeOrthographicSize * aspect * 1.70f;
                    pos = new Vector3(xpos, pos.y, pos.z);
                }
                ActionButton.transform.localPosition = pos + PositionOffset;
            }

            if (CouldUse())
            {
                ActionButton.graphic.color = ActionButton.buttonLabelText.color = Palette.EnabledColor;
                ActionButton.graphic.material.SetFloat(Desat, 0f);
            }
            else
            {
                ActionButton.graphic.color = ActionButton.buttonLabelText.color = Palette.DisabledClear;
                ActionButton.graphic.material.SetFloat(Desat, 1f);
            }

            if (Timer >= 0)
            {
                if ((HasEffect && IsEffectActive) || nightmare || gopher || Ascended.AscendedMiner(localPlayer))
                {
                    Timer -= Time.deltaTime;
                }
                else if (!localPlayer.inVent && moveable)
                {
                    if (Undertaker.DeadBodyDragged == null || !undertaker)
                        Timer -= Time.deltaTime;
                }
            }
            if (Timer <= 0 && HasEffect && IsEffectActive)
            {
                IsEffectActive = false;
                ActionButton.cooldownTimerText.color = Palette.EnabledColor;
                OnEffectEnds();
            }

            ActionButton.SetCoolDown(Timer, (HasEffect && IsEffectActive) ? EffectDuration : MaxTimer);

            // Trigger OnClickEvent if the hotkey is being pressed down
            if (!ActionName.IsNullOrWhiteSpace() && Rewired.ReInput.players.GetPlayer(0).GetButtonDown(ActionName)) OnClickEvent();
        }
    }
}