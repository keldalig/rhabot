using System;
using System.Collections.Generic;
using ISXBotHelper.Properties;
using ISXWoW;

namespace ISXBotHelper
{
    /// <summary>
    /// Functions for dealing with items
    /// </summary>
    public static class clsItem
    {
        public static int GetItemCooldown(WoWItem item)
        {
            try
            {
                using (new clsFrameLock.LockBuffer())
                {
                    // exit if item is invalid
                    if ((item == null) || (!item.IsValid))
                        return 0;

                    // get the cooldown
                    string coolDown = clsSettings.isxwow.WoWScript<string>(string.Format("GetContainerItemCooldown({0}, {1})", item.Bag.Number.ToString().Trim(), item.Bag.Slot.ToString().Trim()), 1);

                    // return 0 if null or empty or not numeric
                    if (!clsSettings.IsNumeric(coolDown))
                        return 0;

                    // return the cooldown
                    return Convert.ToInt32(coolDown);
                }
            }
            
            catch (Exception excep)
            {
            	clsError.ShowError(excep, "GetItemCooldown");
            }

            return 0;
        }

        /// <summary>
        /// Returns one item from your bag that is usable and has the highest level or returns null
        /// </summary>
        /// <param name="searchString">the item to search fo</param>
        public static WoWItem FindItem(string searchString)
        {
            WoWItem tempItem = null;
            int CurrLevel = clsCharacter.CurrentLevel;

            try
            {
                // log it
                clsSettings.Logging.AddToLogFormatted("FindItem", Resources.FindingUsuableItemX, searchString);

                // get the item list
                List<WoWItem> itemList = clsSearch.Search_Item(searchString);

                // if nothing return, exit
                if (itemList.Count == 0)
                    return null;

                // if only one item, return it
                if ((itemList.Count == 1) && (GetItemCooldown(itemList[0]) == 0))
                    return itemList[0];

                // find the item with the highest minimum level
                using (new clsFrameLock.LockBuffer())
                {
                    foreach (WoWItem item in itemList)
                    {
                        // if the min level is above our level or has a cooldown, skip it
                        if ((item.MinLevel > CurrLevel) || (GetItemCooldown(item) > 0))
                            continue;

                        // compare min level to temp item
                        if ((tempItem == null) || (tempItem.MinLevel < item.MinLevel))
                            tempItem = item;
                    }
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "FindItem");
            }

            return tempItem;
        }

        /// <summary>
        /// Returns true if the item is valid
        /// </summary>
        public static bool ItemIsValid(WoWObject ItemToCheck)
        {
            using (new clsFrameLock.LockBuffer())
                return ((ItemToCheck != null) && (ItemToCheck.IsValid));
        }
    }
}
