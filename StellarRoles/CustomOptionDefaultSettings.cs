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

        private const string VanillaTweaks = "!AGQAgADHAICDgIAAzQAA+gCHjYSBgIAALAGCkAAxAYSAhoAAXgEAkAGDggSUAQDCAYKAAcYBhpIFygEA9AGIiJ0C+QGGg4CChIEBCAIAJgKFg4CEhoABOgIAWAIAYgKHgoSDAIoCiImCALwChoQA3AWAAAwGgICAgIGFgIAAQgaAgICAgICAgICAgAByBoOBAKQGgoSEgoMACAeFgIMAxAmAAPYJgYGCgQAoCoCAAFoKh4CAhACMCoKCgQaRCoIAvgqEhIABwwoA8AqEhACsDYAA3g2CgAAQDoAEFQ6BABoOgYCAAEIOB0QOgQB0DoCBgIEEeg4AfA4Apg6AAakOgYGBANgOgICPACMPAyUPgIGBASoPADwPhISAAEEPAG4PgoSAAXMPgwCgD4KAggKlD4AA0g+AgQAEEAMIEIaBgAENEIAANhCCgoYCOxAAaBCCAmsQAW0QgQDMEISDgQCIE4CAgICABewTgAMeFIQFUBSCgQOCFICAgASMFISBgAW0FIAFGBWAAAwXiICABnAXAXIXgYGAgYGBgICAgICAgICAgICAAKEXgIGBgYEAuxeAgICAAdQXg4KFgICAgAbeFwLoF4OFhICFAvIXg4WEgIUC/BeDhYSAhQIGGIOFhYCHAhAYg4WFgIcCGhiDhYWAhwBqGICAgICAgADqGoKCAFkbgICAgABiGwZkG4GBgQC8G4CAgIGBgYCBgICAAEAfAEIfAEUfgICAgQFLH4EAKSOAgIAALiOAgIAAMyOAgIAA5yaAgAAAJw==!CHMAAAEAZAwAAQAAAgAAoD8AAIA+AACgPwAAtEECAwgBAAAAAgEAAAAAeAAAAAAUAAABAQcFAAAAAwAAAAoIAgAAAAIAAA8FBAAAAAMAADwKAAMAAAACAAAeDwgAAAACAAAKAQkAAAACAAAPHgoAAAADAAAPHgE=";
        private const string Standard = "!AmQAggDHAIKDgIAAzQAD+gCHjYSBgIAFLAGCkgAxAYaChoAHXgEEkAGDggWUAQTCAYOAAcYBhpAFygEC9AGGiJ0C+QGGg4KChIEBCAIBJgKFg4CEhoABOgIBWAIEYgKHgoSDAIoCiImCAbwChoQB3AWBAAwGgICAgIGDgIAAQgaAgICAgICAgICAgAZyBoOBAqQGgoSEgoMECAeFgIMBxAmCAvYJgYGCgQIoCoCABVoKhoCBhgKMCoSCgQeRCoIEvgqEhIABwwoA8AqEhAisDYoF3g2CgAEQDoEEFQ6BBhoOgYCAA0IOCUQOgQR0DoCBgIEFeg4CfA4Jpg6AAakOgYGBBdgOgICOAyMPAyUPgIGBASoPBjwPgoSAAEEPBG4PgoOAAXMPgwKgD4KAggKlD4AA0g+AgQEEEAMIEIaBgAENEIAFNhCCgoYCOxAEaBCCAmsQAW0QgQLMEISDgQCIE4OAgoCCBewTgAMeFIQFUBSCgQOCFICAgASMFISBgAW0FIAFGBWAAAwXiICABHAXAXIXgYGAgYGBhYWBgIGBgYCFhISEAKEXgIGBgYEAuxeAgICAAdQXgoKFgICBgQXeFwLoF4SGhICFAvIXhIaEgIUC/BeEhoSAhQIGGIOFhoCJBBAYhISFgIcDGhiEhYaAiQBqGIGBgYGBgQDqGoKCAFkbgICAgABiGwBkG4GBgQC8G4CAgIGBgYCBgICAAEAfAEIfAEUfgICAgAFLH4EAKSOBgYABLiOBgIAAMyOAgIEA5yaAgAAAJw==!CHMAAAEAZAwAAQAAAgAAoD8AAIA+AACgPwAAtEECAwcBAAAAAgEAAAAAeAAAAAAUAAABAQcFAAAAAwAAAAoIAgAAAAIAAA8FBAAAAAMAADwKAAMAAAACAAAeDwgAAAACAAAKAQkAAAACAAAPHgoAAAADAAAPHgE=";
        private const string Proximity = "!AmQAggDHAIKKgIAAzQAE+gCIjYSBgIAFLAGCkgExAYeChoEIXgEEkAGAmwOUAQXCAYKBAcYBhpoIygEC9AGGiZ0B+QGGg4SAhIEBCAICJgKFg4CDhoABOgICWAIEYgKHg4OCAYoChoGCALwChoQB3AWBAAwGgICAgIKFgIAAQgaAgICAgICAgICAgQRyBoOBBKQGgoSEgoMECAeFgIMCxAmCAvYJgYGCgQIoCoCABVoKh4CBhAKMCoSCgQWRCoIFvgqDhIACwwoA8AqEhAqsDYwF3g2EgAAQDoEEFQ6BBhoOgYCABUIOA0QOgQZ0DoCBgIEAeg4CfA4Jpg6AAKkOgICABdgOgICQBCMPAiUPgIGDAyoPBjwPhoSAAEEPBW4PhYWAAHMPgAKgD4KBggKlD4AA0g+AgQEEEAcIEIeAgAANEIQFNhCCgoYCOxAFaBCDAWsQAW0QgQTMEISDgQGIE4SBgoCCBewTgAMeFIQFUBSCgQOCFIGAgQWMFISBgAW0FIAFGBWAAAwXiICACnAXAXIXgYGBgYGBgYGAgIGBgYCBgYGBAKEXgIGBgYEAuxeAgICAAdQXg4GGgICAgAbeFwLoF4KChIGFAvIXgoKEgYUC/BeCgoSBhQIGGIKChYGHAhAYgoKFgYcCGhiCgoWBhwBqGICAgICAgADqGoKCAFkbgICAgABiGwNkG4CBgQC8G4CAgIGAgICAgICAAEAfAEIfAEUfgICAgQFLH4EAKSOBgYABLiOBgIAAMyOBgIEA5yaAgAAAJw==!CHMAAAEAZAwAAQAAAAAAoD8AAAA/AACgPwAAtEECAgIBAAAAAgEPAAAAaQAAAAAUAAABAQcFAAAAAwAAAAoIAgAAAAIAAA8FBAAAAAMAADwKAAMAAAACAAAeDwgAAAACAAAKAQkAAAACAAAPHgoAAAADAAAPHgE=";
        private const string Chaotic = "!AmQAggDHAIKKgIAAzQAD+gCGjYWBgIAFLAGAkgExAYCChoEIXgEEkAGAtAGUAQXCAYCAAcYBhZ4GygEC9AGGiZ0B+QGGg4WAgoEBCAIDJgKCgoCDhoEBOgICWAIEYgKHg4OCAooChYGCALwChoQC3AWCAAwGgICAgIKFgIAAQgaAgICAgICAgICAgQRyBoKBBKQGgoSEgoMECAeFgIMBxAmCAvYJgYGCgQIoCoCABVoKh4CBhAKMCoSCgQWRCoIFvgqDhIACwwoA8AqEhAqsDYwF3g2EgAAQDoEEFQ6BBhoOgYGBBUIOA0QOgQZ0DoCBgIEAeg4AfA4Jpg6AAakOgICBBdgOgICTBCMPAiUPgIGDAyoPBTwPiISAAEEPBW4PhYaAAHMPgAOgD4KCggKlD4AA0g+AgQEEEAkIEIiAgAANEIQFNhCCgoQCOxAFaBCDAWsQAW0QgQTMEIODgQKIE4SBgoCCBewTgAUeFIQFUBSCgQOCFICAgQWMFISBgAW0FIAFGBWAAAwXiICACnAXAXIXgYGBgYGBgICAgIGBgYCAgICAAKEXgIGBgYEAuxeAgICAAdQXg4GGgYCAgQbeFwHoF4GChIGFAfIXgYKEgYUB/BeBgoSBhQEGGIGChYGHARAYgYKFgYcBGhiBgoWBhwBqGICAgICAgADqGoKCAFkbgICAgABiGwBkG4CBgQC8G4CAgIGAgICAgICAAEAfAEIfAEUfgICAgQFLH4EAKSOAgYABLiOBgIAAMyOAgIEA5yaAgAAAJw==!CHMAAAEAZAwAAQAAAAAAoD8AAAA/AACgPwAAtEECAQIBAAAAAgEPAAAAeAAAAAAUAAABAQcFAAAAAwAAAAoIAgAAAAIAAA8FBAAAAAMAADwKAAMAAAACAAAeDwgAAAACAAAKAQkAAAACAAAPHgoAAAADAAAPHgE=";
        private const string Beginner = "!AmQAggDHAIKKgIAAzQAA+gCGjYWBgIAFLAGCkgAxAYSAhoAIXgEDkAGAgQSUAQXCAYKBAMYBipoIygEA9AGGiYkB+QGIhYCAgoEACAIAJgKFg4CEhoABOgICWAIFYgKHgoOCAIoChoGCALwCiIQA3AWAAAwGgICAgIKFgIAAQgaAgICAgICAgICAgARyBoKAAqQGgoSEgoMCCAeFgIMBxAmBAvYJgYGCgQIoCoCABFoKh4CAhACMCoSCgQWRCoAAvgqDhIACwwoA8AqGhAasDYwD3g2EgAAQDoECFQ6ABRoOgYCAAkIOA0QOgQZ0DoCBgYECeg4CfA4Ipg6AAakOgYGBBdgOgICPAiMPAyUPgIGDAyoPBjwPhYOAAEEPBW4PhISAAHMPgAKgD4KAggKlD4AA0g+AgQIEEAcIEIaBgAANEIADNhCBgYYCOxACaBCDAWsQAW0QgQPMEISDgQGIE4SBgoCBCOwTgAgeFIQIUBSCgQKCFICAgAWMFISBgAi0FIAIGBWAAAwXiICACnAXAXIXgYGBgYGBgYGAgIGBgYCBgYGBAKEXgICAgIAAuxeAgICAANQXg4KGgICAgQbeFwLoF4OEhIGFAvIXg4SEgYUC/BeDhISBhQIGGIOEhYGHAhAYg4SFgYcCGhiDhIWBhwBqGICAgICAgADqGoKCAFkbgICAgABiGwFkG4CBgQC8G4CAgIGAgICAgICAAEAfAEIfAEUfgICAgABLH4AAKSOBgYABLiOBgIABMyOAgIAA5yaAgAAAJw==!CHMAAAEAZAwAAQAAAAAAoD8AAAA/AACgPwAAtEECAwUBAAAAAgEPAAAAeAAAAAAUAAABAQcFAAAAAwAAAAoIAgAAAAIAAA8FBAAAAAMAADwKAAMAAAACAAAeDwgAAAACAAAKAQkAAAACAAAPHgoAAAADAAAPHgE=";
        private const string VanillaPlus = "!AWQAgQDHAICBgIAAzQAA+gCHjYSBgIAFLAGCkgAxAYSAhoAFXgEDkAGDgAWUAQXCAYKAAMYBhpAFygEA9AGIiJ0C+QGGg4GChIEBCAIAJgKFg4CEhoABOgIDWAIFYgKHgoSDAIoCiImCALwChoQA3AWAAAwGgICAgIGFgIAAQgaAgICAgICAgICAgAByBoOBAKQGgoSEgoMACAeFgIMAxAmAAPYJgYGCgQAoCoCAAFoKhoCBhgCMCoSCgQeRCoIAvgqEhIABwwoA8AqEhAOsDYUA3g2BgAAQDoEEFQ6BBRoOgYCAAEIOCkQOgQV0DoCBgIEEeg4AfA4Cpg6AAakOgYGBBNgOgICOACMPAyUPgIGBASoPBTwPg4SAAEEPBG4PgoWAAHMPgASgD4KAggKlD4AA0g+AgQIEEAMIEIaBgAENEIAANhCCgoYCOxAAaBCCAmsQAW0QgQDMEISDgQCIE4CAgICABewTgAMeFIQFUBSCgQOCFICAgAOMFIKBgAC0FIAFGBWAAAwXiICABnAXAXIXgYGAgYGBhISBgICAgICEhISEAKEXgIGBgYEAuxeAgICAAdQXg4KFgICBgQbeFwLoF4OFhICFAvIXg4WEgIUC/BeDhYSAhQIGGIOFhYCHAhAYg4WFgIcCGhiDhYWAhwBqGICAgICAgADqGoKCAFkbgICAgABiGwBkG4GBgQC8G4CAgIGBgYCBgICAAEAfAEIfAEUfgICAgQBLH4EBKSOBgYEBLiOBgYEBMyOBgYEA5yaAgAAAJw==!CHMAAAEAZAwAAQAAAgAAoD8AAIA+AACgPwAAtEECAwUBAAAAAgEAAAAAeAAAAAAUAAABAQcFAAAAAwAAAAoIAgAAAAIAAA8FBAAAAAMAADwKAAMAAAACAAAeDwgAAAACAAAKAQkAAAACAAAPHgoAAAADAAAPHgE=";
        private const string Default = "!AGQAgADHAICDgIAAzQAA+gCHjYSBgIAALAGCkAAxAYWAhoEAXgEAkAGDggSUAQDCAYKAAcYBhpIFygEA9AGIiJ0C+QGGg4CChIEBCAIAJgKFg4CEhoABOgIAWAIAYgKHgoSDAIoCiImCALwChoQA3AWAAAwGgICAgIGFgIAAQgaAgICAgICAgYCAgAByBoOBAKQGgoSEgoMACAeFgIMAxAmAAPYJgYGCgQAoCoCAAFoKh4CAhACMCoKCgQaRCoIAvgqEhIABwwoA8AqEhACsDYAA3g2CgAAQDoAEFQ6BABoOgYCAAEIOB0QOgQB0DoCBgIEEeg4AfA4Apg6AAakOgYGBANgOgICPACMPAyUPgIGBASoPADwPhISAAEEPAG4PgoSAAXMPgwCgD4KAggKlD4AA0g+AgQAEEAMIEIaBgAENEIAANhCCgoYCOxAAaBCCAmsQAW0QgQDMEISDgQCIE4CAgICABewTgAMeFIQFUBSCgQOCFICAgASMFISBgAW0FIAFGBWAAAwXiICABnAXAXIXgYGAgYGBgICAgICAgICAgICAAKEXgIGBgYEAuxeAgICAANQXg4KFgICAgAbeFwLoF4OFhIGFAvIXg4WEgYUC/BeDhYSBhQIGGIOFhYGHAhAYg4WFgYcCGhiDhYWBhwBqGICAgICAgADqGoKCAFkbgICAgAFiGwVkG4GBgQC8G4CAgIGBgYCBgICAAEAfAEIfAEUfgICAgAFLH4EAKSOAgIAALiOAgIAAMyOAgIAA5yaAgAAAJw==!CHMAAAEAZAwAAQAAAgAAoD8AAAA/AACgPwAAtEECAwUBAAAAAgAAAAAAeAAAAAAUAAABAQcFAAAAAwAAAAoIAgAAAAIAAA8FBAAAAAMAADwKAAMAAAACAAAeDwgAAAACAAAKAQkAAAACAAAPHgoAAAADAAAPHgE=";

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
