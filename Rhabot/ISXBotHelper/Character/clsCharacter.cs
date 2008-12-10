// Event Names: 
// http://www.wowwiki.com/Events/Names
// http://www.wowwiki.com/Events/Communication

// Hooking Events:
// http://www.isxwow.net/forums/viewtopic.php?f=17&t=928&e=0
// http://www.isxwow.net/forums/viewtopic.php?f=15&t=611

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ISXBotHelper.Communication;
using ISXBotHelper.Properties;
using ISXWoW;

namespace ISXBotHelper
{
    /// <summary>
    /// General functions about the character
    /// </summary>
    public static class clsCharacter
    {
        #region Properties

        public static int MyCopper
        {
            get
            {
                using (new clsFrameLock.LockBuffer())
                    return clsSettings.isxwow.Me.Copper;
            }
        }

        /// <summary>
        /// Returns true if we are flying or swimming
        /// </summary>
        public static bool FlyingOrSwimming
        {
            get 
            {
                using (new clsFrameLock.LockBuffer())
                    return ((clsSettings.isxwow.Me.Flying) || (clsSettings.isxwow.Me.IsSwimming));
            }
        }

        public static bool IsFlying
        {
            get
            {
                using (new clsFrameLock.LockBuffer())
                    return clsSettings.isxwow.Me.Flying;
            }
        }

        /// <summary>
        /// Returns the toon's name
        /// </summary>
        public static string CharacterName
        {
            get 
            {
                using (new clsFrameLock.LockBuffer())
                    return clsSettings.isxwow.Me.Name;
            }
        }

        public static string ZoneText
        {
            get
            {
                using (new clsFrameLock.LockBuffer())
                    return clsSettings.isxwow.ZoneText;
            }
        }

        public static string RealZoneText
        {
            get
            {
                using (new clsFrameLock.LockBuffer())
                    return clsSettings.isxwow.RealZoneText;
            }
        }

        /// <summary>
        /// Returns casting status
        /// </summary>
        public static bool IsCasting
        {
            get 
            {
                using (new clsFrameLock.LockBuffer())
                    return clsSettings.isxwow.Me.IsCasting;
            }
        }

        /// <summary>
        /// returns true if the toon is in water
        /// </summary>
        /// <returns></returns>
        public static bool IsSwimming
        {
            get 
            { 
                using (new clsFrameLock.LockBuffer())
                    return clsSettings.isxwow.Me.IsSwimming; 
            }
        }

        /// <summary>
        /// Returns true if the toon is dead
        /// </summary>
        /// <returns></returns>
        public static bool IsDead
        {
            get 
            {
                using (new clsFrameLock.LockBuffer())
                    return clsSettings.isxwow.Me.Dead || clsSettings.isxwow.Me.IsGhost;
            }
        }

        /// <summary>
        /// Returns the current level of the character
        /// </summary>
        /// <returns></returns>
        public static int CurrentLevel
        {
            get 
            {
                using (new clsFrameLock.LockBuffer())
                    return clsSettings.isxwow.Me.Level; 
            }
        }

        public static WoWClass MyClass
        {
            get 
            {
                using (new clsFrameLock.LockBuffer())
                    return clsSettings.isxwow.Me.Class; 
            }
        }

        public static WoWFactionGroup MyFaction
        {
            get 
            {
                using (new clsFrameLock.LockBuffer())
                    return clsSettings.isxwow.Me.FactionGroup; 
            }
        }

        public static string MountName
        {
            get 
            {
                using (new clsFrameLock.LockBuffer())
                    return clsSettings.isxwow.Me.MountName; 
            }
        }

        public static PathListInfo.PathPoint MyLocation
        {
            get
            {
                using (new clsFrameLock.LockBuffer())
                    return new PathListInfo.PathPoint(clsSettings.isxwow.Me.Location);
            }
        }

        /// <summary>
        /// Returns the durability percent of the least item
        /// </summary>
        public static int DurabilityPercent
        {
            get
            {
                double lastDurable = 200;
                WoWItem tempItem;

                try
                {
                    // get each item's DurabilityPercent
                    using (new clsFrameLock.LockBuffer())
                    {
                        // back
                        tempItem = clsSettings.isxwow.Me.Equip(WoWEquipSlot.Back).GetItem();
                        //clsSettings.Logging.DebugWrite(string.Format("Durability - back: {0}", tempItem.DurabilityPercent));
                        if ((tempItem != null) && (tempItem.IsValid))
                            lastDurable = tempItem.DurabilityPercent;

                        // chest
                        tempItem = clsSettings.isxwow.Me.Equip(WoWEquipSlot.Chest).GetItem();
                        //clsSettings.Logging.DebugWrite(string.Format("Durability - chest: {0}", tempItem.DurabilityPercent));
                        if ((tempItem != null) && (lastDurable > tempItem.DurabilityPercent) && (tempItem.IsValid))
                            lastDurable = tempItem.DurabilityPercent;

                        // feet
                        tempItem = clsSettings.isxwow.Me.Equip(WoWEquipSlot.Feet).GetItem();
                        //clsSettings.Logging.DebugWrite(string.Format("Durability - feet: {0}", tempItem.DurabilityPercent));
                        if ((tempItem != null) && (lastDurable > tempItem.DurabilityPercent) && (tempItem.IsValid))
                            lastDurable = tempItem.DurabilityPercent;

                        // hands
                        tempItem = clsSettings.isxwow.Me.Equip(WoWEquipSlot.Hands).GetItem();
                        //clsSettings.Logging.DebugWrite(string.Format("Durability - hands: {0}", tempItem.DurabilityPercent));
                        if ((tempItem != null) && (lastDurable > tempItem.DurabilityPercent) && (tempItem.IsValid))
                            lastDurable = tempItem.DurabilityPercent;

                        // head
                        tempItem = clsSettings.isxwow.Me.Equip(WoWEquipSlot.Head).GetItem();
                        //clsSettings.Logging.DebugWrite(string.Format("Durability - head: {0}", tempItem.DurabilityPercent));
                        if ((tempItem != null) && (lastDurable > tempItem.DurabilityPercent) && (tempItem.IsValid))
                            lastDurable = tempItem.DurabilityPercent;

                        // legs
                        tempItem = clsSettings.isxwow.Me.Equip(WoWEquipSlot.Legs).GetItem();
                        //clsSettings.Logging.DebugWrite(string.Format("Durability - legs: {0}", tempItem.DurabilityPercent)); 
                        if ((tempItem != null) && (lastDurable > tempItem.DurabilityPercent) && (tempItem.IsValid))
                            lastDurable = tempItem.DurabilityPercent;

                        // mainhand
                        tempItem = clsSettings.isxwow.Me.Equip(WoWEquipSlot.MainHand).GetItem();
                        //clsSettings.Logging.DebugWrite(string.Format("Durability - mainhand: {0}", tempItem.DurabilityPercent));
                        if ((tempItem != null) && (lastDurable > tempItem.DurabilityPercent) && (tempItem.IsValid))
                            lastDurable = tempItem.DurabilityPercent;

                        // offhand
                        tempItem = clsSettings.isxwow.Me.Equip(WoWEquipSlot.OffHand).GetItem();
                        //clsSettings.Logging.DebugWrite(string.Format("Durability - offhand: {0}", tempItem.DurabilityPercent));
                        if ((tempItem != null) && (lastDurable > tempItem.DurabilityPercent) && (tempItem.IsValid))
                            lastDurable = tempItem.DurabilityPercent;

                        // ranged
                        tempItem = clsSettings.isxwow.Me.Equip(WoWEquipSlot.Ranged).GetItem();
                        //clsSettings.Logging.DebugWrite(string.Format("Durability - ranged: {0}", tempItem.DurabilityPercent));
                        if ((tempItem != null) && (lastDurable > tempItem.DurabilityPercent) && (tempItem.IsValid))
                            lastDurable = tempItem.DurabilityPercent;

                        // shoulders
                        tempItem = clsSettings.isxwow.Me.Equip(WoWEquipSlot.Shoulders).GetItem();
                        //clsSettings.Logging.DebugWrite(string.Format("Durability - shoulders: {0}", tempItem.DurabilityPercent));
                        if ((tempItem != null) && (lastDurable > tempItem.DurabilityPercent) && (tempItem.IsValid))
                            lastDurable = tempItem.DurabilityPercent;

                        // waist
                        tempItem = clsSettings.isxwow.Me.Equip(WoWEquipSlot.Waist).GetItem();
                        //clsSettings.Logging.DebugWrite(string.Format("Durability - waist: {0}", tempItem.DurabilityPercent));
                        if ((tempItem != null) && (lastDurable > tempItem.DurabilityPercent) && (tempItem.IsValid))
                            lastDurable = tempItem.DurabilityPercent;

                        // wrists
                        tempItem = clsSettings.isxwow.Me.Equip(WoWEquipSlot.Wrists).GetItem();
                        //clsSettings.Logging.DebugWrite(string.Format("Durability - wrists: {0}", tempItem.DurabilityPercent));
                        if ((tempItem != null) && (lastDurable > tempItem.DurabilityPercent) && (tempItem.IsValid))
                            lastDurable = tempItem.DurabilityPercent;
                    }
                }

                catch (Exception excep)
                {
                    clsError.ShowError(excep, "DurabilityPercent");
                }

                // reset if necessary
                if (lastDurable > 100)
                    lastDurable = 100;

                // we may need to change from percentage to whole number
                if (lastDurable < 1)
                    lastDurable = lastDurable * 100;


                // return the result
                return Convert.ToInt32(lastDurable);
            }
        }

        /// <summary>
        /// Returns the lowest mob to search based on settings
        /// </summary>
        public static int MobLowLevel
        {
            get
            {
                int LowLevel = CurrentLevel - clsSettings.gclsLevelSettings.LowLevelAttack;

                // return level 1 if the low level attack is set too low
                if (LowLevel < 1)
                    return 1;

                return LowLevel;
            }
        }

        /// <summary>
        /// Returns the highest mob to search based on settings
        /// </summary>
        public static int MobHighLevel
        {
            get
            {
                return CurrentLevel + clsSettings.gclsLevelSettings.HighLevelAttack;
            }
        }

        /// <summary>
        /// Returns our health percent
        /// </summary>
        public static double HealthPercent
        {
            get 
            {
                using (new clsFrameLock.LockBuffer())
                    return clsSettings.isxwow.Me.HealthPercent; 
            }
        }

        public static double ManaPercent
        {
            get
            {
                double mPercent = 100;

                // get mana if caster
                if (IsCaster())
                {
                    using (new clsFrameLock.LockBuffer())
                        mPercent = clsSettings.isxwow.Me.ManaPercent;
                }

                // return
                return mPercent;
            }
        }

        /// <summary>
        /// Holds the list of bag names that should not be checked for vendoring
        /// </summary>
        private static readonly string[] BadBags = new string[] { "QUIVER", "SOUL", "GEM", "HERB", "AMMO", "MINING", "FELCLOTH" };

        /// <summary>
        /// Returns true if we need to see a vendor for full bags or durability
        /// </summary>
        public static bool NeedsVendor
        {
            get
            {
                // log it
                if (clsSettings.VerboseLogging)
                    clsSettings.Logging.AddToLog(Resources.CheckIfNeedsVendor);

                // durability check
                int dp = DurabilityPercent;
                if (dp <= clsSettings.gclsLevelSettings.DurabilityPercent)
                {
                    clsSettings.Logging.AddToLogFormatted(Resources.NeedsVendor, Resources.NeedsVendorDurabilityLow, dp);
                    return true;
                }

                return BagsFull;
            }
        }

        /// <summary>
        /// Returns true if your bags are full
        /// </summary>
        public static bool BagsFull
        {
            get
            {
                // check for full bags
                bool BagFull = true;
                bool ScannedBag = false;
                WoWContainer wc;
                try
                {
                    using (new clsFrameLock.LockBuffer())
                    {
                        // loop through bags to see if they are full
                        for (int i = 0; i < 5; i++)
                        {
                            wc = clsSettings.isxwow.Me.Bag(i).GetContainer();

                            if ((wc != null) && (clsSettings.VerboseLogging))
                                clsSettings.Logging.DebugWrite(string.Format(Resources.ScanningBagX, wc.Name));

                            // skip if no bag
                            if ((wc == null) || (!wc.IsValid))
                            {
                                if (clsSettings.VerboseLogging)
                                    clsSettings.Logging.DebugWrite(string.Format(Resources.BagInSlotX, i.ToString().Trim()));
                                continue;
                            }

                            // skip if bag is quiver or other speciality
                            ScannedBag = true;
                            bool GoodBag = true;
                            foreach (string badName in BadBags)
                            {
                                if (wc.Name.ToUpper().Contains(badName))
                                {
                                    clsSettings.Logging.DebugWrite(string.Format(Resources.NeedsVendorBadBag, wc.Name));
                                    GoodBag = false;
                                    break;
                                }
                            }

                            // if bad bag, skip
                            if (!GoodBag)
                                continue;

                            // if the bag is NOT full, we can return false
                            if (wc.EmptySlots > 1)
                            {
                                if (clsSettings.VerboseLogging)
                                    clsSettings.Logging.AddToLog(Resources.DoesNotNeedVendor);
                                return false;
                            }
                        }
                    }

                    // if no bags scanned, reset bagfull
                    if (!ScannedBag)
                        BagFull = false;
                }

                catch (Exception excep)
                {
                    clsError.ShowError(excep, Resources.NeedsVendor);
                }

                clsSettings.Logging.AddToLogFormatted(Resources.NeedsVendor, Resources.ReturningX, BagFull.ToString());
                return BagFull;
            }
        }

        /// <summary>
        /// Gets the player's current experience
        /// </summary>
        public static int MyXP
        {
            get 
            {
                using (new clsFrameLock.LockBuffer())
                    return clsSettings.isxwow.Me.Exp; 
            }
        }

        public static bool CharacterIsValid
        {
            get 
            { 
                using (new clsFrameLock.LockBuffer())
                    return ((clsSettings.isxwow.Me != null) && (clsSettings.isxwow.Me.IsValid)); 
            }
        }

        public static WoWItem EquipedItem(WoWEquipSlot EquipSlot)
        {
            using (new clsFrameLock.LockBuffer())
                return clsSettings.isxwow.Me.Equip(EquipSlot);
        }

        public static double MyHeading
        {
            get
            {
                using (new clsFrameLock.LockBuffer())
                    return clsSettings.isxwow.Me.Heading;
            }
        }

        /// <summary>
        /// Returns if the loot window is visible
        /// </summary>
        public static bool LootWindowIsVisible
        {
            get
            {
                using (new clsFrameLock.LockBuffer())
                    return UI.LootWindowFrame.IsVisible;
            }
        }

        /// <summary>
        /// Returns how many items are in the loot window
        /// </summary>
        public static int LootWindowItemCount
        {
            get
            {
                using (new clsFrameLock.LockBuffer())
                    return UI.LootWindowFrame.ItemCount;
            }
        }

        public static bool IsGhost
        {
            get
            {
                using (new clsFrameLock.LockBuffer())
                    return clsSettings.isxwow.Me.IsGhost;
            }
        }

        public static bool IsHoldingBreath
        {
            get
            {
                using (new clsFrameLock.LockBuffer())
                    return clsSettings.isxwow.Me.HoldingBreath;
            }
        }

        /// <summary>
        /// Returns the zone name enum for the current zone
        /// </summary>
        public static ISXRhabotGlobal.clsGlobals.EZoneName EZoneName
        {
            get
            {
                // get the zone text
                string zText = ZoneText;

                // get the "real" text if the regular one fails
                if (string.IsNullOrEmpty(zText))
                    zText = RealZoneText;

                if (! string.IsNullOrEmpty(zText))
                    zText = zText.Replace(" ", "_").Replace("'", "");

                // get the enum
                try
                {
                    if (string.IsNullOrEmpty(zText))
                        return ISXRhabotGlobal.clsGlobals.EZoneName.NONE;

                    return (ISXRhabotGlobal.clsGlobals.EZoneName)Enum.Parse(typeof(ISXRhabotGlobal.clsGlobals.EZoneName), zText);
                }

                catch (Exception excep)
                {
                    clsError.ShowError(excep, "EZoneName");
                    return ISXRhabotGlobal.clsGlobals.EZoneName.NONE;
                }
            }
        }

        #region Pet_Stuff

        public static bool HunterHasPet
        {
            get
            {
                using (new clsFrameLock.LockBuffer())
                {
                    // if less than level 10, return false
                    if (clsSettings.isxwow.Me.Level < 10)
                        return false;

                    return ((clsSettings.isxwow.Me.Pet != null) && 
                        (clsSettings.isxwow.Me.Pet.IsValid) &&
                        (clsSettings.isxwow.Me.Pet.Classification != WoWClassification.NULL));
                }
            }
        }

        public static bool IsPetDead
        {
            get
            {
                using (new clsFrameLock.LockBuffer())
                {
                    if (!HunterHasPet)
                        return false;
                    return clsSettings.isxwow.Me.Pet.Dead;
                }
            }
        }

        public static double PetHappiness
        {
            get
            {
                using (new clsFrameLock.LockBuffer())
                {
                    if (!HunterHasPet)
                        return 0;
                    return clsSettings.isxwow.Me.Pet.HappinessPercent;
                }
            }
        }

        /// <summary>
        /// Revives or calls your pet
        /// </summary>
        public static void CallPet()
        {
            // http://www.wowwiki.com/Useful_macros/Hunter#Revive_Pet_-.3E_Call_Pet_-.3E_Mend_Pet
            clsSettings.ExecuteWoWAPI("cast [nopet] Call Pet; Revive Pet");

            // sleep while we cast
            while (IsCasting)
                Thread.Sleep(100);
        }

        // Pet_Stuff
        #endregion

        // Properties
        #endregion

        #region WoW Events

        /// <summary>
        /// Hooks the event for characters leveling
        /// </summary>
        public static void HookCharacterLevelEvent(object objState)
        {
            try
            {
                // log it
                clsSettings.Logging.AddToLog(Resources.HookingCharacterLevelEvent);

                // hook it
                clsWowEvents.CharLevel += clsWowEvents_CharLevel;
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "HookCharacterLevelEvent");
            }
        }

        /// <summary>
        /// Raised by WoW when the character levels
        /// </summary>
        static void clsWowEvents_CharLevel(int Level)
        {
            try
            {
                // raise the level event
                clsEvent.Raise_CharacterLeveled(Level);

                // check each item to see if it needs to be updated
                ItemLevelCheck();
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "CharLevel");
            }
        }

        /// <summary>
        /// Checks items to ensure they are 25% or better of our current level
        /// </summary>
        private static void ItemLevelCheck()
        {
            StringBuilder sb = new StringBuilder();
            int CurrLevel = CurrentLevel;

            try
            {
                // if we are less than 10, then exit
                if (CurrLevel < 10)
                    return;

                using (new clsFrameLock.LockBuffer())
                {
                    // check all items and build a message string
                    sb.Append(CheckItemLevel(WoWEquipSlot.Back));
                    sb.Append(CheckItemLevel(WoWEquipSlot.Chest));
                    sb.Append(CheckItemLevel(WoWEquipSlot.Feet));
                    sb.Append(CheckItemLevel(WoWEquipSlot.Hands));
                    sb.Append(CheckItemLevel(WoWEquipSlot.Head));
                    sb.Append(CheckItemLevel(WoWEquipSlot.Legs));
                    sb.Append(CheckItemLevel(WoWEquipSlot.MainHand));
                    sb.Append(CheckItemLevel(WoWEquipSlot.Neck));
                    sb.Append(CheckItemLevel(WoWEquipSlot.OffHand));
                    sb.Append(CheckItemLevel(WoWEquipSlot.Ranged));
                    sb.Append(CheckItemLevel(WoWEquipSlot.Shoulders));
                    sb.Append(CheckItemLevel(WoWEquipSlot.Waist));
                    sb.Append(CheckItemLevel(WoWEquipSlot.Wrists));
                }

                // if no string, exit
                if (sb.Length == 0)
                    return;

                // insert our message
                sb.Insert(0, string.Format(Resources.UpgradeNotice, CharacterName, CurrentLevel));

                // send to all clients
                clsSend.SendToAll(sb.ToString(), string.Format(Resources.EquipmentUpgradeNotice, CharacterName));
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "ItemLevelCheck");
            }            
        }

        /// <summary>
        /// Returns a string if the item needs to be upgraded
        /// </summary>
        private static string CheckItemLevel(WoWEquipSlot eSlot)
        {
            using (new clsFrameLock.LockBuffer())
            {
                WoWItem tempItem = clsSettings.isxwow.Me.Equip(eSlot).GetItem();
                if ((tempItem != null) &&
                    (tempItem.IsValid) &&
                    (tempItem.ItemLevel > 0) &&
                    (((double)tempItem.ItemLevel / (double)CurrentLevel) <= 0.7))
                    return string.Format(Resources.XItemLevel, eSlot, tempItem.ItemLevel);
                else
                    return string.Empty;
            }
        }

        public static WoWUnit CurrentTarget
        {
            get
            {
                using (new clsFrameLock.LockBuffer())
                    return clsSettings.isxwow.Me.CurrentTarget;
            }
        }

        // WoW Events
        #endregion

        #region Functions

        private static bool m_IsCaster = false;
        private static bool m_CasterSet = false; // set to true when IsCaster is cached
        /// <summary>
        /// returns true if the toon is a caster
        /// </summary>
        public static bool IsCaster()
        {
            // if IsCaster is cached, return that
            if (m_CasterSet)
                return m_IsCaster;

            // not cached, load value
            m_IsCaster = false;
            using (new clsFrameLock.LockBuffer())
            {
                // get caster types
                switch (clsSettings.isxwow.Me.Class)
                {
                    case WoWClass.Hunter:
                    case WoWClass.Mage:
                    case WoWClass.Paladin:
                    case WoWClass.Priest:
                    case WoWClass.Shaman:
                    case WoWClass.Warlock:
                        m_IsCaster = true;
                        break;

                    case WoWClass.Druid:
                        // if we are morphed, we don't need mana
                        if ((BuffExists("Bear Form")) || (BuffExists("Aquatic Form")) || (BuffExists("Cat Form")) || (BuffExists("Travel Form")) || (BuffExists("Dire Bear Form")) || (BuffExists("Flight Form")) || (BuffExists("Swift Flight Form")))
                            return false;
                        else
                            return true;
                }
            }

            // mark as cached
            m_CasterSet = true;

            // return result
            return m_IsCaster;
        }

        private static bool m_IsWandUser = false;
        private static bool m_IsWandUserSet = false; // set to true when IsCaster is cached
        /// <summary>
        /// returns true if the toon is a caster
        /// </summary>
        public static bool IsWandUser()
        {
            // if IsCaster is cached, return that
            if (m_IsWandUserSet)
                return m_IsWandUser;

            // not cached, load value
            m_IsWandUser = false;
            using (new clsFrameLock.LockBuffer())
            {
                // get caster types
                switch (clsSettings.isxwow.Me.Class)
                {
                    case WoWClass.Mage:
                    case WoWClass.Priest:
                    case WoWClass.Warlock:
                        m_IsWandUser = true;
                        break;
                }
            }

            // mark as cached
            m_IsWandUserSet = true;

            // return result
            return m_IsWandUser;
        }

        #region Bags

        /// <summary>
        /// Returns a list of all items in your bag
        /// </summary>
        public static List<WoWItem> GetBagItems()
        {
            return clsSearch.Search_Item("-items,-inventory");
        }

        /// <summary>
        /// Returns a list of the names of all items in your bag
        /// </summary>
        public static List<string> GetBagItemsNames()
        {
            List<string> retList = new List<string>();

            try
            {
                using (new clsFrameLock.LockBuffer())
                {
                    // get the list of bag items
                    List<WoWItem> itemList = clsSearch.Search_Item("-items,-inventory");

                    // loop through the list to get item names
                    foreach (WoWItem item in itemList)
                        retList.Add(item.FullName);
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "GetBagItemsNames");
            }

            return retList;
        }

        // Bags
        #endregion

        /// <summary>
        /// The bot stops moving and stones home
        /// </summary>
        public static void StoneHome()
        {
            try
            {
                // stop moving
                clsPath.StopMoving();
                clsMount.Dismount();

                // exit if dead
                if (IsDead)
                    return;

                clsSettings.Logging.AddToLog(Resources.StoneHome, Resources.CastingSpell);

                // try to cast the spell, first
                if (!clsCombat.CastSpell("Hearthstone"))
                {
                    clsSettings.Logging.AddToLog(Resources.StoneHome, Resources.SpellCastFailed1);

                    // go home
                    List<WoWItem> itemList = clsSearch.Search_Item("-items,-inventory,Hearthstone");
                    if ((itemList != null) && (itemList.Count > 0))
                    {
                        using (new clsFrameLock.LockBuffer())
                            itemList[0].Use();
                    }
                    else
                        clsSettings.Logging.AddToLog(Resources.StoneHome, Resources.CouldNotUseHearthstone);

                    // wait for the stone to cast
                    while (IsCasting)
                        Thread.Sleep(200);
                }

                // sleep for 60 seconds for screen to refresh
                Thread.Sleep(60000);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.StoneHome);
            }
        }

        internal static void Shutdown()
        {
            // detach level up
            clsWowEvents.CharLevel -= clsWowEvents_CharLevel;
        }

        /// <summary>
        /// Returns currency in 0g 0s 0c format
        /// </summary>
        /// <param name="Copper">the amount of money, in copper, to parse</param>
        public static string ParseCoinage(int Copper)
        {
            try
            {
                // split into g, s, and c
                string gParse = Copper.ToString().Trim();

                // pad 0's
                while (gParse.Length < 6)
                    gParse = gParse.Insert(0, "0");

                // get coinage
                string copper = gParse.Substring(gParse.Length - 2);
                string silver = gParse.Substring(gParse.Length - 4, 2);
                string gold = gParse.Substring(0, gParse.Length - 4);

                // return amount
                return string.Format(Resources.GoldFormat, copper, silver, gold);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "ParseCoinage");
            }

            return Resources.GoldFormat;
        }

        public static bool BuffExists(string BuffName)
        {
            try
            {
                WoWBuff buff;

                using (new clsFrameLock.LockBuffer())
                {
                    buff = clsSettings.isxwow.Me.Buff(BuffName);
                    return ((buff != null) && (buff.IsValid));
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "BuffExists");
            }
            return false;
        }

        public static bool BuffExists(WoWUnit TargetUnit, string BuffName)
        {
            try
            {
                WoWBuff buff;

                using (new clsFrameLock.LockBuffer())
                {
                    buff = TargetUnit.Buff(BuffName);
                    return ((buff != null) && (buff.IsValid));
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "BuffExists");
            }
            return false;
        }

        /// <summary>
        /// Removes a list of buffs player applied buffs. NOTE: The buff names must match a spell you can cast
        /// to remove the buff. For example: "Bear Form", "Aquatic Form", etc
        /// </summary>
        /// <param name="BuffList">the list to remove</param>
        public static bool RemoveListOfBuffs(List<string> BuffList)
        {
            bool rVal = false;

            try
            {
                WoWBuff buff;

                // lock frame
                using (new clsFrameLock.LockBuffer())
                {
                    // loop through list and remove buff
                    foreach (string BuffName in BuffList)
                    {
                        // get the buff
                        buff = clsSettings.isxwow.Me.Buff(BuffName);

                        // remove it if we can
                        if ((buff != null) && (buff.IsValid))
                            clsCombat.CastSpell_NoCheck(BuffName);
                    }
                }

                rVal = true;
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "RemoveListOfBuffs");
            }

            return rVal;
        }

        // Functions
        #endregion
    }
}
