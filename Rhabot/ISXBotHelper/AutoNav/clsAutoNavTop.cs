using System;
using System.Collections.Generic;
using System.Text;
using ISXWoW;
using LavishVMAPI;
using LavishScriptAPI;
using ISXBotHelper.Properties;
using PathMaker;
using PathMaker.Graph;

namespace ISXBotHelper.AutoNav
{
    internal static class clsAutoNavTop
    {
        #region Go To Unit

        public static WoWUnit GoToUnit(clsBlockListItem UnitInfo, string SearchStr)
        {
            List<WoWUnit> unitList = null;
            clsPath.PathListInfoEx uPath = null;

            try
            {
                // first, try to find the unit nearby
                unitList = clsSearch.Search_Unit(SearchStr);

                // if we found one, then go to that one
                if ((unitList != null) && (unitList.Count > 0))
                {
                    clsPath.MoveToTarget(unitList[0], 2);
                    return unitList[0];
                }

                // no unit found, we need to run to it
                uPath = new clsPath.PathListInfoEx(
                    new PathListInfo("AutoNav Path", false, false, false,
                    new clsPPather().BuildPath(
                        clsCharacter.ZoneText,
                        clsCharacter.MyLocation.ToLocation(),
                        UnitInfo.DestinationPoint.ToLocation(),
                        clsSettings.gclsLevelSettings.SearchRange)));

                // exit if no path
                if ((uPath == null) || (uPath.PathList.Count == 0))
                {
                    clsSettings.Logging.AddToLog("GoToUnit: Exiting. Could not build path to unit");
                    return null;
                }

                // set flying
                uPath.CanFly = UnitInfo.PathCanFly;

                // run to the unit
                if (clsPath.RunPath(uPath) != clsPath.EMovementResult.Success)
                {
                    clsSettings.Logging.AddToLog("GoToUnit: Exiting. Could not run to unit");
                    return null;
                }

                // get the unit
                unitList = clsSearch.Search_Unit(SearchStr);

                if ((unitList != null) && (unitList.Count > 0))
                    return unitList[0];
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "GoToUnit");
            }

            return null;
        }

        /// <summary>
        /// Runs to an object
        /// </summary>
        public static WoWObject GoToObject(clsBlockListItem UnitInfo, string SearchStr)
        {
            List<WoWObject> unitList = null;
            clsPath.PathListInfoEx uPath = null;
            clsPath cPath = new clsPath();
            PathListInfo.PathPoint oPoint = null;

            try
            {
                // first, try to find the unit nearby
                unitList = clsSearch.Search_Object(SearchStr);

                // if we found one, then go to that one
                if ((unitList != null) && (unitList.Count > 0))
                {
                    // get the object location
                    using (new clsFrameLock.LockBuffer())
                        oPoint = new PathListInfo.PathPoint(unitList[0].X, unitList[0].Y, unitList[0].Z);

                    cPath.MoveToPoint(oPoint);
                    return unitList[0];
                }

                // no unit found, we need to run to it
                uPath = new clsPath.PathListInfoEx(
                    new PathListInfo("AutoNav Path", false, false, false,
                    new clsPPather().BuildPath(
                        clsCharacter.ZoneText,
                        clsCharacter.MyLocation.ToLocation(),
                        UnitInfo.DestinationPoint.ToLocation(),
                        clsSettings.gclsLevelSettings.SearchRange)));

                // exit if no path
                if ((uPath == null) || (uPath.PathList.Count == 0))
                {
                    clsSettings.Logging.AddToLog("GoToUnit: Exiting. Could not build path to unit");
                    return null;
                }

                // mount up for this path
                uPath.CanMount = true;
                uPath.CanFly = UnitInfo.PathCanFly;

                // run to the unit
                if (clsPath.RunPath(uPath) != clsPath.EMovementResult.Success)
                {
                    clsSettings.Logging.AddToLog("GoToUnit: Exiting. Could not run to unit");
                    return null;
                }

                // get the unit
                unitList = clsSearch.Search_Object(SearchStr);

                if ((unitList != null) && (unitList.Count > 0))
                    return unitList[0];
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "GoToObject");
            }

            return null;
        }

        // Go To Unit
        #endregion

        #region Go To Vendor

        public static WoWUnit GoToVendorX(clsBlockListItem VendorInfo)
        {
            WoWUnit vendor = null;

            try
            {
                // log it
                clsSettings.Logging.AddToLogFormatted(Resources.AutoNav, "Started. Looking for Vendor '{0}'", VendorInfo.DestName);

                // first, try to find the vendor nearby
                vendor = GoToUnit(VendorInfo, string.Format("-units,-merchant,{0}", VendorInfo.DestName));
                
                // sell to the vendor
                clsVendor.SellToVendor(vendor);
                return vendor;
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "GoToVendorX");
            }

            return null;
        }

        // Go To Vendor
        #endregion

        #region Go To Flightmaster

        /// <summary>
        /// Goes to the flightmaster.
        /// </summary>
        /// <param name="FMInfo">The FM info.</param>
        public static WoWUnit GoToFlightmaster(clsBlockListItem FMInfo)
        {
            try
            {
                clsSettings.Logging.AddToLogFormatted(Resources.AutoNav, "Going to '{0}'", FMInfo.DestName);

                // go to the flight master
                return GoToUnit(FMInfo, string.Format("-flightmaster,{0}", FMInfo.DestName));
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "GoToFlightmaster");
            }

            return null;
        }

        // Go To Flightmaster
        #endregion

        #region Go To Trainer

        /// <summary>
        /// Goes to the trainer.
        /// </summary>
        /// <param name="TrainerInfo">The trainer info.</param>
        public static bool GoToTrainer(clsBlockListItem TrainerInfo)
        {
            WoWUnit trainer = null;
            bool rVal = false;

            try
            {
                // run to the trainer
                trainer = GoToUnit(TrainerInfo, string.Format("-classtrainer,{0}", TrainerInfo.DestName));

                // if we can't find a trainer, retry with no name
                if ((trainer == null) || (! trainer.IsValid))
                    trainer = GoToUnit(TrainerInfo, "-classtrainer");

                // if no trainer, then exit
                if ((trainer == null) || (!trainer.IsValid))
                {
                    clsSettings.Logging.AddToLog("GoToTrainer: Exiting because we can't find a trainer");
                    return false;
                }

                // do training
                rVal = BuyTrainerItems(trainer);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "GoToTrainer");
            }
            return rVal;
        }

        /* Code From Undrgrnd59 */
        /// <summary>
        /// Buys training
        /// </summary>
        /// <param name="trainer">the trainer to use</param>
        private static bool BuyTrainerItems(WoWUnit trainer)
        {
            bool rVal = false;

            try
            {
                //log it
                clsSettings.Logging.AddToLogFormatted(Resources.AutoNav, "Getting training from '{0}'", trainer.Name);

                // if the dialog is not open, open it
                if (!UI.TrainerFrame.IsVisible)
                {
                    // open trainer
                    trainer.Target();
                    trainer.Use();

                    // wait a second
                    System.Threading.Thread.Sleep(3000);

                    // gossip
                    if (trainer.CanGossip)
                        UI.GossipOptionsFrame.SelectGossipOption(1);

                    // wait
                    System.Threading.Thread.Sleep(3000);
                }

                // only show available options
                clsSettings.isxwow.WoWScript("SetTrainerServiceTypeFilter(\"available\",1)"); // make it show available
                clsSettings.isxwow.WoWScript("SetTrainerServiceTypeFilter(\"unavailable\",0)"); // make it hide unavailable
                clsSettings.isxwow.WoWScript("SetTrainerServiceTypeFilter(\"used\",0)"); // make it hide used

                // get training
                uint serviceCount = Convert.ToUInt32(UI.TrainerFrame.ServiceCount); // how many options are there avaliable
                if (serviceCount > 0)
                {
                    // loop through each option
                    for (uint i = 1; i < serviceCount + 1; i++) 
                    {
                        // skip if the header/category title
                        if (UI.TrainerFrame.Skill(i).IsHeader)
                            continue;

                        // get our money and the training cost
                        uint myMoney = Convert.ToUInt32(clsSettings.isxwow.Me.Copper);
                        uint cost = ISXWoW.UI.TrainerFrame.Skill(i).Cost;

                        // if we have the money for it, learn it!
                        if (myMoney > cost) 
                            UI.TrainerFrame.Skill(i).Learn();
                    }
                }

                rVal = true;
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "BuyTrainerItem");
            }

            return rVal;
        }

        // Go To Trainer
        #endregion

        #region Go To Innkeeper

        /// <summary>
        /// Goes to the innkeeper.
        /// </summary>
        /// <param name="InnkeeperInfo">The innkeeper info.</param>
        /// <param name="MakeInnHome">if set to <c>true</c> makes the inn your home.</param>
        public static bool GoToInnkeeper(clsBlockListItem InnkeeperInfo, bool MakeInnHome)
        {
            bool rVal = false;
            WoWUnit iUnit = null;

            try
            {
                // log it
                clsSettings.Logging.AddToLogFormatted(Resources.AutoNav, "Finding Innkeeper '{0}'", InnkeeperInfo.DestName);

                // go to the innkeeper
                iUnit = GoToUnit(InnkeeperInfo, string.Format("-innkeeper,{0}", InnkeeperInfo.DestName));

                // exit if no innkeeper
                if ((iUnit == null) || (! iUnit.IsValid))
                {
                    clsSettings.Logging.AddToLog(Resources.AutoNav, "Exiting. Could not find the innkeeper");
                    return false;
                }

                // talk to the innkeeper
                iUnit.Target();
                iUnit.Use();
                System.Threading.Thread.Sleep(3000);

                // select make this inn your home
                // TODO: make sure this is the correct verbage
                // TODO: maybe there is a function we can use instead?
                // http://www.isxwow.net/forums/viewtopic.php?f=5&t=2039&hilit=&sid=9e9ad7d65832966ce3d786f587e368f8
                if (MakeInnHome)
                {
                    if (!UI.GossipOptionsFrame.SelectGossipOption("Make this Inn your home"))
                    {
                        clsSettings.Logging.AddToLog("GoToInnkeeper: Could not make this inn your home");
                        return false;
                    }

                    // click yes
                    System.Threading.Thread.Sleep(1000);
                    using (new clsFrameLock.LockBuffer())
                        UI.GlueDialogFrame.DefaultButton.Click();
                }

                rVal = true;
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "GoToInnkeeper");
            }

            return rVal;
        }

        // Go To Innkeeper
        #endregion

        #region Go To Mailbox

        /// <summary>
        /// Goes to the mailbox.
        /// </summary>
        /// <param name="MailboxInfo">The mailbox info.</param>
        public static bool GoToMailbox(clsBlockListItem MailboxInfo)
        {
            try
            {
                // run to the object
                if (GoToObject(MailboxInfo, "-mailbox") == null)
                {
                    clsSettings.Logging.AddToLog("GoToMailbox: Exiting, could not find a mailbox");
                    return false;
                }

                // send mail
                return clsMail.SendMailList();
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "GoToMailbox");
            }
            return false;
        }

        // Go To Mailbox
        #endregion

        #region Go To XYZ

        public static bool GoToXYZ(clsBlockList LocationInfo)
        {
            bool rVal = false;
            bool IsFlyable = false;
            List<WoWItem> itemList = null;
            List<WoWGameObject> gObjList = null;

            try
            {
                // log it
                clsSettings.Logging.AddToLogFormatted("GoToXYZ", "Going to {0}", LocationInfo.BlockItem.DestinationPoint.ToString());

                // find out if the path is flyable
                IsFlyable = LocationInfo.BlockItem.PathCanFly;

                // run the path
                if (! RunToXYZ(new PathListInfo.PathPoint(LocationInfo.BlockItem.DestinationPoint), IsFlyable))
                {
                    clsSettings.Logging.AddToLogFormatted("GoToXYZ", "Exiting. Could not run path");
                    return false;
                }

                // exit if no sub blocks
                if ((LocationInfo.SubBlocks == null) || (LocationInfo.SubBlocks.Count == 0))
                    return true;

                // handle sub blocks
                foreach (clsBlockList sbl in LocationInfo.SubBlocks)
                {
                    switch (sbl.BlockItem.NodeType)
                    {
                    	case clsBlockListItem.ENodeType.Use_Item_X: // use item x
                            // log it
                            clsSettings.Logging.AddToLogFormatted("GoToXYZ", "Using '{0}'", sbl.BlockItem.DestName);

                            // get it from the bag
                            itemList = clsSearch.Search_BagItem(sbl.BlockItem.DestName);

                            // skip if nothing found
                            if ((itemList == null) || (itemList.Count == 0) || (!itemList[0].IsValid))
                            {
                                clsSettings.Logging.AddToLogFormatted("GoToXYZ", "Could not find item '{0}' in your bags", sbl.BlockItem.DestName);
                                break;
                            }

                            // use the first item, sleep until we finish using it
                            if (itemList[0].Use())
                            {
                                System.Threading.Thread.Sleep(1000);
                                while (clsCharacter.IsCasting)
                                    System.Threading.Thread.Sleep(300);
                            }
                            else
                                clsSettings.Logging.AddToLogFormatted("GoToXYZ", "Could not Use item '{0}'", itemList[0].FullName);
                            
                    		break;

                        // pick up at at Y
                        case clsBlockListItem.ENodeType.Pickup_Item_Named_X_Zone_Y:
                        case clsBlockListItem.ENodeType.Pickup_Item_Named_X_Zone_Y_MULTI:
                            // log it
                            clsSettings.Logging.AddToLogFormatted("GoToXYZ", "Picking up '{0}'", sbl.BlockItem.DestName);

                            // search for it
                            gObjList = clsSearch.Search_GameObject(string.Format("-dynamicobjects,{0}", sbl.BlockItem.DestName));

                            // skip if nothing found
                            if ((gObjList == null) || (gObjList.Count == 0) || (! gObjList[0].IsValid))
                            {
                                clsSettings.Logging.AddToLogFormatted("GoToXYZ", "Could not find object '{0}'", sbl.BlockItem.DestName);
                                break;
                            }

                            // use the first item (should pick it up), sleep until we finish using it
                            if (gObjList[0].Use())
                            {
                                System.Threading.Thread.Sleep(1000);
                                while (clsCharacter.IsCasting)
                                    System.Threading.Thread.Sleep(300);
                            }
                            else
                                clsSettings.Logging.AddToLogFormatted("GoToXYZ", "Could not Use item '{0}'", gObjList[0].Name);

                            break;
                    }
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "GoToXYZ");
            }
            return rVal;
        }

        // Go To XYZ
        #endregion

        #region Run To XYZ

        /// <summary>
        /// Checks to see if the Path Flyable option exists in the sublist
        /// </summary>
        /// <param name="SubList">the list to search</param>
        public static bool IsPathFlyable(List<clsBlockList> SubList)
        {
            // find out if the path is flyable
            if ((SubList == null) || (SubList.Count > 0))
                return false;

            // loop through all items
            foreach (clsBlockList bl in SubList)
            {
                if (bl.BlockItem.PathCanFly)
                    return true;
            }

            // nothing found
            return false;
        }

        /// <summary>
        /// Runs to XYZ
        /// </summary>
        /// <param name="pathPoint">the point to run to</param>
        /// <param name="IsFlyable">true if path is flyable</param>
        public static bool RunToXYZ(PathListInfo.PathPoint pathPoint, bool IsFlyable)
        {
            clsPath.PathListInfoEx pInfo = null;
            clsPath cPath = new clsPath();

            try
            {
                // if it's less than 100 yards, just run there
                if (pathPoint.Distance(clsCharacter.MyLocation) <= 100)
                {
                    // log it
                    clsSettings.Logging.AddToLog("RunToXYZ: Trying MoveToPoint");

                    // fly it
                    if ((IsFlyable) && (cPath.MoveToPoint_Fly(pathPoint, true) == clsPath.EMovementResult.Success))
                        return true;

                    // run it
                    if (cPath.MoveToPoint(pathPoint) == clsPath.EMovementResult.Success)
                        return true;
                }

                // get the path
                pInfo = new clsPath.PathListInfoEx(
                    new PathListInfo("AutoNav Path", false, false, false,
                    new clsPPather().BuildPath(
                        clsCharacter.ZoneText,
                        clsCharacter.MyLocation.ToLocation(),
                        pathPoint.ToLocation(),
                        clsSettings.gclsLevelSettings.SearchRange)));

                // exit if no path
                if ((pInfo == null) || (pInfo.PathList.Count == 0))
                {
                    clsSettings.Logging.AddToLog("RunToXYZ: Could not navigate to XYZ. Could not build a path");
                    return false;
                }

                // set mountable/flyable
                pInfo.CanMount = true;
                pInfo.CanFly = IsFlyable;

                // go there
                return clsPath.RunPath(pInfo) == clsPath.EMovementResult.Success;
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "RunToXYZ");
            }

            return false;
        }

        // Run To XYZ
        #endregion

        #region Fish At XYZ

        /// <summary>
        /// Fishes at XYZ
        /// </summary>
        public static bool FishAtXYZ(clsBlockList FishInfo)
        {
            bool rVal = false;
            Rhabot.BotThreads.clsSushiBot SushiBot = new Rhabot.BotThreads.clsSushiBot();
            DateTime StartTime = DateTime.MinValue;
            int Duration = 0;
            List<clsBlockListItem> fishNames = new List<clsBlockListItem>();
            bool NeedsMoreFish = false;

            try
            {
                // log it
                clsSettings.Logging.AddToLogFormatted("FishAtXYZ", "Going to {0}", FishInfo.BlockItem.DestinationPoint);

                // run the path
                if (! RunToXYZ(new PathListInfo.PathPoint(FishInfo.BlockItem.DestinationPoint), IsPathFlyable(FishInfo.SubBlocks)))
                {
                    clsSettings.Logging.AddToLog("FishAtXYZ", "Could no reach destination");
                    return false;
                }

                // loop through and get the end conditions
                foreach (clsBlockList fbl in FishInfo.SubBlocks)
                {
                    // fish until X
                    if (fbl.BlockItem.NodeType == clsBlockListItem.ENodeType.Fish_Until_X_Time)
                        Duration = fbl.BlockItem.Quantity;

                    // fish until X caught
                    if (fbl.BlockItem.NodeType == clsBlockListItem.ENodeType.Fish_Capture_X_of_Y)
                        fishNames.Add(fbl.BlockItem);
                }

                // start sushi bot
                clsSettings.Logging.AddToLog("FishAtXYZ: Starting SushiBot");
                StartTime = DateTime.Now;
                SushiBot.StartSushi();

                // loop until conditions met
                while (true)
                {
                    // exit if shutting down
                    if (clsSettings.TestPauseStop("FishAtXYZ: Exiting because of script stop"))
                        return true;

                    // check fish time
                    if (new TimeSpan(DateTime.Now.Ticks - StartTime.Ticks).Minutes >= Duration)
                    {
                        // time expired
                        clsSettings.Logging.AddToLog("FishATXYZ: Stopping SushiBot. We fished long enough"); 
                        SushiBot.StopSushi();
                        break;
                    }

                    if (fishNames.Count > 0)
                    {
                        NeedsMoreFish = false;

                        // check for fish caught
                        foreach (clsBlockListItem fbli in fishNames)
                        {
                            if (clsSearch.NumItemsInBag(fbli.DestName) < fbli.Quantity)
                            {
                                NeedsMoreFish = true;
                                break;
                            }
                        }

                        // break if we caught enough fish
                        if (!NeedsMoreFish)
                        {
                            clsSettings.Logging.AddToLog("FishAtXYZ: Exiting. We caught enough fish");
                            break;
                        }
                    }
                }

                rVal = true;
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "FishAtXYZ");
            }
            return rVal;
        }

        // Fish At XYZ
        #endregion

        #region ConvertPathList

        /// <summary>
        /// Converts a path list
        /// </summary>
        public static List<PathListInfo.PathPoint> ConvertANListToPathList(List<clsPathPoint> pointList)
        {
            // exit if no list
            if ((pointList == null) || (pointList.Count == 0))
                return new List<PathListInfo.PathPoint>();

            return pointList.ConvertAll<PathListInfo.PathPoint>(x => new PathListInfo.PathPoint(x));
        }

        // ConvertPathList
        #endregion
    }
}