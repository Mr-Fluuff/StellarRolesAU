using HarmonyLib;
using System;
using UnityEngine;

namespace StellarRoles
{
    [HarmonyPatch]
    public static class Impostor
    {
        public static Color color = Palette.ImpostorRed;
        public static PlayerControl CurrentTarget = null;
        public static bool ImpChatOn = false;
        public static Sprite __ImpChatSprite = null;
        public static bool ChatEnabled => CustomOptionHolder.EnableImpChat.GetBool();

        public static void ClearAndReload()
        {
            CurrentTarget = null;
            ImpChatOn = false;
            if (HudManager.Instance != null)
            {
                HudManager.Instance.Chat.chatButton.transform.FindChild("Inactive").GetComponent<SpriteRenderer>().color = Color.white;
                HudManager.Instance.Chat.chatButton.transform.FindChild("Active").GetComponent<SpriteRenderer>().color = Color.white;
                /*                var ChatScreenContainer = GameObject.Find("ChatScreenContainer");
                                var Background = ChatScreenContainer.transform.FindChild("Background");
                                Background.GetComponent<SpriteRenderer>().color = Color.white;*/
            }
        }

        public static Sprite GetImpChatSprite()
        {
            return __ImpChatSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.ImpChatButton.png", 150f);
        }

        public static bool IsRoleAblilityBlocked()
        {
            return Helpers.IsCommsActive() && MapOptions.ImposterAbiltiesRoleBlock;
        }

        [HarmonyPatch(typeof(ChatController), nameof(ChatController.Toggle))]
        public static class ImpChatScreen
        {
            public static void Postfix(ChatController __instance)
            {
                try
                {
                    if (__instance.IsOpenOrOpening)
                    {
                        var ChatScreenContainer = GameObject.Find("ChatScreenContainer");
                        var Background = ChatScreenContainer.transform.FindChild("Background");

                        if (ImpChatOn)
                        {
                            var color = Palette.ImpostorRed;
                            color.a = 0.6f;
                            Background.GetComponent<SpriteRenderer>().color = color;
                            if (MeetingHud.Instance)
                            {
                                ChatScreenContainer.transform.localPosition = new Vector3(-4.19f, -2.236f, 0);
                            }
                            else
                            {
                                ChatScreenContainer.transform.localPosition = new Vector3(-3.49f, -2.236f, 0);
                            }
                        }
                        else
                        {
                            Background.GetComponent<SpriteRenderer>().color = Color.white;
                            ChatScreenContainer.transform.localPosition = new Vector3(-3.49f, -2.236f, 0);
                        }
                    }
                    else
                    {
                        ImpChatOn = false;
                    }
                }
                catch { }
            }
        }

        [HarmonyPatch(typeof(ChatController), nameof(ChatController.AddChat))]
        public static class AddChat
        {
            public static bool Prefix(ChatController __instance, [HarmonyArgument(0)] PlayerControl sourcePlayer, ref string chatText)
            {
                PlayerControl localPlayer = PlayerControl.LocalPlayer;

                if (chatText.StartsWith("[ImpChat]"))
                {
                    return localPlayer.Data.Role.IsImpostor || localPlayer.Data.IsDead;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(ChatController), nameof(ChatController.Toggle))]
        public static class ImpChatRedBubbles
        {
            public static void Postfix(ChatController __instance)
            {
                if (!PlayerControl.LocalPlayer.Data.Role.IsImpostor) return;

                try
                {
                    __instance.StartCoroutine(Effects.Lerp(0.6f, new Action<float>((p) =>
                    {
                        if (__instance.IsOpenOrOpening)
                        {
                            var activeChildren = __instance.scroller.transform.GetChild(0).GetComponentsInChildren<ChatBubble>();
                            for (int i = 0; i < activeChildren.Count; i++)
                            {
                                ChatBubble chatBubble = activeChildren[i];
                                if (chatBubble != null && chatBubble.TextArea.text.StartsWith("[ImpChat]"))
                                {
                                    var color = chatBubble.Background.color = ImpChatOn ? Color.white : Palette.ImpostorRed;
                                    color.a = ImpChatOn ? 1f : 0.5f;
                                }
                            }
                        }
                    })));
                }
                catch { }
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSendChat))]
        public static class SendChat
        {
            public static bool Prefix(PlayerControl __instance, [HarmonyArgument(0)] ref string chatText)
            {
                if (PlayerControl.LocalPlayer.Data.Role.IsImpostor && ImpChatOn)
                {
                    chatText = "[ImpChat]" + "\n" + chatText;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        private static class EnableChat
        {
            private static bool changed = false;
            private static void Postfix(HudManager __instance)
            {
                if (!Helpers.GameStarted || !PlayerControl.LocalPlayer.Data.Role.IsImpostor)
                {
                    ImpChatOn = false;
                    return;
                }
                try
                {
                    if (Spy.Player == null)
                    {
                        if (!__instance.Chat.isActiveAndEnabled && PlayerControl.LocalPlayer.IsTeamCultist())
                            __instance.Chat.SetVisible(true);

                        if (ChatEnabled || PlayerControl.LocalPlayer.IsTeamCultist())
                        {
                            if (__instance.Chat.isActiveAndEnabled)
                            {
                                if (!MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead && changed)
                                {
                                    ImpChatOn = true;
                                    changed = false;
                                    __instance.Chat.chatButton.transform.FindChild("Inactive").GetComponent<SpriteRenderer>().color = Palette.ImpostorRed;
                                    __instance.Chat.chatButton.transform.FindChild("Active").GetComponent<SpriteRenderer>().color = Palette.ImpostorRed;

                                }
                                else if (!changed)
                                {
                                    __instance.Chat.chatButton.transform.FindChild("Active").GetComponent<SpriteRenderer>().color = Color.white;
                                    __instance.Chat.chatButton.transform.FindChild("Inactive").GetComponent<SpriteRenderer>().color = Color.white;
                                    changed = true;
                                    ImpChatOn = false;
                                }
                            }
                        }
                    }
                }
                catch
                {

                }
            }
        }
    }
}
