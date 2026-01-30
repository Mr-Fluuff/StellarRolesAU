using BepInEx;
using HarmonyLib;
using Reactor.Utilities;
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace StellarRoles.Modules.Cosmetics
{
    internal class CosmeticsBundleUpdater
    {
        [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
        class StartupPatch
        {
            static void Postfix()
            {
                string bundleUrl;
                string hashUrl;
                string bundlePath;

                if (StellarRoles.IsAndroid())
                {
                    bundleUrl = "https://raw.githubusercontent.com/Mr-Fluuff/StellarHats/main/stellar-cosmetics-android.bundle";
                    hashUrl = "https://raw.githubusercontent.com/Mr-Fluuff/StellarHats/main/stellar-cosmetics-android.hash";

                    bundlePath = Path.Combine(Application.persistentDataPath, "stellar-cosmetics-android.bundle");
                }
                else
                {
                    bundleUrl = "https://raw.githubusercontent.com/Mr-Fluuff/StellarHats/main/stellar-cosmetics-win.bundle";
                    hashUrl = "https://raw.githubusercontent.com/Mr-Fluuff/StellarHats/main/stellar-cosmetics-win.hash";

                    bundlePath = Path.Combine(Paths.PluginPath, "stellar-cosmetics-win.bundle");
                }

                Coroutines.Start(CheckForBundleUpdate(hashUrl, bundleUrl, bundlePath));
            }
        }


        public static string GetFileHash(string path)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            using var stream = File.OpenRead(path);
            var hash = sha.ComputeHash(stream);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }

        public static IEnumerator CheckForBundleUpdate(string hashUrl, string bundleUrl, string bundlePath)
        {
            var hashReq = UnityWebRequest.Get(hashUrl);
            yield return hashReq.SendWebRequest();

            if (hashReq.result != UnityWebRequest.Result.Success)
            {
                Helpers.Log("Failed to fetch bundle hash.");
                yield break;
            }

            var remoteHash = hashReq.downloadHandler.text.Trim();
            hashReq.Dispose();

            var localHash = File.Exists(bundlePath) ? GetFileHash(bundlePath) : "";

            if (remoteHash != localHash)
            {
                Helpers.Log("Bundle out of date — downloading update...");
                yield return DownloadBundle(bundleUrl, bundlePath);
            }

            CustomAssets.LoadCosmeticsBundle();
            CosmeticsFetcher.LaunchCosmeticsFetcher();
        }

        public static IEnumerator DownloadBundle(string url, string localPath)
        {
            var req = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET);
            req.downloadHandler = new DownloadHandlerBuffer();
            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                Helpers.Log("Failed to download bundle: " + req.error);
                req.Dispose();
                yield break;
            }

            var nativeData = req.downloadHandler.GetNativeData();
            if (nativeData == null || nativeData.Length == 0)
            {
                Helpers.Log("Download returned empty data.");
                req.Dispose();
                yield break;
            }

            // Copy NativeArray<byte> into managed byte[]
            byte[] managedBytes = new byte[nativeData.Length];
            for (int i = 0; i < nativeData.Length; i++)
                managedBytes[i] = nativeData[i];

            File.WriteAllBytes(localPath, managedBytes);

            req.Dispose();
            Helpers.Log("Bundle updated successfully.");
        }
    }
}
