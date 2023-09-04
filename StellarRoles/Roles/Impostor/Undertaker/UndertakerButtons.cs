using HarmonyLib;
using StellarRoles.Objects;

namespace StellarRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
    public static class UndertakerButtons
    {
        private static bool Initialized;

        public static CustomButton UndertakerDragButton { get; set; }

        public static void InitUndertakerButtons()
        {
            DragButton();

            Initialized = true;
            SetUndertakerCooldowns();
        }

        public static void DragButton()
        {
            UndertakerDragButton = new CustomButton(
                () =>
                {
                    if (Undertaker.DeadBodyDragged == null && Undertaker.DeadBodyCurrentTarget != null)
                    {
                        RPCProcedure.Send(CustomRPC.DragBody, Undertaker.DeadBodyCurrentTarget.ParentId);
                        RPCProcedure.DragBody(Undertaker.DeadBodyCurrentTarget.ParentId);
                        Undertaker.DeadBodyCurrentTarget = null;
                    }
                    else
                    {
                        RPCProcedure.Send(CustomRPC.DropBody);
                        Undertaker.DeadBodyDragged = null;
                        UndertakerDragButton.Timer = UndertakerDragButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer);
                    }
                    RPCProcedure.Send(CustomRPC.PsychicAddCount);

                },
                () => Undertaker.Player == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead,
                () =>
                {
                    if (Undertaker.DeadBodyDragged != null)
                    {
                        UndertakerDragButton.Sprite = Undertaker.GetDropButtonSprite();
                        Helpers.ShowTargetNameOnButtonExplicit(null, UndertakerDragButton, "Drop");
                        return true;
                    }
                    else
                    {
                        UndertakerDragButton.Sprite = Undertaker.GetDragButtonSprite();
                        Helpers.ShowTargetNameOnButtonExplicit(null, UndertakerDragButton, "Drag");
                        return PlayerControl.LocalPlayer.CanMove && Undertaker.DeadBodyCurrentTarget && !Impostor.IsRoleAblilityBlocked();
                    }

                },
                () => { UndertakerDragButton.Timer = UndertakerDragButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer); },
                Undertaker.GetDragButtonSprite(),
                CustomButton.ButtonPositions.UpperRow3,
                "ActionQuaternary",
                false,
                0f,
                () => { }
            );
        }

        public static void SetUndertakerCooldowns()
        {
            if (!Initialized)
            {
                InitUndertakerButtons();
            }
            UndertakerDragButton.MaxTimer = Undertaker.DraggingCooldown;
        }

        public static void Postfix()
        {
            Initialized = false;
            InitUndertakerButtons();
        }
    }
}
