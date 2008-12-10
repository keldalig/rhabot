using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ISXBotHelper.Items;
using ISXBotHelper.Properties;
using ISXWoW;
using ISXWoW.WoWUI.Frames;
using ISXBotHelper.Settings.Settings;

namespace ISXBotHelper
{
    /// <summary>
    /// Functions for working with vendors
    /// </summary>
    public static class clsVendor
    {
        /// <summary>
        /// Returns the the nearest vendor, or null if none nearby
        /// </summary>
        /// <param name="CanRepair">true to find vendors who can repair</param>
        public static WoWUnit GetNearestVendor(bool CanRepair)
        {
            StringBuilder sb = new StringBuilder();
            List<WoWUnit> vendorList;

            try
            {
                // build search string
                sb.Append("-units,-merchant,-nearest");
                if (CanRepair)
                    sb.Append(",-repair");

                // find vendor
                vendorList = clsSearch.Search(sb.ToString());

                // if no vendors, then exit null
                if ((vendorList == null) || (vendorList.Count == 0))
                    return null;

                // return the first vendor
                return vendorList[0];
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Searching for nearest vendor");
            }

            // nothing found
            return null;
        }

        /// <summary>
        /// Sells to the selected vendor. Moves to vendor if you are too far away
        /// </summary>
        /// <param name="Vendor">the vendor to use</param>
        public static clsPath.EMovementResult SellToVendor(WoWUnit Vendor)
        {
            PathListInfo.PathPoint vendorPoint, CurrentPoint;
            clsPath cPath = new clsPath();
            clsPath.EMovementResult eResult;
            WoWItem tempItem;
            WoWContainer MyBag;

            try
            {
                // exit if no vendor
                if ((Vendor == null) || (!Vendor.IsValid))
                    return clsPath.EMovementResult.Error; // no vendor

                // get the vendor point
                vendorPoint = clsPath.GetUnitLocation(Vendor);
                CurrentPoint = clsCharacter.MyLocation;
//                CanGossip = Vendor.CanGossip;
                bool CanRepair = Vendor.CanRepair; // CanGossip = false

                // log it
                clsSettings.Logging.AddToLogFormatted(Resources.MovingFromXToVendor_,
                    CurrentPoint.ToString(), Vendor.Name, vendorPoint.ToString());

                // move if we are too far away
                if (CurrentPoint.Distance(vendorPoint) > 2)
                {
                    // move to the vendor
                    eResult = cPath.MoveToPoint(vendorPoint);

                    // if not success, then exit
                    if (eResult != clsPath.EMovementResult.Success)
                    {
                        clsSettings.Logging.AddToLogFormatted(Resources.SellToVendorExitingX, eResult.ToString());
                        return eResult;
                    }
                }

                // open lockboxes
                if (clsSettings.gclsLevelSettings.IsRogue)
                    OpenLockboxes();

                // open items
                OpenItems();

                // delete junk items
                DeleteJunkItems();

                // disenchant items
                DisenchantItems();

                // open the vendor dialog
                if (! SpeakWithVendor(Vendor))
                {
                    clsSettings.Logging.AddToLog(Resources.CouldNotOpenVendorDialog);
                    return clsPath.EMovementResult.Error;
                }

                #region Sell

                // loop through all 5 bags and sell their items
                int i;
                for (i = 0; i < 6; i++)
                {
                    int SlotCount;
                    using (new clsFrameLock.LockBuffer())
                    {
                        // get the bag
                        MyBag = clsSettings.isxwow.Me.Bag(i).GetContainer();

                        // skip if invalid
                        if ((MyBag == null) || (! MyBag.IsValid))
                            continue;

                        // log it
                        clsSettings.Logging.AddToLogFormatted("SellToVendor", Resources.GettingItemsFromBag,i, MyBag.Name);

                        // get the number of slots for this bag
                        SlotCount = MyBag.SlotCount;
                    }

                    // loop through each slot and see if the item can be sold
                    int itemCounter;
                    for (itemCounter = 0; itemCounter < SlotCount; itemCounter++)
                    {
                        // get the item
                        using (new clsFrameLock.LockBuffer())
                            tempItem = MyBag.Item(itemCounter).GetItem();

                        // if null, skip
                        if ((tempItem == null) || (!tempItem.IsValid))
                            continue;

                        // if a colored item that we can sell or in the sell list, sell it
                        if (((clsSettings.gclsGlobalSettings.ItemSellColors.Grey) && (tempItem.Rarity == 0)) ||
                            ((clsSettings.gclsGlobalSettings.ItemSellColors.White) && (tempItem.Rarity == 1)) ||
                            ((clsSettings.gclsGlobalSettings.ItemSellColors.Green) && (tempItem.Rarity == 2)) ||
                            ((clsSettings.gclsGlobalSettings.ItemSellColors.Blue) && (tempItem.Rarity == 3)) ||
                            ((clsSettings.gclsGlobalSettings.ItemSellColors.Purple) && (tempItem.Rarity == 4)) ||
                            clsSettings.gclsGlobalSettings.ItemSellList.Contains(tempItem.Name))
                        {
                            SellItem(tempItem);
                            continue;
                        }
                    }
                }

                // Sell
                #endregion

                #region Buy

                // buy one of each item
                foreach (string itemName in clsSettings.gclsGlobalSettings.ItemBuyList)
                    UI.MerchantFrame.BuyItem(itemName, 1);

                // Buy
                #endregion

                // repair (from WoWBot - misc.iss)
                if (CanRepair)
                {
                    clsSettings.Logging.AddToLog(Resources.Reparing);

                    using (new clsFrameLock.LockBuffer())
                        //clsSettings.isxwow.WoWScript("RepairAllItems()");
                        UI.MerchantFrame.RepairAll();

                    // sleep for 5 seconds
                    Thread.Sleep(5000);
                }

                // exit
                clsSettings.Logging.AddToLog(Resources.SellToVendorExitingSuccessfull);
                eResult = clsPath.EMovementResult.Success;
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "SellToVendor");
                eResult = clsPath.EMovementResult.Error;
            }
            finally
            {
                UI.MerchantFrame.CloseFrame();
            }
            
            // return the result
            return eResult;
        }

        /// <summary>
        /// sells the item to the vendor
        /// </summary>
        /// <param name="tempItem">the item to sell</param>
        private static void SellItem(WoWItem tempItem)
        {
            try
            {
                // exit if invalid 
                if ((tempItem == null) || (!tempItem.IsValid))
                    return;

                // this item can be sold
                clsSettings.Logging.AddToLogFormatted("SellItem", Resources.SellingX, tempItem.Name);
                tempItem.Use();

                // wait for the item to sell
                Thread.Sleep(2000);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Sell Item");
            }
        }

        #region Delete Items

        /// <summary>
        /// Deletes all junk items from your bags. Returns true on success
        /// </summary>
        public static bool DeleteJunkItems()
        {
            if (!clsSettings.GuidValid)
                return false;

            bool rVal = false;

            try
            {
                // delete colored items
                if (clsSettings.gclsGlobalSettings.ItemJunkColors.Grey)
                    DeleteItems(string.Format("-items,-inventory,{0}", clsGlobalSettings.clsItemColorInfo.GreyFilter));
                if (clsSettings.gclsGlobalSettings.ItemJunkColors.White)
                    DeleteItems(string.Format("-items,-inventory,{0}", clsGlobalSettings.clsItemColorInfo.WhiteFilter));
                if (clsSettings.gclsGlobalSettings.ItemJunkColors.Green)
                    DeleteItems(string.Format("-items,-inventory,{0}", clsGlobalSettings.clsItemColorInfo.GreenFilter));
                if (clsSettings.gclsGlobalSettings.ItemJunkColors.Blue)
                    DeleteItems(string.Format("-items,-inventory,{0}", clsGlobalSettings.clsItemColorInfo.BlueFilter));
                if (clsSettings.gclsGlobalSettings.ItemJunkColors.Purple)
                    DeleteItems(string.Format("-items,-inventory,{0}", clsGlobalSettings.clsItemColorInfo.PurpleFilter));

                // if nothing in delete list, exit
                if (clsSettings.gclsGlobalSettings.ItemJunkList.Count == 0)
                    return true;

                // loop through delete list
                foreach (string itemName in clsSettings.gclsGlobalSettings.ItemJunkList)
                    DeleteItems(string.Format("-inventory,{0}", itemName.Trim()));

                rVal = true;

                // Check if we need to equip any bagged items
                clsAutoEquip.CheckNeedEquipItems();
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "DeleteJunkItems");
            }

            return rVal;
        }

        /// <summary>
        /// Deletes items in teh list
        /// </summary>
        private static void DeleteItems(string SearchStr)
        {
            List<WoWItem> itemList;

            try
            {
                // see if this item is in inventory
                itemList = clsSearch.Search_Item(SearchStr);

                // skip if no items found
                if ((itemList == null) || (itemList.Count == 0))
                    return;

                // pick up items and delete them
                foreach (WoWItem item in itemList)
                {
                    // pick it up
                    if (!item.PickUp())
                        continue;

                    // delete it
                    UI.Cursor.DeleteItem();

                    // wait
                    Thread.Sleep(500);
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, string.Empty);
            }            
        }

        // Delete Items
        #endregion

        #region Disenchant Items

        public static bool DisenchantItems()
        {
            if (!clsSettings.GuidValid)
                return false;

            bool rVal = false;

            try
            {
                // delete colored items
                if (clsSettings.gclsGlobalSettings.ItemDisenchantColors.Grey)
                    DisenchantItems(string.Format("-items,-inventory,{0}", clsGlobalSettings.clsItemColorInfo.GreyFilter));
                if (clsSettings.gclsGlobalSettings.ItemDisenchantColors.White)
                    DisenchantItems(string.Format("-items,-inventory,{0}", clsGlobalSettings.clsItemColorInfo.WhiteFilter));
                if (clsSettings.gclsGlobalSettings.ItemDisenchantColors.Green)
                    DisenchantItems(string.Format("-items,-inventory,{0}", clsGlobalSettings.clsItemColorInfo.GreenFilter));
                if (clsSettings.gclsGlobalSettings.ItemDisenchantColors.Blue)
                    DisenchantItems(string.Format("-items,-inventory,{0}", clsGlobalSettings.clsItemColorInfo.BlueFilter));
                if (clsSettings.gclsGlobalSettings.ItemDisenchantColors.Purple)
                    DisenchantItems(string.Format("-items,-inventory,{0}", clsGlobalSettings.clsItemColorInfo.PurpleFilter));

                // if nothing in delete list, exit
                if (clsSettings.gclsGlobalSettings.ItemJunkList.Count == 0)
                    return true;

                // loop through DisenchantItems list
                foreach (string itemName in clsSettings.gclsGlobalSettings.ItemDisenchantList)
                    DisenchantItems(string.Format("-inventory,{0}", itemName.Trim()));

                rVal = true;
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "DisenchantItems");
            }

            return rVal;
        }

        /// <summary>
        /// Disenchants items in the list
        /// </summary>
        private static void DisenchantItems(string SearchStr)
        {
            List<WoWItem> itemList;

            try
            {
                // see if this item is in inventory
                itemList = clsSearch.Search_Item(SearchStr);

                // skip if no items found
                if ((itemList == null) || (itemList.Count == 0))
                    return;

                // pick up items and Disenchant them
                foreach (WoWItem item in itemList)
                {
                    // cast de, skip if failed
                    if (!clsCombat.CastSpell("Disenchant"))
                        continue;

                    // wait 2 seconds
                    Thread.Sleep(2000);

                    // use it
                    if (!item.Use())
                        continue;

                    // log it
                    clsSettings.Logging.AddToLogFormatted(Resources.Disenchant, Resources.DisenchantingX, item.Name);

                    // while casting, sleep
                    while (clsCharacter.IsCasting)
                        Thread.Sleep(1000);

                    // get the loot
                    clsLoot.GetLoot();
                    
                    // wait before doing it again
                    Thread.Sleep(1000);
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "DisenchantItems");
            }
        }

        // Disenchant Items
        #endregion

        #region Open Lockbox

        /// <summary>
        /// Opens lockboxes in your bag
        /// </summary>
        public static void OpenLockboxes()
        {
            try
            {
                // wait a sec for loot window to close
                if (UI.LootWindowFrame.IsVisible)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        // exit the loot window is gone
                        if (!UI.LootWindowFrame.IsVisible)
                            break;

                        // sleep 1/10 second
                        Thread.Sleep(100);
                    }
                }

                // the items in your bag
                List<WoWItem> itemList = clsCharacter.GetBagItems();

                // loop through to find any lock boxes
                foreach (WoWItem item in itemList)
                {
                    // skip if not a lockbox
                    if (! item.FullName.Contains("Lockbox"))
                        continue;

                    // this item is a lockbox, cast picklock spell
                    clsSettings.Logging.AddToLogFormatted(Resources.Open, Resources.OpenLockboxesFoundX, item.Name);
                    if (clsCombat.CastSpell("Pick Lock"))
                        item.PickUp();

                    // try to open the item
                    item.Use();
                    Thread.Sleep(500);

                    // get the loot
                    clsLoot.GetLoot();
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "OpenLockboxes");
            }            
        }

        // Open Lockbox
        #endregion

        #region Open Items

        /// <summary>
        /// Opens items in your bag
        /// </summary>
        public static void OpenItems()
        {
            try
            {
                // wait a sec for loot window to close
                if (UI.LootWindowFrame.IsVisible)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        // exit the loot window is gone
                        if (!UI.LootWindowFrame.IsVisible)
                            break;

                        // sleep 1/10 second
                        Thread.Sleep(100);
                    }
                }

                // the items in your bag
                List<WoWItem> itemList = clsCharacter.GetBagItems();

                // loop through to find any items in the Open List
                foreach (WoWItem item in itemList)
                {
                    // skip if not in the list
                    if (!clsSettings.gclsGlobalSettings.ItemOpenList.Contains(item.FullName))
                        continue;

                    // this item is in the list, open it
                    clsSettings.Logging.AddToLogFormatted(Resources.Open, Resources.OpenItemsFoundX, item.FullName);
                    item.Use();
                    Thread.Sleep(500);

                    // get the loot
                    clsLoot.GetLoot();
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "OpenItems");
            }
        }

        // Open Items
        #endregion

        #region Need Vendor Item

        /// <summary>
        /// Used for Return to vendor when X has Y quantity
        /// </summary>
        public class clsNeedVendorItem
        {
            /// <summary>
            /// return to vendor when item is at or below this quantity
            /// </summary>
            public int Quantity = 0;

            /// <summary>
            /// Item name
            /// </summary>
            public string ItemName = string.Empty;

            /// <summary>
            /// Initializes a new instance of the clsAutoNav class.
            /// </summary>
            public clsNeedVendorItem()
            {
            }

            /// <summary>
            /// Initializes a new instance of the clsNeedVendorItem class.
            /// </summary>
            /// <param name="quantity">return to vendor when item is at or below this quantity</param>
            /// <param name="itemName">item name</param>
            public clsNeedVendorItem(int quantity, string itemName)
            {
                Quantity = quantity;
                ItemName = itemName;
            }
        }

        // Need Vendor Item
        #endregion

        #region Speak With Vendor

        /// <summary>
        /// Speaks with the vendor. returns false on error
        /// </summary>
        /// <param name="Vendor">The vendor.</param>
        private static bool SpeakWithVendor(WoWUnit Vendor)
        {
            bool rVal = false;

            try
            {
                // target and use the vendor
                Vendor.Target();
                Vendor.Use();

                // sleep for 3 seconds so the window can display
                Thread.Sleep(3000);

                // see if this vendor has gossip
                if (Vendor.CanGossip)
                {
                    // we need to select the gossip option
                    // http://www.isxwow.net/forums/viewtopic.php?f=20&t=713&hilit=dialog
                    // http://www.isxwow.net/forums/viewtopic.php?p=6902#p6902
                    using (new clsFrameLock.LockBuffer())
                        UI.GossipOptionsFrame.SelectOption(GossipFrame.GossipTypes.Gossip, 1);

                    // sleep 3 seconds for safety
                    Thread.Sleep(3000);
                }

                // return the visibilit of the merchant frame
                rVal = UI.MerchantFrame.IsVisible;
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "SpeakWithVendor");
            }

            return rVal;
        }

        // Speak With Vendor
        #endregion

        #region Buy Items

        /// <summary>
        /// Buys the item.
        /// </summary>
        /// <param name="vendor">The vendor buy from</param>
        /// <param name="ItemName">Name of the item.</param>
        /// <param name="Quantity">how many of this item to buy. NOTE: For items that sell in stacks, this is how many stacks to buy</param>
        public static bool BuyItem(WoWUnit vendor, string ItemName, int Quantity)
        {
            bool rVal = false;

            try
            {
                // move to the vendor if we are too far away
                if (clsPath.MoveToTarget(vendor, 2) != clsPath.EMovementResult.Success)
                {
                    clsSettings.Logging.AddToLog(Resources.CouldNotRunToVendor);
                    return false;
                }

                // open the dialog
                if ((! UI.MerchantFrame.IsVisible) && (! SpeakWithVendor(vendor)))
                {
                    clsSettings.Logging.AddToLog(Resources.CouldNotOpenDialog);
                    return false;
                }
                
                // dialog open, buy the item
                rVal = UI.MerchantFrame.BuyItem(ItemName, (uint)Quantity);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "BuyItem");
            }

            return rVal;
        }

        // Buy Items
        #endregion
    }
}
