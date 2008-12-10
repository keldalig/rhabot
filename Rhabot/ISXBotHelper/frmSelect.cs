using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ISXBotHelper
{
    public partial class frmSelect : Form
    {
        private bool IsClosing = false;
        private string SelItem = string.Empty;
        private int SelColumn = 0;

        public frmSelect()
        {
            InitializeComponent();
        }

        private void frmSelect_FormClosing(object sender, FormClosingEventArgs e)
        {
            IsClosing = true;
        }

        /// <summary>
        /// Returns the selected item
        /// </summary>
        /// <param name="Caption">title caption</param>
        /// <param name="ColumnHeaders">headers to display</param>
        /// <param name="ColumnDataList">list of data to display</param>
        /// <param name="ReturnColumn">column number to return on select</param>
        public string ShowForm(string Caption, List<string> ColumnHeaders, List<List<string>> ColumnDataList, int ReturnColumn)
        {
            ListViewItem lvi = null;
            int ColCount = 0;

            try
            {
                // set the caption
                this.Text = Rhabot.clsGlobals.SetFormText(Caption);

                // setup columns
                ColCount = ColumnHeaders.Count;
                this.lstvSelectList.Columns.Clear();
                foreach (string col in ColumnHeaders)
                    this.lstvSelectList.Columns.Add(col, 200);

                // add data
                this.lstvSelectList.Items.Clear();
                foreach (List<string> rowList in ColumnDataList)
                {
                    // create new listview item
                    lvi = new ListViewItem(rowList[0]);

                    // add the sub items
                    for (int i = 1; i < ColCount; i++)
                        lvi.SubItems.Add(rowList[i]);

                    // add to the listview
                    this.lstvSelectList.Items.Add(lvi);
                }

                // set return col
                SelColumn = ReturnColumn;

                // wait until the form is closed or the user selects something
                while ((! IsClosing) && (string.IsNullOrEmpty(SelItem)))
                {
                    System.Threading.Thread.Sleep(200);
                    Application.DoEvents();
                }

                // return the item
                return SelItem;
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Show Select Form");
            }

            return string.Empty;
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                // exit if nothing selected
                if (this.lstvSelectList.SelectedIndices.Count == 0)
                    return;

                // return the selected column
                if (SelColumn > 0)
                    SelItem = this.lstvSelectList.Items[this.lstvSelectList.SelectedIndices[0]].SubItems[SelColumn].Text;
                else
                    SelItem = this.lstvSelectList.Items[this.lstvSelectList.SelectedIndices[0]].Text;
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Select List Double Clicked");
            }
        }
    }
}