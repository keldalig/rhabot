using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ISXRhabotGlobal
{
    public class clsGlobals
    {
        #region Enums

        /// <summary>
        /// Outcomes for combat
        /// </summary>
        public enum AttackOutcome
        {
            /// <summary>
            /// user killed monster
            /// </summary>
            Success = 0,

            /// <summary>
            /// user died
            /// </summary>
            Dead,

            /// <summary>
            /// panic mode, try to escape or other panic options
            /// NOTE: combat mode exits. You need to run or take other measures
            /// </summary>
            Panic,

            /// <summary>
            /// unit being attacked is bugged. disengage and blacklist (Rhabot will blacklist for you)
            /// </summary>
            Bugged,

            /// <summary>
            /// Script stopped by user
            /// </summary>
            Stopped
        }

        /// <summary>
        /// Specifies which unit(s) are dead
        /// </summary>
        public enum DeadUnitEnum
        {
            Character = 0,
            Mob,
            Neither
        }

        // Enums
        #endregion

        #region AutoEquip Enums

        public enum ENeedEquipStat
        {
            None = 0,
            Armor,
            Intellect,
            Stamina,
            Agility,
            Strength,
            DPS,
            Spirit,
            AttackPower,
            FireResist,
            ArcaneResist,
            FrostResist,
            HolyResist,
            NatureResist,
            ShadowResist
        }

        public enum EEquipItemMaterialType
        {
            None = 0,
            Cloth,
            Leather,
            Mail,
            Plate
        }

        // AutoEquip Enums
        #endregion

        #region Combat Class Enums

        public enum ECombatClass_Druid_Form
        {
            Humanoid = 0,
            Bear,
            Cat,
            DireBear,
            MoonKin
        }

        public enum ECombatClass_Warlock_Pet
        {
            Imp = 0,
            Voidwalker,
            Succubus,
            Felhunter,
            Felguard
        }

        // Combat Class Enums
        #endregion

        #region ZoneName_Enums

        public enum EZoneName
        {
            NONE = 0,
            Alterac_Mountains = 26,
            Arathi_Highlands = 27,
            Ashenvale = 2,
            Azshara = 3,
            Azuremyst_Isle = 4,
            Badlands = 28,
            Blades_Edge_Mountains = 54,
            Blasted_Lands = 29,
            Bloodmyst_Isle = 5,
            Burning_Steppes = 30,
            Darkshore = 6,
            Darnassus = 7,
            Deadwind_Pass = 31,
            Desolace = 8,
            Dun_Morogh = 32,
            Durotar = 9,
            Duskwood = 33,
            Dustwallow_Marsh = 10,
            Eastern_Plaguelands = 34,
            Elwynn_Forest = 35,
            Eversong_Woods = 36,
            Felwood = 11,
            Feralas = 12,
            Ghostlands = 37,
            Hellfire_Peninsula = 55,
            Hillsbrad_Foothills = 38,
            Ironforge = 39,
            Loch_Modan = 40,
            Moonglade = 13,
            Mulgore = 14,
            Nagrand = 56,
            Netherstorm = 57,
            Orgrimmar = 15,
            Redridge_Mountains = 41,
            Searing_Gorge = 42,
            Shadowmoon_Valley = 58,
            Shattrath_City = 59,
            Silithus = 16,
            Silvermoon_City = 43,
            Silverpine_Forest = 44,
            Stonetalon_Mountains = 17,
            Stormwind_City = 45,
            Stranglethorn_Vale = 46,
            Swamp_of_Sorrows = 47,
            Tanaris = 18,
            Teldrassil = 19,
            Terokkar_Forest = 60,
            The_Barrens = 20,
            The_Exodar = 21,
            The_Hinterlands = 48,
            Thousand_Needles = 22,
            Thunder_Bluff = 23,
            Tirisfal_Glades = 49,
            UnGoro_Crater = 24,
            Undercity = 50,
            Western_Plaguelands = 51,
            Westfall = 52,
            Wetlands = 53,
            Winterspring = 25,
            Zangarmarsh = 61
        }

        // ZoneName_Enums
        #endregion

        #region Properties

        private static bool m_ScriptStopped = false;
        /// <summary>
        /// This is set to true by Rhabot when the script has been stopped. True when
        /// script has stopped, false when not stopped.
        /// </summary>
        public static bool ScriptStopped
        {
            get { return m_ScriptStopped; }
            set { m_ScriptStopped = value; }
        }

        // Properties
        #endregion

        #region Functions

        /// <summary>
        /// Adds an item to the Rhabot log
        /// </summary>
        /// <param name="LogMessage">the message to add to the log</param>
        public static void AddStringToLog(string LogMessage)
        {
            if (NewLogEntry != null)
                NewLogEntry(LogMessage);
        }

        // Functions
        #endregion

        #region Events

        public delegate void MobKilledHandler(string MobName, int Level);
        public static event MobKilledHandler MobKilled;

        public static void Raise_MobKilled(string MobName, int Level)
        {
            if (MobKilled != null)
                MobKilled(MobName, Level);
        }

        public delegate void NewLogEntryHandler(string LogMessage);
        public static event NewLogEntryHandler NewLogEntry;

        // Events
        #endregion
    }
}
