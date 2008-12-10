using System;
using ISXRhabotGlobal;
using System.Xml.Serialization;
using ISXWoW;

namespace ISXBotHelper.Items
{
    [Serializable]
    public class clsAutoEquipItem : IXmlSerializable
    {
        public WoWEquipSlot EquipSlot = WoWEquipSlot.Bag1;
        public clsGlobals.EEquipItemMaterialType MaterialType = clsGlobals.EEquipItemMaterialType.None;
        public clsGlobals.ENeedEquipStat EquipStat = clsGlobals.ENeedEquipStat.None;

        /// <summary>
        /// Initializes a new instance of the clsAutoEquipItem class.
        /// </summary>
        /// <param name="equipSlot">slot to check</param>
        /// <param name="materialType">material to favor</param>
        /// <param name="equipStat">stat to favor</param>
        public clsAutoEquipItem(WoWEquipSlot equipSlot, clsGlobals.EEquipItemMaterialType materialType, clsGlobals.ENeedEquipStat equipStat)
        {
            EquipSlot = equipSlot;
            MaterialType = materialType;
            EquipStat = equipStat;
        }

        /// <summary>
        /// Initializes a new instance of the clsAutoEquipItem class.
        /// </summary>
        public clsAutoEquipItem()
        {
        }

        /// <summary>
        /// Initializes a new instance of the clsAutoEquipItem class.
        /// </summary>
        public clsAutoEquipItem(string equipSlot, string materialType, string equipStat)
        {
            // equip slot
            if (!string.IsNullOrEmpty(equipSlot))
                EquipSlot = (WoWEquipSlot)Enum.Parse(typeof(WoWEquipSlot), equipSlot);
            else
                EquipSlot = WoWEquipSlot.Bag1;

            // material
            if (!string.IsNullOrEmpty(materialType))
                MaterialType = (clsGlobals.EEquipItemMaterialType)Enum.Parse(typeof(clsGlobals.EEquipItemMaterialType), materialType);
            else
                MaterialType = clsGlobals.EEquipItemMaterialType.None;

            // stat
            if (!string.IsNullOrEmpty(equipStat))
                EquipStat = (clsGlobals.ENeedEquipStat)Enum.Parse(typeof(clsGlobals.ENeedEquipStat), equipStat);
            else
                EquipStat = clsGlobals.ENeedEquipStat.None;
        }

        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            EquipSlot = (WoWEquipSlot)Enum.Parse(typeof(WoWEquipSlot), reader.GetAttribute("EquipSlot"));
            MaterialType = (clsGlobals.EEquipItemMaterialType)Enum.Parse(typeof(clsGlobals.EEquipItemMaterialType), reader.GetAttribute("MaterialType"));
            EquipStat = (clsGlobals.ENeedEquipStat)Enum.Parse(typeof(clsGlobals.ENeedEquipStat), reader.GetAttribute("EquipStat"));
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString("EquipSlot", EquipSlot.ToString());
            writer.WriteAttributeString("MaterialType", MaterialType.ToString());
            writer.WriteAttributeString("EquipStat", EquipStat.ToString());
        }

        #endregion
    }

}
