using BepInEx.Unity.IL2CPP.Utils;
using HarmonyLib;
using System.Collections;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace StellarRoles
{
    public class AddAssets
    {
        public static ShipStatus Polus;

        public static void AddObjects()
        {
            AddVitals();
            AddAdmin();
        }

        public static void AddVitals()
        {
            if (Helpers.IsMap(Map.Mira))
            {
                Transform Vital = Object.Instantiate(PolusObject.transform.FindChild("Office").FindChild("panel_vitals"), GameObject.Find("MiraShip(Clone)").transform);
                Vital.transform.position = new Vector3(69f, 69f, 0f);
            }
            if (Helpers.IsMap(Map.Skeld))
            {
                Transform Vital = Object.Instantiate(PolusObject.transform.FindChild("Office").FindChild("panel_vitals"), GameObject.Find("SkeldShip(Clone)").transform);
                Vital.transform.position = new Vector3(69f, 69f, 0.0142f);
            }
        }

        public static void AddAdmin()
        {
            if (Helpers.IsMap(Map.Fungal) && MapOptions.AddFungleAdmin)
            {
                Transform Admin = Object.Instantiate(PolusObject.transform.FindChild("Admin").FindChild("mapTable").FindChild("panel_map"), GameObject.Find("FungleShip(Clone)").transform);
                Admin.position = new Vector3(9.5972f, -13.4905f, -0.0131f);
                Admin.rotation = Quaternion.Euler(0, 0, 120.1311f);
                Admin.gameObject.GetComponent<MapConsole>().usableDistance = 0.5f;
            }
        }

        public static GameObject PolusObject => Polus.gameObject;

        public static IEnumerator LoadPolus()
        {
            while (AmongUsClient.Instance == null)
            {
                yield return null;
            }
            AsyncOperationHandle<GameObject> polusAsset = AmongUsClient.Instance.ShipPrefabs.ToArray()[2].LoadAsset<GameObject>();
            while (!polusAsset.IsDone)
            {
                yield return null;
            }
            Polus = polusAsset.Result.GetComponent<ShipStatus>();
            yield break;
        }
    }

    [HarmonyPatch(typeof(AmongUsClient), "Awake")]
    public static class AmongUsClient_Awake_Patch
    {
        private static bool Loaded;

        public static void Prefix(AmongUsClient __instance)
        {
            bool loaded = Loaded;
            if (!loaded)
            {
                Loaded = true;
                __instance.StartCoroutine(AddAssets.LoadPolus());
            }
        }
    }
}
