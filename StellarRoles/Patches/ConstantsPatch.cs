using HarmonyLib;

namespace StellarRoles.Patches
{
    [HarmonyPatch(typeof(Constants), nameof(Constants.IsVersionModded))]
    public static class ConstantsPatch
    {
        public static void Postfix(ref bool __result)
        {
            //__result = true;
        }
    }
}
