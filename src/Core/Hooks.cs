using Chara;
using System;
using Il2CppSystem.Collections.Generic;
using System.Linq;
using Il2CppSystem.IO;
using BepInEx.Logging;

namespace RGExtendedSave.Core
{
    internal class Hooks
    {
        private static ManualLogSource Log = RGExtendedSavePlugin.Log;

        
        private static byte[] currentlySavingData = null;

        internal static void ChaFileLoadFilePreHook()
        {
            //cardReadEventCalled = true;
        }

        internal static void ChaFileLoadFileHook(ChaFile file, BlockHeader header, BinaryReader reader)
        {
           
        }

        internal static void ChaFileLoadFilePostHook(ChaFile cf, bool result, BinaryReader br)
        {
            if (!result) return;

            //If the event wasn't called at this point, it means the card doesn't contain any data, but we still need to call the event for consistency.
            
        }

        internal static void ChaFileSaveFilePreHook(ChaFile cf)
        {
            
        }

        internal static void ChaFileSaveFileHook(ChaFile file, BlockHeader header, ref long[] array3)
        {
            
        }

        internal static void ChaFileSaveFilePostHook(BinaryWriter bw)
        {
            if (currentlySavingData == null)
                return;

            bw.Write(currentlySavingData);
        }
    }
}
