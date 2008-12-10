using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ISXBotHelper.AutoNav;
using System.Xml.Serialization;
using ISXRhabotGlobal.SerializedProperties;

namespace ISXBotHelper.AutoNav
{
    [Serializable]
    public class clsAutoNavSaveList : IXmlSerializable
    {
        public List<clsBlockList> AutoNavList = new List<clsBlockList>();
        public string PlanName = "";

        #region Init

        /// <summary>
        /// Initializes a new instance of the clsAutoNavSaveList class.
        /// </summary>
        public clsAutoNavSaveList()
        {
        }

        /// <summary>
        /// Initializes a new instance of the clsAutoNavSaveList class.
        /// </summary>
        /// <param name="autoNavList"></param>
        /// <param name="planName"></param>
        public clsAutoNavSaveList(List<clsBlockList> autoNavList, string planName)
        {
            AutoNavList = autoNavList;
            PlanName = planName;
        }

        // Init
        #endregion

        #region ISerializable Members

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("PlanName", PlanName);

            // set autonav count
            int count = AutoNavList.Count;
            info.AddValue("AutoNavListCount", count);

            // add block items
            for (int i = 0; i < count; i++)
                info.AddValue(string.Format("AutoNavList_{0}", i.ToString().Trim()), AutoNavList[i]);
        }

        #endregion

        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            PlanName = reader.GetAttribute("PlanName");

            // set autonav count
            AutoNavList = clsXMLSerialList.ReadXMLList<clsBlockList>("AutoNavList", typeof(clsBlockList), reader);
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString("PlanName", PlanName);
            clsXMLSerialList.WriteXMLList<clsBlockList>("AutoNavList", AutoNavList, typeof(clsBlockList), ref writer);
        }

        #endregion
    }
}
