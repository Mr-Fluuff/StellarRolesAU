using AmongUs.Data;
using HarmonyLib;
using Newtonsoft.Json.Linq;
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
using UnityEngine.AddressableAssets;


namespace StellarRoles.Modules
{
    [HarmonyPatch]
    public class CustomVisors : VisorData
    {
        private static bool LOADED = false;
        private static bool RUNNING = false;
        public static Material visorShader;

        public static Dictionary<string, VisorExtenstion> CustomVisorExtRegistry = new();
        public static Dictionary<string, VisorViewData> CustomVisorViewDatas = new();

        public static VisorExtenstion TestExt = null;

        public class VisorExtenstion
        {
            public string author { get; set; }
            public string package { get; set; }
            public string condition { get; set; }
            public Sprite FlipImage { get; set; }
            public List<Sprite> Animation { get; set; }
            public int frame { get; set; }
            public float time { get; set; }
        }

        public class CustomVisor
        {
            public string Author { get; set; }
            public string Package { get; set; }
            public string Condition { get; set; }
            public string Name { get; set; }
            public string Resource { get; set; }
            public string FlipResource { get; set; }
            public List<string> Animation { get; set; }
            public string AnimationPrefix { get; set; }

            public bool InFront { get; set; }
            public bool Adaptive { get; set; }
        }

        private static Sprite CreateVisorSprite(string path, bool fromDisk = false)
        {
            Texture2D texture = fromDisk ? Helpers.LoadTextureFromDisk(path) : Helpers.LoadTextureFromResources(path);
            if (texture == null)
                return null;
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.53f, 0.575f), texture.width * 0.375f);
            if (sprite == null)
                return null;
            texture.hideFlags |= HideFlags.HideAndDontSave | HideFlags.DontUnloadUnusedAsset;
            sprite.hideFlags |= HideFlags.HideAndDontSave | HideFlags.DontUnloadUnusedAsset;
            return sprite;
        }

        private static VisorData CreateVisorBehaviour(CustomVisor ch, bool fromDisk = false)
        {
            if (visorShader == null)
            {
                visorShader = FastDestroyableSingleton<HatManager>.Instance.PlayerMaterial;
            }

            VisorViewData viewdata = CreateInstance<VisorViewData>();
            VisorData visor = CreateInstance<VisorData>();
            viewdata.IdleFrame = CreateVisorSprite(ch.Resource, fromDisk);
            viewdata.FloorFrame = viewdata.IdleFrame;
            visor.name = ch.Name;
            visor.displayOrder = 99;
            visor.ProductId = "CustomVisor_" + ch.Name.Replace(' ', '_');
            visor.behindHats = !ch.InFront;
            visor.ChipOffset = new Vector2(0f, 0.2f);
            visor.Free = true;
            visor.SpritePreview = viewdata.IdleFrame;
            if (ch.FlipResource != null)
                viewdata.LeftIdleFrame = CreateVisorSprite(ch.FlipResource, fromDisk);
            if (ch.Adaptive && visorShader != null)
                viewdata.AltShader = visorShader;


            VisorExtenstion extend = new()
            {
                author = ch.Author ?? "Unknown",
                package = ch.Package ?? "Misc.",
                condition = ch.Condition ?? "none"
            };

            if (ch.Animation != null)
            {
                string filePath = Path.GetDirectoryName(Application.dataPath) + @"\StellarVisors\AnimatedVisors\" + ch.Name + @"\";

                extend.Animation = new List<Sprite>();
                foreach (string frame in ch.Animation)
                {
                    extend.Animation.Add(CreateVisorSprite(filePath + frame, fromDisk));
                }
                extend.time = 0;
                extend.frame = 0;
            }

            CustomVisorExtRegistry.Add(visor.name, extend);
            CustomVisorViewDatas.Add(visor.ProductId, viewdata);

            AssetReference assetRef = new(viewdata.Pointer);

            visor.ViewDataRef = assetRef;
            visor.CreateAddressableAsset();

            return visor;
        }

        private static VisorData CreateVisorBehaviour(CustomVisorLoader.CustomVisorOnline chd)
        {
            string filePath = Path.GetDirectoryName(Application.dataPath) + @"\StellarVisors\";
            chd.Resource = filePath + chd.Resource;
            if (chd.FlipResource != null)
                chd.FlipResource = filePath + chd.FlipResource;
            return CreateVisorBehaviour(chd, true);
        }

        static VisorViewData getVisorViewData(string id)
        {
            return CustomVisorViewDatas.ContainsKey(id) ? CustomVisorViewDatas[id] : null;
        }

        [HarmonyPatch(typeof(CosmeticsCache), nameof(CosmeticsCache.GetVisor))]
        class CosmeticsCacheGetVisorPatch
        {
            public static bool Prefix(CosmeticsCache __instance, string id, ref VisorViewData __result)
            {
                if (!id.StartsWith("CustomVisor_")) return true;
                __result = getVisorViewData(id);
                if (__result == null)
                    __result = __instance.visors["visor_EmptyVisor"].GetAsset();
                return false;
            }
        }

        [HarmonyPatch(typeof(HatManager), nameof(HatManager.GetVisorById))]
        private static class HatManagerPatch
        {
            private static List<VisorData> allVisorsList;
            static void Prefix(HatManager __instance)
            {
                if (RUNNING || LOADED) return;
                RUNNING = true; // prevent simultanious execution
                allVisorsList = __instance.allVisors.ToList();

                try
                {
                    while (CustomVisorLoader.VistorDetails.Count > 0)
                    {
                        allVisorsList.Add(CreateVisorBehaviour(CustomVisorLoader.VistorDetails[0]));
                        CustomVisorLoader.VistorDetails.RemoveAt(0);
                    }
                    __instance.allVisors = allVisorsList.ToArray();
                    LOADED = true;
                }
                catch (Exception e)
                {
                    if (!LOADED)
                        System.Console.WriteLine("Unable to add Custom Visors\n" + e);
                }
            }
            static void Postfix()
            {
                RUNNING = false;
            }
        }

        [HarmonyPatch(typeof(VisorLayer), nameof(VisorLayer.SetFlipX))]
        class VisorLayerSetFlipXPatch
        {
            public static bool Prefix(VisorLayer __instance, bool flipX)
            {
                if (__instance.currentVisor == null || !__instance.currentVisor.ProductId.StartsWith("CustomVisor_")) return true;

                __instance.Image.flipX = flipX;
                VisorViewData asset = getVisorViewData(__instance.currentVisor.ProdId);
                if (flipX && asset.LeftIdleFrame)
                {
                    __instance.Image.sprite = asset.LeftIdleFrame;
                }
                else
                {
                    __instance.Image.sprite = asset.IdleFrame;
                }
                return false;
            }
        }

        [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.FixedUpdate))]
        private static class PlayerPhysicsHandleAnimationPatch
        {
            public static float delay = 10;
            public static float currenttime => Time.deltaTime * 150;
            private static void Postfix(PlayerPhysics __instance)
            {
                AnimationClip currentAnimation = __instance.Animations.Animator.GetCurrentAnimation();
                PlayerControl player = __instance.myPlayer;

                if (currentAnimation == __instance.Animations.group.ClimbUpAnim || currentAnimation == __instance.Animations.group.ClimbDownAnim) return;

                if (player.cosmetics.visor.currentVisor != null)
                {
                    string visorId = player.cosmetics.visor.currentVisor.ProductId;
                    VisorViewData viewData = getVisorViewData(visorId);
                    SetVisor(__instance, viewData);
                }

                if (player.AmOwner)
                {
                    CustomVisorExtRegistry.Values.ToList().ForEach(extend =>
                    {
                        extend.time += currenttime;
                        if (extend.time >= delay)
                        {
                            UpdateVisorFrames(extend);
                            extend.time = 0;
                        }
                    });
                }
            }

            public static void SetVisor(PlayerPhysics __instance, VisorViewData viewData)
            {
                VisorLayer visor = __instance.myPlayer.cosmetics.visor;
                if (visor == null) return;
                VisorExtenstion extend = visor.currentVisor.GetVisorExtension();
                if (extend == null) return;

                visor.Image.sprite = extend.Animation?.Count > 0 ? visor.Image.sprite = extend.Animation[extend.frame]
                    : (extend.FlipImage != null && __instance.FlipX) ? viewData.LeftIdleFrame
                    : viewData.IdleFrame;
            }

            public static void UpdateVisorFrames(VisorExtenstion extenstion)
            {
                if (extenstion.Animation != null && extenstion.Animation.Count > 0)
                {
                    extenstion.frame++;

                    if (extenstion.frame >= extenstion.Animation.Count)
                    {
                        extenstion.frame = 0;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(VisorLayer), nameof(VisorLayer.SetVisor), new Type[] { typeof(VisorData), typeof(VisorViewData), typeof(int) })]
        class VisorLayerSetVisor2Patch
        {
            public static bool Prefix(VisorLayer __instance, VisorData data, VisorViewData visorView, int colorId)
            {
                if (!data.ProductId.StartsWith("CustomVisor_")) return true;
                __instance.currentVisor = data;
                __instance.transform.SetLocalZ(__instance.ZIndexSpacing * (data.BehindHats ? -1.5f : -3f));
                __instance.SetFlipX(__instance.Image.flipX);
                __instance.SetMaterialColor(colorId);
                return false;
            }
        }

        [HarmonyPatch(typeof(VisorLayer), nameof(VisorLayer.SetVisor), new Type[] { typeof(VisorData), typeof(int) })]
        class VisorLayerSetVisorPatch
        {
            public static bool Prefix(VisorLayer __instance, VisorData data, int colorId)
            {
                if (!data.ProductId.StartsWith("CustomVisor_")) return true;
                __instance.currentVisor = data;
                __instance.SetVisor(__instance.currentVisor, getVisorViewData(data.ProdId), colorId);
                return false;
            }
        }


        [HarmonyPatch(typeof(VisorLayer), nameof(VisorLayer.UpdateMaterial))]
        class VisorLayerUpdateMaterialPatch
        {
            public static bool Prefix(VisorLayer __instance)
            {
                if (__instance.currentVisor == null || !__instance.currentVisor.ProductId.StartsWith("CustomVisor_")) return true;
                VisorViewData asset = getVisorViewData(__instance.currentVisor.ProductId);
                __instance.Image.sharedMaterial = asset.AltShader ?? DestroyableSingleton<HatManager>.Instance.DefaultShader;
                PlayerMaterial.SetColors(__instance.matProperties.ColorId, __instance.Image);
                switch (__instance.matProperties.MaskType)
                {
                    case PlayerMaterial.MaskType.SimpleUI:
                    case PlayerMaterial.MaskType.ScrollingUI:
                        __instance.Image.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                        break;
                    case PlayerMaterial.MaskType.Exile:
                        __instance.Image.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
                        break;
                    default:
                        __instance.Image.maskInteraction = SpriteMaskInteraction.None;
                        break;
                }
                return false;
            }
        }

        [HarmonyPatch(typeof(VisorsTab), nameof(VisorsTab.OnEnable))]
        public class VisorsTabOnEnablePatch
        {
            public const string innerslothPackageName = "Innersloth Visors";
            private static TMPro.TMP_Text textTemplate;

            public static float CreateVisorPackage(List<(VisorData, VisorExtenstion)> visors, string packageName, float YStart, VisorsTab __instance)
            {
                bool isDefaultPackage = innerslothPackageName == packageName;
                if (!isDefaultPackage)
                    visors = visors.OrderBy(x => x.Item1.name).ToList();
                float offset = YStart;

                if (textTemplate != null)
                {
                    TMPro.TMP_Text title = Instantiate(textTemplate, __instance.scroller.Inner);
                    title.transform.localPosition = new Vector3(2.25f, YStart, -1f);
                    title.transform.localScale = Vector3.one * 1.5f;
                    title.fontSize *= 0.5f;
                    title.enableAutoSizing = false;
                    __instance.StartCoroutine(Effects.Lerp(0.1f, new Action<float>((p) => { title.SetText(packageName); })));
                    offset -= 0.8f * __instance.YOffset;
                }
                for (int i = 0; i < visors.Count; i++)
                {
                    VisorData visor = visors[i].Item1;
                    VisorExtenstion ext = visors[i].Item2;

                    float xpos = __instance.XRange.Lerp(i % __instance.NumPerRow / (__instance.NumPerRow - 1f));
                    float ypos = offset - i / __instance.NumPerRow * (isDefaultPackage ? 1f : 1.5f) * __instance.YOffset;
                    ColorChip colorChip = Instantiate(__instance.ColorTabPrefab, __instance.scroller.Inner);
                    if (ActiveInputManager.currentControlType == ActiveInputManager.InputType.Keyboard)
                    {
                        colorChip.Button.OnMouseOver.AddListener((Action)(() => __instance.SelectVisor(visor)));
                        colorChip.Button.OnMouseOut.AddListener((Action)(() => __instance.SelectVisor(FastDestroyableSingleton<HatManager>.Instance.GetVisorById(DataManager.Player.Customization.Visor))));
                        colorChip.Button.OnClick.AddListener((Action)(() => __instance.ClickEquip()));
                    }
                    else
                    {
                        colorChip.Button.OnClick.AddListener((Action)(() => __instance.SelectVisor(visor)));
                    }
                    colorChip.Button.ClickMask = __instance.scroller.Hitbox;
                    Transform background = colorChip.transform.FindChild("Background");
                    Transform foreground = colorChip.transform.FindChild("ForeGround");

                    if (ext != null)
                    {
                        if (background != null)
                        {
                            background.localPosition = Vector3.down * 0.243f;
                            background.localScale = new Vector3(background.localScale.x, 0.8f, background.localScale.y);
                        }
                        if (foreground != null)
                        {
                            foreground.localPosition = Vector3.down * 0.243f;
                        }

                        if (textTemplate != null)
                        {
                            TMPro.TMP_Text description = Instantiate(textTemplate, colorChip.transform);
                            description.transform.localPosition = new Vector3(0f, -0.65f, -1f);
                            description.alignment = TMPro.TextAlignmentOptions.Center;
                            description.transform.localScale = Vector3.one * 0.65f;
                            __instance.StartCoroutine(Effects.Lerp(0.1f, new Action<float>((p) => { description.SetText($"{visor.name}\nby {ext.author}"); })));
                        }
                    }

                    colorChip.transform.localPosition = new Vector3(xpos, ypos, -1f);
                    colorChip.ProductId = visor.ProductId;
                    colorChip.Inner.transform.localPosition = visor.ChipOffset;
                    __instance.UpdateMaterials(colorChip.Inner.FrontLayer, visor);
                    visor.SetPreview(colorChip.Inner.FrontLayer, __instance.HasLocalPlayer() ? PlayerControl.LocalPlayer.Data.DefaultOutfit.ColorId : ((int)DataManager.Player.Customization.Color));
                    colorChip.Tag = visor;
                    colorChip.SelectionHighlight.gameObject.SetActive(false);
                    __instance.ColorChips.Add(colorChip);
                }
                return offset - (visors.Count - 1) / __instance.NumPerRow * (isDefaultPackage ? 1f : 1.5f) * __instance.YOffset - 1.75f;
            }

            public static void Postfix(VisorsTab __instance)
            {
                for (int i = 0; i < __instance.scroller.Inner.childCount; i++)
                    Destroy(__instance.scroller.Inner.GetChild(i).gameObject);
                __instance.ColorChips = new Il2CppSystem.Collections.Generic.List<ColorChip>();

                VisorData[] unlockedVisors = FastDestroyableSingleton<HatManager>.Instance.GetUnlockedVisors();
                Dictionary<string, List<(VisorData, VisorExtenstion)>> packages = new();

                foreach (VisorData visorBehavior in unlockedVisors)
                {
                    VisorExtenstion ext = visorBehavior.GetVisorExtension();

                    if (ext != null)
                    {
                        if (!packages.ContainsKey(ext.package))
                            packages[ext.package] = new List<(VisorData, VisorExtenstion)>();
                        packages[ext.package].Add((visorBehavior, ext));
                    }
                    else
                    {
                        if (!packages.ContainsKey(innerslothPackageName))
                            packages[innerslothPackageName] = new List<(VisorData, VisorExtenstion)>();
                        packages[innerslothPackageName].Add((visorBehavior, null));
                    }
                }

                float YOffset = __instance.YStart;
                textTemplate = GameObject.Find("VisorGroup").transform.FindChild("Text").GetComponent<TMPro.TMP_Text>();

                IOrderedEnumerable<string> orderedKeys = packages.Keys.OrderBy((x) => x switch
                {
                    "Developer Visors" => 1,
                    "Animated Visors" => 2,
                    "StellarVisors" => 3,
                    innerslothPackageName => 4,
                    _ => 5,
                });
                foreach (string key in orderedKeys)
                    YOffset = CreateVisorPackage(packages[key], key, YOffset, __instance);

                __instance.scroller.ContentYBounds.max = -(YOffset + 4.1f);
            }
        }

    }

    public class CustomVisorLoader
    {
        private static bool Running = false;
        private const string REPO_SV = "https://raw.githubusercontent.com/Mr-Fluuff/StellarHats/main";

        public static readonly List<CustomVisorOnline> VistorDetails = new();
        public static void LaunchVisorFetcher()
        {
            if (Running)
                return;
            Running = true;
            _ = LaunchVisorFetcherAsync();
        }

        private static async Task LaunchVisorFetcherAsync()
        {
            try
            {
                HttpStatusCode status = await FetchVisors_SV();
                if (status != HttpStatusCode.OK)
                    Helpers.Log(LogLevel.Warning, "Custom SH Visors could not be loaded");
            }
            catch (Exception e)
            {
                Helpers.Log(LogLevel.Error, "Unable to fetch visors: " + e.StackTrace);
            }
            Running = false;
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

        public static async Task<HttpStatusCode> FetchVisors_SV()
        {
            HttpClient http = new();
            http.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };
            HttpResponseMessage response = await http.GetAsync(new Uri($"{REPO_SV}/CustomVisors.json"), HttpCompletionOption.ResponseContentRead);
            try
            {
                if (response.StatusCode != HttpStatusCode.OK) return response.StatusCode;
                if (response.Content == null)
                {
                    Helpers.Log(LogLevel.Warning, "Server returned no data (fetching visors): " + response.StatusCode.ToString());
                    return HttpStatusCode.ExpectationFailed;
                }
                string json = await response.Content.ReadAsStringAsync();
                JToken jObj = JObject.Parse(json)["visors"];
                if (!jObj.HasValues) return HttpStatusCode.ExpectationFailed;

                for (JToken current = jObj.First; current != null; current = current.Next)
                {
                    if (current.HasValues)
                    {
                        string name = current["name"]?.ToString();
                        string resource = SanitizeResourcePath(current["resource"]?.ToString());

                        if (resource == null || name == null) // required
                            continue;
                        CustomVisorOnline info = new()
                        {
                            Name = name,
                            Resource = resource,
                            ResHashA = current["reshasha"]?.ToString(),
                            FlipResource = SanitizeResourcePath(current["flipresource"]?.ToString()),
                            ResHashF = current["reshashf"]?.ToString(),

                            Author = current["author"]?.ToString(),
                            Package = current["package"]?.ToString(),
                            Condition = current["condition"]?.ToString(),
                            Adaptive = current["adaptive"] != null,
                            InFront = current["infront"] != null,

                            Animation = new List<string>(),
                            AnimationPrefix = current["animationprefix"]?.ToString()
                        };

                        int frames = current["animationframes"] == null ? 0 : int.Parse(current["animationframes"].ToString());

                        if (info.AnimationPrefix != null && frames > 0)
                        {
                            List<string> animationList = info.Animation = new();
                            for (int i = 0; i < frames; i++)
                                animationList.Add($"{info.AnimationPrefix}_{i:000}.png");
                        }
                        VistorDetails.Add(info);
                    }
                }

                List<string> markedForDownload = new();
                List<(string, string)> markedForDownload2 = new();

                string filePath = Path.GetDirectoryName(Application.dataPath) + @"\StellarVisors\";
                string animatedFilePath = Path.GetDirectoryName(Application.dataPath) + @"\StellarVisors\AnimatedVisors\";
                if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
                if (!Directory.Exists(animatedFilePath)) Directory.CreateDirectory(animatedFilePath);
                MD5 md5 = MD5.Create();

                foreach (CustomVisorOnline data in VistorDetails)
                {
                    if (DoesResourceRequireDownload(filePath + data.Resource, data.ResHashA, md5))
                        markedForDownload.Add(data.Resource);
                    if (data.FlipResource != null && DoesResourceRequireDownload(filePath + data.FlipResource, data.ResHashF, md5))
                        markedForDownload.Add(data.FlipResource);
                    if (data.Animation.Count > 0)
                    {
                        string newpath = animatedFilePath + data.Name + @"\";
                        if (!Directory.Exists(newpath)) Directory.CreateDirectory(newpath);
                        int files = Directory.GetFiles(newpath).Count(x => x.StartsWith($"{newpath}{data.AnimationPrefix}"));
                        if (DoesResourceRequireDownload(filePath + data.Resource, data.ResHashA, md5) || files != data.Animation.Count)
                            foreach (string frame in data.Animation)
                                markedForDownload2.Add((data.Name, frame));
                    }
                }
                while (markedForDownload.Count > 0)
                {
                    string file = markedForDownload[0];
                    HttpResponseMessage visorFileResponse = await http.GetAsync($"{REPO_SV}/visors/{file}", HttpCompletionOption.ResponseContentRead);
                    if (visorFileResponse.StatusCode != HttpStatusCode.OK) continue;
                    using Stream responseStream = await visorFileResponse.Content.ReadAsStreamAsync();
                    using FileStream fileStream = File.Create($"{filePath}\\{file}");
                    responseStream.CopyTo(fileStream);
                    markedForDownload.RemoveAt(0);
                }

                while (markedForDownload2.Count > 0)
                {
                    (string name, string frame) = markedForDownload2[0];

                    string newPath = animatedFilePath + name + @"\";
                    HttpResponseMessage visorFileResponse = await http.GetAsync($"{REPO_SV}/visors/{name}/{frame}", HttpCompletionOption.ResponseContentRead);
                    if (visorFileResponse.StatusCode != HttpStatusCode.OK) continue;
                    using Stream responseStream = await visorFileResponse.Content.ReadAsStreamAsync();
                    using FileStream fileStream = File.Create($"{newPath}\\{frame}");
                    responseStream.CopyTo(fileStream);
                    //Helpers.Log(ErrorLevel.Message, $"Animated Visor {frame} Downloaded");
                    markedForDownload2.RemoveAt(0);
                }
            }
            catch (Exception ex)
            {
                Helpers.Log(LogLevel.Error, "Error fetching visor metadata: " + ex.ToString());
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

        public class CustomVisorOnline : CustomVisors.CustomVisor
        {
            public string ResHashA { get; set; }
            public string ResHashF { get; set; }
        }
    }
    public static class CustomVisorExtensions
    {
        public static CustomVisors.VisorExtenstion GetVisorExtension(this VisorData visor)
        {
            return CustomVisors.CustomVisorExtRegistry.TryGetValue(visor.name, out CustomVisors.VisorExtenstion ext) ? ext : null;
        }
    }
}
