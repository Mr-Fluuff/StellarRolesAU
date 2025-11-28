using HarmonyLib;
using Rewired;
using Rewired.Data;

namespace StellarRoles.Patches;

[HarmonyPatch(typeof(InputManager_Base), nameof(InputManager_Base.Awake))]
static class ControllerPatch
{
    private static void Prefix(InputManager_Base __instance)
    {
        __instance.userData.RegisterBind("SecondAbility", "Second Ability", KeyboardKeyCode.G);
        __instance.userData.RegisterBind("Zoom", "Zoom In/Out", KeyboardKeyCode.KeypadPlus);
        __instance.userData.RegisterBind("Help", "Help Button", KeyboardKeyCode.KeypadMinus);
        __instance.userData.RegisterBind("ParasiteUp", "Parasite Up", KeyboardKeyCode.I);
        __instance.userData.RegisterBind("ParasiteDown", "Parasite Down", KeyboardKeyCode.K);
        __instance.userData.RegisterBind("ParasiteLeft", "Parasite Left", KeyboardKeyCode.J);
        __instance.userData.RegisterBind("ParasiteRight", "Parasite Right", KeyboardKeyCode.L);
    }

    private static int RegisterBind(this UserData self, string name, string description, KeyboardKeyCode keycode, int elementIdentifierId = -1, int category = 0, InputActionType type = InputActionType.Button)
    {
        self.AddAction(category);
        InputAction action = self.GetAction(self.actions.Count - 1)!;

        action.name = name;
        action.descriptiveName = description;
        action.categoryId = category;
        action.type = type;
        action.userAssignable = true;

        ActionElementMap a = new()
        {
            _elementIdentifierId = elementIdentifierId,
            _actionId = action.id,
            _elementType = ControllerElementType.Button,
            _axisContribution = Pole.Positive,
            _keyboardKeyCode = keycode,
            _modifierKey1 = ModifierKey.None,
            _modifierKey2 = ModifierKey.None,
            _modifierKey3 = ModifierKey.None
        };
        self.keyboardMaps[0].actionElementMaps.Add(a);
        self.joystickMaps[0].actionElementMaps.Add(a);

        return action.id;
    }
}
