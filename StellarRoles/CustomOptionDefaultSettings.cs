using Il2CppSystem.Runtime.Remoting.Messaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace StellarRoles
{
    public enum Preset
    {
        Default,
        Proximity,
        Standard,
        Beginner,
        Chaotic,
        VanillaTweaks,
        VanillaPlus
    }
    public static class CustomOptionDefaultSettings
    {

        public static Dictionary<Preset, string> PresetIdToPasteKey;
        public static Dictionary<Preset, Dictionary<int, int>> PresetIdDefault = new();


        /*        public static object GetDefaultPresetValue(float id, int preset)
                {
                    PresetIdToMapping ??= LoadPresets();

                    return PresetIdToMapping.TryGetValue(id, out PresetMapping mapping) ? mapping.GetDefaultValue(preset) : null;
                }*/

        public static string GetDefaultPresetPasteKey(Preset id)
        {
            PresetIdToPasteKey ??= LoadPasteKeys();

            if (PresetIdToPasteKey.TryGetValue(id, out string pasteKey))
            {
                return pasteKey;
            }
            return "";
        }

        private const string VanillaTweaks = "!AGQAgADHAICDgIAAzQAA+gCHjYSBgIAALAGCkAAxAYSAhoAAXgEAYQGAgIKCgYAAkAGDggSUAQDCAYKAAcYBhpIFygEA9AGIiJ0C+QGGg4CChIEBCAIAJgKFg4CEhoABOgIAWAIAYgKHgoSDAIoCiImCALwChoQA3AWAAAwGgICAgIGFgIAAQgaAgICAgICAgICAgIAAcgaDgQCkBoKEhIKDAAgHhYCDAMQJgAD2CYGBgoEAKAqAgABaCoeAgIQAjAqCgoEGkQqCAL4KhISAAcMKAPAKhIQArA2AAN4NgoAAEA6ABBUOgQAaDoGAgABCDgdEDoEAdA6AgYCBBHoOAHwOAKYOgAGpDoGBgQDYDoCAjwAjDwMlD4CBgQEqDwA8D4SEgABBDwBuD4KEgAFzD4MAoA+CgIICpQ+AANIPgIEABBADCBCGgYABDRCAADYQgoKGAjsQAGgQggJrEAFtEIEAzBCEg4EAiBOAgICAgAXsE4ADHhSEBVAUgoEDghSAgIAEjBSEgYAFtBSABRgVgAAMF4iAgAZwFwFyF4GBgIGBgYCAgICAgICAgICAgAChF4CBgYGBALsXgICAgICDgICAgAHUF4OChYCAgIAG3hcC6BeDhYSAhQLyF4OFhICFAvwXg4WEgIUCBhiDhYWAhwIQGIOFhYCHAhoYg4WFgIcAahiAgICAgIAA6hqCggBZG4CAgIAAYhsGZBuBgYEAvBuAgICBgYGAgYCAgABAHwBCHwBFH4CAgIEBSx+BACkjgICAAC4jgICAADMjgICAAOcmgIAAACc=!CXQAAAEAZAwAAQAABQAAoD8AAAA/AAAAQAAAtEECAgQBAAAAAgEPAAAAeAAAAAAUAAABAQAHBQAAAAMAAAAKCAIAAAACAAAPBQQAAAADAAA8CgADAAAAAgAAHg8IAAAAAgAACgEJAAAAAgAADx4KAAAAAwAADx4B";
        private const string Standard = "!AmQAggDHAIKDgIAAzQAD+gCHjYSBgIAFLAGCkgAxAYaChoAHXgEAYQGDg4KCgIEEkAGDggWUAQTCAYOAAcYBhpAFygEC9AGGiJ0C+QGGg4KChIEBCAIBJgKFg4CEhoABOgIBWAIEYgKHgoSDAIoCiImCAbwChoQB3AWBAAwGgICAgIGDgIAAQgaAgICAgICAgICAgIAGcgaDgQKkBoKEhIKDBAgHhYCDAcQJggL2CYGBgoECKAqAgAVaCoaAgYYCjAqEgoEHkQqCBL4KhISAAcMKAPAKhIQIrA2KBd4NgoABEA6BBBUOgQYaDoGAgANCDglEDoEEdA6AgYCBBXoOAnwOCaYOgAGpDoGBgQXYDoCAjgMjDwMlD4CBgQEqDwY8D4KEgABBDwRuD4KDgAFzD4MCoA+CgIICpQ+AANIPgIEBBBADCBCGgYABDRCABTYQgoKGAjsQBGgQggJrEAFtEIECzBCEg4EAiBODgIKAggXsE4ADHhSEBVAUgoEDghSAgIAEjBSEgYAFtBSABRgVgAAMF4iAgARwFwFyF4GBgIGBgYWFgYCBgYGAhYSEhAChF4CBgYGBALsXgICAgICDgICAgAHUF4KChYCAgYEF3hcC6BeEhoSAhQLyF4SGhICFAvwXhIaEgIUCBhiDhYaAiQQQGISEhYCHAxoYhIWGgIkAahiBgYGBgYEA6hqCggBZG4CAgIAAYhsAZBuBgYEAvBuAgICBgYGAgYCAgABAHwBCHwBFH4CAgIABSx+BACkjgYGAAS4jgYCAADMjgICBAOcmgIAAACc=!CXQAAAEAZAwAAQAABQAAoD8AAAA/AAAAQAAAtEECAgQBAAAAAgEPAAAAeAAAAAAUAAABAQAHBQAAAAMAAAAKCAIAAAACAAAPBQQAAAADAAA8CgADAAAAAgAAHg8IAAAAAgAACgEJAAAAAgAADx4KAAAAAwAADx4B";
        private const string Proximity = "!AmQAggDHAIKKgIAAzQAE+gCIjYSBgIAFLAGCkgExAYeChoEIXgEAYQGDhIKCgYEEkAGAmwOUAQXCAYKBAcYBhpoIygEC9AGGiZ0B+QGGg4SAhIEBCAICJgKFg4CDhoABOgICWAIEYgKHg4OCAYoChoGCALwChoQB3AWBAAwGgICAgIKFgIAAQgaAgICAgICAgICAgYAEcgaDgQSkBoKEhIKDBAgHhYCDAsQJggL2CYGBgoECKAqAgAVaCoeAgYQCjAqEgoEFkQqCBb4Kg4SAAsMKAPAKhIQKrA2MBd4NhIAAEA6BBBUOgQYaDoGAgAVCDgNEDoEGdA6AgYCBAHoOAnwOCaYOgACpDoCAgAXYDoCAkAQjDwIlD4CBgwMqDwY8D4aEgABBDwVuD4WFgABzD4ACoA+CgYICpQ+AANIPgIEBBBAHCBCHgIAADRCEBTYQgoKGAjsQBWgQgwFrEAFtEIEEzBCEg4EBiBOEgYKAggXsE4ADHhSEBVAUgoEDghSBgIEFjBSEgYAFtBSABRgVgAAMF4iAgApwFwFyF4GBgYGBgYGBgICBgYGAgYGBgQChF4CBgYGBALsXgICAgICClICAgAHUF4OBhoCAgIAG3hcC6BeCgoSBhQLyF4KChIGFAvwXgoKEgYUCBhiCgoWBhwIQGIKChYGHAhoYgoKFgYcAahiAgICAgIAA6hqCggBZG4CAgIAAYhsDZBuAgYEAvBuAgICBgICAgICAgABAHwBCHwBFH4CAgIEBSx+BACkjgYGAAS4jgYCAADMjgYCBAOcmgIAAACc=!CXQAAAEAZAwAAQAABQAAoD8AAAA/AAAAQAAAtEECAgQBAAAAAgEPAAAAeAAAAAAUAAABAQAHBQAAAAMAAAAKCAIAAAACAAAPBQQAAAADAAA8CgADAAAAAgAAHg8IAAAAAgAACgEJAAAAAgAADx4KAAAAAwAADx4B";
        private const string Chaotic = "!AmQAggDHAIKKgIAAzQAD+gCGjYWBgIAFLAGAkgExAYCChoEIXgEAYQGDg4KDgoEEkAGAtAGUAQXCAYCAAcYBhZ4GygEC9AGGiZ0B+QGGg4WAgoEBCAIDJgKCgoCDhoEBOgICWAIEYgKHg4OCAooChYGCALwChoQC3AWCAAwGgICAgIKFgIAAQgaAgICAgICAgICAgYAEcgaCgQSkBoKEhIKDBAgHhYCDAcQJggL2CYGBgoECKAqAgAVaCoeAgYQCjAqEgoEFkQqCBb4Kg4SAAsMKAPAKhIQKrA2MBd4NhIAAEA6BBBUOgQYaDoGBgQVCDgNEDoEGdA6AgYCBAHoOAHwOCaYOgAGpDoCAgQXYDoCAkwQjDwIlD4CBgwMqDwU8D4iEgABBDwVuD4WGgABzD4ADoA+CgoICpQ+AANIPgIEBBBAJCBCIgIAADRCEBTYQgoKEAjsQBWgQgwFrEAFtEIEEzBCDg4ECiBOEgYKAggXsE4AFHhSEBVAUgoEDghSAgIEFjBSEgYAFtBSABRgVgAAMF4iAgApwFwFyF4GBgYGBgYCAgICBgYGAgICAgAChF4CBgYGBALsXgICAgICDgICAgAHUF4OBhoGAgIEG3hcB6BeBgoSBhQHyF4GChIGFAfwXgYKEgYUBBhiBgoWBhwEQGIGChYGHARoYgYKFgYcAahiAgICAgIAA6hqCggBZG4CAgIAAYhsAZBuAgYEAvBuAgICBgICAgICAgABAHwBCHwBFH4CAgIEBSx+BACkjgIGAAS4jgYCAADMjgICBAOcmgIAAACc=!CXQAAAEAZAwAAQAABQAAoD8AAAA/AAAAQAAAtEECAgQBAAAAAgEPAAAAeAAAAAAUAAABAQAHBQAAAAMAAAAKCAIAAAACAAAPBQQAAAADAAA8CgADAAAAAgAAHg8IAAAAAgAACgEJAAAAAgAADx4KAAAAAwAADx4B";
        private const string Beginner = "!AmQAggDHAIKKgIAAzQAA+gCGjYWBgIAFLAGCkgAxAYSAhoAIXgEBYQGDgoGDgIEDkAGAgQSUAQXCAYKBAMYBipoIygEA9AGGiYkB+QGIhYCAgoEACAIAJgKFg4CEhoABOgICWAIFYgKHgoOCAIoChoGCALwCiIQA3AWAAAwGgICAgIKFgIAAQgaAgICAgICAgICAgIAEcgaCgAKkBoKEhIKDAggHhYCDAcQJgQL2CYGBgoECKAqAgARaCoeAgIQAjAqEgoEFkQqAAL4Kg4SAAsMKAPAKhoQGrA2MA94NhIAAEA6BAhUOgAUaDoGAgAJCDgNEDoEGdA6AgYGBAnoOAnwOCKYOgAGpDoGBgQXYDoCAjwIjDwMlD4CBgwMqDwY8D4WDgABBDwVuD4SEgABzD4ACoA+CgIICpQ+AANIPgIECBBAHCBCGgYAADRCAAzYQgYGGAjsQAmgQgwFrEAFtEIEDzBCEg4EBiBOEgYKAgQjsE4AIHhSECFAUgoECghSAgIAFjBSEgYAItBSACBgVgAAMF4iAgApwFwFyF4GBgYGBgYGBgICBgYGAgYGBgQChF4CAgICAALsXgICAgICDgICAgADUF4OChoCAgIEG3hcC6BeDhISBhQLyF4OEhIGFAvwXg4SEgYUCBhiDhIWBhwIQGIOEhYGHAhoYg4SFgYcAahiAgICAgIAA6hqCggBZG4CAgIAAYhsBZBuAgYEAvBuAgICBgICAgICAgABAHwBCHwBFH4CAgIAASx+AACkjgYGAAS4jgYCAATMjgICAAOcmgIAAACc=!CXQAAAEAZAwAAQAABQAAoD8AAAA/AAAAQAAAtEECAgQBAAAAAgEPAAAAeAAAAAAUAAABAQAHBQAAAAMAAAAKCAIAAAACAAAPBQQAAAADAAA8CgADAAAAAgAAHg8IAAAAAgAACgEJAAAAAgAADx4KAAAAAwAADx4B";
        private const string VanillaPlus = "!AWQAgQDHAICBgIAAzQAA+gCHjYSBgIAFLAGCkgAxAYSAhoAFXgEBYQGDg4GDgIADkAGDgAWUAQXCAYKAAMYBhpAFygEA9AGIiJ0C+QGGg4GChIEBCAIAJgKFg4CEhoABOgIDWAIFYgKHgoSDAIoCiImCALwChoQA3AWAAAwGgICAgIGFgIAAQgaAgICAgICAgICAgIAAcgaDgQCkBoKEhIKDAAgHhYCDAMQJgAD2CYGBgoEAKAqAgABaCoaAgYYAjAqEgoEHkQqCAL4KhISAAcMKAPAKhIQDrA2FAN4NgYAAEA6BBBUOgQUaDoGAgABCDgpEDoEFdA6AgYCBBHoOAHwOAqYOgAGpDoGBgQTYDoCAjgAjDwMlD4CBgQEqDwU8D4OEgABBDwRuD4KFgABzD4AEoA+CgIICpQ+AANIPgIECBBADCBCGgYABDRCAADYQgoKGAjsQAGgQggJrEAFtEIEAzBCEg4EAiBOAgICAgAXsE4ADHhSEBVAUgoEDghSAgIADjBSCgYAAtBSABRgVgAAMF4iAgAZwFwFyF4GBgIGBgYSEgYCAgICAhISEhAChF4CBgYGBALsXgICAgICDgICAgAHUF4OChYCAgYEG3hcC6BeDhYSAhQLyF4OFhICFAvwXg4WEgIUCBhiDhYWAhwIQGIOFhYCHAhoYg4WFgIcAahiAgICAgIAA6hqCggBZG4CAgIAAYhsAZBuBgYEAvBuAgICBgYGAgYCAgABAHwBCHwBFH4CAgIEASx+BASkjgYGBAS4jgYGBATMjgYGBAOcmgIAAACc=!CXQAAAEAZAwAAQAABQAAoD8AAAA/AAAAQAAAtEECAgQBAAAAAgEPAAAAeAAAAAAUAAABAQAHBQAAAAMAAAAKCAIAAAACAAAPBQQAAAADAAA8CgADAAAAAgAAHg8IAAAAAgAACgEJAAAAAgAADx4KAAAAAwAADx4B";
        private const string Default = "!AGQAgADHAICDgIAAzQAA+gCHjYSBgIAALAGCkAAxAYWAhoEAXgEBYQGDgIKCgYAAkAGDggSUAQDCAYKAAcYBhpIFygEA9AGIiJ0C+QGGg4CChIEBCAIAJgKFg4CEhoABOgIAWAIAYgKHgoSDAIoCiImCALwChoQA3AWAAAwGgICAgIGFgIAAQgaAgICAgICAgYCAgIAAcgaDgQCkBoKEhIKDAAgHhYCDAMQJgAD2CYGBgoEAKAqAgABaCoeAgIQAjAqCgoEGkQqCAL4KhISAAcMKAPAKhIQArA2AAN4NgoAAEA6ABBUOgQAaDoGAgABCDgdEDoEAdA6AgYCBBHoOAHwOAKYOgAGpDoGBgQDYDoCAjwAjDwMlD4CBgQEqDwA8D4SEgABBDwBuD4KEgAFzD4MAoA+CgIICpQ+AANIPgIEABBADCBCGgYABDRCAADYQgoKGAjsQAGgQggJrEAFtEIEAzBCEg4EAiBOAgICAgAXsE4ADHhSEBVAUgoEDghSAgIAEjBSEgYAFtBSABRgVgAAMF4iAgAZwFwFyF4GBgIGBgYCAgICAgICAgICAgAChF4CBgYGBALsXgICAgICDgoCAgADUF4OChYCAgIAG3hcC6BeDhYSBhQLyF4OFhIGFAvwXg4WEgYUCBhiDhYWBhwIQGIOFhYGHAhoYg4WFgYcAahiAgICAgIAA6hqCggBZG4CAgIABYhsFZBuBgYEAvBuAgICBgYGAgYCAgABAHwBCHwBFH4CAgIABSx+BACkjgICAAC4jgICAADMjgICAAOcmgIAAACc=!CXQAAAEAZAwAAQAABQAAoD8AAAA/AAAAQAAAtEECAgQBAAAAAgEPAAAAeAAAAAAUAAABAQAHBQAAAAMAAAAKCAIAAAACAAAPBQQAAAADAAA8CgADAAAAAgAAHg8IAAAAAgAACgEJAAAAAgAADx4KAAAAAwAADx4B";

        public static Dictionary<Preset, string> LoadPasteKeys()
        {
            return new Dictionary<Preset, string>
            {
                { Preset.VanillaTweaks, StellarRolesPlugin.VersionString + VanillaTweaks},
                { Preset.Standard, StellarRolesPlugin.VersionString + Standard},
                { Preset.Proximity, StellarRolesPlugin.VersionString + Proximity},
                { Preset.Chaotic, StellarRolesPlugin.VersionString + Chaotic},
                { Preset.Beginner, StellarRolesPlugin.VersionString + Beginner},
                { Preset.VanillaPlus, StellarRolesPlugin.VersionString + VanillaPlus},
                { Preset.Default, StellarRolesPlugin.VersionString + Default}
            };
        }

        public static void SetPresetDefaults(Preset preset)
        {
            var pasteKey = GetDefaultPresetPasteKey(preset);
            string[] settingsSplit = pasteKey.Split("!");
            string stellarSettings = settingsSplit[1];
            byte[] inputValues = Convert.FromBase64String(stellarSettings);
            BinaryReader reader = new BinaryReader(new MemoryStream(inputValues));
            int lastId = -1;
            int errors = 0;
            while (reader.BaseStream.Position < inputValues.Length)
            {
                try
                {
                    int selection = reader.ReadByte();
                    int id = -1;
                    bool consecutive = selection >= 128;
                    if (consecutive)
                    {
                        selection -= 128;
                        id = lastId + 1;
                    }
                    else
                    {
                        id = reader.ReadUInt16();
                    }
                    if (id == 0) continue;
                    lastId = id;
                    if (PresetIdDefault.ContainsKey(preset))
                    {
                        PresetIdDefault[preset].Add(id, selection);
                    }
                    else
                    {
                        PresetIdDefault.Add(preset, new Dictionary<int, int> { { id, selection } });
                    }
                    CustomOption option = CustomOption.Options.First(option => option.Id == id);
                    option.Entry = StellarRolesPlugin.Instance.Config.Bind($"Preset{preset}", option.Id.ToString(), selection);
                }
                catch (Exception e)
                {
                    StellarRolesPlugin.Logger.LogWarning($"id:{lastId}:{e}: while deserializing - tried to paste invalid settings!");
                    errors++;
                }
            }
        }

        public static List<Preset> sets = new List<Preset> { Preset.Default, Preset.Proximity, Preset.Standard, Preset.Beginner, Preset.Chaotic, Preset.VanillaTweaks, Preset.VanillaPlus };
        public static Dictionary<int, int> Presets(Preset preset)
        {
            CreatePresets();
            if (PresetIdDefault.ContainsKey(preset))
            {
                return PresetIdDefault[preset];
            }
            else return null;
        }

        public static void CreatePresets()
        {
            if (PresetIdDefault == null || PresetIdDefault.Count == 0)
            {
                foreach (var set in sets)
                {
                    SetPresetDefaults(set);
                }
            }
        }
    }
}
