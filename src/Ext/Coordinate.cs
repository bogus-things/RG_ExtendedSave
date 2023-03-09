using Chara;
using Il2CppSystem.Collections.Generic;
using Il2CppSystem.IO;
using UnhollowerBaseLib;

namespace RGExtendedSave.Ext
{
    class Coordinate
    {
        internal static void ReadData(ChaFileCoordinate cfc, Il2CppStructArray<byte> data, BinaryReader binaryReader, ExtKeyContainer keys)
        {
            if (keys == null)
            {
                keys = new ExtKeyContainer();
            }

            ExtKeyContainer.CoordinateKeys coordkeys = new ExtKeyContainer.CoordinateKeys();
            for (int i = 0; i < keys.Coordinate.Length; i++)
            {
                if (keys.Coordinate[i] == null)
                {
                    keys.Coordinate[i] = coordkeys;
                    break;
                }
            }

            if (data.Length - binaryReader.BaseStream.Position >= 4)
            {
                // ChaFileClothes
                coordkeys.Clothes = Util.ReadBlock(binaryReader, cfc.clothes);

                // ChaFileClothes.PartsInfo
                for(int i = 0; i < cfc.clothes.parts.Length; i++)
                {
                    ChaFileClothes.PartsInfo part = cfc.clothes.parts[i];
                    ExtKeyContainer.ClothesPartKeys partKeys = new ExtKeyContainer.ClothesPartKeys();
                    coordkeys.ClothesParts[i] = partKeys;

                    partKeys.Part = Util.ReadBlock(binaryReader, part);
 
                    // ChaFileClothes.PartsInfo.ColorInfo
                    for (int j = 0; j < part.colorInfo.Length; j++)
                    {
                        ChaFileClothes.PartsInfo.ColorInfo color = part.colorInfo[j];
                        partKeys.PartColors[j] = Util.ReadBlock(binaryReader, color);
                    }
                }

                // ChaFileAccessory
                coordkeys.Accessory = Util.ReadBlock(binaryReader, cfc.accessory);
                coordkeys.AccessoryParts = new ExtKeyContainer.AccessoryPartKeys[cfc.accessory.parts.Length];

                // ChaFileAccessory.PartsInfo
                for (int i = 0; i < cfc.accessory.parts.Length; i++)
                {
                    ChaFileAccessory.PartsInfo part = cfc.accessory.parts[i];
                    ExtKeyContainer.AccessoryPartKeys partKeys = new ExtKeyContainer.AccessoryPartKeys();
                    coordkeys.AccessoryParts[i] = partKeys;

                    partKeys.Part = Util.ReadBlock(binaryReader, part);

                    // ChaFileClothes.PartsInfo.ColorInfo
                    for (int j = 0; j < part.colorInfo.Length; j++)
                    {
                        ChaFileAccessory.PartsInfo.ColorInfo color = part.colorInfo[j];
                        partKeys.PartColors[j] = Util.ReadBlock(binaryReader, color);
                    }
                }

                // ChaFileHair
                coordkeys.Hair = Util.ReadBlock(binaryReader, cfc.hair);

                // ChaFileHair.PartsInfo
                for (int i = 0; i < cfc.hair.parts.Length; i++)
                {
                    ChaFileHair.PartsInfo part = cfc.hair.parts[i];
                    ExtKeyContainer.HairPartKeys partKeys = new ExtKeyContainer.HairPartKeys();
                    coordkeys.HairParts[i] = partKeys;

                    partKeys.Part = Util.ReadBlock(binaryReader, part);

                    // ChaFileHair.PartsInfo.ColorInfo
                    for (int j = 0; j < part.acsColorInfo.Length; j++)
                    {
                        ChaFileHair.PartsInfo.ColorInfo color = part.acsColorInfo[j];
                        partKeys.PartColors[j] = Util.ReadBlock(binaryReader, color);
                    }


                    // ChaFileHair.PartsInfo.BundleInfo
                    int bundleSize = binaryReader.ReadInt32();
                    if (bundleSize > 0)
                    {
                        Il2CppStructArray<byte> bundleBytes = binaryReader.ReadBytes(bundleSize);
                        Dictionary<int, Il2CppStructArray<byte>> byteMap = ExtendedSave.MessagePackDeserialize<Dictionary<int, Il2CppStructArray<byte>>>(bundleBytes);
                        foreach (KeyValuePair<int, Il2CppStructArray<byte>> kv in byteMap)
                        {
                            if (part.dictBundle[kv.Key] != null)
                            {
                                ChaFileHair.PartsInfo.BundleInfo bundleInfo = part.dictBundle[kv.Key];
                                Extensions.SetBytes($"{part.Pointer}-{kv.Key}", kv.Value);
                                partKeys.PartBundles[kv.Key] = $"{part.Pointer}-{kv.Key}";
                            }
                        }
                    }
                }
            }
        }

        internal static void WriteData(ChaFileCoordinate cfc, BinaryWriter binaryWriter)
        {
            // ChaFileClothes
            Util.WriteBlock(binaryWriter, cfc.clothes);

            // ChaFileClothes.PartsInfo
            for (int i = 0; i < cfc.clothes.parts.Length; i++)
            {
                ChaFileClothes.PartsInfo part = cfc.clothes.parts[i];
                Util.WriteBlock(binaryWriter, part);

                // ChaFileClothes.PartsInfo.ColorInfo
                for (int j = 0; j < part.colorInfo.Length; j++)
                {
                    ChaFileClothes.PartsInfo.ColorInfo color = part.colorInfo[j];
                    Util.WriteBlock(binaryWriter, color);
                }
            }

            // ChaFileAccessory
            Util.WriteBlock(binaryWriter, cfc.accessory);

            // ChaFileAccessory.PartsInfo
            for (int i = 0; i < cfc.accessory.parts.Length; i++)
            {
                ChaFileAccessory.PartsInfo part = cfc.accessory.parts[i];
                Util.WriteBlock(binaryWriter, part);

                // ChaFileClothes.PartsInfo.ColorInfo
                for (int j = 0; j < part.colorInfo.Length; j++)
                {
                    ChaFileAccessory.PartsInfo.ColorInfo color = part.colorInfo[j];
                    Util.WriteBlock(binaryWriter, color);
                }
            }

            // ChaFileHair
            Util.WriteBlock(binaryWriter, cfc.hair);

            // ChaFileHair.PartsInfo
            for (int i = 0; i < cfc.hair.parts.Length; i++)
            {
                ChaFileHair.PartsInfo part = cfc.hair.parts[i];
                Util.WriteBlock(binaryWriter, part);

                // ChaFileHair.PartsInfo.ColorInfo
                for (int j = 0; j < part.acsColorInfo.Length; j++)
                {
                    ChaFileHair.PartsInfo.ColorInfo color = part.acsColorInfo[j];
                    Util.WriteBlock(binaryWriter, color);
                }

                // ChaFileHair.PartsInfo.BundleInfo
                Dictionary<int, Il2CppStructArray<byte>> byteMap = new Dictionary<int, Il2CppStructArray<byte>>();
                foreach (KeyValuePair<int, ChaFileHair.PartsInfo.BundleInfo> kv in part.dictBundle)
                {
                    byteMap[kv.Key] = Extensions.GetBytes($"{part.Pointer}-{kv.Key}");
                }
                byte[] bundleBytes = ExtendedSave.MessagePackSerialize(byteMap);
                if (bundleBytes != null && bundleBytes.Length > 0)
                {
                    binaryWriter.Write(bundleBytes.Length);
                    binaryWriter.Write(bundleBytes);
                }
                else
                {
                    binaryWriter.Write(0);
                }
            }

        }
    }
}
