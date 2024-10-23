using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

namespace StellarRoles.Objects
{
    public class FootprintHolder : MonoBehaviour
    {
        static FootprintHolder() => ClassInjector.RegisterTypeInIl2Cpp<FootprintHolder>();

        public FootprintHolder(IntPtr ptr) : base(ptr) { }

        private static FootprintHolder _instance;
        public static FootprintHolder Instance
        {
            get => _instance ? _instance : _instance = new GameObject("FootprintHolder").AddComponent<FootprintHolder>();
            set => _instance = value;

        }

        private static Sprite _footprintSprite;
        private static Sprite FootprintSprite => _footprintSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.Footprint.png", 600f);

        private static bool AnonymousFootprints => Investigator.AnonymousFootprints;
        private static float FootprintDuration => Investigator.FootprintDuration;

        private class Footprint
        {
            public GameObject GameObject;
            public Transform Transform;
            public SpriteRenderer Renderer;
            public PlayerControl Owner;
            public NetworkedPlayerInfo Data;
            public float Lifetime;

            public Footprint()
            {
                GameObject = new("Footprint") { layer = 8 };
                Transform = GameObject.transform;
                Renderer = GameObject.AddComponent<SpriteRenderer>();
                Renderer.sprite = FootprintSprite;
                Renderer.color = Color.clear;
                GameObject.AddSubmergedComponent(SubmergedCompatibility.Classes.ElevatorMover);
            }
        }



        private readonly ConcurrentBag<Footprint> _pool = new();
        private readonly List<Footprint> _activeFootprints = new();
        private readonly List<Footprint> _toRemove = new();

        [HideFromIl2Cpp]
        public void MakeFootprint(PlayerControl player)
        {
            if (!_pool.TryTake(out Footprint print))
            {
                print = new();
            }

            print.Lifetime = FootprintDuration;

            Vector3 pos = player.transform.position;
            pos.z = pos.y / 1000f + 0.001f;
            print.Transform.SetPositionAndRotation(pos, Quaternion.EulerRotation(0, 0, UnityEngine.Random.Range(0.0f, 360.0f)));
            print.GameObject.SetActive(true);
            print.Owner = player;
            print.Data = player.Data;
            _activeFootprints.Add(print);
        }

        private void Update()
        {
            if (PlayerControl.LocalPlayer != Investigator.Player)
            {
                if (_activeFootprints.Count > 0)
                {
                    foreach (Footprint footprint in _activeFootprints)
                    {
                        footprint.GameObject.SetActive(false);
                        _activeFootprints.Remove(footprint);
                        _pool.Add(footprint);
                    }
                }
                return;
            }

            float dt = Time.deltaTime;
            _toRemove.Clear();
            foreach (Footprint activeFootprint in _activeFootprints)
            {
                float p = activeFootprint.Lifetime / FootprintDuration;

                if (activeFootprint.Lifetime <= 0)
                {
                    _toRemove.Add(activeFootprint);
                    continue;
                }

                Color color;
                if (AnonymousFootprints || Camouflager.CamouflageTimer > 0 || activeFootprint.Owner.IsMushroomMixupActive())
                    color = Palette.PlayerColors[6];
                else if (activeFootprint.Owner.IsMorphed() && Morphling.MorphTarget.Data != null)
                    color = Palette.PlayerColors[Morphling.MorphTarget.Data.DefaultOutfit.ColorId];
                else if (activeFootprint.Owner.IsInfested() && Parasite.Player.Data != null)
                    color = Palette.PlayerColors[Parasite.Player.Data.DefaultOutfit.ColorId];
                else
                    color = Palette.PlayerColors[activeFootprint.Data.DefaultOutfit.ColorId];

                color.a = Math.Clamp(p, 0f, 1f);
                activeFootprint.Renderer.color = color;

                activeFootprint.Lifetime -= dt;
            }

            foreach (Footprint footprint in _toRemove)
            {
                footprint.GameObject.SetActive(false);
                _activeFootprints.Remove(footprint);
                _pool.Add(footprint);
            }
        }

        private void OnDestroy()
        {
            Instance = null;
        }
    }
}
