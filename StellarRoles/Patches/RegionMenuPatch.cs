// Adapted from https://github.com/MoltenMods/Unify
/*
MIT License

Copyright (c) 2021 Daemon

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

/*using HarmonyLib;
using StellarRoles.Utilities;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace StellarRoles.Patches
{
    [HarmonyPatch(typeof(RegionMenu), nameof(RegionMenu.Open))]
    public static class RegionMenuOpenPatch
    {
        private static TextBoxTMP ipField;
        private static TextBoxTMP portField;
        private static JoinGameButton template;

        public static void Postfix(RegionMenu __instance)
        {
            if (!__instance.TryCast<RegionMenu>()) return;
            bool isCustomRegion = ServerManager.Instance.CurrentRegion.Name == "Custom";
            if (template == null)
            {
                var buttons = UnityEngine.Object.FindObjectsOfType<JoinGameButton>();
                foreach (var button in buttons)
                {
                    if (button.GameIdText != null && button.GameIdText.Background != null)
                    {
                        template = button;
                        break;
                    }
                }
            }
            if (template == null || template.GameIdText == null) return;

            if (ipField == null || ipField.gameObject == null)
            {
                ipField = UnityEngine.Object.Instantiate(template.GameIdText, __instance.transform);
                ipField.gameObject.name = "IpTextBox";
                Transform arrow = ipField.transform.FindChild("arrowEnter");
                if (arrow == null || arrow.gameObject == null) return;
                UnityEngine.Object.DestroyImmediate(arrow.gameObject);

                ipField.transform.localPosition = new Vector3(-2.5f, -1.55f, -100f);
                ipField.characterLimit = 30;
                ipField.AllowSymbols = true;
                ipField.ForceUppercase = false;
                ipField.SetText(StellarRolesPlugin.Ip.Value);
                __instance.StartCoroutine(Effects.Lerp(0.1f, new Action<float>((p) =>
                {
                    ipField.outputText.SetText(StellarRolesPlugin.Ip.Value);
                    ipField.SetText(StellarRolesPlugin.Ip.Value);
                })));

                ipField.ClearOnFocus = false;
                ipField.OnEnter = ipField.OnChange = new Button.ButtonClickedEvent();
                ipField.OnFocusLost = new Button.ButtonClickedEvent();
                ipField.OnChange.AddListener((UnityAction)onEnterOrIpChange);
                ipField.OnFocusLost.AddListener((UnityAction)onFocusLost);
                ipField.gameObject.SetActive(isCustomRegion);

                void onEnterOrIpChange()
                {
                    StellarRolesPlugin.Ip.Value = ipField.text;
                }

                void onFocusLost()
                {
                    CustomServerManager.UpdateRegions();
                    //__instance.ChooseOption(ServerManager.DefaultRegions[ServerManager.DefaultRegions.Length - 1]);
                }
            }

            if (portField == null || portField.gameObject == null)
            {
                portField = UnityEngine.Object.Instantiate(template.GameIdText, __instance.transform);
                portField.gameObject.name = "PortTextBox";
                Transform arrow = portField.transform.FindChild("arrowEnter");
                if (arrow == null || arrow.gameObject == null) return;
                UnityEngine.Object.DestroyImmediate(arrow.gameObject);

                portField.transform.localPosition = new Vector3(2.8f, -1.55f, -100f);
                portField.characterLimit = 5;
                portField.SetText(StellarRolesPlugin.Port.Value.ToString());
                __instance.StartCoroutine(Effects.Lerp(0.1f, new Action<float>((p) =>
                {
                    portField.outputText.SetText(StellarRolesPlugin.Port.Value.ToString());
                    portField.SetText(StellarRolesPlugin.Port.Value.ToString());
                })));


                portField.ClearOnFocus = false;
                portField.OnEnter = portField.OnChange = new Button.ButtonClickedEvent();
                portField.OnFocusLost = new Button.ButtonClickedEvent();
                portField.OnChange.AddListener((UnityAction)onEnterOrPortFieldChange);
                portField.OnFocusLost.AddListener((UnityAction)onFocusLost);
                portField.gameObject.SetActive(isCustomRegion);

                void onEnterOrPortFieldChange()
                {
                    if (ushort.TryParse(portField.text, out ushort port))
                    {
                        StellarRolesPlugin.Port.Value = port;
                        portField.outputText.color = Color.white;
                    }
                    else
                    {
                        portField.outputText.color = Color.red;
                    }
                }

                void onFocusLost()
                {
                    CustomServerManager.UpdateRegions();
                    //__instance.ChooseOption(ServerManager.DefaultRegions[ServerManager.DefaultRegions.Length - 1]);
                }
            }

            ipField?.gameObject?.SetActive(isCustomRegion);
            portField?.gameObject?.SetActive(isCustomRegion);

        }
    }

    [HarmonyPatch(typeof(RegionMenu), nameof(RegionMenu.ChooseOption))]
    public static class RegionMenuChooseOptionPatch
    {
        public static bool Prefix(RegionMenu __instance, IRegionInfo region)
        {
            if (region.Name != "Custom" || ServerManager.Instance.CurrentRegion.Name == "Custom") return true;
            DestroyableSingleton<ServerManager>.Instance.SetRegion(region);
            __instance.RegionText.text = "Custom";
            foreach (PoolableBehavior Button in __instance.ButtonPool.activeChildren)
            {
                ServerListButton serverListButton = Button.TryCast<ServerListButton>();
                if (serverListButton != null) serverListButton.SetSelected(serverListButton.Text.text == "Custom");
            }
            __instance.Open();
            return false;
        }
    }
}*/