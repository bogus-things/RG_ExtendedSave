using BepInEx.Logging;
using Chara;
using System;
using System.Collections.Generic;

namespace RGExtendedSave
{
    class Util
    {
        private static ManualLogSource Log = RGExtendedSavePlugin.Log;

        internal static byte[] GetMessagePackStringBytes(byte[] bytes, int startPos)
        {
            if (bytes.Length == 0)
            {
                return new byte[0];
            }

            byte startByte = bytes[startPos];
            int length = 0;

            if (startByte >> 5 == 5) //fixstr
            {
                length = startByte - 160 + 1;
            }
            else if (startByte == 0xd9) //str8
            {
                length = bytes[startPos + 1];
            }
            else if (startByte == 0xda) //str16
            {
                length = BitConverter.ToUInt16(bytes, startPos + 1);
            }
            else if (startByte == 0xdb) //str32
            {
                length = (int)BitConverter.ToUInt32(bytes, startPos + 1);
            }

            return GetSlice(bytes, startPos, startPos + length); ;
        }

        internal static int Get32BitInt(byte[] bytes, int pos)
        {
            if (bytes.Length < 4)
            {
                return 0;
            }

            byte[] b = GetSlice(bytes, pos, pos + 4);
            return BitConverter.ToInt32(b, 0);
        }

        internal static T[] GetSlice<T>(T[] arr, int from, int to)
        {
            int length = to - from;
            T[] result = new T[length];
            if (length > arr.Length)
            {
                return new T[0];
            }
            Array.Copy(arr, from, result, 0, length);

            return result;
        }

        internal static T[] GetSlice<T>(T[] arr, int from)
        {
            return GetSlice(arr, from, arr.Length);
        }

        internal static Il2CppSystem.Collections.Generic.Dictionary<TKey, TValue> DictToIl2Cpp<TKey, TValue>(Dictionary<TKey, TValue> dict)
        {
            Il2CppSystem.Collections.Generic.Dictionary<TKey, TValue> cppDict = new Il2CppSystem.Collections.Generic.Dictionary<TKey, TValue>();

            foreach(KeyValuePair<TKey, TValue> kv in dict)
            {
                cppDict.Add(kv.Key, kv.Value);
            }

            return cppDict;
        }

        internal static Dictionary<TKey, TValue> DictFromIl2Cpp<TKey, TValue>(Il2CppSystem.Collections.Generic.Dictionary<TKey, TValue> cppDict)
        {
            Dictionary<TKey, TValue> dict = new Dictionary<TKey, TValue>();

            foreach(Il2CppSystem.Collections.Generic.KeyValuePair<TKey, TValue> kv in cppDict)
            {
                dict.Add(kv.Key, kv.Value);
            }

            return dict;
        }

        internal static byte[] GetExtendedData(ChaFile file)
        {
            ExtendedData extendedData = ExtendedSave.GetAllExtendedData(file);
            return ExtendedDataToBytes(extendedData);
        }

        internal static byte[] GetExtendedData(ChaFileCoordinate file)
        {
            ExtendedData extendedData = ExtendedSave.GetAllExtendedData(file);
            return ExtendedDataToBytes(extendedData);
        }

        private static byte[] ExtendedDataToBytes(ExtendedData extendedData)
        {
            if (extendedData == null)
            {
                return new byte[0];
            }

            //Remove null entries
            List<string> keysToRemove = new List<string>();
            foreach (var entry in extendedData)
            {
                if (entry.Value == null)
                {
                    keysToRemove.Add(entry.Key);
                }
            }

            foreach (var key in keysToRemove)
            {
                extendedData.Remove(key);
            }

            return extendedData.Serialize();
        }
    }
}
