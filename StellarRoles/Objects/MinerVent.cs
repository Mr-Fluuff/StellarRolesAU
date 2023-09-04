using StellarRoles.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StellarRoles.Objects
{

    public class MinerVent
    {
        public static readonly List<MinerVent> AllMinerVents = new();

        public GameObject GameObject;
        public Vent vent;
        public SpriteRenderer holeRender;
        private static Sprite _PolusMinerHole;
        private static Sprite _OtherMinerHole;

        public static Sprite GetMinerConstruction()
        {
            return Helpers.IsMap(Map.Polus) ? _PolusMinerHole ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.Vent.png", 200f)
                : _OtherMinerHole ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.Vent.png", 225f);
        }


        public MinerVent(Vector2 p)
        {
            GameObject = new GameObject("MinerVentLocation") { layer = 11 };
            Vector3 position = new(p.x, p.y, p.y / 1000 + 0.0008f); // just behind player
            Vector2 offset = PlayerControl.LocalPlayer.Collider.offset * .7f;
            position += new Vector3(offset.x, offset.y, 0); // Add collider offset that DoMove moves the player up at a valid position

            // Create the marker
            GameObject.transform.position = position;
            holeRender = GameObject.AddComponent<SpriteRenderer>();
            holeRender.sprite = GetMinerConstruction();
            holeRender.color = new Color(1f, 1f, 1f, 0.5f);

            // Create the vent
            Vent referenceVent = UnityEngine.Object.FindObjectOfType<Vent>();
            vent = UnityEngine.Object.Instantiate(referenceVent, referenceVent.transform.parent);
            vent.transform.position = GameObject.transform.position;
            vent.Left = null;
            vent.Right = null;
            vent.Center = null;
            vent.EnterVentAnim = null;
            vent.ExitVentAnim = null;
            vent.Offset = new Vector3(0f, 0.1f, 0f);
            vent.GetComponent<PowerTools.SpriteAnim>()?.Stop();
            vent.Id = MapUtilities.CachedShipStatus.AllVents.Select(x => x.Id).Max() + 1; // Make sure we have a unique id
            SpriteRenderer render = vent.GetComponent<SpriteRenderer>();
            render.sprite = GetMinerConstruction();
            vent.myRend = render;
            vent.name = "MinerVent_" + vent.Id;
            vent.gameObject.SetActive(false);
            if (SubmergedCompatibility.IsSubmerged)
            {
                vent.gameObject.AddSubmergedComponent(SubmergedCompatibility.Classes.ElevatorMover); // just in case elevator vent is not blocked
            }
            List<Vent> allVentsList = MapUtilities.CachedShipStatus.AllVents.ToList();
            allVentsList.Add(vent);
            MapUtilities.CachedShipStatus.AllVents = allVentsList.ToArray();


            // Only render the vent for the Miner
            GameObject.SetActive(PlayerControl.LocalPlayer == Miner.Player);

            AllMinerVents.Add(this);
            // Miner Vents

            if (Miner.VisibleInstantly)
                ConvertToVents();
            else if (Miner.VisibleDelay)
                FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(Miner.Delay, new Action<float>((p) =>
                {
                    if (p == 1f)
                    {
                        ConvertToVents();
                    }
                })));

        }

        public static void UpdateStates()
        {
            foreach (MinerVent vent in AllMinerVents)
                vent.GameObject.SetActive(PlayerControl.LocalPlayer == Miner.Player);
        }

        public void ConvertToVent()
        {
            if (GameObject.active) GameObject.SetActive(false);
            if (!vent.gameObject.active)
            {
                vent.gameObject.SetActive(true);
                vent.myRend.sprite = GetMinerConstruction();
            }
            //vent.myRend.color = new Color(1f, 1f, 1f, 1f);

            return;
        }

        public static void ConvertToVents()
        {
            foreach (MinerVent vent in AllMinerVents)
                vent.ConvertToVent();
            ConnectVents();
            return;
        }

        private static void ConnectVents()
        {
            if (AllMinerVents.Count <= 1) return;

            for (int i = 0; i < AllMinerVents.Count - 1; i++)
            {
                MinerVent a = AllMinerVents[i];
                MinerVent b = AllMinerVents[i + 1];
                a.vent.Right = b.vent;
                b.vent.Left = a.vent;
            }
            // Connect first with last
            AllMinerVents.First().vent.Left = AllMinerVents.Last().vent;
            AllMinerVents.Last().vent.Right = AllMinerVents.First().vent;
        }

        public static void ClearMinerVents()
        {
            foreach(MinerVent vent in AllMinerVents)
            {
                vent.GameObject.SetActive(false);
                UnityEngine.Object.Destroy(vent.GameObject);
                UnityEngine.Object.Destroy(vent.vent.gameObject);
            }
            AllMinerVents.Clear();
        }
    }
}