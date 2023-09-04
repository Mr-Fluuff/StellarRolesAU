using System.Collections.Generic;
using UnityEngine;

namespace StellarRoles.Objects
{

    public class Lantern
    {
        public static Lantern CurrentLantern { get; set; } = null;
        public static readonly List<Lantern> BrokenLanterns = new();

        public readonly GameObject LanternGameObject;

        private SpriteRenderer _HoleRender;
        private static Sprite _LanternSprite;
        private static Sprite _BrokenLanternSprite;


        public static Sprite getLantern()
        {
            return _LanternSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.Lantern.png", 115f);
        }

        public static Sprite GetBrokenLantern()
        {
            return _BrokenLanternSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.BrokenLantern.png", 115f);
        }


        public Lantern(Vector2 p)
        {
            LanternGameObject = new GameObject("Lantern") { layer = 11 };
            LanternGameObject.AddSubmergedComponent(SubmergedCompatibility.Classes.ElevatorMover);
            Vector3 position = new(p.x, p.y, p.y / 1000 + 0.001f); // just behind player

            // Create the marker
            LanternGameObject.transform.position = position;
            _HoleRender = LanternGameObject.AddComponent<SpriteRenderer>();
            _HoleRender.sprite = getLantern();
            _HoleRender.color = new Color(1f, 1f, 1f, 1f);

            // Only render for the Wraith
            LanternGameObject.SetActive(PlayerControl.LocalPlayer == Wraith.Player);

            CurrentLantern = this;
        }

        public static void UpdateStates()
        {
            foreach (Lantern lantern in BrokenLanterns)
                lantern.LanternGameObject.SetActive(PlayerControl.LocalPlayer == Wraith.Player);
        }

        public static void BreakLantern()
        {
            if (CurrentLantern != null)
            {
                BrokenLanterns.Add(CurrentLantern);
                foreach (Lantern lantern in BrokenLanterns)
                    lantern.ShowLantern();
                CurrentLantern = null;
            }
        }

        public void ShowLantern()
        {
            LanternGameObject.SetActive(true);
            _HoleRender.sprite = GetBrokenLantern();
        }

        public static void ClearLanterns()
        {
            foreach (Lantern lantern in BrokenLanterns)
            {
                lantern.LanternGameObject.SetActive(false);
                Object.Destroy(lantern.LanternGameObject);
            }
            BrokenLanterns.Clear();
            CurrentLantern = null;
        }
    }
}