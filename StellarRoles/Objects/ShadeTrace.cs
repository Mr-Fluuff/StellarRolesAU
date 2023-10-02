using StellarRoles.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StellarRoles.Objects
{
    public class ShadeTrace
    {
        public static readonly List<ShadeTrace> Traces = new();

        private readonly GameObject TraceGameObject;
        private float TimeRemaining;

        public ShadeTrace(Vector2 p, float duration = 1f)
        {
            TraceGameObject = new GameObject("ShadeTrace");
            TraceGameObject.AddSubmergedComponent(SubmergedCompatibility.Classes.ElevatorMover);
            Vector3 position = new(p.x, p.y, PlayerControl.LocalPlayer.transform.localPosition.z + 0.001f); // just behind player
            TraceGameObject.transform.position = position;
            TraceGameObject.transform.localPosition = position;

            SpriteRenderer traceRenderer = TraceGameObject.AddComponent<SpriteRenderer>();
            traceRenderer.sprite = Shade.GetShadeEvidenceSprite();

            TimeRemaining = duration;

            float fadeOutDuration = 1f;
            if (fadeOutDuration > duration) fadeOutDuration = 0.5f * duration;
            FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(duration, new Action<float>((p) =>
            {
                float interP = 0f;
                if (p < (duration - fadeOutDuration) / duration)
                    interP = 0f;
                else 
                    interP = (p * duration + fadeOutDuration - duration) / fadeOutDuration;

                if (traceRenderer) 
                    traceRenderer.color = new Color(traceRenderer.color.r, traceRenderer.color.g, traceRenderer.color.b, Mathf.Clamp01(1 - interP));
            })));

            TraceGameObject.SetActive(true);
            Traces.Add(this);
        }

        public void Update()
        {
            TimeRemaining -= Time.deltaTime;

            if (TimeRemaining <= 0)
            {
                TraceGameObject.SetActive(false);
                Traces.Remove(this);
            }
        }

        public static void UpdateAll()
        {
            foreach (ShadeTrace trace in Traces)
            {
                trace.Update();
            }
        }

        public static void ClearTraces()
        {
            foreach (ShadeTrace traceCurrent in Traces)
            {
                traceCurrent.TraceGameObject.SetActive(false);
                UnityEngine.Object.Destroy(traceCurrent.TraceGameObject);
            }
            Traces.Clear();
        }
    }
}
