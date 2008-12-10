// http://www.lavishsoft.com/wiki/index.php/IS:.NET

using System;
using System.Collections.Generic;
using System.Threading;
using ISXBotHelper.AutoNav;
using ISXBotHelper.Properties;
using ISXBotHelper.Threading;
using ISXRhabotGlobal;
using ISXWoW;
using PathMaker;

namespace ISXBotHelper
{
    /// <summary>
    /// Handles navigation for autonav
    /// </summary>
    public class clsAutoNavPath : IDisposable
    {
        #region Enums

        private enum EMoveState
        {
            AttackUnit = 0,
            PickHerb,
            OpenChest,
            Mine,
            GroundObject,
            None
        }

        // Enums
        #endregion

        #region Variables

        private DateTime LastStandFace = DateTime.MinValue;
        private DateTime LastFace = DateTime.MinValue;
        private clsPath cPath = new clsPath();

        // Variables
        #endregion

        #region MoveThroughAutoNavPath

        /// <summary>
        /// Moves from the current location to each of the waypoints, saving the path points
        /// as it moves along. Handles Death. Exits on NeedsVendor, Stuck, and Stopped.
        /// </summary>
        /// <param name="searchForObjects">true to search for chests/herbs/etc (does NOT apply to the <paramref name="objectsToFind"/> list</param>
        /// <param name="searchForMobs">true to search for mobs to attack (otherwise only handles aggro)</param>
        /// <param name="checkBagsDurability">true to check for bags full and durability</param>
        /// <param name="checkForDead">true to check for character dead (default). false is this is a corpse run</param>
        /// <param name="CanFly">true if we can fly on this path</param>
        /// <param name="CanMount">true if we can mount on this path</param>
        /// <param name="objectsToFind">list of objects to search for (such as quest items on ground)</param>
        /// <param name="NeedVendorItemList">list of items we need to keep updated from vendor</param>
        /// <param name="Conditions">list of conditions that we should test for</param>
        /// <param name="PathList">path list to run</param>
        /// <param name="threadBase">thread base class, for monitor shutdown</param>
        public clsPath.EMovementResult MoveThroughAutoNavPath(bool searchForObjects, bool searchForMobs, bool checkBagsDurability, bool checkForDead, bool CanFly, bool CanMount, List<string> objectsToFind, List<clsVendor.clsNeedVendorItem> NeedVendorItemList, List<PathListInfo.PathPoint> PathList, List<clsBlockListItem> Conditions, ThreadBase threadBase)
        {
            clsPath.EMovementResult retResult;
            clsCombat combat = new clsCombat();
            WoWUnit UnitToAttack, objectUnit;
            PathListInfo.PathPoint NextPoint, LastPoint = null;
            bool FindObjects = ((objectsToFind != null) && (objectsToFind.Count > 0)); // true to search for objects in the objectsToFind list
            clsGhost ghost = new clsGhost();
            DateTime LastVendorCheck = DateTime.MinValue, LastIPOCheck = DateTime.MinValue;
            int PointCounter = 0;

            try
            {
                // log it
                clsSettings.Logging.AddToLog(Resources.MoveThroughAutoNavPath, Resources.BeginMovingThroughAutoNavWaypoints);

                // get the starting point
                NextPoint = PathList[PointCounter++];

                // loop until we reach then of waypoints or the user stops the bot
                while (true)
                {
                    // exit if thread is shutdown
                    if ((threadBase != null) && (threadBase.Shutdown))
                        return clsPath.EMovementResult.Stopped;

                    // exit if stopped 
                    if (!clsSettings.TestPauseStop(Resources.MoveThroughAutoNavPath, Resources.ExitingDueToScriptStop))
                        return clsPath.EMovementResult.Stopped;

                    // if dead, handle dead. if can't handle, return error
                    if ((checkForDead) && (clsCharacter.IsDead) && (!ghost.HandleDeadAutoNav()))
                        return clsPath.EMovementResult.Error;

                    // face the next point
                    FaceNextPoint(NextPoint);

                    #region Check if too far from point

                    // return to the next point if we are too far away
                    if (clsPath.CheckIfTooFarFromPoint(NextPoint))
                    {
                        clsSettings.Logging.AddToLogFormatted(Resources.MoveThroughAutoNavPath, Resources.TooFarFromPoint, NextPoint.ToString(), clsCharacter.MyLocation.Distance(NextPoint));
                        cPath.MoveToPoint(NextPoint);
                    }

                    // Check if too far from point
                    #endregion

                    // start moving if we are not moving already
                    if (!clsPath.Moving)
                        clsPath.StartMoving();

                    // check for combat, objects, etc
                    EMoveState MoveState;
                    using (new clsFrameLock.LockBuffer())
                    {
                        // reset move state
                        MoveState = EMoveState.None;
                        UnitToAttack = null;
                        objectUnit = null;

                        #region find units to attack

                        if (searchForMobs)
                        {
                            // log it (Searching for unit to attack)
                            if (clsSettings.VerboseLogging)
                                clsSettings.Logging.AddToLog(Resources.MoveThroughAutoNavPath, Resources.SearchingForUnitToAttack);

                            UnitToAttack = clsSearch.FindTargetToAttack();
                            if ((UnitToAttack != null) && (UnitToAttack.IsValid))
                                MoveState = EMoveState.AttackUnit;
                        }

                        // find units to attack
                        #endregion

                        #region Find_Objects

                        if ((FindObjects) && (MoveState == EMoveState.None))
                        {
                            // log it (Searching for objects in the objects list)
                            if (clsSettings.VerboseLogging)
                                clsSettings.Logging.AddToLog(Resources.MoveThroughAutoNavPath, Resources.SearchingForObjectsInList);

                            // search for the unit
                            foreach (string itemName in objectsToFind)
                            {
                                // find the object
                                objectUnit = clsSearch.FindGroundObject(clsSearch.EGroundObjectType.Other, itemName);

                                // if we found something then break out
                                if ((objectUnit != null) && (objectUnit.IsValid))
                                {
                                    MoveState = EMoveState.GroundObject;
                                    break;
                                }
                            }
                        }

                        // Find_Objects
                        #endregion

                        #region Find_Herbs

                        if ((MoveState == EMoveState.None) && (searchForObjects) && (clsSettings.gclsLevelSettings.IsFlowerPicker))
                        {
                            // search for the herb
                            objectUnit = SearchForObject(clsSearch.EGroundObjectType.Herb);

                            // if we found something, change state
                            if ((objectUnit != null) && (objectUnit.IsValid))
                                MoveState = EMoveState.PickHerb;
                        }

                        // Find_Herbs
                        #endregion

                        #region Find_Chests

                        if ((MoveState == EMoveState.None) && (searchForObjects) && (clsSettings.gclsLevelSettings.Search_Chest))
                        {
                            // search for the chest
                            objectUnit = SearchForObject(clsSearch.EGroundObjectType.Chest);

                            // if we found something, change state
                            if ((objectUnit != null) && (objectUnit.IsValid))
                                MoveState = EMoveState.OpenChest;
                        }

                        // Find_Chests
                        #endregion

                        #region Find_Mines

                        if ((MoveState == EMoveState.None) && (searchForObjects) && (clsSettings.gclsLevelSettings.IsMiner))
                        {
                            // find a mine
                            objectUnit = SearchForObject(clsSearch.EGroundObjectType.Mine);

                            // we found something, change state
                            if ((objectUnit != null) && (objectUnit.IsValid))
                                MoveState = EMoveState.Mine;
                        }

                        // Find_Mines
                        #endregion

                        #region Vendor and Durability

                        if ((MoveState != EMoveState.AttackUnit) && (checkBagsDurability) && (LastVendorCheck <= DateTime.Now))
                        {
                            // check for vendor/durability
                            //using (new clsFrameLock.LockBuffer())
                            {
                                if (clsCharacter.NeedsVendor)
                                {
                                    clsSettings.Logging.AddToLog(Resources.MoveThroughAutoNavPath, Resources.ExitingNeedsVendor);
                                    return clsPath.EMovementResult.NeedVendor;
                                }

                                // check if we are low on any specified item
                                if ((NeedVendorItemList != null) && (NeedVendorItemList.Count > 0))
                                {
                                    // loop through each item in the need vendor list
                                    foreach (clsVendor.clsNeedVendorItem needItem in NeedVendorItemList)
                                    {
                                        // return need vendor if the item quantity is below what it should be
                                        if ((int)(needItem.Quantity * 0.25) <= clsSearch.NumItemsInBag(needItem.ItemName))
                                        {
                                            clsSettings.Logging.AddToLogFormatted(Resources.MoveThroughAutoNavPath, Resources.ExitingNeedsToBuy, needItem.ItemName);
                                            return clsPath.EMovementResult.NeedVendor;
                                        }
                                    }
                                }
                            }

                            LastVendorCheck = DateTime.Now.AddMilliseconds(2000);
                        }

                        // Vendor and Durability
                        #endregion
                    }

                    #region Handle Move State

                    switch (MoveState)
                    {
                        case EMoveState.AttackUnit:
                            #region Handle_Combat

                            clsPath.EMovementResult CombatResult;
                            try
                            {
                                // set our current location so we can return to it after combat
                                LastPoint = clsCharacter.MyLocation;

                                // do combat
                                CombatResult = HandleCombat(combat, UnitToAttack);

                                // exit if stopped
                                if (CombatResult == clsPath.EMovementResult.Stopped)
                                {
                                    clsSettings.Logging.AddToLog(Resources.MoveThroughAutoNavPath, Resources.ExitingDueToScriptStop);
                                    return clsPath.EMovementResult.Stopped;
                                }

                                // if dead, handle dead. if can't handle, return error
                                if ((checkForDead) && (clsCharacter.IsDead) && (!ghost.HandleDeadAutoNav()))
                                    return clsPath.EMovementResult.Error;

                                // loot
                                if (combat.DoLoot())
                                {
                                    // if here, we aggro'd while looting, do combat
                                    CombatResult = HandleCombat(combat, UnitToAttack);

                                    // exit if stopped
                                    if (CombatResult == clsPath.EMovementResult.Stopped)
                                    {
                                        clsSettings.Logging.AddToLog(Resources.MoveThroughAutoNavPath, Resources.ExitingDueToScriptStop);
                                        return clsPath.EMovementResult.Stopped;
                                    }
                                }
                            }

                            catch //(Exception excep)
                            {
                                // return to our last point
                                ReturnToLastPoint(CanFly, CanMount, LastPoint);
                            }

                            // Handle_Combat
                            #endregion
                            break;

                        case EMoveState.PickHerb:
                        case EMoveState.OpenChest:
                        case EMoveState.Mine:
                        case EMoveState.GroundObject:
                            #region Search

                            // skip if item is invalid
                            if (!clsItem.ItemIsValid(objectUnit))
                                break;

                            try
                            {
                                // set our lastpoint
                                LastPoint = clsCharacter.MyLocation;

                                // TESTING
                                // build a path to the object
                                clsPath.StopMoving();
                                List<PathListInfo.PathPoint> ObjectList = new PathMaker.clsPPather().BuildPath(
                                    clsCharacter.ZoneText,
                                    clsCharacter.MyLocation.ToLocation(),
                                    clsPath.GetUnitLocation(objectUnit).ToLocation(),
                                    clsSettings.gclsLevelSettings.SearchRange).ConvertAll<PathListInfo.PathPoint>(x=> new PathListInfo.PathPoint(x));

                                // run to the object
                                bool objPathRanOK = true;
                                foreach (PathListInfo.PathPoint objPoint in ObjectList)
                                {
                                    // move to the next point
                                    if (cPath.MoveToPoint(objPoint, false) != clsPath.EMovementResult.Success)
                                    {
                                        // break out, and set flag
                                        objPathRanOK = false;
                                        break;
                                    }
                                }

                                // reset if we couldn't reach the object
                                if (! objPathRanOK)
                                {
                                    // go back to the last point, then redo loop
                                    ReturnToLastPoint(CanFly, CanMount, LastPoint);
                                    break;
                                }

                                // loot this item
                                clsPath.EMovementResult LootResult = clsLoot.LootGameOjbect(objectUnit, true);

                                // if we aggro'd something, do combat
                                if (LootResult == clsPath.EMovementResult.Aggroed)
                                {
                                    // fight whatever we aggro'd
                                    CombatResult = HandleCombat(combat, clsCombat.GetUnitAttackingMe());

                                    // exit if stopped
                                    if (CombatResult == clsPath.EMovementResult.Stopped)
                                    {
                                        clsSettings.Logging.AddToLog(Resources.MoveThroughAutoNavPath, Resources.ExitingDueToScriptStop);
                                        return clsPath.EMovementResult.Stopped;
                                    }

                                    // if dead, handle dead. if can't handle, return error
                                    if ((checkForDead) && (clsCharacter.IsDead) && (!ghost.HandleDeadAutoNav()))
                                        return clsPath.EMovementResult.Error;
                                }
                            }

                            catch //(Exception excep)
                            {
                                // return to our last point
                                ReturnToLastPoint(CanFly, CanMount, LastPoint);
                            }

                            // Search
                            #endregion
                            break;
                    }

                    // Handle Move State
                    #endregion

                    #region Stuck / Obstacle Check

                    if (LastIPOCheck <= DateTime.Now)
                    {
                        #region Check if stuck

                        if (clsStuck.CheckStuckEx(NextPoint, true))
                        {
                            // log it
                            clsPath.StopMoving();
                            clsSettings.Logging.AddToLog(Resources.MoveThroughAutoNavPath, Resources.STUCKExiting);
                            return clsPath.EMovementResult.Stuck;
                        }

                        // Check if stuck
                        #endregion

                        #region Check for Obstacles

                        // IPO check (by EQJoe)
                        if ((clsCharacter.MyLocation.Distance(NextPoint) > clsSettings.PathPrecision) && 
                            (cPath.IPO(NextPoint)))
                        {
                            // try to get unstuck
                            clsStuck.MoveAround(NextPoint);

                            // if we are still ipo, then exit
                            if((clsCharacter.MyLocation.Distance(NextPoint) > clsSettings.PathPrecision) &&
                                (cPath.IPO(NextPoint)))
                            {
                                // can't find route to next point, consider us stuck
                                clsPath.StopMoving();

                                // log it and exit
                                clsSettings.Logging.AddToLog(Resources.MoveThroughAutoNavPath, Resources.ObstacleFound);
                                return clsPath.EMovementResult.Stuck;
                            }
                        }

                        // Check for Obstacles
                        #endregion

                        LastIPOCheck = DateTime.Now.AddMilliseconds(2000);
                    }

                    // Stuck / Obstacle Check
                    #endregion

                    #region Next Point

                    // get next point if we are near to our point
                    if (clsCharacter.MyLocation.Distance(NextPoint) <= clsSettings.PathPrecision)
                    {
                        // exit if no points left
                        if (PointCounter >= PathList.Count)
                            return clsPath.EMovementResult.Success;

                        // get the next point
                        NextPoint = PathList[PointCounter++];
                    }

                    // face point, change pitch if flying/swimming, and move if not moving
                    FaceNextPoint(NextPoint);

                    // Next Point
                    #endregion

                    #region Conditions

                    // check if conditions have been met
                    if ((! clsCharacter.IsDead) && (Conditions != null) && (Conditions.Count > 0))
                    {
                        // check if any of these are the special return to vendor condition
                        foreach (clsBlockListItem bli in Conditions)
                        {
                            // skip if not special condition
                            if (bli.NodeType != clsBlockListItem.ENodeType.Continue_Until_X_Item_Y_Quantity)
                                continue;

                            // special condition found. check if we need to return to vendor
                            if (clsSearch.Search_BagItem(bli.DestName).Count < bli.Quantity)
                            {
                                clsSettings.Logging.AddToLog(Resources.MoveThroughAutoNavPath, Resources.AutoNavSpecialConditionMet);
                                return clsPath.EMovementResult.NeedVendor;
                            }
                        }

                        // test conditions met. if met, then just exit
                        if (clsAutoNav.ConditionsMet(Conditions))
                        {
                            clsSettings.Logging.AddToLog(Resources.MoveThroughAutoNavPath, Resources.AutoNavConditionsMet);
                            return clsPath.EMovementResult.Success;
                        }
                    }

                    // Conditions
                    #endregion

                    // sleep a tick
                    Thread.Sleep(100);
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.MoveThroughAutoNavPath);
                retResult = clsPath.EMovementResult.Error;
            }

            finally
            {
                clsPath.StopMoving();
            }

            return retResult;
        }

        // MoveThroughAutoNavPath
        #endregion

        #region Support_Functions

        /// <summary>
        /// Logs and searches for a ground object
        /// </summary>
        /// <param name="GroundObjectType">the object to look for</param>
        private WoWUnit SearchForObject(clsSearch.EGroundObjectType GroundObjectType)
        {
            try
            {
                // log it
                if (clsSettings.VerboseLogging)
                {
                    switch (GroundObjectType)
                    {
                        case clsSearch.EGroundObjectType.Herb:
                            // Searching for herbs
                            clsSettings.Logging.AddToLog(Resources.AutoNavPath, Resources.SearchingForHerbs);
                            break;
                        case clsSearch.EGroundObjectType.Mine:
                            // Searching for mines
                            clsSettings.Logging.AddToLog(Resources.AutoNavPath, Resources.SearchingForMines);
                            break;
                        case clsSearch.EGroundObjectType.Chest:
                            // Searching for chests
                            clsSettings.Logging.AddToLog(Resources.AutoNavPath, Resources.SearchingForChests);
                            break;
                    }
                }

                // search and return
                return clsSearch.FindGroundObject(GroundObjectType);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.AutoNavPath, "SearchForObject");
            }            

            // should only be here on error
            return null;
        }

        /// <summary>
        /// Mounts up if we can
        /// </summary>
        /// <param name="CanFly">true to mount onto flying mount</param>
        /// <param name="CanMount">true to use best regular mount</param>
        private void DoMount(bool CanFly, bool CanMount)
        {
            try
            {
                // skip if we can't mount
                if ((! CanFly) && (! CanMount))
                    return;

                // if we can fly, mount that
                if (CanFly)
                    clsMount.MountFlying();
                else
                    clsMount.MountUp();
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.AutoNavPath, "DoMount");
            }            
        }

        /// <summary>
        /// Returns the last point saved
        /// </summary>
        /// <param name="CanFly">true if we can fly</param>
        /// <param name="CanMount">true if we can mount up</param>
        /// <param name="LastPoint">the point to return</param>
        private void ReturnToLastPoint(bool CanFly, bool CanMount, PathListInfo.PathPoint LastPoint)
        {
            try
            {
                // mount up
                DoMount(CanFly, CanMount);

                // go to the point
                if (CanFly)
                    cPath.MoveToPoint_Fly(LastPoint, true);
                else
                    cPath.MoveToPoint(LastPoint);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.AutoNavPath, "ReturnToLastPoint");
            }            
        }

        /// <summary>
        /// Handles combat
        /// </summary>
        /// <param name="combat">the combat class</param>
        /// <param name="UnitToAttack">the unit to attack</param>
        private clsPath.EMovementResult HandleCombat(clsCombat combat, WoWUnit UnitToAttack)
        {
            List<WoWUnit> searchList;

            // do combat
            while (true)
            {
                clsGlobals.AttackOutcome CombatResult = combat.BeginCombat(UnitToAttack);
                if (CombatResult == clsGlobals.AttackOutcome.Stopped)
                    return clsPath.EMovementResult.Stopped;

                // if dead, handle dead. if can't handle, return error
                if ((clsCharacter.IsDead) && (! new clsGhost().HandleDeadAutoNav()))
                    return clsPath.EMovementResult.Error;

                // check for and handle aggro
                using (new clsFrameLock.LockBuffer())
                {
                    if (clsCombat.IsInCombat())
                    {
                        UnitToAttack = clsCombat.GetUnitAttackingMe();
                        if ((UnitToAttack != null) && (UnitToAttack.IsValid))
                            continue;
                    }
                }

                // if nearby mobs, then do downtime first, else loot first
                using (new clsFrameLock.LockBuffer())
                    searchList = clsSearch.Search_Unit(clsSearch.BuildTargetString(20, clsCharacter.MobLowLevel, clsCharacter.MobHighLevel, clsSettings.SearchForHostiles));

                // found mobs, downtime first
                if ((searchList != null) && (searchList.Count > 0))
                {
                    clsSettings.Logging.AddToLog(Resources.AutoNavPath, Resources.FoundNearbyMobs);

                    // downtime
                    if ((combat.NeedDowntime()) && (combat.DoDowntime()))
                        continue; // aggroed

                    // time to loot
                    if (combat.DoLoot())
                        continue; // aggroed
                }

                else
                {
                    clsSettings.Logging.AddToLog(Resources.AutoNavPath, Resources.NoNearbyMobs);

                    // time to loot
                    if (combat.DoLoot())
                        continue; // aggroed

                    // downtime
                    if ((combat.NeedDowntime()) && (combat.DoDowntime()))
                        continue; // aggroed
                }

                // return result
                if (clsCharacter.IsDead)
                    return clsPath.EMovementResult.Dead;
                else
                    return clsPath.EMovementResult.Success;
            }
        }

        /// <summary>
        /// Face the next point and move if we are not moving
        /// </summary>
        /// <param name="NextPoint">the point to face</param>
        private void FaceNextPoint(PathListInfo.PathPoint NextPoint)
        {
            // skip if dead and NOT a ghost (not released yet)
            if ((clsCharacter.IsDead) && (!clsCharacter.IsGhost))
                return;

            // exit if we faced recently
            if (LastStandFace < DateTime.Now)
            {
                // stand up if sitting
                clsSettings.StandUp();

                // face our target pitch again
                if (clsCharacter.FlyingOrSwimming)
                    clsPath.ChangePitch(clsPath.PitchToPoint(NextPoint));

                // reset last stand face
                LastStandFace = DateTime.Now.AddMilliseconds(3000);
            }

            if (LastFace < DateTime.Now)
            {
                // face. only move if we haven't moved yet
                if ((!clsFace.FacePointEx(NextPoint)) || (!clsPath.Moving))
                {
                    // move if not moving
                    clsSettings.Logging.AddToLog(Resources.FaceNextPoint, Resources.StartMoving);
                    clsPath.StartMoving();
                }

                // reset last face
                LastFace = DateTime.Now.AddMilliseconds(1500);
            }
        }

        // Support_Functions
        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            // dispose of clsPath
            cPath.Dispose();
            cPath = null;
        }

        #endregion
    }
}
