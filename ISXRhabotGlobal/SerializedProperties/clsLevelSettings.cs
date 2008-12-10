using System;
using System.Collections.Generic;
using ISXWoW;
using System.Xml.Serialization;
using ISXRhabotGlobal.SerializedProperties;

namespace ISXBotHelper.Settings.Settings
{
    [Serializable]
    public class clsLevelSettings : IXmlSerializable
    {
        #region Properties

        /// <summary>
        /// Number of levels below your level to initiate an attack.
        /// Example: If you are lvl 20, setting LLA to 4 would allow you to initiate an attack
        ///      on lvl 16+ mobs
        /// </summary>
        public int LowLevelAttack = 4;

        /// <summary>
        /// Number of levels above your level to initiate an attack.
        /// Example: If you are lvl 20, setting LLA to would allow you to initiate an attack
        ///     on lvl 20-21 mobs
        /// </summary>
        public int HighLevelAttack = 1;

        /// <summary>
        /// Distance to attack targets
        /// </summary>
        public int TargetRange = 20;

        /// <summary>
        /// Distance to search for targets
        /// </summary>
        public int SearchRange = 30;

        /// <summary>
        /// When true, elites will be targetted in the target search
        /// </summary>
        public bool TargetElites = false;

        /// <summary>
        /// True if the user is a skinner
        /// </summary>
        public bool IsSkinner = false;

        /// <summary>
        /// True if the user is a miner
        /// </summary>
        public bool IsMiner = false;

        /// <summary>
        /// True if the user is an herbalist
        /// </summary>
        public bool IsFlowerPicker = false;

        /// <summary>
        /// Set to true if the character is a rogue
        /// </summary>
        public bool IsRogue = false;

        /// <summary>
        /// When true, usable chests will be searched for when appropriate
        /// </summary>
        public bool Search_Chest = true;

        /// <summary>
        /// When one or more equipped item's durability equals this percent, go repair
        /// </summary>
        public int DurabilityPercent = 30;

        #region Combat Options

        /// <summary>
        /// List of spells to buff at start at of combat
        /// </summary>
        public List<string> Combat_PreBuffSpells_List = new List<string>();

        /// <summary>
        /// Spell used for pulling mobs
        /// </summary>
        public string Combat_PullSpell = string.Empty;

        /// <summary>
        /// List of spells to spam during attack
        /// </summary>
        public List<string> Combat_SpamSpells_List = new List<string>();

        /// <summary>
        /// Healing spell
        /// </summary>
        public string Combat_HealSpell = string.Empty;

        /// <summary>
        /// protection spell (like totem, or self buff)
        /// </summary>
        public string Combat_ProtectionSpell = string.Empty;

        /// <summary>
        /// Totem to cast during combat (such as Fire Nova). Leave blank for non-shamans
        /// </summary>
        public string Combat_CombatTotem = string.Empty;

        /// <summary>
        /// Set to the name of a healing over time spell (or healing stream totem)
        /// Used when more than one mob attacks
        /// </summary>
        public string Combat_HealingOT = string.Empty;

        /// <summary>
        /// List of DOT's (Damage Over Time spells) to cast ONCE per mob
        /// </summary>
        public List<string> Combat_DOT_List = new List<string>();

        /// <summary>
        /// Percent of health at which Heal should be cast
        /// </summary>
        public int Combat_HealthPercent = 45;

        /// <summary>
        /// Use spam spells until Mana is below this percent
        /// </summary>
        public int Combat_ManaSpam = 55;

        /// <summary>
        /// Time, in milliseconds, to wait before restarting combat loop. 0 for no wait
        /// If you are a caster, like a mage or warlock, you might want to set the time lower
        /// For tanks, such as warriors, you might want to set the time higher.
        /// 1000 = 1 second
        /// </summary>
        public int Combat_WaitTime_ms = 3000;

        /// <summary>
        /// List of units which need to be looted
        /// </summary>
        public List<WoWUnit> LootList = new List<WoWUnit>();

        /// <summary>
        /// percent at which mana or health must be before 'downtime' is used
        /// Downtime causes the character to sit. Toon will eat/drink if 
        /// food/water is available.
        /// </summary>
        public int Combat_DowntimePercent = 75;

        /// <summary>
        /// Run in panic when the number of attacking mobs is >= this number
        /// </summary>
        public int Combat_PanicThreshold = 3;

        /// <summary>
        /// When true, panic mode is enabled. Combat mode can exit if panic conditions are met
        /// When false, panic mode is disabled. Combat mode does not exit for panic conditions.
        /// </summary>
        public bool Combat_DoPanic = true;

        /// <summary>
        /// When true, spam spells are cast in random order
        /// </summary>
        public bool Combat_CastSpamRandomly = false;

        /// <summary>
        /// List of spells to cast after combat
        /// </summary>
        public List<string> Combat_PostCombatSpells = new List<string>();

        /// <summary>
        /// Combat class settings
        /// </summary>
        public clsCombatSettings Combat_ClassSettings = new clsCombatSettings();

        /// <summary>
        /// Spell used when a mob attempts to run away
        /// </summary>
        public string Combat_StopRunawayAttemptSpell = string.Empty;

        // Combat Options
        #endregion

        // Properties
        #endregion

        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null; 
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            this.Combat_CastSpamRandomly = reader.GetAttribute("Combat_CastSpamRandomly").ConvertToBool(false);
            this.Combat_ClassSettings = reader.GetAttribute("Combat_ClassSettings").DeserializeCombatSettings();
            this.Combat_CombatTotem = reader.GetAttribute("Combat_CombatTotem");
            this.Combat_DoPanic = reader.GetAttribute("Combat_DoPanic").ConvertToBool(true);
            this.Combat_DOT_List = clsXMLSerialList.ReadXMLList<string>("Combat_DOT_List", typeof(string), reader);
            this.Combat_DowntimePercent = reader.GetAttribute("Combat_DowntimePercent").ConvertToInt(75);
            this.Combat_HealingOT = reader.GetAttribute("Combat_HealingOT");
            this.Combat_HealSpell = reader.GetAttribute("Combat_HealSpell");
            this.Combat_HealthPercent = reader.GetAttribute("Combat_HealthPercent").ConvertToInt(50);
            this.Combat_ManaSpam = reader.GetAttribute("Combat_ManaSpam").ConvertToInt(25);
            this.Combat_PanicThreshold = reader.GetAttribute("Combat_PanicThreshold").ConvertToInt(3);
            this.Combat_PostCombatSpells = clsXMLSerialList.ReadXMLList<string>("Combat_PostCombatSpells", typeof(string), reader);
            this.Combat_PreBuffSpells_List = clsXMLSerialList.ReadXMLList<string>("Combat_PreBuffSpells_List", typeof(string), reader);
            this.Combat_ProtectionSpell = reader.GetAttribute("Combat_ProtectionSpell");
            this.Combat_PullSpell = reader.GetAttribute("Combat_PullSpell");
            this.Combat_SpamSpells_List = clsXMLSerialList.ReadXMLList<string>("Combat_SpamSpells_List", typeof(string), reader);
            this.Combat_StopRunawayAttemptSpell = reader.GetAttribute("Combat_StopRunawayAttemptSpell");
            this.Combat_WaitTime_ms = reader.GetAttribute("Combat_WaitTime_ms").ConvertToInt(1000);
            this.DurabilityPercent = reader.GetAttribute("DurabilityPercent").ConvertToInt(30);
            this.HighLevelAttack = reader.GetAttribute("HighLevelAttack").ConvertToInt(2);
            this.IsFlowerPicker = reader.GetAttribute("IsFlowerPicker").ConvertToBool(false);
            this.IsMiner = reader.GetAttribute("IsMiner").ConvertToBool(false);
            this.IsRogue = reader.GetAttribute("IsRogue").ConvertToBool(false);
            this.IsSkinner = reader.GetAttribute("IsSkinner").ConvertToBool(false);
            this.LowLevelAttack = reader.GetAttribute("LowLevelAttack").ConvertToInt(4);
            this.Search_Chest = reader.GetAttribute("Search_Chest").ConvertToBool(true);
            this.SearchRange = reader.GetAttribute("SearchRange").ConvertToInt(30);
            this.TargetElites = reader.GetAttribute("TargetElites").ConvertToBool(false);
            this.TargetRange = reader.GetAttribute("TargetRange").ConvertToInt(20);
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString("Combat_CastSpamRandomly", this.Combat_CastSpamRandomly.ToString());
            writer.WriteAttributeString("Combat_ClassSettings", this.Combat_ClassSettings.Serialize());
            writer.WriteAttributeString("Combat_CombatTotem", this.Combat_CombatTotem);
            writer.WriteAttributeString("Combat_DoPanic", this.Combat_DoPanic.ToString());
            clsXMLSerialList.WriteXMLList<string>("Combat_DOT_List", this.Combat_DOT_List, typeof(string), ref writer);
            writer.WriteAttributeString("Combat_DowntimePercent", this.Combat_DowntimePercent.ToString().Trim());
            writer.WriteAttributeString("Combat_HealingOT", Combat_HealingOT);
            writer.WriteAttributeString("Combat_HealSpell", Combat_HealSpell);
            writer.WriteAttributeString("Combat_HealthPercent", Combat_HealthPercent.ToString().Trim());
            writer.WriteAttributeString("Combat_ManaSpam", Combat_ManaSpam.ToString().Trim());
            writer.WriteAttributeString("Combat_PanicThreshold", Combat_PanicThreshold.ToString().Trim());
            clsXMLSerialList.WriteXMLList<string>("Combat_PostCombatSpells", Combat_PostCombatSpells, typeof(string), ref writer);
            clsXMLSerialList.WriteXMLList<string>("Combat_PreBuffSpells_List", Combat_PreBuffSpells_List, typeof(string), ref writer);
            writer.WriteAttributeString("Combat_ProtectionSpell", Combat_ProtectionSpell);
            writer.WriteAttributeString("Combat_PullSpell", Combat_PullSpell);
            clsXMLSerialList.WriteXMLList<string>("Combat_SpamSpells_List", Combat_SpamSpells_List, typeof(string), ref writer);
            writer.WriteAttributeString("Combat_StopRunawayAttemptSpell", Combat_StopRunawayAttemptSpell);
            writer.WriteAttributeString("Combat_WaitTime_ms", Combat_WaitTime_ms.ToString().Trim());
            writer.WriteAttributeString("DurabilityPercent", DurabilityPercent.ToString().Trim());
            writer.WriteAttributeString("HighLevelAttack", HighLevelAttack.ToString().Trim());
            writer.WriteAttributeString("IsFlowerPicker", IsFlowerPicker.ToString().Trim());
            writer.WriteAttributeString("IsMiner", IsMiner.ToString().Trim());
            writer.WriteAttributeString("IsRogue", IsRogue.ToString().Trim());
            writer.WriteAttributeString("IsSkinner", IsSkinner.ToString().Trim());
            writer.WriteAttributeString("LowLevelAttack", LowLevelAttack.ToString().Trim());
            writer.WriteAttributeString("Search_Chest", Search_Chest.ToString().Trim());
            writer.WriteAttributeString("SearchRange", SearchRange.ToString().Trim());
            writer.WriteAttributeString("TargetElites", TargetElites.ToString().Trim());
            writer.WriteAttributeString("TargetRange", TargetRange.ToString().Trim());
        }

        #endregion
    }
}
