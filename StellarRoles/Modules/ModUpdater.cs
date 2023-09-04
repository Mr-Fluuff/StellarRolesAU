using AmongUs.Data;
using Assets.InnerNet;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Unity.IL2CPP;
using BepInEx.Unity.IL2CPP.Utils;
using Mono.Cecil;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Twitch;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Action = System.Action;
using IntPtr = System.IntPtr;
using Version = SemanticVersioning.Version;

namespace StellarRoles.Modules
{
    public class ModUpdateBehaviour : MonoBehaviour
    {
        public static readonly bool CheckForSubmergedUpdates = true;
        public static bool showPopUp = true;
        public static bool updateInProgress = false;

        public static ModUpdateBehaviour Instance { get; private set; }
        public ModUpdateBehaviour(IntPtr ptr) : base(ptr) { }
        public class UpdateData
        {
            public string Content;
            public string Tag;
            public string TimeString;
            public JObject Request;
            public Version Version => Version.Parse(Tag);

            public UpdateData(JObject data)
            {
                Tag = data["tag_name"]?.ToString().TrimStart('v');
                Content = data["body"]?.ToString();
                TimeString = DateTime.FromBinary(((Il2CppSystem.DateTime)data["published_at"]).ToBinaryRaw()).ToString();
                Request = data;
            }

            public bool IsNewer(Version version)
            {
                if (!Version.TryParse(Tag, out Version myVersion)) return false;
                return myVersion.BaseVersion() > version.BaseVersion();
            }
        }

        public UpdateData SRUpdate;
        public UpdateData SubmergedUpdate;

        [HideFromIl2Cpp]
        public UpdateData RequiredUpdateData => SRUpdate ?? SubmergedUpdate;

        public void Awake()
        {
            if (Instance) Destroy(this);
            Instance = this;

            SceneManager.add_sceneLoaded((Action<Scene, LoadSceneMode>)OnSceneLoaded);
            StartCoroutine(Effects.Lerp(0.2f, new Action<float>((p) =>
            {
                if (p == 1)
                {
                    this.StartCoroutine(CoCheckUpdates());
                }
            })));

            foreach (string file in Directory.GetFiles(Paths.PluginPath, "*.old"))
            {
                File.Delete(file);
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (updateInProgress || scene.name != "MainMenu") return;
            if (RequiredUpdateData is null)
            {
                showPopUp = false;
                return;
            }

            GameObject template = GameObject.Find("ExitGameButton");
            if (!template) return;

            GameObject button = Instantiate(template, null);
            Transform buttonTransform = button.transform;
            button.GetComponent<AspectPosition>().anchorPoint = new Vector2(0.458f, 0.124f);

            PassiveButton passiveButton = button.GetComponent<PassiveButton>();
            passiveButton.OnClick = new Button.ButtonClickedEvent();
            passiveButton.OnClick.AddListener((Action)(() =>
            {
                this.StartCoroutine(CoUpdate());
                button.SetActive(false);
            }));

            TMPro.TMP_Text text = button.transform.GetComponentInChildren<TMPro.TMP_Text>();
            string t = "Update";
            if (SRUpdate is null && SubmergedUpdate is not null) t = SubmergedCompatibility.Loaded ? $"Update\nSubmerged" : $"Download\nSubmerged";

            StartCoroutine(Effects.Lerp(0.1f, (Action<float>)(p => text.SetText(t))));

            passiveButton.OnMouseOut.AddListener((Action)(() => text.color = Color.white));
            passiveButton.OnMouseOver.AddListener((Action)(() => text.color = Color.red));

            bool isSubmerged = SRUpdate == null;
            string announcement = $"<size=150%>A new {(isSubmerged ? "Submerged" : "StellarRoles")} update to {(isSubmerged ? SubmergedUpdate.Tag : SRUpdate.Tag)} is available</size>\n{(isSubmerged ? SubmergedUpdate.Content : SRUpdate.Content)}";
            MainMenuManager mgr = FindObjectOfType<MainMenuManager>(true);

            if (!isSubmerged)
            {
                try
                {
                    string updateVersion = SRUpdate.Content[^5..];
                    if (Version.Parse(StellarRolesPlugin.VersionString).BaseVersion() < Version.Parse(updateVersion).BaseVersion())
                    {
                        passiveButton.OnClick.RemoveAllListeners();
                        passiveButton.OnClick = new Button.ButtonClickedEvent();
                        passiveButton.OnClick.AddListener((Action)(() =>
                        {
                            mgr.StartCoroutine(CoShowAnnouncement($"<size=150%>A MANUAL UPDATE IS REQUIRED</size>"));
                        }));
                    }
                }
                catch (Exception ex)
                {
                    Helpers.Log(LogLevel.Error, "Auto Updater version parsing failed: " + ex.StackTrace);
                }

            }
            if (isSubmerged && !SubmergedCompatibility.Loaded) showPopUp = false;
            if (showPopUp) mgr.StartCoroutine(CoShowAnnouncement(announcement, shortTitle: isSubmerged ? "Submerged Update" : "Stellar Update", date: isSubmerged ? SubmergedUpdate.TimeString : SRUpdate.TimeString));

        }

        [HideFromIl2Cpp]
        public IEnumerator CoUpdate()
        {
            updateInProgress = true;
            bool isSubmerged = SRUpdate is null;
            string updateName = isSubmerged ? "Submerged" : "StellarRoles";

            GenericPopup popup = Instantiate(TwitchManager.Instance.TwitchPopup);
            popup.TextAreaTMP.fontSize *= 0.7f;
            popup.TextAreaTMP.enableAutoSizing = false;

            popup.Show();

            GameObject button = popup.transform.GetChild(2).gameObject;
            button.SetActive(false);
            popup.TextAreaTMP.text = $"Updating {updateName}\nPlease wait...";

            Task<bool> download = Task.Run(DownloadUpdate);
            while (!download.IsCompleted) yield return null;

            button.SetActive(true);
            popup.TextAreaTMP.text = download.Result ? $"{updateName}\nupdated successfully\nPlease restart the game." : "Update wasn't successful\nTry again later,\nor update manually.";
        }

        private static int announcementNumber = 504;

        [HideFromIl2Cpp]
        public IEnumerator CoShowAnnouncement(string announcement, bool show = true, string shortTitle = "SR Update", string title = "", string date = "")
        {
            MainMenuManager mgr = FindObjectOfType<MainMenuManager>(true);
            AnnouncementPopUp popUpTemplate = FindObjectOfType<AnnouncementPopUp>(true);
            if (popUpTemplate == null)
            {
                Helpers.Log(LogLevel.Warning, "Couldn't show credits, `popUpTemplate` is unexpectedly null");
                yield return null;
            }
            AnnouncementPopUp popUp = Instantiate(popUpTemplate);
            popUp.gameObject.SetActive(true);

            Announcement creditsAnnouncement = new()
            {
                Id = "SRAnnouncement",
                Language = 0,
                Number = announcementNumber++,
                Title = title == "" ? "Stellar Roles Announcement" : title,
                ShortTitle = shortTitle,
                SubTitle = "",
                PinState = false,
                Date = date == "" ? DateTime.Now.Date.ToString() : date,
                Text = announcement,
            };
            mgr.StartCoroutine(Effects.Lerp(0.1f, new Action<float>((p) =>
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

        }

        [HideFromIl2Cpp]
        public static IEnumerator CoCheckUpdates()
        {
            Task<UpdateData> torUpdateCheck = Task.Run(() => Instance.GetGithubUpdate("Mr-Fluuff", "StellarRolesAU"));
            while (!torUpdateCheck.IsCompleted) yield return null;
            Helpers.Log($"Task Running");
            if (torUpdateCheck.Result != null && torUpdateCheck.Result.IsNewer(Version.Parse(StellarRolesPlugin.VersionString)))
            {
                Instance.SRUpdate = torUpdateCheck.Result;
            }

            if (CheckForSubmergedUpdates)
            {
                Task<UpdateData> submergedUpdateCheck = Task.Run(() => Instance.GetGithubUpdate("SubmergedAmongUs", "Submerged"));
                while (!submergedUpdateCheck.IsCompleted) yield return null;
                if (submergedUpdateCheck.Result != null && (!SubmergedCompatibility.Loaded || submergedUpdateCheck.Result.IsNewer(SubmergedCompatibility.Version)))
                {
                    Instance.SubmergedUpdate = submergedUpdateCheck.Result;
                    if (Instance.SubmergedUpdate.Tag.Equals("2022.10.26")) Instance.SubmergedUpdate = null;
                }
            }

            Instance.OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
        }

        [HideFromIl2Cpp]
        public async Task<UpdateData> GetGithubUpdate(string owner, string repo)
        {
            HttpClient client = new();
            client.DefaultRequestHeaders.Add("User-Agent", "StellarRoles Updater");

            try
            {
                HttpResponseMessage req = await client.GetAsync($"https://api.github.com/repos/{owner}/{repo}/releases/latest", HttpCompletionOption.ResponseContentRead);

                if (!req.IsSuccessStatusCode) return null;

                string dataString = await req.Content.ReadAsStringAsync();
                JObject data = JObject.Parse(dataString);
                return new UpdateData(data);
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        private bool TryUpdateSubmergedInternally()
        {
            if (SubmergedUpdate == null) return false;
            try
            {
                if (!SubmergedCompatibility.LoadedExternally) return false;
                Assembly thisAsm = Assembly.GetCallingAssembly();
                string resourceName = thisAsm.GetManifestResourceNames().FirstOrDefault(s => s.EndsWith("Submerged.dll"));
                if (resourceName == default) return false;

                using Stream submergedStream = thisAsm.GetManifestResourceStream(resourceName)!;
                AssemblyDefinition asmDef = AssemblyDefinition.ReadAssembly(submergedStream, TypeLoader.ReaderParameters);
                TypeDefinition pluginType = asmDef.MainModule.Types.FirstOrDefault(t => t.IsSubtypeOf(typeof(BasePlugin)));
                PluginInfo info = IL2CPPChainloader.ToPluginInfo(pluginType, "");
                if (SubmergedUpdate.IsNewer(info.Metadata.Version)) return false;
                File.Delete(SubmergedCompatibility.Assembly.Location);

            }
            catch (Exception ex)
            {
                Helpers.Log(LogLevel.Error, "Error attempting to update Submerged: " + ex.StackTrace);
                return false;
            }
            return true;
        }


        [HideFromIl2Cpp]
        public async Task<bool> DownloadUpdate()
        {
            bool isSubmerged = SRUpdate is null;
            if (isSubmerged && TryUpdateSubmergedInternally()) return true;
            UpdateData data = isSubmerged ? SubmergedUpdate : SRUpdate;

            HttpClient client = new();
            client.DefaultRequestHeaders.Add("User-Agent", "StellarRoles Updater");

            JToken assets = data.Request["assets"];
            string downloadURI = "";
            for (JToken current = assets.First; current != null; current = current.Next)
            {
                string browser_download_url = current["browser_download_url"]?.ToString();
                if (browser_download_url != null && current["content_type"] != null)
                {
                    if (current["content_type"].ToString().Equals("application/x-msdownload") &&
                        browser_download_url.EndsWith(".dll"))
                    {
                        downloadURI = browser_download_url;
                        break;
                    }
                }
            }

            if (downloadURI.Length == 0) return false;

            HttpResponseMessage res = await client.GetAsync(downloadURI, HttpCompletionOption.ResponseContentRead);
            string filePath = Path.Combine(Paths.PluginPath, isSubmerged ? "Submerged.dll" : "StellarRoles.dll");
            if (File.Exists(filePath + ".old")) File.Delete(filePath + ".old");
            if (File.Exists(filePath)) File.Move(filePath, filePath + ".old");

            await using Stream responseStream = await res.Content.ReadAsStreamAsync();
            await using FileStream fileStream = File.Create(filePath);
            await responseStream.CopyToAsync(fileStream);

            return true;
        }
    }
}
