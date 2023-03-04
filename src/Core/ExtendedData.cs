using System;
using System.Collections.Generic;
using System.Linq;
using UnhollowerBaseLib;

namespace RGExtendedSave
{
    public class ExtendedData : Dictionary<string, PluginData>
    {
        public byte[] Serialize()
        {
            List<IEnumerable<byte>> data = new List<IEnumerable<byte>>();
            
            foreach(string key in Keys)
            {
                if (TryGetValue(key, out PluginData value))
                {
                    byte[] k = ExtendedSave.MessagePackSerialize(key);
                    byte[] v = value.Serialize();
                    byte[] size = BitConverter.GetBytes(v.Length);
                    data.Add(k.Concat(size).Concat(v));
                }   
            }

            IEnumerable<byte> bytes = BitConverter.GetBytes(Count);

            foreach (IEnumerable<byte> item in data)
            {
                bytes = bytes.Concat(item);
            }

            return bytes.ToArray();
        }

        public static ExtendedData Deserialize(byte[] bytes)
        {
            ExtendedData ext = new ExtendedData();
            if (bytes != null && bytes.Length > 0)
            {
                int count = Util.Get32BitInt(bytes, 0);
                int cursor = 4;

                for (int i = 0; i < count; i++)
                {
                    byte[] keyBytes = Util.GetMessagePackStringBytes(bytes, cursor);
                    string key = ExtendedSave.MessagePackDeserialize<string>(new Il2CppStructArray<byte>(keyBytes));
                    cursor += keyBytes.Length;

                    int dataSize = Util.Get32BitInt(bytes, cursor);
                    cursor += 4;

                    PluginData data = PluginData.Deserialize(Util.GetSlice(bytes, cursor, cursor + dataSize));
                    cursor += dataSize;

                    ext.Add(key, data);
                }
            }

            return ext;
        }
    }
}
