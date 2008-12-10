using System;
using System.Collections.Generic;
using System.Threading;
using ISXBotHelper.Properties;
using ISXRhabotGlobal;
using ISXWoW;

namespace ISXBotHelper.Items
{
    public class clsAutoEquip
    {
        /// <summary>
        /// Checks your bag to see if items meet new equip requirements
        /// </summary>
        public static void CheckNeedEquipItems()
        {
            int myLevel = clsCharacter.CurrentLevel;

            try
            {
                // skip if invalid or not full version
                if ((!clsCharacter.CharacterIsValid) || (!clsSettings.IsFullVersion))
                    return;

                clsSettings.Logging.AddToLog(Resources.CheckNeedEquipItems);

                // exit if we have no rules established
                if ((clsSettings.gclsGlobalSettings.AutoEquipList == null) || (clsSettings.gclsGlobalSettings.AutoEquipList.Count == 0))
                {
                    clsSettings.Logging.AddToLog(Resources.AutoEquip, Resources.ExitingNoRules);
                    return;
                }

                // get the item list
                List<WoWItem> itemList = clsCharacter.GetBagItems();

                // loop through the item list to see what is equipable
                foreach (WoWItem item in itemList)
                {
                    // skip if not equipable or not usable
                    if ((! item.IsValid) || (!item.Usable) || (item.MinLevel > myLevel) || (string.IsNullOrEmpty(item.EquipType)))
                        continue;

                    // loop through the rule list to see if this item matches a rule
                    foreach (clsAutoEquipItem equipRule in clsSettings.gclsGlobalSettings.AutoEquipList)
                    {
                        switch (equipRule.EquipSlot)
                        {
                            case WoWEquipSlot.Back:
                            case WoWEquipSlot.Chest:
                            case WoWEquipSlot.Feet:
                            case WoWEquipSlot.Finger1:
                            case WoWEquipSlot.Finger2:
                            case WoWEquipSlot.Hands:
                            case WoWEquipSlot.Head:
                            case WoWEquipSlot.Legs:
                            case WoWEquipSlot.MainHand:
                            case WoWEquipSlot.Neck:
                            case WoWEquipSlot.OffHand:
                            case WoWEquipSlot.Ranged:
                            case WoWEquipSlot.Shoulders:
                            case WoWEquipSlot.Trinket1:
                            case WoWEquipSlot.Trinket2:
                            case WoWEquipSlot.Waist:
                            case WoWEquipSlot.Wrists:

                                // skip if not back or stats don't match
                                if ((!equipRule.EquipSlot.ToString().Contains(item.EquipType)) || (!StatsMatch(item, equipRule)))
                                    continue;

                                // stats match, equip this item
                                clsSettings.Logging.AddToLogFormatted(Resources.AutoEquip, Resources.FoundMatchEquippingX, item.Name);
                                EquipItem(item);
                                break;
                        }
                    }
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Check Need Equip Item");
            }
        }

        /// <summary>
        /// Checks if this item matchs our equip rule. Returns true if we can equip the item
        /// </summary>
        /// <param name="item">the item to check</param>
        /// <param name="equipRule">the rule to use</param>
        private static bool StatsMatch(WoWItem item, clsAutoEquipItem equipRule)
        {
            // exit if the rule is none
            if (equipRule.EquipStat == clsGlobals.ENeedEquipStat.None)
                return false;

            // exit invalid material type
            if (equipRule.MaterialType == clsGlobals.EEquipItemMaterialType.None)
            {
                switch (equipRule.EquipSlot)
                {
                    case WoWEquipSlot.Back:
                    case WoWEquipSlot.Chest:
                    case WoWEquipSlot.Feet:
                    case WoWEquipSlot.Hands:
                    case WoWEquipSlot.Head:
                    case WoWEquipSlot.Legs:
                    case WoWEquipSlot.Shoulders:
                    case WoWEquipSlot.Waist:
                    case WoWEquipSlot.Wrists:
                        return false;
                }
            }

            // skip if not correct cloth type
            if ((equipRule.MaterialType != clsGlobals.EEquipItemMaterialType.None) && (item.SubType != equipRule.MaterialType.ToString().Trim()))
                return false;

            // if we are not wearing an item in that slot, then use this one instead
            WoWItem equipItem = clsSettings.isxwow.Me.Equip(equipRule.EquipSlot);
            if ((equipItem == null) || (!equipItem.IsValid))
                return true;

            // see if the stats match. if both items do not have the stat, compare armor
            switch (equipRule.EquipStat)
            {
                case clsGlobals.ENeedEquipStat.Agility:
                    return 
                        ((item.Agility <= 0) && 
                            (equipItem.Agility <= 0) && 
                            (equipItem.Armor < item.Armor)) ||
                        equipItem.Agility < item.Agility;
                case clsGlobals.ENeedEquipStat.ArcaneResist:
                    return
                        ((item.ArcaneResist <= 0) && 
                            (equipItem.ArcaneResist <= 0) && 
                            (equipItem.Armor < item.Armor)) ||
                        equipItem.ArcaneResist < item.ArcaneResist;
                case clsGlobals.ENeedEquipStat.Armor:
                    return equipItem.Armor < item.Armor;
                case clsGlobals.ENeedEquipStat.AttackPower:
                    return
                        ((item.AttackPower <= 0) &&
                            (equipItem.AttackPower <= 0) &&
                            (equipItem.Armor < item.Armor)) ||
                        equipItem.AttackPower < item.AttackPower;
                case clsGlobals.ENeedEquipStat.DPS:
                    return
                        ((item.Stats.DPSTotal <= 0) &&
                            (equipItem.Stats.DPSTotal <= 0) &&
                            (equipItem.Armor < item.Armor)) ||
                        equipItem.Stats.DPSTotal < item.Stats.DPSTotal;
                case clsGlobals.ENeedEquipStat.FireResist:
                    return
                        ((item.FireResist <= 0) &&
                            (equipItem.FireResist <= 0) &&
                            (equipItem.Armor < item.Armor)) ||
                        equipItem.FireResist < item.FireResist;
                case clsGlobals.ENeedEquipStat.FrostResist:
                    return
                        ((item.FrostResist <= 0) &&
                            (equipItem.FrostResist <= 0) &&
                            (equipItem.Armor < item.Armor)) ||
                        equipItem.FrostResist < item.FrostResist;
                case clsGlobals.ENeedEquipStat.HolyResist:
                    return
                        ((item.HolyResist <= 0) &&
                            (equipItem.HolyResist <= 0) &&
                            (equipItem.Armor < item.Armor)) ||
                        equipItem.HolyResist < item.HolyResist;
                case clsGlobals.ENeedEquipStat.Intellect:
                    return
                        ((item.Intellect <= 0) &&
                            (equipItem.Intellect <= 0) &&
                            (equipItem.Armor < item.Armor)) ||
                        equipItem.Intellect < item.Intellect;
                case clsGlobals.ENeedEquipStat.NatureResist:
                    return
                        ((item.NatureResist <= 0) &&
                            (equipItem.NatureResist <= 0) &&
                            (equipItem.Armor < item.Armor)) ||
                        equipItem.NatureResist < item.NatureResist;
                case clsGlobals.ENeedEquipStat.ShadowResist:
                    return
                        ((item.ShadowResist <= 0) &&
                            (equipItem.ShadowResist <= 0) &&
                            (equipItem.Armor < item.Armor)) ||
                        equipItem.ShadowResist < item.ShadowResist;
                case clsGlobals.ENeedEquipStat.Spirit:
                    return
                        ((item.Spirit <= 0) &&
                            (equipItem.Spirit <= 0) &&
                            (equipItem.Armor < item.Armor)) ||
                        equipItem.Spirit < item.Spirit;
                case clsGlobals.ENeedEquipStat.Stamina:
                    return
                        ((item.Stamina <= 0) &&
                            (equipItem.Stamina <= 0) &&
                            (equipItem.Armor < item.Armor)) ||
                        equipItem.Stamina < item.Stamina;
                case clsGlobals.ENeedEquipStat.Strength:
                    return
                        ((item.Strength <= 0) &&
                            (equipItem.Strength <= 0) &&
                            (equipItem.Armor < item.Armor)) ||
                        equipItem.Strength < item.Strength;
            }

            // default to not wearable
            return false;
        }

        /// <summary>
        /// Equips the item and logs the information
        /// </summary>
        /// <param name="item"></param>
        private static void EquipItem(WoWItem item)
        {
            clsSettings.Logging.AddToLogFormatted(Resources.AutoEquip, Resources.FoundMatchingItem, item.Name, item.EquipType);
            using (new clsFrameLock.LockBuffer())
                item.Use();
            Thread.Sleep(200);

            // confirm boe
            clsSettings.ExecuteWoWAPI("ConfirmBindOnUse()");
        }
    }
}
