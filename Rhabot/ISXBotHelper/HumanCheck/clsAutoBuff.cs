// Code by Undrgrnd59 - Apr 2007

using System;
using System.Collections.Generic;
using System.Threading;
using ISXBotHelper.Properties;
using ISXBotHelper.Threading;
using ISXWoW;

namespace ISXBotHelper
{
    /// <summary>
    /// Automatically buffs out-of-party players
    /// </summary>
    public class clsAutoBuff : ThreadBase
    {
        #region Player Info Class

        private class clsPlayerInfo
        {
            private DateTime m_LastSeen = DateTime.MinValue;
            /// <summary>
            /// Last time we saw this player
            /// </summary>
            public DateTime LastSeen
            {
                get { return m_LastSeen; }
                set { m_LastSeen = value; }
            }

            private DateTime m_LastBuffed = DateTime.MinValue;
            /// <summary>
            /// Last time we emoted to this player
            /// </summary>
            public DateTime LastBuffed
            {
                get { return m_LastBuffed; }
                set { m_LastBuffed = value; }
            }

            private readonly string m_PlayerName = string.Empty;
            /// <summary>
            /// Player's name
            /// </summary>
            public string PlayerName
            {
                get { return m_PlayerName; }
            }

            /// <summary>
            /// Initializes a new instance of the clsPlayerInfo class.
            /// </summary>
            public clsPlayerInfo(string playerName)
            {
                m_LastSeen = DateTime.Now;
                m_LastBuffed = DateTime.Now;
                m_PlayerName = playerName;
            }
        }

        // Player Info Class
        #endregion

        #region Variables

        private readonly List<clsPlayerInfo> PlayerList = new List<clsPlayerInfo>();
        private static clsSettings.ThreadItem threadItem;
        private static clsAutoBuff AutoBuff;

        /// <summary>
        /// time to sleep between searches
        /// </summary>
        private const int SleepTime = 1000;

        /// <summary>
        /// minutes to wait before removing someone from the list
        /// </summary>
        private const int WaitTime = 5;

        /// <summary>
        /// time, in minutes, to wait between emotes
        /// </summary>
        private const int BuffTime = 5;

        private readonly Random rnd = new Random(DateTime.Now.Millisecond);
        

        // Variables
        #endregion

        #region Functions

        #region Start/Stop

        /// <summary>
        /// Starts running the Humanchecker
        /// </summary>
        public static void Start()
        {
            // exit if not valid
            if (!clsSettings.GuidValid)
                return;

            AutoBuff = new clsAutoBuff();
            Thread thread = new Thread(AutoBuff.Start_Thread);
            thread.Name = Resources.AutoBuff;
            threadItem = new clsSettings.ThreadItem(thread, AutoBuff);
            clsSettings.ThreadList.Add(threadItem);
            thread.Start();
        }

        /// <summary>
        /// Stops running Humanchecker
        /// </summary>
        public static void DoShutdown()
        {
            // kill the thread
            clsSettings.KillThread(threadItem, "AutoBuff stopping");
        }

        // Start/Stop
        #endregion

        #region Auto Buff

        /// <summary>
        /// Starts running auto buff in a new thread
        /// </summary>
        private void Start_Thread()
        {
            string searchStr = string.Format("-players,-noself,-alive,-nearest,-range 0-20");

            try
            {
                // loop until the thread is shutdown
                while (! Shutdown)
                {
                    // check for shutdown
                    if (!clsSettings.TestPauseStop(Resources.ExitingDueToScriptStop))
                        return;

                    try
                    {
                        // sleep for the specified time
                        Thread.Sleep(SleepTime);

                        // for safety, exit if shutting down
                        if (clsSettings.IsShuttingDown)
                            break;

                        // clean up the list
                        CleanList();

                        // get the list of players nearby and update the list
                        UpdateList(clsSearch.Search(searchStr));
                    }

                    catch (Exception iexcep)
                    {
                        clsError.ShowError(iexcep, "AutoBuff Loop");
                    }
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "AutoBuff Loop - Exiting due to error");
            }
        }

        /// <summary>
        /// Cleans the list.
        /// </summary>
        private void CleanList()
        {
            List<clsPlayerInfo> pList = new List<clsPlayerInfo>();

            try
            {
                // exit if no list
                if ((PlayerList == null) || (PlayerList.Count == 0))
                    return;

                // remove people from our list who are older than the specified time
                foreach (clsPlayerInfo pInfo in PlayerList)
                {
                    // if the last time seen is older than the specified time, remove him
                    if (pInfo.LastSeen.AddMinutes(WaitTime) <= DateTime.Now)
                        pList.Add(pInfo);
                }

                // remove everyone who is old
                foreach (clsPlayerInfo pRemove in pList)
                    PlayerList.Remove(pRemove);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "AutoBuff.CleanList");
            }
        }

        /// <summary>
        /// Loops through the searchlist and updates the playerlist
        /// </summary>
        /// <param name="searchList">the list of found players</param>
        private void UpdateList(List<WoWUnit> searchList)
        {
            bool CanHeal = !string.IsNullOrEmpty(clsSettings.gclsGlobalSettings.AutoBuff_Heal);

            try
            {
                // exit if no list
                if ((searchList == null) || (searchList.Count == 0))
                    return;

                // exit if in combat
                if (clsCombat.IsInCombat())
                    return;

                // loop through search list
                foreach (WoWUnit sUnit in searchList)
                {
                    // get player's name
                    string PlayerName;
                    using (new clsFrameLock.LockBuffer())
                    {
                        // skip if opposing faction
                        if (sUnit.FactionGroup != clsCharacter.MyFaction)
                            continue;

                        PlayerName = sUnit.Name;
                    }

                    // log it
                    if (clsSettings.VerboseLogging)
                        clsSettings.Logging.AddToLogFormatted(Resources.AutoBuff, Resources.FoundNearbyX, PlayerName);

                    // see if this player already exists in the list
                    bool IsFound = false;
                    foreach (clsPlayerInfo pInfo in PlayerList)
                    {
                        // check if found
                        if (pInfo.PlayerName != PlayerName)
                            continue;

                        IsFound = true;

                        // get unit health
                        double UnitHealthPct = 100;
                        using (new clsFrameLock.LockBuffer())
                            UnitHealthPct = sUnit.HealthPercent;

                        // heal if needed
                        if ((CanHeal) && (!clsPath.Moving) && (sUnit.HealthPercent <= clsSettings.gclsGlobalSettings.AutoBuff_HealPercent))
                        {
                            clsSettings.Logging.AddToLogFormatted(Resources.AutoBuff, Resources.TryingHealX, PlayerName);

                            // stop moving
                            if (clsPath.Moving)
                                clsPath.StopMoving();

                            try
                            {
                                // place the movehold
                                clsPath.MovingHold = true;

                                // wait for stop
                                System.Threading.Thread.Sleep(300);

                                // cast the spell
                                if (clsCombat.CastSpell(clsSettings.gclsGlobalSettings.AutoBuff_Heal, sUnit))
                                {
                                    clsSettings.Logging.AddToLogFormatted(Resources.AutoBuff, Resources.HealedX, PlayerName);

                                    // update player info and continue loop
                                    pInfo.LastBuffed = DateTime.Now;
                                    pInfo.LastSeen = DateTime.Now;
                                    continue;
                                }
                            }
                            catch (Exception iexcep)
                            {
                                clsError.ShowError(iexcep, "AutoBuff", "Casting Heal");
                            }
                            finally
                            {
                                // remove the move hold
                                clsPath.MovingHold = false;
                            }
                        }

                        // skip if no buffs available
                        if ((clsSettings.gclsGlobalSettings.AutoBuff_BuffList == null) || (clsSettings.gclsGlobalSettings.AutoBuff_BuffList.Count == 0))
                            continue;

                        // buff if needed
                        if ((pInfo.LastSeen == DateTime.MinValue) ||
                            (new TimeSpan(DateTime.Now.Ticks - pInfo.LastBuffed.Ticks).Minutes >= BuffTime))
                        {
                            // get the emote
                            string emote = GetRandomEmote();

                            // get the buff
                            string buff = GetRandomBuff();

                            // log it
                            clsSettings.Logging.AddToLogFormatted(Resources.AutoBuff, Resources.EmotingX,
                                emote, buff, PlayerName);

                            // stop moving
                            if (clsPath.Moving)
                                clsPath.StopMoving();

                            try
                            {
                                // place the movehold
                                clsPath.MovingHold = true;

                                // wait for stop
                                System.Threading.Thread.Sleep(300);

                                // cast the spell
                                if (clsCombat.CastSpell(buff, sUnit))
                                    pInfo.LastBuffed = DateTime.Now;
                            }
                            catch (Exception iexcep)
                            {
                                clsError.ShowError(iexcep, "AutoBuff", "Casting Buff");
                            }
                            finally
                            {
                                // remove the move hold
                                clsPath.MovingHold = false;
                            }

                            using (new clsFrameLock.LockBuffer())
                                sUnit.Target();

                            // send the random emote
                            clsSettings.ExecuteWoWAPI(string.Format("DoEmote(\"{0}\", \"{1}\")", emote, pInfo.PlayerName));
                        }

                        // update the info
                        pInfo.LastSeen = DateTime.Now;

                        // move on
                        break;
                    }

                    // if we did not find him in the list, add him now
                    if (!IsFound)
                        PlayerList.Add(new clsPlayerInfo(PlayerName));
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "AutoBuff.UpdateList");
            }
        }

        // http://www.wowwiki.com/API_DoEmote
        // http://www.wowwiki.com/API_TYPE_Emotes_Token
        /// <summary>
        /// Gets a random emote.
        /// </summary>
        private string GetRandomEmote()
        {
            // get a random emote
            switch (rnd.Next(0, 11))
            {
                case 0:
                    return "BOW";
                case 1:
                    return "CHEER";
                case 2:
                    return "DRINK";
                case 3:
                    return "GREET";
                case 4:
                    return "HELLO";
                case 5:
                    return "NOD";
                case 6:
                    return "WAVE";
                case 7:
                    return "BONK";
                case 8:
                    return "FLEX";
                case 9:
                    return "SALUTE";
                case 10:
                    return "RAISE";
            }

            return "WAVE";
        }

        private string GetRandomBuff()
        {
            // return a random buff from the list
            return clsSettings.gclsGlobalSettings.AutoBuff_BuffList[rnd.Next(0, clsSettings.gclsGlobalSettings.AutoBuff_BuffList.Count)];
        }

        // AutoBuff
        #endregion

        // Functions
        #endregion
    }
}
