using HarmonyLib;
using StellarRoles.Objects;
using System;
using UnityEngine;

namespace StellarRoles
{
    public static class LobbyBehaviour_Patch
    {
        private static GameObject _Prefab = null;
        private static GameObject _Engine = null;

        private static GameObject GetOldLobby(string name = "Lobby(Clone)")
        {

            GameObject lobby = GameObject.Find(name);
            if (lobby == null)
            {
                Helpers.Log(LogLevel.Fatal, "Lobby GameObject Not Found");
                Application.Quit(1);
            }
            return lobby;
        }

        private static void LoadPrefab()
        {
            _Prefab = StellarRolesPlugin.CustomLobbyPrefab;
            if (_Prefab == null)
            {
                Helpers.Log(LogLevel.Fatal, "Lobby Prefab Not Found");
                Application.Quit(1);
            }
        }
        private static GameObject StellarBanner;

        [HarmonyPatch(typeof(LobbyBehaviour), nameof(LobbyBehaviour.Start))]

        public static class LobbyBehavour_Start_Patch
        {
            public static void Postfix()
            {
                if (_Prefab == null)
                {
                    LoadPrefab();
                }

                GameObject instance = UnityEngine.Object.Instantiate(_Prefab);
                instance.transform.position = new Vector3(0f, 0.85f, 0f);


                GameObject oldLobby = GetOldLobby();
                oldLobby.GetComponent<Collider2D>().enabled = false;
                GameObject.Find("Lobby(Clone)/Background").SetActive(false);
                StellarBanner = new GameObject("StellarBanner");
                Vector3 position = new(0f, 4.5f, 4.5f / 1000 + 0.001f);
                StellarBanner.transform.position = position;
                StellarBanner.transform.localScale = new Vector3(1, 1, 1);

                SpriteRenderer panelRenderer = StellarBanner.AddComponent<SpriteRenderer>();
                if (DateTime.Today.Month == 12 && DateTime.Today.Day > 15 && DateTime.Today.Day < 31)
                    panelRenderer.sprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.Banners.Christmas.png", 100f);
                else if (DateTime.Today.Month == 2 && DateTime.Today.Day > 11 && DateTime.Today.Day < 16)
                {
                    panelRenderer.sprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.Banners.ValentinesBanner.png", 200f);

                    SpriteRenderer leftsprite = GameObject.Find("Lobby(Clone)/Leftbox").GetComponent<SpriteRenderer>();
                    SpriteRenderer rightsprite = GameObject.Find("Lobby(Clone)/RightBox").GetComponent<SpriteRenderer>();

                    leftsprite.sprite = rightsprite.sprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.CustomLobby.LeftBoxHeart.png", 200f);
                }
                else
                    panelRenderer.sprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.Banners.StellarBanner.png", 200f);

                StellarBanner.gameObject.transform.SetParent(instance.transform);

                GameObject leftEngine = GameObject.Find("Lobby(Clone)/LeftEngine");

                GameObject.Find("Lobby(Clone)/RightEngine").transform.position = new Vector3(12.45f, -0.3f, 0.5f);
                leftEngine.transform.position = new Vector3(-4.775f, -3f, 0.5f);
                GameObject.Find("Lobby(Clone)/ShipRoom").SetActive(false);
                _Engine = UnityEngine.Object.Instantiate(leftEngine, new Vector3(-4.775f, 1.75f, 0.5f), Quaternion.identity);

                Mochi.ClearMochi();
                Mochi.CreateMochi();
                GameObject mochi = Mochi.MochiGameObject;
                mochi.SetActive(true);
                mochi.transform.position = new Vector3(9.57f, -.11f, -1f);
                mochi.transform.localScale = new Vector3(1, 1, 1) * 1.8f;
                mochi.gameObject.transform.SetParent(instance.transform);
            }
        }

        [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Start))]
        public static class GameStartPatch
        {
            public static void Prefix()
            {
                //Reset states for custom lobby
                try
                {
                    if (!DestroyableSingleton<TutorialManager>.InstanceExists)
                    {
                        GameObject.Find("allul_customlobby(Clone)").SetActive(false);

                        _Engine.SetActive(false);
                    }
                }
                catch
                {

                }
            }
        }
    }
}
