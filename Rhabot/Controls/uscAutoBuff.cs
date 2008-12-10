using System;
using System.Windows.Forms;
using ISXBotHelper;
using ISXBotHelper.Properties;

namespace Rhabot
{
    public partial class uscAutoBuff : UserControl
    {
        public uscAutoBuff()
        {
            InitializeComponent();
        }

        private void uscAutoBuff_Load(object sender, EventArgs e)
        {
            // hook settings changed event
            clsGlobals.SettingsLoaded += clsGlobals_SettingsLoaded;

            // pop form
            LoadSettings();
        }

        void clsGlobals_SettingsLoaded(object sender, EventArgs e)
        {
            LoadSettings();
        }

        /// <summary>
        /// Load the settings
        /// </summary>
        private void LoadSettings()
        {
            // exit if in designer
            if (DesignMode)
                return;

            this.txtHealSpell.Text = clsSettings.gclsGlobalSettings.AutoBuff_Heal;
            if (clsSettings.gclsGlobalSettings.AutoBuff_HealPercent < 10)
                clsSettings.gclsGlobalSettings.AutoBuff_HealPercent = 40;
            this.txtHealthPercent.Value = clsSettings.gclsGlobalSettings.AutoBuff_HealPercent;

            // add buff list
            this.grdBuffs.Rows.Clear();
            foreach (string buff in clsSettings.gclsGlobalSettings.AutoBuff_BuffList)
            {
                if (!string.IsNullOrEmpty(buff))
                    this.grdBuffs.Rows.Add(new string[] { buff });
            }

            // add an extra blank row to the grid
            this.grdBuffs.Rows.Add();
        }

        /// <summary>
        /// Add a new row
        /// </summary>
        private void grdBuffs_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            this.grdBuffs.Rows.Add();
        }

        /// <summary>
        /// Start/Stop AutoBuff
        /// </summary>
        private void chkAutoBuff_CheckedChanged(object sender, EventArgs e)
        {
            if (this.chkAutoBuff.Checked)
                clsAutoBuff.Start();
            else
                clsAutoBuff.DoShutdown();
        }

        /// <summary>
        /// save settings
        /// </summary>
        private void cmdSave_Click(object sender, EventArgs e)
        {
            try
            {
                // set wait cursor
                this.Cursor = Cursors.WaitCursor;

                clsSettings.gclsGlobalSettings.AutoBuff_Heal = this.txtHealSpell.Text.Trim();
                clsSettings.gclsGlobalSettings.AutoBuff_HealPercent = (int)this.txtHealthPercent.Value;

                // get all buffs from the list
                clsSettings.gclsGlobalSettings.AutoBuff_BuffList.Clear();
                for (int i = 0; i < this.grdBuffs.Rows.Count; i++)
                {
                    // get buff and add it
                    object objBuff = this.grdBuffs.Rows[i].Cells[0].Value;
                    if (objBuff != null)
                    {
                        string buff = objBuff.ToString();
                        if (!string.IsNullOrEmpty(buff))
                            clsSettings.gclsGlobalSettings.AutoBuff_BuffList.Add(buff);
                    }
                }

                // save it
                clsSettings.SaveGlobalSettings();
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.AutoBuff);
            }

            finally
            {
                // set cursor
                this.Cursor = Cursors.Default;
            }
        }
    }
}
