using BepInEx.Logging;
using UnityEngine;
using UniverseLib.Input;
using RGExtendedSave;
using System;
using Chara;
using UnhollowerBaseLib.Attributes;

namespace RGExtendedSaveTester
{
    class TestController : MonoBehaviour
    {
        private static ManualLogSource Log = RGExtendedSaveTesterPlugin.Log;

        private static string ExtendedSaveKey = "ExtendedSaveTester";

        private static bool Loading = false;
        private static bool Testing = false;
        private static bool Clearing = false;

        public void Update()
        {
            if (InputManager.GetKeyUp(RGExtendedSaveTesterPlugin.LoadKey.Value))
            {
                if (Loading)
                {
                    return;
                }

                Loading = true;

                try
                {
                    LoadData();
                }
                finally
                {
                    Loading = false;
                }                
            }

            if (InputManager.GetKeyUp(RGExtendedSaveTesterPlugin.ClearKey.Value))
            {
                if (Clearing)
                {
                    return;
                }

                Clearing = true;

                try
                {
                    ClearData();
                }
                catch (Exception ex)
                {
                    Log.LogError(ex);
                }
                finally
                {
                    Clearing = false;
                }
            }

            if (InputManager.GetKeyUp(RGExtendedSaveTesterPlugin.TestKey.Value))
            {
                if (Testing)
                {
                    return;
                }

                Testing = true;

                try
                {
                    TestData();
                }
                catch (Exception ex)
                {
                    Log.LogError(ex);
                }
                finally
                {
                    Testing = false;
                }
            }
        }

        private void LoadData()
        {
            GameObject obj = GameObject.Find("CharaCustom");
            if (obj == null)
            {
                Log.LogError("CharaCustom not found! Are you in the editor?");
                return;
            }

            CharaCustom.CharaCustom charaCustom = obj.GetComponent<CharaCustom.CharaCustom>();
            ChaFile cf = charaCustom.customCtrl.chaFile;

            PluginData chaData = new PluginData();
            chaData.data.Add("testString", "testStringValue");
            chaData.data.Add("testByte", -1);
            chaData.data.Add("testSByte", 1);
            chaData.data.Add("testInt16", (short)-10);
            chaData.data.Add("testUInt16", (ushort)10);
            chaData.data.Add("testInt32", -20);
            chaData.data.Add("testUInt32", (uint)20);
            chaData.data.Add("testInt64", (long)-30);
            chaData.data.Add("testUInt64", (ulong)30);
            chaData.data.Add("testDouble", (double)4.20);
            chaData.data.Add("testSingle", (float)6.9);

            ExtendedSave.SetExtendedDataById(cf, ExtendedSaveKey, chaData);

            // ChaFileFace
            chaData = CreateNewPluginData(cf.Custom.face);
            Extensions.SetExtendedDataById(cf.Custom.face, ExtendedSaveKey, chaData);
            //ChaFileFace.EyesInfo
            chaData = CreateNewPluginData(cf.Custom.face.pupil[0]);
            Extensions.SetExtendedDataById(cf.Custom.face.pupil[0], ExtendedSaveKey, chaData);
            //ChaFIleFace.MakeupInfo
            chaData = CreateNewPluginData(cf.Custom.face.makeup);
            Extensions.SetExtendedDataById(cf.Custom.face.makeup, ExtendedSaveKey, chaData);
            // ChaFileBody
            chaData = CreateNewPluginData(cf.Custom.body);
            Extensions.SetExtendedDataById(cf.Custom.body, ExtendedSaveKey, chaData);
            // ChaFileClothes
            chaData = CreateNewPluginData(cf.Coordinate[0].clothes);
            Extensions.SetExtendedDataById(cf.Coordinate[0].clothes, ExtendedSaveKey, chaData);
            // ChaFileClothes.PartsInfo
            chaData = CreateNewPluginData(cf.Coordinate[0].clothes.parts[0]);
            Extensions.SetExtendedDataById(cf.Coordinate[0].clothes.parts[0], ExtendedSaveKey, chaData);
            // ChaFileCLothes.PartsInfo.ColorInfo
            chaData = CreateNewPluginData(cf.Coordinate[0].clothes.parts[0].colorInfo[0]);
            Extensions.SetExtendedDataById(cf.Coordinate[0].clothes.parts[0].colorInfo[0], ExtendedSaveKey, chaData);
            // ChaFileAccessory
            chaData = CreateNewPluginData(cf.Coordinate[0].accessory);
            Extensions.SetExtendedDataById(cf.Coordinate[0].accessory, ExtendedSaveKey, chaData);
            // ChaFileAccessory.PartsInfo
            chaData = CreateNewPluginData(cf.Coordinate[0].accessory.parts[0]);
            Extensions.SetExtendedDataById(cf.Coordinate[0].accessory.parts[0], ExtendedSaveKey, chaData);
            // ChaFileAccessory.PartsInfo.ColorInfo
            chaData = CreateNewPluginData(cf.Coordinate[0].accessory.parts[0].colorInfo[0]);
            Extensions.SetExtendedDataById(cf.Coordinate[0].accessory.parts[0].colorInfo[0], ExtendedSaveKey, chaData);
            // ChaFileHair
            chaData = CreateNewPluginData(cf.Coordinate[0].hair);
            Extensions.SetExtendedDataById(cf.Coordinate[0].hair, ExtendedSaveKey, chaData);
            // ChaFileHair.PartsInfo
            chaData = CreateNewPluginData(cf.Coordinate[0].hair.parts[0]);
            Extensions.SetExtendedDataById(cf.Coordinate[0].hair.parts[0], ExtendedSaveKey, chaData);
            // ChaFileHair.PartsInfo.ColorInfo
            chaData = CreateNewPluginData(cf.Coordinate[0].hair.parts[0].acsColorInfo[0]);
            Extensions.SetExtendedDataById(cf.Coordinate[0].hair.parts[0].acsColorInfo[0], ExtendedSaveKey, chaData);

            // ChaFileHair.PartsInfo.BundleInfo
            string key = $"{cf.Coordinate[0].hair.parts[0].Pointer}-0";
            chaData = CreateNewPluginData(cf.Coordinate[0].hair.parts[0].dictBundle[0]);
            Extensions.SetExtendedDataById(cf.Coordinate[0].hair.parts[0], 0, ExtendedSaveKey, chaData);

            // ChaFileGameInfo
            chaData = CreateNewPluginData(cf.GameInfo);
            Extensions.SetExtendedDataById(cf.GameInfo, ExtendedSaveKey, chaData);

            // ChaFileParameter
            chaData = CreateNewPluginData(cf.Parameter);
            Extensions.SetExtendedDataById(cf.Parameter, ExtendedSaveKey, chaData);

            Log.LogMessage("Load complete!");
        }

        private void ClearData()
        {
            GameObject obj = GameObject.Find("CharaCustom");
            if (obj == null)
            {
                Log.LogError("CharaCustom not found! Are you in the editor?");
                return;
            }

            CharaCustom.CharaCustom charaCustom = obj.GetComponent<CharaCustom.CharaCustom>();
            ChaFile cf = charaCustom.customCtrl.chaFile;

            ExtendedSave.SetExtendedDataById(cf, ExtendedSaveKey, null);

            Log.LogMessage("Data cleared");
        }

        private void TestData()
        {
            GameObject obj = GameObject.Find("CharaCustom");
            if (obj == null)
            {
                Log.LogError("CharaCustom not found! Are you in the editor?");
                return;
            }

            CharaCustom.CharaCustom charaCustom = obj.GetComponent<CharaCustom.CharaCustom>();
            ChaFile cf = charaCustom.customCtrl.chaFile;

            PluginData chaData = ExtendedSave.GetExtendedDataById(cf, ExtendedSaveKey);

            TestNotNull(chaData, cf);
            TestValue(chaData, "testString", "testStringValue");
            TestValue(chaData, "testByte", -1);
            TestValue(chaData, "testSByte", 1);
            TestValue(chaData, "testInt16", (short)-10);
            TestValue(chaData, "testUInt16", (ushort)10);
            TestValue(chaData, "testInt32", -20);
            TestValue(chaData, "testUInt32", (uint)20);
            TestValue(chaData, "testInt64", (long)-30);
            TestValue(chaData, "testUInt64", (ulong)30);
            TestValue(chaData, "testDouble", (double)4.20);
            TestValue(chaData, "testSingle", (float)6.9);

            // ChaFileFace
            Extensions.TryGetExtendedDataById(cf.Custom.face, ExtendedSaveKey, out chaData);
            TestNotNull(chaData, cf.Custom.face);
            TestPluginData(chaData, cf.Custom.face);

            //ChaFileFace.EyesInfo
            Extensions.TryGetExtendedDataById(cf.Custom.face.pupil[0], ExtendedSaveKey, out chaData);
            TestNotNull(chaData, cf.Custom.face.pupil[0]);
            TestPluginData(chaData, cf.Custom.face.pupil[0]);

            //ChaFIleFace.MakeupInfo
            Extensions.TryGetExtendedDataById(cf.Custom.face.makeup, ExtendedSaveKey, out chaData);
            TestNotNull(chaData, cf.Custom.face.makeup);
            TestPluginData(chaData, cf.Custom.face.makeup);

            // ChaFileBody
            Extensions.TryGetExtendedDataById(cf.Custom.body, ExtendedSaveKey, out chaData);
            TestNotNull(chaData, cf.Custom.body);
            TestPluginData(chaData, cf.Custom.body);

            // ChaFileClothes
            Extensions.TryGetExtendedDataById(cf.Coordinate[0].clothes, ExtendedSaveKey, out chaData);
            TestNotNull(chaData, cf.Coordinate[0].clothes);
            TestPluginData(chaData, cf.Coordinate[0].clothes);

            // ChaFileClothes.PartsInfo
            Extensions.TryGetExtendedDataById(cf.Coordinate[0].clothes.parts[0], ExtendedSaveKey, out chaData);
            TestNotNull(chaData, cf.Coordinate[0].clothes.parts[0]);
            TestPluginData(chaData, cf.Coordinate[0].clothes.parts[0]);

            // ChaFileCLothes.PartsInfo.ColorInfo
            Extensions.TryGetExtendedDataById(cf.Coordinate[0].clothes.parts[0].colorInfo[0], ExtendedSaveKey, out chaData);
            TestNotNull(chaData, cf.Coordinate[0].clothes.parts[0].colorInfo[0]);
            TestPluginData(chaData, cf.Coordinate[0].clothes.parts[0].colorInfo[0]);

            // ChaFileAccessory
            Extensions.TryGetExtendedDataById(cf.Coordinate[0].accessory, ExtendedSaveKey, out chaData);
            TestNotNull(chaData, cf.Coordinate[0].accessory);
            TestPluginData(chaData, cf.Coordinate[0].accessory);

            // ChaFileAccessory.PartsInfo
            Extensions.TryGetExtendedDataById(cf.Coordinate[0].accessory.parts[0], ExtendedSaveKey, out chaData);
            TestNotNull(chaData, cf.Coordinate[0].accessory.parts[0]);
            TestPluginData(chaData, cf.Coordinate[0].accessory.parts[0]);

            // ChaFileAccessory.PartsInfo.ColorInfo
            Extensions.TryGetExtendedDataById(cf.Coordinate[0].accessory.parts[0].colorInfo[0], ExtendedSaveKey, out chaData);
            TestNotNull(chaData, cf.Coordinate[0].accessory.parts[0].colorInfo[0]);
            TestPluginData(chaData, cf.Coordinate[0].accessory.parts[0].colorInfo[0]);

            // ChaFileHair
            Extensions.TryGetExtendedDataById(cf.Coordinate[0].hair, ExtendedSaveKey, out chaData);
            TestNotNull(chaData, cf.Coordinate[0].hair);
            TestPluginData(chaData, cf.Coordinate[0].hair);

            // ChaFileHair.PartsInfo
            Extensions.TryGetExtendedDataById(cf.Coordinate[0].hair.parts[0], ExtendedSaveKey, out chaData);
            TestNotNull(chaData, cf.Coordinate[0].hair.parts[0]);
            TestPluginData(chaData, cf.Coordinate[0].hair.parts[0]);

            // ChaFileHair.PartsInfo.ColorInfo
            Extensions.TryGetExtendedDataById(cf.Coordinate[0].hair.parts[0].acsColorInfo[0], ExtendedSaveKey, out chaData);
            TestNotNull(chaData, cf.Coordinate[0].hair.parts[0].acsColorInfo[0]);
            TestPluginData(chaData, cf.Coordinate[0].hair.parts[0].acsColorInfo[0]);

            // ChaFileHair.PartsInfo.BundleInfo
            string key = $"{cf.Coordinate[0].hair.parts[0].Pointer}-0";
            Extensions.TryGetExtendedDataById(cf.Coordinate[0].hair.parts[0], 0, ExtendedSaveKey, out chaData);
            TestNotNull(chaData, cf.Coordinate[0].hair.parts[0].dictBundle[0]);
            TestPluginData(chaData, cf.Coordinate[0].hair.parts[0].dictBundle[0]);

            
            chaData = CreateNewPluginData(cf.Coordinate[0].hair.parts[0].dictBundle[0]);
            Extensions.SetExtendedDataById(cf.Coordinate[0].hair.parts[0], 0, ExtendedSaveKey, chaData);

            // ChaFileGameInfo
            Extensions.TryGetExtendedDataById(cf.GameInfo, ExtendedSaveKey, out chaData);
            TestNotNull(chaData, cf.GameInfo);
            TestPluginData(chaData, cf.GameInfo);

            // ChaFileParameter
            Extensions.TryGetExtendedDataById(cf.Parameter, ExtendedSaveKey, out chaData);
            TestNotNull(chaData, cf.Parameter);
            TestPluginData(chaData, cf.Parameter);

            Log.LogMessage("Tests passed!");
        }

        [HideFromIl2Cpp]
        private PluginData CreateNewPluginData(Il2CppSystem.Object obj)
        {
            PluginData pd = new PluginData();
            pd.data.Add("testKey", obj.GetIl2CppType().FullName);

            return pd;
        }

        [HideFromIl2Cpp]
        private void TestPluginData(PluginData pd, Il2CppSystem.Object obj)
        {
            TestNotNull(pd, obj);
            if (pd.data.TryGetValue("testKey", out object actual))
            {
                if (obj.GetIl2CppType().FullName.Equals(actual))
                {
                    return;
                }
            }

            throw new Exception($"PluginData values incorrect for {obj.GetIl2CppType().Name} -- actual testKey value was {actual.GetType()} {actual}");
        }

        [HideFromIl2Cpp]
        private void TestNotNull(PluginData pd, Il2CppSystem.Object obj)
        {
            if (pd == null)
            {
                throw new Exception($"Plugin data for {obj.GetIl2CppType().Name} not found");
            }
        }

        [HideFromIl2Cpp]
        private void TestValue(PluginData pd, string key, object expected)
        {
            if (pd.data.TryGetValue(key, out object actual))
            {
                if (expected.Equals(actual))
                {
                    return;
                }
            }

            throw new Exception($"Value for key {key} should equal {expected.GetType()} {expected} -- got {actual.GetType()} {actual}");
        }
    }
}
