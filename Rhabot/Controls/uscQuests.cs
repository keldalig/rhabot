using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ISXBotHelper.Controls
{
    public partial class uscQuests : UserControl
    {
        private readonly Dictionary<string, List<clsQuest.clsQuestObjective>> QuestDict = new Dictionary<string, List<clsQuest.clsQuestObjective>>();

        public uscQuests()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Form loading
        /// </summary>
        private void uscQuests_Load(object sender, EventArgs e)
        {
            PopQuestList();
        }

        /// <summary>
        /// Refresh the quest list
        /// </summary>
        private void cmdRefresh_Click(object sender, EventArgs e)
        {
            PopQuestList();
        }

        /// <summary>
        /// Pop the quest list
        /// </summary>
        private void PopQuestList()
        {
            try
            {
                // clear dictionary
                QuestDict.Clear();
                this.grdvQuests.Rows.Clear();

                // get the number of quests
                int NumQuests = clsQuest.GetNumQuestLogEntries();

                // loop through all the quests
                for (int i = 1; i < NumQuests; i++)
                {
                    // get num objectives
                    int NumObjectives = clsQuest.GetNumQuestObjectives(i);

                    // skip if completed or no objectives
                    if ((clsQuest.IsQuestComplete(i)) ||
                        (NumObjectives == 0))
                        continue;

                    // add the row
                    string QuestName = clsQuest.GetQuestLogName(i);
                    this.grdvQuests.Rows.Add(false, QuestName);

                    // add to the dictionary
                    QuestDict.Add(QuestName, new List<clsQuest.clsQuestObjective>());
                    for (int j = 1; j < NumObjectives; j++)
                        QuestDict[QuestName].Add(clsQuest.GetQuestObjective(j, i));
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "PopQuestList");
            }
        }

        /// <summary>
        /// Set these quests to be worked
        /// </summary>
        private void cmdDoTheseQuests_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                // clear quest list
                clsQuest.AutoNavQuestList.Clear();

                // loop through the table rows
                int RowCount = this.grdvQuests.Rows.Count;
                for (int i = 0; i < RowCount; i++)
                {
                    // skip if not checked
                    if ((bool)this.grdvQuests[0, i].Value)
                        continue;

                    // get the quest name
                    string QuestName = this.grdvQuests[1, i].Value as string;

                    // copy the objectives
                    foreach (clsQuest.clsQuestObjective qObjective in QuestDict[QuestName])
                        clsQuest.AutoNavQuestList.Add(qObjective);
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "DoTheseQuests");
            }

            finally
            {
                this.Cursor = Cursors.Default;
            }
        }
    }
}
