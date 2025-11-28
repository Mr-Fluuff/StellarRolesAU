using AmongUs.Data;
using HarmonyLib;
using Newtonsoft.Json.Linq;
using PowerTools;
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
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace StellarRoles.Modules
{
    [HarmonyPatch]
    public class CustomHats
    {
        private static bool Loaded = false;
        private static bool IsRunning = false;
        public static Material HatShader { get; set; } = null;

        public static readonly Dictionary<string, HatExtension> CustomHatRegistry = new();
        public static readonly Dictionary<string, HatViewData> CustomHatViewDatas = new();

        public static HatExtension TestExt { get; set; } = null;

        public class HatExtension
        {
            public string Author { get; set; }
            public string Package { get; set; }
            public string Condition { get; set; }
            public Sprite FlipImage { get; set; }
            public Sprite BackFlipImage { get; set; }
            public List<Sprite> Animation { get; set; }
            public List<Sprite> BackAnimation { get; set; }
            public int Frame { get; set; }
            public float Time { get; set; }
        }

        public class CustomHat
        {
            public string Author { get; set; }
            public string Package { get; set; }
            public string Condition { get; set; }
            public string Name { get; set; }
            public string Resource { get; set; }
            public string FlipResource { get; set; }
            public string BackflipResource { get; set; }
            public string BackResource { get; set; }
            public string ClimbResource { get; set; }
            public List<string> Animation { get; set; }
            public string AnimationPrefix { get; set; }
            public List<string> BackAnimation { get; set; }
            public string BackAnimationPrefix { get; set; }
            public bool Bounce { get; set; }
            public bool Adaptive { get; set; }
            public bool Behind { get; set; }
        }

        private static List<CustomHat> CreateCustomHatDetails(string[] hats, bool fromDisk = false)
        {
            Dictionary<string, CustomHat> fronts = new();
            Dictionary<string, string> backs = new();
            Dictionary<string, string> flips = new();
            Dictionary<string, string> backflips = new();
            Dictionary<string, string> climbs = new();

            for (int i = 0; i < hats.Length; i++)
            {
                string s = fromDisk ? hats[i][(hats[i].LastIndexOf("\\") + 1)..].Split('.')[0] : hats[i].Split('.')[3];
                string[] p = s.Split('_');

                List<string> options = new();
                for (int j = 1; j < p.Length; j++)
                    options.Add(p[j]);

                if (options.Contains("back") && options.Contains("flip"))
                    backflips.Add(p[0], hats[i]);
                else if (options.Contains("climb"))
                    climbs.Add(p[0], hats[i]);
                else if (options.Contains("back"))
                    backs.Add(p[0], hats[i]);
                else if (options.Contains("flip"))
                    flips.Add(p[0], hats[i]);
                else
                    fronts.Add(p[0], new()
                    {
                        Resource = hats[i],
                        Name = p[0].Replace('-', ' '),
                        Bounce = options.Contains("bounce"),
                        Adaptive = options.Contains("adaptive"),
                        Behind = options.Contains("behind")
                    });
            }

            List<CustomHat> customhats = new();

            foreach (string key in fronts.Keys)
            {
                CustomHat hat = fronts[key];
                if (backs.TryGetValue(key, out string backResource))
                    hat.BackResource = backResource;
                if (climbs.TryGetValue(key, out string climbResource))
                    hat.ClimbResource = climbResource;
                if (flips.TryGetValue(key, out string flipResource))
                    hat.FlipResource = flipResource;
                if (backflips.TryGetValue(key, out string backflipResource))
                    hat.BackflipResource = backflipResource;

                if (hat.BackResource != null)
                    hat.Behind = true;

                customhats.Add(hat);
            }

            return customhats;
        }

        private static Sprite CreateHatSprite(string path, bool fromDisk = false)
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

        private static HatData CreateHatBehaviour(CustomHat ch, bool fromDisk = false, bool testOnly = false)
        {
            // TODO: remove this variable
            if (HatShader == null)
            {
                Material hatShader = HatManager.Instance.PlayerMaterial;
                HatShader = hatShader;
            }

            HatViewData viewData = ScriptableObject.CreateInstance<HatViewData>();
            viewData.MainImage = CreateHatSprite(ch.Resource, fromDisk);
            viewData.FloorImage = viewData.MainImage;
            if (ch.BackResource != null)
            {
                viewData.BackImage = CreateHatSprite(ch.BackResource, fromDisk);
                ch.Behind = true; // Required to view backresource
            }
            if (ch.ClimbResource != null)
            {
                viewData.ClimbImage = CreateHatSprite(ch.ClimbResource, fromDisk);
                viewData.LeftClimbImage = viewData.ClimbImage;
            }

            HatData hat = ScriptableObject.CreateInstance<HatData>();
            hat.name = ch.Name;
            hat.displayOrder = 99;
            hat.ProductId = "hat_" + ch.Name.Replace(' ', '_');
            hat.InFront = !ch.Behind;
            hat.NoBounce = !ch.Bounce;
            hat.ChipOffset = new Vector2(0f, 0.2f);
            hat.Free = true;

            if (ch.Adaptive)
                viewData.MatchPlayerColor = true;


            HatExtension extend = new()
            {
                Author = ch.Author ?? "Unknown",
                Package = ch.Package ?? "Misc.",
                Condition = ch.Condition ?? "none"
            };

            if (ch.FlipResource != null)
                extend.FlipImage = CreateHatSprite(ch.FlipResource, fromDisk);
            if (ch.BackflipResource != null)
                extend.BackFlipImage = CreateHatSprite(ch.BackflipResource, fromDisk);
            if (ch.Animation != null)
            {
                string filePath = Path.GetDirectoryName(Application.dataPath) + @"\StellarHats\AnimatedHats\" + ch.Name + @"\";

                extend.Animation = new List<Sprite>();
                foreach (string frame in ch.Animation)
                    extend.Animation.Add(CreateHatSprite(filePath + frame, fromDisk));
                extend.Time = 0;
                extend.Frame = 0;
            }
            if (ch.BackAnimation != null)
            {
                string filePath = Path.GetDirectoryName(Application.dataPath) + @"\StellarHats\AnimatedHats\" + ch.Name + @"\";
                extend.BackAnimation = new List<Sprite>();
                foreach (string frame in ch.BackAnimation)
                    extend.BackAnimation.Add(CreateHatSprite(filePath + frame, fromDisk));
                extend.Time = 0;
                extend.Frame = 0;
            }
            if (testOnly)
            {
                TestExt = extend;
                TestExt.Condition = hat.name;
            }
            else
            {
                CustomHatRegistry.Add(hat.name, extend);
            }
            CustomHatViewDatas.TryAdd(hat.name, viewData);
            AssetReference assetRef = new(viewData.Pointer);

            hat.ViewDataRef = assetRef;
            hat.CreateAddressableAsset();

            return hat;
        }

        private static HatData CreateHatBehaviour(CustomHatLoader.CustomHatOnline chd)
        {
            string filePath = Path.GetDirectoryName(Application.dataPath) + @"\StellarHats\";
            chd.Resource = filePath + chd.Resource;
            if (chd.BackResource != null)
                chd.BackResource = filePath + chd.BackResource;
            if (chd.ClimbResource != null)
                chd.ClimbResource = filePath + chd.ClimbResource;
            if (chd.FlipResource != null)
                chd.FlipResource = filePath + chd.FlipResource;
            if (chd.BackflipResource != null)
                chd.BackflipResource = filePath + chd.BackflipResource;

            return CreateHatBehaviour(chd, true);
        }

        static HatViewData GetHatViewData(string id)
        {
            return CustomHatViewDatas.TryGetValue(id, out HatViewData data) ? data : null;
        }

        [HarmonyPatch(typeof(HatManager), nameof(HatManager.GetHatById))]
        public static class HatManagerPatch
        {
            private static List<HatData> AllHatsList;
            private static List<HatData> VanillaHatsList;
            static void Prefix(HatManager __instance)
            {
                if (IsRunning || Loaded) return;
                LoadHats(__instance, true);
            }
            static void Postfix()
            {
                IsRunning = false;
            }

            public static void LoadHats(HatManager __instance, bool GameStart)
            {
                IsRunning = true; // prevent simultanious execution
                AllHatsList = __instance.allHats.ToList();
                if (GameStart)
                {
                    VanillaHatsList = AllHatsList;
                }
                else
                {
                    AllHatsList = VanillaHatsList;
                    Helpers.Log("ResetHats");
                }

                try
                {
                    while (CustomHatLoader.HatDetails.Count > 0)
                    {
                        AllHatsList.Add(CreateHatBehaviour(CustomHatLoader.HatDetails[0]));
                        CustomHatLoader.HatDetails.RemoveAt(0);
                    }
                    Helpers.Log("LoadedNewHats");
                    __instance.allHats = AllHatsList.ToArray();
                    Loaded = true;
                }
                catch (Exception e)
                {
                    if (!Loaded)
                        System.Console.WriteLine("Unable to add Custom Hats\n" + e);
                }

                if (!GameStart)
                {
                    IsRunning = false;
                }
            }
        }

        [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.FixedUpdate))]
        private static class PlayerPhysicsHandleAnimationPatch
        {
            public static float Delay = 10;
            public static float CurrentTime => Time.deltaTime * 150;
            private static void Postfix(PlayerPhysics __instance)
            {
                try
                {
                    AnimationClip currentAnimation = __instance.Animations.Animator.GetCurrentAnimation();
                    PlayerControl player = __instance.myPlayer;
                    if (currentAnimation == __instance.Animations.group.ClimbUpAnim || currentAnimation == __instance.Animations.group.ClimbDownAnim) return;

                    if (player.cosmetics.hat.Hat != null)
                    {
                        string hatId = player.cosmetics.hat.Hat.name;
                        HatViewData viewData = GetHatViewData(hatId);
                        SetHat(__instance, viewData);
                    }

                    if (player.AmOwner)
                    {
                        CustomHatRegistry.Values.ToList().ForEach(extend =>
                        {
                            extend.Time += CurrentTime;
                            if (extend.Time >= Delay)
                            {
                                UpdateHatFrames(extend);
                                extend.Time = 0;
                            }
                        });
                    }
                }
                catch { }
            }

            public static void SetHat(PlayerPhysics __instance, HatViewData viewData)
            {
                HatParent hp = __instance.myPlayer.cosmetics.hat;
                if (hp?.Hat == null) return;
                HatExtension extend = hp.Hat.GetHatExtension();
                if (extend == null) return;

                if (extend.Animation?.Count > 0 || extend.BackAnimation?.Count > 0)
                {
                    if (extend.Animation?.Count > 0)
                        hp.FrontLayer.sprite = extend.Animation[extend.Frame];
                    if (extend.BackAnimation?.Count > 0)
                        hp.BackLayer.sprite = extend.BackAnimation[extend.Frame];
                }
                else
                {
                    hp.FrontLayer.sprite = extend.FlipImage != null && __instance.FlipX ? extend.FlipImage : viewData.MainImage;
                    hp.BackLayer.sprite = extend.BackFlipImage != null && __instance.FlipX ? extend.BackFlipImage : viewData.BackImage;
                }
            }

            public static void UpdateHatFrames(HatExtension extend)
            {
                bool frontAnimation = extend.Animation != null && extend.Animation.Count > 0;
                bool backAnimation = extend.BackAnimation != null && extend.BackAnimation.Count > 0;
                if (!frontAnimation && !backAnimation) return;

                extend.Frame++;

                if ((frontAnimation && extend.Frame >= extend.Animation.Count) ||
                    (backAnimation && extend.Frame >= extend.BackAnimation.Count))
                {
                    extend.Frame = 0;
                }
            }
        }

        [HarmonyPatch]
        private static class FreeplayHatTestingPatches
        {
            [HarmonyPriority(Priority.High)]
            [HarmonyPatch(typeof(HatParent), nameof(HatParent.SetHat), typeof(int))]
            private static class HatParentSetHatPatchColor
            {
                static void Prefix(HatParent __instance)
                {
                    if (TutorialManager.InstanceExists)
                    {
                        try
                        {
                            string filePath = Path.GetDirectoryName(Application.dataPath) + @"\StellarHats\Test";
                            if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
                            DirectoryInfo d = new(filePath);
                            string[] filePaths = d.GetFiles("*.png").Select(x => x.FullName).ToArray(); // Getting Text files
                            List<CustomHat> hats = CreateCustomHatDetails(filePaths, true);
                            if (hats.Count > 0)
                                __instance.Hat = CreateHatBehaviour(hats[0], true, true);
                        }
                        catch (Exception ex)
                        {
                            Helpers.Log(LogLevel.Error, "Unable to create test hat: " + ex.StackTrace);
                        }
                    }
                }
            }

            [HarmonyPatch(typeof(HatParent), nameof(HatParent.SetHat), typeof(HatData), typeof(int))]
            private static class HatParentSetHatPatchExtra
            {
                static bool Prefix(HatParent __instance, HatData hat, int color)
                {
                    if (!TutorialManager.InstanceExists) return true;

                    try
                    {
                        __instance.Hat = hat;
                        __instance.viewAsset = __instance.Hat.CreateAddressableAsset();

                        string filePath = Path.GetDirectoryName(Application.dataPath) + @"\StellarHats\Test";
                        if (!Directory.Exists(filePath)) return true;
                        DirectoryInfo d = new(filePath);
                        string[] filePaths = d.GetFiles("*.png").Select(x => x.FullName).ToArray(); // Getting Test files
                        List<CustomHat> hats = CreateCustomHatDetails(filePaths, true);
                        if (hats.Count > 0)
                        {
                            __instance.Hat = CreateHatBehaviour(hats[0], true, true);
                            __instance.Hat.CreateAddressableAsset();
                        }
                    }
                    catch (Exception ex)
                    {
                        Helpers.Log(LogLevel.Error, "Unable to create test hat: " + ex.StackTrace);
                        return true;
                    }


                    __instance.PopulateFromViewData();
                    __instance.SetMaterialColor(color);
                    return false;
                }
            }
        }

        [HarmonyPatch(typeof(HatParent), nameof(HatParent.SetHat), typeof(int))]
        public class SetHatPatch
        {
            public static bool Prefix(HatParent __instance, int color)
            {
                if (!CustomHatViewDatas.ContainsKey(__instance.Hat.name)) return true;
                __instance.viewAsset = null;
                __instance.PopulateFromViewData();
                __instance.SetMaterialColor(color);
                return false;
            }
        }

        [HarmonyPatch(typeof(HatParent), nameof(HatParent.UpdateMaterial))]
        public class UpdateMaterialPatch
        {
            public static bool Prefix(HatParent __instance)
            {
                HatViewData asset;
                try
                {
                    HatViewData vanillaAsset = __instance.viewAsset.GetAsset();
                    return true;
                }
                catch
                {
                    try
                    {
                        asset = CustomHatViewDatas[__instance.Hat.name];
                    }
                    catch
                    {
                        return false;
                    }
                }
                if (asset.MatchPlayerColor)
                {
                    __instance.FrontLayer.sharedMaterial = HatShader;
                    if (__instance.BackLayer)
                        __instance.BackLayer.sharedMaterial = HatShader;
                }
                else
                {
                    __instance.FrontLayer.sharedMaterial = HatManager.Instance.DefaultShader;
                    if (__instance.BackLayer)
                        __instance.BackLayer.sharedMaterial = HatManager.Instance.DefaultShader;
                }
                int colorId = __instance.matProperties.ColorId;
                PlayerMaterial.SetColors(colorId, __instance.FrontLayer);
                if (__instance.BackLayer)
                {
                    PlayerMaterial.SetColors(colorId, __instance.BackLayer);
                }
                __instance.FrontLayer.material.SetInt(PlayerMaterial.MaskLayer, __instance.matProperties.MaskLayer);
                if (__instance.BackLayer)
                    __instance.BackLayer.material.SetInt(PlayerMaterial.MaskLayer, __instance.matProperties.MaskLayer);
                PlayerMaterial.MaskType maskType = __instance.matProperties.MaskType;
                if (maskType == PlayerMaterial.MaskType.ScrollingUI)
                {
                    if (__instance.FrontLayer)
                        __instance.FrontLayer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                    if (__instance.BackLayer)
                        __instance.BackLayer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                }
                else if (maskType == PlayerMaterial.MaskType.Exile)
                {
                    if (__instance.FrontLayer)
                        __instance.FrontLayer.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
                    if (__instance.BackLayer)
                        __instance.BackLayer.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
                }
                else
                {
                    if (__instance.FrontLayer)
                        __instance.FrontLayer.maskInteraction = SpriteMaskInteraction.None;
                    if (__instance.BackLayer)
                        __instance.BackLayer.maskInteraction = SpriteMaskInteraction.None;
                }
                if (__instance.matProperties.MaskLayer <= 0)
                {
                    PlayerMaterial.SetMaskLayerBasedOnLocalPlayer(__instance.FrontLayer, __instance.matProperties.IsLocalPlayer);
                    if (__instance.BackLayer)
                    {
                        PlayerMaterial.SetMaskLayerBasedOnLocalPlayer(__instance.BackLayer, __instance.matProperties.IsLocalPlayer);
                    }
                }
                return false;
            }
        }

        [HarmonyPatch(typeof(HatParent), nameof(HatParent.SetFloorAnim))]
        public class HatParentSetFloorAnimPatch
        {
            public static bool Prefix(HatParent __instance)
            {
                try
                {
                    HatViewData vanillaAsset = __instance.viewAsset.GetAsset();
                    return true;
                }
                catch { }
                HatViewData hatViewData = CustomHatViewDatas[__instance.Hat.name];
                __instance.BackLayer.enabled = false;
                __instance.FrontLayer.enabled = true;
                __instance.FrontLayer.sprite = hatViewData.FloorImage;
                return false;
            }
        }

        [HarmonyPatch(typeof(HatParent), nameof(HatParent.SetIdleAnim))]
        public class HatParentSetIdleAnimPatch
        {
            public static bool Prefix(HatParent __instance, int colorId)
            {
                if (!__instance.Hat) return false;
                if (!CustomHatViewDatas.ContainsKey(__instance.Hat.name)) return true;
                __instance.viewAsset = null;
                __instance.PopulateFromViewData();
                __instance.SetMaterialColor(colorId);
                return false;
            }
        }

        [HarmonyPatch(typeof(HatParent), nameof(HatParent.SetClimbAnim))]
        public class HatParentSetClimbAnimPatch
        {
            public static bool Prefix(HatParent __instance)
            {
                if (!CustomHatViewDatas.TryGetValue(__instance.Hat.name, out HatViewData data)) return true;
                if (!__instance.options.ShowForClimb) return false;
                __instance.BackLayer.enabled = false;
                __instance.FrontLayer.enabled = true;
                __instance.FrontLayer.sprite = data.ClimbImage;
                return false;
            }
        }


        [HarmonyPatch(typeof(HatParent), nameof(HatParent.PopulateFromViewData))]
        public class PopulateFromHatViewDataPatch
        {
            public static bool Prefix(HatParent __instance)
            {
                try
                {
                    HatViewData vanillaAsset = __instance.viewAsset.GetAsset();
                    return true;
                }
                catch
                {
                    if (__instance.Hat && !CustomHatViewDatas.ContainsKey(__instance.Hat.name))
                        return true;
                }


                HatViewData asset = CustomHatViewDatas[__instance.Hat.name];

                if (!asset)
                    return true;

                __instance.UpdateMaterial();

                SpriteAnimNodeSync spriteAnimNodeSync = __instance.SpriteSyncNode ?? __instance.GetComponent<SpriteAnimNodeSync>();
                if (spriteAnimNodeSync)
                    spriteAnimNodeSync.NodeId = (__instance.Hat.NoBounce ? 1 : 0);
                if (__instance.Hat.InFront)
                {
                    __instance.BackLayer.enabled = false;
                    __instance.FrontLayer.enabled = true;
                    __instance.FrontLayer.sprite = asset.MainImage;
                }
                else if (asset.BackImage)
                {
                    __instance.BackLayer.enabled = true;
                    __instance.FrontLayer.enabled = true;
                    __instance.BackLayer.sprite = asset.BackImage;
                    __instance.FrontLayer.sprite = asset.MainImage;
                }
                else
                {
                    __instance.BackLayer.enabled = true;
                    __instance.FrontLayer.enabled = false;
                    __instance.FrontLayer.sprite = null;
                    __instance.BackLayer.sprite = asset.MainImage;
                }
                if (__instance.options.Initialized && __instance.HideHat())
                {
                    __instance.FrontLayer.enabled = false;
                    __instance.BackLayer.enabled = false;
                }
                return false;
            }
        }

        [HarmonyPatch(typeof(CosmeticsCache), nameof(CosmeticsCache.GetHat))]
        public class CosmeticsCacheGetHatPatch
        {
            public static bool Prefix(string id, ref HatViewData __result)
            {
                Helpers.Log($"trying to load hat {id} from cosmetics cache");
                if (CustomHatViewDatas.ContainsKey(id))
                {
                    __result = CustomHatViewDatas[id];
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(HatsTab), nameof(HatsTab.OnEnable))]
        public class HatsTabOnEnablePatch
        {
            public const string InnerslothPackageName = "Innersloth Hats";
            private static TMP_Text textTemplate;

            public static float CreateHatPackage(List<(HatData, HatExtension)> hats, string packageName, float YStart, HatsTab __instance)
            {
                bool isDefaultPackage = InnerslothPackageName == packageName;
                if (!isDefaultPackage)
                    hats = hats.OrderBy(x => x.Item1.name).ToList();
                float offset = YStart;

                if (textTemplate != null)
                {
                    TMP_Text titleText = UnityEngine.Object.Instantiate(textTemplate, __instance.scroller.Inner);
                    titleText.transform.localPosition = new Vector3(2.25f, YStart, -1f);
                    titleText.transform.localScale = Vector3.one * 1.5f;
                    titleText.fontSize *= 0.5f;
                    titleText.enableAutoSizing = false;
                    __instance.StartCoroutine(Effects.Lerp(0.1f, new Action<float>((p) => { titleText.SetText(packageName); })));
                    offset -= 0.8f * __instance.YOffset;
                }
                for (int i = 0; i < hats.Count; i++)
                {
                    (HatData hat, HatExtension ext) = hats[i];

                    float xPos = __instance.XRange.Lerp(i % __instance.NumPerRow / (__instance.NumPerRow - 1f));
                    float yPos = offset - i / __instance.NumPerRow * (isDefaultPackage ? 1f : 1.5f) * __instance.YOffset;
                    ColorChip colorChip = UnityEngine.Object.Instantiate(__instance.ColorTabPrefab, __instance.scroller.Inner);
                    if (ActiveInputManager.currentControlType == ActiveInputManager.InputType.Keyboard)
                    {
                        colorChip.Button.OnMouseOver.AddListener((Action)(() => __instance.SelectHat(hat)));
                        colorChip.Button.OnMouseOut.AddListener((Action)(() => __instance.SelectHat(HatManager.Instance.GetHatById(DataManager.Player.Customization.Hat))));
                        colorChip.Button.OnClick.AddListener((Action)(() => __instance.ClickEquip()));
                    }
                    else
                        colorChip.Button.OnClick.AddListener((Action)(() => __instance.SelectHat(hat)));
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
                            foreground.localPosition = Vector3.down * 0.243f;

                        if (textTemplate != null)
                        {
                            TMP_Text description = UnityEngine.Object.Instantiate(textTemplate, colorChip.transform);
                            description.transform.localPosition = new Vector3(0f, -0.65f, -1f);
                            description.alignment = TextAlignmentOptions.Center;
                            description.transform.localScale = Vector3.one * 0.65f;
                            __instance.StartCoroutine(Effects.Lerp(0.1f, new Action<float>((p) => { description.SetText($"{hat.name}\nby {ext.Author}"); })));
                        }
                    }

                    colorChip.transform.localPosition = new Vector3(xPos, yPos, -1f);
                    colorChip.Inner.SetMaskType(PlayerMaterial.MaskType.SimpleUI);
                    colorChip.Inner.SetHat(hat, __instance.HasLocalPlayer() ? PlayerControl.LocalPlayer.Data.DefaultOutfit.ColorId : DataManager.Player.Customization.Color);
                    colorChip.Inner.transform.localPosition = new Vector3(hat.ChipOffset.x, hat.ChipOffset.y, -10);
                    colorChip.Tag = hat;
                    colorChip.SelectionHighlight.gameObject.SetActive(false);
                    __instance.ColorChips.Add(colorChip);
                }
                return offset - (hats.Count - 1) / __instance.NumPerRow * (isDefaultPackage ? 1f : 1.5f) * __instance.YOffset - 1.75f;
            }

            public static bool Prefix(HatsTab __instance)
            {
                for (int i = 0; i < __instance.scroller.Inner.childCount; i++)
                    UnityEngine.Object.Destroy(__instance.scroller.Inner.GetChild(i).gameObject);
                __instance.ColorChips = new Il2CppSystem.Collections.Generic.List<ColorChip>();

                HatData[] unlockedHats = HatManager.Instance.GetUnlockedHats();
                Dictionary<string, List<(HatData, HatExtension)>> packages = new();

                foreach (HatData hatBehaviour in unlockedHats)
                {
                    HatExtension ext = hatBehaviour.GetHatExtension();

                    string packageName = ext == null ? InnerslothPackageName : ext.Package;

                    if (!packages.TryGetValue(packageName, out List<(HatData, HatExtension)> package))
                        packages[packageName] = package = new List<(HatData, HatExtension)>();

                    package.Add((hatBehaviour, ext));
                }

                float YOffset = __instance.YStart;
                textTemplate = GameObject.Find("HatsGroup").transform.FindChild("Text").GetComponent<TMP_Text>();

                IOrderedEnumerable<string> orderedKeys = packages.Keys.OrderBy((x) => x switch
                {
                    "Developer Hats" => 1,
                    "Animated Hats" => 2,
                    "StellarHats" => 3,
                    InnerslothPackageName => 4,
                    _ => 5,
                });
                foreach (string key in orderedKeys)
                    YOffset = CreateHatPackage(packages[key], key, YOffset, __instance);

                __instance.scroller.ContentYBounds.max = -(YOffset + 4.1f);
                return false;
            }
        }

    }

    public class CustomHatLoader
    {

        public static List<CustomHatOnline> HatDetails = new();
        public class CustomHatOnline : CustomHats.CustomHat
        {
            public string ResHashA { get; set; }
            public string ResHashB { get; set; }
            public string ResHashC { get; set; }
            public string ResHashF { get; set; }
            public string ResHashBF { get; set; }
        }
    }
    public static class CustomHatExtensions
    {
        public static CustomHats.HatExtension GetHatExtension(this HatData hat)
        {
            if (CustomHats.TestExt != null && CustomHats.TestExt.Condition.Equals(hat.name))
                return CustomHats.TestExt;
            CustomHats.CustomHatRegistry.TryGetValue(hat.name, out CustomHats.HatExtension ret);
            return ret;
        }
    }
}
