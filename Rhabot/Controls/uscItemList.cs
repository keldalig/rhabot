using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using ISXBotHelper;
using ISXBotHelper.Properties;
using ISXWoW;
using ISXBotHelper.Settings.Settings;

namespace Rhabot.Controls
{
    public partial class uscItemList : UserControl
    {
        public uscItemList()
        {
            InitializeComponent();
        }

        #region Enum

        public enum EItemListType
        {
            Mail = 0,
            Sell = 1,
            Delete = 2,
            Disenchant = 3,
            Keep = 4,
            Open = 5
        }

        // Enum
        #endregion

        #region Variables

        public bool ListLocked = false;

        // Variables
        #endregion

        #region Events

        public delegate void ListChangedHandler(EItemListType ItemType);
        public event ListChangedHandler ListChanged;

        public delegate void CheckChangedHandler(EItemListType ItemType, bool Checked, string ColorValue);
        public event CheckChangedHandler CheckChanged;

        // Events
        #endregion

        #region Properties

        private string ItemTypeStr = string.Empty;
        private EItemListType m_ItemListType = EItemListType.Mail;
        /// <summary>
        /// Type of item list to display
        /// </summary>
        public EItemListType ItemListType
        {
            get { return m_ItemListType; }
            set 
            { 
                m_ItemListType = value;
                string CheckString = string.Empty;

                this.chkBlues.Visible = true;
                this.chkGreens.Visible = true;
                this.chkPurple.Visible = true;
                this.chkGreys.Visible = true;
                this.chkWhite.Visible = true;

                // change display properties
                switch (m_ItemListType)
                {
                    case EItemListType.Mail:
                        this.lblMessage.Text = Resources.ItemsToMailToMule;
                        this.lblItemList.Text = Resources.ListOfItemToMail;
                        CheckString = Resources.MailAll;
                        ItemTypeStr = Resources.Mail;
                        break;
                    case EItemListType.Sell:
                        this.lblMessage.Text = Resources.SellAtVendors;
                        this.lblItemList.Text = string.Format("{0}{1}", Resources.ListofItemsTo, Resources.SellAtVendors);
                        CheckString = Resources.SellAll;
                        ItemTypeStr = Resources.Sell;
                        break;
                    case EItemListType.Delete:
                        this.lblMessage.Text = Resources.Delete;
                        this.lblItemList.Text = string.Format("{0}{1}", Resources.ListofItemsTo, Resources.Delete);
                        ItemTypeStr = Resources.Delete;
                        CheckString = Resources.DeleteAll;
                        break;
                    case EItemListType.Disenchant:
                        this.lblMessage.Text = Resources.Disenchant;
                        this.lblItemList.Text = string.Format("{0}{1}", Resources.ListofItemsTo, Resources.Disenchant);                        
                        this.chkGreys.Visible = false;
                        this.chkWhite.Visible = false;
                        ItemTypeStr = Resources.Disenchant;
                        CheckString = Resources.DisenchantAll;
                        break;
                    case EItemListType.Keep:
                        this.lblMessage.Text = Resources.Keep;
                        this.lblItemList.Text = string.Format("{0}{1}", Resources.ListofItemsTo, Resources.Keep); 
                        ItemTypeStr = Resources.Keep;
                        CheckString = string.Empty;
                        break;
                    case EItemListType.Open:
                        this.lblMessage.Text = Resources.Open;
                        this.lblItemList.Text = string.Format("{0}{1}", Resources.ListofItemsTo, Resources.Open); 
                        ItemTypeStr = Resources.Open;
                        CheckString = string.Empty; 
                        break;
                }

                // set check text
                if (!string.IsNullOrEmpty(CheckString))
                {
                    this.chkBlues.Text = string.Format("{0}{1}", CheckString, Resources.Blues);
                    this.chkGreens.Text = string.Format("{0}{1}", CheckString, Resources.Greens);
                    this.chkGreys.Text = string.Format("{0}{1}", CheckString, Resources.Greys);
                    this.chkPurple.Text = string.Format("{0}{1}", CheckString, Resources.Purples);
                    this.chkWhite.Text = string.Format("{0}{1}", CheckString, Resources.Whites);

                    // set tags
                    this.chkBlues.Tag = clsGlobalSettings.clsItemColorInfo.BlueFilter;
                    this.chkGreens.Tag = clsGlobalSettings.clsItemColorInfo.GreenFilter;
                    this.chkGreys.Tag = clsGlobalSettings.clsItemColorInfo.GreyFilter;
                    this.chkPurple.Tag = clsGlobalSettings.clsItemColorInfo.PurpleFilter;
                    this.chkWhite.Tag = clsGlobalSettings.clsItemColorInfo.WhiteFilter;
                }
            }
        }

        // Properties
        #endregion

        #region Button Clicks

        /// <summary>
        /// move item from backpack to the item list
        /// </summary>
        private void cmdMove_Click(object sender, EventArgs e)
        {
            string item;

            try
            {
                // exit if nothing selected
                if (this.lstBagList.SelectedIndices.Count == 0)
                    return;

                // lock buttons until we finish
                this.cmdMove.Enabled = false;
                this.cmdRefresh.Enabled = false;
                this.cmdRemove.Enabled = false;
                this.lstItemList.SuspendLayout();

                // loop through all selected indices
                foreach (int index in this.lstBagList.SelectedIndices)
                {
                    // move the item
                    item = this.lstBagList.Items[index].ToString();
                    if (this.lstItemList.Items.Contains(item))
                        continue;

                    // add the item
                    this.lstItemList.Items.Add(item);
                    clsSettings.Logging.AddToLogFormatted(Resources.PopBagList, Resources.AddItemToListXY, ItemTypeStr, item);
                }

                // raise the event
                if (ListChanged != null)
                    ListChanged(m_ItemListType);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, string.Empty);
            }

            finally
            {
                // unlock buttons after we finish
                this.cmdMove.Enabled = true;
                this.cmdRefresh.Enabled = true;
                this.cmdRemove.Enabled = true;
                this.lstItemList.ResumeLayout(true);
            }
        }

        /// <summary>
        /// Remove the item from the list
        /// </summary>
        private void cmdRemove_Click(object sender, EventArgs e)
        {
            try
            {
                int index = this.lstItemList.SelectedIndex;
                string item;

                // exit if nothing selected
                if (index < 0)
                    return;

                // move the item
                item = this.lstItemList.Items[index].ToString();
                this.lstItemList.Items.RemoveAt(index);
                clsSettings.Logging.AddToLogFormatted(Resources.PopBagList, Resources.RemovingItemFromListXY, ItemTypeStr, item);

                // raise the event
                if (ListChanged != null)
                    ListChanged(m_ItemListType);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.PopBagList);
            }            
        }

        private void CheckedChanged(object sender, EventArgs e)
        {
            // get the box
            CheckBox cb = (CheckBox) sender;

            // raise the event
            if (CheckChanged != null)
                CheckChanged(ItemListType, cb.Checked, cb.Tag as string);
        }

        private void cmdRefresh_Click(object sender, EventArgs e)
        {
            PopBagList();
        }

        // Button Clicks
        #endregion

        #region Functions

        /// <summary>
        /// Pops the bag list
        /// </summary>
        public void PopBagList()
        {
            List<string> wItemList = new List<string>();
            try
            {
                // exit if shutting down or character is invalid
                if ((clsSettings.IsShuttingDown) || (! clsCharacter.CharacterIsValid))
                    return;

                // pause while locked
                while (ListLocked)
                    Thread.Sleep(300);

                // lock the list
                ListLocked = true;

                // clear the bag list
                this.lstBagList.Items.Clear();

                // get inventory list
                List<WoWItem> wBagList = clsCharacter.GetBagItems();

                // add items to wItemList, so we don't have dupes
                using (new clsFrameLock.LockBuffer())
                {
                    foreach (WoWItem wItem in wBagList)
                    {
                        // only add if the fullname does not already exist
                        if (!wItemList.Contains(wItem.FullName))
                            wItemList.Add(wItem.FullName);
                    }
                }

                // if no list returned, clear listbox and exit
                if (wItemList.Count == 0)
                    return;

                try
                {
                    // lock listbox so we can update it
                    this.cmdRemove.Enabled = false;
                    this.cmdRefresh.Enabled = false;
                    this.cmdMove.Enabled = false;
                    this.lstBagList.SuspendLayout();

                    // clear the list
                    this.lstBagList.Items.Clear();

                    // loop through and add items that are not in mail/sell/delete/etc lists
                    foreach (string name in wItemList)
                    {
                        // add to bag listbox if the item is not in any of the lists
                        if ((!clsSettings.gclsGlobalSettings.ItemMailList.Contains(name)) &&
                            (!clsSettings.gclsGlobalSettings.ItemSellList.Contains(name)) &&
                            (!clsSettings.gclsGlobalSettings.ItemJunkList.Contains(name)) &&
                            (!clsSettings.gclsGlobalSettings.ItemDisenchantList.Contains(name)) &&
                            (!clsSettings.gclsGlobalSettings.ItemKeepList.Contains(name)) &&
                            (!clsSettings.gclsGlobalSettings.ItemOpenList.Contains(name)))
                                this.lstBagList.Items.Add(name);
                    }
                }
                catch (Exception excep2)
                {
                    clsError.ShowError(excep2, Resources.PopBagList);
                }
                finally
                {
                    // unlock listbox
                    this.lstBagList.ResumeLayout(true);
                    this.cmdRemove.Enabled = true;
                    this.cmdRefresh.Enabled = true;
                    this.cmdMove.Enabled = true;
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.PopBagList);
            }

            finally
            {
                // release the list lock
                ListLocked = false;
            }
        }

        // Functions
        #endregion
    }
}
