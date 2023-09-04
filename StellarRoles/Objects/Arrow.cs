using UnityEngine;

namespace StellarRoles.Objects
{
    public class Arrow
    {
        public readonly SpriteRenderer Renderer;
        public readonly GameObject Object;
        private Vector3 OldTarget;
        private readonly ArrowBehaviour ArrowBehaviour;

        private static Sprite _Sprite;
        public static Sprite GetSprite()
        {
            return _Sprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.Arrow.png", 200f);
        }

        public Arrow(Color color)
        {
            Object = new GameObject("Arrow")
            {
                layer = 5
            };
            Renderer = Object.AddComponent<SpriteRenderer>();
            Renderer.sprite = GetSprite();
            Renderer.color = color;
            ArrowBehaviour = Object.AddComponent<ArrowBehaviour>();
            ArrowBehaviour.image = Renderer;
        }

        public void Update()
        {
            Vector3 target = OldTarget;
            Update(target);
        }

        public void Update(Vector3 target, Color? color = null)
        {
            OldTarget = target;

            if (color.HasValue) Renderer.color = color.Value;

            ArrowBehaviour.target = target;
            ArrowBehaviour.Update();
        }
    }
}