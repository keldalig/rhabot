using System;
using System.Threading;
using ISXBotHelper;
using ISXBotHelper.Properties;
using ISXBotHelper.Threading;
using ISXWoW;

namespace Rhabot.BotThreads
{
    public class clsCombatOnly : ThreadBase
    {
        private clsSettings.ThreadItem threadItem;
        private bool IsShutdown = false;
        private static readonly string SectionName = Resources.BotThread_CombatOnly;

        #region Start / Stop

        /// <summary>
        /// Starts the bot in a new thread
        /// </summary>
        public void Start()
        {
            try
            {
                IsShutdown = false;
                Thread thread = new Thread(Start_Thread);
                thread.Name = "Rhabot Combat Only";
                threadItem = new clsSettings.ThreadItem(thread, this);
                clsSettings.GlobalThreadList.Add(threadItem);
                thread.Start();
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, SectionName, Resources.RhabotStart);
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

            // detach target change
            clsWowEvents.TargetChanged -= TargetChange;
            
            // stop the thread
            clsSettings.KillThread(threadItem, Resources.RhabotStop);
        }

        // Start / Stop
        #endregion

        #region Run Bot

        /// <summary>
        /// Raised when we are forced to shutdown
        /// </summary>
        private void clsEvent_ForcingShutdown()
        {
            // change shutdown flag
            IsShutdown = true;

            // log it  (Rhabot_Main_Forced_Shutdown)
            clsSettings.Logging.AddToLog(SectionName, Resources.RhabotMainForceQuit);
        }

        /// <summary>
        /// Runs Rhabot from a seperate thread
        /// </summary>
        private void Start_Thread()
        {
            clsCombat combat = new clsCombat();

            try
            {
                // hook forced shutdow
                clsEvent.ForcingShutdown += clsEvent_ForcingShutdown;

                // hook target change event
                HookTargetChangeEvent();

                // do startup
                clsSettings.Start();

                // loop until shutdown
                while ((!Shutdown) && (!clsSettings.IsShuttingDown))
                {
                    // check for forced shutdown
                    if ((Shutdown) || (IsShutdown) || (!clsSettings.TestPauseStop(Resources.ExitingDueToScriptStop)))
                        return;

                    // sleep
                    Thread.Sleep(300);

                    // reset target
                    WoWUnit Target = null;

                    // check if in combat, if so get attacking unit
                    using (new clsFrameLock.LockBuffer())
                    {
                        if (((clsSettings.isxwow.Me.AttackingUnit != null) &&
                            (clsSettings.isxwow.Me.AttackingUnit.IsValid)) ||
                            (clsCombat.IsInCombat()))
                        {
                            // get unit attacking me
                            Target = clsCombat.GetUnitAttackingMe();

                            // if no unit, try get the unit I'm targetting
                            if (Target == null)
                                Target = clsSettings.isxwow.Me.AttackingUnit;
                        }
                    }

                    // if no target, continue loop
                    if (Target == null)
                        continue;

                    // start combat
                    ISXRhabotGlobal.clsGlobals.AttackOutcome CombatOutcome = combat.BeginCombat(Target);

                    // handle outcome
                    switch (CombatOutcome)
                    {
                        case ISXRhabotGlobal.clsGlobals.AttackOutcome.Dead:
                        case ISXRhabotGlobal.clsGlobals.AttackOutcome.Success:
                            continue;

                        case ISXRhabotGlobal.clsGlobals.AttackOutcome.Bugged:
                        case ISXRhabotGlobal.clsGlobals.AttackOutcome.Panic:
                            if (clsCombat_Helper.PanicRun(true) == clsPath.EMovementResult.Stopped)
                                return;
                            continue;

                        case ISXRhabotGlobal.clsGlobals.AttackOutcome.Stopped:
                            return;
                    }

                    // downtime
                    if (combat.NeedDowntime())
                        combat.DoDowntime();
                }
            }

            catch (Exception excep)
            {
                // skip if thread abort
                if (excep.Message.ToLower().Contains(Resources.abort))
                    return;

                // Rhabot_Main_Thread
                clsError.ShowError(excep, SectionName, Resources.RhabotMainThread);

                clsSettings.Stop = true;
            }

            finally
            {
                clsSettings.Logging.AddToLog(SectionName, Resources.Exiting);
            }
        }

        // Run Bot
        #endregion

        #region Target Change Event

        /// <summary>
        /// Hooks the event for when the target changes
        /// </summary>
        public static void HookTargetChangeEvent()
        {
            try
            {
                // log it
                clsSettings.Logging.AddToLog(Resources.HookingTargetChangeEvent);

                // hook it
                clsWowEvents.TargetChanged += TargetChange;
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, SectionName, Resources.HookTargetChangeEvent);
            }
        }

        /// <summary>
        /// Raised by WoW when the target changes
        /// </summary>
        private static void TargetChange()
        {
            WoWUnit unit;

            try
            {
                // exit if setting is disable
                if (!clsSettings.CombatOnly_AttackOnTarget)
                    return;

                // exit if already in combat
                if (! clsCombat.IsInCombat())
                    return;

                // get the target name
                string targetName;
                using (new clsFrameLock.LockBuffer())
                    targetName = clsSettings.isxwow.WoWScript<string>("UnitName(\"target\")");

                // exit if no target
                if (string.IsNullOrEmpty(targetName))
                    return;

                // log it
                clsSettings.Logging.AddToLogFormatted(SectionName, "{1} '{0}'", targetName, Resources.TargetChangedTo);

                // get the unit
                using (new clsFrameLock.LockBuffer())
                    unit = clsSettings.isxwow.Me.CurrentTarget.GetUnit();

                // exit if nothing found
                using (new clsFrameLock.LockBuffer())
                {
                    if ((unit == null) || (!unit.IsValid) || (unit.Dead))
                    {
                        clsSettings.Logging.AddToLog(SectionName, Resources.TargetDeadOrInvalid);
                        return;
                    }
                }

                // target and attack
                using (new clsFrameLock.LockBuffer())
                {
                    clsSettings.Logging.AddToLogFormatted(SectionName, "{1} '{0}'", unit.Name, Resources.TargettingAndAttacking);
                    unit.Target();
                    unit.SpellTarget();
                    if (!clsSettings.isxwow.Me.Attacking)
                        clsSettings.isxwow.Me.ToggleAttack();
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, SectionName, Resources.TargetChange);
            }
        }

        // Target Change Event
        #endregion
    }
}
