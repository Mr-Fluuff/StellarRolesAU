using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace StellarRoles
{
    public static class Sounds
    {
        public const string Douse = "ArsonistDouse";
        public const string Repair = "EngineerRepair";
        public const string Fail = "Fail";
        public const string Clean = "JanitorClean";
        public const string Click = "Click";
        public const string Shield = "MedicShield";
        public const string Morph = "MorphlingMorph";
        public const string Sample = "MorphlingSample";
        public const string TrackCorpses = "ScavengerTrackCorpses";
        public const string TrackPlayer = "TrackerTrackPlayer";
        public const string PlaceTrap = "TrapperPlaceTrap";
        public const string Bite = "VampireBite";
        public const string Victory = "Victory";
        public const string Eat = "ScavengerEat";
        public const string Curse = "WarlockCurse";
        public const string Hammer = "Hammer";
    }
    // Class to preload all audio/sound effects that are contained in the embedded resources.
    // The effects are made available through the soundEffects Dict / the get and the play methods.
    public static class SoundEffectsManager
    {
        private static readonly Dictionary<string, AudioClip> SoundEffects = new();

        public static void Load()
        {
            SoundEffects.Clear();
            foreach (string resourceName in Assembly.GetExecutingAssembly().GetManifestResourceNames())
                if (resourceName.StartsWith("StellarRoles.Resources.SoundEffects.") && resourceName.EndsWith(".raw"))
                    SoundEffects.Add(resourceName, Helpers.LoadAudioClipFromResources(resourceName));
        }

        public static AudioClip Get(string path)
        {
            // Convenience: As as SoundEffects are stored in the same folder, allow using just the name as well
            if (!path.Contains('.')) path = $"StellarRoles.Resources.SoundEffects.{path}.raw";

            return SoundEffects.TryGetValue(path, out AudioClip returnValue) ? returnValue : null;
        }


        public static void Play(string path, float volume = 0.8f)
        {
            if (!MapOptions.EnableSoundEffects) return;
            AudioClip clip = Get(path);
            Stop(clip);
            if (Constants.ShouldPlaySfx()) SoundManager.Instance.PlaySound(clip, false, volume);
        }

        public static void Forceplay(string path, float volume = 0.8f)
        {
            try
            {
                SoundManager.Instance.PlaySound(Helpers.LoadAudioClipFromResources($"StellarRoles.Resources.SoundEffects.{path}.raw"), false, volume);
            }
            catch { }
        }

        public static void Stop(AudioClip clip) => SoundManager.Instance.StopSound(clip);
        public static void Stop(string path) => Stop(Get(path));

        public static void StopAll()
        {
            foreach (string path in SoundEffects.Keys)
                Stop(path);
        }
    }
}
