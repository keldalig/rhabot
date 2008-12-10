using System;
using System.Collections.Generic;
using System.Threading;
using ISXBotHelper.Properties;
using ISXBotHelper.Threading;
using ISXWoW;

namespace ISXBotHelper
{
    /// <summary>
    /// Checks for recently seen human players and emotes to them
    /// </summary>
    public class clsHumanCheck : ThreadBase
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

            private DateTime m_LastEmoted = DateTime.MinValue;
            /// <summary>
            /// Last time we emoted to this player
            /// </summary>
            public DateTime LastEmoted
            {
                get { return m_LastEmoted; }
                set { m_LastEmoted = value; }
            }

            private readonly string m_PlayerName = string.Empty;
            /// <summary>
            /// Player's name
            /// </summary>
            public string PlayerName
            {
                get { return m_PlayerName; }
            }

            private long m_LastSeenCycle = 0;
            /// <summary>
            /// The cycle number this player was last seen on
            /// </summary>
            public long LastSeenCycle
            {
                get { return m_LastSeenCycle; }
                set { m_LastSeenCycle = value; }
            }

            /// <summary>
            /// Initializes a new instance of the clsPlayerInfo class.
            /// </summary>
            public clsPlayerInfo(string playerName)
            {
                m_LastSeen = DateTime.Now;
                m_LastEmoted = DateTime.Now;
                m_LastSeenCycle = LoopCount;
                m_PlayerName = playerName;
            }
        }

        // Player Info Class
        #endregion

        #region Variables

        private readonly List<clsPlayerInfo> PlayerList = new List<clsPlayerInfo>();
        private static clsSettings.ThreadItem threadItem;

        /// <summary>
        /// time to sleep between searches
        /// </summary>
        private const int SleepTime = 1000;

        /// <summary>
        /// minutes to wait before removing someone from the list
        /// </summary>
        private const int WaitTime = 5;

        /// <summary>
        /// The current loop iteration
        /// </summary>
        private static long LoopCount = 0;

        /// <summary>
        /// time, in minutes, to wait between emotes
        /// </summary>
        private const int EmoteTime = 5;

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

            clsHumanCheck hc = new clsHumanCheck();
            Thread thread = new Thread(hc.Start_Thread);
            thread.Name = "HumanCheck";
            threadItem = new clsSettings.ThreadItem(thread, hc);
            clsSettings.ThreadList.Add(threadItem);
            thread.Start();
        }

        /// <summary>
        /// Stops running Humanchecker
        /// </summary>
        public static void DoShutdown()
        {
            // kill the thread
            clsSettings.KillThread(threadItem, Resources.HumanCheckingStopping);
        }

        // Start/Stop
        #endregion

        #region Human Check

        /// <summary>
        /// Starts running human check in a new thread
        /// </summary>
        private void Start_Thread()
        {
            string searchStr = string.Format("-players,-noself,-alive,-nearest,-range 0-30");

            try
            {
                // loop until the thread is shutdown
                while ((!Shutdown) && (clsSettings.TestPauseStop(Resources.ExitingDueToScriptStop)))
                {
                    try
                    {
                        // inc iteration number
                        LoopCount++;

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
                        clsError.ShowError(iexcep, "HumanCheck Loop");
                    }
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "HumanCheck Loop - Exiting due to error");
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
                clsError.ShowError(excep, "HumanCheck.CleanList");
            }            
        }

        /// <summary>
        /// Loops through the searchlist and updates the playerlist
        /// </summary>
        /// <param name="searchList">the list of found players</param>
        private void UpdateList(List<WoWUnit> searchList)
        {
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
                        clsSettings.Logging.AddToLogFormatted(Resources.HumanCheck, Resources.FoundNearbyX, PlayerName);

                    // see if this player already exists in the list
                    bool IsFound = false;
                    foreach (clsPlayerInfo pInfo in PlayerList)
                    {
                        // check if found
                        if (pInfo.PlayerName == PlayerName)
                        {
                            IsFound = true;

                            // emote if needed
                            if ((new TimeSpan(DateTime.Now.Ticks - pInfo.LastEmoted.Ticks).Minutes >= EmoteTime))
                            {
                                // get the emote
                                string emote = GetRandomEmote();

                                // log it
                                clsSettings.Logging.AddToLogFormatted(Resources.HumanCheck, Resources.EmotingXToY, emote, PlayerName);

                                // send the random emote
                                clsSettings.ExecuteWoWAPI(string.Format("DoEmote(\"{0}\", \"{1}\")", emote, pInfo.PlayerName));

                                // update emote time
                                pInfo.LastEmoted = DateTime.Now;
                            }

                            // update the info
                            pInfo.LastSeen = DateTime.Now;
                            pInfo.LastSeenCycle = LoopCount;
                            if (pInfo.LastEmoted == DateTime.MinValue)
                                pInfo.LastEmoted = DateTime.Now;

                            // move on
                            break;
                        }
                    }

                    // if we did not find him in the list, add him now
                    if (! IsFound)
                        PlayerList.Add(new clsPlayerInfo(PlayerName));
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "HumanCheck.UpdateList");
            }            
        }

        // http://www.wowwiki.com/API_DoEmote
        // http://www.wowwiki.com/API_TYPE_Emotes_Token
        /// <summary>
        /// Gets a random emote.
        /// </summary>
        private string GetRandomEmote()
        {
            Random rnd = new Random(DateTime.Now.Millisecond);

            // get a random emote
            switch (rnd.Next(0, 9))
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
                    return "SALUTE";
                case 8:
                    return "RAISE";
            }

            return "WAVE";
        }

        // Human Check
        #endregion

        // Functions
        #endregion
    }
}
