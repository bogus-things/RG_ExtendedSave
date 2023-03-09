using BepInEx.Logging;
using Chara;
using System;
using System.Collections.Generic;

namespace RGExtendedSave
{
    public static class Extensions
    {
        private static ManualLogSource Log = RGExtendedSavePlugin.Log;

        internal static Dictionary<string, ExtendedData> InternalExtDictionary = new Dictionary<string, ExtendedData>();
        private static ExtendedData GetExtendedData(string key)
        {
            try
            {
                if (key == null) throw new ArgumentNullException(nameof(key));

                if (InternalExtDictionary.TryGetValue(key, out ExtendedData data))
                {
                    return data;
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
            }
            return null;
        }

        private static bool GetExtendedData(string key, string id, out PluginData data)
        {
            ExtendedData extendedData = GetExtendedData(key);

            if (extendedData != null && extendedData.TryGetValue(id, out data))
            {
                return true;
            }
                
            data = null;
            return false;
        }

        private static void SetExtendedData(string key, string id, PluginData data)
        {
            ExtendedData extendedData = GetExtendedData(key);
            if (extendedData == null) extendedData = new ExtendedData();

            if (data == null)
            {
                extendedData.Remove(id);
            }
            else
            {
                extendedData[id] = data;
            }

            InternalExtDictionary[key] = extendedData;
        }

        internal static byte[] GetBytes(Il2CppSystem.Object obj)
        {
            return GetBytes(obj.Pointer.ToString());
        }

        internal static byte[] GetBytes(string key)
        {
            ExtendedData extendedData = GetExtendedData(key);
            return extendedData != null ? extendedData.Serialize() : null;
        }

        internal static void SetBytes(Il2CppSystem.Object obj, byte[] bytes)
        {
            SetBytes(obj.Pointer.ToString(), bytes);
        }

        internal static void SetBytes(string key, byte[] bytes)
        {
            if (bytes == null)
            {
                return;
            }

            ExtendedData extendedData;
            try
            {
                extendedData = ExtendedData.Deserialize(bytes);
                InternalExtDictionary[key] = extendedData;
            }
            catch (Exception e)
            {
                Log.LogWarning($"Failed to deserialize extended data -- {e.Message}");
            }
        }

        internal static void MoveData(IntPtr from, IntPtr to)
        {
            if (from == IntPtr.Zero || to == IntPtr.Zero)
            {
                return;
            }

            MoveData(from.ToString(), to.ToString());
        }

        internal static void MoveData(string from, string to)
        {
            if (InternalExtDictionary.TryGetValue(from, out ExtendedData data))
            {
                InternalExtDictionary[to] = data;
                InternalExtDictionary.Remove(from);
            }
        }

        internal static void Remove(IntPtr key)
        {
            if (key == IntPtr.Zero)
            {
                return;
            }

            Remove(key.ToString());
        }

        internal static void Remove(string key)
        {
            InternalExtDictionary.Remove(key);
        }

        //Body
        public static Dictionary<string, PluginData> GetAllExtendedData(this ChaFileBody obj) => GetExtendedData(obj?.Pointer.ToString());
        public static bool TryGetExtendedDataById(this ChaFileBody obj, string id, out PluginData data) => GetExtendedData(obj?.Pointer.ToString(), id, out data);
        public static void SetExtendedDataById(this ChaFileBody obj, string id, PluginData data) => SetExtendedData(obj?.Pointer.ToString(), id, data);

        //Face
        public static Dictionary<string, PluginData> GetAllExtendedData(this ChaFileFace obj) => GetExtendedData(obj?.Pointer.ToString());
        public static bool TryGetExtendedDataById(this ChaFileFace obj, string id, out PluginData data) => GetExtendedData(obj?.Pointer.ToString(), id, out data);
        public static void SetExtendedDataById(this ChaFileFace obj, string id, PluginData data) => SetExtendedData(obj?.Pointer.ToString(), id, data);

        public static Dictionary<string, PluginData> GetAllExtendedData(this ChaFileFace.EyesInfo obj) => GetExtendedData(obj?.Pointer.ToString());
        public static bool TryGetExtendedDataById(this ChaFileFace.EyesInfo obj, string id, out PluginData data) => GetExtendedData(obj?.Pointer.ToString(), id, out data);
        public static void SetExtendedDataById(this ChaFileFace.EyesInfo obj, string id, PluginData data) => SetExtendedData(obj?.Pointer.ToString(), id, data);

        public static Dictionary<string, PluginData> GetAllExtendedData(this ChaFileFace.MakeupInfo obj) => GetExtendedData(obj?.Pointer.ToString());
        public static bool TryGetExtendedDataById(this ChaFileFace.MakeupInfo obj, string id, out PluginData data) => GetExtendedData(obj?.Pointer.ToString(), id, out data);
        public static void SetExtendedDataById(this ChaFileFace.MakeupInfo obj, string id, PluginData data) => SetExtendedData(obj?.Pointer.ToString(), id, data);

        //Hair
        public static Dictionary<string, PluginData> GetAllExtendedData(this ChaFileHair obj) => GetExtendedData(obj?.Pointer.ToString());
        public static bool TryGetExtendedDataById(this ChaFileHair obj, string id, out PluginData data) => GetExtendedData(obj?.Pointer.ToString(), id, out data);
        public static void SetExtendedDataById(this ChaFileHair obj, string id, PluginData data) => SetExtendedData(obj?.Pointer.ToString(), id, data);

        public static Dictionary<string, PluginData> GetAllExtendedData(this ChaFileHair.PartsInfo obj) => GetExtendedData(obj?.Pointer.ToString());
        public static bool TryGetExtendedDataById(this ChaFileHair.PartsInfo obj, string id, out PluginData data) => GetExtendedData(obj?.Pointer.ToString(), id, out data);
        public static void SetExtendedDataById(this ChaFileHair.PartsInfo obj, string id, PluginData data) => SetExtendedData(obj?.Pointer.ToString(), id, data);

        public static Dictionary<string, PluginData> GetAllExtendedData(this ChaFileHair.PartsInfo obj, int bundleId) => GetExtendedData($"{obj?.Pointer}-{bundleId}");
        public static bool TryGetExtendedDataById(this ChaFileHair.PartsInfo obj, int bundleId, string id, out PluginData data) => GetExtendedData($"{obj?.Pointer}-{bundleId}", id, out data);
        public static void SetExtendedDataById(this ChaFileHair.PartsInfo obj, int bundleId, string id, PluginData data) => SetExtendedData($"{obj?.Pointer}-{bundleId}", id, data);

        public static Dictionary<string, PluginData> GetAllExtendedData(this ChaFileHair.PartsInfo.ColorInfo obj) => GetExtendedData(obj?.Pointer.ToString());
        public static bool TryGetExtendedDataById(this ChaFileHair.PartsInfo.ColorInfo obj, string id, out PluginData data) => GetExtendedData(obj?.Pointer.ToString(), id, out data);
        public static void SetExtendedDataById(this ChaFileHair.PartsInfo.ColorInfo obj, string id, PluginData data) => SetExtendedData(obj?.Pointer.ToString(), id, data);

        //Clothes
        public static Dictionary<string, PluginData> GetAllExtendedData(this ChaFileClothes obj) => GetExtendedData(obj?.Pointer.ToString());
        public static bool TryGetExtendedDataById(this ChaFileClothes obj, string id, out PluginData data) => GetExtendedData(obj?.Pointer.ToString(), id, out data);
        public static void SetExtendedDataById(this ChaFileClothes obj, string id, PluginData data) => SetExtendedData(obj?.Pointer.ToString(), id, data);

        public static Dictionary<string, PluginData> GetAllExtendedData(this ChaFileClothes.PartsInfo obj) => GetExtendedData(obj?.Pointer.ToString());
        public static bool TryGetExtendedDataById(this ChaFileClothes.PartsInfo obj, string id, out PluginData data) => GetExtendedData(obj?.Pointer.ToString(), id, out data);
        public static void SetExtendedDataById(this ChaFileClothes.PartsInfo obj, string id, PluginData data) => SetExtendedData(obj?.Pointer.ToString(), id, data);

        public static Dictionary<string, PluginData> GetAllExtendedData(this ChaFileClothes.PartsInfo.ColorInfo obj) => GetExtendedData(obj?.Pointer.ToString());
        public static bool TryGetExtendedDataById(this ChaFileClothes.PartsInfo.ColorInfo obj, string id, out PluginData data) => GetExtendedData(obj?.Pointer.ToString(), id, out data);
        public static void SetExtendedDataById(this ChaFileClothes.PartsInfo.ColorInfo obj, string id, PluginData data) => SetExtendedData(obj?.Pointer.ToString(), id, data);

        //Accessory
        public static Dictionary<string, PluginData> GetAllExtendedData(this ChaFileAccessory obj) => GetExtendedData(obj?.Pointer.ToString());
        public static bool TryGetExtendedDataById(this ChaFileAccessory obj, string id, out PluginData data) => GetExtendedData(obj?.Pointer.ToString(), id, out data);
        public static void SetExtendedDataById(this ChaFileAccessory obj, string id, PluginData data) => SetExtendedData(obj?.Pointer.ToString(), id, data);

        public static Dictionary<string, PluginData> GetAllExtendedData(this ChaFileAccessory.PartsInfo obj) => GetExtendedData(obj?.Pointer.ToString());
        public static bool TryGetExtendedDataById(this ChaFileAccessory.PartsInfo obj, string id, out PluginData data) => GetExtendedData(obj?.Pointer.ToString(), id, out data);
        public static void SetExtendedDataById(this ChaFileAccessory.PartsInfo obj, string id, PluginData data) => SetExtendedData(obj?.Pointer.ToString(), id, data);

        public static Dictionary<string, PluginData> GetAllExtendedData(this ChaFileAccessory.PartsInfo.ColorInfo obj) => GetExtendedData(obj?.Pointer.ToString());
        public static bool TryGetExtendedDataById(this ChaFileAccessory.PartsInfo.ColorInfo obj, string id, out PluginData data) => GetExtendedData(obj?.Pointer.ToString(), id, out data);
        public static void SetExtendedDataById(this ChaFileAccessory.PartsInfo.ColorInfo obj, string id, PluginData data) => SetExtendedData(obj?.Pointer.ToString(), id, data);

        public static Dictionary<string, PluginData> GetAllExtendedData(this ChaFileGameInfo obj) => GetExtendedData(obj?.Pointer.ToString());
        public static bool TryGetExtendedDataById(this ChaFileGameInfo obj, string id, out PluginData data) => GetExtendedData(obj?.Pointer.ToString(), id, out data);
        public static void SetExtendedDataById(this ChaFileGameInfo obj, string id, PluginData data) => SetExtendedData(obj?.Pointer.ToString(), id, data);

        public static Dictionary<string, PluginData> GetAllExtendedData(this ChaFileParameter obj) => GetExtendedData(obj?.Pointer.ToString());
        public static bool TryGetExtendedDataById(this ChaFileParameter obj, string id, out PluginData data) => GetExtendedData(obj?.Pointer.ToString(), id, out data);
        public static void SetExtendedDataById(this ChaFileParameter obj, string id, PluginData data) => SetExtendedData(obj?.Pointer.ToString(), id, data);

        public static Dictionary<string, PluginData> GetAllExtendedData(this ChaFileStatus obj) => GetExtendedData(obj?.Pointer.ToString());
        public static bool TryGetExtendedDataById(this ChaFileStatus obj, string id, out PluginData data) => GetExtendedData(obj?.Pointer.ToString(), id, out data);
        public static void SetExtendedDataById(this ChaFileStatus obj, string id, PluginData data) => SetExtendedData(obj?.Pointer.ToString(), id, data);
    }
}
