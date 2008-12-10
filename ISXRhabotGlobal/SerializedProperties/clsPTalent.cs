// Code by Undrgrnd59 - Apr 2007

using System;
using System.Xml.Serialization;

namespace ISXBotHelper.Talents
{
    [Serializable]
    public class clsPTalent : IXmlSerializable
    {
        public string name;
        public int level;

        public clsPTalent(string name, int level)
        {
            this.name = name;
            this.level = level;
        }

        public clsPTalent()
        {
            
        }

        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            this.name = reader.GetAttribute("name");
            this.level = reader.GetAttribute("level").ConvertToInt(0);
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString("name", this.name);
            writer.WriteAttributeString("level", this.level.ToString().Trim());
        }

        #endregion
    }
}
