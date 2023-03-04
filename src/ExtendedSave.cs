using Chara;
using MessagePack;
using System;
using Il2CppSystem.Threading;
using BepInEx.Logging;
using UnhollowerBaseLib;
using MessagePack.Resolvers;

namespace RGExtendedSave
{
    public class ExtendedSave
    {
        private static ManualLogSource Log = RGExtendedSavePlugin.Log;

        private static IFormatterResolver resolver;

        public static string Marker = "RGEx";

        // Version of the extended save data on cards
        public static int DataVersion = 1;

        // Whether extended data load events should be triggered. Temporarily disable it when extended data will never be used, for example loading lists of cards.
        public static bool LoadEventsEnabled = true;

        internal static WeakKeyDictionary<ChaFile, ExtendedData> InternalCharaDictionary = new WeakKeyDictionary<ChaFile, ExtendedData>();

        internal static WeakKeyDictionary<ChaFileCoordinate, ExtendedData> InternalCoordinateDictionary = new WeakKeyDictionary<ChaFileCoordinate, ExtendedData>();


        // Get a dictionary of ID, PluginData containing all extended data for a ChaFile
        public static ExtendedData GetAllExtendedData(ChaFile file) => InternalCharaDictionary.Get(file);

        // Get PluginData for a ChaFile for the specified extended save data ID
        public static PluginData GetExtendedDataById(ChaFile file, string id)
        {
            if (file == null || id == null)
                return null;

            var dict = InternalCharaDictionary.Get(file);

            return dict != null && dict.TryGetValue(id, out var extendedSection) ? extendedSection : null;
        }

        // Set PluginData for a ChaFile for the specified extended save data ID
        public static void SetExtendedDataById(ChaFile file, string id, PluginData extendedFormatData)
        {
            ExtendedData chaDictionary = InternalCharaDictionary.Get(file);

            if (chaDictionary == null)
            {
                chaDictionary = new ExtendedData();
                InternalCharaDictionary.Set(file, chaDictionary);
            }

            chaDictionary[id] = extendedFormatData;
        }

        // Get a dictionary of ID, PluginData containing all extended data for a ChaFileCoordinate
        public static ExtendedData GetAllExtendedData(ChaFileCoordinate file) => InternalCoordinateDictionary.Get(file);

        // Get PluginData for a ChaFileCoordinate for the specified extended save data ID
        public static PluginData GetExtendedDataById(ChaFileCoordinate file, string id)
        {
            if (file == null || id == null)
                return null;

            var dict = InternalCoordinateDictionary.Get(file);

            return dict != null && dict.TryGetValue(id, out var extendedSection) ? extendedSection : null;
        }

        // Set PluginData for a ChaFileCoordinate for the specified extended save data ID
        public static void SetExtendedDataById(ChaFileCoordinate file, string id, PluginData extendedFormatData)
        {
            ExtendedData chaDictionary = InternalCoordinateDictionary.Get(file);

            if (chaDictionary == null)
            {
                chaDictionary = new ExtendedData();
                InternalCoordinateDictionary.Set(file, chaDictionary);
            }

            chaDictionary[id] = extendedFormatData;
        }

        internal static byte[] MessagePackSerialize<T>(T obj)
        {
            if (resolver == null)
            {
                InitializeResolver();
            }
            try
            {
                MessagePackSerializerOptions options = MessagePackSerializerOptions.Standard.WithResolver(resolver);
                return MessagePackSerializer.Serialize(obj, options, CancellationToken.None);
            }
            catch (Exception)
            {
                Log.LogWarning("Only primitive types are supported. Serialize your data first.");
                throw;
            }
        }

        internal static T MessagePackDeserialize<T>(byte[] obj)
        {
            if (resolver == null)
            {
                InitializeResolver();
            }

            MessagePackSerializerOptions options = MessagePackSerializerOptions.Standard.WithResolver(resolver);
            try
            {
                Il2CppStructArray<byte> bytes = new Il2CppStructArray<byte>(obj);
                return MessagePackSerializer.Deserialize<T>(bytes, options, CancellationToken.None);
            }
            catch (Exception)
            {
                Log.LogWarning("Only primitive types are supported. Serialize your data first.");
                throw;
            }
        }

        private static void InitializeResolver()
        {
            resolver = CompositeResolver.Create(new[] {
                DynamicGenericResolver.Instance.Cast<IFormatterResolver>(),
                StandardResolver.Instance.Cast<IFormatterResolver>()
            });
        }
    }
}
