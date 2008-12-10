using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using ISXBotHelper.Explore;
using ISXBotHelper.Properties;
using ISXRhabotGlobal;
using ISXWoW;
using ISXWoW.WoWUI.Frames;

namespace ISXBotHelper.AutoNav
{
    public class clsAutoNav
    {
        #region Enums

        public enum EAutoNavResult
        {
            /// <summary>
            /// Navigated successfully
            /// </summary>
            Success = 0,

            /// <summary>
            /// Stuck, can not move
            /// </summary>
            Stuck,

            /// <summary>
            /// Error
            /// </summary>
            Error,

            /// <summary>
            /// Script stopped
            /// </summary>
            Stopped
        }

        // Enums
        #endregion

        #region Variables

        private bool IsStopped = false;
        private static List<clsMonsterKill> MonsterKills = new List<clsMonsterKill>();

        // Variables
        #endregion

        #region Init

        /// <summary>
        /// Initializes a new instance of the clsAutoNav class.
        /// </summary>
        public clsAutoNav()
        {
        }

        // Init
        #endregion

        #region Monster Kill List

        private class clsMonsterKill
        {
            public string MonsterName = string.Empty;
            public int Count = 1;
        }

        // Monster Kill List
        #endregion

        #region Auto Nav Path

        /// <summary>
        /// Runs the auto nav path
        /// </summary>
        /// <param name="AutoNavList">list of autonav actions to perform</param>
        public EAutoNavResult AutoNavPath(List<clsBlockList> AutoNavList)
        {
            clsBlockList blockAction = null;
            List<clsBlockList> Conditions = new List<clsBlockList>();
            List<clsBlockList> blockList = new List<clsBlockList>();
            int i = 0, j = 0;
            WoWUnit tempUnit = null;
            clsQuest quest = new clsQuest();
            DateTime StartTime = DateTime.MinValue;
            EAutoNavResult result = EAutoNavResult.Error;
            clsPath.PathListInfoEx pInfo = null;
            string[] pSplit = null;

            try
            {
                // exit if stopped
                if (!clsSettings.TestPauseStop(Resources.AutoNavPath, Resources.ExitingDueToScriptStop))
                    return EAutoNavResult.Stopped;

                // log it
                clsSettings.Logging.AddToLog(Resources.AutoNav, Resources.AutoNavBeginExec);

                // exit if invalid guid
                if (!clsSettings.GuidValid)
                    return EAutoNavResult.Error;

                // exit if no items in list
                if ((AutoNavList == null) || (AutoNavList.Count == 0))
                    return EAutoNavResult.Success;

                // stop moving
                clsPath.StopMoving();

                // loop through each item in the list. pop conditions and block list stuff
                j = AutoNavList.Count;
                for (i = 0; i < j; i++)
                {
                    // choose how to handle it
                    switch (AutoNavList[i].BlockItem.NodeType)
                    {
                        case clsBlockListItem.ENodeType.Continue_Durability_X_Percent:
                        case clsBlockListItem.ENodeType.Continue_Until_Bags_Full:
                        case clsBlockListItem.ENodeType.Continue_Until_Character_Is_Level_X:
                        case clsBlockListItem.ENodeType.Continue_Until_X_Item_Y_Quantity:
                        case clsBlockListItem.ENodeType.Continue_Until_X_Mob_Y_Killed:
                            Conditions.Add(AutoNavList[i]);
                            break;

                        default:
                            blockList.Add(AutoNavList[i]);
                            break;
                    }
                }

                // loop through until conditions are met
                j = blockList.Count;
                for (i = 0; i < j; i++)
                {
                    blockAction = blockList[i];

                    switch (blockAction.BlockItem.NodeType)
                    {
                    	case clsBlockListItem.ENodeType.Go_To_Vendor_X_City_Y:
                        case clsBlockListItem.ENodeType.Go_To_Repair_Vendor_X_City_Y:
                            // run to the vendor
                            tempUnit = clsAutoNavTop.GoToVendorX(blockAction.BlockItem);

                            // process sub units if we have any
                            if ((tempUnit != null) && (tempUnit.IsValid) && (blockAction.SubBlocks != null) && (blockAction.SubBlocks.Count > 0))
                            {
                                foreach (clsBlockList vbli in blockAction.SubBlocks)
                                    clsVendor.BuyItem(tempUnit, vbli.BlockItem.DestName, vbli.BlockItem.Quantity);
                            }

                    		break;

                        case clsBlockListItem.ENodeType.Go_To_Flightmaster_X_City_Y:
                            // run to flight master
                            tempUnit = clsAutoNavTop.GoToFlightmaster(blockAction.BlockItem);

                            //if ((blockAction.SubBlocks != null) && (blockAction.SubBlocks.Count > 0))
                                // TODO: fly from X to Y. blockAction.SubBlocks[0];
                            break;

                        case clsBlockListItem.ENodeType.Go_To_Trainer_X_City_Y:
                            // run to trainer
                            clsAutoNavTop.GoToTrainer(blockAction.BlockItem);
                            break;

                        case clsBlockListItem.ENodeType.Take_Boat_To_City_X:
                            // take boat
                            clsSettings.Logging.AddToLogFormatted(Properties.Resources.AutoNav, "Running to boat at {0}", blockAction.BlockItem.DestinationPoint.ToString());

                            // run to the boat location
                            if (! clsAutoNavTop.RunToXYZ(new PathListInfo.PathPoint(blockAction.BlockItem.DestinationPoint), false))
                            {
                                clsSettings.Logging.AddToLog(Properties.Resources.AutoNav,"Could not navigation path to boat");
                                break;
                            }

                            // TODO: get on the boat (see runningman)
                            break;

                        case clsBlockListItem.ENodeType.Run_Rhabot_Path:
                            // get the file and path name
                            pSplit = Regex.Split(blockAction.BlockItem.DestName, ":::");

                            // TODO: pSplit[0] should be the pathname, pSplit[1] should be the group
                            // TODO: path level should be 0 (?)
                            pInfo = clsPath.LoadPathList(pSplit[0], pSplit[1], 0);

                            // skip if no path loaded
                            if (pInfo == null)
                            {
                                clsSettings.Logging.AddToLogFormatted(Resources.AutoNav, "Could not load Rhabot path {0}, {1}", pSplit[1], pSplit[0]);
                                break;
                            }

                            // run the path
                            if (clsPath.RunPath(pInfo,true, false) != clsPath.EMovementResult.Success)
                            {
                                clsSettings.Logging.AddToLogFormatted(Resources.AutoNav, "Could not run Rhabot path '{0}'", pSplit[1]);
                                break;
                            }

                            break;

                        case clsBlockListItem.ENodeType.Go_To_Innkeeper_X_City_Y:
                            // run to innkeeper
                            clsAutoNavTop.GoToInnkeeper(blockAction.BlockItem, ((blockAction.SubBlocks != null) && (blockAction.SubBlocks.Count > 0)));
                            break;

                        case clsBlockListItem.ENodeType.Stone_Home:
                            // stone home
                            clsCharacter.StoneHome();
                            break;

                        case clsBlockListItem.ENodeType.Go_To_Mailbox_X_City_Y:
                            // go to mailbox
                            clsAutoNavTop.GoToMailbox(blockAction.BlockItem);
                            break;

                        case clsBlockListItem.ENodeType.Speak_To_Person_X_At_Y:
                            // go to person X at Y
                            tempUnit = clsAutoNavTop.GoToUnit(blockAction.BlockItem, string.Format("-units,{0}", blockAction.BlockItem.DestName));

                            // if we have a unit, then cycle the sub blocks
                            if ((tempUnit != null) && (tempUnit.IsValid) && (blockAction.SubBlocks != null) && (blockAction.SubBlocks.Count > 0))
                            {
                                foreach (clsBlockList pbl in blockAction.SubBlocks)
                                {
                                    // target and use the unit if the window is not open
                                    if (! UI.GossipOptionsFrame.IsVisible)
                                    {
                                        clsSettings.Logging.AddToLogFormatted(Resources.AutoNav, "Targetting and using '{0}'", tempUnit.Name);

                                        // target and use
                                        tempUnit.Target();
                                        tempUnit.Use();
                                        Thread.Sleep(1000);
                                    }

                                    // skip if no ui
                                    if (! UI.GossipOptionsFrame.IsVisible)
                                    {
                                        clsSettings.Logging.AddToLog(Resources.AutoNav, "Skipping gossip dialog. Gossip dialog is not displayed");
                                        break;
                                    }

                                    // select gossip option
                                    clsSettings.Logging.AddToLogFormatted(Resources.AutoNav, "Selecting gossip option '{0}'", pbl.BlockItem.Quantity);
                                    UI.GossipOptionsFrame.SelectGossipOption((uint)pbl.BlockItem.Quantity);

                                    // sleep so it can refresh
                                    Thread.Sleep(1000);
                                }
                            }
                            break;

                        case clsBlockListItem.ENodeType.Go_To_QuestGiver_X_At_Y:
                            // go to the questgiver
                            tempUnit = clsAutoNavTop.GoToUnit(blockAction.BlockItem, string.Format("-quest,{0}", blockAction.BlockItem.DestName));

                            // skip if no one found
                            if ((tempUnit == null) || (! tempUnit.IsValid))
                            {
                                clsSettings.Logging.AddToLogFormatted(Resources.AutoNav, "Could not find questgiver '{0}'", blockAction.BlockItem.DestName);
                                break;
                            }
                            
                            // cycle through and do the quest actions
                            if ((blockAction.SubBlocks != null) && (blockAction.SubBlocks.Count > 0))
                            {
                                foreach (clsBlockList qbl in blockAction.SubBlocks)
                                {
                                    // open the gossip window
                                    if (!UI.GossipOptionsFrame.IsVisible)
                                    {
                                        clsSettings.Logging.AddToLogFormatted(Resources.AutoNav, "Targetting and using '{0}'", tempUnit.Name);
                                        tempUnit.Target();
                                        tempUnit.Use();
                                        Thread.Sleep(2000);
                                    }

                                    // pick up a quest
                                    if (qbl.BlockItem.NodeType == clsBlockListItem.ENodeType.Pick_Up_Quest_X)
                                    {
                                        // hook the quest
                                        quest = new clsQuest();
                                        quest.Hook_QuestDetails();

                                        // log it
                                        clsSettings.Logging.AddToLogFormatted(Resources.AutoNav, "Picking up quest '{0}'", qbl.BlockItem.DestName);

                                        // get the quest
                                        if (! clsQuest.SelectQuestByName(qbl.BlockItem.DestName))
                                        {
                                            clsSettings.Logging.AddToLogFormatted(Resources.AutoNav, "could not get quest with name '{0}'", qbl.BlockItem.DestName);
                                            break;
                                        }
                                    }

                                    // turn in a quest
                                    if (qbl.BlockItem.NodeType == clsBlockListItem.ENodeType.Turn_In_Quest_Y)
                                    {
                                        // hook the quest
                                        quest = new clsQuest();
                                        quest.Hook_QuestComplete();

                                        // log it
                                        clsSettings.Logging.AddToLogFormatted(Resources.AutoNav, "Turning in quest '{0}'", qbl.BlockItem.DestName);

                                        // turn in the quest
                                        if (!clsQuest.SelectActiveQuestByName(qbl.BlockItem.DestName))
                                        {
                                            clsSettings.Logging.AddToLogFormatted(Resources.AutoNav, "Could not turn in quest with name '{0}'", qbl.BlockItem.DestName);
                                            break;
                                        }
                                    }

                                    // sleep 5 seconds so we can get/turn in the quest
                                    StartTime = DateTime.Now;
                                    while (new TimeSpan(DateTime.Now.Ticks - StartTime.Ticks).Seconds < 5)
                                    {
                                        Application.DoEvents();
                                        Thread.Sleep(100);
                                    }

                                    // kill the quest object
                                    quest = null;
                                }
                            }

                            break;

                        case clsBlockListItem.ENodeType.Go_To_XYZ:
                            // run to XYZ
                            clsAutoNavTop.GoToXYZ(blockAction);
                            break;

                        case clsBlockListItem.ENodeType.Fish_At_XYZ:
                            // fishing
                            clsAutoNavTop.FishAtXYZ(blockAction);
                            break;
                    }
                }

                result = EAutoNavResult.Success;
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.AutoNav);
            }

            return result;
        }

        // Auto Nav Path
        #endregion

        #region Auto Nav Sub Path

        /// <summary>
        /// Performs the Auto Navigation functions for sub blocks
        /// </summary>
        /// <param name="BlockList">The block list.</param>
        private EAutoNavResult AutoNavPath_Sub(List<clsBlockList> SubBlockList)
        {
            EAutoNavResult result = EAutoNavResult.Error;
            List<clsBlockListItem> Conditions = new List<clsBlockListItem>();
            List<clsBlockList> BlockList = new List<clsBlockList>(); // cloned list of SubBlockList
            List<clsBlockList> removeList = new List<clsBlockList>();
            List<clsVendor.clsNeedVendorItem> NeedVendorList = new List<clsVendor.clsNeedVendorItem>();
            List<clsBlockListItem> NeedVendorBuyList = new List<clsBlockListItem>();
            List<PathListInfo.PathPoint> PathPointList = new List<PathListInfo.PathPoint>(); // list of points to visit, before path is made
            List<PathListInfo.PathPoint> MobExcludeList = new List<PathListInfo.PathPoint>();
            List<string> ObjectsToSearch = new List<string>();
            clsPath.PathListInfoEx PathInfo = new clsPath.PathListInfoEx(), vendorPath = new clsPath.PathListInfoEx();
            clsPath.PathListInfoEx tempPath = new clsPath.PathListInfoEx(), startHuntPath = new clsPath.PathListInfoEx();
            int counter = 0, bCount = 0;
            int pCounter = 0, pCount = 0;
            clsBlockListItem bItem = null;
            string fName = "AutoNav - SubBlock:", DestName = string.Empty;
            clsPath.EMovementResult moveResult = clsPath.EMovementResult.Success;
            clsPath cPath = new clsPath();
            PathListInfo.PathPoint vendorPoint = null;
            clsQuest quest = null;
            bool HasFullRhabotPath = false;

            try
            {
                clsSettings.Logging.AddToLogFormatted(Resources.AutoNavPath, "Beginning Sub Block Navigation");

                // exit if invalid guid
                if (!clsSettings.GuidValid)
                    return EAutoNavResult.Error;

                // exit if no items in list
                if ((SubBlockList == null) || (SubBlockList.Count == 0))
                    return EAutoNavResult.Success;

                // clear monster kills list
                MonsterKills.Clear();

                // stop moving
                clsPath.StopMoving();

                // clone the list and fill conditions list
                foreach (clsBlockList bl in SubBlockList)
                {
                    switch (bl.BlockItem.NodeType)
                    {
                    	case clsBlockListItem.ENodeType.Continue_Durability_X_Percent:
                        case clsBlockListItem.ENodeType.Continue_Until_Bags_Full:
                        case clsBlockListItem.ENodeType.Continue_Until_X_Item_Y_Quantity:
                        case clsBlockListItem.ENodeType.Continue_Until_X_Mob_Y_Killed:
                        case clsBlockListItem.ENodeType.Continue_Until_Character_Is_Level_X:
                            Conditions.Add(bl.BlockItem);
                    		break;

                        // need vendor list
                        case clsBlockListItem.ENodeType.Return_To_Vendor_When_X_Has_Quantity_Y:
                            if ((bl.SubBlocks != null) && (bl.SubBlocks.Count > 0) && (bl.BlockItem.DestinationPoint != null))
                            {
                                // add to the need list, and the need buy list
                                NeedVendorList.Add(new clsVendor.clsNeedVendorItem(bl.BlockItem.Quantity, bl.BlockItem.DestName));

                                foreach (clsBlockList nvbl in bl.SubBlocks)
                                    NeedVendorBuyList.Add(nvbl.BlockItem);
                                vendorPoint = new PathListInfo.PathPoint(bl.BlockItem.DestinationPoint);
                            }
                            break;

                        // search for objects
                        case clsBlockListItem.ENodeType.Pickup_Item_Named_X_Zone_Y:
                        case clsBlockListItem.ENodeType.Pickup_Item_Named_X_Zone_Y_MULTI:
                            ObjectsToSearch.Add(bl.BlockItem.DestName);
                            BlockList.Add(bl.Clone());
                            break;

                        default: // clone it to the new list
                            BlockList.Add(bl.Clone());
                            break;
                    }
                }

                // exit if blocklist is empty
                bCount = BlockList.Count;
                if (bCount == 0)
                    return EAutoNavResult.Success;

                #region Vendor/NPC Actions

                // loop through actions in the list to perform anything we can do now
                removeList = new List<clsBlockList>();
                for (counter = 0; counter < bCount; counter++)
                {
                    // exit if stopped
                    if ((IsStopped) || (clsSettings.TestPauseStop(string.Format("{0} Exiting because of script stop", fName))))
                        return EAutoNavResult.Stopped;

                    // get the item
                    bItem = BlockList[counter].BlockItem;

                    switch (bItem.NodeType)
                    {
                        case clsBlockListItem.ENodeType.Make_Inn_Home:
                            // make this inn your home. the innkeeper dialog window should already be open
                            if ((UI.GossipOptionsFrame.IsVisible) &&
                                (UI.GossipOptionsFrame.HasOption(GossipFrame.GossipTypes.Binder)))
                            {
                                clsSettings.Logging.AddToLogFormatted(Resources.AutoNav, "{0} Making Inn Home", fName);
                                UI.GossipOptionsFrame.SelectOption(GossipFrame.GossipTypes.Binder);
                                Thread.Sleep(1000);
                            }
                            removeList.Add(BlockList[counter]);
                            break;

                        case clsBlockListItem.ENodeType.Pick_Up_Quest_X:
                            // get the quest with the specified name, quest window must already be open
                            clsSettings.Logging.AddToLogFormatted(Resources.AutoNav, "{0} Picking up Quest '{1}'", fName, bItem.DestName);
                            clsQuest.SelectQuestByName(bItem.DestName);
                            clsQuest.AcceptQuest();
                            removeList.Add(BlockList[counter]);
                            break;

                        case clsBlockListItem.ENodeType.Purchase_X_of_Y:
                            // purchases the item, vendor purchase window must already be open
                            if (UI.MerchantFrame.IsVisible)
                            {
                                clsSettings.Logging.AddToLogFormatted(Resources.AutoNav, "{0} Purchasing {1} of '{2}'", fName, bItem.Quantity, bItem.DestName);
                                if (!UI.MerchantFrame.BuyItem(bItem.DestName, (uint)bItem.Quantity))
                                    clsSettings.Logging.AddToLogFormatted(Resources.AutoNav, "{0} Could not purchase item. Skipping", fName);
                            }
                            removeList.Add(BlockList[counter]);
                            break;

                        case clsBlockListItem.ENodeType.Take_Flight_X_To_Y:
                            // TODO: fly
                            removeList.Add(BlockList[counter]);
                            break;

                        case clsBlockListItem.ENodeType.Turn_In_Quest_Y:
                            // turn in the quest. the quest window must be open
                            clsSettings.Logging.AddToLogFormatted(Resources.AutoNav, "{0} Turning in Quest '{1}'", fName, bItem.DestName);
                            quest = new clsQuest();
                            quest.Hook_QuestComplete();
                            clsQuest.SelectActiveQuestByName(bItem.DestName);
                            removeList.Add(BlockList[counter]);
                            Thread.Sleep(2000);
                            quest = null;
                            break;

                        case clsBlockListItem.ENodeType.Use_Item_X:
                            // get the item
                            List<WoWItem> useList = clsSearch.Search_Item(string.Format("-inventory,-items,{0}", bItem.DestName));

                            // use it if something returned
                            if ((useList != null) && (useList.Count > 0))
                            {
                                clsSettings.Logging.AddToLogFormatted(Resources.AutoNav, "{0} Using item '{1}'", fName, useList[0].FullName);
                                useList[0].Use();
                            }
                            else
                                clsSettings.Logging.AddToLogFormatted(Resources.AutoNav, "{0} Could not use item '{0}'. No matching items found in inventory", fName, bItem.DestName);
                            removeList.Add(BlockList[counter]);
                            break;
                    }
                }

                // remove items that were processed
                foreach (clsBlockList rItem in removeList)
                    BlockList.Remove(rItem);
                removeList = null;

                // Vendor/NPC Actions
                #endregion

                #region Rhabot Full Path

                // if we have a full path in the list, then use it
                HasFullRhabotPath = false;
                foreach (clsBlockList rbList in SubBlockList)
                {
                    // exit if we found a full rhabot path
                    if (rbList.BlockItem.NodeType == clsBlockListItem.ENodeType.Run_Rhabot_Path_Full)
                    {
                        DestName = rbList.BlockItem.DestName;
                        HasFullRhabotPath = true;
                        break;
                    }
                }

                // if we have a full path, run it
                if (HasFullRhabotPath)
                {
                    List<string> ObjToFind = new List<string>();
                    clsFullPath fPath = new clsFullPath();

                    // build the list of objects to find
                    foreach (clsBlockList rbList in SubBlockList)
                    {
                        // objects to find
                        if ((rbList.BlockItem.NodeType == clsBlockListItem.ENodeType.Pickup_Item_Named_X_Zone_Y) ||
                            (rbList.BlockItem.NodeType == clsBlockListItem.ENodeType.Pickup_Item_Named_X_Zone_Y_MULTI))
                            ObjToFind.Add(rbList.BlockItem.DestName);
                    }

                    // run the path
                    while (!ConditionsMet(Conditions))
                    {
                        fPath = new clsFullPath();

                        // run and get result
                        clsPath.EMovementResult rResult = fPath.RunPath(DestName, ObjToFind);

                        switch (rResult)
                        {
                            case clsPath.EMovementResult.Dead:
                                if (!new clsGhost().HandleDead(fPath.ActivePath, fPath.GraveyardPaths))
                                    return EAutoNavResult.Error;
                                break;

                            case clsPath.EMovementResult.Error:
                                return EAutoNavResult.Error;

                            case clsPath.EMovementResult.PathObstructed:
                            case clsPath.EMovementResult.Stuck:
                                return EAutoNavResult.Stuck;

                            case clsPath.EMovementResult.Stopped:
                                return EAutoNavResult.Stopped;
                        }
                    }

                    // return success
                    return EAutoNavResult.Success;
                }

                // Rhabot Full Path
                #endregion

                #region Build List of Points to Visit

                // if no items left in list, return success
                if (BlockList.Count == 0)
                    return EAutoNavResult.Success;

                // loop through blocklist and get all the path points we need
                foreach (clsBlockList bList in BlockList)
                {
                    switch (bList.BlockItem.NodeType)
                    {
                        case clsBlockListItem.ENodeType.Fight_Elite_Mobs_Zone_X:
                            PathPointList.AddRange(XML_GetPointsForName(bList.BlockItem.ZoneName, string.Empty, clsUnitInfo.EUnitType.EliteMob));
                            break;
                            
                        case clsBlockListItem.ENodeType.Fight_Elite_Mobs_Zone_X_Named_Y:
                            PathPointList.AddRange(XML_GetPointsForName(bList.BlockItem.ZoneName, bList.BlockItem.DestName, clsUnitInfo.EUnitType.EliteMob));
                            break;

                        case clsBlockListItem.ENodeType.Fight_Mobs_Zone_X:
                            PathPointList.AddRange(XML_GetPointsForName(bList.BlockItem.ZoneName, string.Empty, clsUnitInfo.EUnitType.Mob));
                            break;

                        case clsBlockListItem.ENodeType.Fight_Mobs_Zone_X_Named_Y:
                            PathPointList.AddRange(XML_GetPointsForName(bList.BlockItem.ZoneName, bList.BlockItem.DestName, clsUnitInfo.EUnitType.Mob));
                            break;
                            
                        case clsBlockListItem.ENodeType.Find_Chests_Zone_X:
                            PathPointList.AddRange(XML_GetPointsForName(bList.BlockItem.ZoneName, string.Empty, clsUnitInfo.EUnitType.Chest));
                            break;

                        case clsBlockListItem.ENodeType.Find_Herbs_Zone_X:
                            PathPointList.AddRange(XML_GetPointsForName(bList.BlockItem.ZoneName, string.Empty, clsUnitInfo.EUnitType.Herb));
                            break;

                        case clsBlockListItem.ENodeType.Find_Mines_Zone_X:
                            PathPointList.AddRange(XML_GetPointsForName(bList.BlockItem.ZoneName, string.Empty, clsUnitInfo.EUnitType.Mine));
                            break;

                        case clsBlockListItem.ENodeType.Pickup_Item_Named_X_Zone_Y:
                        case clsBlockListItem.ENodeType.Pickup_Item_Named_X_Zone_Y_MULTI:
                            PathPointList.AddRange(XML_GetPointsForName(bList.BlockItem.ZoneName, bList.BlockItem.DestName, clsUnitInfo.EUnitType.Other));
                            break;

                        case clsBlockListItem.ENodeType.Go_To_XYZ:
                            PathPointList.Add(new PathListInfo.PathPoint(bList.BlockItem.DestinationPoint));
                            break;

                        case clsBlockListItem.ENodeType.Exclude_Mobs_Zone_X_Named_Y: // remove these points from the list
                            // get the list of mob locations
                            MobExcludeList = XML_GetPointsForName(bList.BlockItem.ZoneName, bList.BlockItem.DestName, clsUnitInfo.EUnitType.Mob);
                            break;
                    }                    
                }

                // remove excluded mobs
                if ((MobExcludeList != null) && (MobExcludeList.Count > 0))
                {
                    foreach (PathListInfo.PathPoint pPoint in MobExcludeList)
                        PathPointList.Remove(pPoint);
                }

                // sort the list of path points by order of distance
                PathPointList = SortPathPoints(PathPointList);

                // Build List of Points to Visit
                #endregion

                #region Build Nav Path

                // TODO: this path we create has duplicate points where the paths are merged
                //  we may need to remove one of the dup points. TEST

                // loop through the pathpoint list to create a complete path
                pCount = PathPointList.Count - 1;
                for (pCounter = 0; pCounter < pCount; pCounter++)
                {
                    // TODO: use AutoNav path builder
                    //tempPath = clsPath.GreyNav_BuildPath(PathPointList[pCount], PathPointList[pCount + 1]);
                    if ((tempPath.PathList != null) && (tempPath.PathList.Count > 0))
                        PathInfo.PathList.AddRange(tempPath.PathList);
                    else
                        PathInfo.PathList.Add(PathPointList[pCount + 1]);
                }

                // build the hunt start path
                if (clsCharacter.MyLocation.Distance(PathInfo.PathList[0]) > 100)
                    // TODO: use AutoNav path builder
                    //startHuntPath = clsPath.GreyNav_BuildPath(clsCharacter.MyLocation, PathInfo.PathList[0]);
                    startHuntPath = null;
                else
                    startHuntPath = null;

                // Build Nav Path
                #endregion

                #region Navigate Path

                // run start hunt path first
                if ((startHuntPath != null) && (startHuntPath.PathList.Count > 0))
                {
                    clsSettings.Logging.AddToLog("AutoNav Sub: Running Start Hunt Path");

                    // run, exit if not successful
                    if (clsPath.RunPath(startHuntPath, true, false) != clsPath.EMovementResult.Success)
                    {
                        clsSettings.Logging.AddToLogFormatted(Resources.AutoNav, "Exiting. Could not complete Start Hunt Path");
                        return EAutoNavResult.Stopped;
                    }
                }

                // hook the monster kill event so we can update the list
                clsGlobals.MobKilled += new clsGlobals.MobKilledHandler(clsGlobals_MobKilled);

                // run the hunt path
                while (true)
                {
                    // exit if stopped
                    if ((IsStopped) || (clsSettings.TestPauseStop("AutoNavPath: Exiting due to script stop")))
                        return EAutoNavResult.Stopped;

                    // move through the path
                    moveResult = clsPath.RunPath(PathInfo, true, true, NeedVendorList, ObjectsToSearch, null);

                    // keep looping until we have success or failure
                    while (moveResult == clsPath.EMovementResult.Success)
                    {
                        // if shutting down
                        if ((IsStopped) || (clsSettings.TestPauseStop("AutoNavPath: Exiting due to script stop")))
                            return EAutoNavResult.Stopped;

                        // check if conditions met
                        if (ConditionsMet(Conditions))
                            return EAutoNavResult.Success;

                        // reverse direction
                        clsSettings.Logging.AddToLog("AutoNavPath: Reverse path direction");
                        PathInfo.ReversePath();

                        // continue moving through the path
                        clsSettings.Logging.AddToLog("AutoNavPath: Continue moving through the path");
                        moveResult = clsPath.RunPath(PathInfo, true, true, NeedVendorList, ObjectsToSearch, null);
                    }

                    // return error if not vendor
                    if (moveResult != clsPath.EMovementResult.NeedVendor)
                        return EAutoNavResult.Stopped;

                    #region Vendor Run

                    // vendor, run to vendor and launch sub thread if we have purchase items
                    // NeedVendorBuyList is items to buy at vendor
                    if (vendorPoint != null)
                    {
                        // TODO: use AutoNav path builder
                        //vendorPath = clsPath.GreyNav_BuildPath(clsCharacter.MyLocation, vendorPoint);

                        // run the path if we got a result
                        if ((vendorPath != null) && (vendorPath.PathList != null) && (vendorPath.PathList.Count > 0))
                        {
                            clsSettings.Logging.AddToLog("AutoNavPath: Returning to vendor");
                            if (cPath.MoveThroughPathEx(vendorPath, true, false) == clsPath.EMovementResult.Success)
                            {
                                // we are now at the vendor, open the vendor panel
                                WoWUnit wvendor = clsVendor.GetNearestVendor(true);
                                if ((wvendor != null) && (wvendor.IsValid))
                                {
                                    // sell and repair
                                    clsVendor.SellToVendor(wvendor);

                                    // buy what we need
                                    if (UI.MerchantFrame.IsVisible)
                                    {
                                        foreach (clsBlockListItem vbli in NeedVendorBuyList)
                                        {
                                            try
                                            {
                                                // buy this item
                                                clsSettings.Logging.AddToLogFormatted(Resources.AutoNavPath, "Buy {0} of '{1}", vbli.Quantity, vbli.DestName);
                                                UI.MerchantFrame.BuyItem(vbli.DestName, (uint)vbli.Quantity);
                                            }

                                            catch (Exception excep)
                                            {
                                                clsError.ShowError(excep, "AutoNavPath: Buying from vendor");
                                            }
                                        }
                                    }
                                }

                                // we need to build a path from our current location back to
                                // our hunt path
                                // TODO: use AutoNav path builder
                                //vendorPath = clsPath.GreyNav_BuildPath(clsCharacter.MyLocation, PathInfo.PathList[0]);

                                // throw error if no path
                                if ((vendorPath != null) && (vendorPath.PathList == null) || (vendorPath.PathList.Count == 0))
                                {
                                    clsError.ShowError(new Exception("AutoNavPath: Unable to return to hunt path"), string.Empty, true, new StackFrame(0, true), false);
                                    return EAutoNavResult.Error;
                                }

                                // merge the new path into the current path, reset currentstep
                                PathInfo.PathList.InsertRange(0, vendorPath.PathList);
                                PathInfo.CurrentStep = 0;
                            }
                        }
                    }

                    // Vendor Run
                    #endregion
                }

                //Navigate Path
                #endregion

                // if MTPex returns NeedVendor, go to vendor. Run new AutoNavPath_Sub once we
                //  reach the vendor, to buy items in the NeedVendorBuyList
                result = EAutoNavResult.Success;
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "AutoNavPath");
                result = EAutoNavResult.Error;
            }

            finally
            {
                clsSettings.Logging.AddToLogFormatted(Resources.AutoNavPath, "Exiting '{0}'", result.ToString());
            }

            return result;
        }

        /// <summary>
        /// Raised when a mob is killed. Update the mob kill list
        /// </summary>
        void clsGlobals_MobKilled(string MobName, int Level)
        {
            try
            {
                // re-init if null
                if (MonsterKills == null)
                    MonsterKills = new List<clsMonsterKill>();

                // loop through the list to see if we already have this monster
                foreach (clsMonsterKill mKill in MonsterKills)
                {
                    // if we find a match, update and exit
                    if (mKill.MonsterName == MobName)
                    {
                        mKill.Count++;
                        return;
                    }
                }

                // no matches found, add a new one
                MonsterKills.Add(new clsMonsterKill() { MonsterName = MobName });
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.AutoNav, "Mob Killed Event");
            }            
        }

        // Auto Nav Sub Path
        #endregion

        #region Read Explore Map File

        /// <summary>
        /// Gets pathpoints that meet the specified criteria
        /// </summary>
        /// <param name="Section">zone name</param>
        /// <param name="ItemName">mob/item name. blank for non-named items</param>
        /// <param name="ItemType">string of the item type to find</param>
        private List<PathListInfo.PathPoint> XML_GetPointsForName(string Section, string ItemName, clsUnitInfo.EUnitType UnitType)
        {
            return null;
            /*
            List<PathListInfo.PathPoint> retList = new List<PathListInfo.PathPoint>();
            Xml MapXML = null;
            string[] entries = null;
            string tempStr = string.Empty;
            bool Added = false;

            // wait until we can lock the xml file
            while (Explore.clsProcessExplore.FileLocked)
            {
                clsSettings.Logging.DebugWrite("XML_GetPointsForName: Waiting for explore file to unlock");
                System.Threading.Thread.Sleep(200);
            }

            // lock the explore file
            Explore.clsProcessExplore.FileLocked = true;

            try
            {
                // uppercase itemname
                if (! string.IsNullOrEmpty(ItemName))
                    ItemName = ItemName.Trim().ToUpper();

                // open the xml file
                MapXML = new Xml(clsSettings.ExplorePath);

                // get all entries for this section
                entries = MapXML.GetEntryNames(Section);

                // loop through and find matches
                foreach (string entry in entries)
                {
                    // reset added flag
                    Added = false;

                    // add by name
                    if (!string.IsNullOrEmpty(ItemName))
                    {
                        // get the item name
                        tempStr = (string)MapXML.GetValue(Section, entry);

                        // mark that we added this item, so we don't add it again
                        if (tempStr.Trim().ToUpper() == ItemName)
                            Added = true;
                    }

                    // add by type
                    if ((!Added) && (!string.IsNullOrEmpty(MType)))
                    {
                        // get the item type
                        tempStr = MapXML.GetValue_Attribute(Section, entry, "Type");

                        // mark it if it matchs
                        if (tempStr == MType)
                            Added = true;
                    }

                    // add the point if we found something
                    if (Added)
                    {
                        retList.Add(new PathListInfo.PathPoint(
                            Convert.ToDouble(MapXML.GetValue_Attribute(Section, entry, "X")),
                            Convert.ToDouble(MapXML.GetValue_Attribute(Section, entry, "Y")),
                            Convert.ToDouble(MapXML.GetValue_Attribute(Section, entry, "Z"))));

                    }
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "XML_GetPointsForName");
            }
            
            finally
            {
                // reset locked flag
                Explore.clsProcessExplore.FileLocked = false;
            }

            return retList;
            */
        }

        // Read Explore Map File
        #endregion

        #region Sort Path Points

        /// <summary>
        /// Sorts the path point list by distance
        /// </summary>
        /// <param name="PathPointList">the list to sort</param>
        private List<PathListInfo.PathPoint> SortPathPoints(List<PathListInfo.PathPoint> PathPointList)
        {
            PathListInfo.PathPoint LastPoint = clsCharacter.MyLocation;
            PathListInfo.PathPoint testPoint = null;
            List<PathListInfo.PathPoint> retList = new List<PathListInfo.PathPoint>();
            int count = PathPointList.Count - 1, i = 0;

            // exit if no list
            if ((PathPointList == null) || (PathPointList.Count == 0))
                return retList;

            // log it
            clsSettings.Logging.AddToLogFormatted(Resources.AutoNav, "Sorting Path Points by distance to current location. {0} Points to sort", PathPointList.Count);

            // add lastpoint to the list
            //retList.Add(LastPoint);

            // loop through and add the points by distance
            while (PathPointList.Count > 0)
            {
                LastPoint = PathPointList[0];

                // loop through path point list and get closest point to last point
                for (i = 0; i < count; i++)
                {
                    // get the point for testing
                    testPoint = PathPointList[i];
                    
                    // compare distances        
                    if (testPoint.Distance(LastPoint) < LastPoint.Distance(LastPoint))
                        LastPoint = testPoint;
                }

                // we now have the nearest point, remove it from the old list, add to new list
                PathPointList.Remove(LastPoint);
                retList.Add(LastPoint);
            }

            return retList;
        }

        // Sort Path Points
        #endregion

        #region Conditions Met

        /// <summary>
        /// Checks if all the conditions have been met
        /// </summary>
        /// <param name="Conditions">list of conditions to check</param>
        public static bool ConditionsMet(List<clsBlockListItem> Conditions)
        {
            bool rVal = false;
            bool CanBeMet = false, HasOr = false, HasAnd = false; ;

            try
            {
                // log it
                if (clsSettings.VerboseLogging)
                    clsSettings.Logging.AddToLog(Resources.AutoNav, Resources.AutoNavConditionsMetTest);

                // exit if no conditions
                if ((Conditions == null) || (Conditions.Count == 0))
                    return true;

                // loop through and check each condition. if any are not met, exit false
                using (new clsFrameLock.LockBuffer())
                {
                    foreach (clsBlockListItem bli in Conditions)
                    {
                        switch (bli.NodeType)
                        {
                            case clsBlockListItem.ENodeType.Continue_Durability_X_Percent: // OR
                                HasOr = true;
                                if (clsCharacter.DurabilityPercent <= bli.Quantity)
                                    CanBeMet = true;
                                break;

                            case clsBlockListItem.ENodeType.Continue_Until_Bags_Full: // OR
                                HasOr = true;
                                if (clsCharacter.BagsFull)
                                    CanBeMet = true;
                                break;

                            case clsBlockListItem.ENodeType.Continue_Until_X_Item_Y_Quantity: // AND
                                HasAnd = true;
                                // loop through the items in our bag to see if we have enough
                                if (clsSearch.NumItemsInBag(bli.DestName) < bli.Quantity)
                                    return rVal; // not enough items
                                break;

                            case clsBlockListItem.ENodeType.Continue_Until_X_Mob_Y_Killed: // AND
                                HasAnd = true;
                                // loop through mobkills
                                foreach (clsMonsterKill mKill in MonsterKills)
                                {
                                    // check if this mob matches and we enough kills
                                    if ((mKill.MonsterName == bli.DestName) && (mKill.Count < bli.Quantity))
                                        return rVal; // not enough kills
                                }
                                break;

                            case clsBlockListItem.ENodeType.Continue_Until_Character_Is_Level_X: // AND
                                HasAnd = true;
                                if (clsCharacter.CurrentLevel < bli.Quantity)
                                    return rVal; // too low still
                                break;
                        }
                    }
                }

                // if we have an OR, and no ands, return the or result
                if ((!HasAnd) && (HasOr))
                    rVal = CanBeMet;
                else
                    rVal = true; // nothing failed
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Conditions Test");
            }

            finally
            {
                if (clsSettings.VerboseLogging)
                    clsSettings.Logging.AddToLogFormatted("ConditionsMet", "Condition Met:{0}", rVal.ToString());
            }

            return rVal;
        }

        // Conditions Met
        #endregion
    }
}
