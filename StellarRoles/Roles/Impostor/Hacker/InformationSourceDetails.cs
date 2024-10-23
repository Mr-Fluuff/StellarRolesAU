using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;


namespace StellarRoles
{
    [HarmonyPatch]
    public class InformationSourceDetails
    {
        public static readonly Vector3 PolusVitals = new(26.7f, -16.7f, 1f);
        public static readonly Vector3 PolusAdminTable = new(22.8f, -21.37f, 1f);
        public static readonly Vector3 PolusAdminTable2 = new(25f, -21.8f, 1f);
        public static readonly Vector3 PolusCameras = new(2.8f, -12.2f, 1f);
        public static readonly Vector3 SkeldAdmin = new(3.2f, -8.7f, 1f);
        public static readonly Vector3 SkeldCameras = new(-12.9f, -3.2f, 1f);
        public static readonly Vector3 MiraAdmin = new(19.6f, 18.8f, 1f);
        public static readonly Vector3 MiraAdmin2 = new(22.2f, 18.9f, 1f);
        public static readonly Vector3 MiraAdmin3 = new(21f, 19.9f, 1f);
        public static readonly Vector3 MiraDoorLogs = new(15.9f, 4.8f, 1f);
        public static readonly Vector3 AirshipAdmin1 = new(-22.1f, .6f, 1f);
        public static readonly Vector3 ArishipAdmin2 = new(20.3f, 12.1f, 1f);
        public static readonly Vector3 AirshipVitals = new(25.5f, -8.5f, 1f);
        public static readonly Vector3 AirshipCameras = new(8.5f, -10.5f, 1f);
        public static readonly Vector3 AirshipCameras2 = new(7.6f, -10.5f, 1f);
        public static readonly Vector3 FungleVitals = new(-2.79f, -9.95f, 1f);
        public static readonly Vector3 FungleCams = new(7.06f, 1.16f, 1f);
        public static readonly Vector3 FungleAdmin = new(9.59f, -13.43f, 1f);

        public static readonly Vector3 BetterPolusVitals = new(31.3f, -7.3f, 1f);


        public InformationSource InformationSource;
        public Map MapType;
        public Vector3 Location;
        public float Range = 0.75f;
        public InformationSourceDetails(InformationSource informationSource, Map map, Vector3 location)
        {
            InformationSource = informationSource;
            MapType = map;
            Location = location;
        }

        public InformationSourceDetails(InformationSource informationSource, Map map, Vector3 location, float range)
        {
            InformationSource = informationSource;
            MapType = map;
            Location = location;
            Range = range;
        }


        public static List<InformationSourceDetails> BuildAllInformationSources(bool betterPolusVitals)
        {
            Vector3 vitalsLocation = betterPolusVitals ? BetterPolusVitals : PolusVitals;

            return new()
            {
                new InformationSourceDetails(InformationSource.Vitals, Map.Polus, vitalsLocation, .55f),
                new InformationSourceDetails(InformationSource.Admin, Map.Polus, PolusAdminTable, 1f),
                new InformationSourceDetails(InformationSource.Admin, Map.Polus, PolusAdminTable2, 1f),
                new InformationSourceDetails(InformationSource.Cameras, Map.Polus, PolusCameras),
                new InformationSourceDetails(InformationSource.Admin, Map.Skeld, SkeldAdmin, 1.15f),
                new InformationSourceDetails(InformationSource.Cameras, Map.Skeld, SkeldCameras, .35f),
                new InformationSourceDetails(InformationSource.Admin, Map.Mira, MiraAdmin, 1f),
                new InformationSourceDetails(InformationSource.Admin, Map.Mira, MiraAdmin2, 1f),
                new InformationSourceDetails(InformationSource.Admin, Map.Mira, MiraAdmin3, .75f),
                new InformationSourceDetails(InformationSource.Cameras, Map.Mira, MiraDoorLogs),
                new InformationSourceDetails(InformationSource.Admin, Map.Airship, AirshipAdmin1, 1f),
                new InformationSourceDetails(InformationSource.Admin, Map.Airship, ArishipAdmin2, 1f),
                new InformationSourceDetails(InformationSource.Cameras, Map.Airship, AirshipCameras, .75f),
                new InformationSourceDetails(InformationSource.Cameras, Map.Airship, AirshipCameras2, .75f),
                new InformationSourceDetails(InformationSource.Vitals, Map.Airship, AirshipVitals),
                new InformationSourceDetails(InformationSource.Vitals, Map.Fungal, FungleVitals),
                new InformationSourceDetails(InformationSource.Admin, Map.Fungal, FungleAdmin),
                new InformationSourceDetails(InformationSource.Cameras, Map.Fungal, FungleCams)
            };
        }
    }

}
