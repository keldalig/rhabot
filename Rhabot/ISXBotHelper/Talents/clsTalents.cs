// Code by Undrgrnd59 - Apr 2007

using System;
using System.Collections.Generic;
using System.Threading;

namespace ISXBotHelper.Talents
{
    public class clsTalents : Threading.ThreadBase
    {
        #region Start/Stop

        internal void Start()
        {
            try
            {
                // hook the level up event
                clsWowEvents.CharLevel += clsEvent_CharacterLeveled;

                // apply any new talents to the character
                ApplyTalents();
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "clsTalents.Start");
            }            
        }

        internal void Stop()
        {
            try
            {
                // unhook the level up event
                clsEvent.CharacterLeveled -= clsEvent_CharacterLeveled;
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "clsTalents.Stop");
            }            
        }

        // Start/Stop
        #endregion

        #region Apply Talents

        /// <summary>
        /// Applies talents in a new thread
        /// </summary>
        private void ApplyTalents_Thread()
        {
            // we need to sleep for 2 minutes to allow the talents to update
            DateTime WakeUp = DateTime.Now.AddMinutes(2);
            while (WakeUp > DateTime.Now)
            {
                // sleep, exit if shutting down
                Thread.Sleep(new TimeSpan(0, 0, 2));
                if (Shutdown)
                    return;
            }

            // apply the talents
            if (! Shutdown)
                ApplyTalents();
        }

        private void clsEvent_CharacterLeveled(int Level)
        {
            // start up in a new thread, because we should wait like 2 minutes for the talents to update
            Thread ApplyThread = new Thread(ApplyTalents_Thread);
            ApplyThread.Name = "Apply Talents Thread";
            clsSettings.GlobalThreadList.Add(new clsSettings.ThreadItem(ApplyThread, this));
            ApplyThread.Start();
        }

        /// <summary>
        /// Applies available talents to the character
        /// </summary>
        public static void ApplyTalents()
        {
            int myLevel = clsCharacter.CurrentLevel;
            string thisLevelsTalent = string.Empty;

            try
            {
                // exit if not valid, or level less than 10
                if ((!clsSettings.GuidValid) || (!clsCharacter.CharacterIsValid) || (clsCharacter.CurrentLevel < 10))
                    return;

                clsSettings.Logging.AddToLog("ApplyTalents", "Getting Talent List");

                List<clsTalentInfo> tList = GetTalents();
                int avaliablePts = AvaliableTalents;

                clsSettings.Logging.AddToLogFormatted("ApplyTalents", "Available Points = {0}", avaliablePts);

                #region Apply One Point

                // Step 1a. If there's 1 Point to spend...
                int i;
                int talentIndex;
                int treeNum;
                if (avaliablePts == 1)
                {
                    // Step 2a. Search for the talent were supposed to use this level.
                    for (i = 0; i < clsSettings.gclsGlobalSettings.TalentList.Count; i++)
                    {
                        if (clsSettings.gclsGlobalSettings.TalentList[i].level == myLevel)
                        {
                            thisLevelsTalent = clsSettings.gclsGlobalSettings.TalentList[i].name;
                            break;
                        }
                    }

                    // Step 3a. If we found a talent...
                    if (! string.IsNullOrEmpty(thisLevelsTalent))
                    {
                        for (i = 0; i < tList.Count; i++)
                        {
                            // Step 4a. Search our full talent list for the one we want.
                            if (tList[i].name == thisLevelsTalent)
                            {
                                treeNum = tList[i].treeNum;
                                talentIndex = tList[i].talentIndex;
                                // Step 5a. Add the talent!
                                LearnTalent(treeNum, talentIndex);
                                clsSettings.Logging.AddToLogFormatted("Talents", "Applying talent '{0}'", thisLevelsTalent);
                                break;
                            }
                        }
                    }

                    return;
                }

                // Apply One Point
                #endregion

                #region Apply More Than One Point

                // Step 1b. Those basterd's havent used this every level, we have to fill in previous ones for them.
                if (avaliablePts > 1)
                {
                    // Step 2b. Loop through each talent that we have to do.
                    for (i = avaliablePts - 1; i >= 0; i--)
                    {
                        int iThisLevelsTalent = myLevel - i;
                        thisLevelsTalent = string.Empty;

                        // Step 3b. Find the talent name for this level.
                        int q;
                        for (q = 0; q < clsSettings.gclsGlobalSettings.TalentList.Count; q++)
                        {
                            if (clsSettings.gclsGlobalSettings.TalentList[q].level == iThisLevelsTalent)
                            {
                                thisLevelsTalent = clsSettings.gclsGlobalSettings.TalentList[q].name;
                                break;
                            }
                        }

                        if (! string.IsNullOrEmpty(thisLevelsTalent))
                        {
                            // Step 4b. Look in our full talent list for the talent for this level.
                            for (q = 0; q < tList.Count; q++)
                            {
                                if (tList[q].name == thisLevelsTalent)
                                {
                                    treeNum = tList[q].treeNum;
                                    talentIndex = tList[q].talentIndex;
                                    // Step 5b. Add the talent!
                                    LearnTalent(treeNum, talentIndex);
                                    clsSettings.Logging.AddToLogFormatted("Talents", "Applying talent '{0}'", thisLevelsTalent);
                                    Thread.Sleep(5000);
                                    break;
                                }
                            }
                        }
                    }
                }

                // Apply More Than One Point
                #endregion
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "ApplyTalents");
            }
        }

        // Apply Talents
        #endregion

        #region Functions

        public static List<clsPTalent> GetSavedTalentList
        {
            get { return clsSettings.gclsGlobalSettings.TalentList; }
        }

        public static void LearnTalent(int treeNum, int talentNum)
        {
            try
            {
                clsSettings.isxwow.WoWScript(string.Format("LearnTalent({0},{1})", treeNum, talentNum));
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "LearnTalent");
            }
        }

        /* ============= GetMyTalents() =============
         *  Retrieves a list of your current talents
         * ========================================== */

        public static List<clsTalentInfo> MyTalents
        {
            get
            {
                List<clsTalentInfo> rList = new List<clsTalentInfo>();

                try
                {
                    int t1Points = GetTabPointsSpent(1);

                    if (t1Points > 0)
                    {
                        for (int t = 1; t < (t1Points + 1); t++)
                        {
                            clsTalentInfo talent = GetTalentInfo(1, t);
                            if (talent.currentRank > 0)
                                rList.Add(talent);
                        }
                    }

                    int t2Points = GetTabPointsSpent(2);

                    if (t2Points > 0)
                    {
                        for (int q = 1; q < (t2Points + 1); q++)
                        {
                            clsTalentInfo talent = GetTalentInfo(2, q);
                            if (talent.currentRank > 0)
                                rList.Add(talent);
                        }
                    }

                    int t3Points = GetTabPointsSpent(3);

                    if (t3Points > 0)
                    {
                        for (int v = 1; v < (t3Points + 1); v++)
                        {
                            clsTalentInfo talent = GetTalentInfo(3, v);
                            if (talent.currentRank > 0)
                                rList.Add(talent);
                        }
                    }
                }

                catch (Exception excep)
                {
                    clsError.ShowError(excep, "GetMyTalents");
                }

                return rList;
            }
        }

        /* ========== GetTalents() ==========
         * Returns a list of all that classes
         * possible talents. 
         * ================================== */

        public static List<clsTalentInfo> GetTalents()
        {
            List<clsTalentInfo> rList = new List<clsTalentInfo>();

            try
            {
                int t1Points = GetNumTalents(1);

                for (int t = 1; t < (t1Points + 1); t++)
                {
                    clsTalentInfo talent = GetTalentInfo(1, t);
                    rList.Add(talent);
                }

                int t2Points = GetNumTalents(2);

                if (t2Points > 0)
                {
                    for (int q = 1; q < (t2Points + 1); q++)
                    {
                        clsTalentInfo talent = GetTalentInfo(2, q);
                        rList.Add(talent);
                    }
                }

                int t3Points = GetNumTalents(3);
                for (int v = 1; v < (t3Points + 1); v++)
                {
                    clsTalentInfo talent = GetTalentInfo(3, v);
                    rList.Add(talent);
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "GetTalents");
            }

            return rList;
        }

        #endregion

        #region Returns

        public static int AvaliableTalents
        {
            get
            {
                try
                {
                    int p1s = GetTabPointsSpent(1);
                    int p2s = GetTabPointsSpent(2);
                    int p3s = GetTabPointsSpent(3);
                    int pointsSpent = p1s + p2s + p3s;
                    int myLevel = clsCharacter.CurrentLevel;
                    if (myLevel >= 10)
                    {
                        int avaliablePts = (myLevel - 9) - pointsSpent;
                        return avaliablePts;
                    }
                }

                catch (Exception excep)
                {
                    clsError.ShowError(excep, "GetAvaliableTalents");
                }

                return 0;
            }
        }

        public static int GetNumTalents(int treeNum)
        {
            return clsSettings.isxwow.WoWScript<int>(string.Format("GetNumTalents({0})", treeNum));
        }

        public static clsTalentInfo GetTalentInfo(int treeNum, int talentNum)
        {
            try
            {
                using (new clsFrameLock.LockBuffer())
                {
                    string talentName = clsSettings.isxwow.WoWScript<string>(string.Format("GetTalentInfo({0},{1})", treeNum, talentNum), 1);
                    int tier = clsSettings.isxwow.WoWScript<int>(string.Format("GetTalentInfo({0},{1})", treeNum, talentNum), 3);
                    int column = clsSettings.isxwow.WoWScript<int>(string.Format("GetTalentInfo({0},{1})", treeNum, talentNum), 4);
                    int currentRank = clsSettings.isxwow.WoWScript<int>(string.Format("GetTalentInfo({0},{1})", treeNum, talentNum), 5);
                    int maxRank = clsSettings.isxwow.WoWScript<int>(string.Format("GetTalentInfo({0},{1})", treeNum, talentNum), 6);

                    clsTalentInfo t = new clsTalentInfo(talentName, tier, column, currentRank, maxRank, treeNum, talentNum);
                    return t;
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "GetTalentInfo");
            }

            return null;
        }

//        public static string GetTalentTabInfo(int treeNum)
//        {
//            using (new clsFrameLock.LockBuffer())
//            {
//                string tabName = clsSettings.isxwow.WoWScript<string>("GetTalentTabInfo(" + treeNum + ")", 1);
//                int pointsSpent = clsSettings.isxwow.WoWScript<int>("GetTalentTabInfo(" + treeNum + ")", 3);
//            }
//
//            return string.Empty;
//        }

        public static int GetTabPointsSpent(int treeNum)
        {
            int rVal = 0;

            try
            {
                using (new clsFrameLock.LockBuffer())
                {
                    rVal = clsSettings.isxwow.WoWScript<int>(string.Format("GetTalentTabInfo({0})", treeNum), 3);
                }
            }
            
            catch (Exception excep)
            {
            	clsError.ShowError(excep, "GetTabPointsSpent");
            }

            return rVal;
        }

        #endregion

        #region Add Talent

        /// <summary>
        /// Adds the talent to the talent tree, overwriting if this level already exists
        /// </summary>
        /// <param name="Talent">the talent to add</param>
        public static void AddTalent(clsPTalent Talent)
        {
            try
            {
                // loop through and see if this talent level already exists
                foreach (clsPTalent pTalent in clsSettings.gclsGlobalSettings.TalentList)
                {
                    if (pTalent.level == Talent.level)
                    {
                        // found a match. update it and exit
                        pTalent.name = Talent.name;
                        return;
                    }
                }

                // no matches found, add it
                clsSettings.gclsGlobalSettings.TalentList.Add(Talent);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "AddTalent");
            }
            
        }

        /// <summary>
        /// Clears the talent list
        /// </summary>
        public static void ClearTalents()
        {
            clsSettings.gclsGlobalSettings.TalentList.Clear();
        }

        // Add Talent
        #endregion
    }
}
