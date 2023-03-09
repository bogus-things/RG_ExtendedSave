using System.Collections.Generic;
using System;
using System.Linq;
using UnhollowerBaseLib;
using UnhollowerRuntimeLib;

namespace RGExtendedSave
{
    /// An object containing data saved to and loaded from cards.
    public class PluginData: Il2CppSystem.Object
    {
        public PluginData(IntPtr pointer) : base(pointer)
        {

        }

        public PluginData(): base(ClassInjector.DerivedConstructorPointer<PluginData>())
        {
            ClassInjector.DerivedConstructorBody(this);
        }

        public int version;

        public Dictionary<string, object> data = new Dictionary<string, object>();

        public Il2CppStructArray<byte> Serialize()
        {
            byte[] v = BitConverter.GetBytes(version);
            byte[] d = ExtendedSave.MessagePackSerialize(Util.DataToIl2Cpp(data));

            return v.Concat(d).ToArray();
        }

        public static PluginData Deserialize(Il2CppStructArray<byte> bytes)
        {
            PluginData pd = new PluginData();

            if (bytes != null && bytes.Length > 0)
            {
                pd.version = Util.Get32BitInt(bytes, 0);
                Il2CppStructArray<byte> data = Util.GetStructArraySlice(bytes, 4);
                pd.data = Util.DataFromIl2Cpp(ExtendedSave.MessagePackDeserialize<Il2CppSystem.Collections.Generic.Dictionary<string, string>>(data));
            }

            return pd;
        }
    }
}
