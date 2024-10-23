using HarmonyLib;
using System;
using TMPro;
using UnityEngine;

namespace StellarRoles.Patches
{
    [HarmonyPatch]
    public static class CredentialsPatch
    {
        public readonly static string StellarTitle = "<size=130%><color=#B2FEFE>StellarRoles</color></size>";
        public readonly static string Fluff = "<color=#C50000>Fluff</color>";
        public readonly static string Ilyssa = "<color=#C46AD8>Ilyssa</color>";
        public readonly static string Stell = "<color=#9bd3ff>Stell</color>";

        public readonly static string fullCredentials =
$@"{StellarTitle} v{StellarRolesPlugin.UpdateString} {StellarRolesPlugin.BetaVersion}
<size=80%>Created by {Fluff}, {Ilyssa}, and {Stell} </size>";

        public readonly static string mainMenuCredentials = $@"Created by {Fluff}, {Ilyssa}, and {Stell}";

        public readonly static string contributorsCredentials = $@"<size=80%> <color=#B2FEFE>Special thanks to Om3ga</color></size>";

        public readonly static string artCredentials = $@"<size=80f%>Art by Crayonvex, Stell, Phylo, and SeaGirl13</size>";


        [HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
        internal static class PingTrackerPatch
        {
            static void Postfix(PingTracker __instance)
            {
                __instance.text.alignment = TextAlignmentOptions.Top;
                __instance.transform.localScale = Vector3.one * 0.4f;
                var position = __instance.GetComponent<AspectPosition>();
                position.Alignment = AspectPosition.EdgeAlignments.Top;

                if (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started)
                {
                    __instance.text.text = Camera.main.orthographicSize == 3f
                        ? $"{StellarTitle} v{StellarRolesPlugin.UpdateString} {StellarRolesPlugin.BetaVersion}\n{__instance.text.text}" : "";
                    position.DistanceFromEdge = new Vector3(0.25f, 0.11f, 0);
                }
                else
                {
                    __instance.text.text = $"{fullCredentials}\n{__instance.text.text}";
                    position.DistanceFromEdge = new Vector3(0f, 0.1f, 0);
                }
            }
        }

        [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
        public static class LogoPatch
        {
            public static SpriteRenderer renderer;
            public static Sprite bannerSprite;
            public static Sprite playerSprite;
            public static Sprite[] Sprites;
            static void Postfix()
            {
                GameObject StellarLogo = new("bannerLogo_SR");
                StellarLogo.transform.SetParent(GameObject.Find("RightPanel").transform, false);
                StellarLogo.transform.localPosition = new Vector3(-0.2f, .4f, 5f);
                renderer = StellarLogo.AddComponent<SpriteRenderer>();

                loadSprites();
                renderer.sprite = bannerSprite;

                GameObject credentialObject = new("credentialsSR");
                TextMeshPro credentials = credentialObject.AddComponent<TextMeshPro>();
                credentials.SetText($"v{StellarRolesPlugin.UpdateString} {StellarRolesPlugin.BetaVersion}\n<size=40f%>\n</size>{mainMenuCredentials}\n{artCredentials}\n{contributorsCredentials}");
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

                ModManager.Instance.ShowModStamp();
                ModManager.Instance.enabled = true;
            }

            public static void loadSprites()
            {
                if (bannerSprite == null) bannerSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.Banner.png", 175f);
                if (playerSprite == null) playerSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.Gooper.png", 175f);
            }
        }
    }
}
