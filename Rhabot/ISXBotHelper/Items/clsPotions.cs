using System;
using System.Collections.Generic;
using ISXWoW;

namespace ISXBotHelper
{
    /// <summary>
    /// Functions for using potions and bandages
    /// </summary>
    public static class clsPotions
    {
        #region Potions

        /// <summary>
        /// Drinks the best potion in the list (best is element 0)
        /// </summary>
        /// <param name="PotionList">the list to search</param>
        private static bool DrinkBestPotion(string[] PotionList)
        {
            try
            {
                // get the items from your bag
                List<string> itemList = clsCharacter.GetBagItemsNames();

                // loop through the potion list to see if a match is found
                for (int i = 0; i < PotionList.Length; i++)
                {
                    if (itemList.Contains(PotionList[i]))
                        return UsePotion(PotionList[i]);
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "DrinkBestPotion");
            }

            return false;
        }

        /// <summary>
        /// Finds the best healing potion in inventory and uses. Returns false on error
        /// </summary>
        public static bool DrinkBestHealingPotion()
        {
            string[] PotionList = new string[13] { "Super Rejuvenation Potion", "Major Combat Healing Potion", "Super Healing Potion", "Major Rejuvenation Potion", "Volatile Healing Potion", "Major Healing Potion", "Combat Healing Potion", "Superior Healing Potion", "Greater Healing Potion", "Healing Potion", "Discolored Healing Potion", "Lesser Healing Potion", "Minor Healing Potion" };
            return DrinkBestPotion(PotionList);
        }

        /// <summary>
        /// Finds the best mana potion in inventory and uses. Returns true if the potion was used
        /// </summary>
        public static bool DrinkBestManaPotion()
        {
            string[] PotionList = new string[10] { "Major Combat Mana Potion", "Super Mana Potion", "Volatile Mana Potion", "Major Mana Potion", "Combat Mana Potion", "Superior Mana Potion", "Greater Mana Potion", "Mana Potion", "Lesser Mana Potion", "Minor Mana Potion" };
            return DrinkBestPotion(PotionList);
        }

        /// <summary>
        /// Uses the potion if it exists. Returns true if used, else returns false
        /// </summary>
        /// <param name="PotionName"></param>
        /// <returns></returns>
        public static bool UsePotion(string PotionName)
        {
            bool rVal = false;

            try
            {
                using (new clsFrameLock.LockBuffer())
                {
                    // find the item
                    WoWItem tempItem = clsItem.FindItem(string.Format("-inventory, {0}", PotionName));

                    // if something returned, then use it
                    if ((tempItem != null) && (tempItem.IsValid))
                    {
                        // use the potion
                        rVal = tempItem.Use();
                    }
                }

                // if used, then wait for half a second
                if (rVal)
                    System.Threading.Thread.Sleep(500);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "clsPotions.UseItem");
            }

            // return the result
            return rVal;
        }

        // Potions
        #endregion

        #region Bandages

        /// <summary>
        /// Uses the best bandage. Returns true on success, otherwise false
        /// </summary>
        public static bool UseBestBandage()
        {
            string[] BandageList = new string[10] { "Heavy Netherweave Bandage", "Netherweave Bandage", "Heavy Runecloth Bandage", "Runecloth Bandage", "Heavy Mageweave Bandage", "Mageweave Bandage", "Heavy Silk Bandage", "Silk Bandage", "Heavy Linen Bandage", "Linen Bandage" };

            try
            {
                // get the items from your bag
                List<string> itemList = clsCharacter.GetBagItemsNames();

                // loop through the potion list to see if a match is found
                for (int i = 0; i < BandageList.Length; i++)
                {
                    if (itemList.Contains(BandageList[i]))
                        return UseBandage(BandageList[i]);
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "UseBestBandage");
            }

            return false;
        }

        /// <summary>
        /// Uses the bandage if it exists. Returns true if used, else returns false
        /// </summary>
        public static bool UseBandage(string BandageName)
        {
            bool rVal = false;

            try
            {
                using (new clsFrameLock.LockBuffer())
                {
                    // exit if we have the bandage debuff
                    if (clsCharacter.BuffExists("Recently Bandaged"))
                    {
                        clsSettings.Logging.AddToLog("UseBestBandage: We have been recently bandaged. Can not bandage again");
                        return true;
                    }

                    // find the item
                    WoWItem tempItem = clsItem.FindItem(string.Format("-inventory, {0}", BandageName));

                    // if something returned, then use it
                    if ((tempItem != null) && (tempItem.IsValid))
                    {
                        // use the potion
                        rVal = tempItem.Use();
                    }
                }

                // if used, then wait for two seconds
                while (clsCharacter.IsCasting)
                    System.Threading.Thread.Sleep(200);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "clsPotions.UseBandage");
            }

            // return the result
            return rVal;
        }
        // Bandages
        #endregion
    }
}
