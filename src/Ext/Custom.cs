using BepInEx.Logging;
using Chara;
using Il2CppSystem.IO;
using UnhollowerBaseLib;

namespace RGExtendedSave.Ext
{
    class Custom
    {
        private static ManualLogSource Log = RGExtendedSavePlugin.Log;
        internal static void ReadData(ChaFileCustom cfc, Il2CppStructArray<byte> data, BinaryReader binaryReader, ExtKeyContainer keys)
        {
            if (keys == null)
            {
                keys = new ExtKeyContainer();
            }

            if (data.Length - binaryReader.BaseStream.Position >= 4)
            {

                // ChaFileFace
                keys.Face.Face = Util.ReadBlock(binaryReader, cfc.face);

                // ChaFileBody
                keys.Body = Util.ReadBlock(binaryReader, cfc.body);

                // ChaFileFace.EyesInfo
                for (int i = 0; i < cfc.face.pupil.Length; i++)
                {
                    keys.Face.Eyes[i] = Util.ReadBlock(binaryReader, cfc.face.pupil[i]);
                }

                // ChaFileFace.MakeupInfo
                keys.Face.Makeup = Util.ReadBlock(binaryReader, cfc.face.makeup);
            }
        }

        internal static void WriteData(ChaFileCustom cfc, BinaryWriter binaryWriter)
        {
            // ChaFileFace
            Util.WriteBlock(binaryWriter, cfc.face);

            // ChaFileBody
            Util.WriteBlock(binaryWriter, cfc.body);

            // ChaFileFace.EyesInfo
            foreach (ChaFileFace.EyesInfo eye in cfc.face.pupil)
            {
                Util.WriteBlock(binaryWriter, eye);
            }

            // ChaFileFace.MakeupInfo
            Util.WriteBlock(binaryWriter, cfc.face.makeup);
        }
    }
}
