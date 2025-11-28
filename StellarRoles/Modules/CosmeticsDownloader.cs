using AmongUs.Data;
using HarmonyLib;
using Newtonsoft.Json.Linq;
using PowerTools;
using StellarRoles.Modules;
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
using static StellarRoles.Modules.CustomNameplateLoader;
using static StellarRoles.Modules.CustomVisorLoader;
using static StellarRoles.Modules.CustomHatLoader;

namespace StellarRoles
{
    public class CosmeticsDownloader
    {
        public static bool IsRunning = false;
        private const string REPO_SR = "https://raw.githubusercontent.com/Mr-Fluuff/StellarHats/main";
        public static int TotalHatsDownloaded = 0;
        public static int TotalHatsToDownload = 0;
        public static int TotalNameplatesDownloaded = 0;
        public static int TotalNameplatesToDownload = 0;
        public static int TotalVisorsDownloaded = 0;
        public static int TotalVisorsToDownload = 0;

        public static void LaunchCosmeticsFetcher()
        {
            if (IsRunning)
                return;
            IsRunning = true;
            Helpers.Log("LauchCosmeticsFetcher");
            TotalHatsDownloaded = 0;
            _ = LaunchCosmeticsFetcherAsync();
        }
        private static async Task LaunchCosmeticsFetcherAsync()
        {
            try
            {
                HttpStatusCode hatstatus = await FetchHats_SR();
                if (hatstatus != HttpStatusCode.OK)
                {
                    StellarRolesPlugin.Logger.LogMessage("Custom SH Hats could not be loaded");
                }
                HttpStatusCode visorstatus = await FetchVisors_SR();
                if (hatstatus != HttpStatusCode.OK)
                {
                    StellarRolesPlugin.Logger.LogMessage("Custom SH Visors could not be loaded");
                }
                HttpStatusCode nameplatestatus = await FetchNamePlates_SR();
                if (hatstatus != HttpStatusCode.OK)
                {
                    StellarRolesPlugin.Logger.LogMessage("Custom SH Nameplates could not be loaded");
                }

            }
            catch (Exception exception)
            {
                StellarRolesPlugin.Logger.LogMessage("Unable to fetch Cosmetics");
                StellarRolesPlugin.Logger.LogError(exception);
            }
            IsRunning = false;
        }
        public static async Task<HttpStatusCode> FetchVisors_SR()
        {
            HttpClient http = new();
            http.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };
            HttpResponseMessage response = await http.GetAsync(new Uri($"{REPO_SR}/CustomVisors.json"), HttpCompletionOption.ResponseContentRead);
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

                List<string> markedForDownload = new();
                List<(string, string)> markedForDownload2 = new();

                string filePath = Path.GetDirectoryName(Application.dataPath) + @"\StellarVisors\";
                string animatedFilePath = Path.GetDirectoryName(Application.dataPath) + @"\StellarVisors\AnimatedVisors\";
                if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
                if (!Directory.Exists(animatedFilePath)) Directory.CreateDirectory(animatedFilePath);
                MD5 md5 = MD5.Create();

                foreach (CustomVisorOnline data in visordatas)
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
                        if (files > data.Animation.Count)
                        {
                            foreach (var item in Directory.GetFiles(newpath))
                            {
                                File.Delete(item);
                            }
                        }
                        if (DoesResourceRequireDownload(filePath + data.Resource, data.ResHashA, md5) || files != data.Animation.Count)
                            foreach (string frame in data.Animation)
                                markedForDownload2.Add((data.Name, frame));
                    }
                }

                TotalVisorsToDownload = markedForDownload.Count + markedForDownload2.Count;

                if (markedForDownload.Count <= 0 && markedForDownload2.Count <= 0)
                {
                    VisorDetails = visordatas;
                }
                else
                {
                    for (int i = 0; i < markedForDownload.Count; i++)
                    {
                        string file = markedForDownload[i];
                        Helpers.Log(file + " Downloaded");
                        TotalVisorsDownloaded++;

                        HttpResponseMessage visorFileResponse = await http.GetAsync($"{REPO_SR}/visors/{file}", HttpCompletionOption.ResponseContentRead);
                        if (visorFileResponse.StatusCode != HttpStatusCode.OK) continue;
                        using Stream responseStream = await visorFileResponse.Content.ReadAsStreamAsync();
                        using FileStream fileStream = File.Create($"{filePath}\\{file}");
                        responseStream.CopyTo(fileStream);
                    }

                    for (int i = 0; i < markedForDownload2.Count; i++)
                    {
                        var file = markedForDownload2[i];
                        Helpers.Log(file + " Downloaded");
                        TotalVisorsDownloaded++;
                        string name = file.Item1;
                        string frame = file.Item2;

                        string newPath = animatedFilePath + name + @"\";
                        HttpResponseMessage visorFileResponse = await http.GetAsync($"{REPO_SR}/visors/{name}/{frame}", HttpCompletionOption.ResponseContentRead);
                        if (visorFileResponse.StatusCode != HttpStatusCode.OK) continue;
                        using Stream responseStream = await visorFileResponse.Content.ReadAsStreamAsync();
                        using FileStream fileStream = File.Create($"{newPath}\\{frame}");
                        responseStream.CopyTo(fileStream);
                    }
                    VisorDetails = visordatas;
                }
            }
            catch (Exception ex)
            {
                Helpers.Log(LogLevel.Error, "Error fetching visor metadata: " + ex.ToString());
            }
            return HttpStatusCode.OK;
        }

        public static async Task<HttpStatusCode> FetchNamePlates_SR()
        {
            HttpClient http = new();
            http.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };
            HttpResponseMessage response = await http.GetAsync(new Uri($"{REPO_SR}/CustomPlates.json"), HttpCompletionOption.ResponseContentRead);
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

                        HttpResponseMessage NameplateFileResponse = await http.GetAsync($"{REPO_SR}/plates/{file}", HttpCompletionOption.ResponseContentRead);
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
        public static async Task<HttpStatusCode> FetchHats_SR()
        {
            HttpClient http = new();
            http.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };
            HttpResponseMessage response = await http.GetAsync(new Uri($"{REPO_SR}/CustomHats.json"), HttpCompletionOption.ResponseContentRead);
            try
            {
                if (response.StatusCode != HttpStatusCode.OK) return response.StatusCode;
                if (response.Content == null)
                {
                    StellarRolesPlugin.Logger.LogMessage("Server returned no data: " + response.StatusCode.ToString());
                    return HttpStatusCode.ExpectationFailed;
                }
                string json = await response.Content.ReadAsStringAsync();
                JToken jobj = JObject.Parse(json)["hats"];
                if (!jobj.HasValues) return HttpStatusCode.ExpectationFailed;

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
                            ResHashA = current["reshasha"]?.ToString(),
                            BackResource = SanitizeResourcePath(current["backresource"]?.ToString()),
                            ResHashB = current["reshashb"]?.ToString(),
                            ClimbResource = SanitizeResourcePath(current["climbresource"]?.ToString()),
                            ResHashC = current["reshashc"]?.ToString(),
                            FlipResource = SanitizeResourcePath(current["flipresource"]?.ToString()),
                            ResHashF = current["reshashf"]?.ToString(),
                            BackflipResource = SanitizeResourcePath(current["backflipresource"]?.ToString()),
                            ResHashBF = current["reshashbf"]?.ToString(),

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
                            List<string> animationList = info.Animation = new();
                            for (int i = 0; i < frames; i++)
                                animationList.Add($"{info.AnimationPrefix}_{i:000}.png");
                        }

                        if (info.BackAnimationPrefix != null && frames > 0)
                        {
                            List<string> backanimationList = info.BackAnimation = new();
                            for (int i = 0; i < frames; i++)
                                backanimationList.Add($"{info.BackAnimationPrefix}_{i:000}.png");
                        }

                        hatdatas.Add(info);
                    }
                }

                List<string> markedForDownload = new();
                List<(string, string)> markedForDownload2 = new();

                string filePath = Path.GetDirectoryName(Application.dataPath) + @"\StellarHats\";
                string animatedFilePath = Path.GetDirectoryName(Application.dataPath) + @"\StellarHats\AnimatedHats\";
                if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
                if (!Directory.Exists(animatedFilePath)) Directory.CreateDirectory(animatedFilePath);
                MD5 md5 = MD5.Create();

                foreach (CustomHatOnline data in hatdatas)
                {
                    if (DoesResourceRequireDownload(filePath + data.Resource, data.ResHashA, md5))
                        markedForDownload.Add(data.Resource);
                    if (data.BackResource != null && DoesResourceRequireDownload(filePath + data.BackResource, data.ResHashB, md5))
                        markedForDownload.Add(data.BackResource);
                    if (data.ClimbResource != null && DoesResourceRequireDownload(filePath + data.ClimbResource, data.ResHashC, md5))
                        markedForDownload.Add(data.ClimbResource);
                    if (data.FlipResource != null && DoesResourceRequireDownload(filePath + data.FlipResource, data.ResHashF, md5))
                        markedForDownload.Add(data.FlipResource);
                    if (data.BackflipResource != null && DoesResourceRequireDownload(filePath + data.BackflipResource, data.ResHashBF, md5))
                        markedForDownload.Add(data.BackflipResource);
                    if (data.Animation.Count > 0 || data.BackAnimation.Count > 0)
                    {
                        string newPath = animatedFilePath + data.Name + @"\";
                        if (!Directory.Exists(newPath)) Directory.CreateDirectory(newPath);

                        if (Directory.GetFiles(newPath).Length > (data.Animation.Count + data.BackAnimation.Count))
                        {
                            foreach (var item in Directory.GetFiles(newPath))
                            {
                                File.Delete(item);
                            }
                        }
                        if (data.Animation.Count > 0)
                        {
                            var newFileCount = Directory.GetFiles(newPath).Count(x => x.StartsWith($"{newPath}{data.AnimationPrefix}"));
                            if (DoesResourceRequireDownload(filePath + data.Resource, data.ResHashA, md5) || newFileCount != data.Animation.Count)
                                foreach (string frame in data.Animation)
                                    markedForDownload2.Add((data.Name, frame));
                        }

                        if (data.BackAnimation.Count > 0)
                        {
                            var newFileCount = Directory.GetFiles(newPath).Count(x => x.StartsWith($"{newPath}{data.BackAnimationPrefix}"));
                            if (DoesResourceRequireDownload(filePath + data.BackResource, data.ResHashB, md5) || newFileCount != data.BackAnimation.Count)
                                foreach (string frame in data.BackAnimation)
                                    markedForDownload2.Add((data.Name, frame));
                        }
                    }
                }

                TotalHatsToDownload = markedForDownload.Count + markedForDownload2.Count;

                for (int i = 0; i < markedForDownload.Count; i++)
                {
                    var file = markedForDownload[i];
                    Helpers.Log(file + " Downloaded");
                    TotalHatsDownloaded++;
                    HttpResponseMessage hatFileResponse = await http.GetAsync($"{REPO_SR}/hats/{file}", HttpCompletionOption.ResponseContentRead);
                    if (hatFileResponse.StatusCode != HttpStatusCode.OK) continue;
                    using Stream responseStream = await hatFileResponse.Content.ReadAsStreamAsync();
                    using FileStream fileStream = File.Create($"{filePath}\\{file}");
                    responseStream.CopyTo(fileStream);
                }

                for (int i = 0; i < markedForDownload2.Count; i++)
                {
                    var file = markedForDownload2[i];
                    Helpers.Log(file + " Downloaded");
                    TotalHatsDownloaded++;
                    string name = file.Item1;
                    string frame = file.Item2;

                    string animatedPath = animatedFilePath + name + @"\";
                    HttpResponseMessage hatFileResponse = await http.GetAsync($"{REPO_SR}/hats/{name}/{frame}", HttpCompletionOption.ResponseContentRead);
                    if (hatFileResponse.StatusCode != HttpStatusCode.OK) continue;
                    using Stream responseStream = await hatFileResponse.Content.ReadAsStreamAsync();
                    using FileStream fileStream = File.Create($"{animatedPath}\\{frame}");
                    responseStream.CopyTo(fileStream);
                }
                CustomHatLoader.HatDetails = hatdatas;
            }
            catch (Exception ex)
            {
                StellarRolesPlugin.Instance.Log.LogError(ex);
            }
            return HttpStatusCode.OK;
        }

        private static bool DoesResourceRequireDownload(string respath, string reshash, MD5 md5)
        {
            if (reshash == null || !File.Exists(respath))
                return true;

            return !reshash.Equals(BitConverter.ToString(md5.ComputeHash(File.OpenRead(respath))).Replace("-", "").ToLowerInvariant());
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
