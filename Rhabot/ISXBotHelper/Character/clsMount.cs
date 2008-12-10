// with many thanks to the post at: http://www.ismods.com/forums/viewtopic.php?p=20136

using System;
using System.Collections.Generic;
using System.Threading;
using ISXBotHelper.Properties;
using ISXWoW;
using LavishScriptAPI;

namespace ISXBotHelper
{
    public static class clsMount
    {
        #region Properties

        private static WoWItem m_MountItem = null;
        /// <summary>
        /// The mount item used
        /// </summary>
        public static WoWItem MountItem
        {
            get { return m_MountItem; }
            set { m_MountItem = value; }
        }

        private static string m_MountSummonSpell = string.Empty;
        /// <summary>
        /// The spell used to summon this mount. Empty if spell was not used
        /// </summary>
        public static string MountSummonSpell
        {
            get { return m_MountSummonSpell; }
            set { m_MountSummonSpell = value; }
        }

        public static bool IsMounted
        {
            get
            {
                using (new clsFrameLock.LockBuffer())
                {
                    // if we are shaman and shape shifted, return true
                    if ((clsSettings.isxwow.Me.Class == WoWClass.Shaman) &&
                        ((clsCharacter.CurrentLevel > 19) && (clsCharacter.CurrentLevel < 40)) &&
                        clsCharacter.BuffExists("Ghost Wolf"))
                            return true;

                    // exit if too low level
                    if (clsCharacter.CurrentLevel < 40)
                        return false;

                    // if we are mounted, then exit
                    if (!string.IsNullOrEmpty(clsCharacter.MountName))
                        return true;
                }

                return false;
            }
        }

        public static bool IsFlyMounted
        {
            get
            {
                using (new clsFrameLock.LockBuffer())
                {
                    string mName = clsCharacter.MountName.ToLower();
                    if (string.IsNullOrEmpty(mName))
                        return false;

                    if ((mName.Contains("gryphon")) ||
                        (mName.Contains("windrider")) ||
                        (mName.Contains("nether ray")) ||
                        (mName.Contains("nether drake")) ||
                        (mName.Contains("phoenix")))
                        return true;

                    return true;
                }
            }
        }

        // Properties
        #endregion

        #region Mount Up

        /// <summary>
        /// Finds and uses the best mount for you. Returns true if we mounted
        /// </summary>
        public static bool MountUp()
        {
            bool DoGhostWolf = false;

            try
            {
                // exit if dead
                if (clsCharacter.IsDead)
                    return true;

                // log it
                clsSettings.Logging.AddToLog(Resources.FindingMount);

                if (IsMounted)
                    return true;

                // reset properties
                m_MountItem = null;
                m_MountSummonSpell = string.Empty;

                #region Shape Shifting
                
                // if we are shaman and too low for a mount, try to shapeshift
                using (new clsFrameLock.LockBuffer())
                {
                    // check if shaman and 20-39
                    if ((clsSettings.isxwow.Me.Class == WoWClass.Shaman) &&
                        ((clsCharacter.CurrentLevel > 19) && (clsCharacter.CurrentLevel < 40)))
                    {
                        // if we are already morphed, then exit
                        if (clsCharacter.BuffExists("Ghost Wolf"))
                            return true;

                        // cast ghost wolf
                        DoGhostWolf = true;
                    }
                }
                if (DoGhostWolf)
                {
                    clsPath.StopMoving();
                    // cast ghost wolf
                    clsSettings.Logging.AddToLog(Resources.FoundGhostWolf);
                    m_MountSummonSpell = "Ghost Wolf";
                    return CastMountSpell();
                }

                // Shape Shifting
                #endregion

                // if we can not shape shift, and are less than 40, then we should exit
                if (clsCharacter.CurrentLevel < 40)
                {
                    clsSettings.Logging.AddToLog(Resources.LevelIsLessThan40Exiting);
                    return false;
                }

                // get the items in this character's bag
                List<WoWItem> itemList = clsSearch.Search_Item("-items,-inventory,-usable");

                // if the bags are empty, then exit
                if (itemList.Count == 0)
                {
                    clsSettings.Logging.AddToLog(Resources.NoMountsBag);
                    return false;
                }

                // loop through each item to see if it can be mountable. if we mount, then exit
                foreach (WoWItem item in itemList)
                {
                    // check if this is a mount we can use
                    if (FindMount(item))
                    {
                        // sleep while casting
                        while (clsCharacter.IsCasting)
                            Thread.Sleep(200);

                        // log it
                        clsSettings.Logging.AddToLogFormatted(Resources.UsingMount, item.Name);

                        // set property
                        m_MountItem = item;

                        return true;
                    }
                }

                #region Summon Mount

                // exit if not a summoning class
                switch (clsCharacter.MyClass)
                {
                    case WoWClass.Warrior:
                    case WoWClass.Rogue:
                    case WoWClass.Hunter:
                    case WoWClass.Mage:
                    case WoWClass.Priest:
                        return false; // these can not mount
                    
                    case WoWClass.Shaman:
                        // try ghost wolf
                        clsPath.StopMoving();
                        m_MountSummonSpell = "Ghost Wolf";
                        return clsCombat.CastSpell(m_MountSummonSpell);

                    case WoWClass.Paladin:
                        // try charger, then warhorse
                        clsPath.StopMoving();
                        m_MountSummonSpell = "Summon Charger";
                        if (!CastMountSpell())
                        {
                            m_MountSummonSpell = "Summon Warhorse";
                            return CastMountSpell();
                        }
                        else
                            return true;

                    case WoWClass.Warlock:
                        // try dreadstead, then felstead
                        clsPath.StopMoving();
                        m_MountSummonSpell = "Summon Dreadsteed";
                        if (!CastMountSpell())
                        {
                            m_MountSummonSpell = "Summon Felsteed";
                            return CastMountSpell();
                        }
                        else
                            return true;
                }

                // Summon Mount
                #endregion
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "MountUp");
            }

            // if we are here, then no mount was found
            clsSettings.Logging.AddToLog(Resources.ExitingMountUpNoMountsFound); 
            return false;
        }

        private static bool CastMountSpell()
        {
            clsPath.StopMoving();
            Thread.Sleep(500);

            // cast the mount spell
            bool rVal = clsCombat.CastSpell(m_MountSummonSpell);

            // wait a sec for the effect to happen
            Thread.Sleep(1000);
            return rVal;
        }

        /// <summary>
        /// Checks if the item is a mount. If so, mounts. Returns true if we mounted up
        /// </summary>
        /// <param name="item">the item to check</param>
        private static bool FindMount(WoWItem item)
        {
            try
            {
                using (new clsFrameLock.LockBuffer())
                {
                    // exit if not usable
                    if (!item.Usable)
                        return false;

                    #region Alliance >= 60

                    if ((clsCharacter.CurrentLevel >= 60) && (clsCharacter.MyFaction == WoWFactionGroup.Alliance))
                    {
                        // see if our mount is uber
                        switch (item.Name)
                        {
                            case "Stormpike Battle Charger":
                            case "Swift Brown Steed":
                            case "Swift Palomino":
                            case "Swift White Steed":
                            case "Swift Brown Ram":
                            case "Swift Gray Ram":
                            case "Swift White Ram":
                            case "Swift White Mechanostrider":
                            case "Swift Yellow Mechanostrider":
                            case "Reins of the Swift Frostsaber":
                            case "Reins of the Swift Mistsaber":
                            case "Reins of the Swift Stormsaber":
                            case "Reins of the Winterspring Frostsaber":// Reputation Nightelf Mount
                            case "Reins of the Black War Tiger": // Epic PVP Mount
                            case "Black War Steed Bridle": // Epic PVP Mount
                            case "Black War Ram": // Epic PVP Mount
                            case "Black Battlestrider":// Epic PVP Mount
                            case "Icy Blue Mechanostrider Mod A":// Old Item - No longer sold
                            case "White Stallion Bridle":// Old Item - No longer sold
                            case "Frost Ram":// Old Item - No longer sold
                            case "Reins of the Frostsaber":// Old Item - No longer sold
                            case "Reins of the Nightsaber":// Old Item - No longer sold
                                clsPath.StopMoving();
                                Thread.Sleep(500);
                                return item.Use();
                        }
                    }

                    // Alliance > 60
                    #endregion

                    #region Horde >= 60

                    if ((clsCharacter.CurrentLevel >= 60) && (clsCharacter.MyFaction == WoWFactionGroup.Horde))
                    {
                        switch (item.Name)
                        {
                            case "Horn of the Frostwolf Howler":// Done first since all 60's can get this easily.
                            case "Horn of the Swift Timber Wolf":
                            case "Horn of the Swift Gray Wolf":
                            case "Horn of the Swift Brown Wolf":
                            case "Swift Blue Raptor":
                            case "Swift Olive Raptor":
                            case "Swift Orange Raptor":
                            case "Purple Skeletal Warhorse":
                            case "Green Skeletal Warhorse":
                            case "Great Brown Kodo":
                            case "Great Gray Kodo":
                            case "Great White Kodo":
                            case "Red Skeletal Warhorse":// Epic PVP Mount
                            case "Whistle of the Black War Raptor":// Epic PVP Mount
                            case "Black War Kodo":// Epic PVP Mount
                            case "Horn of the Black War Wolf":// Epic PVP Mount
                            case "Green Kodo":// Old Item - No longer sold
                            case "Teal Kodo":// Old Item - No longer sold
                            case "Artic Wolf":// Old Item - No longer sold
                            case "Whistle of the Mottled Red Raptor":// Old Item - No longer sold
                            case "Whistle of the Ivory Raptor":// Old Item - No longer sold
                                clsPath.StopMoving();
                                Thread.Sleep(500);
                                return item.Use();
                        }
                    }

                    // Horde >= 60
                    #endregion

                    #region Epic Mount Drops

                    switch (item.Name)
                    {
                        case "Swift Razzashi Raptor":// Epic Mount Drop from Zul'Gurub
                        case "Swift Zulian Tiger":// Epic Mount Drop from Zul'Gurub
                        case "Deathcharger's Reins":// Epic Mount Drop from Baron Rivendare from UD Strat
                        case "Summon Blue Qiraji Battle Tank":// Epic Mount Drop from Ahn'Qiraj
                        case "Summon Green Qiraji Battle Tank":// Epic Mount Drop from Ahn'Qiraj
                        case "Summon Red Qiraji Battle Tank":// Epic Mount Drop from Ahn'Qiraj
                        case "Summon Yellow Qiraji Battle Tank":// Epic Mount Drop from Ahn'Qiraj
                        case "Summon Black Qiraji Battle Tank":// Prize for ringing Ahn'Qiraj Gong
                            clsPath.StopMoving();
                            Thread.Sleep(500);
                            return item.Use();
                    }

                    // Epic Mount Drops
                    #endregion

                    #region Alliance

                    if (clsCharacter.MyFaction == WoWFactionGroup.Alliance)
                    {
                        switch (item.Name)
                        {
                            //------Dwarf mounts-------
                            case "Black Ram":
                            case "Blue Ram":
                            case "Brown Ram":
                            case "Frost Ram":
                            case "Gray Ram":
                            case "Swift Brown Ram":
                            case "Swift Gray Ram":
                            case "Swift White Ram":
                            case "White Ram":

                            //------Gnome mounts-------
                            case "Blue Mechanostrider":
                            case "Flourescent Green Mechanostrider":
                            case "Green Mechanostrider":
                            case "Icy Mechanostrider":
                            case "Icy Blue Mechanostrider Mod A":
                            case "Purple Mechanostrider":
                            case "Red Mechanostrider":
                            case "Red & Blue Mechanostrider":
                            case "Steel Mechanostrider":
                            case "Swift Green Mechanostrider":
                            case "Swift White Mechanostrider":
                            case "Swift Yellow Mechanostrider":
                            case "Unpainted Mechanostrider":
                            case "White Mechanostrider":
                            case "White Mechanostrider Mod A":

                            //------Human mounts--------
                            case "Black Stallion":
                            case "Brown Horse":
                            case "Chestnut Mare":
                            case "Palomino":
                            case "Palamino Stallion":
                            case "Pinto":
                            case "Pinto Horse":
                            case "Swift White Steed":
                            case "White Stallion":
                            case "Swift Palomino":

                            //------Night elf mounts-------
                            case "Spotted Frostsaber":
                            case "Striped Frostsaber":
                            case "Striped Nightsaber":
                            case "Swift Frostsaber":
                            case "Swift Mistsaber":
                            case "Swift Stormsaber":
                            case "Frost Saber":
                            case "Frostsaber":
                            case "Night Saber":
                            case "Nightsaber":
                            case "Spotted Frost Saber":
                            case "Striped Frost Saber":
                            case "Winterspring Frostsaber":
                                clsPath.StopMoving();
                                Thread.Sleep(500);
                                return item.Use();
                        }
                    }

                    // Alliance
                    #endregion

                    #region Horde

                    switch (item.Name)
                    {
                        //-- Orc mounts
                        case "Arctic Wolf":
                        case "Black Wolf":
                        case "Brown Wolf":
                        case "Dire Wolf":
                        case "Gray Wolf":
                        case "Large Timber Wolf":
                        case "Red Wolf":
                        case "Timber Wolf":
                        case "Winter Wolf":

                        //-- Tauren mounts
                        case "Brown Kodo":
                        case "Gray Kodo":
                        case "Grey Kodo":
                        case "Green Kodo":
                        case "Teal Kodo":
                        case "Great Brown Kodo":
                        case "Great Gray Kodo":
                        case "Great White Kodo":

                        //-- Troll mounts
                        case "Crimson Raptor":
                        case "Emerald Raptor":
                        case "Ivory Raptor":
                        case "Mottled Red Raptor":
                        case "Obsidian Raptor":
                        case "Turquoise Raptor":
                        case "Red Raptor":
                        case "Violet Raptor":
                        case "Swift Blue Raptor":
                        case "Swift Green Raptor":
                        case "Swift Orange Raptor":

                        //   -- Undead mounts
                        case "Skeletal Horse":
                        case "Black Skeletal Warhorse":
                        case "Blue Skeletal Horse":
                        case "Brown Skeletal Horse":
                        case "Green Skeletal Warhorse":
                        case "Purple Skeletal Warhorse":
                        case "Red Skeletal Horse":
                            clsPath.StopMoving();
                            Thread.Sleep(500);
                            return item.Use();
                    }

                    // Horde
                    #endregion

                    #region Misc/Unknown Factions

                    switch (item.Name)
                    {
                        case "Golden Sabercat":
                        case "Tawny Sabercat":
                        case "Leopard":
                        case "Primal Leopard":
                        case "Panther":
                        case "Summon Brown Tallstrider":
                        case "Summon Gray Tallstrider":
                        case "Summon Ivory Tallstrider":
                        case "Summon Pink Tallstrider":
                        case "Summon Purple Tallstrider":
                        case "Summon Turquoise Tallstrider":

                        //-- Christmas mount
                        case "Reindeer":
                            clsPath.StopMoving();
                            Thread.Sleep(500);
                            return item.Use();
                    }

                    // Misc/Unknown Factions
                    #endregion
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, string.Format("FindMount {0}", item != null ? item.Name : string.Empty));
            }

            return false;
        }

        // Mount Up
        #endregion

        #region Dismount

        /// <summary>
        /// Tries to unmount
        /// </summary>
        public static bool Dismount()
        {
            bool rVal = false, PressingX = false;
            WoWBuff wbuff;

            try
            {
                // exit if level low
                using (new clsFrameLock.LockBuffer())
                {
                    if (((clsCharacter.MyClass == WoWClass.Shaman) && (clsCharacter.CurrentLevel < 20)) ||
                        (clsCharacter.CurrentLevel < 40))
                        return true;
                }

                // log it
                clsSettings.Logging.AddToLog(Resources.AttemptingToDismount);

                // if we have ghost wolf, remove it
                if (clsCharacter.MyClass == WoWClass.Shaman)
                {
                    if (clsCharacter.BuffExists("Ghost Wolf"))
                    {
                        clsPath.StopMoving();
                        if ((clsCombat.CastSpell("Ghost Wolf")) && (!clsCharacter.BuffExists("Ghost Wolf")))
                        {
                            clsSettings.Logging.AddToLog(Resources.DismountGhostWolfRemoved);
                            rVal = true;
                            return rVal;
                        }
                    }

                    // still ghost wolf
                    if (clsCharacter.BuffExists("Ghost Wolf"))
                    {
                        using (new clsFrameLock.LockBuffer())
                        {
                            wbuff = clsSettings.isxwow.Me.Buff("Ghost Wolf");

                            if (wbuff.Remove())
                            {
                                clsSettings.Logging.AddToLog(Resources.DismountGhostWolfRemoved);
                                rVal = true;
                            }
                            else
                            {
                                clsSettings.Logging.AddToLog(Resources.CouldNotRemoveGhostWolf);
                                rVal = false;
                            }
                        }

                        Thread.Sleep(300);
                        return rVal;
                    }
                }

                // exit if not mounted
                if (! IsMounted)
                    return rVal;

                // if we are shapeshifted or on summoned mount, revert
                if (!string.IsNullOrEmpty(m_MountSummonSpell))
                {
                    clsPath.StopMoving();
                    if (clsCombat.CastSpell(m_MountSummonSpell))
                    {
                        clsSettings.Logging.AddToLogFormatted(Resources.SpellSummonXsuccessfullyRemoved, m_MountSummonSpell);
                        rVal = true;
                        return rVal;
                    }
                }

                // if we are flying, then get to land first
                if (clsCharacter.IsFlying)
                {
                    clsSettings.Logging.AddToLog(Resources.AttemptingToStopFlying);

                    // press X to go to ground
                    // clsSettings.isxwow.WoWScript("SitStandOrDescendStart()");
                    PressingX = true;
                    LavishScript.ExecuteCommand("wowpress -hold x");

                    DateTime dTime = DateTime.Now;

                    while (clsCharacter.IsFlying)
                    {
                        // if more than 10 seconds, exit
                        if (new TimeSpan(DateTime.Now.Ticks - dTime.Ticks).Seconds > 10)
                        {
                            // release x first
                            PressingX = false;
                            LavishScript.ExecuteCommand("wowpress -release x");

                            // try to dismount anyway
                            clsPath.StopMoving();
                            using (new clsFrameLock.LockBuffer())
                            {
                                if (clsSettings.isxwow.Me.Buff(clsCharacter.MountName).Remove())
                                {
                                    clsSettings.Logging.AddToLogFormatted("Dismount", Resources.RemovedMountBuff);
                                    rVal = true;
                                    return rVal;
                                }
                                else
                                {
                                    clsSettings.Logging.AddToLog(Resources.DismountExitingWaited10Seconds);
                                    return rVal;
                                }
                            }
                        }
                        Thread.Sleep(500);
                    }

                    // release x
                    LavishScript.ExecuteCommand("wowpress -release x");
                }

                // try to remove it as a buff, first
                if (!string.IsNullOrEmpty(clsCharacter.MountName))
                {
                    clsPath.StopMoving();
                    using (new clsFrameLock.LockBuffer())
                    {
                        // try to remove. return if removed
                        if (clsSettings.isxwow.Me.Buff(clsCharacter.MountName).Remove())
                        {
                            clsSettings.Logging.AddToLogFormatted("Dismount", Resources.RemovedMountBuff);
                            rVal = true;
                            return rVal;
                        }
                    }

                    // try to use the item
                    using (new clsFrameLock.LockBuffer())
                    {
                        if (m_MountItem != null)
                        {
                            if (m_MountItem.Use())
                            {
                                clsSettings.Logging.AddToLogFormatted(Resources.RemovedItemBuffX, m_MountItem.Name);
                                rVal = true;
                                return rVal;
                            }
                        }
                    }
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Unable to dismount");
            }

            finally
            {
                // release x
                if (PressingX)
                    LavishScript.ExecuteCommand("wowpress -release x");

                // sleep if dismounted
                if (rVal)
                    Thread.Sleep(200);

                clsSettings.Logging.DebugWrite(string.Format(Resources.DismountReturningX, rVal));
            }

            return rVal;
        }

        // Dismount
        #endregion

        #region Flying

        // http://www.wowwiki.com/Flying_Mount

        /// <summary>
        /// Mount on a flying mount. Returns true on success
        /// </summary>
        internal static bool MountFlying()
        {
            try
            {
                // exit if dead
                if (clsCharacter.IsDead)
                    return true;

                // log it
                clsSettings.Logging.AddToLog(Resources.FindingFlyingMount);

                // if we are less than 60, then we should exit
                if (clsCharacter.CurrentLevel < 60)
                {
                    clsSettings.Logging.AddToLog(Resources.LevelLessThan60ExitingMountFlying);
                    return false;
                }

                // exit if already mounted
                if (IsFlyMounted)
                    return true;

                // reset properties
                m_MountItem = null;
                m_MountSummonSpell = string.Empty;

                // get the items in this character's bag
                List<WoWItem> itemList = clsSearch.Search_Item("-items,-inventory,-usable");

                // if the bags are empty, then exit
                if (itemList.Count == 0)
                {
                    clsSettings.Logging.AddToLog(Resources.NoMountsInBag2);
                    return false;
                }

                // loop through each item to see if it can be mountable. if we mount, then exit
                foreach (WoWItem item in itemList)
                {
                    // check if this is a mount we can use
                    if (FindFlyingMount(item))
                    {
                        // sleep while casting
                        while (clsCharacter.IsCasting)
                            Thread.Sleep(200);

                        // log it
                        clsSettings.Logging.AddToLogFormatted(Resources.UsingFlyingMountX, item.Name);

                        // set property
                        m_MountItem = item;

                        return true;
                    }
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "MountFlying");
            }

            // if we are here, then no mount was found
            clsSettings.Logging.AddToLog(Resources.ExitingMountFlyingNoMountsFound);
            return false;
        }

        /// <summary>
        /// Find a flying mount
        /// </summary>
        /// <param name="item">the item to check</param>
        private static bool FindFlyingMount(WoWItem item)
        {
            try
            {
                using (new clsFrameLock.LockBuffer())
                {
                    string iName = item.Name.ToLower();

                    // return if not a flying mount
                    if ((!iName.Contains("gryphon")) &&
                        (!iName.Contains("windrider")) &&
                        (!iName.Contains("nether ray")) &&
                        (!iName.Contains("nether drake")) &&
                        (!iName.Contains("phoenix")))
                        return false;

                    // try to mount it
                    return item.Use();
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "FindFlyingMount");
            }
            return false;
        }

        // Flying
        #endregion
    }
}
