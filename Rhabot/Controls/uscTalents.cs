// Code by Undrgrnd59 - Apr 2007

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ISXBotHelper;
using ISXBotHelper.Properties;
using ISXBotHelper.Talents;

namespace Rhabot
{
    public partial class uscTalents : UserControl
    {
        public uscTalents()
        {
            InitializeComponent();
        }

        private void uscTalents_Load(object sender, EventArgs e)
        {
            // exit if in designer
            if (DesignMode)
                return;

            // hook settings changed event
            clsGlobals.SettingsLoaded += clsGlobals_SettingsLoaded;

            // Load talents
            LoadTalents();
        }

        void clsGlobals_SettingsLoaded(object sender, EventArgs e)
        {
            // Load Talents
            LoadTalents();
        }

        /// <summary>
        /// Load the talents
        /// </summary>
        private void LoadTalents()
        {
            try
            {
                // get talents and pop grid accordingly
                List<clsPTalent> talentList = clsTalents.GetSavedTalentList;
                clsPTalent pTalent;

                if ((talentList == null) || (talentList.Count == 0))
                {
                    // pop the grid
                    for (int i = 10; i <= 70; i++)
                    {
                        string[] str = { i.ToString(), string.Empty };
                        sData.Rows.Add(str);
                    }
                }

                else
                {
                    // pop from talent list
                    for (int i = 10; i <= 70; i++)
                    {
                        // find talent
                        pTalent = null;
                        foreach (clsPTalent eTalent in talentList)
                        {
                            if (eTalent.level == i)
                            {
                                pTalent = eTalent;
                                break;
                            }
                        }

                        // if we have a talent, use it
                        string[] strTalent;
                        if (pTalent != null)
                            strTalent = new string[] { pTalent.level.ToString().Trim(), pTalent.name };
                        else
                            strTalent = new string[] { i.ToString(), string.Empty };

                        // add the row
                        sData.Rows.Add(strTalent);
                    }
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "LoadTalents");
            }
        }

        /// <summary>
        /// Display the list of current talents
        /// </summary>
        private void cmdCurrentTalents_Click(object sender, EventArgs e)
        {
            try
            {
                this.lstTalents.Items.Clear();
                foreach (clsTalentInfo tInfo in clsTalents.MyTalents)
                    this.lstTalents.Items.Add(string.Format("{0}\t{1}", tInfo.name, tInfo.currentRank));
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, string.Empty);
            }
        }

        /// <summary>
        /// Display the list of talents available to this class
        /// </summary>
        private void cmdFullList_Click(object sender, EventArgs e)
        {
            try
            {
                this.lstTalents.Items.Clear();
                foreach (clsTalentInfo tInfo in clsTalents.GetTalents())
                    this.lstTalents.Items.Add(string.Format("{0}\t{1}", tInfo.name, tInfo.currentRank));
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, string.Empty);
            }
        }

        /// <summary>
        /// Learn talents
        /// </summary>
        private void cmdApplyTalents_Click(object sender, EventArgs e)
        {
            try
            {
                // set wait cursor
                this.Cursor = Cursors.WaitCursor;

                // save talents
                clsTalents.ApplyTalents();
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Talents");
            }

            finally
            {
                // set cursor
                this.Cursor = Cursors.Default;
            }
        }

        private void cmdSave_Click(object sender, EventArgs e)
        {
            string talent;
            
            try
            {
                // copy the grid to the talents list
                int rows = sData.RowCount;
                clsTalents.ClearTalents();
                for (int i = 0; i < rows; i++)
                {
                    if (string.IsNullOrEmpty((string)sData[1, i].Value))
                        continue;

                    // get the talent text. if we have something, add it
                    talent = sData[1, i].Value.ToString();
                    if (! string.IsNullOrEmpty(talent))
                        clsTalents.AddTalent(new clsPTalent(talent, Convert.ToInt32(sData[0, i].Value.ToString())));
                }

                // save the list
                clsSettings.SaveGlobalSettings();

                // tell user it saved
                MessageBox.Show(Resources.TalentsSaved);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Save Talents");
            }
        }
    }
}
