using Chara;
using Il2CppSystem.IO;
using BepInEx.Logging;
using HarmonyLib;
using RG.Scene;

namespace RGExtendedSave
{
    internal class ExtendedSaveHooks
    {
        private static ManualLogSource Log = RGExtendedSavePlugin.Log;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(CharaCustom.CharaCustom), nameof(CharaCustom.CharaCustom.OnDestroy))]
        [HarmonyPatch(typeof(ActionScene), nameof(ActionScene.OnDestroy))]
        [HarmonyPatch(typeof(HomeScene), nameof(HomeScene.OnDestroy))]
        [HarmonyPatch(typeof(HScene), nameof(HScene.OnDestroy))]
        private static void SceneDestroyPre()
        {
            ExtendedSave.ClearData();
        }
    }
}
