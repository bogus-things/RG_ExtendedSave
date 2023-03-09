using BepInEx.Logging;
using Chara;
using Il2CppSystem.IO;
using System;
using System.Collections.Generic;
using UnhollowerBaseLib;

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

        internal static Il2CppStructArray<byte> GetStructArraySlice(Il2CppStructArray<byte> arr, int from)
        {
            return new Il2CppStructArray<byte>(GetSlice((byte[])arr, from, arr.Length));
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

        internal static Il2CppSystem.Collections.Generic.Dictionary<string, string> DataToIl2Cpp(Dictionary<string, object> dict)
        {
            Il2CppSystem.Collections.Generic.Dictionary<string, string> cppDict = new Il2CppSystem.Collections.Generic.Dictionary<string, string>();

            foreach(KeyValuePair<string, object> kv in dict)
            {
                Type t = kv.Value.GetType();
                if ((!t.IsPrimitive && t != typeof(string)) || t == typeof(IntPtr) || t == typeof(UIntPtr))
                {
                    throw new ArgumentException($"Unsupported type {t}. Only non-pointer primitive types & strings are supported. Serialize your data first.");
                }

                string value = $"{t}::{kv.Value}";
                cppDict.Add(kv.Key, value);
            }

            return cppDict;
        }

        internal static Dictionary<string, object> DataFromIl2Cpp(Il2CppSystem.Collections.Generic.Dictionary<string, string> cppDict)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();

            foreach(Il2CppSystem.Collections.Generic.KeyValuePair<string, string> kv in cppDict)
            {
                int separatorIndex = kv.Value.IndexOf("::");
                object pValue;
                if (separatorIndex > -1)
                {
                    string pType = kv.Value.Substring(0, separatorIndex);
                    string pStr = kv.Value.Substring(separatorIndex + 2);

                    Type t = Type.GetType(pType);
                    if (t == typeof(string))
                    {
                        pValue = pStr;
                    }
                    else if (t == typeof(bool))
                    {
                        pValue = bool.Parse(pStr);
                    }
                    else if (t == typeof(byte))
                    {
                        pValue = byte.Parse(pStr);
                    }
                    else if (t == typeof(sbyte))
                    {
                        pValue = sbyte.Parse(pStr);
                    }
                    else if (t == typeof(short))
                    {
                        pValue = short.Parse(pStr);
                    }
                    else if (t == typeof(ushort))
                    {
                        pValue = ushort.Parse(pStr);
                    }
                    else if (t == typeof(int))
                    {
                        pValue = int.Parse(pStr);
                    }
                    else if (t == typeof(uint))
                    {
                        pValue = uint.Parse(pStr);
                    }
                    else if (t == typeof(long))
                    {
                        pValue = long.Parse(pStr);
                    }
                    else if (t == typeof(ulong))
                    {
                        pValue = ulong.Parse(pStr);
                    }
                    else if (t == typeof(char))
                    {
                        pValue = char.Parse(pStr);
                    }
                    else if (t == typeof(double))
                    {
                        pValue = double.Parse(pStr);
                    }
                    else if (t == typeof(float))
                    {
                        pValue = float.Parse(pStr);
                    }
                    else
                    {
                        pValue = pStr;
                    }
                }
                else
                {
                    pValue = kv.Value;
                }

                dict.Add(kv.Key, pValue);
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

        internal static IntPtr ReadBlock<T>(BinaryReader br, T obj) where T : Il2CppSystem.Object
        {
            int count = br.ReadInt32();
            if (count > 0)
            {
                Il2CppStructArray<byte> bytes = br.ReadBytes(count);
                Extensions.SetBytes(obj, bytes);
                return obj.Pointer;
            }
            return default;
        }

        internal static void WriteBlock<T>(BinaryWriter bw, T obj) where T : Il2CppSystem.Object
        {
            byte[] bytes = Extensions.GetBytes(obj);
            if (bytes != null && bytes.Length > 0)
            {
                bw.Write(bytes.Length);
                bw.Write(bytes);
            }
            else
            {
                bw.Write(0);
            }
        }

        internal static (int, int) FindBlockStart(byte[] bytes)
        {
            char[] marker = $"«{ExtendedSave.Marker}»".ToCharArray();
            int i;
            for (i = bytes.Length - 1; i >= 0; i--)
            {
                if (i + marker.Length > bytes.Length - 1)
                {
                    continue;
                }

                bool match = true;
                for (int j = 0; j < marker.Length; j++)
                {
                    if (marker[j] != bytes[i + j])
                    {
                        match = false;
                        break;
                    }
                }
                
                if (match)
                {
                    break;
                }
            }

            return (i, marker.Length);
        }
    }    
}
