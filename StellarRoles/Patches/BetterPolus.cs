using HarmonyLib;
using System.Linq;
using UnityEngine;

namespace StellarRoles
{
    [HarmonyPatch(typeof(ShipStatus))]
    public static class ShipStatusPatch
    {
        public static readonly Vector3 DvdScreenNewPos = new(26.635f, -15.92f, 1f);
        public static readonly Vector3 VitalsNewPos = new(31.275f, -6.45f, 1f);

        public static readonly Vector3 WifiNewPos = new(15.975f, 0.084f, 1f);
        public static readonly Vector3 NavNewPos = new(11.07f, -15.298f, -0.015f);

        public static readonly Vector3 TempColdNewPos = new(25.4f, -6.4f, 1f);
        public static readonly Vector3 TempColdNewPosDV = new(7.772f, -17.103f, -0.017f);

        public const float DvdScreenNewScale = 0.75f;

        public static bool IsAdjustmentsDone;
        public static bool IsObjectsFetched;
        public static bool IsRoomsFetched;
        public static bool IsVentsFetched;

        public static Console WifiConsole;
        public static Console NavConsole;

        public static SystemConsole Vitals;
        public static GameObject DvdScreenOffice;

        public static Vent ElectricBuildingVent;
        public static Vent ElectricalVent;
        public static Vent ScienceBuildingVent;
        public static Vent StorageVent;
        public static Vent LightCageVent;

        public static Console TempCold;

        public static GameObject Comms;
        public static GameObject DropShip;
        public static GameObject Outside;
        public static GameObject Science;

        [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Begin))]
        public static class ShipStatusBeginPatch
        {
            [HarmonyPostfix]
            [HarmonyPatch]
            public static void Postfix(ShipStatus __instance)
            {
                ApplyChanges(__instance);
            }
        }

        [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Awake))]
        public static class ShipStatusAwakePatch
        {
            [HarmonyPostfix]
            [HarmonyPatch]
            public static void Postfix(ShipStatus __instance)
            {
                ApplyChanges(__instance);
            }
        }

        private static void ApplyChanges(ShipStatus instance)
        {
            if (instance.Type == ShipStatus.MapType.Pb)
            {
                FindPolusObjects();
                AdjustPolus();
            }
        }

        public static void FindPolusObjects()
        {
            FindVents();
            FindRooms();
            FindObjects();
        }

        public static void AdjustPolus()
        {
            if (IsObjectsFetched && IsRoomsFetched)
            {
                if (CustomOptionHolder.MoveVitals.GetBool()) MoveVitals();
                if (!CustomOptionHolder.ColdTempDeathValley.GetBool() && CustomOptionHolder.MoveVitals.GetBool()) MoveTempCold();
                if (CustomOptionHolder.ColdTempDeathValley.GetBool()) MoveTempColdDV();
                if (CustomOptionHolder.WifiChartCourseSwap.GetBool()) SwitchNavWifi();
            }

            if (CustomOptionHolder.VentSystem.GetBool()) AdjustVents();
            AddPengu();
            IsAdjustmentsDone = true;
        }

        public static void FindVents()
        {
            var ventsList = Object.FindObjectsOfType<Vent>().ToList();

            if (ElectricBuildingVent == null)
            {
                ElectricBuildingVent = ventsList.Find(vent => vent.gameObject.name == "ElectricBuildingVent");
            }

            if (ElectricalVent == null)
            {
                ElectricalVent = ventsList.Find(vent => vent.gameObject.name == "ElectricalVent");
            }

            if (ScienceBuildingVent == null)
            {
                ScienceBuildingVent = ventsList.Find(vent => vent.gameObject.name == "ScienceBuildingVent");
            }

            if (StorageVent == null)
            {
                StorageVent = ventsList.Find(vent => vent.gameObject.name == "StorageVent");
            }

            if (LightCageVent == null)
            {
                LightCageVent = ventsList.Find(vent => vent.gameObject.name == "ElecFenceVent");
            }

            IsVentsFetched = ElectricBuildingVent != null && ElectricalVent != null && ScienceBuildingVent != null &&
                              StorageVent != null && LightCageVent != null;
        }

        public static void FindRooms()
        {
            if (Comms == null)
            {
                Comms = Object.FindObjectsOfType<GameObject>().ToList().Find(o => o.name == "Comms");
            }

            if (DropShip == null)
            {
                DropShip = Object.FindObjectsOfType<GameObject>().ToList().Find(o => o.name == "Dropship");
            }

            if (Outside == null)
            {
                Outside = Object.FindObjectsOfType<GameObject>().ToList().Find(o => o.name == "Outside");
            }

            if (Science == null)
            {
                Science = Object.FindObjectsOfType<GameObject>().ToList().Find(o => o.name == "Science");
            }

            IsRoomsFetched = Comms != null && DropShip != null && Outside != null && Science != null;
        }

        public static void FindObjects()
        {
            if (WifiConsole == null)
            {
                WifiConsole = Object.FindObjectsOfType<Console>().ToList()
                    .Find(console => console.name == "panel_wifi");
            }

            if (NavConsole == null)
            {
                NavConsole = Object.FindObjectsOfType<Console>().ToList()
                    .Find(console => console.name == "panel_nav");
            }

            if (Vitals == null)
            {
                Vitals = Object.FindObjectsOfType<SystemConsole>().ToList()
                    .Find(console => console.name == "panel_vitals");
            }

            if (DvdScreenOffice == null)
            {
                GameObject DvdScreenAdmin = Object.FindObjectsOfType<GameObject>().ToList()
                    .Find(o => o.name == "dvdscreen");

                if (DvdScreenAdmin != null)
                {
                    DvdScreenOffice = Object.Instantiate(DvdScreenAdmin);
                }
            }

            if (TempCold == null)
            {
                TempCold = Object.FindObjectsOfType<Console>().ToList()
                    .Find(console => console.name == "panel_tempcold");
            }

            IsObjectsFetched = WifiConsole != null && NavConsole != null && Vitals != null &&
                               DvdScreenOffice != null && TempCold != null;
        }

        public static void AdjustVents()
        {
            if (IsVentsFetched)
            {
                ElectricBuildingVent.Left = ElectricalVent;
                ElectricalVent.Center = ElectricBuildingVent;
                ElectricBuildingVent.Center = LightCageVent;
                LightCageVent.Center = ElectricBuildingVent;

                ScienceBuildingVent.Left = StorageVent;
                StorageVent.Center = ScienceBuildingVent;
            }
        }

        public static void MoveTempCold()
        {
            if (TempCold.transform.position != TempColdNewPos)
            {
                Transform tempColdTransform = TempCold.transform;
                tempColdTransform.parent = Outside.transform;
                tempColdTransform.position = TempColdNewPos;

                BoxCollider2D collider = TempCold.GetComponent<BoxCollider2D>();
                collider.isTrigger = false;
                collider.size += new Vector2(0f, -0.3f);
            }
        }

        public static void MoveTempColdDV()
        {
            if (TempCold.transform.position != TempColdNewPosDV)
            {
                Transform tempColdTransform = TempCold.transform;
                tempColdTransform.parent = Outside.transform;
                tempColdTransform.position = TempColdNewPosDV;

                BoxCollider2D collider = TempCold.GetComponent<BoxCollider2D>();
                collider.isTrigger = false;
                collider.size += new Vector2(0f, -0.3f);
            }
        }

        public static void SwitchNavWifi()
        {
            if (WifiConsole.transform.position != WifiNewPos)
            {
                Transform wifiTransform = WifiConsole.transform;
                wifiTransform.parent = DropShip.transform;
                wifiTransform.position = WifiNewPos;
            }

            if (NavConsole.transform.position != NavNewPos)
            {
                Transform navTransform = NavConsole.transform;
                navTransform.parent = Comms.transform;
                navTransform.position = NavNewPos;

                NavConsole.checkWalls = true;
            }
        }

        public static void MoveVitals()
        {
            if (Vitals.transform.position != VitalsNewPos)
            {
                Transform vitalsTransform = Vitals.gameObject.transform;
                vitalsTransform.parent = Science.transform;
                vitalsTransform.position = VitalsNewPos;
            }

            if (DvdScreenOffice.transform.position != DvdScreenNewPos)
            {
                Transform dvdScreenTransform = DvdScreenOffice.transform;
                dvdScreenTransform.position = DvdScreenNewPos;

                Vector3 localScale = dvdScreenTransform.localScale;
                localScale = new Vector3(DvdScreenNewScale, localScale.y, localScale.z);
                dvdScreenTransform.localScale = localScale;
            }
        }

        static GameObject SnowMan;
        static Sprite PenguSprite;
        public static void AddPengu()
        {
            if (PenguSprite == null) PenguSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.PolusPengu.png", 450f);
            SnowMan = Object.FindObjectsOfType<GameObject>().ToList().Find(snowman => snowman.name == "snowman (5)");
            if (SnowMan != null)
            {
                var BoxRenderer = SnowMan.GetComponent<SpriteRenderer>();
                BoxRenderer.sprite = PenguSprite;
            }
        }
    }
}