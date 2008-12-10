/*
 * Helper partial class for clsCombat
*/

using System;
using System.Collections.Generic;
using ISXWoW;
using System.Text.RegularExpressions;
using System.Threading;

using ISXRhabotGlobal;

namespace ISXCombat
{
    public partial class clsCombat
    {
        // the instance of ISXWoW
        private ISXWoW.ISXWoW isxwow = new ISXWoW.ISXWoW();
        private WoWMovement movement = null;

        /// <summary>
        /// If the totem does not exist, then drops the totem
        /// </summary>
        /// <param name="totem">the totem to drop</param>
        private void DropTotem(string totem)
        {
            GuidList MyTotems = null;

            // get the list of totems
            using (new clsFrameLock.LockBuffer())
            {
                MyTotems = new GuidList();
                MyTotems.Search("-units", "-totem", "-mine", totem);

                // if the totem exists, exit
                if (MyTotems.Count > 0)
                    return;
            }

            // totem does not exist, drop it
            CastSpell(totem);
        }

        /// <summary>
        /// Applies a buff to the weapon
        /// </summary>
        /// <param name="weapon">the weapon to buff</param>
        /// <param name="BuffName">the buff name</param>
        private void ApplyWeaponBuff(WoWItem weapon, string BuffName)
        {
            bool WeapHasBuff = false;
            int i = 0;

            try
            {
                using (new clsFrameLock.LockBuffer())
                {
                    // exit if the weapon is invalid
                    if ((weapon == null) || (!weapon.IsValid))
                    {
                        clsGlobals.AddStringToLog("ApplyWeaponBuff: Weapon is invalid");
                        return;
                    }

                    // exit if the item is not a weapon
                    if (!weapon.Class.ToLower().Contains("weapon"))
                    {
                        clsGlobals.AddStringToLog(string.Format("ApplyWeaponBuff: Weapon '{0}' is not a weapon. Class = {1}", weapon.FullName, weapon.Class));
                        return;
                    }

                    // loop through enchantments to find flametongue
                    for (i = 0; i < weapon.EnchantmentCount; i++)
                    {
                        // set if the weapon has the buff
                        if ((weapon.Enchantment(i).IsValid) && (weapon.Enchantment(i).Name.Contains(BuffName)))
                            return;
                    }
                }

                // if the weapon does not have the buff, apply it now
                if ((!WeapHasBuff) && (CastSpell(BuffName)))
                    //if (!weapon.PickUp()) // log if we can't apply the buff
                    clsGlobals.AddStringToLog(string.Format("BeginCombat: Could not buff weapon '{0}' with '{1}'",
                        weapon.FullName, BuffName));
            }

            catch (Exception excep)
            {
                clsGlobals.AddStringToLog(string.Format("ApplyWeaponBuff: ERROR\r\n{0}", excep.Message));
            }

            finally
            {

            }
        }

        /// <summary>
        /// Face this X,Y
        /// </summary>
        private void Face(double X, double Y)
        {
            using (new clsFrameLock.LockBuffer())
                isxwow.Face(X, Y);
        }

        private double Distance(WoWUnit TargetUnit)
        {
            using (new clsFrameLock.LockBuffer())
                return Distance(isxwow.Me.Location.X, isxwow.Me.Location.Y, TargetUnit.Location.X, TargetUnit.Location.Y);
        }

        /// <summary>
        /// Returns distance in yards
        /// </summary>
        private double Distance(double SourceX, double SourceY, double DestX, double DestY)
        {
            return Math.Sqrt(Math.Pow(SourceX - DestX, 2) + Math.Pow(SourceY - DestY, 2));
        }

        /// <summary>
        /// Faces unit, then moves towards it if too far away
        /// </summary>
        /// <param name="UnitToAttack">the unit to face/attack</param>
        private void MoveAndFace(WoWUnit UnitToAttack)
        {
            // target if not alrady targetting
            if (UnitToAttack.GUID != isxwow.Me.CurrentTarget.GUID)
            {
                UnitToAttack.Target();
                UnitToAttack.SpellTarget();
            }

            // move to within 5 yards of the target
            MoveToTarget(UnitToAttack, 5);
        }

        /// <summary>
        /// Moves to target.
        /// </summary>
        /// <param name="Distance">The distance to be from target before stopping.</param>
        /// <param name="TargetUnit"></param>
        public void MoveToTarget(WoWUnit TargetUnit, int distance)
        {
            // move closer to the unit, if too far away
            while (Distance(TargetUnit) > distance)
            {
                // face the mob
                using (new clsFrameLock.LockBuffer())
                    Face(TargetUnit.Location.X, TargetUnit.Location.Y);

                // delay
                Thread.Sleep(100);
            }
        }

        /// <summary>
        /// Returns true if we are in combat (attacking or being attacked)
        /// </summary>
        private bool IsInCombat()
        {
            return ((isxwow.Me.IsInCombat) || (isxwow.Me.Attacking));
        }

        /// <summary>
        /// Casts a spell. You must already have something targetted. Returns when spellcast is complete
        /// returns false if spell can't be cast
        /// </summary>
        /// <param name="SpellName">the spell to cast</param>
        private bool CastSpell(string SpellName)
        {
            bool rVal = false;
            WoWSpell wSpell = null;
            DateTime CastTime = DateTime.MinValue;
            WoWUnit currTarget = null;

            try
            {
                // exit if no spell
                if (string.IsNullOrEmpty(SpellName))
                    return false;

                // face the target
                using (new clsFrameLock.LockBuffer())
                {
                    currTarget = isxwow.Me.CurrentTarget;

                    if ((currTarget != null) && (currTarget.IsValid) && (currTarget.GUID != isxwow.Me.GUID))
                        Face(currTarget.X, currTarget.Y);
                }

                // get the spell
                using (new clsFrameLock.LockBuffer())
                {
                    wSpell = WoWSpell.Get(SpellName);

                    // check if valid spell
                    if ((wSpell == null) || (!wSpell.IsValid))
                    {
                        clsGlobals.AddStringToLog(string.Format("CastSpell: Exiting. Invalid spell. Recheck spelling of spell name '{0}'", SpellName));
                        return false;
                    }

                    // exit if not usable or cost too high
                    if (wSpell.PowerCost > CurrentPower())
                    {
                        clsGlobals.AddStringToLog(string.Format("Spell '{0}' costs too much. Exiting CastSpell", SpellName));
                        return false;
                    }
                }

                // cast it
                clsGlobals.AddStringToLog(string.Format("CastSpell: Casting '{0}'", SpellName));
                rVal = CastSpell(wSpell);
            }

            catch (Exception excep)
            {
                clsGlobals.AddStringToLog(string.Format("CastSpell: ERROR\r\n{0}", excep.Message));
            }

            return rVal;
        }

        /// <summary>
        /// Casts a prepared spell
        /// </summary>
        /// <param name="wSpell">the spell to cast (MUST NOT BE NULL)</param>
        /// <returns></returns>
        private bool CastSpell(WoWSpell wSpell)
        {
            bool GoodCast = false;

            // cast it
            using (new clsFrameLock.LockBuffer())
                GoodCast = wSpell.Cast();
            if (!GoodCast)
                return false;

            // loop until spell is cast
            while (MeIsCasting)
                System.Threading.Thread.Sleep(200);

            int cTime = 0;

            // keep sleeping until we finish the spell cooldown
            using (new clsFrameLock.LockBuffer())
            {
                // get time remaining to wait
                if (wSpell.CastTime <= 1500)
                    cTime = 1500 - wSpell.CastTime;
            }

            // sleep until spell cast has cooled down
            if (cTime > 0)
                System.Threading.Thread.Sleep(cTime);

            return true;
        }

        /// <summary>
        /// Returns isxwow.Me.IsCasting
        /// </summary>
        public bool MeIsCasting
        {
            get
            {
                using (new clsFrameLock.LockBuffer())
                    return isxwow.Me.IsCasting;
            }
        }

        /// <summary>
        /// Returns a result if the character or mob is dead
        /// </summary>
        private clsGlobals.DeadUnitEnum CheckDead(WoWUnit Target)
        {
            clsGlobals.DeadUnitEnum due = clsGlobals.DeadUnitEnum.Neither;

            using (new clsFrameLock.LockBuffer())
            {
                // get mob death
                if (Target.Dead)
                    return clsGlobals.DeadUnitEnum.Mob;

                // get character death
                if (isxwow.Me.Dead)
                    return clsGlobals.DeadUnitEnum.Character;
            }

            // return result
            return due;
        }

        /// <summary>
        /// Returns true if the unit has the specified buff
        /// </summary>
        private bool UnitHasBuff(WoWUnit UnitToAttack, string BuffName)
        {
            // get the buff
            using (new clsFrameLock.LockBuffer())
            {
                WoWBuff wbuff = UnitToAttack.Buff(BuffName);

                // return it's validity
                return ((wbuff != null) && (wbuff.IsValid));
            }
        }

        private void ShowError(Exception excep)
        {
            clsGlobals.AddStringToLog(string.Format("ERROR:\r\n{0}", excep.Message));
        }

        /// <summary>
        /// Returns true if the buff exists on the player
        /// </summary>
        /// <param name="BuffName">the buff to find</param>
        private bool BuffExists(string BuffName)
        {
            return BuffExists(isxwow.Me, BuffName);
        }

        /// <summary>
        /// Returns true if the buff exists on the unit
        /// </summary>
        /// <param name="TargetUnit">the unit to check</param>
        /// <param name="BuffName">the buff to find</param>
        public static bool BuffExists(WoWUnit TargetUnit, string BuffName)
        {
            WoWBuff buff;

            using (new clsFrameLock.LockBuffer())
            {
                buff = TargetUnit.Buff(BuffName);
                return ((buff != null) && (buff.IsValid));
            }
        }

        #region Item Search

        /// <summary>
        /// Returns one item from your bag that is usable and has the highest level or returns null
        /// </summary>
        /// <param name="searchString">the item to search fo</param>
        private WoWItem FindItem(string searchString)
        {
            WoWItem tempItem = null;
            int CurrLevel = isxwow.Me.Level;

            try
            {
                // get the item list
                List<WoWItem> itemList = Search_Item(searchString);

                // if nothing return, exit
                if (itemList.Count == 0)
                    return null;

                // if only one item, return it
                if (itemList.Count == 1)
                    return itemList[0];

                // find the item with the highest minimum level
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

            catch (Exception excep)
            {
                ShowError(excep);
            }

            return tempItem;
        }

        /// <summary>
        /// Searches searchString for items
        /// </summary>
        /// <param name="searchString">the string to search</param>
        private List<WoWItem> Search_Item(string searchString)
        {
            List<WoWItem> retList = new List<WoWItem>();
            int gCount = 0;
            WoWItem tempItem = null;

            try
            {
                // get the list of items
                GuidList gl = null;
                using (new clsFrameLock.LockBuffer())
                    gl = GuidList.New(Regex.Split(searchString, ","));

                // get the count
                gCount = (int)gl.Count;

                // if nothing found, then return
                if (gCount == 0)
                    return retList;

                // pop retList
                for (int i = 0; i < gCount; i++)
                {
                    using (new clsFrameLock.LockBuffer())
                        tempItem = gl.Object((uint)i).GetItem();
                    if ((tempItem != null) && (tempItem.IsValid))
                        retList.Add(tempItem);
                }
            }

            catch (Exception excep)
            {
                // log error
                ShowError(excep);
            }

            // return the list
            return retList;
        }

        private int GetItemCooldown(WoWItem item)
        {
            try
            {
                // exit if item is invalid
                if ((item == null) || (!item.IsValid))
                    return 0;

                // get the cooldown
                string coolDown = isxwow.WoWScript<string>(string.Format("GetContainerItemCooldown({0}, {1})", item.Bag.Number.ToString().Trim(), item.Bag.Slot.ToString().Trim()), 1);

                // return 0 if null or empty or not numeric
                if (string.IsNullOrEmpty(coolDown))
                    return 0;

                // return the cooldown
                return Convert.ToInt32(coolDown);
            }

            catch (Exception excep)
            {
                ShowError(excep);
            }

            return 0;
        }

        // Item Search
        #endregion

        #region Power and Power Type

        // code by Loop - http://www.isxwow.net/forums/viewtopic.php?f=15&t=1465
        private enum powerType { Rage, Energy, Mana };
        private powerType GetPowerType()
        {
            int mE = isxwow.Me.MaxEnergy;
            int mR = isxwow.Me.MaxRage;
            int mM = isxwow.Me.MaxMana;

            if ((mE >= mR) && (mE >= mM))
                return powerType.Energy;

            if ((mR >= mE) && (mR >= mM))
                return powerType.Rage;

            if ((mM >= mE) && (mM >= mR))
                return powerType.Mana;

            return powerType.Mana;
        }

        /// <summary>
        /// Returns the current amount of power (mana,rage, energy)
        /// </summary>
        private int CurrentPower()
        {
            switch (GetPowerType())
            {
                case powerType.Energy:
                    return isxwow.Me.Energy;
                case powerType.Rage:
                    return isxwow.Me.Rage;
                case powerType.Mana:
                default:
                    return isxwow.Me.Mana;
            }
        }

        // Power and Power Type
        #endregion

        #region Potions

        /// <summary>
        /// Finds the best healing potion in inventory and uses. Returns false on error
        /// </summary>
        private bool DrinkBestHealingPotion()
        {
            try
            {
                if (UsePotion("Super Rejuvenation Potion"))
                    return true;
                if (UsePotion("Major Combat Healing Potion"))
                    return true;
                if (UsePotion("Super Healing Potion"))
                    return true;
                if (UsePotion("Major Rejuvenation Potion"))
                    return true;
                if (UsePotion("Volatile Healing Potion"))
                    return true;
                if (UsePotion("Major Healing Potion"))
                    return true;
                if (UsePotion("Combat Healing Potion"))
                    return true;
                if (UsePotion("Superior Healing Potion"))
                    return true;
                if (UsePotion("Greater Healing Potion"))
                    return true;
                if (UsePotion("Healing Potion"))
                    return true;
                if (UsePotion("Discolored Healing Potion"))
                    return true;
                if (UsePotion("Lesser Healing Potion"))
                    return true;
                if (UsePotion("Minor Healing Potion"))
                    return true;
            }

            catch (Exception excep)
            {
                ShowError(excep);
            }

            return false;
        }

        /// <summary>
        /// Finds the best mana potion in inventory and uses. Returns true if the potion was used
        /// </summary>
        private bool DrinkBestManaPotion()
        {
            try
            {
                if (UsePotion("Major Combat Mana Potion"))
                    return true;
                if (UsePotion("Super Mana Potion"))
                    return true;
                if (UsePotion("Volatile Mana Potion"))
                    return true;
                if (UsePotion("Major Mana Potion"))
                    return true;
                if (UsePotion("Combat Mana Potion"))
                    return true;
                if (UsePotion("Superior Mana Potion"))
                    return true;
                if (UsePotion("Greater Mana Potion"))
                    return true;
                if (UsePotion("Mana Potion"))
                    return true;
                if (UsePotion("Lesser Mana Potion"))
                    return true;
                if (UsePotion("Minor Mana Potion"))
                    return true;
            }

            catch (Exception excep)
            {
                ShowError(excep);
            }

            return false;
        }

        /// <summary>
        /// Uses the potion if it exists. Returns true if used, else returns false
        /// </summary>
        /// <param name="PotionName"></param>
        /// <returns></returns>
        private bool UsePotion(string PotionName)
        {
            bool rVal = false;

            try
            {
                // find the item
                WoWItem tempItem = FindItem(string.Format("-inventory, {0}", PotionName));

                using (new clsFrameLock.LockBuffer())
                    rVal = ((tempItem != null) && (tempItem.IsValid));

                // if something returned, then use it
                if (rVal)
                {
                    // use the potion
                    using (new clsFrameLock.LockBuffer())
                        rVal = tempItem.Use();

                    // if used, then wait for half a second
                    if (rVal)
                        System.Threading.Thread.Sleep(500);

                    return rVal;
                }
            }

            catch (Exception excep)
            {
                ShowError(excep);
            }

            // since we are here, we couldn't have used the item
            return false;
        }

        // Potions
        #endregion

        #region Bandages

        /// <summary>
        /// Uses the best bandage. Returns true on success, otherwise false
        /// </summary>
        private bool UseBestBandage()
        {
            try
            {
                if (UseBandage("Heavy Netherweave Bandage"))
                    return true;
                if (UseBandage("Netherweave Bandage"))
                    return true;
                if (UseBandage("Heavy Runecloth Bandage"))
                    return true;
                if (UseBandage("Runecloth Bandage"))
                    return true;
                if (UseBandage("Heavy Mageweave Bandage"))
                    return true;
                if (UseBandage("Mageweave Bandage"))
                    return true;
                if (UseBandage("Heavy Silk Bandage"))
                    return true;
                if (UseBandage("Silk Bandage"))
                    return true;
                if (UseBandage("Heavy Linen Bandage"))
                    return true;
                if (UseBandage("Linen Bandage"))
                    return true;
            }

            catch (Exception excep)
            {
                ShowError(excep);
            }

            return false;
        }

        /// <summary>
        /// Uses the bandage if it exists. Returns true if used, else returns false
        /// </summary>
        private bool UseBandage(string BandageName)
        {
            bool rVal = false;
            bool GoodBandage = false;

            try
            {
                // exit if we have the bandage debuff
                using (new clsFrameLock.LockBuffer())
                {
                    if (UnitHasBuff(isxwow.Me, "Recently Bandaged"))
                        return true;
                }

                // find the item
                WoWItem tempItem = FindItem(string.Format("-inventory, {0}", BandageName));

                using (new clsFrameLock.LockBuffer())
                    GoodBandage = ((tempItem != null) && (tempItem.IsValid));

                // if something returned, then use it
                if (GoodBandage)
                {
                    // use the potion
                    using (new clsFrameLock.LockBuffer())
                        rVal = tempItem.Use();

                    // sleep while casting
                    while (MeIsCasting)
                        System.Threading.Thread.Sleep(200);

                    // if used, then wait for two seconds
                    if (rVal)
                        System.Threading.Thread.Sleep(2000);

                    return rVal;
                }
            }

            catch (Exception excep)
            {
                ShowError(excep);
            }

            // since we are here, we couldn't have used the item
            return false;
        }

        // Bandages
        #endregion

        #region Units Attacking

        /// <summary>
        /// returns the number of units attacking me
        /// </summary>
        private int NumUnitsAttackingMe()
        {
            // get the list of attackers
            using (new clsFrameLock.LockBuffer())
            {
                GuidList myAttackers = new GuidList();
                myAttackers.Search("-units", "-targetingme", "-aggro");
                return (int)myAttackers.Count;
            }
        }

        /// <summary>
        /// returns a list of units attacking me
        /// </summary>
        private List<WoWUnit> UnitsAttackingMe()
        {
            GuidList myAttackers = null;
            WoWUnit tempUnit = null;
            List<WoWUnit> retList = new List<WoWUnit>();

            // get the list of attackers
            try
            {
                using (new clsFrameLock.LockBuffer())
                {
                    // get the list
                    myAttackers = new GuidList();
                    myAttackers.Search("-units", "-targetingme", "-aggro");

                    int SearchCount = (int)myAttackers.Count;

                    // loop through and add to return list
                    for (int UnitCounter = 0; UnitCounter < SearchCount; UnitCounter++)
                    {
                        // get the unit
                        tempUnit = myAttackers.Object((uint)UnitCounter).GetUnit();

                        // make sure it's valid
                        if ((tempUnit == null) || (!tempUnit.IsValid))
                            continue;

                        // add unit to the list 
                        retList.Add(tempUnit);
                    }
                }

                // if the list is empty, add an empty element
                if (retList.Count == 0)
                    retList = new List<WoWUnit>();
            }

            catch (Exception excep)
            {
                ShowError(excep);
            }

            // return the list
            return retList;
        }

        /// <summary>
        /// Returns one unit attacking me, or null
        /// </summary>
        private WoWUnit GetUnitAttackingMe()
        {
            try
            {
                // get attacking units
                List<WoWUnit> units = UnitsAttackingMe();

                // if unit found, then return it
                using (new clsFrameLock.LockBuffer())
                {
                    if ((units != null) && (units.Count > 0))
                        return units[0];
                }
            }

            catch (Exception excep)
            {
                ShowError(excep);
            }


            // no units, or error
            return null;
        }

        // Units Attacking
        #endregion
    }
}
