using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ISXBotHelper.Properties;
using Rhabot;
using rs;
using PathMaker;
using PathMaker.Graph;

namespace ISXBotHelper
{
    /// <summary>
    /// Functions for handlind death and ghosthood
    /// </summary>
    public class clsGhost
    {
        private const int TenYards = 10; // ten yards from corpse (??)

        #region HandleDead

        /// <summary>
        /// Releases from corpse and runs back. Rez at corpse. Returns true on success, false on failure
        /// </summary>
        public bool HandleDead(clsPath.PathListInfoEx CurrentPath, List<clsPath.PathListInfoEx> GraveyardPaths)
        {
            bool rVal;
            clsPath cPath = new clsPath();
            clsPath.NearPathPoint nearPoint;
            clsPath.PathListInfoEx dPath, clonePath;
            clsPath.PathListInfoEx pInfo = null;

            try
            {
                // exit if we are not dead
                if (!clsCharacter.IsDead)
                    return true;

                clsSettings.Logging.AddToLog(Resources.HandleDead);

                // release from the corpse
                ReleaseFromBody();

                // we should now be at the graveyard. get the corpse point and build a path to there
                PathListInfo.PathPoint corpsePoint;
                using (new clsFrameLock.LockBuffer())
                    corpsePoint = new PathListInfo.PathPoint(clsSettings.isxwow.Me.Corpse);

                // see if we can hop onto the current path, first
                clsPath.NearPathPoint npoint = cPath.GetNearestPoint(CurrentPath, clsCharacter.MyLocation, 30);
                if (npoint != null)
                {
                    // use the current path
                    pInfo = CurrentPath;

                    // check if the corpse point is before or after the nearest point
                    clsPath.NearPathPoint cnpoint = cPath.GetNearestPoint(CurrentPath, corpsePoint);
                    if (cnpoint != null)
                    {
                        // found a point, check it's index
                        // reverse path if the corpse is before our start point
                        if (npoint.Element > cnpoint.Element)
                            pInfo.ReversePath();

                        // set current step on the path
                        pInfo.CurrentStep = cPath.GetNearestPoint(pInfo, clsCharacter.MyLocation, 15).Element;

                        // delete after the corpse point
                        cnpoint = cPath.GetNearestPoint(CurrentPath, corpsePoint);
                        int pCount = pInfo.PathList.Count;
                        if ((pCount - 1 > cnpoint.Element + 1) && (pCount - cnpoint.Element + 1 > 0))
                            pInfo.PathList.RemoveRange(cnpoint.Element + 1, pCount - cnpoint.Element + 1);
                    }
                    else
                    {
                        // no point, try the graveyard way
                        pInfo = null;
                        npoint = null;
                    }
                }

                // try to use a graveyard path, if we couldn't use current path
                if (pInfo == null)
                {
                    // get the nearest path to our current location
                    pInfo = clsPath.GetNearestPath(GraveyardPaths);

                    // if nothing found, then exit
                    if (pInfo == null)
                    {
                        clsSettings.Logging.AddToLog(Resources.HandleDead, Resources.CouldNotBuildPathFromGhosttoCorpse);
                        return false;
                    }
                }

                if (npoint == null)
                {
                    // check nearest point
                    npoint = cPath.GetNearestPoint(pInfo, clsCharacter.MyLocation, 15);
                    if (npoint == null)
                    {
                        clsSettings.Logging.AddToLog(Resources.HandleDead, Resources.CouldNotBuildPathFromGhosttoCorpse);
                        return false;
                    }

                    // check if we need to reverse the graveyard path
                    if (npoint.Element > 2)
                    {
                        // we found a path, we need to reverse it (because we are at the GY, which is the
                        // end of the path
                        if (!pInfo.PathReversed)
                            pInfo.ReversePath();
                        else
                            pInfo.SetCurrentStep(); // make sure we reset the current step
                    }

                    // get nearest point
                    pInfo.CurrentStep = cPath.GetNearestPoint(pInfo, clsCharacter.MyLocation, 15).Element;
                }

                // we found a path, run it
                clsSettings.Logging.AddToLogFormatted(Resources.HandleDead, Resources.RunningGraveyardPathX, string.IsNullOrEmpty(pInfo.PathName) ? string.Empty : pInfo.PathName);
                if (cPath.MoveThroughPathEx(pInfo, true) != clsPath.EMovementResult.Success)
                {
                    clsSettings.Logging.AddToLog(Resources.HandleDead, Resources.Couldnotrunpathfromghosttocorpse);
                    return false;
                }

                // if we are not near the corpse, try the other paths
                if (corpsePoint.Distance(clsCharacter.MyLocation) > 50)
                {
                    // find out if we are near the start of the current path
                    clonePath = CurrentPath.Clone();
                    clonePath.RevertToPathForward();
                    nearPoint = cPath.GetNearestPoint(clonePath, clsCharacter.MyLocation);

                    // if no point found, then assume the current path is the hunt path, so
                    // we need the hunt start and hunt paths built
                    if ((nearPoint == null) || (nearPoint.PPoint == null))
                    {
                        // get hunt start
                        dPath = clsPath.GetPathFromList(clsGlobals.Paths, clsGlobals.Path_StartHunt);

                        // get the hunt path
                        clonePath = clsPath.GetPathFromList(clsGlobals.Paths, clsGlobals.Path_Hunting);

                        // if no path, then exit
                        if (clonePath == null)
                        {
                            clsSettings.Logging.AddToLogFormatted(Resources.HandleDead, Resources.CouldNotBuildPath1);
                            return false;
                        }

                        if (clsSettings.VerboseLogging)
                            clsSettings.Logging.AddToLogFormatted(Resources.HandleDead, Resources.MergingHuntStartandHuntpaths);

                        // if we have a hunt start, then insert the hunt start path                        
                        if ((dPath != null) && (dPath.PathList.Count > 0))
                            clonePath.PathList.InsertRange(clonePath.PathList.Count, dPath.PathList);
                    }

                    // get the nearest point to our corpse point
                    // in this case, we will check the search range + 50 yards, in case we wandered
                    // far from the path
                    nearPoint = cPath.GetNearestPoint(clonePath, corpsePoint, clsSettings.gclsLevelSettings.SearchRange + 50);
                    if ((nearPoint == null) || (nearPoint.PPoint == null))
                    {
                        clsSettings.Logging.AddToLogFormatted(Resources.HandleDead, Resources.CouldNotFindPointNearCorpse);
                        return false;
                    }

                    // reset pInfo and build path to it
                    clsSettings.Logging.AddToLog(Resources.HandleDead, Resources.FoundPathFromGhosttoCorpse);
                    if ((nearPoint.Element + 1 < clonePath.PathList.Count) && (clonePath.PathList.Count - nearPoint.Element - 1 > 0))
                        clonePath.PathList.RemoveRange(nearPoint.Element + 1, clonePath.PathList.Count - nearPoint.Element - 1);
                    clonePath.SetCurrentStep();

                    // set pInfo
                    pInfo = clonePath;

                    // we have a path, let's run to our corpse
                    clsSettings.Logging.AddToLog(Resources.HandleDead, Resources.RunningtoCorpse);
                    if (cPath.MoveThroughPathEx(pInfo, true) != clsPath.EMovementResult.Success)
                    {
                        clsSettings.Logging.AddToLog(Resources.HandleDead, Resources.CouldNotRuntoCorpse);
                        return false;
                    }
                }

                // move to the corpse itself
                cPath.MoveToPoint(corpsePoint);

                // find the best spot to rez and return result
                clsSettings.Logging.AddToLog(Resources.HandleDead, Resources.AttemptingToRezAtCorpse);
                rVal = RezMe(true);

                // do downtime
                clsCombat combat = new clsCombat();
                if (combat.NeedDowntime())
                    combat.DoDowntime();

                // reset point
                CurrentPath.SetCurrentStep();
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Ghost - Handle Dead");
                rVal = false;
            }

            return rVal;
        }

        // HandleDead
        #endregion

        #region HandleDeadAutoNav

        /// <summary>
        /// Releases from corpse and runs back to body. Returns true on success, false on failure
        /// </summary>
        public bool HandleDeadAutoNav()
        {
            bool rVal = false;
            clsPath.PathListInfoEx pInfo;
            PathListInfo.PathPoint corpsePoint, myLoc;

            try
            {
                // exit if we are not dead
                if (!clsCharacter.IsDead)
                    return true;

                // log it
                clsSettings.Logging.AddToLog(Resources.HandleDead);

                // release from the corpse
                if (!ReleaseFromBody())
                    return false; // error

                // we should now be at the graveyard. get the corpse point and build a path to there
                using (new clsFrameLock.LockBuffer())
                {
                    corpsePoint = CorpsePoint;
                    myLoc = clsCharacter.MyLocation;
                }

                // log it
                clsSettings.Logging.AddToLog(Resources.HandleDead, "Building a path to the corpse point");

                // build a path to the corpse point
                pInfo = new clsPath.PathListInfoEx("Graveyard Run", false, false, false,
                    new clsPPather().BuildPath(
                        clsCharacter.ZoneText,
                        clsCharacter.MyLocation.ToLocation(),
                        corpsePoint.ToLocation(),
                        clsSettings.gclsLevelSettings.SearchRange));

                // if no path found, exit
                if ((pInfo == null) || (pInfo.PathList == null) || (pInfo.PathList.Count == 0))
                {
                    clsSettings.Logging.AddToLog(Resources.HandleDead, "Could not build a path from the graveyard to the corpse");
                    return false;
                }

                // run the path, set return val
                rVal = (new clsAutoNavPath().MoveThroughAutoNavPath(
                    false, false, false, false, false, false,
                    null, null,
                    pInfo.PathList,
                    null, null) == clsPath.EMovementResult.Success);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Ghost - Handle Dead");
            }

            return rVal;
        }

        // HandleDeadAutoNav
        #endregion

        #region Support_Functions

        /// <summary>
        /// The location of your corpse
        /// </summary>
        public PathListInfo.PathPoint CorpsePoint
        {
            get
            {
                using (new clsFrameLock.LockBuffer())
                    return new PathListInfo.PathPoint(clsSettings.isxwow.Me.Corpse);
            }
        }

        /// <summary>
        /// Releases from the body. returns true on success, false on failure
        /// </summary>
        public bool ReleaseFromBody()
        {
            try
            {
                // log it
                clsSettings.Logging.AddToLog(Resources.ReleaseFromBody);

                // not dead
                if (!clsCharacter.IsDead)
                {
                    clsSettings.Logging.AddToLog(Resources.ReleaseFromBody, Resources.NotDead);
                    return false;
                }

                // send message
                if (clsSettings.SendMsgOnDead)
                    Communication.clsSend.SendToAll("Toon has died", string.Format("{0} - Toon has died", Resources.Rhabot));

                // if already a ghost, then just return true
                if (clsCharacter.IsGhost)
                {
                    clsSettings.Logging.AddToLog(Resources.ReleaseFromBody, Resources.AlreadyGhost);
                    return true;
                }

                // wait 5 seconds
                Thread.Sleep(5000);

                // release from the body
                clsSettings.Logging.AddToLog(Resources.ReleaseFromBody, Resources.ReleasingFromCorpse);
                using (new clsFrameLock.LockBuffer())
                    clsSettings.isxwow.WoWScript("RepopMe()");

                // wait 5 seconds
                Thread.Sleep(5000);

                clsSettings.Logging.AddToLog(Resources.ReleaseFromBody, Resources.Complete);
                return true;
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "ReleaseFromBody");
                return false;
            }
        }

        /// <summary>
        /// Resurrects the body. You must be at/near the corpse. Use RunToCorpse instead
        /// </summary>
        /// <param name="findBestSpot">true to find best spot to rez</param>
        public bool RezMe(bool findBestSpot)
        {
            try
            {
                // exit if not dead
                using (new clsFrameLock.LockBuffer())
                    if ((!clsCharacter.IsDead) || (!clsCharacter.IsGhost))
                        return true;

                // check if the spot is clear
                if ((findBestSpot) && (!IsCorpseClear(CorpsePoint)))
                {
                    // corpse is not clear, so we need to move to a better spot
                    FindBestRezSpot();
                }

                // wait a few seconds
                Thread.Sleep(1000);

                // rez
                using (new clsFrameLock.LockBuffer())
                    clsSettings.isxwow.WoWScript("RetrieveCorpse()");

                // check if we are dead/ghost
                using (new clsFrameLock.LockBuffer())
                {
                    if ((clsCharacter.IsDead) || (clsCharacter.IsGhost))
                    {
                        clsSettings.Logging.AddToLog(Resources.RezMeFailedRez);
                        return false;
                    }
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "RezMe");
                return false;
            }

            // log it and exit
            clsSettings.Logging.AddToLog(Resources.Resurrected);
            return true;
        }

        // Support_Functions
        #endregion

        #region Find Rez Spot

        /* From: ClearSpot v3.12.2007.11.48 by bobbyjayblack
         * Source: isxwow.net
         * 	Contacts: (none are linked to my WoW accounts)
		        Email: bobbyjayblack@gmail.com
		        MSN Messenger: bobbyjayblack@hotmail.com
		        AIM: bobbyjayblack
        */

        /// <summary>
        /// Check if our corspe is clear of mobs. returns true if ok to rez here
        /// </summary>
        private bool IsCorpseClear(PathListInfo.PathPoint RezPoint)
        {
            // default to true. worst case, we rez near mob
            bool rVal = true;
            StringBuilder sb = new StringBuilder();

            try
            {
                // log it
                clsSettings.Logging.AddToLogFormatted(Resources.CheckingIfRezPointClear, RezPoint.ToString());

                // build search string
                // -units,-alive,-hostile,-range 0-39,-origin,${Me.Corpse}
                sb.Append("-units,-alive,-hostile,-range 0-39,");
                sb.AppendFormat("-origin {0},{1},{2}", RezPoint.X, RezPoint.Y, RezPoint.Z);

                // search
                rVal = clsSearch.Search(sb.ToString()).Count == 0;
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, string.Format("IsCorpseClear: {0}", RezPoint));
            }

            // log it
            clsSettings.Logging.AddToLogFormatted("IsCorpseClear", Resources.IsCorpseClearX, rVal);
            return rVal;
        }

        /// <summary>
        /// Find the best spot to resurrect
        /// </summary>
        private void FindBestRezSpot()
        {
            int CordX = 40, CordY = -1;
            int BestX = 0, BestY = 0, BestHostileCount = 1000;

            try
            {
                // log it
                clsSettings.Logging.AddToLogFormatted(Resources.FindBestRezSpotCorpseX, CorpsePoint.ToString());

                // scans from 0 to 90 degrees from your corpse
                int HostileCount;
                while (CordX > 0)
                {
                	CordX--;
                    CordY++;

                    // if we find a spot, move to it. returns number of hostiles
                    HostileCount = FindBestSpot(CordX, CordY);
                    if (HostileCount < BestHostileCount)
                    {
                    	BestHostileCount = HostileCount;
                        BestX = CordX;
                        BestY = CordY;
                    }
                }

                // scans from 90 to 180 degrees from your corpse
                while (CordX > -39)
                {
                	CordX--;
                    CordY--;

                    // if we find a spot, move to it. returns number of hostiles
                    HostileCount = FindBestSpot(CordX, CordY);
                    if (HostileCount < BestHostileCount)
                    {
                    	BestHostileCount = HostileCount;
                        BestX = CordX;
                        BestY = CordY;
                    }
                }

                // scans from 180 to 270 degrees from your corpse
                while (CordX < 0)
                {
                	CordX++;
                    CordY--;

                    // if we find a spot, move to it. returns number of hostiles
                    HostileCount = FindBestSpot(CordX, CordY);
                    if (HostileCount < BestHostileCount)
                    {
                    	BestHostileCount = HostileCount;
                        BestX = CordX;
                        BestY = CordY;
                    }
                }

                // scans from 270 to 3600 degrees from your corpse
                while (CordX < 39)
                {
                	CordX++;
                    CordY++;

                    // if we find a spot, move to it. returns number of hostiles
                    HostileCount = FindBestSpot(CordX, CordY);
                    if (HostileCount < BestHostileCount)
                    {
                    	BestHostileCount = HostileCount;
                        BestX = CordX;
                        BestY = CordY;
                    }
                }

                // log best spot
                clsSettings.Logging.AddToLogFormatted(Resources.FindBestRezSpot, Resources.BestSpotXY, BestX, BestY, BestHostileCount);

                // move to spot
                clsPath cPath = new clsPath();
                clsPath.StopMoving();
                cPath.MoveToPoint(new PathListInfo.PathPoint(
                    CorpsePoint.X + CordX,
                    CorpsePoint.Y + CordY + TenYards, // 10 yards from your corpse (??)
                    CorpsePoint.Z));

                // if we are too far from corpse, then we went too far, return to corpse instead
                if (CorpsePoint.Distance(clsCharacter.MyLocation) > 10)
                {
                    clsSettings.Logging.AddToLog(Resources.FindBestRezSpot, Resources.TooFarFromCorpse);
                    cPath.MoveToPoint(CorpsePoint);
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, string.Format("FindBestRezSpot: {0}", CorpsePoint));
                clsPath.StopMoving();
            }
        }

        /// <summary>
        /// Checks if the spot is clear for rezzing
        /// </summary>
        /// <param name="CordX">X</param>
        /// <param name="CordY">Y</param>
        private int FindBestSpot(int CordX, int CordY)
        {
            StringBuilder sb = new StringBuilder();

            // build search string
            // -units,-alive,-hostile,-range 0-39,-origin,${Me.Corpse}
            sb.Append("-units,-alive,-hostile,-range 0-39,");
            sb.AppendFormat("-origin {0},{1},{2}",
                CorpsePoint.X + CordX,
                CorpsePoint.Y + CordY + TenYards, // 10 yards from your corpse (??)
                CorpsePoint.Z);

            // return search result
            return clsSearch.Search(sb.ToString()).Count;
        }

        // Find Rez Spot
        #endregion

        #region Find Rez Spot by BobbyJack (Lavish Script)

        /*
            / *
	            ClearSpot v3.12.2007.11.48 by bobbyjayblack
	            Credits: Everyone who contributed to the wikipedia
	            Info: This script scans 360 degrees around your corpse
			            and finds the best location with the fewest mobs
			            then it moves there, but doesnt resurrect. (not a bug)
	            Bugs: None that I know of
	            Todo: Collision detection
	            Contacts: (none are linked to my WoW accounts)
		            Email: bobbyjayblack@gmail.com
		            MSN Messenger: bobbyjayblack@hotmail.com
		            AIM: bobbyjayblack
            * /

            function main()
            {
	            call ClearSpot()
            }

            function ClearSpot()
            {
	            ; declares global variables
	            call DeclareVariables
            	
	            ;checks to see if you have no hostiles
	            hostiles:Search[-units,-alive,-hostile,-range 0-39,-origin,${Me.Corpse}]
	            if ${hostiles.Count}==0
	            {
		            return ${Me.Corpse.X} ${Me.Corpse.Y}
	            }
            	
	            ; if you have hostiles
	            else
	            {
		            ; scans from 0 to 90 degrees from your corpse
		            while (${CordX} > 0)
		            {
			            CordX:Set[${CordX}-1]
			            CordY:Set[${CordY}+1]
			            call FindBestSpot
		            }
            		
		            ; scans from 90 to 180 degrees from your corpse
		            while ${CordX} > -39
		            {
			            CordX:Set[${CordX}-1]
			            CordY:Set[${CordY}-1]
			            call FindBestSpot
		            }
            		
		            ; scans from 180 to 270 degrees from your corpse
		            while ${CordX} < 0
		            {
			            CordX:Set[${CordX}+1]
			            CordY:Set[${CordY}-1]
			            call FindBestSpot
		            }
            		
		            ; scans from 270 to 360 degrees from your corpse
		            while ${CordX} < 39
		            {
			            CordX:Set[${CordX}+1]
			            CordY:Set[${CordY}+1]
			            call FindBestSpot
		            }
            		
		            ; moves to the best location found
		            call MoveToBestCord
	            }
            	
	            / * Old stuff I used for finding the max distances.
	            while 1
	            {
	            ;0/180          X.Distance.Max: Abs(39.8) Y.Distance.Max:  0.0
	            ;90/270         X.Distance.Max:      0.0  Y.Distance.Max: Abs(39.8)
	            ;45/135/225/315 X.Distance.Max: Abs(28.0) Y.Distance.Max: Abs(28.0)
	            ;Sqrt(80) = 8.944
	            ;Sqrt(40) = 6.324
		            echo CordX: ${Math.Abs[(${Math.Abs[${Me.Corpse.X}]}-${Math.Abs[${Me.X}]})]}
		            echo CordY: ${Math.Distance[${Me.Y},${Me.Corpse.Y}]}
		            wait 1
	            }
	            * /
            }

            function DeclareVariables()
            {
	            declare hostiles guidlist global
	            declare CordX int global 40
	            declare CordY int global -1
	            declare ScanOrginX float global 0
	            declare ScanOrginY float global 0
	            declare CurrentScanRadius int global 0
	            declare MaxScanDistance int global 0
	            declare BestScanRadius int global 0
	            declare BestHostileCount int global 1000
	            declare BestOrginX float global 0
	            declare BestOrginY float global 0
            }

            function FindBestSpot()
            {
	            ; creates the scan orgin
	            ScanOrginX:Set[${Me.Corpse.X}+${CordX}]
	            ScanOrginY:Set[${Me.Corpse.Y}+${CordY}]
            	
	            ; sets MaxScanDistance equal to the distance between your corpse and the scan orgin
	            MaxScanDistance:Set[${Math.Distance[${Me.Corpse.X},${Me.Corpse.Y},${ScanOrginX},${ScanOrginY}]}]
            	
	            ; sets CurrentScanRadius to 1 and loops until CurrentScanRadius equals MaxScanDistance
	            for(CurrentScanRadius:Set[1]; ${CurrentScanRadius}<${MaxScanDistance}; CurrentScanRadius:Inc)
	            {
		            ; searchs the scan orgin for alive hostiles in a range from 0 to CurrentScanRadius
		            hostiles:Search[-units,-alive,-range 0-${CurrentScanRadius},-origin,${ScanOrginX},${ScanOrginY},${Me.Corpse.Z}]

		            ; sets best location
		            if (${hostiles.Count} <= ${BestHostileCount}) && (${CurrentScanRadius} > ${BestScanRadius})
		            {
			            BestScanRadius:Set[${CurrentScanRadius}]
			            BestHostileCount:Set[0]
			            BestOrginX:Set[${ScanOrginX}]
			            BestOrginY:Set[${ScanOrginY}]
		            }
	            }
            }

            function MoveToBestCord()
            {
	            ; moves until your 2 yards from the best spot
	            while ${Math.Distance[${Me.X},${Me.Y},${BestOrginX},${BestOrginY}]} > 2
	            {
		            face ${BestOrginX} ${BestOrginY}
		            if ${ISXWoW.Facing}
		            {
			            move -stop
		            }
		            if !${ISXWoW.Facing}
		            {
			            move forward
		            }
	            }
	            move -stop
            }         
        */

        // Find Rez Spot by BobbyJack (Lavish Script)
        #endregion
    }
}
