using AmongUs.Data;
using Assets.InnerNet;
using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Button;
using Object = UnityEngine.Object;

namespace StellarRoles.Modules
{
    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
    public class MainMenuPatch
    {
        private static GameObject regionResetConfirmTemplate;
        private static AnnouncementPopUp popUp;

        private static void Prefix(MainMenuManager __instance)
        {

            SoundEffectsManager.Load();
            GameObject NewsB = GameObject.Find("NewsButton");
            GameObject AccountB = GameObject.Find("AcountButton");
            GameObject SettingsB = GameObject.Find("SettingsButton");
            List<GameObject> objects = new() { NewsB, AccountB, SettingsB };
            foreach (GameObject obj in objects)
            {
                obj.transform.localScale = new Vector3(0.41f, 0.84f, 1);
                var pos = obj.transform.localPosition;
                pos.x = -0.87f;
                obj.transform.localPosition = pos;

                var FontPlacer = obj.transform.FindChild("FontPlacer").gameObject;
                FontPlacer.transform.localScale = new Vector3(2, 1, 1);
                FontPlacer.transform.localPosition = new Vector3(-1.6159f, -0.0818f, 0);

                var Icon = obj.transform.FindChild("Inactive").FindChild("Icon").gameObject;
                Icon.transform.localScale += new Vector3(0.4f, 0, 0);

                var Icon2 = obj.transform.FindChild("Highlight").FindChild("Icon").gameObject;
                Icon2.transform.localScale += new Vector3(0.4f, 0, 0);
            }



            #region DiscordButton
            GameObject buttonDiscord = Object.Instantiate(AccountB, AccountB.transform.parent);
            buttonDiscord.name = "DiscordButton";
            buttonDiscord.transform.localPosition = new Vector3(0.87f, -0.387f, 0);
            buttonDiscord.transform.FindChild("Inactive").FindChild("Icon").GetComponent<SpriteRenderer>().sprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.discord.png", 240f);
            buttonDiscord.transform.FindChild("Highlight").FindChild("Icon").GetComponent<SpriteRenderer>().sprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.discord.png", 240f);

            TMPro.TMP_Text textDiscord = buttonDiscord.transform.GetComponentInChildren<TMPro.TMP_Text>();
            __instance.StartCoroutine(Effects.Lerp(0.5f, new Action<float>((p) =>
            {
                textDiscord.SetText("SR Discord");
            })));
            PassiveButton passiveButtonDiscord = buttonDiscord.GetComponent<PassiveButton>();
            passiveButtonDiscord.OnClick = new ButtonClickedEvent();
            passiveButtonDiscord.OnClick.AddListener((Action)(() => Application.OpenURL("https://discord.gg/7UcvM9CFdY")));

            #endregion DiscordButton

            #region SR Credits
            GameObject creditsButton = Object.Instantiate(AccountB, AccountB.transform.parent);
            creditsButton.name = "SRCreditsButton";
            creditsButton.transform.localPosition = new Vector3(0.87f, -0.912f, 0);
            creditsButton.transform.FindChild("Inactive").FindChild("Icon").GetComponent<SpriteRenderer>().sprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.Gooper.png", 200f);
            creditsButton.transform.FindChild("Highlight").FindChild("Icon").GetComponent<SpriteRenderer>().sprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.Gooper.png", 200f);


            TMPro.TMP_Text textCreditsButton = creditsButton.transform.GetComponentInChildren<TMPro.TMP_Text>();
            __instance.StartCoroutine(Effects.Lerp(0.5f, new Action<float>((p) =>
            {
                textCreditsButton.SetText("SR Credits");
                //creditsButton.transform.GetChild(2).localPosition = new Vector3(-1, 0);
            })));

            PassiveButton passiveCreditsButton = creditsButton.GetComponent<PassiveButton>();
            passiveCreditsButton.OnClick = new ButtonClickedEvent();
            passiveCreditsButton.OnClick.AddListener((Action)delegate
            {
                // do stuff
                if (popUp != null) Object.Destroy(popUp);
                AnnouncementPopUp popUpTemplate = Object.FindObjectOfType<AnnouncementPopUp>(true);
                if (popUpTemplate == null)
                {
                    Helpers.Log(LogLevel.Error, "Unable to show credits as `popUpTemplate` is unexpectedly null");
                    return;
                }
                popUp = Object.Instantiate(popUpTemplate);
                popUp.gameObject.SetActive(true);
                string creditsString = @$"<align=""center"">Special Thanks:
TheOtherRoles - Original Codebase
JustSysAdmin

";
                creditsString += $@"<size=80%> Other Credits & Resources:
Reactor - Use of API
BepInEx - Used to hook game functions

Jester - Idea for the Jester role came from Maartii
ExtraRolesAmongUs - Idea for the Engineer and Guardian role came from NotHunter101
TownOfHost - Idea for Enginner Advanced Sabotage
Among-Us-Sheriff-Mod - Idea for the Sheriff role came from Woodi-dev
Slushiegoose - Idea for the Arsonist, Scavenger, a similar Mayor role, and code snippets from Grenadiar for Nightmare
BryBry16 - For the code used for Better Polus.
Ottomated - Idea for the Morphling, Camouflager, and Parasite roles</size>";
                creditsString += "</align>";

                Announcement creditsAnnouncement = new()
                {
                    Id = "StellarCredits",
                    Language = 0,
                    Number = 502,
                    Title = "StellarRoles Credits & Resources",
                    ShortTitle = "StellarRoles Credits",
                    SubTitle = "",
                    PinState = false,
                    Date = "03.07.2025",
                    Text = creditsString,
                };

                __instance.StartCoroutine(Effects.Lerp(0.01f, new Action<float>((p) =>
                {
                    if (p == 1)
                    {
                        Il2CppSystem.Collections.Generic.List<Announcement> backup = DataManager.Player.Announcements.allAnnouncements;
                        DataManager.Player.Announcements.allAnnouncements = new();
                        popUp.Init(false);
                        DataManager.Player.Announcements.SetAnnouncements(new Announcement[] { creditsAnnouncement });
                        popUp.CreateAnnouncementList();
                        popUp.UpdateAnnouncementText(creditsAnnouncement.Number);
                        popUp.visibleAnnouncements[0].PassiveButton.OnClick.RemoveAllListeners();
                        DataManager.Player.Announcements.allAnnouncements = backup;
                    }
                })));
            });
            #endregion SR Credits

            #region RegionReset
            regionResetConfirmTemplate = SettingsB;
            GameObject buttonRegion = Object.Instantiate(AccountB, AccountB.transform.parent);
            buttonRegion.name = "DiscordButton";
            buttonRegion.transform.localPosition = new Vector3(0.87f, -1.444f, 0);
            buttonRegion.transform.FindChild("Inactive").FindChild("Icon").GetComponent<SpriteRenderer>().sprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.region.png", 240f);
            buttonRegion.transform.FindChild("Highlight").FindChild("Icon").GetComponent<SpriteRenderer>().sprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.region.png", 240f);

            TMPro.TMP_Text textRegion = buttonRegion.transform.GetComponentInChildren<TMPro.TMP_Text>();
            __instance.StartCoroutine(Effects.Lerp(0.5f, new Action<float>((p) =>
            {
                textRegion.SetText("Reset Region");
            })));
            PassiveButton passiveButtonRegionReset = buttonRegion.GetComponent<PassiveButton>();
            passiveButtonRegionReset.OnClick = new ButtonClickedEvent();
            passiveButtonRegionReset.OnClick.AddListener((Action)delegate
            {
                // do stuff
                if (popUp != null) Object.Destroy(popUp);
                AnnouncementPopUp popUpTemplate = Object.FindObjectOfType<AnnouncementPopUp>(true);
                if (popUpTemplate == null)
                {
                    Helpers.Log(LogLevel.Error, "Unable to show credits as `popUpTemplate` is unexpectedly null");
                    return;
                }
                popUp = Object.Instantiate(popUpTemplate);
                popUp.gameObject.SetActive(true);
                string PopupInitString = @$"<align=""center"">Are you sure you want to reset your Among Us regions to default? This tool should only be used if your regions are not displaying correctly. Press Confirm to reset your Among Us regions. This will also restart your Among Us client.";
                PopupInitString += $@"<size=60%> </size>";
                PopupInitString += "</align>";
                Announcement creditsAnnouncement = new()
                {
                    Id = "StellarCredits",
                    Language = 0,
                    Number = 502,
                    Title = "StellarRoles Region Reset",
                    ShortTitle = "StellarRoles Regions",
                    SubTitle = "",
                    PinState = false,
                    Date = "03.07.2025",
                    Text = PopupInitString,
                };

                __instance.StartCoroutine(Effects.Lerp(0.01f, new Action<float>((p) =>
                {
                    if (p == 1)
                    {
                        Il2CppSystem.Collections.Generic.List<Announcement> backup = DataManager.Player.Announcements.allAnnouncements;
                        DataManager.Player.Announcements.allAnnouncements = new();
                        popUp.Init(false);
                        DataManager.Player.Announcements.SetAnnouncements(new Announcement[] { creditsAnnouncement });
                        popUp.CreateAnnouncementList();
                        popUp.UpdateAnnouncementText(creditsAnnouncement.Number);
                        popUp.visibleAnnouncements[0].PassiveButton.OnClick.RemoveAllListeners();
                        DataManager.Player.Announcements.allAnnouncements = backup;
                    }
                })));

                string ActionCompleteString = @$"<align=""center"">Region info reset, game will close when pop is dismissed...";
                ActionCompleteString += $@"<size=60%> </size>";
                ActionCompleteString += "</align>";


                GameObject regionResetConfirmationButton = Object.Instantiate(regionResetConfirmTemplate, popUp.transform);
                regionResetConfirmationButton.transform.localPosition = new Vector3(0.75f, -1.25f, -5);
                TMPro.TMP_Text textRegionResetConfirmationButton = regionResetConfirmationButton.transform.GetComponentInChildren<TMPro.TMP_Text>();
                __instance.StartCoroutine(Effects.Lerp(0.1f, new Action<float>((p) =>
                {
                    textRegionResetConfirmationButton.SetText("Confirm");
                })));
                PassiveButton passiveRegionResetConfirmationButtom = regionResetConfirmationButton.GetComponent<PassiveButton>();
                passiveRegionResetConfirmationButtom.gameObject.SetActive(true);
                passiveRegionResetConfirmationButtom.OnClick.RemoveAllListeners();
                passiveRegionResetConfirmationButtom.OnClick = new ButtonClickedEvent();
                passiveRegionResetConfirmationButtom.OnClick.AddListener((Action)(() =>
                {
                    BlackScreenFix.BeginFix();
                    string regionJsonPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).Replace("Local", "LocalLow"), "InnerSloth\\Among Us\\regionInfo.json");
                    if (System.IO.File.Exists(regionJsonPath)) System.IO.File.Delete(regionJsonPath);
                    popUp.AnnouncementBodyText.SetText(ActionCompleteString);
                    passiveRegionResetConfirmationButtom.gameObject.SetActive(false);
                    __instance.StartCoroutine(Effects.Lerp(0.5f, new Action<float>((p) =>
                    {
                        if (p == 1)
                        Application.Quit();
                    })));
                }));
                popUp.OnDismissed = new Action(() => passiveRegionResetConfirmationButtom.gameObject.SetActive(false));

            });
            #endregion RegionReset

            #region FriendsList Color
/*            GameObject FriendsListManager = GameObject.Find("FriendsListManager");
            if (FriendsListManager != null)
            {
                Helpers.Log("Found FriendsListManager");
            }

            if (FriendsListManager != null)
            {
                Transform friendsListButton = FriendsListManager.transform.FindChild("Friends List Button").FindChild("Friends List Button");
                friendsListButton.FindChild("Highlight").gameObject.GetComponent<SpriteRenderer>().color = Color.blue;
                friendsListButton.FindChild("Tab").gameObject.GetComponent<SpriteRenderer>().color = Color.blue;
            }*/
            #endregion FriendsList Color

            #region Account Color
/*            GameObject account = GameObject.Find("AccountManager");

            if (account != null)
            {
                Helpers.Log("Found AccountManager");

                SpriteRenderer accountoutline = account.transform.FindChild("AccountTab").FindChild("AccountWindow").FindChild("Tab").FindChild("AccountTab").gameObject.GetComponent<SpriteRenderer>();
                accountoutline.color = Color.blue;
            }*/
            #endregion Account Color
        }

        private static void Postfix()
        {
            CustomVisorLoader.LaunchVisorFetcher();
            CustomHatLoader.LaunchHatFetcher();
            CustomNameplateLoader.LaunchNameplateFetcher();
            CustomOptionDefaultSettings.CreatePresets();
        }
    }
}