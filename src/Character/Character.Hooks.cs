using BepInEx.Logging;
using HarmonyLib;
using Chara;
using Il2CppSystem.IO;
using CharaCustom;

namespace RGExtendedSave.Character
{
    class Hooks
    {
        private static ManualLogSource Log = RGExtendedSavePlugin.Log;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(CustomCharaFileInfoAssist), nameof(CustomCharaFileInfoAssist.AddList))]
        [HarmonyPatch(typeof(CvsO_CharaLoad), nameof(CvsO_CharaLoad.UpdateCharasList))]
        [HarmonyPatch(typeof(CvsO_CharaSave), nameof(CvsO_CharaSave.UpdateCharasList))]
        private static void AddListPre()
        {
            ExtendedSave.LoadEventsEnabled = false;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(CustomCharaFileInfoAssist), nameof(CustomCharaFileInfoAssist.AddList))]
        [HarmonyPatch(typeof(CvsO_CharaLoad), nameof(CvsO_CharaLoad.UpdateCharasList))]
        [HarmonyPatch(typeof(CvsO_CharaSave), nameof(CvsO_CharaSave.UpdateCharasList))]
        private static void AddListPost()
        {
            ExtendedSave.LoadEventsEnabled = true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(CustomControl), nameof(CustomControl.InitializeUI))]
        private static void CustomAwakePost(CustomControl __instance)
        {
            Patches.CustomCtrl = __instance; 
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(CharaCustom.CharaCustom), nameof(CharaCustom.CharaCustom.OnDestroy))]
        private static void CustomDestroyPost()
        {
            Patches.CustomCtrl = null;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ChaFile), nameof(ChaFile.LoadFile), new[] { typeof(BinaryReader), typeof(int), typeof(bool), typeof(bool) })]
        private static void LoadFilePre(ChaFile __instance, ref bool __result, BinaryReader br, bool noLoadPNG = false, bool noLoadStatus = true)
        {
            Patches.PreLoad();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ChaFile), nameof(ChaFile.LoadFile), new[] { typeof(BinaryReader), typeof(int), typeof(bool), typeof(bool) })]
        private static void LoadFilePost(ChaFile __instance, BinaryReader br, bool __result)
        {
            if (__result && ExtendedSave.LoadEventsEnabled)
            {
                Patches.LoadExtendedData(__instance, br);
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ChaFile), nameof(ChaFile.SaveFile), new[] { typeof(BinaryWriter), typeof(bool), typeof(int) })]
        private static bool SaveFilePre(ChaFile __instance, BinaryWriter bw, bool savePng, int lang, ref bool __result)
        {
            __result = Patches.SaveFile(__instance, bw, savePng, lang);

            return false;
        }
    }
}
