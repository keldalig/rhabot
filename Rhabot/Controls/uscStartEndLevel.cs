using System;
using System.Windows.Forms;

namespace Rhabot.Controls
{
    public partial class uscStartEndLevel : UserControl
    {
        #region Properties

        public int StartLevel
        {
            get { return (int) this.txtStartLevel.Value; }
        }

        public int EndLevel
        {
            get { return (int) this.txtEndLevel.Value; }
        }

        // Properties
        #endregion

        public uscStartEndLevel()
        {
            InitializeComponent();
            clsGlobals.SettingsLoaded += clsGlobals_SettingsLoaded;
        }

        void clsGlobals_SettingsLoaded(object sender, EventArgs e)
        {
            LoadSettings();
        }

        private void uscStartEndLevel_Load(object sender, EventArgs e)
        {
            LoadSettings();
        }

        private void LoadSettings()
        {
            if (clsGlobals.SettingsLevel > 0)
            {
                this.txtStartLevel.Value = clsGlobals.SettingsLevel;
                this.txtEndLevel.Value = clsGlobals.SettingsLevel;
            }
        }        
    }
}
