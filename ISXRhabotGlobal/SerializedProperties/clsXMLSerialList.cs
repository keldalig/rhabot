using System;
using System.Collections.Generic;
using System.Xml;
using ISXBotHelper;

namespace ISXRhabotGlobal.SerializedProperties
{
    public class clsXMLSerialList
    {
        #region XMLSerialLists

        /// <summary>
        /// Reads the xml attributes to rebuild an xml serialized list
        /// </summary>
        public static List<T> ReadXMLList<T>(string ListName, Type typeOfItem, XmlReader reader)
        {
            List<T> retList = new List<T>();

            // get the counter name
            string counterName = string.Format("{0}_count", ListName);

            // get list count
            int counter = reader.GetAttribute(counterName).ConvertToInt(0);

            // exit nothing
            if (counter == 0)
                return retList;

            // pop the list
            for (int i = 0; i < counter; i++)
                retList.Add((T)reader.GetAttribute(string.Format("{0}_{1}", ListName, i.ToString().Trim())).Deserialize(typeOfItem));

            // return the list
            return retList;
        }

        /// <summary>
        /// Writes an xml serialized list
        /// </summary>
        public static void WriteXMLList<T>(string ListName, List<T> ListToSave, Type typeOfItem, ref XmlWriter writer)
        {
            // get the counter name
            string counterName = string.Format("{0}_count", ListName);

            // get list count
            int counter = ListToSave.Count;

            // write the counter
            writer.WriteAttributeString(counterName, counter.ToString().Trim());

            // exit nothing
            if (counter == 0)
                return;

            // write the list
            for (int i = 0; i < counter; i++)
                writer.WriteAttributeString(string.Format("{0}_{1}", ListName, i.ToString().Trim()), ListToSave[i].Serialize(typeOfItem));
        }

        // XMLSerialLists
        #endregion
    }
}
