using BepInEx;
using BepInEx.Unity.IL2CPP.Utils;
using HarmonyLib;
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace StellarRoles.Modules;

public class BepInExUpdater : MonoBehaviour
{
    public const string RequiredBepInExVersion = "6.0.0-be.671+9caf61dca07043beae57b0771f6a5283aa02436b";
    public const string BepInExDownloadURL = "https://builds.bepinex.dev/projects/bepinex_be/671/BepInEx-Unity.IL2CPP-win-x86-6.0.0-be.671%2B9caf61d.zip";
    public static bool UpdateRequired => Paths.BepInExVersion.ToString() != RequiredBepInExVersion;

    public void Awake()
    {
        Helpers.Log(LogLevel.Info, $"BepInEx Update Required... ({Paths.BepInExVersion} > {RequiredBepInExVersion})");
        this.StartCoroutine(CoUpdate());
    }

    [HideFromIl2Cpp]
    public IEnumerator CoUpdate()
    {
        Task.Run(() => MessageBox(GetForegroundWindow(), "Required BepInEx update is downloading, please wait...", "StellarRoles", 0));
        UnityWebRequest www = UnityWebRequest.Get(BepInExDownloadURL);
        yield return www.Send();
        if (www.isNetworkError || www.isHttpError)
        {
            Helpers.Log(LogLevel.Error, "Error updating BepInEx: " + www.error);
            yield break;
        }

        string zipPath = Path.Combine(Paths.GameRootPath, ".bepinex_update");
        File.WriteAllBytes(zipPath, www.downloadHandler.data);


        string tempPath = Path.Combine(Path.GetTempPath(), "StellarUpdater.exe");
        Assembly asm = Assembly.GetExecutingAssembly();
        string exeName = asm.GetManifestResourceNames().FirstOrDefault(n => n.EndsWith("StellarUpdater.exe"));

        using (Stream resource = asm.GetManifestResourceStream(exeName))
        {
            using FileStream file = new(tempPath, FileMode.OpenOrCreate, FileAccess.Write);
            resource!.CopyTo(file);
        }

        ProcessStartInfo startInfo = new(tempPath, $"--game-path \"{Paths.GameRootPath}\" --zip \"{zipPath}\"")
        {
            UseShellExecute = false
        };
        Process.Start(startInfo);
        Application.Quit();
    }

    [DllImport("user32.dll")]
    public static extern IntPtr GetForegroundWindow();
    [DllImport("user32.dll")]
    public static extern int MessageBox(IntPtr hWnd, string text, string caption, int options);
    [DllImport("user32.dll")]
    public static extern int MessageBoxTimeout(IntPtr hwnd, string text, string title, uint type, short wLanguageId, int milliseconds);

}

[HarmonyPatch(typeof(SplashManager), nameof(SplashManager.Update))]
public static class StopLoadingMainMenu
{
    public static bool Prefix()
    {
        return !BepInExUpdater.UpdateRequired;
    }
}