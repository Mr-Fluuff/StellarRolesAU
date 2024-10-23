using StellarRoles.Modules;
using StellarRoles.Utilities;
using System;
using System.Linq;
using UnityEngine;

namespace StellarRoles.Objects
{
    public class Goopy
    {
        private static GameObject GoopyGameObject;
        private static readonly Sprite[] GoopyAnimation = new Sprite[193];
        private static SpriteRenderer BoxRenderer;
        private static Console TempHot;

        public static Sprite GetGoopyAnimation(int index)
        {
            try
            {
                if (GoopyAnimation == null || GoopyAnimation.Length == 0) return null;
                index = Mathf.Clamp(index, 0, GoopyAnimation.Length - 1);
                if (GoopyAnimation[index] == null)
                    GoopyAnimation[index] = Helpers.LoadSpriteFromResources($"StellarRoles.Resources.Goopy.Goopy_0{index + 1:000}.png", 175f);
                return GoopyAnimation[index];
            }
            catch (Exception ex)
            {
                Helpers.Log(LogLevel.Error, "Goopy Animation Error: " + ex.StackTrace);
                return null;
            }
        }

        [HideFromIl2Cpp]
        public static void CreateGoopy()
        {
            try
            {
                if (GoopyGameObject == null)
                    _ = new Goopy();
                StartAnimation();
            }
            catch (Exception ex)
            {
                Helpers.Log(LogLevel.Error, "Error Creating Goopy: " + ex.StackTrace);
            }
        }

        [HideFromIl2Cpp]
        public static void StartAnimation()
        {
            if (GoopyGameObject == null || MeetingHud.Instance) return;

            int time = RandomSeed._random.Next(20, 50);

            ((float)time).DelayedAction(Animation);
        }

        [HideFromIl2Cpp]
        public static void Animation()
        {
            if (GoopyGameObject == null || MeetingHud.Instance)
                return;

            HudManager.Instance.StartCoroutine(Effects.Lerp(7, new Action<float>((p) =>
            {
                if (BoxRenderer != null)
                    BoxRenderer.sprite = GetGoopyAnimation((int)(p * GoopyAnimation.Length));

                if (p == 1f)
                {
                    BoxRenderer.sprite = GetGoopyAnimation(0);

                    if (!MeetingHud.Instance)
                        StartAnimation();
                }
            })));
        }

        private Goopy()
        {
            GoopyGameObject = new GameObject("Goopy") { layer = 2 };
            Vector3 position = new(50f, 50f);
            TempHot = UnityEngine.Object.FindObjectsOfType<Console>().ToList().Find(console => console.name == "panel_temphot");
            if (TempHot != null)
            {
                GoopyGameObject.transform.SetParent(TempHot.transform);
                position = TempHot.gameObject.transform.position;
            }
            position.x += 2.4f;
            position.y -= 1f;
            GoopyGameObject.transform.position = position;
            BoxRenderer = GoopyGameObject.AddComponent<SpriteRenderer>();
            GoopyGameObject.SetActive(true);
        }

        private static void PreloadSprites()
        {
            for (int i = 0; i < GoopyAnimation.Length; i++)
                GetGoopyAnimation(i);
        }

        public static void ClearGoopy()
        {
            PreloadSprites();
            GoopyGameObject = null;
        }
    }
}