using HarmonyLib;
using Reactor.Utilities.Extensions;
using StellarRoles.Modules;
using StellarRoles.Patches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace StellarRoles.Objects
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class GameTimer
    {
        public static float MaxGameTime => CustomOptionHolder.GameTimer.GetFloat() * 60;
        public static bool Enabletimer => CustomOptionHolder.GameTimer.GetBool();
        public static bool TriggerTimesUpEndGame = false;
        public static float GameTime = 800f;
        public static bool _isCountingDown = false;
        public static TextMeshPro TimerText;

        public static void Postfix()
        {
            if (TimerText == null)
            {
                CreateTimerText();
            }

            var position = TimerText.gameObject.GetComponent<AspectPosition>();
            position.Alignment = AspectPosition.EdgeAlignments.LeftTop;
            position.DistanceFromEdge = new Vector3(4.5f, 0.11f, 0f) + (HelpMenu.Reference != null ? HelpMenu.Reference.transform.localPosition : Vector3.zero);
            TimerText.gameObject.SetActive(Enabletimer && Helpers.GameStarted);

            if (!_isCountingDown) return;
            GameTime -= Time.deltaTime;
            UpdateTimer(GameTime);

            if (GameTime <= 0.0f)
            {
                _isCountingDown = false;
                if (AmongUsClient.Instance.AmHost && Helpers.GameStarted)
                {
                    EndGame();
                }
            }
        }

        public static void UpdateTimer(float time)
        {
            TimeSpan ts = TimeSpan.FromSeconds(time);
            TimerText.text = ts.ToString(format: @"mm\:ss");
        }

        public static void CreateTimerText()
        {
            var font = GameObject.Find("PingTrackerTMP").GetComponent<TextMeshPro>();
            TimerText = UnityEngine.Object.Instantiate(font);
            TimerText.gameObject.GetComponent<PingTracker>().Destroy();
            TimerText.transform.SetParent(HudManager._instance.transform);
            TimerText.transform.localScale = Vector3.one * 1f;
            TimerText.gameObject.layer = 5;
            TimeSpan ts = TimeSpan.FromSeconds(GameTime);
            TimerText.text = ts.ToString(format: @"mm\:ss");

            TimerText.gameObject.SetActive(false);
        }

        public static void EndGame()
        {
            TriggerTimesUpEndGame = true;
        }
    }

    [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.ShowRole))]
    class StartGameTimer
    {
        public static void Prefix()
        {
            GameTimer._isCountingDown = false;
            GameTimer.TriggerTimesUpEndGame = false;
            if (GameTimer.Enabletimer)
            {
                GameTimer.GameTime = GameTimer.MaxGameTime;
            }
        }

        public static void Postfix()
        {
            if (GameTimer.Enabletimer)
            {
                GameTimer._isCountingDown = true;
            }
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.StartMeeting))]
    class StopGameTimerMeeting
    {
        public static void Postfix()
        {
            GameTimer._isCountingDown = false;
        }
    }
}
