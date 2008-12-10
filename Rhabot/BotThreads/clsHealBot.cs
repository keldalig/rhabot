using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using ISXBotHelper;
using ISXBotHelper.Properties;
using ISXBotHelper.Threading;
using ISXWoW;

namespace Rhabot.BotThreads
{
    internal class clsHealBot : ThreadBase
    {
        #region Variables

        private bool IsShutdown = false;
        private clsSettings.ThreadItem theadItem;
        private WoWUnit TargetUnit = null;
        
        // Variables
        #endregion

        #region Start/Stop

        public void Start()
        {
            try
            {
                // exit if no player info
                if ((string.IsNullOrEmpty(clsSettings.gclsGlobalSettings.HealBot_Target)) || (string.IsNullOrEmpty(clsSettings.gclsGlobalSettings.HealBot_HealSpell)))
                {
                    // No target and/or heal spell found in HealBot settings. HealBot can not start
                    clsError.ShowError(new Exception(Resources.NoTargetAndHealSpellFound), Resources.HealBot, string.Empty, true, new StackFrame(0, true), false);
                    return;
                }

                IsShutdown = false;
                Thread thread = new Thread((Start_Thread));
                thread.Name = "HealBot";
                theadItem = new clsSettings.ThreadItem(thread, this);
                clsSettings.GlobalThreadList.Add(theadItem);
                thread.Start();
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.HealBot);
            }
        }

        /// <summary>
        /// Stops the thread
        /// </summary>
        public void Stop()
        {
            // stop the bot
            IsShutdown = true;
            clsSettings.Stop = true;
            clsSettings.KillThread(theadItem, Resources.RhabotStop);
        }

        // Start/Stop
        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Stop();
        }

        #endregion

        #region Run Thread

        private void Start_Thread()
        {
            List<WoWUnit> SearchList;
            WoWBuff wbuff;
            clsGhost ghost = new clsGhost();

            try
            {
                // build search string
                string SearchStr = string.Format("-players,{0},-exact", clsSettings.gclsGlobalSettings.HealBot_Target.Trim());

                // loop until stopped
                while (true)
                {
                    // check for stop/pause/shutdown
                    // HealBot Stopping
                    if ((IsShutdown) || (! clsSettings.TestPauseStop(Resources.HealBotStopping)))
                    {
                        // Exiting due to script stop
                    	clsSettings.Logging.AddToLog(Resources.ExitingDueToScriptStop);
                        return;
                    }

                    // if we are dead, run back
                    if ((clsCharacter.IsDead) && (!ghost.HandleDead(null, null)))
                    {
                        // could not find or rez at corpse
                        clsError.ShowError(new Exception(Resources.CouldNotRezAtCorpse), Resources.HealBot, string.Empty, true, new StackFrame(0, true), false);
                        Stop();
                        return;
                    }

                    // sleep 1 second
                    Thread.Sleep(1000);

                    #region Check for Valid / Live Target

                    // get our unit if we don't have it for some reason
                    if (! TargetIsValid)
                    {
                        // HealBotTargetLost
                        clsSettings.Logging.AddToLog(Resources.HealBot, Resources.TargetLost);

                        // search
                        SearchList = clsSearch.Search_Unit(SearchStr);

                        // if nothing returned, wait, then reloop
                        if ((SearchList == null) || (SearchList.Count == 0))
                        {
                            // HealBotTargetNotFound
                            clsSettings.Logging.AddToLog(Resources.HealBot, Resources.HealBotTargetNotFound);
                            continue;
                        }

                        // found our target, let's get him
                        using (new clsFrameLock.LockBuffer())
                        {
                            TargetUnit = SearchList[0];

                            // if unit is still invalid, then skip and restart loop
                            if (! TargetIsValid)
                            {
                                // HealBotTargetNotFound
                                clsSettings.Logging.AddToLog(Resources.HealBot, Resources.HealBotTargetNotFound);
                                continue;
                            }
                        }
                        SearchList.Clear();
                    }

                    // if unit is dead, then just wait
                    using (new clsFrameLock.LockBuffer())
                    {
                        if (TargetUnit.Dead)
                        {
                            // HealBotTargetDead
                            clsSettings.Logging.AddToLogFormatted(Resources.HealBot, Resources.HealBotTargetDead);
                            continue;
                        }
                    }

                    // Check for Valid / Live Target
                    #endregion

                    #region Check for Heal / Buffs

                    // move closer if too far away
                    if (clsPath.DistanceToTarget(TargetUnit) > 15)
                        clsPath.MoveToTarget(TargetUnit, 10);

                    // check for heal
                    if (TargetHealthPct <= clsSettings.gclsGlobalSettings.HealBot_HealPercent)
                    {
                        clsPath.StopMoving();
                        clsSettings.Logging.AddToLogFormatted(Resources.HealBot, Resources.HealingX, clsSettings.gclsGlobalSettings.HealBot_Target);
                        clsCombat.CastSpell(clsSettings.gclsGlobalSettings.HealBot_HealSpell, TargetUnit);
                    }

                    // check for aggro. if begincombat, heal target first
                    CheckAggro();

                    // check for buffs
                    if (TargetIsValidNotDead)
                    {
                        foreach (string buff in clsSettings.gclsGlobalSettings.HealBot_BuffList)
                        {
                            // cast if the buff doesn't exist
                            if (! clsCharacter.BuffExists(TargetUnit, buff))
                            {
                                // move closer if too far away
                                if (clsPath.DistanceToTarget(TargetUnit) > 15)
                                    clsPath.MoveToTarget(TargetUnit, 10);

                                // cast the buff
                                clsCombat.CastSpell(buff, TargetUnit);
                            }
                        }
                    }

                    // Check for Heal / Buffs
                    #endregion

                    #region Check for Poison / Disease / Curse

                    // only process if we have remove spells
                    if ((!string.IsNullOrEmpty(clsSettings.gclsGlobalSettings.HealBot_RemoveCurse)) ||
                        (!string.IsNullOrEmpty(clsSettings.gclsGlobalSettings.HealBot_RemoveDisease)) ||
                        (!string.IsNullOrEmpty(clsSettings.gclsGlobalSettings.HealBot_RemovePoison)))
                    {
                        // loop through all buffs on the toon
                        for (int buffCounter = 0; buffCounter < 41; buffCounter++)
                        {
                            try
                            {
                                string debuffSPell = string.Empty;

                                // get the buff
                                using (new clsFrameLock.LockBuffer())
                                {
                                    wbuff = TargetUnit.Buff(buffCounter);

                                    // skip if no buff or invalid or not harmful
                                    if ((wbuff == null) || (!wbuff.IsValid) || (!wbuff.IsHarmful))
                                        continue;

                                    // log it (HealBotFoundDebuff)
                                    clsSettings.Logging.AddToLogFormatted(Resources.HealBot, Resources.FoundDebuff, wbuff.Name);

                                    // cure poison
                                    if ((wbuff.DispelType == "Poison") && (!string.IsNullOrEmpty(clsSettings.gclsGlobalSettings.HealBot_RemovePoison)))
                                    {
                                        // HealBotRemovingPoison
                                        clsSettings.Logging.AddToLog(Resources.HealBot, Resources.RemovingPoison);
                                        debuffSPell = clsSettings.gclsGlobalSettings.HealBot_RemovePoison;
                                    }

                                    // cure disease
                                    if ((wbuff.DispelType == "Disease") && (!string.IsNullOrEmpty(clsSettings.gclsGlobalSettings.HealBot_RemoveDisease)))
                                    {
                                        // HealBotRemovingDisease
                                        clsSettings.Logging.AddToLog(Resources.HealBot, Resources.RemovingDisease);
                                        debuffSPell = clsSettings.gclsGlobalSettings.HealBot_RemoveDisease;
                                    }

                                    // remove curse
                                    if ((wbuff.DispelType == "Curse") && (!string.IsNullOrEmpty(clsSettings.gclsGlobalSettings.HealBot_RemoveCurse)))
                                    {
                                        clsSettings.Logging.AddToLog(Resources.HealBot, Resources.RemovingCurse);
                                        debuffSPell = clsSettings.gclsGlobalSettings.HealBot_RemoveCurse;
                                    }
                                }

                                // cast the spell
                                if (! string.IsNullOrEmpty(debuffSPell))
                                    clsCombat.CastSpell(debuffSPell, TargetUnit);
                            }
                            catch { }
                        }
                    }

                    // Check for Poison / Disease / Curse
                    #endregion
                }
            }

            catch (Exception excep)
            {
                // skip if thread abort
                if (excep.Message.ToLower().Contains(Resources.abort))
                    return;

                clsError.ShowError(excep, Resources.HealBotThread);

                clsSettings.Stop = true;
            }            
        }

        /// <summary>
        /// Handles combat. Heals unit before combat starts
        /// </summary>
        private void CheckAggro()
        {
            clsCombat combat = new clsCombat();
            WoWUnit attackingUnit;

            try
            {
                // check for aggro
                attackingUnit = clsCombat.GetUnitAttackingMe();
                if ((attackingUnit == null) || (!attackingUnit.IsValid))
                    return;

                // we have aggro, heal first
                if (TargetIsValidNotDead)
                    clsCombat.CastSpell(clsSettings.gclsGlobalSettings.HealBot_HealSpell, TargetUnit);

                // do combat
                combat.BeginCombat(attackingUnit);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.HealBot, Resources.CheckAggro);
            }
        }

        #region Properties

        private bool TargetIsValid
        {
            get
            {
                using (new clsFrameLock.LockBuffer())
                    return ((TargetUnit != null) && (TargetUnit.IsValid));
            }
        }

        private double TargetHealthPct
        {
            get
            {
                using (new clsFrameLock.LockBuffer())
                    return TargetUnit.HealthPercent;
            }
        }

        private bool TargetIsValidNotDead
        {
            get
            {
                using (new clsFrameLock.LockBuffer())
                    return ((TargetUnit.IsValid) && (!TargetUnit.Dead));
            }
        }

        // Properties
        #endregion

        // Run Thread
        #endregion
    }
}
