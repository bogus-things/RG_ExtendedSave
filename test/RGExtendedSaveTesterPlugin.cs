using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using UnityEngine;
using BepInEx.Logging;
using UnhollowerRuntimeLib;

namespace RGExtendedSaveTester
{
    [BepInProcess("RoomGirl")]
    [BepInPlugin(GUID, PluginName, Version)]
    public class RGExtendedSaveTesterPlugin : BasePlugin
    {
        public const string PluginName = "RG ExtendedSave Tester";
        public const string GUID = "com.bogus.RGExtendedSaveTester";
        public const string Version = "0.0.1";
        private const string ComponentName = "BogusComponents";

        internal static new ManualLogSource Log;

        const string CONFIG_SECTION_GENERAL = "General";
        internal static ConfigEntry<KeyCode> LoadKey { get; set; }
        internal static ConfigEntry<KeyCode> TestKey { get; set; }
        internal static ConfigEntry<KeyCode> ClearKey { get; set; }

        public GameObject BogusComponents;

        public override void Load()
        {
            Log = base.Log;

            LoadKey = Config.Bind(CONFIG_SECTION_GENERAL, "Load Data Key", KeyCode.Home, "Press this key to load data onto a character in the editor (save after to write it to the card)");
            TestKey = Config.Bind(CONFIG_SECTION_GENERAL, "Run Tests Key", KeyCode.End, "Press this key to validate the loaded data on a character in the editor");
            ClearKey = Config.Bind(CONFIG_SECTION_GENERAL, "Clear Data Key", KeyCode.PageDown, "Press this key to clear the loaded data on a character in the editor");

            ClassInjector.RegisterTypeInIl2Cpp<TestController>();

            BogusComponents = GameObject.Find(ComponentName);
            if (BogusComponents == null)
            {
                BogusComponents = new GameObject(ComponentName);
                GameObject.DontDestroyOnLoad(BogusComponents);
                BogusComponents.hideFlags = HideFlags.HideAndDontSave;
            }
            BogusComponents.AddComponent(Il2CppType.Of<TestController>());
        }
    }
}
