/* By Undrgrnd59 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ISXBotHelper.Properties;
using ISXWoW;

namespace ISXBotHelper
{
    public class clsQuest : IDisposable
    {
        #region Hooks

        /// <summary>
        /// Listens for when the quest details window opens. Clicks Accept for you
        /// </summary>
        public void Hook_QuestDetails()
        {
            clsWowEvents.QuestDetails += clsWowEvents_QuestDetails;
        }

        /// <summary>
        /// Listens for when the last window of the quest displays (where you click the complete button)
        /// </summary>
        public void Hook_QuestComplete()
        {
            clsWowEvents.QuestComplete += clsWowEvents_QuestComplete;
        }

        /// <summary>
        /// Listens for when you hit the progress frame with the "Continue" button
        /// </summary>
        public void Hook_QuestProgress()
        {
            clsWowEvents.QuestProgress += clsWowEvents_QuestProgress;
        }

        // Hooks
        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            // unhook events
            clsWowEvents.QuestDetails -= clsWowEvents_QuestDetails;
            clsWowEvents.QuestComplete -= clsWowEvents_QuestComplete;
            clsWowEvents.QuestProgress -= clsWowEvents_QuestProgress;
        }

        #endregion

        #region Events

        /// <summary>
        /// Raised when the quest details window opens. Clicks Accept for you
        /// </summary>
        void clsWowEvents_QuestDetails()
        {
            try
            {
                // accept the quest
                clsSettings.Logging.AddToLog(Resources.Quest, Resources.AcceptingQuest);
                clsSettings.isxwow.WoWScript("AcceptQuest()");
                Thread.Sleep(500);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "QuestDetails Event");
            }
        }

        /// <summary>
        /// Raised when the last window of the quest displays (where you click the complete button)
        /// </summary>
        void clsWowEvents_QuestComplete()
        {
            try
            {
                clsSettings.Logging.AddToLog(Resources.Quest, Resources.CompletingQuest);

                // get the number of reward choices
                int numOfRewardItems = clsSettings.isxwow.WoWScript<int>("GetNumQuestChoices()");
                if (numOfRewardItems > 0)
                {
                    // choose a random item to get
                    Random randGen = new Random();
                    int itemNum = randGen.Next(1, numOfRewardItems + 1);

                    // complete the quest with a random reward
                    clsSettings.isxwow.WoWScript(string.Format("GetQuestReward({0})",itemNum)); 
                }
                else
                {
                    // no reward to choose just complete it (questframerewardpanel crap? I dunno just leave it)
                    clsSettings.isxwow.WoWScript("GetQuestReward(QuestFrameRewardPanel.itemChoice)");
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Quest Complete Event");
            }
        }

        /// <summary>
        /// Raised when you hit the progress frame with the "Continue" button
        /// </summary>
        void clsWowEvents_QuestProgress()
        {
            try
            {
                clsSettings.Logging.AddToLog(Resources.Quest, Resources.UpdatingQuestProgress);
                clsSettings.isxwow.WoWScript("CompleteQuest()");
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Quest Progress Event");
            }            
        }

        // Events
        #endregion

        #region AutoNav_Quests

        public readonly static List<clsQuestObjective> AutoNavQuestList = new List<clsQuestObjective>();

        // AutoNav_Quests
        #endregion

        #region Public Static Functions

        /// <summary>
        /// SelectLogQuest() - Selects a quest in your quest log based on the quest's name
        /// </summary>
        /// <param name="name">quest name</param>
        public static void SelectLogQuest(string name)
        {
            int questNum = GetNumQuestLogEntries();

            for (int i = 1; i < questNum + 1; i++)
            {
                if (GetQuestLogName(i) == name)
                    clsSettings.isxwow.WoWScript("SelectQuestLogEntry(" + i + ")");
            }
        }

        /// <summary>
        /// SelectQuestLog() - Selects a quest in your log based on its number in the quest list
        /// </summary>
        /// <param name="index">quest number</param>
        public static void SelectLogQuest(uint index)
        {
            clsSettings.isxwow.WoWScript("SelectQuestLogEntry(" + index + ")");
        }

        /// <summary>
        /// GetCompleteQuestNames() - Gives a list of the names of completed quests
        /// </summary>
        /// <returns>list of completed quests (names of quests)</returns>
        public static List<string> GetCompleteQuestNames()
        {
            List<string> rList = new List<string>();

            try
            {
                int numQuests = GetNumQuestLogEntries();
                for (int i = 1; i < numQuests + 1; i++)
                {
                    if (IsQuestComplete(i))
                        rList.Add(GetQuestLogName(i));
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "GetCompleteQuestNames");
            }

            return rList;
        }

        /// <summary>
        /// GetAvailableQuestNames() - returns a list of all available quest names.
        ///                            Must have NPC gossip window open.
        /// </summary>
        /// <returns>List of quest names</returns>
        public static List<string> GetAvailableQuestNames()
        {
            List<string> rList = new List<string>();

            try
            {
                // exit if no gossip window
                if (!UI.GossipOptionsFrame.IsVisible)
                    return null;

                for (uint i = 1; i < 20; i += 2)
                {
                    string questName = clsSettings.isxwow.WoWScript<string>("GetGossipAvailableQuests()", i);
                    if (!string.IsNullOrEmpty(questName))
                        rList.Add(questName);
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "GetAvailableQuestNames");
            }

            return rList;
        }

        /// <summary>
        /// SelectQuestByName() - Selects the available quest based on its name
        /// </summary>
        /// <param name="name">name of the quest to select</param>
        public static bool SelectQuestByName(string name)
        {
            bool rVal = false;

            try
            {
                clsSettings.Logging.AddToLogFormatted(Resources.Quest, Resources.SelectQuestByNameX, name);

                // exit if no gossip window
                if (!UI.GossipOptionsFrame.IsVisible)
                {
                    clsSettings.Logging.AddToLog(Resources.SelectQuestByNameExiting);
                    return false;
                }

                uint questIndex = 0; // the index of the quest to be used later
                for (uint i = 1; i < 20; i += 2)
                {
                    // because this wow function returns the name and level, we only want the name right now (i+1)
                    string questName = clsSettings.isxwow.WoWScript<string>("GetGossipAvailableQuests()", i);
                    if ((!string.IsNullOrEmpty(questName)) && (questName == name))
                        questIndex = (i == 1) ? 1 : i / 2;
                }

                if (questIndex != 0)
                {
                    rVal = true;
                    UI.GossipOptionsFrame.SelectAvailableQuest(questIndex);
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "SelectQuestByName");
            }

            return rVal;
        }

        /// <summary>
        /// SelectActiveQuestByName() - Selects one of your current quests by its name
        /// </summary>
        /// <param name="name">name of the currect quest</param>
        public static bool SelectActiveQuestByName(string name)
        {
            bool rVal = false;
            try
            {
                clsSettings.Logging.AddToLogFormatted(Resources.Quest, Resources.SelectActiveQuestByNameX, name);

                // exit if no gossip window
                if (!UI.GossipOptionsFrame.IsVisible)
                {
                    clsSettings.Logging.AddToLog(Resources.SelectActiveQuestByNameExiting);
                    return false;
                }

                uint questIndex = 0;
                for (uint i = 1; i < 20; i += 2)
                {
                    string questName = clsSettings.isxwow.WoWScript<string>("GetGossipActiveQuests()", i);
                    if ((!string.IsNullOrEmpty(questName)) && (questName == name))
                        questIndex = (i == 1) ? 1 : i / 2;
                }

                if (questIndex != 0)
                {
                    rVal = true;
                    UI.GossipOptionsFrame.SelectActiveQuest(questIndex);
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "SelectActiveQuestByName");
            }

            return rVal;
        }

        /// <summary>
        /// AcceptQuest() - Accepts the quest in the 'detail' quest frame (from an NPC).
        /// </summary>
        public static void AcceptQuest()
        {
            clsSettings.isxwow.WoWScript("AcceptQuest()");
        }

        /// <summary>
        /// AbandonQuest() - Abandons the quest currently selected in your quest log
        /// </summary>
        public static void AbandonQuest()
        {
            clsSettings.isxwow.WoWScript("SetAbandonQuest()");
            clsSettings.isxwow.WoWScript("AbandonQuest()");
        }

        // Public Static Functions
        #endregion

        #region Returns

        /// <summary>
        /// GetQuestObjective() - Returns an objective for the specified quest
        /// </summary>
        /// <param name="objNum">objective number</param>
        /// <param name="qID">quest number (in quest log)</param>
        /// <returns>the objective</returns>
        public static clsQuestObjective GetQuestObjective(int objNum, int qID)
        {
            string description = clsSettings.isxwow.WoWScript<string>("GetQuestLogLeaderBoard(" + objNum + "," + qID + ")", 1);
            string type = clsSettings.isxwow.WoWScript<string>("GetQuestLogLeaderBoard(" + objNum + "," + qID + ")", 2);
            int doneInt = clsSettings.isxwow.WoWScript<int>("GetQuestLogLeaderBoard(" + objNum + "," + qID + ")", 3);
            return new clsQuestObjective(description, type, (doneInt == 1));
        }

        /// <summary>
        /// GetNumQuestObjectives() - Returns the number of objectives in a quest
        /// </summary>
        /// <param name="qID">the number of the quest (in quest log)</param>
        /// <returns>number of quest objectives</returns>
        public static int GetNumQuestObjectives(int qID)
        {
            return clsSettings.isxwow.WoWScript<int>("GetNumQuestLeaderBoards(" + qID + ")");
        }

        /// <summary>
        /// IsQuestSelected() - Tells you if the named quest is selected in the quest log
        /// </summary>
        /// <param name="name">Name of the quest</param>
        /// <returns>Is the quest selected?</returns>
        public static bool IsQuestSelected(string name)
        {
            int questLogSelection = GetQuestLogSelectionIndex();

            return GetQuestLogName(questLogSelection) == name;
        }

        public static int GetQuestLogSelectionIndex()
        {
            return clsSettings.isxwow.WoWScript<int>("GetQuestLogSelection()");
        }

        /// <summary>
        /// GetNumQuestLogEntires() - Retrieves the number of quests you have
        /// </summary>
        /// <returns># of quests you have...</returns>
        public static int GetNumQuestLogEntries()
        {
            return clsSettings.isxwow.WoWScript<int>("GetNumQuestLogEntries()", 1);
        }

        /// <summary>
        /// GetNumQuestLogRewards() - Returns the count of rewards for a quest
        /// </summary>
        /// <returns># of rewards</returns>
        public static int GetNumQuestChoices()
        {
            return clsSettings.isxwow.WoWScript<int>("GetNumQuestChoices()");
        }

        /// <summary>
        /// IsQuestComplete() - Checks if a quest in your quest log is complete
        /// </summary>
        /// <param name="questID">index of the quest in your quest log</param>
        /// <returns>if the specified quest is complete</returns>
        public static bool IsQuestComplete(int questID)
        {
            int complete = clsSettings.isxwow.WoWScript<int>("GetQuestLogTitle(" + questID + ")", 7);
            return complete == 1;
        }

        /// <summary>
        /// GetQuestLogName() - Tells you the name of a quest in your quest log
        /// </summary>
        /// <param name="questID">index of the quest in your quest log</param>
        /// <returns>name of the quest</returns>
        public static string GetQuestLogName(int questID)
        {
            return clsSettings.isxwow.WoWScript<string>("GetQuestLogTitle(" + questID + ")", 1);
        }

        /// <summary>
        /// QuestWindowOpen() - Is the quest window (with accept/decline buttons) open?
        /// </summary>
        /// <returns>if the window is open</returns>
        public static bool QuestWindowOpen()
        {
            string activeTitle = clsSettings.isxwow.WoWScript<string>("GetTitleText()"); // If the quest is already open, this variable won't be null.
            return (!string.IsNullOrEmpty(activeTitle));
        }

        public static clsWoWObjective ParseObjective(clsQuestObjective qObjective)
        {
            // === Parse Slay Objectives ===
            string objective = qObjective.description;
            int totalNeeded = 0;
            int totalDone = 0;
            string mobToKill = "";

            // If it is a quest where you have a /
            if (objective.Contains("/"))
            {
                string[] slashObj = objective.Split('/');
                totalNeeded = Convert.ToInt32(slashObj[1]);

                if (slashObj[0].Contains(":")) // Its a quest with a :
                {
                    string[] colonObj = slashObj[0].Split(':');
                    totalDone = Convert.ToInt32(colonObj[1].Trim());
                    // objective should now look like "Vile Familiar slain"
                    if (colonObj[0].Contains("slain")) // if it is a slain type quest
                    {
                        string[] monsterObj = colonObj[0].Split(' ');
                        for (int i = 0; i < monsterObj.Length; i++)
                        {
                            if (!monsterObj[i].Contains("slain"))
                            {
                                if (mobToKill.Length == 0)
                                    mobToKill += monsterObj[i];
                                else
                                    mobToKill += " " + monsterObj[i];
                            }
                        }
                    }
                }
            }
            return new clsWoWObjective(mobToKill, totalDone, totalNeeded);
        }

        // Returns
        #endregion

        #region Classes

        #region clsQuestObjective

        public class clsQuestObjective
        {
            public string description;
            public string type;
            public bool done;

            public clsQuestObjective(string description, string type, bool done)
            {
                this.description = description;
                this.type = type;
                this.done = done;
            }

            public bool IsSlayQuest()
            {
                return description.Contains("slain");
            }

            public clsWoWObjective ParseObjective()
            {
                // === Parse Slay Objectives ===
                string objective = description;
                int totalNeeded = 0;
                int totalDone = 0;
                StringBuilder mobToKill = new StringBuilder();

                // If it is a quest where you have a /
                if (objective.Contains("/"))
                {
                    string[] slashObj = objective.Split('/');
                    totalNeeded = Convert.ToInt32(slashObj[1]);

                    if (slashObj[0].Contains(":")) // Its a quest with a :
                    {
                        string[] colonObj = slashObj[0].Split(':');
                        totalDone = Convert.ToInt32(colonObj[1].Trim());

                        // objective should now look like "Vile Familiar slain"
                        if (colonObj[0].Contains("slain")) // if it is a slain type quest
                        {
                            string[] monsterObj = colonObj[0].Split(' ');
                            for (int i = 0; i < monsterObj.Length; i++)
                            {
                                if (!monsterObj[i].Contains("slain"))
                                {
                                    if (mobToKill.Length == 0)
                                        mobToKill.Append(monsterObj[i]);
                                    else
                                        mobToKill.AppendFormat(" {0}", monsterObj[i]);
                                }
                            }
                        }
                    }
                }

                return new clsWoWObjective(mobToKill.ToString(), totalDone, totalNeeded);
            }

        }

        // clsQuestObjective
        #endregion

        #region clsMonster

        internal class clsMonster
        {
            public string name;
            public int level;
            public double x;
            public double y;
            public double z;

            public clsMonster(string name, int level, double x, double y, double z)
            {
                this.name = name;
                this.level = level;
                this.x = x;
                this.y = y;
                this.z = z;
            }
        }

        // clsMonster
        #endregion

        #region clsWoWObjective

        public class clsWoWObjective
        {
            public string monsterToSlay;
            public int myCount;
            public int totalCount;

            public clsWoWObjective(string monsterToSlay, int myCount, int totalCount)
            {
                this.monsterToSlay = monsterToSlay;
                this.myCount = myCount;
                this.totalCount = totalCount;
            }
        }

        // clsWoWObjective
        #endregion

        // Classes
        #endregion
    }
}
