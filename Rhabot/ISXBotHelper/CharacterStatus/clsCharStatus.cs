using System;
using System.Collections.Generic;
using System.Text;
using ISXWoW;
using LavishVMAPI;
using LavishScriptAPI;

namespace ISXBotHelper.CharacterStatus
{
    public class clsCharStatus
    {
        // TODO: current state (looting, vending, training, running, etc)

        #region Properties

        /// <summary>
        /// Number of kills to level
        /// </summary>
        public int KillsToLevel = -1;

        /// <summary>
        /// Set to true when we are stuck
        /// </summary>
        public bool IsStuck = false;

        /// <summary>
        /// Number of times we died
        /// </summary>
        public int NumTimesDied = 0;

        /// <summary>
        /// Average number of mobs attacking me at once
        /// </summary>
        public double AvgMobCountOnMe = 0;

        /// <summary>
        /// List of items found in our bag
        /// </summary>
        public List<BagItemInfo> ItemsInBag = new List<BagItemInfo>();

        /// <summary>
        /// List of skills
        /// </summary>
        public List<clsSkillInfo> Skills = new List<clsSkillInfo>();

        /// <summary>
        /// List of equipped items
        /// </summary>
        public List<EquipItemInfo> EquipItemList = new List<EquipItemInfo>();

        #region Stats

        /// <summary>
        /// Percent chance to block
        /// </summary>
        public double BlockPercent = 0;

        /// <summary>
        /// Class of this character
        /// </summary>
        public string Class = string.Empty;

        /// <summary>
        /// The amount of copper you have
        /// </summary>
        public double Copper = 0;

        /// <summary>
        /// Percent chance to crit
        /// </summary>
        public double CritPercent = 0;

        /// <summary>
        /// Percent chance to dodge
        /// </summary>
        public double DodgePercent = 0;

        /// <summary>
        /// Durability Percent
        /// </summary>
        public double DurabilityPercent = 100;

        /// <summary>
        /// Health Percent
        /// </summary>
        public double HealthPercent = 100;

        /// <summary>
        /// Returns true if you are underwater and holding your breath
        /// </summary>
        public bool HoldingBreath = false;

        /// <summary>
        /// True if we are currently dead
        /// </summary>
        public bool IsDead = false;

        /// <summary>
        /// True if swimming
        /// </summary>
        public bool IsSwimming = false;

        /// <summary>
        /// Current Level
        /// </summary>
        public int Level = 1;

        public double LocationX = 0;
        public double LocationY = 0;
        public double LocationZ = 0;

        /// <summary>
        /// Character's name
        /// </summary>
        public string Name = string.Empty;

        /// <summary>
        /// Amount of XP needed to level
        /// </summary>
        public int NextLevelExp = 0;

        /// <summary>
        /// True if PVP Flagged
        /// </summary>
        public bool PvPFlagged = false;

        /// <summary>
        /// Character's race
        /// </summary>
        public string Race = string.Empty;

        /// <summary>
        /// Amount of rest XP you have
        /// </summary>
        public int RestedExp = 0;

        /// <summary>
        /// True if we are sitting
        /// </summary>
        public bool Sitting = false;

        /// <summary>
        /// True if stealthed
        /// </summary>
        public bool Stealthed = false;

        // Stats
        #endregion

        #region CharInfo2

        /// <summary>
        /// List of equiped items
        /// </summary>
        public List<BagItemInfo> EquipedItems = new List<BagItemInfo>();

        /// <summary>
        /// Time bot has been running, in seconds
        /// </summary>
        public int BotRunTimeSeconds = 0;

        // CharInfo2
        #endregion

        // Properties
        #endregion

        #region Classes

        [Serializable]
        public class clsTargetInfo
        {
            public string TargetName { get; set; }
            public int TargetLevel { get; set; }
            public bool IsElite { get; set; }
            public double HPPct { get; set; }
        }

        [Serializable]
        public class clsMsgInfo
        {
            public string Author = string.Empty;
            public string Message = string.Empty;
            public DateTime MsgDate = DateTime.Now;

            /// <summary>
            /// Initializes a new instance of the clsMsgInfo class.
            /// </summary>
            /// <param name="author"></param>
            /// <param name="message"></param>
            public clsMsgInfo(string author, string message)
            {
                Author = author;
                Message = message;
            }
        }

        [Serializable]
        public class clsSkillInfo
        {
            public int SkillLevel = 0;
            public string SkillName = string.Empty;

            /// <summary>
            /// Initializes a new instance of the clsSkillInfo class.
            /// </summary>
            /// <param name="skillLevel"></param>
            /// <param name="skillName"></param>
            public clsSkillInfo(int skillLevel, string skillName)
            {
                SkillLevel = skillLevel;
                SkillName = skillName;
            }
        }

        public class BagItemInfo
        {
            public int Quantity = 0;
            public string ItemName = string.Empty;
            public int ItemLevel = 0;

            public BagItemInfo(WoWItem item)
            {
                using (new clsFrameLock.LockBuffer())
                {
                    if ((item != null) && (item.IsValid))
                    {
                        Quantity = item.StackCount;
                        ItemName = item.FullName;
                        ItemLevel = item.ItemLevel;
                    }
                }
            }
        }

        public class EquipItemInfo
        {
            public WoWEquipSlot EquippedSlot = WoWEquipSlot.Bag1;
            public string ItemName = string.Empty;
            public int ItemLevel = 0;
            public int Durability = 0;

            /// <summary>
            /// Initializes a new instance of the EquipItemInfo class.
            /// </summary>
            public EquipItemInfo(WoWEquipSlot equippedSlot, WoWItem item)
            {
                using (new clsFrameLock.LockBuffer())
                {
                    if ((item != null) && (item.IsValid))
                    {
                        EquippedSlot = equippedSlot;
                        ItemName = item.Name;
                        ItemLevel = item.MinLevel;
                        Durability = item.Durability;
                    }
                }
            }
        }

        // Classes
        #endregion
    }
}
