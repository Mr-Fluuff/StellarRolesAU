using StellarRoles.Utilities;
using System;
using UnityEngine;

namespace StellarRoles.Objects
{
    public class GhostLad
    {
        public static GameObject GhostLadGameObject;
        private static readonly Sprite[] GhostAnimation = new Sprite[74];
        private static SpriteRenderer Renderer;

        public static Sprite GetGhostLadAnimation(int index)
        {
            try
            {
                if (GhostAnimation == null || GhostAnimation.Length == 0) return null;
                index = Mathf.Clamp(index, 0, GhostAnimation.Length - 1);
                if (GhostAnimation[index] == null)
                    GhostAnimation[index] = Helpers.LoadSpriteFromResources($"StellarRoles.Resources.GhostLad.GhostLad_0{index + 1:00}.png", 225f);
                return GhostAnimation[index];
            }
            catch (Exception ex)
            {
                Helpers.Log(LogLevel.Error, "Mochi Animation Error: " + ex.StackTrace);
                return null;
            }
        }

        [HideFromIl2Cpp]
        public static void CreateGhostLad()
        {
            try
            {
                if (GhostLadGameObject == null)
                    _ = new GhostLad();
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
            if (GhostLadGameObject == null || Helpers.GameStarted) return;

            0.5f.DelayedAction(Animation);
        }

        [HideFromIl2Cpp]
        public static void Animation()
        {
            if (GhostLadGameObject == null || Helpers.GameStarted) return;

            HudManager.Instance.StartCoroutine(Effects.Lerp(2f, new Action<float>((p) =>
            {
                if (Renderer != null)
                    Renderer.sprite = GetGhostLadAnimation((int)(p * GhostAnimation.Length));

                if (p == 1f)
                {
                    Renderer.sprite = GetGhostLadAnimation(0);
                    StartAnimation();
                }

            })));
        }



        private GhostLad()
        {
            GhostLadGameObject = new GameObject("GhostLad");
            Renderer = GhostLadGameObject.AddComponent<SpriteRenderer>();
            Renderer.sprite = GetGhostLadAnimation(0);
            GhostLadGameObject.SetActive(true);
        }

        private static void PreloadSprites()
        {
            for (int i = 0; i < GhostAnimation.Length; i++)
                _ = GetGhostLadAnimation(i);
        }

        public static void ClearGhost()
        {
            PreloadSprites();
            GhostLadGameObject = null;
        }
    }
}