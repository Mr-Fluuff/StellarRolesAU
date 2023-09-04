using AmongUs.Data;
using Assets.InnerNet;
using HarmonyLib;
using System;
using UnityEngine;
using static UnityEngine.UI.Button;
using Object = UnityEngine.Object;

namespace StellarRoles.Modules
{
    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
    public class MainMenuPatch
    {
        //private static GameObject regionResetTemplate;
        //private static GameObject regionResetConfirmTemplate;
        private static AnnouncementPopUp popUp;

        private static void Prefix(MainMenuManager __instance)
        {
            CustomVisorLoader.LaunchVisorFetcher();
            CustomHatLoader.LaunchHatFetcher();

            #region DiscordButton
            GameObject template = GameObject.Find("ExitGameButton");
            GameObject template2 = GameObject.Find("CreditsButton");
            if (template == null || template2 == null) return;
            template.transform.localScale = new Vector3(0.42f, 0.84f, 0.84f);
            template.GetComponent<AspectPosition>().anchorPoint = new Vector2(0.625f, 0.5f);
            template.transform.FindChild("FontPlacer").transform.localScale = new Vector3(1.8f, 0.9f, 0.9f);
            template.transform.FindChild("FontPlacer").transform.localPosition = new Vector3(-1.1f, 0f, 0f);

            template2.transform.localScale = new Vector3(0.42f, 0.84f, 0.84f);
            template2.GetComponent<AspectPosition>().anchorPoint = new Vector2(0.378f, 0.5f);
            template2.transform.FindChild("FontPlacer").transform.localScale = new Vector3(1.8f, 0.9f, 0.9f);
            template2.transform.FindChild("FontPlacer").transform.localPosition = new Vector3(-1.1f, 0f, 0f);

            GameObject buttonDiscord = Object.Instantiate(template, template.transform.parent);
            buttonDiscord.transform.localScale = new Vector3(0.42f, 0.84f, 0.84f);
            buttonDiscord.GetComponent<AspectPosition>().anchorPoint = new Vector2(0.543f, 0.5f);

            TMPro.TMP_Text textDiscord = buttonDiscord.transform.GetComponentInChildren<TMPro.TMP_Text>();
            __instance.StartCoroutine(Effects.Lerp(0.5f, new Action<float>((p) =>
            {
                textDiscord.SetText(" SR Discord");
            })));
            PassiveButton passiveButtonDiscord = buttonDiscord.GetComponent<PassiveButton>();
            passiveButtonDiscord.OnClick = new ButtonClickedEvent();
            passiveButtonDiscord.OnClick.AddListener((Action)(() => Application.OpenURL("https://discord.gg/7UcvM9CFdY")));

            #endregion DiscordButton

            #region SR Credits
            // TOR credits button
            if (template == null) return;

            GameObject creditsButton = Object.Instantiate(template, template.transform.parent);
            creditsButton.transform.localScale = new Vector3(0.42f, 0.84f, 0.84f);
            creditsButton.GetComponent<AspectPosition>().anchorPoint = new Vector2(0.461f, 0.5f);

            TMPro.TMP_Text textCreditsButton = creditsButton.transform.GetComponentInChildren<TMPro.TMP_Text>();
            __instance.StartCoroutine(Effects.Lerp(0.5f, new Action<float>((p) =>
            {
                textCreditsButton.SetText("SR Credits");
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
                creditsString += $@"<size=60%> Other Credits & Resources:
Reactor - Use snippets of code
BepInEx - Used to hook game functions
Essentials - Custom game options by DorCoMaNdO:

Jester - Idea for the Jester role came from Maartii
ExtraRolesAmongUs - Idea for the Engineer and Guardian role came from NotHunter101
TownOfHost - Idea for Enginner Advanced Sabotage
Among-Us-Sheriff-Mod - Idea for the Sheriff role came from Woodi-dev
TownOfUs - Idea for the Arsonist and similar Mayor role came from Slushiegoose and code snippets from Grenadiar for Nightmare
BryBry16 - For the code used for Better Polus.
Ottomated - Idea for the Morphling and Camouflager role came from Ottomated
Goose-Goose-Duck - Idea for the Scavenger role came from Slushiegoose</size>";
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
                    Date = "03.07.2023",
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
            /* //Reset Region Json
             regionResetTemplate = GameObject.Find("InventoryButton");
             regionResetConfirmTemplate = GameObject.Find("ExitGameButton");
             if (regionResetTemplate == null) return;
             GameObject regionResetButton = Object.Instantiate(regionResetTemplate, regionResetTemplate.transform.parent);


             PassiveButton passiveRegionResetButton = regionResetButton.GetComponent<PassiveButton>();

             SpriteRenderer spriteRegionResetButton = regionResetButton.GetComponent<SpriteRenderer>();

             spriteRegionResetButton.sprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.RegionReset.png", 75f);

             passiveRegionResetButton.OnClick = new ButtonClickedEvent();

             passiveRegionResetButton.OnClick.AddListener((Action)delegate
             {
                 // do stuff
                 if (popUp != null) Object.Destroy(popUp);
                 popUp = Object.Instantiate(Object.FindObjectOfType<AnnouncementPopUp>(true));
                 popUp.gameObject.SetActive(true);
                 string PopupInitString = @$"<align=""center"">Are you sure you want to reset your Among Us regions to default? This tool should only be used if your regions are not displaying correctly. Press Confirm to reset your Among Us regions. This will also restart your Among Us client.";
                 PopupInitString += $@"<size=60%> </size>";
                 PopupInitString += "</align>";
                 Assets.InnerNet.Announcement resetRegionsAnnoucement = new()
                 {
                     Id = "ResetRegionsConfirm",
                     Language = 0,
                     Number = 500,
                     Title = "Reset Region File",
                     ShortTitle = "Regions",
                     SubTitle = "",
                     PinState = false,
                     Date = DateTime.Now.ToString(),
                     Text = PopupInitString
                 };
                 string ActionCompleteString = @$"<align=""center"">Region info reset, game will close when pop is dismissed...";
                 ActionCompleteString += $@"<size=60%> </size>";
                 ActionCompleteString += "</align>";

                 __instance.StartCoroutine(Effects.Lerp(0.01f, new Action<float>((p) =>
                 {
                     if (p == 1)
                     {
                         Il2CppSystem.Collections.Generic.List<Assets.InnerNet.Announcement> backup = DataManager.Player.Announcements.allAnnouncements;
                         popUp.Init(false);
                         DataManager.Player.Announcements.allAnnouncements = new();
                         DataManager.Player.Announcements.allAnnouncements.Insert(0, resetRegionsAnnoucement);
                         foreach (AnnouncementPanel item in popUp.visibleAnnouncements) Object.Destroy(item);
                         foreach (AnnouncementPanel item in Object.FindObjectsOfType<AnnouncementPanel>())
                         {
                             if (item != popUp.ErrorPanel) Object.Destroy(item.gameObject);
                         }
                         popUp.CreateAnnouncementList();
                         popUp.visibleAnnouncements[0].PassiveButton.OnClick.RemoveAllListeners();
                         DataManager.Player.Announcements.allAnnouncements = backup;
                         TMPro.TextMeshPro titleText = GameObject.Find("Title_Text").GetComponent<TMPro.TextMeshPro>();
                         if (titleText != null) titleText.text = "";
                     }
                 })));
                 GameObject regionResetConfirmationButton = Object.Instantiate(regionResetConfirmTemplate, null);
                 regionResetConfirmationButton.transform.localPosition = new Vector3(popUp.transform.localPosition.x + .75f, popUp.transform.localPosition.y + -1.25f, popUp.transform.localPosition.z - 1);
                 TMPro.TMP_Text textRegionResetConfirmationButton = regionResetConfirmationButton.transform.GetChild(0).GetComponent<TMPro.TMP_Text>();
                 __instance.StartCoroutine(Effects.Lerp(0.1f, new Action<float>((p) =>
                 {
                     textRegionResetConfirmationButton.SetText("Confirm");
                 })));
                 SpriteRenderer spriteRegionResetConfirmationButton = regionResetConfirmationButton.GetComponent<SpriteRenderer>();
                 PassiveButton passiveRegionResetConfirmationButtom = regionResetConfirmationButton.GetComponent<PassiveButton>();
                 passiveRegionResetConfirmationButtom.gameObject.SetActive(true);
                 passiveRegionResetConfirmationButtom.OnClick.RemoveAllListeners();
                 passiveRegionResetConfirmationButtom.OnClick = new ButtonClickedEvent();
                 passiveRegionResetConfirmationButtom.OnClick.AddListener((Action)(() =>
                 {
                     string regionJsonPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).Replace("Local", "LocalLow"), "InnerSloth\\Among Us\\regionInfo.json");
                     if (System.IO.File.Exists(regionJsonPath)) System.IO.File.Delete(regionJsonPath);
                     popUp.AnnouncementBodyText.SetText(ActionCompleteString);
                     passiveRegionResetConfirmationButtom.gameObject.SetActive(false);
                     __instance.StartCoroutine(Effects.Lerp(0.5f, new Action<float>((p) =>
                     {
                         Application.Quit();
                     })));
                 }));
                 popUp.OnDismissed = new Action(() => passiveRegionResetConfirmationButtom.gameObject.SetActive(false));

             });*/
            #endregion RegionReset

            #region FriendsList Color
            GameObject FriendsListManager = GameObject.Find("FriendsListManager");
            if (FriendsListManager != null)
            {
                Transform friendsListButton = FriendsListManager.transform.FindChild("Friends List Button").FindChild("Friends List Button");
                friendsListButton.FindChild("Highlight").gameObject.GetComponent<SpriteRenderer>().color = Color.blue;
                friendsListButton.FindChild("Tab").gameObject.GetComponent<SpriteRenderer>().color = Color.blue;
            }
            #endregion FriendsList Color

            #region Account Color
            GameObject account = GameObject.Find("AccountManager");
            if (account != null)
            {
                SpriteRenderer accountoutline = account.transform.FindChild("AccountTab").FindChild("AccountWindow").FindChild("Tab").FindChild("AccountTab").gameObject.GetComponent<SpriteRenderer>();
                accountoutline.color = Color.blue;
            }
            #endregion Account Color
        }
    }
}