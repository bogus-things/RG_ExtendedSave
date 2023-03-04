﻿using BepInEx.Logging;
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
        private static void AddListPre()
        {
            ExtendedSave.LoadEventsEnabled = false;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(CustomCharaFileInfoAssist), nameof(CustomCharaFileInfoAssist.AddList))]
        private static void AddListPost()
        {
            ExtendedSave.LoadEventsEnabled = true;
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
            Core.Hooks.ChaFileSaveFilePreHook(__instance);
            __result = Patches.SaveFile(__instance, bw, savePng, lang);

            return false;
        }
    }
}