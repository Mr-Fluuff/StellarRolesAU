using Newtonsoft.Json.Linq;
using Reactor.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using static StellarRoles.Modules.Cosmetics.CustomHatLoader;
using static StellarRoles.Modules.Cosmetics.CustomNameplateLoader;
using static StellarRoles.Modules.Cosmetics.CustomVisorLoader;

namespace StellarRoles.Modules.Cosmetics
{
    public class CosmeticsFetcher
    {
        public static bool IsRunning = false;

        public static void LaunchCosmeticsFetcher()
        {
            if (IsRunning)
                return;
            IsRunning = true;
            Helpers.Log("LauchCosmeticsFetcher");
            LaunchCosmeticsFetcherAsync();
        }
        private static void LaunchCosmeticsFetcherAsync()
        {

            Coroutines.Start(FetchHats_SR());
            Coroutines.Start(FetchVisors_SR());
            Coroutines.Start(FetchNamePlates_SR());

            IsRunning = false;
        }
        public static IEnumerator FetchVisors_SR()
        {
            var jsonAsset = CustomAssets.VisorJsonFile.LoadAsset();
            if (jsonAsset == null)
            {
                Helpers.Log("Missing CustomVisors.json");
                yield break;
            }
            string json = jsonAsset.text;
            try
            {
                JToken jObj = JObject.Parse(json)["visors"];

                List<CustomVisorOnline> visordatas = new();

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
                        visordatas.Add(info);
                    }
                }

                VisorDetails = visordatas;
            }
            catch (Exception ex)
            {
                Helpers.Log(LogLevel.Error, "Error fetching visor metadata: " + ex.ToString());
            }
        }

        public static IEnumerator FetchNamePlates_SR()
        {
            var jsonAsset = CustomAssets.NameplateJsonFile.LoadAsset();
            if (jsonAsset == null)
            {
                Helpers.Log("Missing CustomPlates.json");
                yield break;
            }
            string json = jsonAsset.text;

            try
            {

                JToken jObj = JObject.Parse(json)["plates"];

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

                NameplateDetails = nameplatedatas;
            }
            catch (Exception ex)
            {
                Helpers.Log(LogLevel.Error, "Error fetching Nameplate metadata: " + ex.ToString());
            }
        }
        public static IEnumerator FetchHats_SR()
        {
            var jsonAsset = CustomAssets.HatsJsonFile.LoadAsset();
            if (jsonAsset == null)
            {
                Helpers.Log("Missing CustomHats.json");
                yield break;
            }
            string json = jsonAsset.text;

            try
            {
                JToken jobj = JObject.Parse(json)["hats"];

                List<CustomHatOnline> hatdatas = new();

                for (JToken current = jobj.First; current != null; current = current.Next)
                {
                    if (current.HasValues)
                    {
                        string name = current["name"]?.ToString();
                        string resource = SanitizeResourcePath(current["resource"]?.ToString());
                        if (resource == null || name == null) // required
                            continue;
                        CustomHatOnline info = new()
                        {
                            Name = name,
                            Resource = resource,
                            BackResource = SanitizeResourcePath(current["backresource"]?.ToString()),
                            ClimbResource = SanitizeResourcePath(current["climbresource"]?.ToString()),
                            FlipResource = SanitizeResourcePath(current["flipresource"]?.ToString()),
                            BackflipResource = SanitizeResourcePath(current["backflipresource"]?.ToString()),

                            Author = current["author"]?.ToString(),
                            Package = current["package"]?.ToString(),
                            Condition = current["condition"]?.ToString(),
                            Bounce = current["bounce"] != null,
                            Adaptive = current["adaptive"] != null,
                            Behind = current["behind"] != null,

                            Animation = new List<string>(),
                            BackAnimation = new List<string>(),

                            AnimationPrefix = current["animationprefix"]?.ToString(),
                            BackAnimationPrefix = current["backanimationprefix"]?.ToString()
                        };

                        int frames = current["animationframes"] == null ? 0 : int.Parse(current["animationframes"].ToString());

                        if (info.AnimationPrefix != null && frames > 0)
                        {
                            List<string> animationList = new();
                            for (int i = 0; i < frames; i++)
                                animationList.Add($"{info.AnimationPrefix}_{i:000}.png");

                            info.Animation = animationList;
                        }

                        if (info.BackAnimationPrefix != null && frames > 0)
                        {
                            List<string> backanimationList = new();
                            for (int i = 0; i < frames; i++)
                                backanimationList.Add($"{info.BackAnimationPrefix}_{i:000}.png");

                            info.BackAnimation = backanimationList;
                        }

                        hatdatas.Add(info);
                    }
                }
                HatDetails = hatdatas;
            }
            catch (Exception ex)
            {
                StellarRolesPlugin.Instance.Log.LogError(ex);
            }
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
    }
}
