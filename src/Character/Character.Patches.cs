using BepInEx.Logging;
using Chara;
using Il2CppSystem.IO;
using Il2CppSystem.Threading;
using Illusion.IO;
using MessagePack;

namespace RGExtendedSave.Character
{
    class Patches
    {
        private static ManualLogSource Log = RGExtendedSavePlugin.Log;
        internal static bool CardReadEventCalled;

        internal static void PreLoad()
        {
            CardReadEventCalled = true;
        }

        internal static void LoadExtendedData(ChaFile cf, BinaryReader br)
        {
            long originalPos = br.BaseStream.Position;
            br.BaseStream.Seek(0, SeekOrigin.Begin);

            long pngSize = PngFile.GetPngSize(br);
            br.BaseStream.Seek(pngSize + 103, SeekOrigin.Current);

            long facePngSize = br.ReadInt32();
            br.BaseStream.Seek(facePngSize, SeekOrigin.Current);

            int count = br.ReadInt32();
            BlockHeader blockHeader = MessagePackSerializer.Deserialize<BlockHeader>(br.ReadBytes(count), null, CancellationToken.None);
            BlockHeader.Info info = blockHeader.SearchInfo(ExtendedSave.Marker);

            if (info != null && info.version == ExtendedSave.DataVersion.ToString())
            {
                long infoSize = 0;
                foreach (BlockHeader.Info x in blockHeader.lstInfo)
                {
                    infoSize += x.size;
                }

                long basePosition = originalPos - infoSize;
                br.BaseStream.Position = basePosition + info.pos;
                byte[] data = br.ReadBytes((int)info.size);
                br.BaseStream.Position = originalPos;

                CardReadEventCalled = true;

                try
                {
                    ExtendedData extData = ExtendedData.Deserialize(data);
                    ExtendedSave.InternalCharaDictionary.Set(cf, extData);
                }
                catch (System.Exception e)
                {
                    ExtendedSave.InternalCharaDictionary.Set(cf, new ExtendedData());
                    Log.LogWarning($"Invalid or corrupted extended data in card \"{cf.CharaFileName}\" - {e.Message}");
                }

                Events.CardReadEvent(cf);
            }
            else
            {
                ExtendedSave.InternalCharaDictionary.Set(cf, new ExtendedData());
            }

            if (CardReadEventCalled == false)
            {
                ExtendedSave.InternalCharaDictionary.Set(cf, new ExtendedData());
                Events.CardReadEvent(cf);
            }
        }

        // Because we need to inject into the block header, we have to rewrite the whole save process
        internal static bool SaveFile(ChaFile cf, BinaryWriter bw, bool savePng, int lang)
        {
            Events.CardWriteEvent(cf);

            if (savePng && cf.PngData != null)
            {
                bw.Write(cf.PngData);
            }

            bw.Write(100);
            bw.Write("【RG_Chara】");
            bw.Write(ChaFileDefine.ChaFileVersion.ToString());
            bw.Write(lang);
            bw.Write(cf.UserID);
            bw.Write(cf.DataID);

            if (savePng && cf.FacePngData != null)
            {
                bw.Write(cf.FacePngData.Length);
                bw.Write(cf.FacePngData);
            }
            else
            {
                bw.Write(0);
            }

            byte[] customBytes = cf.GetCustomBytes();
            byte[] coordinateBytes = cf.GetCoordinateBytes();
            byte[] parameterBytes = cf.GetParameterBytes();
            byte[] gameInfoBytes = cf.GetGameInfoBytes();
            byte[] statusBytes = cf.GetStatusBytes();
            byte[] extendedDataByes = Util.GetExtendedData(cf);

            int num = 6;
            long num2 = 0L;

            string[] array = new string[]
            {
                ChaFileCustom.BlockName,
                ChaFileCoordinate.BlockName,
                ChaFileParameter.BlockName,
                ChaFileGameInfo.BlockName,
                ChaFileStatus.BlockName,
                ExtendedSave.Marker
            };

            string[] array2 = new string[]
            {
                ChaFileDefine.ChaFileCustomVersion.ToString(),
                ChaFileDefine.ChaFileCoordinateVersion.ToString(),
                ChaFileDefine.ChaFileParameterVersion.ToString(),
                ChaFileDefine.ChaFileGameInfoVersion.ToString(),
                ChaFileDefine.ChaFileStatusVersion.ToString(),
                ExtendedSave.DataVersion.ToString()
            };

            long[] array3 = new long[num];
            array3[0] = (long)((customBytes == null) ? 0 : customBytes.Length);
            array3[1] = (long)((coordinateBytes == null) ? 0 : coordinateBytes.Length);
            array3[2] = (long)((parameterBytes == null) ? 0 : parameterBytes.Length);
            array3[3] = (long)((gameInfoBytes == null) ? 0 : gameInfoBytes.Length);
            array3[4] = (long)((statusBytes == null) ? 0 : statusBytes.Length);
            array3[5] = (long)((extendedDataByes == null) ? 0 : extendedDataByes.Length);

            long[] array4 = new long[]
            {
                num2,
                num2 + array3[0],
                num2 + array3[0] + array3[1],
                num2 + array3[0] + array3[1] + array3[2],
                num2 + array3[0] + array3[1] + array3[2] + array3[3],
                num2 + array3[0] + array3[1] + array3[2] + array3[3] + array3[4] 
            };

            BlockHeader blockHeader = new BlockHeader();
            for (int i = 0; i < num; i++)
            {
                BlockHeader.Info item = new BlockHeader.Info
                {
                    name = array[i],
                    version = array2[i],
                    size = array3[i],
                    pos = array4[i]
                };
                blockHeader.lstInfo.Add(item);
            }

            byte[] array5 = MessagePackSerializer.Serialize(blockHeader, null, CancellationToken.None);
            bw.Write(array5.Length);
            bw.Write(array5);

            long num3 = 0L;
            foreach (long num4 in array3)
            {
                num3 += num4;
            }

            bw.Write(num3);
            bw.Write(customBytes);
            bw.Write(coordinateBytes);
            bw.Write(parameterBytes);
            bw.Write(gameInfoBytes);
            bw.Write(statusBytes);
            bw.Write(extendedDataByes);

            return true;
        }
    }
}
