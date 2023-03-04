using BepInEx.Logging;
using Chara;
using System;

namespace RGExtendedSave
{
    internal class Events
    {
        private static ManualLogSource Log = RGExtendedSavePlugin.Log;

        // CardEventHandler
        public delegate void CardEventHandler(ChaFile file);
        // Register methods to trigger on card being saved
        public static event CardEventHandler CardBeingSaved;
        // Register methods to trigger on card being loaded
        public static event CardEventHandler CardBeingLoaded;
        // CoordinateEventHandler
        public delegate void CoordinateEventHandler(ChaFileCoordinate file);
        //  Register methods to trigger on coordinate being saved
        public static event CoordinateEventHandler CoordinateBeingSaved;
        // Register methods to trigger on coordinate being loaded
        public static event CoordinateEventHandler CoordinateBeingLoaded;

        internal static void CardWriteEvent(ChaFile file)
        {
            if (CardBeingSaved == null)
                return;

            foreach (var entry in CardBeingSaved.GetInvocationList())
            {
                var handler = (CardEventHandler)entry;
                try
                {
                    handler.Invoke(file);
                }
                catch (Exception ex)
                {
                    Log.LogError($"Subscriber crash in {nameof(ExtendedSave)}.{nameof(CardBeingSaved)} - {ex}");
                }
            }
        }

        internal static void CardReadEvent(ChaFile file)
        {
            if (!ExtendedSave.LoadEventsEnabled || CardBeingLoaded == null)
                return;

            foreach (var entry in CardBeingLoaded.GetInvocationList())
            {
                var handler = (CardEventHandler)entry;
                try
                {
                    handler.Invoke(file);
                }
                catch (Exception ex)
                {
                    Log.LogError($"Subscriber crash in {nameof(ExtendedSave)}.{nameof(CardBeingLoaded)} - {ex}");
                }
            }
        }

        internal static void CoordinateWriteEvent(ChaFileCoordinate file)
        {
            if (CoordinateBeingSaved == null)
                return;

            foreach (var entry in CoordinateBeingSaved.GetInvocationList())
            {
                var handler = (CoordinateEventHandler)entry;
                try
                {
                    handler.Invoke(file);
                }
                catch (Exception ex)
                {
                    Log.LogError($"Subscriber crash in {nameof(ExtendedSave)}.{nameof(CoordinateBeingSaved)} - {ex}");
                }
            }
        }

        internal static void CoordinateReadEvent(ChaFileCoordinate file)
        {
            if (!ExtendedSave.LoadEventsEnabled || CoordinateBeingLoaded == null)
                return;

            foreach (var entry in CoordinateBeingLoaded.GetInvocationList())
            {
                var handler = (CoordinateEventHandler)entry;
                try
                {
                    handler.Invoke(file);
                }
                catch (Exception ex)
                {
                    Log.LogError($"Subscriber crash in {nameof(ExtendedSave)}.{nameof(CoordinateBeingLoaded)} - {ex}");
                }
            }
        }
    }
}
