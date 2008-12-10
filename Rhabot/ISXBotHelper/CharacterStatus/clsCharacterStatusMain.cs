using System;
using System.Threading;
using ISXBotHelper.Properties;
using ISXBotHelper.Threading;
using ISXWoW;
using System.Text;
using System.Web;

namespace ISXBotHelper.CharacterStatus
{
    internal class clsCharacterStatusMain : ThreadBase
    {
        #region Variables

        private clsSettings.ThreadItem threadItem;
        private rs.clsRS cRS = new rs.clsRS();

        // counters
        int NumTimesDied = 0, KillsToLevel = 0, NextLevelExp = 0;

        // Variables
        #endregion

        #region Properties

        private int m_IsRunning = 0;
        /// <summary>
        /// True if the status updater is currently running
        /// </summary>
        public bool IsRunning
        {
            get { return Thread.VolatileRead(ref m_IsRunning) == 1; }
            set { Thread.VolatileWrite(ref m_IsRunning, value ? 1 : 0); }
        }

        // Properties
        #endregion

        #region Start / Stop

        public void Start()
        {
            // skip if invalid
            if (!clsSettings.GuidValid)
                return;

            // set the running flag
            IsRunning = true;

            // start in a new thread
            Thread thread = new Thread(CharacterStatus_Thread);
            thread.Name = "Character Status Monitor";
            threadItem = new clsSettings.ThreadItem(thread, this);
            clsSettings.ThreadList.Add(threadItem);
            thread.Start();
        }

        public void Stop()
        {
            Shutdown = true;
            IsRunning = false;

            // kill the thread
            if (threadItem != null)
                clsSettings.KillThread(threadItem, "Stopping Character Status Update");
        }

        // Start / Stop
        #endregion

        #region Thread Functions

        private void CharacterStatus_Thread()
        {
#if Rhabot
            try
            {
                // log it
                clsSettings.Logging.AddToLog("Character Status Monitor Started");

                // hook events
                clsWowEvents.PlayerDied += clsWowEvents_PlayerDied;
                clsWowEvents.MonsterDied += clsWowEvents_MonsterDied;
                System.Windows.Forms.Application.DoEvents();

                // sleep 2 minutes so everything can smooth out
                DateTime WakeUpTime = DateTime.Now.AddMinutes(2);
                while ((WakeUpTime > DateTime.Now) && (!Shutdown))
                    Thread.Sleep(new TimeSpan(0, 0, 2));

                // loop through and update every X seconds
                while ((! Shutdown) && (clsSettings.TestPauseStop("Character Status Monitor exiting due to script stop")))
                {
                    // log it
                    if (clsSettings.VerboseLogging)
                        clsSettings.Logging.AddToLog("Character Status Monitor: Upload Character Information");

                    if (clsCharacter.CharacterIsValid)
                    {
                        // create new update object
                        rs.CharacterMonitorService.clsCharacterMonitorInfo CharInfo = new rs.CharacterMonitorService.clsCharacterMonitorInfo();

                        // update status
                        using (new clsFrameLock.LockBuffer())
                        {
                            StringBuilder sb = new StringBuilder();

                            // add all the params
                            CharInfo.CharacterName = clsCharacter.CharacterName;
                            CharInfo.RealmName = clsSettings.isxwow.RealmName;
                            CharInfo.Level = clsCharacter.CurrentLevel;
                            CharInfo.KillsToLevel = KillsToLevel;
                            CharInfo.Stuck = false;
                            CharInfo.NumTimesDied = NumTimesDied;
                            CharInfo.AvgMobCount = 1;
                            CharInfo.Class = clsCharacter.MyClass.ToString();
                            CharInfo.Copper = clsCharacter.MyCopper;
                            CharInfo.BlockPct = Convert.ToInt32(clsSettings.isxwow.Me.BlockPercent);
                            CharInfo.CritPct = Convert.ToInt32(clsSettings.isxwow.Me.CritPercent);
                            CharInfo.DodgePct = Convert.ToInt32(clsSettings.isxwow.Me.DodgePercent);
                            CharInfo.Durability = clsCharacter.DurabilityPercent;
                            CharInfo.HealthPct = Convert.ToInt32(clsCharacter.HealthPercent);
                            CharInfo.IsUnderwater = clsSettings.isxwow.Me.HoldingBreath;
                            CharInfo.IsSwimming = clsSettings.isxwow.Me.IsSwimming;
                            CharInfo.LocationX = clsCharacter.MyLocation.X;
                            CharInfo.LocationY = clsCharacter.MyLocation.Y;
                            CharInfo.LocationZ = clsCharacter.MyLocation.Z;
                            CharInfo.XPToLevel = clsSettings.isxwow.Me.NextLevelExp;
                            CharInfo.PVP = false;
                            CharInfo.Race = clsSettings.isxwow.Me.Race.ToString();
                            CharInfo.IsSitting = clsSettings.isxwow.Me.Sitting;
                            CharInfo.IsStealthed = clsSettings.isxwow.Me.Stealth;
                            CharInfo.RunningTimeSeconds = Convert.ToInt32(Rhabot.clsGlobals.RunningTime.TotalSeconds);
                            CharInfo.Gender = clsSettings.isxwow.Me.Gender.ToString();
                            CharInfo.ZoneName = clsCharacter.ZoneText;
                            CharInfo.SubZoneName = clsSettings.isxwow.Me.SubZone.Name;
                            CharInfo.UpdateTime = 60000; // 60 seconds (time in ms)
                            
                            // pet
                            if ((clsSettings.isxwow.Me.Pet != null) && (clsSettings.isxwow.Me.Pet.IsValid))
                            {
                                CharInfo.PetHealthPct = Convert.ToInt32(clsSettings.isxwow.Me.Pet.HealthPercent);
                                if ((clsSettings.isxwow.Me.Pet.CurrentTarget != null) && (clsSettings.isxwow.Me.Pet.CurrentTarget.IsValid))
                                    CharInfo.PetsTargetName = clsSettings.isxwow.Me.Pet.CurrentTarget.Name;
                                else
                                    CharInfo.PetsTargetName = "None";
                            }
                            else
                            {
                                CharInfo.PetHealthPct = 0;
                                CharInfo.PetsTargetName = "None";
                            }

                            // target
                            WoWUnit target = clsSettings.isxwow.Me.CurrentTarget;
                            string targetName = "None";
                            if ((target != null) && (target.IsValid))
                                targetName = target.Name;
                            CharInfo.TargetName = targetName;
                        }

                        // update the webservice
                        cRS.UpdateCharacterMonitor(clsSettings.LoginInfo.UserID, CharInfo, clsSettings.UpdateText, clsSettings.IsDCd);
                    }

                    // sleep by time scale
                    WakeUpTime = DateTime.Now.AddMinutes(1);
                    while ((WakeUpTime > DateTime.Now) && (!Shutdown))
                        Thread.Sleep(new TimeSpan(0, 0, 5));
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Character Status Monitor");
            }   
         
            finally
            {
                // unhook events
                clsWowEvents.PlayerDied -= clsWowEvents_PlayerDied;
                clsWowEvents.MonsterDied -= clsWowEvents_MonsterDied;

                // clear variables
                NumTimesDied = 0;
                KillsToLevel = 0;
                NextLevelExp = 0;
            }
#endif
        }

        // Thread Functions
        #endregion

        #region Events

        void clsWowEvents_PlayerDied()
        {
            NumTimesDied++;
        }

        void clsWowEvents_MonsterDied(int xpGained, int xpToLevel, int killsToLevel)
        {
            KillsToLevel = killsToLevel;
            NextLevelExp = xpToLevel;
        }

        // Events
        #endregion
    }
}
