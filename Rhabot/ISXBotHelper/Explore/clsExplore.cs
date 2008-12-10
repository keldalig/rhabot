using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ISXWoW;
using LavishVMAPI;
using LavishScriptAPI;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace ISXBotHelper.Explore
{
    public class clsExplore
    {
        #region Variables

        private static Thread thread = null;
        private static bool IsStopped = false;

        // Variables
        #endregion

        #region Start / Stop

        public static void Start()
        {
            // exit
            return;

            IsStopped = false;

            // start the clsProcessExplore thread loop
            clsProcessExplore.Start();

            // run if not already running
            if ((thread == null) || (!thread.IsAlive))
            {
                // create new thread
                thread = new Thread(new ThreadStart(Explore_Thread));
                thread.Name = "Explore Items";
                thread.Priority = ThreadPriority.BelowNormal;
                clsSettings.ThreadList.Add(thread);
                thread.Start();
            }
        }

        public static void Stop()
        {
            // stop this thread
            IsStopped = true;

            // stop process explore thread
            clsProcessExplore.Stop();

            clsSettings.KillThread(thread, "Stopping Explore Thread");
        }

        // Start / Stop
        #endregion

        #region Functions

        private static void Explore_Thread()
        {
            try
            {
                clsSettings.Logging.AddToLog("Explore Thread running");

                while (true)
                {
                    // pause/stop scripting
                    if ((IsStopped) || (!clsSettings.TestPauseStop("Explore thread exiting due to script stop")))
                        return;

                    // pause if in combat
                    while (clsCombat.IsInCombat())
                        System.Threading.Thread.Sleep(2000);

                    // search for herbs
                    Search_ExploreObject("-gameobjects,-herb", clsUnitInfo.EUnitType.Herb);
                    System.Threading.Thread.Sleep(1000); // wait so we don't use up fps

                    // mines
                    Search_ExploreObject("-gameobjects,-mine", clsUnitInfo.EUnitType.Mine);
                    System.Threading.Thread.Sleep(1000); // wait so we don't use up fps
                    
                    // chests
                    Search_ExploreObject("-gameobjects,-chest,-not,-herb", clsUnitInfo.EUnitType.Chest);
                    System.Threading.Thread.Sleep(1000); // wait so we don't use up fps
                        
                    // mobs
                    Search_ExploreObject("-units,-nearest,-nocritter,-nopets,-alive,-nonfriendly,-noelite", clsUnitInfo.EUnitType.Mob);
                    System.Threading.Thread.Sleep(1000); // wait so we don't use up fps

                    // elite mobs
                    Search_ExploreObject("-units,-nearest,-nopets,-alive,-nonfriendly-elite", clsUnitInfo.EUnitType.EliteMob);
                    System.Threading.Thread.Sleep(1000); // wait so we don't use up fps

                    // mailboxes
                    Search_ExploreObject("-mailbox", clsUnitInfo.EUnitType.Mailbox);
                    System.Threading.Thread.Sleep(1000); // wait so we don't use up fps

                    // dynamic objects
                    Search_ExploreObject("-dynamicobjects,-not,-herb", clsUnitInfo.EUnitType.Other);
                    System.Threading.Thread.Sleep(1000); // wait so we don't use up fps

                    // NPC's
                    Search_ExploreObject("-units,-friendly,-nopets", clsUnitInfo.EUnitType.NPC);

                    // sleep 30 seconds before redoing cycle
                    System.Threading.Thread.Sleep(30000);
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Explore Thread");
            }
        }

        /// <summary>
        /// Checks if a GUID is valid. Returns false if not valid
        /// </summary>
        /// <returns></returns>
        internal static bool GuidValid(string itemGuid)
        {
            // check for no guid, and len != 16
            if ((string.IsNullOrEmpty(itemGuid)) || (itemGuid.Length != 16))
                return false;
            
            // regext test for hex
            return new Regex(@"^([0-9a-fA-F])*$").IsMatch(itemGuid);
        }

        // Functions
        #endregion

        #region Process Explore

        /// <summary>
        /// Processes the object and add to the explore queue. NOTE: Frame's must be locked when this is called
        /// </summary>
        /// <param name="item">the item to process</param>
        /// <param name="UnitType">unit type</param>
        internal static void ProcessExploreItem(WoWObject item, clsUnitInfo.EUnitType UnitType)
        {
            clsExploreNode node = null;
            WoWUnit unit = null;
            string itemNameL = "";
            List<string> badItems = new List<string>(Regex.Split("mailbox, vein, deposit, campfire, fish, wooden chair, brazier, duel flag, dwarven fire, cozy fire, bonfire, blazing fire, cooking fire, bone fire", ","));

//            try
//            {
                // exit if no item
                if ((item == null) || (!item.IsValid))
                    return;

                // skip if bad guid
                if (!clsExplore.GuidValid(item.GUID))
                    return;

                // skip if no name
                if ((string.IsNullOrEmpty(item.Name)) || (string.IsNullOrEmpty(item.Name.Trim())))
                    return;

                // get the lower name
                itemNameL = item.Name.ToLower();

                // if chest or other, we need to remove mailboxes and mines
                if ((UnitType == clsUnitInfo.EUnitType.Chest) || (UnitType == clsUnitInfo.EUnitType.Other))
                    if (badItems.Contains(itemNameL))
                        return;

                // if chest, remove if "chest" or "box" is not in the name
                if ((UnitType == clsUnitInfo.EUnitType.Chest) &&
                    ((!itemNameL.Contains("chest")) || (!itemNameL.Contains("box"))))
                    return;

                // create new node for item
                node = new clsExploreNode();

                // get location
                node.ExplorePoint = new clsPath.PathPoint(item.Location);

                // zone/sub zone
                node.SubZoneName = clsSettings.isxwow.SubZoneText;
                node.ZoneName = clsSettings.isxwow.ZoneText;
                if (string.IsNullOrEmpty(node.ZoneName))
                    node.ZoneName = clsSettings.isxwow.RealZoneText;

                // unit info
                node.unitInfo.UnitName = item.Name;
                node.unitInfo.UnitGuid = item.GUID;

                // get the item type
                switch (UnitType)
                {
                    // these types carry over
                    case clsUnitInfo.EUnitType.Chest:
                    case clsUnitInfo.EUnitType.Herb:
                    case clsUnitInfo.EUnitType.Mailbox:
                    case clsUnitInfo.EUnitType.Mine:
                    case clsUnitInfo.EUnitType.Other:
                        node.unitInfo.UnitType = UnitType;
                        break;

                    case clsUnitInfo.EUnitType.Mob:
                    case clsUnitInfo.EUnitType.EliteMob:
                        // convert to a unit
                        node.unitInfo = new Explore.clsUnitInfo(UnitType, item.GetUnit());
                        break;
                    case clsUnitInfo.EUnitType.NPC:
                        // convert to a unit
                        unit = item.GetUnit();
                        node.unitInfo = new Explore.clsUnitInfo(UnitType, unit);

                        // see what type of NPC this is
                        if (unit.IsTrainer)
                            node.unitInfo.UnitType = clsUnitInfo.EUnitType.Trainer;
                        else if (unit.IsTaxiMaster)
                            node.unitInfo.UnitType = clsUnitInfo.EUnitType.Flightmaster;
                        else if (unit.IsInnkeeper)
                            node.unitInfo.UnitType = clsUnitInfo.EUnitType.Innkeeper;
                        else if (unit.IsAuctioneer)
                            node.unitInfo.UnitType = clsUnitInfo.EUnitType.Auctioneer;
                        else if (unit.IsStableMaster)
                            node.unitInfo.UnitType = clsUnitInfo.EUnitType.Stablemaster;
                        else if (unit.IsBanker)
                            node.unitInfo.UnitType = clsUnitInfo.EUnitType.Banker;
                        else if (unit.CanRepair)
                            node.unitInfo.UnitType = clsUnitInfo.EUnitType.RepairVendor;
                        else if (unit.IsMerchant)
                            node.unitInfo.UnitType = clsUnitInfo.EUnitType.Vendor;
                        else
                            node.unitInfo.UnitType = clsUnitInfo.EUnitType.NPC;

                        break;
                }

                // add to the process list
                clsProcessExplore.ExploreQueue.Enqueue(node);
//            }
//
//            catch (Exception excep)
//            {
//                clsError.ShowError(excep, "ProcessExploreItem: Process List");
//            }
        }

        /// <summary>
        /// Searches searchString for items
        /// </summary>
        /// <param name="searchString">the string to search</param>
        public static void Search_ExploreObject(string searchString, clsUnitInfo.EUnitType UnitType)
        {
            int gCount = 0;
            WoWObject tempObj = null;

            try
            {
                // get the list of targets
                GuidList gl = null;
                using (new clsFrameLock.LockBuffer())
                {
                    gl = GuidList.New(Regex.Split(searchString, ","));

                    // get the count
                    gCount = (int)gl.Count;

                    // if nothing found, then return
                    if (gCount == 0)
                        return;

                    // pop retList
                    for (int i = 0; i < gCount; i++)
                    {
                        // get the object, skip if null or invalid
                        tempObj = gl.Object((uint)i);
                        if ((tempObj == null) || (!tempObj.IsValid))
                            continue;

                        // process the item and to the list
                        clsExplore.ProcessExploreItem(tempObj, UnitType);
                    }
                }
            }

            catch (Exception excep)
            {
                // log error
                clsError.ShowError(excep, string.Format("Search_ExploreObject: {0}", searchString));
            }
        }

        // Process Explore
        #endregion
    }
}
