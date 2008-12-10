// Hooking Events:
// http://www.isxwow.net/forums/viewtopic.php?f=17&t=928&e=0
// http://www.isxwow.net/forums/viewtopic.php?f=15&t=611

using System.Collections.Generic;
using ISXWoW;

namespace ISXBotHelper
{
    /// <summary>
    /// Holds events that can be raised by the helper classes
    /// </summary>
    public static class clsEvent
    {
        #region Paths

        #region PathPointAdded

        public delegate void PathPointAddedHandler(List<PathListInfo.PathPoint> PathList, PathListInfo.PathPoint PointAdded);
        public static event PathPointAddedHandler PathPointAdded;

        /// <summary>
        /// Raise the added event
        /// </summary>
        /// <param name="PathList"></param>
        /// <param name="PointAdded"></param>
        internal static void Raise_PathPointAdded(List<PathListInfo.PathPoint> PathList, PathListInfo.PathPoint PointAdded)
        {
            if (PathPointAdded != null)
                PathPointAdded(PathList, PointAdded);
        }

        // PathPointAdded
        #endregion

        #region MoveThroughPath

        /// <summary>
        /// Raised when an item is found while moving through the path. Return true to stop moving
        /// </summary>
        /// <param name="ItemFound">the item found</param>
        /// <param name="ItemType">the type of item</param>
        public delegate bool MoveThroughPathFoundItemHandler(WoWUnit ItemFound, clsPath.EItemType ItemType);

        /// <summary>
        /// Raised when an item is found while moving through the path. Return true to stop moving
        /// </summary>
        public static event MoveThroughPathFoundItemHandler MoveThroughPathFoundItem;

        /// <summary>
        /// Raises MoveThroughPathFoundItem. Return true to stop moving
        /// </summary>
        /// <param name="ItemFound">the item found</param>
        /// <param name="ItemType">the type of item</param>
        internal static bool Raise_MoveThroughPathFoundItem(WoWUnit ItemFound, clsPath.EItemType ItemType)
        {
            if (MoveThroughPathFoundItem != null)
                return MoveThroughPathFoundItem(ItemFound, ItemType);
            else
                // event not hooked, so let's get/fight item
                return true;
        }

        // MoveThroughPath
        #endregion

        // Paths
        #endregion

        #region Shutdown

        public delegate void ForcingShutdownHandler();
        public static event ForcingShutdownHandler ForcingShutdown;

        /// <summary>
        /// Raise the forcing shutdown event if it is hooked
        /// </summary>
        public static void Raise_ForcingShutdown()
        {
            if (ForcingShutdown != null)
                ForcingShutdown();
        }

        public delegate void ScriptStopHandler();
        public static event ScriptStopHandler ScriptStop;

        /// <summary>
        /// Raise the Script Stop event if it is hooked
        /// </summary>
        public static void Raise_ScriptStop()
        {
            if (ScriptStop != null)
                ScriptStop();
        }

        // Shutdown
        #endregion

        #region Logging

        public delegate void LogItemReceivedHandler(string LogMessage);
        public static event LogItemReceivedHandler LogItemReceived;
        public static event LogItemReceivedHandler DebugLogItemReceived;

        /// <summary>
        /// Raise the log message received event
        /// </summary>
        /// <param name="LogMessage"></param>
        internal static void Raise_LogItemReceived(string LogMessage)
        {
            if (LogItemReceived != null)
                LogItemReceived(LogMessage);
        }

        /// <summary>
        /// Raise the debug log message received event
        /// </summary>
        /// <param name="LogMessage"></param>
        internal static void Raise_DebugLogItemReceived(string LogMessage)
        {
            if (DebugLogItemReceived != null)
                DebugLogItemReceived(LogMessage);
        }

        // Logging
        #endregion

        #region Character

        #region Level Up

        public delegate void CharacterLeveledHandler(int Level);
        /// <summary>
        /// Raised when a character levels
        /// </summary>
        public static event CharacterLeveledHandler CharacterLeveled;

        /// <summary>
        /// Raised when a character levels
        /// </summary>
        internal static void Raise_CharacterLeveled(int Level)
        {
            if (CharacterLeveled != null)
                CharacterLeveled(Level);
        }

        // Level Up
        #endregion

        #region Durability

        public delegate void CharacterDurabilityLowHandler(int Durability);

        /// <summary>
        /// Raised when the character's durability is low
        /// </summary>
        public static event CharacterDurabilityLowHandler CharacterDurabilityLow;

        /// <summary>
        /// Raised when the character's durability is low
        /// </summary>
        /// <param name="Durability"></param>
        internal static void Raise_CharacterDurabilityLow(int Durability)
        {
            if (CharacterDurabilityLow != null)
                CharacterDurabilityLow(Durability);
        }

        // Durability
        #endregion

        // Character
        #endregion

        #region Ads

        public delegate void AdsReceivedHandler(string NewAd);
        public static event AdsReceivedHandler AdsReceived;

        internal static void Raise_AdsReceived(string NewAd)
        {
            if (AdsReceived != null)
                AdsReceived(NewAd);
        }

        // Ads
        #endregion
    }
}
