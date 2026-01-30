using HarmonyLib;
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

        [HarmonyPatch(typeof(LobbyBehaviour), nameof(LobbyBehaviour.Start))]

        public static class LobbyBehavour_Start_Patch
        {
            public static void Postfix()
            {
                if (_Prefab == null)
                {
                    _Prefab = UnityEngine.Object.Instantiate(CustomAssets.CustomLobbyPrefab.LoadAsset());
                }
                _Prefab.SetActive(true);
                _Prefab.transform.position = new Vector3(4.44f, 0.85f, 0f);

                GameObject oldLobby = GetOldLobby();
                oldLobby.GetComponent<Collider2D>().enabled = false;
                GameObject.Find("Lobby(Clone)/Background").SetActive(false);

                var OriginalBG = _Prefab.transform.FindChild("OriginalBG").gameObject;
                OriginalBG.SetActive(false);


                if ((DateTime.Today.Month == 10 && DateTime.Today.Day >= 17) || (DateTime.Today.Month == 11 && DateTime.Today.Day <= 2))
                {
                    _Prefab.transform.FindChild("HalloweenBG").gameObject.SetActive(true);
                }
                else if (DateTime.Today.Month == 11 && DateTime.Today.Day >= 20 && DateTime.Today.Day <= 30)
                {
                    _Prefab.transform.FindChild("ThanksGivingBG").gameObject.SetActive(true);
                }
                else if (DateTime.Today.Month == 12 && DateTime.Today.Day >= 4 && DateTime.Today.Day <= 30)
                {
                    _Prefab.transform.FindChild("ChristmasBG").gameObject.SetActive(true);
                }
                else
                {
                    OriginalBG.SetActive(true);
                    if (DateTime.Today.Month == 2 && DateTime.Today.Day > 11 && DateTime.Today.Day < 16)
                    {
                        OriginalBG.transform.FindChild("Valentines_Banner").gameObject.SetActive(true);

                        SpriteRenderer leftsprite = GameObject.Find("Lobby(Clone)/Leftbox").GetComponent<SpriteRenderer>();
                        SpriteRenderer rightsprite = GameObject.Find("Lobby(Clone)/RightBox").GetComponent<SpriteRenderer>();

                        leftsprite.sprite = rightsprite.sprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.CustomLobby.LeftBoxHeart.png", 200f);
                    }
                    else
                    {
                        OriginalBG.transform.FindChild("Stellar_Banner").gameObject.SetActive(true);
                    }
                }

                GameObject leftEngine = GameObject.Find("Lobby(Clone)/LeftEngine");

                GameObject.Find("Lobby(Clone)/RightEngine").transform.position = new Vector3(12.45f, -0.3f, 0.5f);
                leftEngine.transform.position = new Vector3(-4.775f, -3f, 0.5f);
                GameObject.Find("Lobby(Clone)/ShipRoom").SetActive(false);
                _Engine = UnityEngine.Object.Instantiate(leftEngine, new Vector3(-4.775f, 1.75f, 0.5f), Quaternion.identity);
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
                    if (!TutorialManager.InstanceExists)
                    {
                        _Prefab.SetActive(false);
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
