using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ISXBotHelper;
using ISXBotHelper.Settings.Settings;

namespace Rhabot.Controls
{
    public partial class uscItemMain : UserControl
    {
        #region Variables

        private delegate void dUpdateBagListHander();

        // Variables
        #endregion

        #region Load

        public uscItemMain()
        {
            InitializeComponent();
        }

        private void uscItemMain_Load(object sender, EventArgs e)
        {
            // exit if in designer
            if (DesignMode)
                return;

            // set the mule name
            this.txtMule.Text = clsSettings.gclsGlobalSettings.Character_MailTo;

            // hook the settings changed event
            clsGlobals.SettingsLoaded += clsGlobals_SettingsLoaded;

            // pop the controls
            PopControls(uscItemList.EItemListType.Mail);
            PopControls(uscItemList.EItemListType.Delete);
            PopControls(uscItemList.EItemListType.Keep);
            PopControls(uscItemList.EItemListType.Disenchant);
            PopControls(uscItemList.EItemListType.Sell);
            PopControls(uscItemList.EItemListType.Open);

            // hook the changed events
            this.uscItemList1.ListChanged += ListChanged;
            this.uscItemList2.ListChanged += ListChanged;
            this.uscItemList3.ListChanged += ListChanged;
            this.uscItemList4.ListChanged += ListChanged;
            this.uscItemList5.ListChanged += ListChanged;
            this.uscItemList6.ListChanged += ListChanged;

            // hook the check change events
            this.uscItemList1.CheckChanged += CheckChanged;
            this.uscItemList2.CheckChanged += CheckChanged;
            this.uscItemList3.CheckChanged += CheckChanged;
            this.uscItemList4.CheckChanged += CheckChanged;
            this.uscItemList5.CheckChanged += CheckChanged;
            this.uscItemList6.CheckChanged += CheckChanged;
        }

        /// <summary>
        /// New settings were loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void clsGlobals_SettingsLoaded(object sender, EventArgs e)
        {
            // pop the controls
            PopControls(uscItemList.EItemListType.Mail);
            PopControls(uscItemList.EItemListType.Delete);
            PopControls(uscItemList.EItemListType.Keep);
            PopControls(uscItemList.EItemListType.Disenchant);
            PopControls(uscItemList.EItemListType.Sell);
            PopControls(uscItemList.EItemListType.Open);
        }

        // Load
        #endregion

        #region Control Events

        /// <summary>
        /// A checkbox was checked/unchecked
        /// </summary>
        /// <param name="Checked">true if the checkbox is now checked</param>
        /// <param name="ColorValue">the color tag of the checkbox</param>
        private void CheckChanged(uscItemList.EItemListType ItemType, bool Checked, string ColorValue)
        {
            // update the check value
            switch (ItemType)
            {
                case uscItemList.EItemListType.Mail:
                    UpdateColoredItem(Checked, ColorValue, clsSettings.gclsGlobalSettings.ItemMailColors);
                    break;
                case uscItemList.EItemListType.Sell:
                    UpdateColoredItem(Checked, ColorValue, clsSettings.gclsGlobalSettings.ItemSellColors);
                    break;
                case uscItemList.EItemListType.Delete:
                    UpdateColoredItem(Checked, ColorValue, clsSettings.gclsGlobalSettings.ItemJunkColors);
                    break;
                case uscItemList.EItemListType.Disenchant:
                    UpdateColoredItem(Checked, ColorValue, clsSettings.gclsGlobalSettings.ItemDisenchantColors);
                    break;

                // these two don't have any checkboxes
                case uscItemList.EItemListType.Keep:
                case uscItemList.EItemListType.Open:
                    break;
            }
        }

        private void UpdateColoredItem(bool Checked, string ColorValue, clsGlobalSettings.clsItemColorInfo ColorInfo)
        {
            switch (ColorValue)
            {
                case clsGlobalSettings.clsItemColorInfo.BlueFilter:
                    ColorInfo.Blue = Checked;
                    break;

                case clsGlobalSettings.clsItemColorInfo.GreenFilter:
                    ColorInfo.Green = Checked;
                    break;

                case clsGlobalSettings.clsItemColorInfo.GreyFilter:
                    ColorInfo.Grey = Checked;
                    break;

                case clsGlobalSettings.clsItemColorInfo.PurpleFilter:
                    ColorInfo.Purple = Checked;
                    break;

                case clsGlobalSettings.clsItemColorInfo.WhiteFilter:
                    ColorInfo.White = Checked;
                    break;
            }
        }

        /// <summary>
        /// One of the sub controls changed, we need to re-load it's data
        /// </summary>
        /// <param name="ItemType">0 = mail, 1 = sell, 2 = delete</param>
        private void ListChanged(uscItemList.EItemListType ItemType)
        {
            RepopList(ItemType);
            PopControls(ItemType);

            PopBagList(this.uscItemList1);
            PopBagList(this.uscItemList2);
            PopBagList(this.uscItemList3);
            PopBagList(this.uscItemList4);
            PopBagList(this.uscItemList5);
            PopBagList(this.uscItemList6);
        }

        private void cmdSave_Click(object sender, EventArgs e)
        {
            // save mule name
            clsSettings.gclsGlobalSettings.Character_MailTo = this.txtMule.Text;
            
            // repop the lists
            RepopList(uscItemList.EItemListType.Delete);
            RepopList(uscItemList.EItemListType.Disenchant);
            RepopList(uscItemList.EItemListType.Keep);
            RepopList(uscItemList.EItemListType.Mail);
            RepopList(uscItemList.EItemListType.Sell);
            RepopList(uscItemList.EItemListType.Open);

            // save lists
            clsSettings.SaveGlobalSettings();
        }

        // Control Events
        #endregion

        #region Pop Controls / Lists

        /// <summary>
        /// Pops one of the sub controls
        /// </summary>
        private void PopControls(uscItemList.EItemListType ItemType)
        {
            switch (ItemType)
            {
                case uscItemList.EItemListType.Mail:
                    PopControlsEx(this.uscItemList1, clsSettings.gclsGlobalSettings.ItemMailList, clsSettings.gclsGlobalSettings.ItemMailColors);
                    break;
                case uscItemList.EItemListType.Sell:
                    PopControlsEx(this.uscItemList2, clsSettings.gclsGlobalSettings.ItemSellList, clsSettings.gclsGlobalSettings.ItemSellColors);
                    break;
                case uscItemList.EItemListType.Delete:
                    PopControlsEx(this.uscItemList3, clsSettings.gclsGlobalSettings.ItemJunkList, clsSettings.gclsGlobalSettings.ItemJunkColors);
                    break;
                case uscItemList.EItemListType.Disenchant:
                    PopControlsEx(this.uscItemList4, clsSettings.gclsGlobalSettings.ItemDisenchantList, clsSettings.gclsGlobalSettings.ItemDisenchantColors);
                    break;
                case uscItemList.EItemListType.Keep:
                    PopControlsEx(this.uscItemList5, clsSettings.gclsGlobalSettings.ItemKeepList, null);
                    break;
                case uscItemList.EItemListType.Open:
                    PopControlsEx(this.uscItemList6, clsSettings.gclsGlobalSettings.ItemOpenList, null);
                    break;
            }
        }

        /// <summary>
        /// Pops the controls on the uscItem subcontrol
        /// </summary>
        private void PopControlsEx(uscItemList uscItem, List<string> ItemList, clsGlobalSettings.clsItemColorInfo ItemColorInfo)
        {
            try
            {
                // clear the listboxes
                uscItem.lstItemList.Items.Clear();

                // set checkbox values
                if (ItemColorInfo != null)
                {
                    uscItem.chkBlues.Checked = ItemColorInfo.Blue;
                    uscItem.chkGreens.Checked = ItemColorInfo.Green;
                    uscItem.chkGreys.Checked = ItemColorInfo.Grey;
                    uscItem.chkPurple.Checked = ItemColorInfo.Purple;
                    uscItem.chkWhite.Checked = ItemColorInfo.White;
                }

                // pop the item listbox
                foreach (string item in ItemList)
                    uscItem.lstItemList.Items.Add(item);

                // pop the bag list
                PopBagList(uscItem);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, string.Empty);
            }
            
        }

        /// <summary>
        /// Pops the bag list
        /// </summary>
        private void PopBagList(uscItemList uscItem)
        {
            try
            {
                // invoke if required
                if (uscItem.lstBagList.InvokeRequired)
                    uscItem.lstBagList.Invoke(new dUpdateBagListHander(uscItem.PopBagList));
                else
                    uscItem.PopBagList();

                // wait until the list is unlocked
                do
                {
                    System.Threading.Thread.Sleep(300);
                    Application.DoEvents();
                } while (uscItem.ListLocked);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, string.Empty);
            }
            
        }

        /// <summary>
        /// Repops this list from the sub control's listbox
        /// </summary>
        /// <param name="ListIndex">0 = mail, 1 = sell, 2 = delete</param>
        private void RepopList(uscItemList.EItemListType ListIndex)
        {
            try
            {
                // update the saved lists
                switch (ListIndex)
                {
                    case uscItemList.EItemListType.Mail:
                        PopList(clsSettings.gclsGlobalSettings.ItemMailList, clsSettings.gclsGlobalSettings.ItemMailColors, this.uscItemList1);
                        break;
                    case uscItemList.EItemListType.Sell:
                        PopList(clsSettings.gclsGlobalSettings.ItemSellList, clsSettings.gclsGlobalSettings.ItemSellColors, this.uscItemList2);
                        break;
                    case uscItemList.EItemListType.Delete:
                        PopList(clsSettings.gclsGlobalSettings.ItemJunkList, clsSettings.gclsGlobalSettings.ItemJunkColors, this.uscItemList3);
                        break;
                    case uscItemList.EItemListType.Disenchant:
                        PopList(clsSettings.gclsGlobalSettings.ItemDisenchantList, clsSettings.gclsGlobalSettings.ItemDisenchantColors, this.uscItemList4);
                        break;
                    case uscItemList.EItemListType.Keep:
                        PopList(clsSettings.gclsGlobalSettings.ItemKeepList, null, this.uscItemList5);
                        break;
                    case uscItemList.EItemListType.Open:
                        PopList(clsSettings.gclsGlobalSettings.ItemOpenList, null, this.uscItemList6);
                        break;
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, string.Empty);
            }            
        }

        /// <summary>
        /// Updates the list from the sub control
        /// </summary>
        /// <param name="ItemList">the list of items</param>
        /// <param name="ItemColorInfo">the color info</param>
        /// <param name="uscItem">the sub control</param>
        private void PopList(List<string> ItemList, clsGlobalSettings.clsItemColorInfo ItemColorInfo, uscItemList uscItem)
        {
            try
            {
                // clear the item list
                ItemList.Clear();

                // pop the item list from the listbox
                foreach (object objItem in uscItem.lstItemList.Items)
                {
                    string itemStr = (string)objItem;
                    if (!ItemList.Contains(itemStr))
                        ItemList.Add(itemStr);
                }

                // pop colors
                if (ItemColorInfo != null)
                {
                    ItemColorInfo.Blue = uscItem.chkBlues.Checked;
                    ItemColorInfo.Green = uscItem.chkGreens.Checked;
                    ItemColorInfo.Grey = uscItem.chkGreys.Checked;
                    ItemColorInfo.Purple = uscItem.chkPurple.Checked;
                    ItemColorInfo.White = uscItem.chkWhite.Checked;
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, string.Empty);
            }
        }

        // Pop Controls / Lists
        #endregion
    }
}
