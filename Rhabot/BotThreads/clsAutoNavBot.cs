using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using ISXBotHelper;
using ISXBotHelper.AutoNav;
using ISXBotHelper.Properties;
using ISXBotHelper.Threading;
using rs;
using System.Linq;
using PathMaker.Graph;
using PathMaker;

namespace Rhabot.BotThreads
{
    internal class clsAutoNavBot : ThreadBase
    {
        #region Variables

        private clsSettings.ThreadItem threadItem;

        // Variables
        #endregion

        #region Start / Stop

        public void Start()
        {
            // exit if the mpq files don't exist
            if (! System.IO.File.Exists(System.IO.Path.Combine(clsSettings.MPQPath, "common.MPQ")))
            {
                // show the error
                clsError.ShowError(new System.IO.FileNotFoundException(), Resources.AutoNav, Resources.nompqfile);

                // raise the stop event
                clsSettings.Stop = true;
                clsGlobals.Raise_FloatStopped();
                return;
            }

            clsSettings.Stop = false;
            Thread thread = new Thread(Start_Thread);
            thread.Name = "AutoNav Bot Thread";
            threadItem = new clsSettings.ThreadItem(thread, this);
            clsSettings.GlobalThreadList.Add(threadItem);
            thread.Start();
        }

        public void Stop()
        {
            // kill the thread
            clsSettings.KillThread(threadItem, Resources.KillingAutoNavBotThread);
        }

        // Start / Stop
        #endregion

        #region ThreadFunc

        private const string Zone = "Azuremyst Isle";
        private static PathListInfo.PathPoint StartLoc = new PathListInfo.PathPoint(-3986.129883, -13922.400391, 100.1);

        private void Start_Thread()
        {
            clsAutoNavPath aPath = new clsAutoNavPath();
            List<clsBlockListItem> condition = new List<clsBlockListItem>();
            clsPath.EMovementResult result = clsPath.EMovementResult.Success;

            try
            {
                // load the current level's settings
                clsSettings.LoadSettings(clsCharacter.CurrentLevel);

                // build the conditions, one level
                condition.Add(new clsBlockListItem(clsBlockListItem.ENodeType.Continue_Until_Character_Is_Level_X, clsCharacter.CurrentLevel + 1, null, string.Empty));
                condition.Add(new clsBlockListItem(clsBlockListItem.ENodeType.Continue_Durability_X_Percent, clsSettings.gclsLevelSettings.DurabilityPercent, null, string.Empty));
                condition.Add(new clsBlockListItem(clsBlockListItem.ENodeType.Continue_Until_Bags_Full, 0, null, string.Empty));

                // add quest objectives
                foreach (clsQuest.clsQuestObjective qObjective in clsQuest.AutoNavQuestList)
                {
                    // skip if not slay
                    if (!qObjective.IsSlayQuest())
                        continue;

                    // get the quest objective info
                    clsQuest.clsWoWObjective ParsedInfo = clsQuest.ParseObjective(qObjective);

                    // add the monster slay info
                    condition.Add(
                        new clsBlockListItem(
                            clsBlockListItem.ENodeType.Continue_Until_X_Mob_Y_Killed,
                            ParsedInfo.totalCount - ParsedInfo.myCount,
                            null,
                            ParsedInfo.monsterToSlay));
                }

                while (result == clsPath.EMovementResult.Success)
                {
                    // log it
                    clsSettings.Logging.AddToLog(Resources.StartingAutoNavBot);

                    // get the list of mobs in this zone
                    List<rs.AutoNav.clsDPathPoint> MobList = new clsRS().GetUnfriendlyUnitsInZone(
                        clsSettings.LoginInfo.UserID,
                        clsCharacter.ZoneText,
                        clsCharacter.MobLowLevel,
                        clsCharacter.MobHighLevel,
                        clsCharacter.MyFaction == ISXWoW.WoWFactionGroup.Alliance,
                        null,
                        clsSettings.gclsLevelSettings.SearchRange,
                        clsSettings.UpdateText,
                        clsSettings.IsDCd);

                    // exit if no units
                    if ((MobList == null) || (MobList.Count == 0))
                    {
                        clsError.ShowError(new Exception(Resources.CouldNotBuildPathToTargetPoint), Resources.AutoNav, Resources.AutoNavPath, true, new StackFrame(0, true), false);
                        return;
                    }

                    // log it
                    clsSettings.Logging.AddToLogFormatted(Resources.AutoNav, "Found {0} points", MobList.Count);

                    // build a path
                    List<clsPathPoint> CombatList = new clsPPather().BuildPath(
                        clsCharacter.ZoneText,
                        clsCharacter.MyLocation.ToLocation(),
                        MobList.ConvertAll(x => new Location((float)x.X, (float)x.Y, (float)x.Z)).ToList(),
                        clsSettings.gclsLevelSettings.SearchRange);

                    // if no points, then show error and exit
                    if ((CombatList == null) || (CombatList.Count == 0))
                    {
                        clsError.ShowError(new Exception(Resources.CouldNotBuildPathToTargetPoint), Resources.AutoNav, string.Empty, true, new StackFrame(0, true), false);
                        return;
                    }

                    // get a path to run
                    clsPath.PathListInfoEx PathList = new clsPath.PathListInfoEx("AutoNav Path",false, false, false, CombatList);

                    // log it
                    clsSettings.Logging.AddToLog(Resources.RunningAutoNavPath);

                    // run the path for one level
                    result = aPath.MoveThroughAutoNavPath(
                        true, true, true, true, false, false,
                        null, null,
                        PathList.PathList,
                        condition,
                        this);

                    // handle result
                    switch (result)
                    {
                        case clsPath.EMovementResult.Dead:
                            // handle dead
                            if (new clsGhost().HandleDeadAutoNav())
                                result = clsPath.EMovementResult.Success;
                            break;

                        case clsPath.EMovementResult.NeedVendor:
                            // TODO: get nearest vendor
                            break;
                    }

                    // load the next level's settings
                    clsSettings.LoadSettings(clsCharacter.CurrentLevel);
                }

                // log it
                clsSettings.Logging.AddToLog(Resources.AutoNavBotCompleted);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.AutoNav);
            }
            finally
            {
                // raise the stop event
                clsSettings.Stop = true;
                clsGlobals.Raise_FloatStopped();
            }
        }

        // ThreadFunc
        #endregion
    }
}
