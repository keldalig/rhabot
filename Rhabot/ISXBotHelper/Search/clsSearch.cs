// http://www.isxwow.net/forums/viewtopic.php?f=15&t=1092&p=7506#p7506 ***My Tutorial***
// http://www.isxwow.net/forums/viewtopic.php?f=20&t=853&p=6037&hilit=#p6037
// http://www.isxwow.net/forums/viewtopic.php?f=40&t=935&hilit=
// http://www.isxwow.net/forums/viewtopic.php?f=23&t=1226&hilit=
// to find a guid, use -guid for first param, and GUID for second param

using System;
using System.Collections.Generic;
using System.Text;
using ISXWoW;
using System.Text.RegularExpressions;
using System.Linq;

namespace ISXBotHelper
{
    public static class clsSearch
    {
        #region Enums

        public enum EGroundObjectType
        {
            Herb = 1,
            Mine,
            Chest,
            Other
        }

        // Enums
        #endregion

        #region Find Target To Attack

        #region Find Target To Attack Support

        /// <summary>
        /// Builds the basic find target search string
        /// </summary>
        /// <param name="HighLevel">highest level to target</param>
        /// <param name="Hostile">true to target -hostile, false to target -nonfriendly</param>
        /// <param name="LowLevel">lowest level to target</param>
        /// <param name="Range">range to search from your location</param>
        public static string BuildTargetString(int Range, int LowLevel, int HighLevel, bool Hostile)
        {
            StringBuilder sb = new StringBuilder();

            // build our search string
            sb.Append("-units,-nearest,-nopets,-nocritters,-alive,-lineofsight,"); // -untapped,
            if (Hostile)
                sb.Append("-hostile,");
            else
                sb.Append("-nonfriendly,");
            sb.AppendFormat("-levels {0}-{1},-range 0-{2}",
                LowLevel.ToString().Trim(), HighLevel.ToString().Trim(), Range.ToString().Trim());

            // no elite
            if (!clsSettings.gclsLevelSettings.TargetElites)
                sb.Append(",-noelite,");

            return sb.ToString();
        }

        /// <summary>
        /// Performs the target search and filters out bad targets
        /// </summary>
        /// <param name="SearchStr">the search string</param>
        private static WoWUnit FilterTargets(string SearchStr)
        {
            return FilterTargets(SearchStr, null);
        }

        /// <summary>
        /// Performs the target search and filters out bad targets
        /// </summary>
        /// <param name="SearchStr">the search string</param>
        private static WoWUnit FilterTargets(string SearchStr, WoWUnit CurrentTarget)
        {
            List<WoWUnit> UnitList = new List<WoWUnit>();
            WoWUnit tempUnit;
            int BestUnit = 0;

            try
            {
                // get the list of targets
                GuidList gl;
                using (new clsFrameLock.LockBuffer())
                {
                    gl = GuidList.New(Regex.Split(SearchStr, ","));

                    // if nothing found, then return
                    if (gl.Count == 0)
                    {
                        // log result
                        if (clsSettings.VerboseLogging)
                        {
                            clsSettings.Logging.DebugWrite(string.Format("FilterTargets: No targets found for {0}", SearchStr));
                            clsSettings.Logging.AddToLog("No targets found");
                        }
                        return null;
                    }

                    // found targets, loop through the list and add to the unit list
                    int SearchCount = (int)gl.Count;
                    int UnitCounter;
                    for (UnitCounter = 0; UnitCounter < SearchCount; UnitCounter++)
                    {
                        // get the unit
                        tempUnit = gl.Object((uint)UnitCounter).GetUnit();

                        // skip if this is the targetted unit
                        if ((CurrentTarget != null) && (CurrentTarget.IsValid) && (tempUnit.GUID == CurrentTarget.GUID))
                            continue;

                        // if ours, skip (pets, totems, etc)
                        if (tempUnit.OwnedByMe)
                        {
                            if (clsSettings.VerboseLogging)
                                clsSettings.Logging.AddToLogFormatted("FilterTarget", "Skipping {0} - Owned by me", tempUnit.Name);
                            continue;
                        }

                        // make sure it is in line of site
                        if (!tempUnit.InLineOfSight)
                        {
                            if (clsSettings.VerboseLogging)
                                clsSettings.Logging.AddToLogFormatted("FilterTarget", "Skipping {0} - Not in line of sight", tempUnit.Name);
                            continue;
                        }

                        // check if in blacklist
                        if (clsBlacklist.IsBlacklisted(clsSettings.BlackList_Combat, tempUnit))
                        {
                            clsSettings.Logging.DebugWrite(string.Format("FilterTarget", "Found Blacklisted - {0}", tempUnit.Name));
                            continue;
                        }

                        // check if unit is in combat with something else (and we are NOT in combat)
                        if ((!clsCombat.IsInCombat()) &&
                            ((tempUnit.AttackingUnit != null) && (tempUnit.AttackingUnit.IsValid)))
                        {
                            if (clsSettings.VerboseLogging)
                                clsSettings.Logging.DebugWrite(string.Format("FilterTargets: Found unit in combat with someone else - {0} vs {1}",
                                    tempUnit.Name, tempUnit.AttackingUnit.Name));
                            continue;
                        }

                        // if unit is attacking us, then return this unit
                        if (tempUnit.AttackingUnit.GUID == clsSettings.isxwow.Me.GUID)
                        {
                            // log found unit
                            clsSettings.Logging.AddToLog(string.Format("FoundTarget", "{0} - Lvl {1} - Location {2}",
                                tempUnit.Name,
                                tempUnit.Level,
                                tempUnit.Location));

                            // return the unit
                            return tempUnit;
                        }

                        // add unit to the list
                        UnitList.Add(tempUnit);
                    }


                    SearchCount = UnitList.Count;

                    // if no units, return null
                    if (SearchCount == 0)
                    {
                        clsSettings.Logging.AddToLog("No targets found");
                        return null;
                    }

                    // if we only have one unit, attack that
                    if (SearchCount == 1)
                    {
                        tempUnit = UnitList[0];
                        clsSettings.Logging.AddToLogFormatted("FoundTarget", "{0} - Lvl {1} - Location {2}",
                            tempUnit.Name,
                            tempUnit.Level,
                            tempUnit.Location.ToString());
                        return UnitList[0];
                    }

                    // check each unit (starting with the closest) and make sure I have a 
                    // good path to the unit. If so, return the first good pathed unit
                    PathListInfo.PathPoint MyLoc = clsCharacter.MyLocation;
                    tempUnit = UnitList.Find(x => !clsPath.IsPathObstructed(MyLoc, clsPath.GetUnitLocation(x)));

                    // log found unit
                    clsSettings.Logging.AddToLogFormatted("FoundTarget", "{0} - Lvl {1} - Location {2}",
                        tempUnit.Name,
                        tempUnit.Level,
                        tempUnit.Location.ToString());

                    // clear path, return this unit
                    return tempUnit;
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "FilterTarget");
            }

            return null;
        }

        // Find Target To Attack Support
        #endregion

        #region FindTargetToAttack

        /// <summary>
        /// Finds a target. Returns the target unit if one is found
        /// </summary>
        /// <returns></returns>
        public static WoWUnit FindTargetToAttack()
        {
            string searchStr;

            // http://www.isxwow.net/forums/viewtopic.php?f=17&t=797&p=0&e=0&sid=0d5924b8b6fc009ee38787bfed02f067
            // targetting filters from WoWbot\core\misc.iss
            //      FindTarget in misc.iss (line 510)

            // exit if we are dead
            if (clsCharacter.IsDead)
                return null;

            // log it
            if (clsSettings.VerboseLogging) 
                clsSettings.Logging.AddToLog("Searching for target");

            try
            {
                // if other units are attacking me, target them first
                WoWUnit tempUnit = clsCombat.GetUnitAttackingMe();
                if ((tempUnit != null) && (tempUnit.IsValid))
                {
                    // log it
                    if (clsSettings.VerboseLogging)
                        clsSettings.Logging.AddToLog(string.Format("Found unit attacking me. Targetting {0}", tempUnit.Name));
                    return tempUnit;
                }

                // build our target string
                searchStr = BuildTargetString(clsSettings.gclsLevelSettings.SearchRange, clsCharacter.MobLowLevel, clsCharacter.CurrentLevel + clsSettings.gclsLevelSettings.HighLevelAttack, clsSettings.SearchForHostiles);
                
                // log the search string
                if (clsSettings.VerboseLogging)
                    clsSettings.Logging.AddToLog(string.Format("FindTargetToAttack Search: {0}", searchStr));

                // get the unit list
                return FilterTargets(searchStr);
            }

            catch (Exception excep)
            {
                // show the error
                clsError.ShowError(excep, "FindTargetToAttack");
            }

            // no units found, return null
            return null;
        }

        // FindTargetToAttack
        #endregion

        #region FindSurroundingTargets

        /// <summary>
        /// Finds units to attack that are near (TargetRange + 10) TargetPoint
        /// </summary>
        /// <param name="TargetPoint">the focus of the search</param>
        /// <param name="MobLowLevel">lowest level of mob to search</param>
        /// <param name="MobHighLevel">highest level of mob to search</param>
        public static WoWUnit FindSurroundingHostileTarget(PathListInfo.PathPoint TargetPoint, int MobLowLevel, int MobHighLevel)
        {
            StringBuilder sb = new StringBuilder();

            try
            {
                // get the search string
                sb.Append(BuildTargetString(clsSettings.gclsLevelSettings.TargetRange + 10, MobLowLevel, MobHighLevel, true));
                sb.AppendFormat(",-origin {0},{1}", TargetPoint.X, TargetPoint.Y);

                // log the search string
                if (clsSettings.VerboseLogging)
                    clsSettings.Logging.AddToLog(string.Format("FindSurroundingTarget Search: {0}", sb.ToString()));

                // get the unit list
                return FilterTargets(sb.ToString());
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "FindSurroundingTarget");
            }

            // nothing found, so return null
            return null;
        }

        /// <summary>
        /// Finds units to attack that are near (TargetRange + 10) TargetPoint
        /// </summary>
        /// <param name="TargetPoint">the focus of the search</param>
        /// <param name="MobLowLevel">lowest level of mob to search</param>
        /// <param name="MobHighLevel">highest level of mob to search</param>
        /// <param name="Range">search range, in yards</param>
        public static WoWUnit FindSurroundingTarget(PathListInfo.PathPoint TargetPoint, int MobLowLevel, int MobHighLevel, int Range)
        {
            StringBuilder sb = new StringBuilder();

            try
            {
                // get the search string
                sb.Append(BuildTargetString(Range, MobLowLevel, MobHighLevel, false));
                sb.AppendFormat(",-origin {0},{1}", TargetPoint.X, TargetPoint.Y);

                // log the search string
                if (clsSettings.VerboseLogging)
                    clsSettings.Logging.AddToLog(string.Format("FindSurroundingTarget Search: {0}", sb.ToString()));

                // get the unit list
                return FilterTargets(sb.ToString());
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "FindSurroundingTarget");
            }

            // nothing found, so return null
            return null;
        }

        /// <summary>
        /// Finds units to attack that are near (TargetRange + 10) TargetPoint
        /// </summary>
        /// <param name="TargetPoint">the focus of the search</param>
        /// <param name="MobLowLevel">lowest level of mob to search</param>
        /// <param name="MobHighLevel">highest level of mob to search</param>
        /// <param name="Range">search range, in yards</param>
        /// <param name="CurrentTarget"></param>
        public static WoWUnit FindSurroundingTarget(PathListInfo.PathPoint TargetPoint, int MobLowLevel, int MobHighLevel, int Range, WoWUnit CurrentTarget)
        {
            StringBuilder sb = new StringBuilder();

            try
            {
                // get the search string
                sb.Append(BuildTargetString(Range, MobLowLevel, MobHighLevel, false));
                sb.AppendFormat(",-origin {0},{1}", TargetPoint.X, TargetPoint.Y);

                // log the search string
                if (clsSettings.VerboseLogging)
                    clsSettings.Logging.AddToLog(string.Format("FindSurroundingTarget Search: {0}", sb.ToString()));

                // get the unit list
                return FilterTargets(sb.ToString(), CurrentTarget);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "FindSurroundingTarget");
            }

            // nothing found, so return null
            return null;
        }

        // FindSurroundingTargets
        #endregion

        // Find Target To Attack
        #endregion

        #region Find Ground Object

        /// <summary>
        /// Finds herbs/mines/chests/objects within the specified search range
        /// </summary>
        public static WoWUnit FindGroundObject(EGroundObjectType GroundObjectType)
        {
            return FindGroundObject(GroundObjectType, string.Empty);
        }

        /// <summary>
        /// Finds herbs/mines/chests/objects within the specified search range
        /// </summary>
        public static WoWUnit FindGroundObject(EGroundObjectType GroundObjectType, string ItemName)
        {
            StringBuilder sb = new StringBuilder();
            WoWUnit tempUnit;
            int gCount;

            // exit if we are dead
            if (clsCharacter.IsDead)
            {
                clsSettings.Logging.AddToLog("FindGroundObject", "Exiting because we are dead");
                return null;
            }

            // log it
            if (clsSettings.VerboseLogging)
                clsSettings.Logging.AddToLogFormatted("FindGroundObject", "Searching for {0}", GroundObjectType.ToString());

            try
            {
                // build our search string
                sb.Append("-dynamicobjects,-nearest,-usable,");

                // herb/mine/chest
                switch (GroundObjectType)
                {
                    case EGroundObjectType.Herb:
                        sb.Append("-herb,");
                        break;
                    case EGroundObjectType.Mine:
                        sb.Append("-mine,");
                        break;
                    case EGroundObjectType.Chest:
                        sb.Append("-chest,");
                        if (!clsSettings.gclsLevelSettings.IsRogue)
                            sb.Append("-unlocked,");
                        break;
                    case EGroundObjectType.Other: // add the object name
                        sb.AppendFormat("{0},", ItemName);
                        break;
                }

                // target distance
                sb.AppendFormat("-range 0-{0}", clsSettings.gclsLevelSettings.SearchRange.ToString().Trim());

                //clsSettings.Logging.AddToLogFormatted("FindGroundObject: {0}", sb.ToString());

                // get the list of targets
                GuidList gl;
                using (new clsFrameLock.LockBuffer())
                {
                    gl = GuidList.New(Regex.Split(sb.ToString(), ","));

                    // if nothing found, then return
                    gCount = (int)gl.Count;
                    //clsSettings.Logging.AddToLogFormatted("FindGroundObject: gCount = {0}", gCount);
                    if (gCount == 0)
                    {
                        // log result
                        if (clsSettings.VerboseLogging)
                            clsSettings.Logging.AddToLog("FindGroundObject: No objects found");
                        return null;
                    }

                    // found objects, loop until we find one that is not blacklisted
                    for (int i = 0; i < gCount; i++)
                    {
                        // get the item
                        tempUnit = gl.Object((uint)i).GetUnit();

                        // skip if invalid
                        if ((tempUnit == null) || (!tempUnit.IsValid))
                            continue;

                        // return if not blacklisted
                        if ((!clsBlacklist.IsBlacklisted(clsSettings.Blacklist_GameObjects, tempUnit)) &&
                            (CheckSlopeHighMobs(tempUnit)))
                        {
                            clsSettings.Logging.AddToLog(string.Format("FindGroundObject: Found object '{1}' at: {0}", clsPath.GetUnitLocation(tempUnit), tempUnit.Name));
                            return tempUnit;
                        }

                        clsSettings.Logging.AddToLogFormatted("FindGroundObject", "Object is blacklisted or on bad slope");
                    }
                }
            }

            catch (Exception excep)
            {
                // show the error
                clsError.ShowError(excep, "FindGroundObject");
            }

            // no units found, return null
            if (clsSettings.VerboseLogging)
                clsSettings.Logging.AddToLog("FindGroundObject: No objects found");
            return null;
        }

        // Find Ground Object
        #endregion

        #region Search

        /// <summary>
        /// Searches searchString for units
        /// </summary>
        /// <param name="searchString">the string to search</param>
        public static List<WoWUnit> Search(string searchString)
        {
            return Search_Unit(searchString);
        }

        /// <summary>
        /// Searches searchString for units
        /// </summary>
        /// <param name="searchString">the string to search</param>
        public static List<WoWUnit> Search_Unit(string searchString)
        {
            List<WoWUnit> retList = new List<WoWUnit>();
            int gCount = 0;
            WoWUnit tempUnit = null;

            try
            {
                // log it
                if (clsSettings.VerboseLogging)
                    clsSettings.Logging.AddToLog("Searching", searchString);

                // get the list of targets
                GuidList gl;
                using (new clsFrameLock.LockBuffer())
                {
                    gl = GuidList.New(Regex.Split(searchString, ","));

                    // get the count
                    gCount = (int)gl.Count;

                    // if nothing found, then return
                    if (gCount == 0)
                    {
                        // log result
                        if (clsSettings.VerboseLogging)
                            clsSettings.Logging.AddToLog("No targets found");
                        return retList;
                    }

                    // pop retList
                    for (int i = 0; i < gCount; i++)
                    {
                        // get the unit
                        tempUnit = gl.Object((uint)i).GetUnit();

                        // test for hidden/invisible objects
                        if (tempUnit.Name.ToUpper().Contains("INVIS BUNNY"))
                            continue;


                        // add to return list
                        if (tempUnit.IsValid)
                            retList.Add(tempUnit);
                    }
                }
            }

            catch (Exception excep)
            {
                // log error
                clsError.ShowError(excep, string.Format("Searching: {0}", searchString));
            }

            // return the list
            return retList;
        }

        /// <summary>
        /// Searches searchString for items
        /// </summary>
        /// <param name="searchString">the string to search</param>
        public static List<WoWItem> Search_Item(string searchString)
        {
            List<WoWItem> retList = new List<WoWItem>();
            int gCount = 0;
            WoWItem tempItem = null;

            try
            {
                // log it
                if (clsSettings.VerboseLogging)
                    clsSettings.Logging.AddToLog("Search_Item", searchString);

                // get the list of targets
                GuidList gl;
                using (new clsFrameLock.LockBuffer())
                {
                    gl = GuidList.New(Regex.Split(searchString, ","));

                    // get the count
                    gCount = (int)gl.Count;

                    // if nothing found, then return
                    if (gCount == 0)
                    {
                        // log result
                        if (clsSettings.VerboseLogging)
                            clsSettings.Logging.AddToLog("No items found");
                        return retList;
                    }

                    // pop retList
                    for (int i = 0; i < gCount; i++)
                    {
                        tempItem = gl.Object((uint)i).GetItem();
                        if ((tempItem != null) && (tempItem.IsValid))
                            retList.Add(tempItem);
                    }
                }
            }

            catch (Exception excep)
            {
                // log error
                clsError.ShowError(excep, string.Format("Search_Item: {0}", searchString));
            }

            // return the list
            return retList;
        }

        /// <summary>
        /// Searches searchString for items
        /// </summary>
        /// <param name="searchString">the string to search</param>
        public static List<WoWObject> Search_Object(string searchString)
        {
            List<WoWObject> retList = new List<WoWObject>();
            int gCount = 0;
            WoWObject tempObj = null;

            try
            {
                // log it
                if (clsSettings.VerboseLogging)
                    clsSettings.Logging.AddToLog("Search_Object", searchString);

                // get the list of targets
                GuidList gl;
                using (new clsFrameLock.LockBuffer())
                {
                    gl = GuidList.New(Regex.Split(searchString, ","));

                    // get the count
                    gCount = (int)gl.Count;

                    // if nothing found, then return
                    if (gCount == 0)
                    {
                        // log result
                        if (clsSettings.VerboseLogging)
                            clsSettings.Logging.AddToLog("No items found");
                        return retList;
                    }

                    // pop retList
                    for (int i = 0; i < gCount; i++)
                    {
                        tempObj = gl.Object((uint)i);
                        if ((tempObj != null) && (tempObj.IsValid))
                            retList.Add(tempObj);
                    }
                }
            }

            catch (Exception excep)
            {
                // log error
                clsError.ShowError(excep, string.Format("Search_Object: {0}", searchString));
            }

            // return the list
            return retList;
        }

        /// <summary>
        /// Searches searchString for items
        /// </summary>
        /// <param name="searchString">the string to search</param>
        public static List<WoWGameObject> Search_GameObject(string searchString)
        {
            List<WoWGameObject> retList = new List<WoWGameObject>();
            int gCount = 0;
            WoWGameObject tempObj = null;

            try
            {
                // log it
                if (clsSettings.VerboseLogging)
                    clsSettings.Logging.AddToLog("Search_GameObject", searchString);

                // get the list of targets
                GuidList gl;
                using (new clsFrameLock.LockBuffer())
                {
                    gl = GuidList.New(Regex.Split(searchString, ","));

                    // get the count
                    gCount = (int)gl.Count;

                    // if nothing found, then return
                    if (gCount == 0)
                    {
                        // log result
                        if (clsSettings.VerboseLogging)
                            clsSettings.Logging.AddToLog("Search_GameObject: No items found");
                        return retList;
                    }

                    // pop retList
                    for (int i = 0; i < gCount; i++)
                    {
                        tempObj = gl.Object((uint)i).GetGameObject();
                        if ((tempObj != null) && (tempObj.IsValid))
                            retList.Add(tempObj);
                    }
                }
            }

            catch (Exception excep)
            {
                // log error
                clsError.ShowError(excep, string.Format("Search_GameObject: {0}", searchString));
            }

            // return the list
            return retList;
        }

        /// <summary>
        /// Returns one item from your bag of this name
        /// </summary>
        /// <param name="ItemName">the name to find</param>
        /// <returns></returns>
        public static WoWItem Search_BagItemOne(string ItemName)
        {
            // get the list and return one item
            List<WoWItem> itemList = Search_BagItem(ItemName);
            if ((itemList == null) || (itemList.Count == 0))
                return null;
            else
                return itemList[0];
        }

        /// <summary>
        /// Returns a list of items that match this name (in your bags)
        /// </summary>
        /// <param name="ItemName">the name to find</param>
        public static List<WoWItem> Search_BagItem(string ItemName)
        {
            List<WoWItem> retList = new List<WoWItem>();

            // get the list
            List<WoWItem> itemList = Search_Item(string.Format("-items,-inventory,{0}", ItemName));

            // if we got a list, return it
            if ((itemList != null) && (itemList.Count > 0))
                return itemList;
            
            // search didn't find anything, so we'll get all the bag items and return a match
            itemList = clsCharacter.GetBagItems();

            // exit if no items
            if ((itemList == null) || (itemList.Count == 0))
                return new List<WoWItem>();

            // loop through the list to find our item
            using (new clsFrameLock.LockBuffer())
            {
                foreach (WoWItem item in itemList)
                {
                    // add if the names match
                    if (item.FullName == ItemName)
                        retList.Add(item);
                }
            }

            // return the list
            return retList;
        }

        // Search
        #endregion

        #region Check Mobs / Slope

        /// <summary>
        /// Returns false if the item is on a bad slope or has high level surrounding mobs
        /// </summary>
        public static bool CheckSlopeHighMobs(WoWObject SearchUnit)
        {
            // get the unit's location
            PathListInfo.PathPoint sPoint = clsPath.GetUnitLocation(SearchUnit);

            //check for surround targets
            WoWUnit tempUnit = FindSurroundingHostileTarget(sPoint, clsCharacter.MobHighLevel + 1, clsCharacter.MobHighLevel + 10);

            // return false if we found a target or there is a collision to the item
            if ((tempUnit != null) && (tempUnit.IsValid)) 
                return false;

            return true;
        }

        // Check Mobs / Slope
        #endregion

        #region Item Count In Bag

        /// <summary>
        /// Returns how many of the specified item exist in your bags
        /// </summary>
        /// <param name="ItemName">the name of the item to search</param>
        public static int NumItemsInBag(string ItemName)
        {
            int rVal = 0, gCount = 0;

            try
            {
                if (clsSettings.VerboseLogging)
                    clsSettings.Logging.AddToLogFormatted("NumItemsInBag", "Searching for '{0}'", ItemName);

                // search for the item
                using (new clsFrameLock.LockBuffer())
                {
                    GuidList gl = GuidList.New("-inventory", "-items", ItemName);

                    // exit if no items
                    gCount = (int)gl.Count;
                    if (gCount == 0)
                    {
                        // get all items in the bag and search those
                        List<WoWItem> itemList = clsCharacter.GetBagItems();

                        // loop through all items in the bag
                        if ((itemList != null) && (itemList.Count > 0))
                        {
                            foreach (WoWItem item in itemList)
                            {
                                // add if we found our item
                                if (item.FullName == ItemName)
                                    rVal += item.StackCount;
                            }
                        }

                        return rVal;
                    }

                    // loop through and add stack counts
                    for (int i = 0; i < gCount; i++)
                        rVal += (int)gl.Object((uint)i).GetItem().StackCount;
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "NumItemsInBag");
            }

            return rVal;
        }

        // Item Count In Bag
        #endregion
    }
}
