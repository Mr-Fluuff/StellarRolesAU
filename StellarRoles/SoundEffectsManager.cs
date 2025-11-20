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
        public const string TrackCorpses = "Sniff";
        public const string TrackPlayer = "TrackerTrackPlayer";
        public const string PlaceTrap = "TrapperPlaceTrap";
        public const string Bite = "VampireBite";
        public const string Victory = "Victory";
        public const string Eat = "Eat";
        public const string Curse = "WarlockCurse";
        public const string Hammer = "Hammer";
        public const string Plant = "PlantBomb";
        public const string Convert = "Convert";
        public const string Jam = "HackerJam";
        public const string Mine = "MinerMine";
        public const string Dash = "WraithDash";
        public const string Break = "LanternBreak";
        public const string Admin = "Admin";
        public const string Monitor = "Monitor";
        public const string Vitals = "Vitals";
        public const string Draft = "Draft";


    }
    // Class to preload all audio/sound effects that are contained in the embedded resources.
    // The effects are made available through the soundEffects Dict / the get and the play methods.
    public static class SoundEffectsManager
    {
        private static readonly Dictionary<string, AudioClip> SoundEffects = new();

        public static void Load()
        {
            //SoundEffects.Clear();
            var assembly = Assembly.GetExecutingAssembly();
            var manifest = assembly.GetManifestResourceNames();
            foreach (string resourceName in manifest)
            {
                if (resourceName.Contains("StellarRoles.Resources.SoundEffects.") && resourceName.EndsWith(".raw"))
                {
                    SoundEffects.TryAdd(resourceName, Helpers.LoadAudioClipFromResources(resourceName));
                }
            }
        }

        public static AudioClip Get(string path)
        {
            // Convenience: As as SoundEffects are stored in the same folder, allow using just the name as well
            if (!path.Contains('.')) path = $"StellarRoles.Resources.SoundEffects.{path}.raw";
            return SoundEffects.TryGetValue(path, out AudioClip returnValue) ? returnValue : null;
        }


        public static void Play(string path, float volume = 0.8f, bool loop = false, bool musicChannel = false)
        {
            if (!MapOptions.EnableSoundEffects) return;
            AudioClip clip = Get(path);
            Stop(clip);
            if (Constants.ShouldPlaySfx() && clip != null)
            {
                var source = SoundManager.Instance.PlaySound(clip, false, volume, audioMixer: musicChannel ? SoundManager.Instance.MusicChannel : null);
                source.loop = loop;
            }
        }

        public static void Forceplay(string path, float volume = 0.8f, bool loop = false, bool musicChannel = false)
        {
            Helpers.Log("Try Play Sound " + path);
            AudioClip clip = Get(path);
            //Stop(clip);
                try
                {
                    var source = SoundManager.Instance.PlaySound(clip, false, volume, audioMixer: musicChannel ? SoundManager.Instance.MusicChannel : null);
                    source.loop = loop;
                }
                catch 
                {
                    Helpers.Log("Could Not Play Sound");
                }
        }

        public static void Stop(AudioClip clip)
        {
            if (clip != null)
                try
                {
                    SoundManager.Instance?.StopSound(clip);
                }
                catch { }
        }
        public static void Stop(string path) => Stop(Get(path));

        public static void StopAll()
        {
            if (SoundEffects == null) return;
            try
            {
                foreach (string path in SoundEffects.Keys)
                {
                    Stop(path);
                }
            }
            catch { }
        }
    }
}
