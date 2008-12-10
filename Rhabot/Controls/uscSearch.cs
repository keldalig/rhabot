using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ISXBotHelper;
using ISXBotHelper.Properties;
using ISXWoW;

namespace Rhabot.Controls
{
    public partial class uscSearch : UserControl
    {
        public uscSearch()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Search
        /// </summary>
        private void cmdSearch_Click(object sender, EventArgs e)
        {
            // exit if no search string
            if (string.IsNullOrEmpty(this.txtSearch.Text))
                return;

            GuidList gl;

            try
            {
                // clear the list
                this.lstItemList.Items.Clear();

                // do the search
                gl = GuidList.New(Regex.Split(this.txtSearch.Text, ","));

                // if nothing, then exit
                if ((gl == null) || (gl.Count == 0))
                {
                    clsError.ShowError(new Exception(Resources.Searchreturnednoresults), Resources.Search, string.Empty, false, new StackFrame(0, true), false);
                    return;
                }

                // loop through and pop the list
                this.lstItemList.SuspendLayout();
                int glCount = (int)gl.Count;
                for (int i = 0; i < glCount; i++)
                    this.lstItemList.Items.Add(new clsSearchItemInfo(gl.Object((uint)i)));
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.Search, true, new StackFrame(0, true), false);
            }
            
            finally
            {
                this.lstItemList.ResumeLayout(true);
            }
        }

        class clsSearchItemInfo
        {
            public readonly WoWObject Item;

            public clsSearchItemInfo(WoWObject item)
            {
                Item = item;
            }

            public override string ToString()
            {
                return Item.Name;
            }
        }

        #region Button Clicks

        private void cmdTarget_Click(object sender, EventArgs e)
        {
            try
            {
                // exit if nothing selected
                int index = this.lstItemList.SelectedIndex;
                if (index < 0)
                {
                    clsError.ShowError(new Exception(Resources.selectanitem_), Resources.Search, string.Empty, false, new StackFrame(0, true), false);
                    return;
                }

                // target the unit
                ((clsSearchItemInfo)this.lstItemList.Items[index]).Item.GetUnit().Target();
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Target", true, new StackFrame(0, true), false);
            }
        }

        private void cmdPickup_Click(object sender, EventArgs e)
        {
            try
            {
                // exit if nothing selected
                int index = this.lstItemList.SelectedIndex;
                if (index < 0)
                {
                    clsError.ShowError(new Exception(Resources.selectanitem_), Resources.Search, string.Empty, false, new StackFrame(0, true), false);
                    return;
                }

                // pick up the item
                ((clsSearchItemInfo)this.lstItemList.Items[index]).Item.GetItem().PickUp();
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Pickup", true, new StackFrame(0, true), false);
            }
        }

        private void cmdUse_Click(object sender, EventArgs e)
        {
            try
            {
                // exit if nothing selected
                int index = this.lstItemList.SelectedIndex;
                if (index < 0)
                {
                    clsError.ShowError(new Exception(Resources.selectanitem_), Resources.Search, string.Empty, false, new StackFrame(0, true), false);
                    return;
                }

                // use the item
                ((clsSearchItemInfo)this.lstItemList.Items[index]).Item.GetItem().Use();
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Use Item", true, new StackFrame(0, true), false);
            }
        }

        // Button Clicks
        #endregion
    }
}
