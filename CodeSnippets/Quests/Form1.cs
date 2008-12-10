using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using ISXWoW;
using LavishVMAPI;
using InnerSpaceAPI;

namespace ISXQuester
{
    public partial class ISXQuester : Form
    {
        ISXWoW.ISXWoW isxwow = new ISXWoW.ISXWoW();

        public ISXQuester()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // ===Accept Quests===

            // This hooks the event to select quests
            // and then selects the quest from the NPC it is told (gossip menu)
            clsQuesting quester = new clsQuesting();
            quester.SetupEvents("QUEST_DETAILS");
            quester.SelectQuestByName(textBox1.Text); // I didn't add in the .ToUpper() like I should have, quest names need to be an exact match
            // When it selects it, the quest_details event is fired
            // check the todo for that event inside the class...
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // ===Turn-in Quests===

            /* This hooks 2 events as you can see. The first event is for if you get the window
             * that is between the "gossip" and actual "quest turnin" window where you have to 
             * press 'Continue' normally.
             * The second event is when you get the window to complete the quest. Currently if you
             * have a choice of quest reward items, it will pick a random one to get (maybe in the
             * future we can make an item-rating system to determine upgrades?)
             * Finally we select the quest we want to turnin and then the 2 events I told about above
             * will take over to finish the turnin. */
            
            clsQuesting quester = new clsQuesting();
            quester.SetupEvents("QUEST_COMPLETE");
            quester.SetupEvents("QUEST_PROGRESS");
            quester.SelectActiveQuestByName(textBox1.Text);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // ===View Complete Quests==

            /* This shows you how to get a list of completed quests.
             * clsQuesting.GetCompleteQuestNames() should also retrieve
             * a List<string> of completed quests. This would be used
             * when you get back to town so you know what to turn in. */

            clsQuesting quester = new clsQuesting();
            int numQuests = quester.GetNumQuestLogEntries();
            for (int i = 1; i < numQuests + 1; i++)
            {
                if (quester.IsQuestComplete(i))
                {
                    debugBox.Items.Add(quester.GetQuestLogName(i));
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // ===Abandon Quests===

            /* First of all it selects the quest in your quest log
             * that you want to abandon. This is necessary because the
             * abandon quest function just abandons what is selected.
             * Next it checks to be sure that the selected quest is what
             * you want it to be, and if it is, abandons it. (Incase the
             * quest you told it to delete doesn't exist, you don't want
             * it abandoning randomly selected quests do you =P) */

            clsQuesting quester = new clsQuesting();
            quester.SelectLogQuest(textBox1.Text);
            if(quester.IsQuestSelected(textBox1.Text))
                quester.AbandonQuest();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // === List Quest Objectives ===

            /* The sample quest I used was "Vile Familiars" in Valley of Trials, Durotar
             * It's only objective was "Vile Familiar slain: 0/12"
             * 
             * I made what I'm guessing is a very inefficient way to parse
             * out the information form that line. In the end if you take
             * that objective and parse it, you can get the monsterToKill,
             * ammount you have killed so far, and ammount you need to kill. */
            
            debugBox.Items.Clear();
            clsQuesting quester = new clsQuesting();

            int questNum = quester.GetNumQuestLogEntries();
            for (int i = 1; i < questNum+1; i++) // Loop through however many quests we have
            {
                int objectiveNum = quester.GetNumQuestObjectives(i);
                if (objectiveNum > 0) // If it is an actual quest (and not a header such as 'Elwynn Forest'
                {
                    debugBox.Items.Add(quester.GetQuestLogName(i));
                    debugBox.Items.Add("========================");
                    for (int q = 1; q < objectiveNum+1; q++) // Loop through however many objectives there are
                    {
                        debugBox.Items.Add("\tComplete? " + quester.GetQuestObjective(q, i).done);
                        debugBox.Items.Add("\tType: " + quester.GetQuestObjective(q, i).type);
                        debugBox.Items.Add("\t"+quester.GetQuestObjective(q, i).description); 
                        if (quester.GetQuestObjective(q, i).IsSlayQuest()) // If its a 'slay' type quest we can parse information
                        {
                            debugBox.Items.Add("\t== PARSED INFORMATION ==");
                            debugBox.Items.Add("\t\tMonster To Slay: " + quester.GetQuestObjective(q, i).ParseObjective().monsterToSlay);
                        }
                    }
                }
            }
        }

    }

    class clsWoWObjective
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

    class clsQuesting
    {
        #region Objects
        ISXWoW.ISXWoW isxwow = new ISXWoW.ISXWoW();
        #endregion

        #region Events
        // Event setup for when we get a new quest details frame.
        private EventHandler<LavishScriptAPI.LSEventArgs> dQuestDetailFrame = null;
        private delegate void NewDetailFrameHandler();
        private event NewDetailFrameHandler NewDetailFrame;
        // Event setup for the quest complete frame
        private EventHandler<LavishScriptAPI.LSEventArgs> dQuestCompleteFrame = null;
        private delegate void NewQuestCompleteFrameHandler();
        private event NewQuestCompleteFrameHandler NewQuestCompleteFrame;
        // Event setup for the quest progress frame
        private EventHandler<LavishScriptAPI.LSEventArgs> dQuestProgressFrame = null;
        private delegate void NewQuestProgressFrameHandler();
        private event NewQuestProgressFrameHandler NewQuestProgressFrame;

        public void SetupEvents(string Event)
        {
            if (Event == "QUEST_DETAILS")
            {
                if (dQuestDetailFrame == null)
                {
                    dQuestDetailFrame = QuestDetailAlert;
                    LavishScriptAPI.LavishScript.Events.AttachEventTarget("QUEST_DETAIL", dQuestDetailFrame);
                }
            }
            if (Event == "QUEST_COMPLETE")
            {
                if (dQuestCompleteFrame == null)
                {
                    dQuestCompleteFrame = QuestCompleteFrameAlert;
                    LavishScriptAPI.LavishScript.Events.AttachEventTarget("QUEST_COMPLETE", dQuestCompleteFrame);
                }
            }
            if (Event == "QUEST_PROGRESS")
            {
                if (dQuestProgressFrame == null)
                {
                    dQuestProgressFrame = QuestProgressFrameAlert;
                    LavishScriptAPI.LavishScript.Events.AttachEventTarget("QUEST_PROGRESS", dQuestProgressFrame);
                }
            }
        }

        /* ==EVENT:
         * Fired when you get to the frame that lets you press "Accept" */
        public void QuestDetailAlert(object sender, LavishScriptAPI.LSEventArgs e)
        {
            // TODO: - Check if we should accept or decline the quest...
            //       - Fix problem with frame not closing
            AcceptQuest();
            
            if (NewDetailFrame != null)
                NewDetailFrame();
        }
        
        /* ==EVENT: 
         * Fired when you get to the frame that lets you press "Complete Quest" */
        public void QuestCompleteFrameAlert(object sender, LavishScriptAPI.LSEventArgs e)
        {
            int numOfRewardItems = GetNumQuestChoices();
            if (numOfRewardItems > 0)
            {
                // choose a random item to get
                Random randGen = new Random();
                int itemNum = randGen.Next(1, numOfRewardItems + 1);
                isxwow.WoWScript("GetQuestReward("+itemNum+")"); // complete the quest with a random reward
            }
            else
            {
                // no reward to choose just complete it (questframerewardpanel crap? I dunno just leave it)
                isxwow.WoWScript("GetQuestReward(QuestFrameRewardPanel.itemChoice)");
            }

            if (NewQuestCompleteFrame != null)
                NewQuestCompleteFrame();
        }

        /* ===EVENT:
         * Fired when you hit the progress frame with the "Continue" button */
        public void QuestProgressFrameAlert(object sender, LavishScriptAPI.LSEventArgs e)
        {
            isxwow.WoWScript("CompleteQuest()");

            if (NewQuestProgressFrame != null)
                NewQuestProgressFrame();
        }

        #endregion

        #region Functions

        /// <summary>
        /// SelectLogQuest() - Selects a quest in your quest log based on the quest's name
        /// </summary>
        /// <param name="name">quest name</param>
        public void SelectLogQuest(string name)
        {
            int questNum = GetNumQuestLogEntries();

            for (int i = 1; i < questNum + 1; i++)
            {
                if (GetQuestLogName(i) == name)
                {
                    isxwow.WoWScript("SelectQuestLogEntry(" + i + ")");
                }
            }
        }

        /// <summary>
        /// SelectQuestLog() - Selects a quest in your log based on its number in the quest list
        /// </summary>
        /// <param name="index">quest number</param>
        public void SelectLogQuest(uint index)
        {
            isxwow.WoWScript("SelectQuestLogEntry(" + index + ")");
        }

        /// <summary>
        /// GetCompleteQuestNames() - Gives a list of the names of completed quests
        /// </summary>
        /// <returns>list of completed quests (names of quests)</returns>
        public List<string> GetCompleteQuestNames()
        {
            List<string> rList = new List<string>();

            int numQuests = GetNumQuestLogEntries();
            for (int i = 1; i < numQuests + 1; i++)
            {
                if (IsQuestComplete(i))
                {
                    rList.Add(GetQuestLogName(i));
                }
            }

            return rList;
        }

        /// <summary>
        /// GetAvailableQuestNames() - returns a list of all available quest names.
        ///                            Must have NPC gossip window open.
        /// </summary>
        /// <returns>List of quest names</returns>
        public List<string> GetAvailableQuestNames()
        {
            List<string> rList = new List<string>();

            if (ISXWoW.UI.GossipOptionsFrame.IsVisible)
            {

                for (uint i = 1; i < 20; i += 2)
                {
                    string questName = isxwow.WoWScript<string>("GetGossipAvailableQuests()", i);
                    if (questName != null && questName != "")
                    {
                        rList.Add(questName);
                    }
                }
            }

            return rList;
        }

        /// <summary>
        /// SelectQuestByName() - Selects the available quest based on its name
        /// </summary>
        /// <param name="name">name of the quest to select</param>
        public void SelectQuestByName(string name)
        {
            if (ISXWoW.UI.GossipOptionsFrame.IsVisible)
            {
                uint questIndex = 0; // the index of the quest to be used later

                for (uint i = 1; i < 20; i += 2)
                {
                    // because this wow function returns the name and level, we only want the name right now (i+1)
                    string questName = isxwow.WoWScript<string>("GetGossipAvailableQuests()", i);
                    if (questName != null && questName != "")
                    {
                        if (questName == name)
                        {
                            if (i == 1)
                            {
                                questIndex = 1;
                            }
                            else
                            {
                                questIndex = i / 2;
                            }
                        }
                    }
                }

                if (questIndex != 0)
                    ISXWoW.UI.GossipOptionsFrame.SelectAvailableQuest(questIndex);
            }
        }

        /// <summary>
        /// SelectActiveQuestByName() - Selects one of your current quests by its name
        /// </summary>
        /// <param name="name">name of the currect quest</param>
        public void SelectActiveQuestByName(string name)
        {
            if (ISXWoW.UI.GossipOptionsFrame.IsVisible)
            {
                uint questIndex = 0;

                for (uint i = 1; i < 20; i += 2)
                {
                    string questName = isxwow.WoWScript<string>("GetGossipActiveQuests()", i);
                    if (questName == name)
                    {
                        if (i == 1)
                        {
                            questIndex = 1;
                        }
                        else
                        {
                            questIndex = i / 2;
                        }
                    }
                }

                if (questIndex != 0)
                    ISXWoW.UI.GossipOptionsFrame.SelectActiveQuest(questIndex);
            }
        }

        /// <summary>
        /// AcceptQuest() - Accepts the quest in the 'detail' quest frame (from an NPC).
        /// </summary>
        public void AcceptQuest()
        {
            isxwow.WoWScript("AcceptQuest()");
        }

        /// <summary>
        /// AbandonQuest() - Abandons the quest currently selected in your quest log
        /// </summary>
        public void AbandonQuest()
        {
            isxwow.WoWScript("SetAbandonQuest()");
            isxwow.WoWScript("AbandonQuest()");
        }

        #endregion

        #region Returns

        /// <summary>
        /// GetQuestObjective() - Returns an objective for the specified quest
        /// </summary>
        /// <param name="objNum">objective number</param>
        /// <param name="qID">quest number (in quest log)</param>
        /// <returns>the objective</returns>
        public clsQuestObjective GetQuestObjective(int objNum, int qID)
        {
            string description = isxwow.WoWScript<string>("GetQuestLogLeaderBoard(" + objNum + "," + qID + ")", 1);
            string type = isxwow.WoWScript<string>("GetQuestLogLeaderBoard(" + objNum + "," + qID + ")", 2);
            int doneInt = isxwow.WoWScript<int>("GetQuestLogLeaderBoard(" + objNum + "," + qID + ")", 3);
            bool done = false;
            if (doneInt == 1)
                done = true;
            return new clsQuestObjective(description, type, done);
        }

        /// <summary>
        /// GetNumQuestObjectives() - Returns the number of objectives in a quest
        /// </summary>
        /// <param name="qID">the number of the quest (in quest log)</param>
        /// <returns>number of quest objectives</returns>
        public int GetNumQuestObjectives(int qID)
        {
            return isxwow.WoWScript<int>("GetNumQuestLeaderBoards("+qID+")");
        }

        /// <summary>
        /// IsQuestSelected() - Tells you if the named quest is selected in the quest log
        /// </summary>
        /// <param name="name">Name of the quest</param>
        /// <returns>Is the quest selected?</returns>
        public bool IsQuestSelected(string name)
        {
            int questLogSelection = GetQuestLogSelectionIndex();

            if (GetQuestLogName(questLogSelection) == name)
                return true;

            return false;
        }

        public int GetQuestLogSelectionIndex()
        {
            return isxwow.WoWScript<int>("GetQuestLogSelection()");
        }

        /// <summary>
        /// GetNumQuestLogEntires() - Retrieves the number of quests you have
        /// </summary>
        /// <returns># of quests you have...</returns>
        public int GetNumQuestLogEntries()
        {
            return isxwow.WoWScript<int>("GetNumQuestLogEntries()", 1);
        }

        /// <summary>
        /// GetNumQuestLogRewards() - Returns the count of rewards for a quest
        /// </summary>
        /// <returns># of rewards</returns>
        public int GetNumQuestChoices()
        {
            return isxwow.WoWScript<int>("GetNumQuestChoices()");
        }

        /// <summary>
        /// IsQuestComplete() - Checks if a quest in your quest log is complete
        /// </summary>
        /// <param name="questID">index of the quest in your quest log</param>
        /// <returns>if the specified quest is complete</returns>
        public bool IsQuestComplete(int questID)
        {
            int complete = isxwow.WoWScript<int>("GetQuestLogTitle(" + questID + ")", 7);
            if (complete == 1)
                return true;
            else
                return false;
        }

        /// <summary>
        /// GetQuestLogName() - Tells you the name of a quest in your quest log
        /// </summary>
        /// <param name="questID">index of the quest in your quest log</param>
        /// <returns>name of the quest</returns>
        public string GetQuestLogName(int questID)
        {
            string name = isxwow.WoWScript<string>("GetQuestLogTitle(" + questID + ")", 1);
            return name;
        }

        /// <summary>
        /// QuestWindowOpen() - Is the quest window (with accept/decline buttons) open?
        /// </summary>
        /// <returns>if the window is open</returns>
        public bool QuestWindowOpen()
        {
            string activeTitle = isxwow.WoWScript<string>("GetTitleText()"); // If the quest is already open, this variable won't be null.
            if (activeTitle == "")
                return false;
            else
                return true;
        }
        #endregion
    }

    class clsQuestObjective
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
            if (description.Contains("slain"))
                return true;
            return false;
        }

        public clsWoWObjective ParseObjective()
        {
            // === Parse Slay Objectives ===
            string objective = description;
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

    }

    class clsMonster
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
}