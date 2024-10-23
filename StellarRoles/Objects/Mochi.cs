using StellarRoles.Utilities;
using System;
using UnityEngine;

namespace StellarRoles.Objects
{
    public class Mochi
    {
        public static GameObject MochiGameObject;
        private static readonly Sprite[] MochiAnimation = new Sprite[10];
        private static SpriteRenderer Renderer;

        public static Sprite GetMochiAnimation(int index)
        {
            try
            {
                if (MochiAnimation == null || MochiAnimation.Length == 0) return null;
                index = Mathf.Clamp(index, 0, MochiAnimation.Length - 1);
                if (MochiAnimation[index] == null)
                    MochiAnimation[index] = Helpers.LoadSpriteFromResources($"StellarRoles.Resources.MochiBlink.Mochi_0{index + 1:00}.png", 175f);
                return MochiAnimation[index];
            }
            catch (Exception ex)
            {
                Helpers.Log(LogLevel.Error, "Mochi Animation Error: " + ex.StackTrace);
                return null;
            }
        }

        [HideFromIl2Cpp]
        public static void CreateMochi()
        {
            try
            {
                if (MochiGameObject == null)
                    _ = new Mochi();
                StartAnimation();
            }
            catch (Exception ex)
            {
                Helpers.Log(LogLevel.Error, "Error Creating Mochi: " + ex.StackTrace);
            }
        }

        [HideFromIl2Cpp]
        public static void StartAnimation()
        {
            if (MochiGameObject == null || Helpers.GameStarted) return;

            float time = StellarRoles.rnd.Next(15, 20);

            time.DelayedAction(Animation);
        }

        [HideFromIl2Cpp]
        public static void Animation()
        {
            if (MochiGameObject == null || Helpers.GameStarted) return;

            HudManager.Instance.StartCoroutine(Effects.Lerp(1.5f, new Action<float>((p) =>
            {
                if (Renderer != null)
                    Renderer.sprite = GetMochiAnimation((int)(p * MochiAnimation.Length));

                if (p == 1f)
                {
                    Renderer.sprite = GetMochiAnimation(0);
                    StartAnimation();
                }

            })));
        }



        private Mochi()
        {
            MochiGameObject = new GameObject("Mochi");
            Renderer = MochiGameObject.AddComponent<SpriteRenderer>();
            Renderer.sprite = GetMochiAnimation(0);
            MochiGameObject.SetActive(true);
        }

        private static void PreloadSprites()
        {
            for (int i = 0; i < MochiAnimation.Length; i++)
                GetMochiAnimation(i);
        }

        public static void ClearMochi()
        {
            PreloadSprites();
            MochiGameObject = null;
        }
    }
}