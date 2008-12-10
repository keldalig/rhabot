using System;
using System.Windows.Forms;

using ISXBotHelper;

namespace Rhabot.Controls
{
    public partial class uscHealBot : UserControl
    {
        public uscHealBot()
        {
            InitializeComponent();
        }

        private void uscHealBot_Load(object sender, EventArgs e)
        {
            // hook the settings changed event
            clsGlobals.SettingsLoaded += clsGlobals_SettingsLoaded;

            LoadSettings();
        }

        /// <summary>
        /// Pop the fields
        /// </summary>
        private void LoadSettings()
        {
            this.txtTarget.Text = clsSettings.gclsGlobalSettings.HealBot_Target;
            this.txtRemovePoison.Text = clsSettings.gclsGlobalSettings.HealBot_RemovePoison;
            this.txtRemoveDisease.Text = clsSettings.gclsGlobalSettings.HealBot_RemoveDisease;
            this.txtRemoveCurse.Text = clsSettings.gclsGlobalSettings.HealBot_RemoveCurse;
            this.txtHealSpell.Text = clsSettings.gclsGlobalSettings.HealBot_HealSpell;
            this.txtHealPercent.Value = clsSettings.gclsGlobalSettings.HealBot_HealPercent;
            this.txtBuffList.Text = clsSettings.BuildListString(clsSettings.gclsGlobalSettings.HealBot_BuffList);
        }

        void clsGlobals_SettingsLoaded(object sender, EventArgs e)
        {
            LoadSettings();
        }

        /// <summary>
        /// Save the healbot settings
        /// </summary>
        private void cmdSaveSettings_Click(object sender, EventArgs e)
        {
            try
            {
                // set wait cursor
                this.Cursor = Cursors.WaitCursor;

                // update
                clsSettings.gclsGlobalSettings.HealBot_BuffList = clsSettings.BuildSaveList(this.txtBuffList);
                clsSettings.gclsGlobalSettings.HealBot_HealPercent = (int)this.txtHealPercent.Value;
                clsSettings.gclsGlobalSettings.HealBot_HealSpell = this.txtHealSpell.Text;
                clsSettings.gclsGlobalSettings.HealBot_RemoveCurse = this.txtRemoveCurse.Text;
                clsSettings.gclsGlobalSettings.HealBot_RemoveDisease = this.txtRemoveDisease.Text;
                clsSettings.gclsGlobalSettings.HealBot_RemovePoison = this.txtRemovePoison.Text;
                clsSettings.gclsGlobalSettings.HealBot_Target = this.txtTarget.Text;

                // save
                clsSettings.SaveGlobalSettings();
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "HealBot");
            }

            finally
            {
                // set cursor
                this.Cursor = Cursors.Default;
            }
        }
    }
}
