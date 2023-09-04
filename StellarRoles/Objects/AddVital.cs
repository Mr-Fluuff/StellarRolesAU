using BepInEx.Unity.IL2CPP.Utils;
using HarmonyLib;
using System.Collections;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace StellarRoles
{
    public class AddVitals
    {
        public static ShipStatus Polus;

        public static void AddVital()
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
                __instance.StartCoroutine(AddVitals.LoadPolus());
            }
        }
    }
}
