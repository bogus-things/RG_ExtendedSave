using System.Collections.Generic;
using System;
using System.Linq;
using UnhollowerBaseLib;

namespace RGExtendedSave
{
    /// An object containing data saved to and loaded from cards.
    public class PluginData
    {

        public int version;

        public Dictionary<string, object> data = new Dictionary<string, object>();

        public byte[] Serialize()
        {
            byte[] v = BitConverter.GetBytes(version);
            byte[] d = ExtendedSave.MessagePackSerialize(Util.DataToIl2Cpp(data));

            return v.Concat(d).ToArray();
        }

        public static PluginData Deserialize(byte[] bytes)
        {
            PluginData pd = new PluginData();

            if (bytes != null && bytes.Length > 0)
            {
                pd.version = Util.Get32BitInt(bytes, 0);
                byte[] data = Util.GetSlice(bytes, 4);
                pd.data = Util.DataFromIl2Cpp(ExtendedSave.MessagePackDeserialize<Il2CppSystem.Collections.Generic.Dictionary<string, string>>(new Il2CppStructArray<byte>(data)));
            }

            return pd;
        }
    }
}
