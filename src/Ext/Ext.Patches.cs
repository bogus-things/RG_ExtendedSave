using BepInEx.Logging;
using Chara;
using System;
using Il2CppSystem.IO;
using Il2CppSystem.Threading;
using MessagePack;
using UnhollowerBaseLib;

namespace RGExtendedSave.Ext
{
    class Patches
    {
        private static ManualLogSource Log = RGExtendedSavePlugin.Log;
        internal static ExtKeyContainer KeysToMigrate;
        internal static bool CustomLoadBytes(ChaFileCustom cfc, Il2CppStructArray<byte> data)
        {
            bool result;
            MemoryStream memoryStream = new MemoryStream(data);
            try
            {
                BinaryReader binaryReader = new BinaryReader(memoryStream);
                try
                {
                    int count = binaryReader.ReadInt32();
                    Il2CppStructArray<byte> bytes = binaryReader.ReadBytes(count);
                    cfc.face = MessagePackSerializer.Deserialize<ChaFileFace>(bytes, null, CancellationToken.None);
                    count = binaryReader.ReadInt32();
                    bytes = binaryReader.ReadBytes(count);
                    cfc.body = MessagePackSerializer.Deserialize<ChaFileBody>(bytes, null, CancellationToken.None);

                    cfc.face.ComplementWithVersion();
                    cfc.body.ComplementWithVersion();
                    result = true;

                    Custom.ReadData(cfc, data, binaryReader, KeysToMigrate);
                }
                finally
                {
                    binaryReader.Dispose();
                }
            }
            finally
            {
                memoryStream.Dispose();
            }
            return result;
        }

        internal static Il2CppStructArray<byte> CustomSaveBytes(ChaFileCustom cfc)
        {
            byte[] array = MessagePackSerializer.Serialize(cfc.face, null, CancellationToken.None);
            byte[] array2 = MessagePackSerializer.Serialize(cfc.body, null, CancellationToken.None);

            byte[] result;
            MemoryStream memoryStream = new MemoryStream();
            try
            {
                BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
                try
                {
                    binaryWriter.Write(array.Length);
                    binaryWriter.Write(array);
                    binaryWriter.Write(array2.Length);
                    binaryWriter.Write(array2);

                    Custom.WriteData(cfc, binaryWriter);

                    result = memoryStream.ToArray();
                }
                finally
                {
                    binaryWriter.Dispose();
                }
            }
            finally
            {
                memoryStream.Dispose();
            }
            return result;
        }

        internal static bool CoordinateLoadBytes(ChaFileCoordinate cfc, Il2CppStructArray<byte> data, Il2CppSystem.Version ver, bool clothes, bool accessory, bool hair)
        {
            bool result;
            MemoryStream memoryStream = new MemoryStream(data);
            try
            {
                BinaryReader binaryReader = new BinaryReader(memoryStream);
                try
                {
                    try
                    {
                        int count = binaryReader.ReadInt32();
                        Il2CppStructArray<byte> bytes;
                        if (clothes)
                        {
                            bytes = binaryReader.ReadBytes(count);
                            cfc.clothes = MessagePackSerializer.Deserialize<ChaFileClothes>(bytes, null, CancellationToken.None);
                        }
                        else
                        {
                            binaryReader.BaseStream.Seek(count, SeekOrigin.Current);
                        }

                        count = binaryReader.ReadInt32();
                        if (accessory)
                        {
                            bytes = binaryReader.ReadBytes(count);
                            cfc.accessory = MessagePackSerializer.Deserialize<ChaFileAccessory>(bytes, null, CancellationToken.None);
                        }
                        else
                        {
                            binaryReader.BaseStream.Seek(count, SeekOrigin.Current);
                        }

                        count = binaryReader.ReadInt32();
                        if (hair)
                        {
                            bytes = binaryReader.ReadBytes(count);
                            cfc.hair = MessagePackSerializer.Deserialize<ChaFileHair>(bytes, null, CancellationToken.None);
                        }
                        else
                        {
                            binaryReader.BaseStream.Seek(count, SeekOrigin.Current);
                        }
                    }
                    catch (System.Exception ex)
                    {
                        Log.LogError(ex);
                        return false;
                    }

                    if (clothes)
                    {
                        cfc.clothes.ComplementWithVersion();
                    }
                    if (accessory)
                    {
                        cfc.accessory.ComplementWithVersion();
                    }
                    if (hair)
                    {
                        cfc.hair.ComplementWithVersion();
                    }

                    result = true;

                    Coordinate.ReadData(cfc, data, binaryReader, KeysToMigrate);
                }
                finally
                {
                    binaryReader.Dispose();
                }
            }
            finally
            {
                memoryStream.Dispose();
            }
            return result;
        }

        internal static Il2CppStructArray<byte> CoordinateSaveBytes(ChaFileCoordinate cfc)
        {
            byte[] clothes = MessagePackSerializer.Serialize(cfc.clothes, null, CancellationToken.None);
            byte[] accessory = MessagePackSerializer.Serialize(cfc.accessory, null, CancellationToken.None);
            byte[] hair = MessagePackSerializer.Serialize(cfc.hair, null, CancellationToken.None);
            byte[] result;

            MemoryStream memoryStream = new MemoryStream();
            try
            {
                BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
                try
                {
                    binaryWriter.Write(clothes.Length);
                    binaryWriter.Write(clothes);
                    binaryWriter.Write(accessory.Length);
                    binaryWriter.Write(accessory);
                    binaryWriter.Write(hair.Length);
                    binaryWriter.Write(hair);

                    Coordinate.WriteData(cfc, binaryWriter);

                    result = memoryStream.ToArray();
                }
                finally
                {
                    binaryWriter.Dispose();
                }
            }
            finally
            {
                memoryStream.Dispose();
            }
            return result;
        }

        internal static Il2CppStructArray<byte> ReadParamData(ChaFile cf, Il2CppStructArray<byte> data)
        {
            (IntPtr key, Il2CppStructArray<byte> newData) = Parameters.ReadData(cf.Parameter, data);
            if (KeysToMigrate != null)
            {
                KeysToMigrate.FileParameter = key;
            }
            return newData;
        }

        internal static Il2CppStructArray<byte> WriteParamData(ChaFile cf, Il2CppStructArray<byte> data)
        {
            return Parameters.WriteData(cf.Parameter, data);
        }

        internal static Il2CppStructArray<byte> ReadGameInfoData(ChaFile cf, Il2CppStructArray<byte> data)
        {
            (IntPtr key, Il2CppStructArray<byte> newData) = Parameters.ReadData(cf.GameInfo, data);
            if (KeysToMigrate != null)
            {
                KeysToMigrate.GameInfo = key;
            }
            return newData;
        }

        internal static Il2CppStructArray<byte> WriteGameInfoData(ChaFile cf, Il2CppStructArray<byte> data)
        {
            return Parameters.WriteData(cf.GameInfo, data);
        }

        internal static Il2CppStructArray<byte> ReadStatusData(ChaFile cf, Il2CppStructArray<byte> data)
        {
            (IntPtr key, Il2CppStructArray<byte> newData) = Parameters.ReadData(cf.Status, data);
            if (KeysToMigrate != null)
            {
                KeysToMigrate.FileStatus = key;
            }
            return newData;
        }

        internal static Il2CppStructArray<byte> WriteStatusData(ChaFile cf, Il2CppStructArray<byte> data)
        {
            return Parameters.WriteData(cf.Status, data);
        }

        internal static void MigrateExtendedData(ChaFile cf, bool face, bool body, bool hair, bool parameter, bool coordinate)
        {
            if (KeysToMigrate != null)
            {
                if (face)
                {
                    KeysToMigrate.Face.MoveKeys(cf.Custom.face);
                }
                else
                {
                    KeysToMigrate.Face.RemoveKeys();
                }
                
                if (body) 
                {
                    Extensions.MoveData(KeysToMigrate.Body, cf.Custom.body.Pointer);
                }
                else
                {
                    Extensions.Remove(KeysToMigrate.Body);
                }

                if (coordinate)
                {
                    for (int i = 0; i < KeysToMigrate.Coordinate.Length; i++)
                    {
                        if (KeysToMigrate.Coordinate[i] != null)
                        {
                            KeysToMigrate.Coordinate[i].MoveOutfitKeys(cf.Coordinate[i]);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < KeysToMigrate.Coordinate.Length; i++)
                    {
                        if (KeysToMigrate.Coordinate[i] != null)
                        {
                            KeysToMigrate.Coordinate[i].RemoveOutfitKeys();
                        }
                    }
                }

                if (hair)
                {
                    for (int i = 0; i < KeysToMigrate.Coordinate.Length; i++)
                    {
                        if (KeysToMigrate.Coordinate[i] != null)
                        {
                            KeysToMigrate.Coordinate[i].MoveHairKeys(cf.Coordinate[i]);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < KeysToMigrate.Coordinate.Length; i++)
                    {
                        if (KeysToMigrate.Coordinate[i] != null)
                        {
                            KeysToMigrate.Coordinate[i].RemoveHairKeys();
                        }
                    }
                }

                if (parameter)
                {
                    Extensions.MoveData(KeysToMigrate.FileParameter, cf.Parameter.Pointer);
                    Extensions.MoveData(KeysToMigrate.GameInfo, cf.GameInfo.Pointer);
                    Extensions.MoveData(KeysToMigrate.FileStatus, cf.Status.Pointer);
                }
                else
                {
                    Extensions.Remove(KeysToMigrate.FileParameter);
                    Extensions.Remove(KeysToMigrate.GameInfo);
                    Extensions.Remove(KeysToMigrate.FileStatus);
                }
            }

            KeysToMigrate = null;
        }
    }
}
