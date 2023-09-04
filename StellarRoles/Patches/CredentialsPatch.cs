using HarmonyLib;
using System;
using TMPro;
using UnityEngine;

namespace StellarRoles.Patches
{
    [HarmonyPatch]
    public static class CredentialsPatch
    {

        public static string fullCredentials =
$@"<size=130%><color=#B2FEFE>StellarRoles</color></size> v{StellarRolesPlugin.Version} {StellarRolesPlugin.BetaVersion}
<size=80%>Created by <color=#C50000>Fluff</color>, <color=#C46AD8>Ilyssa</color>, and <color=#9bd3ff>Stell</color> ";

        public static string mainMenuCredentials =
    $@"Created by <color=#C50000>Fluff</color>, <color=#C46AD8>Ilyssa</color>, and <color=#9bd3ff>Stell</color>";

        public static string contributorsCredentials =
$@"<size=80%> <color=#B2FEFE>Special thanks to Om3ga & Sugden</color></size>";

        public static string artCredentials =
$@"<size=80f%>Art by Crayonvex, Stell, and Phylo</size>";


        [HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
        internal static class PingTrackerPatch
        {
            public static GameObject modStamp;

            static void Prefix(PingTracker __instance)
            {
                if (modStamp == null)
                {
                    modStamp = new GameObject("ModStamp");
                    SpriteRenderer rend = modStamp.AddComponent<SpriteRenderer>();
                    rend.sprite = ModManager.Instance.ModStamp.sprite;
                    rend.color = new Color(1, 1, 1, 0.5f);
                    modStamp.transform.parent = HudManager.Instance.transform;
                    modStamp.transform.localScale *= 0.5f;
                }
            }

            static void Postfix(PingTracker __instance)
            {
                __instance.text.alignment = TextAlignmentOptions.TopRight;
                if (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started)
                {
                    __instance.text.text = Camera.main.orthographicSize == 3f
                        ? $"<size=130%><color=#B2FEFE>StellarRoles</color></size> v{StellarRolesPlugin.Version} {StellarRolesPlugin.BetaVersion}\n{__instance.text.text}" : "";
                    __instance.transform.localPosition = HudManager.Instance.SettingsButton.transform.localPosition + (Vector3.left * 3f) + (Vector3.up * .25f);
                }
                else
                {
                    __instance.text.text = $"{fullCredentials}\n{__instance.text.text}";
                    __instance.transform.localPosition = new Vector3(3.5f, __instance.transform.localPosition.y, __instance.transform.localPosition.z);
                }
                if (HudManager.Instance)
                    modStamp.transform.localPosition = HudManager.Instance.MapButton.transform.localPosition + (Vector3.left * .5f) + (Vector3.up * .25f);
            }
        }

        [HarmonyPatch(typeof(ModManager), nameof(ModManager.LateUpdate))]
        internal static class ModStampUpdate
        {
            static void Postfix(ModManager __instance)
            {
                if (__instance == null) return;
                __instance.ModStamp.gameObject.SetActive(!Helpers.GameStarted && !LobbyBehaviour.Instance);
            }
        }


        [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
        public static class LogoPatch
        {
            public static SpriteRenderer renderer;
            public static Sprite bannerSprite;
            public static Sprite playerSprite;
            public static Sprite[] Sprites;
            private static PingTracker instance;
            static void Postfix(PingTracker __instance)
            {
                GameObject StellarLogo = new("bannerLogo_SR");
                StellarLogo.transform.SetParent(GameObject.Find("RightPanel").transform, false);
                StellarLogo.transform.localPosition = new Vector3(-0.2f, .4f, 5f);
                renderer = StellarLogo.AddComponent<SpriteRenderer>();

                instance = __instance;

                loadSprites();
                renderer.sprite = bannerSprite;

                GameObject credentialObject = new("credentialsSR");
                TextMeshPro credentials = credentialObject.AddComponent<TextMeshPro>();
                credentials.SetText($"v{StellarRolesPlugin.Version} {StellarRolesPlugin.BetaVersion}\n<size=40f%>\n</size>{mainMenuCredentials}\n{artCredentials}\n{contributorsCredentials}");
                credentials.alignment = TextAlignmentOptions.Center;
                credentials.fontSize *= 0.05f;

                credentials.transform.SetParent(StellarLogo.transform);
                credentials.transform.localPosition = Vector3.down * 2;
                //Modifies the loading icon to be the created sprite
                GameObject BackgroundFill = GameObject.Find("AccountManager/BackgroundFill");
                GameObject loading = BackgroundFill.transform.GetChild(0).gameObject;
                SpriteRenderer loadRend = loading.GetComponent<SpriteRenderer>();
                loadRend.sprite = playerSprite;
                loadRend.color = Color.blue;
                GameObject playerParticles = GameObject.Find("PlayerParticles");
                //Modifies each background crew on main menu to be blue
                for (int i = 0; i < playerParticles.transform.childCount; i++)
                {
                    playerParticles.transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().color = Color.blue;
                }
            }

            public static void loadSprites()
            {
                if (bannerSprite == null) bannerSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.Banner.png", 175f);
                if (playerSprite == null) playerSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.Gooper.png", 175f);
            }

            public static void updateSprite()
            {
                loadSprites();
                if (renderer != null)
                {
                    float fadeDuration = 1f;
                    instance.StartCoroutine(Effects.Lerp(fadeDuration, new Action<float>((p) =>
                    {
                        renderer.color = new Color(1, 1, 1, 1 - p);
                        if (p == 1)
                        {
                            renderer.sprite = bannerSprite;
                            instance.StartCoroutine(Effects.Lerp(fadeDuration, new Action<float>((p) =>
                            {
                                renderer.color = new Color(1, 1, 1, p);
                            })));
                        }
                    })));
                }
            }
        }
    }
}
