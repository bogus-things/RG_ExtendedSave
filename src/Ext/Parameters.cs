using System;
using System.Linq;
using UnhollowerBaseLib;

namespace RGExtendedSave.Ext
{
    class Parameters
    {
        internal static (IntPtr, Il2CppStructArray<byte>) ReadData<T>(T obj, Il2CppStructArray<byte> data) where T : Il2CppSystem.Object
        {
            (int blockStart, int markerLength) = Util.FindBlockStart(data);
            if (blockStart > -1)
            {
                byte[] originalBytes = new byte[blockStart];
                Array.Copy(data, originalBytes, blockStart);

                byte[] extendedBytes = Util.GetSlice<byte>(data, blockStart + markerLength);
                Extensions.SetBytes(obj, extendedBytes);

                return (obj.Pointer, new Il2CppStructArray<byte>(originalBytes));
            }

            return (default, data);
        }

        internal static Il2CppStructArray<byte> WriteData<T>(T obj, Il2CppStructArray<byte> data) where T : Il2CppSystem.Object
        {
            byte[] extendedBytes = Extensions.GetBytes(obj);
            if (extendedBytes != null && extendedBytes.Length > 0)
            {
                byte[] marker = $"«{ExtendedSave.Marker}»".Select(c => (byte)c).ToArray();
                byte[] newData = new byte[data.Length + marker.Length + extendedBytes.Length];

                Array.Copy(data, newData, data.Length);
                Array.Copy(marker, 0, newData, data.Length, marker.Length);
                Array.Copy(extendedBytes, 0, newData, data.Length + marker.Length, extendedBytes.Length);

                return newData;
            }

            return data;
        }
    }
}
