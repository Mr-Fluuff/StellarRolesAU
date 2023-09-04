using StellarRoles.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace StellarRoles.Objects
{

    public class Sensor
    {
        public static readonly Dictionary<byte, Sensor> Sensors = new();

        private readonly GameObject SensorGameObject;
        private readonly SpriteRenderer HoleRender;

        private static Sprite _SensorSprite;

        private Arrow Arrow;
        private bool Tripped;
        private bool Activated;
        private PlayerControl PlayerThatTripped;
        private float Duration;
        public byte Id { get; set; }

        public static Sprite getSensor()
        {
            return _SensorSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.Watcher.WatcherSensor.png", 150f);
        }

        public Sensor(Vector2 p)
        {
            SensorGameObject = new GameObject("Sensor") { layer = 11 };
            SensorGameObject.AddSubmergedComponent(SubmergedCompatibility.Classes.ElevatorMover);
            Vector3 position = new(p.x, p.y, p.y / 1000 + 0.001f); // just behind player

            // Create the marker
            SensorGameObject.transform.position = position;
            HoleRender = SensorGameObject.AddComponent<SpriteRenderer>();
            HoleRender.sprite = getSensor();
            HoleRender.color = new Color(1f, 1f, 1f, 1f);
            // Only render for the Watcher
            bool playerIsWatcher = PlayerControl.LocalPlayer == Watcher.Player;
            SensorGameObject.SetActive(playerIsWatcher);
            Tripped = false;
            Activated = false;
            PlayerThatTripped = null;
            Duration = 0;
            Id = (byte)SensorGameObject.GetInstanceID();
        }

        private void ActivateSensor()
        {
            SensorGameObject.active = false;
            if (Watcher.AnonymousArrows || Camouflager.CamouflageTimer > 0)
            {
                Arrow = new(Watcher.Color);
            }
            else
            {
                bool isMorphling = Morphling.Player != null && PlayerThatTripped == Morphling.Player;
                if (isMorphling && Morphling.MorphTimer > 0 && Morphling.MorphTarget && Morphling.MorphTarget.Data != null)
                {
                    PlayerControl target = Morphling.MorphTarget;
                    Arrow = new(Palette.PlayerColors[target.Data.DefaultOutfit.ColorId]);

                }
                else
                {
                    Arrow = new(Palette.PlayerColors[PlayerThatTripped.Data.DefaultOutfit.ColorId]);
                }
            }
            Arrow.Update(SensorGameObject.transform.position);
            Arrow.Object.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && PlayerControl.LocalPlayer == Watcher.Player);
            Duration = 5f;
            Activated = true;
        }

        public static void UpdateActivateSensorPerPlayer()
        {
            if (Sensors.Count <= 0) return;
            if (WatcherAbilities.isRoleBlocked()) return;

            //Activate new tripwires
            foreach (Sensor sensor in Sensors.Values)
            {
                if (sensor.Tripped) continue;

                foreach (PlayerControl player in PlayerControl.AllPlayerControls.GetFastEnumerator())
                {
                    if (player == Watcher.Player || player.Data.IsDead) continue;
                    if (player == Wraith.Player && Wraith.IsInvisible) continue;
                    if (player == Shade.Player && Shade.IsInvisble) continue;
                    if (player.inVent) continue;


                    if (Vector2.Distance(sensor.SensorGameObject.transform.position, player.GetTruePosition()) < 1.6 + .01f && !PhysicsHelpers.AnythingBetween(sensor.SensorGameObject.transform.position, player.GetTruePosition(), Constants.ShadowMask, false))
                    {
                        RPCProcedure.Send(CustomRPC.TripSensor, player.PlayerId);
                        RPCProcedure.TripSensor(player);

                        sensor.Tripped = true;
                        sensor.PlayerThatTripped = player;
                        sensor.Activated = false;
                    }
                }
            }
        }

        public static void UpdateForWatcher()
        {
            //Activate Trip Wires that were recently tripped
            foreach (Sensor sensor in Sensors.Values)
                if (sensor.Tripped && !sensor.Activated && sensor.PlayerThatTripped != null)
                    sensor.ActivateSensor();

            //Decrease time on already activated trip wires
            float dt = Time.deltaTime;
            List<Sensor> toBeRemoved = new();
            foreach (Sensor t in Sensors.Values)
                if (t.Activated && (t.Duration -= dt) <= 0)
                    toBeRemoved.Add(t);

            //Remove trip wires arrows that have run out of time
            foreach (Sensor t in toBeRemoved)
            {
                if (t.Arrow != null)
                {
                    t.Arrow.Object.SetActive(false);
                    Object.Destroy(t.Arrow.Object);
                    Object.Destroy(t.SensorGameObject);
                }
                Sensors.Remove(t.Id);
            }

        }

        public static void ClearTripWires()
        {
            foreach (Sensor t in Sensors.Values)
            {
                Object.Destroy(t.SensorGameObject);
                if (t.Arrow != null)
                {
                    t.Arrow.Object.active = false;
                    Object.Destroy(t.Arrow.Object);
                }

            }
            Sensors.Clear();
        }
    }
}