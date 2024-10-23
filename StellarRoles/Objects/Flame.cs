using Reactor.Utilities.Extensions;
using StellarRoles.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StellarRoles.Objects
{
    public class Flame
    {
        public GameObject FlameBoi;
        private static readonly Sprite[] FlameSprites = new Sprite[32];
        public static List<Flame> FlameList = new();
        private SpriteRenderer Renderer;

        public static Sprite GetFlameAnimation(int index)
        {
            try
            {
                if (FlameSprites == null || FlameSprites.Length == 0) return null;
                index = Mathf.Clamp(index, 0, FlameSprites.Length - 1);
                if (FlameSprites[index] == null)
                    FlameSprites[index] = Helpers.LoadSpriteFromResources($"StellarRoles.Resources.CustomLobby.CandleFlame.Flame_0{index + 1:00}.png", 100f);
                return FlameSprites[index];
            }
            catch (Exception ex)
            {
                Helpers.Log(LogLevel.Error, "Flame Animation Error: " + ex.StackTrace);
                return null;
            }
        }

        [HideFromIl2Cpp]
        public static GameObject CreateFlame()
        {
            var random = UnityEngine.Random.Range(0f, 6f);
            var flame = new Flame();
            flame.StartAnimation(random);
            return flame.FlameBoi;
        }

        [HideFromIl2Cpp]
        public void StartAnimation(float random)
        {
            if (FlameBoi == null || Helpers.GameStarted || !FlameBoi.active) return;
            var randomn = UnityEngine.Random.Range(0.9f, 1.8f);


            if (random == 0) Animation(randomn);
            else
            {
                random.DelayedAction(() => { Animation(randomn); });
            }
        }

        [HideFromIl2Cpp]
        public void Animation(float random)
        {
            if (FlameBoi == null || Helpers.GameStarted) return;

            HudManager.Instance.StartCoroutine(Effects.Lerp(random, new Action<float>((p) =>
            {
                if (Renderer != null)
                    Renderer.sprite = GetFlameAnimation((int)(p * FlameSprites.Length));

                if (p == 1f)
                {
                    Renderer.sprite = GetFlameAnimation(0);
                    StartAnimation(0);
                }

            })));
        }

        private Flame()
        {
            FlameBoi = new GameObject("NewFlame");
            Renderer = FlameBoi.AddComponent<SpriteRenderer>();
            Renderer.sprite = GetFlameAnimation(0);
            FlameBoi.SetActive(true);
            FlameList.Add(this);
        }

        private static void PreloadSprites()
        {
            for (int i = 0; i < FlameSprites.Length; i++)
                _ = GetFlameAnimation(i);
        }

        private void RemoveFlame()
        {
            this.FlameBoi.Destroy();
            FlameList.Remove(this);
        }

        public static void ClearAllFlames()
        {
            if (FlameList.Count <= 0) return;

            foreach (var f in FlameList)
            {
                if (f.FlameBoi != null)
                {
                    f.RemoveFlame();
                }
            }
        }

        public static void CreateFlames(GameObject instance)
        {
            FlameList.Clear();
            PreloadSprites();

            for (int i = 0; i < 11; i++)
            {
                var FlameBoi = CreateFlame();
                bool flipped = i % 2 == 0;
                FlameBoi.name = "Flame " + i;
                FlameBoi.transform.position = FlamePositions[i];
                if (flipped)
                {
                    FlameBoi.transform.Rotate(0, 180, 0);
                    FlameBoi.transform.position += new Vector3(0.04f, 0, 0);
                }
                FlameBoi.gameObject.transform.SetParent(instance.transform);
            }
        }

        public static List<Vector3> FlamePositions = new() 
        {
            new Vector3(3.49f, 2.36f, 0.09f),
            new Vector3(4.49f, 2.34f, 0.09f),
            new Vector3(4.86f, 2.43f, 0.09f),
            new Vector3(5.02f, 2.39f, 0.09f),
            new Vector3(5.67f, 2.25f, 0.09f),
            new Vector3(8.82f, 2.59f, 0.09f),
            new Vector3(9.79f, 2.41f, 0.09f),
            new Vector3(9.97f, 2.52f, 0.09f),
            new Vector3(9.97f, -0.06f, 0.09f),
            new Vector3(3.94f, 0.19f, 0.0005f),
            new Vector3(2.95f, 0.31f, 0.0002f)
        };
    }
}