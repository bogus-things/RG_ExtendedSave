using BepInEx.Logging;
using Chara;
using HarmonyLib;
using Il2CppSystem;
using UnhollowerBaseLib;

namespace RGExtendedSave.Ext
{
    class Hooks
    {
        private static ManualLogSource Log = RGExtendedSavePlugin.Log;

        // ChaFileCustom
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ChaFileCustom), nameof(ChaFileCustom.LoadBytes), new[] { typeof(Il2CppStructArray<byte>), typeof(Version) })]
        private static bool CustomLoadBytesPre(ChaFileCustom __instance, Il2CppStructArray<byte> data, ref bool __result)
        {
            if (ExtendedSave.LoadEventsEnabled)
            {
                __result = Patches.CustomLoadBytes(__instance, data);
            }

            return !ExtendedSave.LoadEventsEnabled;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ChaFileCustom), nameof(ChaFileCustom.SaveBytes))]
        private static bool CustomSaveBytesPre(ChaFileCustom __instance, ref Il2CppStructArray<byte> __result)
        {
            __result = Patches.CustomSaveBytes(__instance);
            return false;
        }

        // ChaFileCoordinate
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ChaFileCoordinate), nameof(ChaFileCoordinate.LoadBytes))]
        private static bool CoordinateLoadBytesPre(ChaFileCoordinate __instance, Il2CppStructArray<byte> data, Version ver, ref bool __result, bool clothes = true, bool accessory = true, bool hair = true)
        {
            if (ExtendedSave.LoadEventsEnabled)
            {
                __result = Patches.CoordinateLoadBytes(__instance, data, ver, clothes, accessory, hair);
            }

            return !ExtendedSave.LoadEventsEnabled;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ChaFileCoordinate), nameof(ChaFileCoordinate.SaveBytes))]
        private static bool CoordinateSaveBytesPre(ChaFileCoordinate __instance, ref Il2CppStructArray<byte> __result)
        {
            __result = Patches.CoordinateSaveBytes(__instance);
            return false;
        }

        // ChaFileParameter
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ChaFile), nameof(ChaFile.SetParameterBytes))]
        private static void SetParamBytesPre(ChaFile __instance, ref Il2CppStructArray<byte> data)
        {
            if (ExtendedSave.LoadEventsEnabled)
            {
                data = Patches.ReadParamData(__instance, data);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ChaFile), nameof(ChaFile.GetParameterBytes), new System.Type[] { })]
        private static void GetParamBytesPost(ChaFile __instance, ref Il2CppStructArray<byte> __result)
        {
            __result = Patches.WriteParamData(__instance, __result);
        }

        // ChaFileGameInfo
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ChaFile), nameof(ChaFile.SetGameInfoBytes))]
        private static void SetGameInfoBytesPre(ChaFile __instance, ref Il2CppStructArray<byte> data)
        {
            if (ExtendedSave.LoadEventsEnabled)
            {
                data = Patches.ReadGameInfoData(__instance, data);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ChaFile), nameof(ChaFile.GetGameInfoBytes), new System.Type[] { })]
        private static void GetGameInfoBytesPost(ChaFile __instance, ref Il2CppStructArray<byte> __result)
        {
            __result = Patches.WriteGameInfoData(__instance, __result);
        }

        // ChaFileStatus
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ChaFile), nameof(ChaFile.SetStatusBytes))]
        private static void SetStatusBytesPre(ChaFile __instance, ref Il2CppStructArray<byte> data)
        {
            if (ExtendedSave.LoadEventsEnabled)
            {
                data = Patches.ReadStatusData(__instance, data);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ChaFile), nameof(ChaFile.GetStatusBytes), new System.Type[] { })]
        private static void GetStatusBytesPost(ChaFile __instance, ref Il2CppStructArray<byte> __result)
        {
            __result = Patches.WriteStatusData(__instance, __result);
        }

        // Editor key migration stuff
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ChaFileControl), nameof(ChaFileControl.LoadFileLimited))]
        private static void LoadFileLimitedPre()
        {
            Patches.KeysToMigrate = new ExtKeyContainer();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ChaFileControl), nameof(ChaFileControl.LoadFileLimited))]
        private static void LoadFileLimitedPost(ChaFileControl __instance, bool face = true, bool body = true, bool hair = true, bool parameter = true, bool coordinate = true)
        {
            Patches.MigrateExtendedData(__instance, face, body, hair, parameter, coordinate);
        }
    }
}
