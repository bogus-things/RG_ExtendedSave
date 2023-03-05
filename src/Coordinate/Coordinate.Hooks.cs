using BepInEx.Logging;
using Chara;
using CharaCustom;
using HarmonyLib;
using Il2CppSystem.IO;


namespace RGExtendedSave.Coordinate
{
    class Hooks
    {
        private static ManualLogSource Log = RGExtendedSavePlugin.Log;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(CustomClothesFileInfoAssist), nameof(CustomClothesFileInfoAssist.AddList))]
        [HarmonyPatch(typeof(CvsC_ClothesLoad), nameof(CvsC_ClothesLoad.UpdateClothesList))]
        [HarmonyPatch(typeof(CvsC_ClothesSave), nameof(CvsC_ClothesSave.UpdateClothesList))]
        private static void AddListPre()
        {
            ExtendedSave.LoadEventsEnabled = false;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(CustomClothesFileInfoAssist), nameof(CustomClothesFileInfoAssist.AddList))]
        [HarmonyPatch(typeof(CvsC_ClothesLoad), nameof(CvsC_ClothesLoad.UpdateClothesList))]
        [HarmonyPatch(typeof(CvsC_ClothesSave), nameof(CvsC_ClothesSave.UpdateClothesList))]
        private static void AddListPost()
        {
            ExtendedSave.LoadEventsEnabled = true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ChaFileCoordinate), nameof(ChaFileCoordinate.LoadFile), new[] { typeof(Stream), typeof(int), typeof(bool), typeof(bool), typeof(bool), typeof(bool) })]
        private static bool LoadFilePre(ChaFileCoordinate __instance, Stream st, int lang, ref bool __result, bool clothes = true, bool accessory = true, bool hair = true, bool skipPng = true)
        {
            if (ExtendedSave.LoadEventsEnabled)
            {
                __result = Patches.LoadFile(__instance, st, lang, clothes, accessory, hair, skipPng);
            }
            
            return !ExtendedSave.LoadEventsEnabled;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ChaFileCoordinate), nameof(ChaFileCoordinate.SaveFile), new[] { typeof(string), typeof(int) })]
        private static bool SaveFilePre(ChaFileCoordinate __instance, string path, int lang)
        {
            Patches.SaveFile(__instance, path, lang);
            return false;
        }
    }
}
