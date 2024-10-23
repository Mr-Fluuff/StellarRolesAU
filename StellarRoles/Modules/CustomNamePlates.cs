using AmongUs.Data;
using HarmonyLib;
using Innersloth.Assets;
using Newtonsoft.Json.Linq;
using Reactor.Utilities.Extensions;
using StellarRoles.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Threading.Tasks;
using UnityEngine;


namespace StellarRoles.Modules
{
    [HarmonyPatch]
    public class CustomNamePlates
    {
        private static bool LOADED = false;
        private static bool RUNNING = false;

        public static Dictionary<string, NameplateExtenstion> CustomNameplateExtRegistry = new();
        public static Dictionary<string, NamePlateViewData> CustomNameplateViewDatas = new();

        public static NameplateExtenstion TestExt = null;

        public class NameplateExtenstion
        {
            public string author { get; set; }
            public string package { get; set; }
            public string condition { get; set; }
        }

        public class CustomNameplate
        {
            public string Author { get; set; }
            public string Package { get; set; }
            public string Condition { get; set; }
            public string Name { get; set; }
            public string Resource { get; set; }
        }

        private static Sprite CreateNameplateSprite(string path, bool fromDisk = false)
        {
            Texture2D texture = fromDisk ? Helpers.LoadTextureFromDisk(path) : Helpers.LoadTextureFromResources(path);
            if (texture == null)
                return null;
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), texture.width * 0.375f);
            if (sprite == null)
                return null;

            sprite.hideFlags |= HideFlags.HideAndDontSave | HideFlags.DontUnloadUnusedAsset;
            return sprite;
        }

        private static NamePlateData CreateNameplateBehaviour(CustomNameplate cn, bool fromDisk = false)
        {
            NamePlateViewData viewdata = ScriptableObject.CreateInstance<NamePlateViewData>().DontDestroy();
            NamePlateData nameplate = ScriptableObject.CreateInstance<NamePlateData>().DontDestroy();
            viewdata.Image = CreateNameplateSprite(cn.Resource, fromDisk);
            //nameplate.SpritePreview = viewdata.Image;
            nameplate.PreviewCrewmateColor = false;
            nameplate.name = cn.Name;
            nameplate.displayOrder = 99;
            nameplate.ProductId = "CustomNameplate_" + cn.Name.Replace(' ', '_');
            nameplate.ChipOffset = new Vector2(0f, 0.2f);
            nameplate.Free = true;

            NameplateExtenstion extend = new()
            {
                author = cn.Author ?? "Unknown",
                package = cn.Package ?? "Misc.",
                condition = cn.Condition ?? "none"
            };

            CustomNameplateExtRegistry.Add(nameplate.name, extend);
            CustomNameplateViewDatas.Add(nameplate.ProductId, viewdata);

            nameplate.ViewDataRef = new(viewdata.Pointer);
            nameplate.CreateAddressableAsset();

            return nameplate;
        }

        private static NamePlateData CreateNameplateBehaviour(CustomNameplateLoader.CustomNameplateOnline cnd)
        {
            string filePath = Path.GetDirectoryName(Application.dataPath) + @"\StellarPlates\";
            cnd.Resource = filePath + cnd.Resource;
            return CreateNameplateBehaviour(cnd, true);
        }

        static NamePlateViewData getNameplateViewData(string id)
        {
            return CustomNameplateViewDatas.ContainsKey(id) ? CustomNameplateViewDatas[id] : null;
        }

        [HarmonyPatch(typeof(CosmeticsCache), nameof(CosmeticsCache.GetNameplate))]
        class CosmeticsCacheGetNameplatePatch
        {
            public static bool Prefix(CosmeticsCache __instance, string id, ref NamePlateViewData __result)
            {
                if (!CustomNameplateViewDatas.TryGetValue(id, out __result))
                    return true;

                if (__result == null)
                    __result = __instance.nameplates["nameplate_NoPlate"].GetAsset();
                return false;
            }
        }

        [HarmonyPatch(typeof(PlayerVoteArea), nameof(PlayerVoteArea.PreviewNameplate))]
        public class PreviewNameplatesPatch
        {
            public static void Postfix(PlayerVoteArea __instance, ref string plateID)
            {
                if (!CustomNameplateViewDatas.TryGetValue(plateID, out var viewData))
                    return;

                __instance.Background.sprite = viewData?.Image;
            }
        }

        [HarmonyPatch(typeof(HatManager), nameof(HatManager.GetNamePlateById))]
        public static class HatManagerPatch
        {
            private static List<NamePlateData> allNameplatesList;
            private static List<NamePlateData> VanillaNameplateList;
            static void Prefix(HatManager __instance)
            {
                if (RUNNING || LOADED) return;
                LoadNameplates(__instance, true);
            }

            static void Postfix()
            {
                RUNNING = false;
            }

            public static void LoadNameplates(HatManager __instance, bool GameStart)
            {
                RUNNING = true; // prevent simultanious execution
                allNameplatesList = __instance.allNamePlates.ToList();

                if (GameStart)
                {
                    VanillaNameplateList = allNameplatesList;
                }
                else
                {
                    allNameplatesList = VanillaNameplateList;
                }

                try
                {
                    while (CustomNameplateLoader.NameplateDetails.Count > 0)
                    {
                        allNameplatesList.Add(CreateNameplateBehaviour(CustomNameplateLoader.NameplateDetails[0]));
                        CustomNameplateLoader.NameplateDetails.RemoveAt(0);
                    }
                    __instance.allNamePlates = allNameplatesList.ToArray();
                    LOADED = true;
                }
                catch (Exception e)
                {
                    if (!LOADED)
                        System.Console.WriteLine("Unable to add Custom Nameplates\n" + e);
                }

                if (!GameStart)
                {
                    RUNNING = false;
                }
            }
        }

        [HarmonyPatch(typeof(NameplatesTab), nameof(NameplatesTab.OnEnable))]
        public class NameplatesTabOnEnablePatch
        {
            public const string innerslothPackageName = "Innersloth Nameplates";
            private static TMPro.TMP_Text textTemplate;

            public static float CreateNameplatePackage(List<(NamePlateData, NameplateExtenstion)> nameplates, string packageName, float YStart, NameplatesTab __instance)
            {
                bool isDefaultPackage = innerslothPackageName == packageName;
                if (!isDefaultPackage)
                    nameplates = nameplates.OrderBy(x => x.Item1.name).ToList();
                float offset = YStart;

                if (textTemplate != null)
                {
                    TMPro.TMP_Text title = UnityEngine.Object.Instantiate(textTemplate, __instance.scroller.Inner);
                    var material = title.GetComponent<MeshRenderer>().material;
                    material.SetFloat("_StencilComp", 4f);
                    material.SetFloat("_Stencil", 1f);

                    title.transform.localPosition = new Vector3(2.25f, YStart, -1f);
                    title.transform.localScale = Vector3.one * 1.5f;
                    title.fontSize *= 0.5f;
                    title.enableAutoSizing = false;
                    __instance.StartCoroutine(Effects.Lerp(0.1f, new Action<float>((p) => { title.SetText(packageName); })));
                    offset -= 0.8f * __instance.YOffset;
                }
                for (int i = 0; i < nameplates.Count; i++)
                {
                    NamePlateData nameplate = nameplates[i].Item1;
                    NameplateExtenstion ext = nameplates[i].Item2;

                    float xpos = __instance.XRange.Lerp(i % __instance.NumPerRow / (__instance.NumPerRow - 1f));
                    float ypos = offset - i / __instance.NumPerRow * (isDefaultPackage ? 1f : 1.5f) * __instance.YOffset;
                    ColorChip colorChip = UnityEngine.Object.Instantiate(__instance.ColorTabPrefab, __instance.scroller.Inner);
                    if (ActiveInputManager.currentControlType == ActiveInputManager.InputType.Keyboard)
                    {
                        colorChip.Button.OnMouseOver.AddListener((Action)(() => __instance.SelectNameplate(nameplate)));
                        colorChip.Button.OnMouseOut.AddListener((Action)(() => __instance.SelectNameplate(HatManager.Instance.GetNamePlateById(DataManager.Player.Customization.NamePlate))));
                        colorChip.Button.OnClick.AddListener((Action)(() => __instance.ClickEquip()));
                    }
                    else
                    {
                        colorChip.Button.OnClick.AddListener((Action)(() => __instance.SelectNameplate(nameplate)));
                    }

                    colorChip.Button.ClickMask = __instance.scroller.Hitbox;
                    if (ext != null)
                    {
                        if (textTemplate != null)
                        {
                            TMPro.TMP_Text description = UnityEngine.Object.Instantiate(textTemplate, colorChip.transform);
                            description.transform.localPosition = new Vector3(0.15f, -0.5f, -1f);
                            description.alignment = TMPro.TextAlignmentOptions.Center;
                            description.transform.localScale = Vector3.one * 0.55f;
                            __instance.StartCoroutine(Effects.Lerp(0.1f, new Action<float>((p) => { description.SetText($"{nameplate.name} by {ext.author}"); })));
                        }
                    }
                    colorChip.transform.localPosition = new(xpos, ypos, -1f);
                    colorChip.ProductId = nameplate.ProductId;
                    colorChip.Tag = nameplate;
                    colorChip.SelectionHighlight.gameObject.SetActive(false);

                    if (CustomNameplateViewDatas.TryGetValue(colorChip.ProductId, out var viewData))
                        colorChip.gameObject.GetComponent<NameplateChip>().image.sprite = viewData.Image;
                    else
                        DefaultNameplateCoro(__instance, colorChip.gameObject.GetComponent<NameplateChip>());

                    __instance.ColorChips.Add(colorChip);
                }
                return offset - (nameplates.Count - 1) / __instance.NumPerRow * (isDefaultPackage ? 1f : 1.5f) * __instance.YOffset - 1.75f;
            }

            public static void DefaultNameplateCoro(NameplatesTab __instance, NameplateChip chip)
            {
                __instance.StartCoroutine(__instance.CoLoadAssetAsync<NamePlateViewData>(HatManager.Instance.GetNamePlateById(chip.ProductId).ViewDataRef, (Action<NamePlateViewData>)(viewData =>
                    chip.image.sprite = viewData?.Image)));
            }

            public static bool Prefix(NameplatesTab __instance)
            {
                for (int i = 0; i < __instance.scroller.Inner.childCount; i++)
                    UnityEngine.Object.Destroy(__instance.scroller.Inner.GetChild(i).gameObject);
                __instance.ColorChips = new();

                NamePlateData[] unlockedNameplates = HatManager.Instance.GetUnlockedNamePlates();
                Dictionary<string, List<(NamePlateData, NameplateExtenstion)>> packages = new();

                foreach (NamePlateData nameplateBehavior in unlockedNameplates)
                {
                    NameplateExtenstion ext = nameplateBehavior.GetNameplateExtension();

                    if (ext != null)
                    {
                        if (!packages.ContainsKey(ext.package))
                            packages[ext.package] = new List<(NamePlateData, NameplateExtenstion)>();
                        packages[ext.package].Add((nameplateBehavior, ext));
                    }
                    else
                    {
                        if (!packages.ContainsKey(innerslothPackageName))
                            packages[innerslothPackageName] = new List<(NamePlateData, NameplateExtenstion)>();
                        packages[innerslothPackageName].Add((nameplateBehavior, null));
                    }
                }

                float YOffset = __instance.YStart;
                textTemplate = __instance.transform.FindChild("Text").GetComponent<TMPro.TMP_Text>();

                IOrderedEnumerable<string> orderedKeys = packages.Keys.OrderBy((x) => x switch
                {
                    "Developer Plates" => 1,
                    "StellarPlates" => 2,
                    innerslothPackageName => 3,
                    _ => 4,
                });
                foreach (string key in orderedKeys)
                    YOffset = CreateNameplatePackage(packages[key], key, YOffset, __instance);

                __instance.scroller.ContentYBounds.max = -(YOffset + 4.1f);
                return false;
            }
        }

    }

    public class CustomNameplateLoader
    {
        public static bool IsRunning = false;
        private const string REPO_SV = "https://raw.githubusercontent.com/Mr-Fluuff/StellarHats/main";
        public static int TotalNameplatesDownloaded;
        public static int TotalNameplatesToDownload;

        public static List<CustomNameplateOnline> NameplateDetails = new();
        public static void LaunchNameplateFetcher()
        {
            if (IsRunning)
                return;
            IsRunning = true;
            TotalNameplatesDownloaded = 0;
            _ = LaunchNameplateFetcherAsync();
        }

        private static async Task LaunchNameplateFetcherAsync()
        {
            try
            {
                HttpStatusCode status = await FetchNameplates_SN();
                if (status != HttpStatusCode.OK)
                    Helpers.Log(LogLevel.Warning, "Custom SH Nameplates could not be loaded");
            }
            catch (Exception e)
            {
                Helpers.Log(LogLevel.Error, "Unable to fetch Nameplates: " + e.StackTrace);
            }
            IsRunning = false;
        }

        private static string SanitizeResourcePath(string res)
        {
            if (res == null || !res.EndsWith(".png"))
                return null;

            return res
                .Replace("\\", "")
                .Replace("/", "")
                .Replace("*", "")
                .Replace("..", "");
        }

        public static async Task<HttpStatusCode> FetchNameplates_SN()
        {
            HttpClient http = new();
            http.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };
            HttpResponseMessage response = await http.GetAsync(new Uri($"{REPO_SV}/CustomPlates.json"), HttpCompletionOption.ResponseContentRead);
            try
            {
                if (response.StatusCode != HttpStatusCode.OK) return response.StatusCode;
                if (response.Content == null)
                {
                    Helpers.Log(LogLevel.Warning, "Server returned no data (fetching Nameplates): " + response.StatusCode.ToString());
                    return HttpStatusCode.ExpectationFailed;
                }
                string json = await response.Content.ReadAsStringAsync();
                JToken jObj = JObject.Parse(json)["plates"];
                if (!jObj.HasValues) return HttpStatusCode.ExpectationFailed;

                List<CustomNameplateOnline> nameplatedatas = new();

                for (JToken current = jObj.First; current != null; current = current.Next)
                {
                    if (current.HasValues)
                    {
                        string name = current["name"]?.ToString();
                        string resource = SanitizeResourcePath(current["resource"]?.ToString());

                        if (resource == null || name == null) // required
                            continue;
                        CustomNameplateOnline info = new()
                        {
                            Name = name,
                            Resource = resource,
                            ResHashA = current["reshasha"]?.ToString(),

                            Author = current["author"]?.ToString(),
                            Package = current["package"]?.ToString(),
                            Condition = current["condition"]?.ToString(),
                        };

                        nameplatedatas.Add(info);
                    }
                }

                List<string> markedForDownload = new();

                string filePath = Path.GetDirectoryName(Application.dataPath) + @"\StellarPlates\";
                if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
                MD5 md5 = MD5.Create();

                foreach (CustomNameplateOnline data in nameplatedatas)
                {
                    if (DoesResourceRequireDownload(filePath + data.Resource, data.ResHashA, md5))
                        markedForDownload.Add(data.Resource);
                }

                TotalNameplatesToDownload = markedForDownload.Count;

                if (markedForDownload.Count <= 0)
                {
                    NameplateDetails = nameplatedatas;
                }
                else
                {
                    for (int i = 0; i < markedForDownload.Count; i++)
                    {
                        string file = markedForDownload[i];
                        Helpers.Log(file + " Downloaded");
                        TotalNameplatesDownloaded++;

                        HttpResponseMessage NameplateFileResponse = await http.GetAsync($"{REPO_SV}/plates/{file}", HttpCompletionOption.ResponseContentRead);
                        if (NameplateFileResponse.StatusCode != HttpStatusCode.OK) continue;
                        using Stream responseStream = await NameplateFileResponse.Content.ReadAsStreamAsync();
                        using FileStream fileStream = File.Create($"{filePath}\\{file}");
                        responseStream.CopyTo(fileStream);
                    }
                    NameplateDetails = nameplatedatas;
                }
            }
            catch (Exception ex)
            {
                Helpers.Log(LogLevel.Error, "Error fetching Nameplate metadata: " + ex.ToString());
            }
            return HttpStatusCode.OK;
        }

        private static bool DoesResourceRequireDownload(string respath, string reshash, MD5 md5)
        {
            if (reshash == null || !File.Exists(respath))
                return true;

            using FileStream stream = File.OpenRead(respath);
            return !reshash.Equals(BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLowerInvariant());
        }

        public class CustomNameplateOnline : CustomNamePlates.CustomNameplate
        {
            public string ResHashA { get; set; }
        }
    }
    public static class CustomNameplateExtensions
    {
        public static CustomNamePlates.NameplateExtenstion GetNameplateExtension(this NamePlateData nameplate)
        {
            return CustomNamePlates.CustomNameplateExtRegistry.TryGetValue(nameplate.name, out CustomNamePlates.NameplateExtenstion ext) ? ext : null;
        }
    }
}
