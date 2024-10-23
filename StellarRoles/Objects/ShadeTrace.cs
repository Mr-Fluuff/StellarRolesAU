using StellarRoles.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StellarRoles.Objects
{
    public class ShadeTrace
    {
        public static List<ShadeTrace> Traces = new();

        private readonly GameObject TraceGameObject;
        private float TimeRemaining;
        private int ID;

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
            ID = Traces.Count + 1;

            float fadeOutDuration = 1f;
            if (fadeOutDuration > duration) fadeOutDuration = 0.5f * duration;
            HudManager.Instance.StartCoroutine(Effects.Lerp(duration, new Action<float>((p) =>
            {
                float interP = 0f;
                if (p < (duration - fadeOutDuration) / duration)
                    interP = 0f;
                else
                    interP = (p * duration + fadeOutDuration - duration) / fadeOutDuration;

                float alpha = TraceGameObject.NearActiveMushroom() ? 0 : Mathf.Clamp01(1 - interP);

                if (traceRenderer)
                    traceRenderer.color = new Color(traceRenderer.color.r, traceRenderer.color.g, traceRenderer.color.b, alpha);
            })));

            TraceGameObject.SetActive(true);
            Traces.Add(this);
        }

        public static void UpdateAll()
        {
            List<ShadeTrace> tracesToRemove = new();

            for (int i = 0; i < Traces.Count; i++)
            {
                var trace = Traces[i];

                trace.TimeRemaining -= Time.deltaTime;

                if (trace.TimeRemaining <= 0 && tracesToRemove.Any(x => x.ID != trace.ID))
                {
                    tracesToRemove.Add(trace);
                }
            }

            foreach (var trace in tracesToRemove)
            {
                trace.TraceGameObject.SetActive(false);
                Traces.Remove(trace);
                UnityEngine.Object.Destroy(trace.TraceGameObject);
            }
        }

        public static void ClearTraces()
        {
            foreach (ShadeTrace trace in Traces)
            {
                trace.TraceGameObject.SetActive(false);
                UnityEngine.Object.Destroy(trace.TraceGameObject);
            }
            Traces = new();
        }
    }
}
