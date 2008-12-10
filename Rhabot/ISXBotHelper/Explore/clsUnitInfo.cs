using System.Collections.Generic;
using ISXBotHelper.Properties;
using ISXWoW;

namespace ISXBotHelper.Explore
{
    public class clsUnitInfo
    {
        #region Enums

        public enum EUnitType
        {
            Mob = 0,
            Herb = 1,
            Mine = 2,
            Chest = 3,
            RepairVendor = 4,
            Vendor = 5,
            NPC = 6,
            Flightmaster = 7,
            Trainer = 8,
            Mailbox = 9,
            Other = 10,
            Innkeeper = 11,
            Auctioneer = 12,
            Stablemaster = 13,
            EliteMob = 14,
            Banker = 15,
            NONE = 16,
            QuestGiver = 17
        }

        // Enums
        #endregion

        #region Properties

        /// <summary>
        /// The type of object found
        /// </summary>
        public EUnitType UnitType = EUnitType.Other;

        /// <summary>
        /// The unit's name
        /// </summary>
        public string UnitName = string.Empty;

        /// <summary>
        /// The unit's guid
        /// </summary>
        public string UnitGuid = string.Empty;

        /// <summary>
        /// True if npc has quest
        /// </summary>
        public bool HasQuest = false;

        /// <summary>
        /// Unit level
        /// </summary>
        public int UnitLevel = 0;

        // Properties
        #endregion

        #region Init

        /// <summary>
        /// Initializes a new instance of the clsUnitInfo class.
        /// </summary>
        public clsUnitInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the clsUnitInfo class.
        /// </summary>
        /// <param name="unitType"></param>
        public clsUnitInfo(EUnitType unitType, WoWUnit unit)
        {
            UnitType = unitType;

            // get the unit's name
            using (new clsFrameLock.LockBuffer())
            {
                UnitName = unit.Name;
                UnitGuid = unit.GUID;
                UnitLevel = unit.Level;

                // see if quest giver
                List<WoWUnit> questList = clsSearch.Search_Unit(string.Format("-units,-quest,{0}", unit.Name));
                HasQuest = ((questList != null) && (questList.Count > 0));
            }
        }

        /// <summary>
        /// Initializes a new instance of the clsUnitInfo class.
        /// </summary>
        /// <param name="unitType"></param>
        /// <param name="unitName"></param>
        public clsUnitInfo(EUnitType unitType, string unitName, string guid, bool hasquest)
        {
            UnitType = unitType;
            UnitName = unitName;
            UnitGuid = guid;
            HasQuest = hasquest;
        }

        // Init
        #endregion

        #region Functions

        public string UnitTypeString(EUnitType UnitType)
        {
            switch (UnitType)
            {
                case EUnitType.Mob:
                    return "Mob";
                case EUnitType.Herb:
                    return "Herb";
                case EUnitType.Mine:
                    return "Mine";
                case EUnitType.Chest:
                    return "Chest";
                case EUnitType.RepairVendor:
                    return "RepairVendor";
                case EUnitType.Vendor:
                    return Resources.Vendor;
                case EUnitType.NPC:
                    return "NPC";
                case EUnitType.Flightmaster:
                    return "Flightmaster";
                case EUnitType.Trainer:
                    return "Trainer";
                case EUnitType.Mailbox:
                    return "Mailbox";
                case EUnitType.Other:
                    return "Other";
                case EUnitType.Innkeeper:
                    return "Innkeeper";
                case EUnitType.Auctioneer:
                    return "Auctioneer";
                case EUnitType.Stablemaster:
                    return "Stablemaster";
                case EUnitType.EliteMob:
                    return "EliteMob";
                case EUnitType.Banker:
                    return "Banker";
                case EUnitType.NONE:
                default:
                    return "NONE";
                case EUnitType.QuestGiver:
                    return "QuestGiver";
            }
        }

        // Functions
        #endregion
    }
}
