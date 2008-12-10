using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using ISXBotHelper;
using ISXBotHelper.Chatter;
using ISXBotHelper.Properties;

namespace Rhabot.Controls
{
    public partial class uscChatter : UserControl
    {
        #region Variables

        List<clsChatterRuleInfo> RuleInfoList = new List<clsChatterRuleInfo>();

        // Variables
        #endregion

        #region Load

        public uscChatter()
        {
            InitializeComponent();
        }

        private void uscChatter_Load(object sender, EventArgs e)
        {
            // exit if in designer
            if (DesignMode)
                return;

            // set the open/save file dialog paths
            this.openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            this.saveFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            this.openFileDialog1.Filter = Resources.ChatterKBFile;
            this.saveFileDialog1.Filter = Resources.ChatterKBFile;
            this.openFileDialog1.FilterIndex = 1;
            this.saveFileDialog1.FilterIndex = 1;
            this.openFileDialog1.CheckFileExists = true;
            this.saveFileDialog1.CheckPathExists = true;

            // build the how to string
            this.txtChatHowTo.Text = Resources.ChatterInstructions;

            // hook the settings loaded event
            clsGlobals.SettingsLoaded += clsGlobals_SettingsLoaded;
            LoadSettings();
        }

        void clsGlobals_SettingsLoaded(object sender, EventArgs e)
        {
            LoadSettings();
        }

        /// <summary>
        /// re-load the chatter settings
        /// </summary>
        private void LoadSettings()
        {
            try
            {
                // clear everything
                this.trvSaveInput.Nodes.Clear();
                this.txtInputMessages.Text = string.Empty;
                this.grdOutputs.Rows.Clear();
                this.grdOutputs.Rows.Add();

                // load the knowledge base file
                if (!string.IsNullOrEmpty(this.lblCurrentFile.Text.Trim()))
                    clsSettings.chatter.ChatterKBFile = this.lblCurrentFile.Text;
                else
                    clsSettings.chatter.ChatterKBFile = clsGlobals.LastChatterFile;

                // exit if no file or file does not exist
                if ((string.IsNullOrEmpty(clsSettings.chatter.ChatterKBFile)) || (!File.Exists(clsSettings.chatter.ChatterKBFile)))
                    return;

                // get the chatter rules list
                RuleInfoList = clsSettings.chatter.GetChatterRules();

                // pop the treeview
                foreach (clsChatterRuleInfo rInfo in RuleInfoList)
                    AddNode(rInfo);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.Chatter);
            }
            
        }

        /// <summary>
        /// Adds a node to the treeview
        /// </summary>
        /// <param name="rInfo"></param>
        private void AddNode(clsChatterRuleInfo rInfo)
        {
            TreeNode node = new TreeNode(rInfo.InputMessage);

            // add the output messages
            foreach (string output in rInfo.OutputMessages)
                node.Nodes.Add(output);

            // add to the tree
            this.trvSaveInput.Nodes.Add(node);
            this.trvSaveInput.ExpandAll();
        }

        // Load
        #endregion

        /// <summary>
        /// Add input/outputs to the treeview and rinfo list
        /// </summary>
        private void cmdSaveMessage_Click(object sender, EventArgs e)
        {
            clsChatterRuleInfo rInfo;

            try
            {
                // exit if no input/outputs
                if ((string.IsNullOrEmpty(this.txtInputMessages.Text.Trim())) || (this.grdOutputs.Rows.Count <= 0))
                {
                    clsError.ShowError(new Exception(Resources.YouMustEnterInputMsg), Resources.Chatter, string.Empty, true, new StackFrame(0, true), false);
                    return;
                }

                // init rule info
                rInfo = new clsChatterRuleInfo(this.txtInputMessages.Text.Trim());

                // add the outputs
                for (int i = 0; i < this.grdOutputs.Rows.Count-1; i++)
                {
                    string outStr = this.grdOutputs[0, i].Value.ToString();
                    if (! string.IsNullOrEmpty(outStr))
                        rInfo.OutputMessages.Add(outStr);
                }

                // exit if no outputs
                if (rInfo.OutputMessages.Count == 0)
                {
                    MessageBox.Show(Resources.YouMustEnterOutputMsg);
                    return;
                }

                // add to the save list
                RuleInfoList.Add(rInfo);

                // add to the treeview
                AddNode(rInfo);

                // clear fields
                this.txtInputMessages.Text = string.Empty;
                this.grdOutputs.Rows.Clear();
                this.grdOutputs.Rows.Add();
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.Chatter);
            }            
        }

        /// <summary>
        /// build the knowledge base
        /// </summary>
        private void cmdBuild_Click(object sender, EventArgs e)
        {
            try
            {
                // ask where to save it
                if (saveFileDialog1.ShowDialog() != DialogResult.OK)
                    return;

                string filename = this.saveFileDialog1.FileName;

                // if the file exists, delete it
                if (File.Exists(filename))
                    File.Delete(filename);

                // set this as the kb file for chatter
                clsSettings.chatter.ChatterKBFile = filename;

                // compile kb, save it, then reload it
                if (clsSettings.chatter.BuildNewKnowledgeBase(RuleInfoList))
                    this.lblCurrentFile.Text = filename;
                else
                    this.lblCurrentFile.Text = string.Empty;
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.Chatter, Resources.ChatterBuildKB);
            }
        }

        private void grdOutputs_DoubleClick(object sender, EventArgs e)
        {
            // add a new row
            //this.grdOutputs.Rows.Add();
        }

        private void cmdLoad_Click(object sender, EventArgs e)
        {
            try
            {
                // show the open file dialog
                if (this.openFileDialog1.ShowDialog() != DialogResult.OK)
                    return;

                // exit if no file
                if (string.IsNullOrEmpty(this.openFileDialog1.FileName))
                    return;

                // set the label
                this.lblCurrentFile.Text = this.openFileDialog1.FileName;

                // load the file
                LoadSettings();
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.Chatter);
            }            
        }
    }
}
