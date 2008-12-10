using System.Collections.Generic;
using System;
using System.Xml.Serialization;
using ISXRhabotGlobal.SerializedProperties;

namespace ISXBotHelper
{
    [Serializable]
    public class clsCombatSettings : IXmlSerializable
    {
        #region Shaman

        public string Shaman_MainHandBuff = string.Empty;

        public string Shaman_OffhandBuff = string.Empty;

        public string Shaman_EarthTotem = string.Empty;

        public string Shaman_FireTotem = string.Empty;

        public string Shaman_WaterTotem = string.Empty;

        public string Shaman_AirTotem = string.Empty;

        //Shaman
        #endregion

        #region Rogue

        // Rogue
        #endregion

        #region Warrior

        public enum EWarriorPullType
        {
            Charge = 1,
            Shoot,
            Throw
        }

        /// <summary>
        /// how to pull mobs
        /// </summary>
        public EWarriorPullType Warrior_PullType = EWarriorPullType.Charge;

        public bool Warrior_UseExecute = true;

        public bool Warrior_UseRampage = false;

        public bool Warrior_UseOverPower = true;

        // Warrior
        #endregion

        #region Warlock

        /// <summary>
        /// True to pull with your pet, false to pull with a spell
        /// </summary>
        public bool Warlock_PullWithPet = true;

        /// <summary>
        /// Time, in milliseconds, to wait from pet pulling until first spell is cast on mob
        /// </summary>
        public int Warlock_PullWaitTime = 1000;

        /// <summary>
        /// Number of soulshards to keep in the bag
        /// </summary>
        public int Warlock_SoulShardCount = 12;

        /// <summary>
        /// When target's health is X percent, begin casting Drain Soul (if we need a soulshard), or
        /// Drain Life if we don't need a soulshard (or health is below downtime level), or 
        /// Drain Mana if we don't need a soulshard, health is above downtime level and mana is below downtime level
        /// </summary>
        public int Warlock_DrainSoulOnHealthPercent = 20;

        /// <summary>
        /// Pet to use
        /// </summary>
        public ISXRhabotGlobal.clsGlobals.ECombatClass_Warlock_Pet Warlock_Pet = ISXRhabotGlobal.clsGlobals.ECombatClass_Warlock_Pet.Voidwalker;

        // Warlock
        #endregion

        #region Hunter

        /// <summary>
        /// Ranged DOT spells to maintain (using bow)
        /// </summary>
        public List<string> Hunter_Ranged_DOT = new List<string>();

        /// <summary>
        /// List of ranged actions to cast on target
        /// </summary>
        public List<string> Hunter_Ranged_SpamSpells = new List<string>();

        /// <summary>
        /// List of foods to feed pet
        /// </summary>
        public List<string> Hunter_Pet_FoodList = new List<string>();

        /// <summary>
        /// When pet's happiness is at or below this amount, feed him
        /// </summary>
        public int Hunter_Pet_FeedAtHappinessPercent = 75;

        // Hunter
        #endregion

        #region Mage

        // Mage
        #endregion

        #region Paladin

        /// <summary>
        /// On aggro, use the Avenger's Shield
        /// </summary>
        public bool Paladin_UseAvengersShield = false;

        /// <summary>
        /// On aggro, use Consecration
        /// </summary>
        public bool Paladin_UseConsecration = false;

        /// <summary>
        /// First seal to use (then transfers to target)
        /// </summary>
        public string Paladin_SealOne = string.Empty;

        /// <summary>
        /// Second seal to use (keeps on character)
        /// </summary>
        public string Paladin_SealTwo = string.Empty;
        
        // Paladin
        #endregion

        #region Priest

        // Priest
        #endregion

        #region Druid

        public ISXRhabotGlobal.clsGlobals.ECombatClass_Druid_Form Druid_CombatForm = ISXRhabotGlobal.clsGlobals.ECombatClass_Druid_Form.Humanoid;
        public bool Druid_ChangeToBearOnAggro = false;

        // Druid
        #endregion

        #region Debuffs

        public string Debuff_Poison = "";
        public string Debuff_Disease = "";
        public string Debuff_Curse = "";

        // Debuffs
        #endregion

        #region Init

        /// <summary>
        /// Initializes a new instance of the clsCombatSettings class.
        /// </summary>
        public clsCombatSettings()
        {
        }

        // Init
        #endregion

        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            Shaman_MainHandBuff = reader.GetAttribute("Shaman_MainHandBuff");
            Shaman_OffhandBuff = reader.GetAttribute("Shaman_OffhandBuff");
            Shaman_EarthTotem = reader.GetAttribute("Shaman_EarthTotem");
            Shaman_FireTotem = reader.GetAttribute("Shaman_FireTotem");
            Shaman_WaterTotem = reader.GetAttribute("Shaman_WaterTotem");
            Shaman_AirTotem = reader.GetAttribute("Shaman_AirTotem");
            Warrior_PullType = (EWarriorPullType)Enum.Parse(typeof(EWarriorPullType), reader.GetAttribute("Warrior_PullType"));
            Warrior_UseExecute = reader.GetAttribute("Warrior_UseExecute").ConvertToBool(false);
            Warrior_UseRampage = reader.GetAttribute("Warrior_UseRampage").ConvertToBool(false);
            Warrior_UseOverPower = reader.GetAttribute("Warrior_UseOverPower").ConvertToBool(false);
            Warlock_PullWithPet = reader.GetAttribute("Warlock_PullWithPet").ConvertToBool(false);
            Warlock_PullWaitTime = reader.GetAttribute("Warlock_PullWaitTime").ConvertToInt(10);
            Warlock_SoulShardCount = reader.GetAttribute("Warlock_SoulShardCount").ConvertToInt(10);
            Warlock_DrainSoulOnHealthPercent = reader.GetAttribute("Warlock_DrainSoulOnHealthPercent").ConvertToInt(75);
            Warlock_Pet = (ISXRhabotGlobal.clsGlobals.ECombatClass_Warlock_Pet)Enum.Parse(typeof(ISXRhabotGlobal.clsGlobals.ECombatClass_Warlock_Pet), reader.GetAttribute("Warlock_Pet"));
            Hunter_Pet_FeedAtHappinessPercent = reader.GetAttribute("Hunter_Pet_FeedAtHappinessPercent").ConvertToInt(75);
            Paladin_UseAvengersShield = reader.GetAttribute("Paladin_UseAvengersShield").ConvertToBool(false);
            Paladin_UseConsecration = reader.GetAttribute("Paladin_UseConsecration").ConvertToBool(false);
            Paladin_SealOne = reader.GetAttribute("Paladin_SealOne");
            Paladin_SealTwo = reader.GetAttribute("Paladin_SealTwo");
            Druid_CombatForm = (ISXRhabotGlobal.clsGlobals.ECombatClass_Druid_Form)Enum.Parse(typeof(ISXRhabotGlobal.clsGlobals.ECombatClass_Druid_Form), reader.GetAttribute("Druid_CombatForm"));
            Druid_ChangeToBearOnAggro = reader.GetAttribute("Druid_ChangeToBearOnAggro").ConvertToBool(false);

            // debuffs
            Debuff_Curse = reader.GetAttribute("Debuff_Curse");
            Debuff_Disease = reader.GetAttribute("Debuff_Disease");
            Debuff_Poison = reader.GetAttribute("Debuff_Poison");

            // Lists
            Hunter_Ranged_DOT = clsXMLSerialList.ReadXMLList<string>("Hunter_Ranged_DOT", typeof(string), reader);
            Hunter_Ranged_SpamSpells = clsXMLSerialList.ReadXMLList<string>("Hunter_Ranged_SpamSpells", typeof(string), reader);
            Hunter_Pet_FoodList = clsXMLSerialList.ReadXMLList<string>("Hunter_Pet_FoodList", typeof(string), reader);
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString("Shaman_MainHandBuff", Shaman_MainHandBuff);
            writer.WriteAttributeString("Shaman_OffhandBuff", Shaman_OffhandBuff);
            writer.WriteAttributeString("Shaman_EarthTotem", Shaman_EarthTotem);
            writer.WriteAttributeString("Shaman_FireTotem", Shaman_FireTotem);
            writer.WriteAttributeString("Shaman_WaterTotem", Shaman_WaterTotem);
            writer.WriteAttributeString("Shaman_AirTotem", Shaman_AirTotem);
            writer.WriteAttributeString("Warrior_PullType", Warrior_PullType.ToString());
            writer.WriteAttributeString("Warrior_UseExecute", Warrior_UseExecute.ToString());
            writer.WriteAttributeString("Warrior_UseRampage", Warrior_UseRampage.ToString());
            writer.WriteAttributeString("Warrior_UseOverPower", Warrior_UseOverPower.ToString());
            writer.WriteAttributeString("Warlock_PullWithPet", Warlock_PullWithPet.ToString());
            writer.WriteAttributeString("Warlock_PullWaitTime", Warlock_PullWaitTime.ToString().Trim());
            writer.WriteAttributeString("Warlock_SoulShardCount", Warlock_SoulShardCount.ToString().Trim());
            writer.WriteAttributeString("Warlock_DrainSoulOnHealthPercent", Warlock_DrainSoulOnHealthPercent.ToString().Trim());
            writer.WriteAttributeString("Warlock_Pet", Warlock_Pet.ToString());
            writer.WriteAttributeString("Hunter_Pet_FeedAtHappinessPercent", Hunter_Pet_FeedAtHappinessPercent.ToString().Trim());
            writer.WriteAttributeString("Paladin_UseAvengersShield", Paladin_UseAvengersShield.ToString());
            writer.WriteAttributeString("Paladin_UseConsecration", Paladin_UseConsecration.ToString());
            writer.WriteAttributeString("Paladin_SealOne", Paladin_SealOne);
            writer.WriteAttributeString("Paladin_SealTwo", Paladin_SealTwo);
            writer.WriteAttributeString("Druid_CombatForm", Druid_CombatForm.ToString());
            writer.WriteAttributeString("Druid_ChangeToBearOnAggro", Druid_ChangeToBearOnAggro.ToString());

            // debufs
            writer.WriteAttributeString("Debuff_Curse", Debuff_Curse);
            writer.WriteAttributeString("Debuff_Disease", Debuff_Disease);
            writer.WriteAttributeString("Debuff_Poison", Debuff_Poison);

            // Lists
            clsXMLSerialList.WriteXMLList<string>("Hunter_Ranged_DOT", Hunter_Ranged_DOT, typeof(string), ref writer);
            clsXMLSerialList.WriteXMLList<string>("Hunter_Ranged_SpamSpells", Hunter_Ranged_SpamSpells, typeof(string), ref writer);
            clsXMLSerialList.WriteXMLList<string>("Hunter_Pet_FoodList", Hunter_Pet_FoodList, typeof(string), ref writer);
        }

        #endregion
    }
}
