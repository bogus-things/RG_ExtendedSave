using BepInEx.Logging;
using Chara;
using Il2CppSystem.IO;
using Illusion.IO;
using Il2CppSystem;

namespace RGExtendedSave.Coordinate
{
    class Patches
    {
        private static ManualLogSource Log = RGExtendedSavePlugin.Log;
        internal static bool LoadFile(ChaFileCoordinate cfc, Stream st, int lang, bool clothes, bool accessory, bool hair, bool skipPng)
        {
            bool result;
            BinaryReader br = new BinaryReader(st);
            try
            {
                if (skipPng)
                {
                    PngFile.SkipPng(br);
                }
                else
                {
                    long pngSize = PngFile.GetPngSize(br);
                    cfc.pngData = br.ReadBytes((int)pngSize);
                }

                if (br.BaseStream.Length - br.BaseStream.Position == 0L)
                {
                    cfc.lastLoadErrorCode = -5;
                    result = false;
                }
                else
                {
                    cfc.loadProductNo = br.ReadInt32();
                    if (cfc.loadProductNo > 100)
                    {
                        cfc.lastLoadErrorCode = -3;
                        result = false;
                    }
                    else if (br.ReadString() != "【RG_Clothes】")
                    {
                        cfc.lastLoadErrorCode = -1;
                        result = false;
                    }
                    else
                    {
                        cfc.loadVersion = new Version(br.ReadString());
                        if (cfc.loadVersion > ChaFileDefine.ChaFileCoordinateVersion)
                        {
                            cfc.lastLoadErrorCode = -2;
                            result = false;
                        }
                        else
                        {
                            cfc.language = br.ReadInt32();
                            cfc.coordinateName = br.ReadString();
                            int count = br.ReadInt32();
                            byte[] data = br.ReadBytes(count);
                            if (Ext.Patches.CoordinateLoadBytes(cfc, data, cfc.loadVersion, clothes, accessory, hair))
                            {
                                cfc.lastLoadErrorCode = 0;
                                result = true;


                                try
                                {
                                    string marker = br.ReadString();
                                    int version = br.ReadInt32();

                                    int length = br.ReadInt32();

                                    if (marker == ExtendedSave.Marker && version == ExtendedSave.DataVersion && length > 0)
                                    {
                                        byte[] bytes = br.ReadBytes(length);
                                        ExtendedData extData = ExtendedData.Deserialize(bytes);
                                        ExtendedSave.SetAllExtendedData(cfc, extData);
                                    }
                                    else
                                    {
                                        //Overriding with empty data just in case there is some remnant from former loads.
                                        ExtendedSave.SetAllExtendedData(cfc, new ExtendedData()); 
                                    }
                                }
                                catch (System.Exception)
                                {
                                    ExtendedSave.SetAllExtendedData(cfc, new ExtendedData());
                                }

                                Events.CoordinateReadEvent(cfc);
                            }
                            else
                            {
                                cfc.lastLoadErrorCode = -999;
                                result = false;
                            }
                        }
                    }
                }
            }
            catch (System.Exception)
            {
                cfc.lastLoadErrorCode = -999;
                result = false;
            }
            finally
            {
                br?.Dispose();
            }
            return result;
        }

        internal static void SaveFile(ChaFileCoordinate cfc, string path, int lang)
        {
            string directoryName = Path.GetDirectoryName(path);
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }

            cfc.coordinateFileName = Path.GetFileName(path);
            FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);

            try
            {
                BinaryWriter bw = new BinaryWriter(fs);

                try
                {
                    if (cfc.pngData != null)
                    {
                        bw.Write(cfc.pngData);
                    }
                    bw.Write(100);
                    bw.Write("【RG_Clothes】");
                    bw.Write(ChaFileDefine.ChaFileCoordinateVersion.ToString());
                    bw.Write(lang);
                    bw.Write(cfc.coordinateName);
                    byte[] array = Ext.Patches.CoordinateSaveBytes(cfc);
                    bw.Write(array.Length);
                    bw.Write(array);

                    Events.CoordinateWriteEvent(cfc);
                    byte[] data = Util.GetExtendedData(cfc);                    

                    bw.Write(ExtendedSave.Marker);
                    bw.Write(ExtendedSave.DataVersion);
                    bw.Write(data.Length);
                    bw.Write(data);
                }
                finally
                {
                    bw?.Dispose();
                }
            }
            finally
            {
                fs?.Dispose();
            }
        }
    }
}
