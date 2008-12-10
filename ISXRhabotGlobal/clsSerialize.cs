using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using ISXRhabotGlobal;
using ICSharpCode.SharpZipLib.BZip2;

using MerlinEncrypt;

namespace ISXBotHelper
{
    public static class clsSerialize
    {
        // http://support.microsoft.com/kb/815813

        private static MerlinEncrypt.Crypt crypt = new Crypt();

        #region Byte and String Conversions

        /// <summary>
        /// Converts a string to a byte array
        /// </summary>
        /// <param name="StrToConvert">the string to convert</param>
        public static byte[] ConvertStringToByteArray(string StrToConvert)
        {
            if (string.IsNullOrEmpty(StrToConvert))
                return null;

            // return as bytes
            byte[] buffer = new byte[StrToConvert.Length / 3];
            int i, j = buffer.Length;

            for (i = 0; i < j; i++)
                buffer[i] = Convert.ToByte(StrToConvert.Substring(i * 3, 3));

            return buffer;
        }

        /// <summary>
        /// Converts a byte array to a string
        /// </summary>
        /// <param name="ByteToConvert"></param>
        /// <returns></returns>
        public static string ConvertByteArrayToString(byte[] ByteToConvert)
        {
            if ((ByteToConvert == null) || (ByteToConvert.Length == 0))
                return string.Empty;

            StringBuilder sb = new StringBuilder();
            int j = ByteToConvert.Length;
            for (int i = 0; i < j; i++)
            {
                // pad 0's, then add byte
                if (ByteToConvert[i] < 10)
                    sb.Append("0");
                if (ByteToConvert[i] < 100)
                    sb.Append("0");
                sb.Append(ByteToConvert[i].ToString().Trim());
            }

            return sb.ToString();
        }

        // Byte and String Conversions
        #endregion // Byte and String Conversions

        #region Serialize Obj To Byte

        /// <summary>
        /// Serializes an object. Don't forget to mark the class with [Serializable]
        /// </summary>
        /// <param name="obj">the object to serialize</param>
        public static byte[] SerializeObject(object obj)
        {
            // exit if no object
            if (obj == null)
                return null;

            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();

            // serialize it
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }

        /// <summary>
        /// Deserializes an object.
        /// </summary>
        /// <param name="objBytes">the byte array to deserialize</param>
        /// <returns></returns>
        public static object DeserializeObject(byte[] objBytes)
        {
            if (objBytes == null)
                return null;

            MemoryStream ms = new MemoryStream(objBytes);
            BinaryFormatter bf = new BinaryFormatter();

            // deserialize and return
            return bf.Deserialize(ms);
        }

        // Serialize Obj To Byte
        #endregion

        #region Serialize

        /// <summary>
        /// Serializes an object
        /// </summary>
        private static string Serialize_ToString(object ObjectToSerialize, Type ObjectType)
        {
            // http://www.codeproject.com/KB/dotnet/mscompression.aspx

            // skip if no object
            if (ObjectToSerialize == null)
                return "";

            MemoryStream ms = new MemoryStream();

            // create the serialization object
            System.Xml.Serialization.XmlSerializer xSerializer = new System.Xml.Serialization.XmlSerializer(ObjectType);

            // serialize it
            xSerializer.Serialize(ms, ObjectToSerialize);

            // compress the encrypted data
            MemoryStream cms = new MemoryStream();
            byte[] buffer = ms.ToArray();
            Int32 size = buffer.Length;

            // Prepend the compressed data with the length of the uncompressed data (firs 4 bytes)
            BinaryWriter writer = new BinaryWriter(cms, System.Text.Encoding.ASCII);
            writer.Write(size);

            // compress it
            BZip2OutputStream bzCompressed = new BZip2OutputStream(cms);
            bzCompressed.Write(buffer, 0, size);
            bzCompressed.Close();

            // return it
            return ConvertByteArrayToString(cms.ToArray());
        }

        /// <summary>
        /// XML Deserializes a string
        /// </summary>
        /// <param name="strSerial">the string to decrypt and deserialize</param>
        /// <returns></returns>
        private static object Deserialize_FromString(string strSerial, Type ObjectType)
        {
            // skip if no data
            if (string.IsNullOrEmpty(strSerial))
                return null;

            // decompress first
            MemoryStream cms = new MemoryStream(ConvertStringToByteArray(strSerial));
            // read final uncompressed string size stored in first 4 bytes
            BinaryReader reader = new BinaryReader(cms, System.Text.Encoding.ASCII);
            Int32 size = reader.ReadInt32() * 4;
            BZip2InputStream bzStream = new BZip2InputStream(cms);
            byte[] buffer = new byte[size];
            int bytesRead = bzStream.Read(buffer, 0, size);
            bzStream.Close();
            cms.Close();

            // remove all bad 0's at the end
            MemoryStream ms = new MemoryStream(buffer);

            // create the serialization object
            System.Xml.Serialization.XmlSerializer xSerializer = new System.Xml.Serialization.XmlSerializer(ObjectType);

            // deserialize it
            return xSerializer.Deserialize(ms);
        }

        // Serialize
        #endregion

        #region Serialize - Extension

        #region Object

        /// <summary>
        /// (Extension) Serializes an object
        /// </summary>
        public static string Serialize(this object ObjectToSerialize, Type ObjectType)
        {
            return Serialize_ToString(ObjectToSerialize, ObjectType);
        }

        /// <summary>
        /// (Extension) XML Deserializes a string
        /// </summary>
        /// <param name="strSerial">the string to decrypt and deserialize</param>
        /// <returns></returns>
        public static object Deserialize(this string strSerial, Type ObjectType)
        {
            return Deserialize_FromString(strSerial, ObjectType);
        }

        // Object
        #endregion

        #region String

        /// <summary>
        /// (Extension) Serializes a string
        /// </summary>
        public static string Serialize(this string StrToSerialize)
        {
            return ConvertByteArrayToString(crypt.EncryptByteArray(UnicodeEncoding.Unicode.GetBytes(StrToSerialize)));
        }

        /// <summary>
        /// (Extension) XML Deserializes a string
        /// </summary>
        /// <param name="strSerial">the string to decrypt and deserialize</param>
        /// <returns></returns>
        public static string DeserializeString(this string strSerial)
        {
            return UnicodeEncoding.Unicode.GetString(crypt.DecryptByteArray(ConvertStringToByteArray(strSerial)));
        }

        // String
        #endregion

        #region PathListInfo

        /// <summary>
        /// (Extension) Serializes a string
        /// </summary>
        public static string Serialize(this PathListInfo pliToSerialize)
        {
            return Serialize_ToString(pliToSerialize, typeof(PathListInfo));
        }

        /// <summary>
        /// (Extension) XML Deserializes a string
        /// </summary>
        /// <param name="strSerial">the string to decrypt and deserialize</param>
        /// <returns></returns>
        public static PathListInfo DeserializePathListInfo(this string strSerial)
        {
            return Deserialize_FromString(strSerial, typeof(PathListInfo)) as PathListInfo;
        }

        // PathListInfo
        #endregion

        #region clsGlobalSettings

        /// <summary>
        /// (Extension) Serializes a string
        /// </summary>
        public static string Serialize(this Settings.Settings.clsGlobalSettings gSettings)
        {
            return Serialize_ToString(gSettings, typeof(Settings.Settings.clsGlobalSettings));
        }

        /// <summary>
        /// (Extension) XML Deserializes a string
        /// </summary>
        /// <param name="strSerial">the string to decrypt and deserialize</param>
        /// <returns></returns>
        public static Settings.Settings.clsGlobalSettings DeserializeGSettings(this string strSerial)
        {
            return Deserialize_FromString(strSerial, typeof(Settings.Settings.clsGlobalSettings)) as Settings.Settings.clsGlobalSettings;
        }

        // clsGlobalSettings
        #endregion

        #region clsLevelSettings

        /// <summary>
        /// (Extension) Serializes a string
        /// </summary>
        public static string Serialize(this Settings.Settings.clsLevelSettings lSettings)
        {
            return Serialize_ToString(lSettings, typeof(Settings.Settings.clsLevelSettings));
        }

        /// <summary>
        /// (Extension) XML Deserializes a string
        /// </summary>
        /// <param name="strSerial">the string to decrypt and deserialize</param>
        /// <returns></returns>
        public static Settings.Settings.clsLevelSettings DeserializeLevel(this string strSerial)
        {
            return Deserialize_FromString(strSerial, typeof(Settings.Settings.clsLevelSettings)) as Settings.Settings.clsLevelSettings;
        }

        // clsLevelSettings
        #endregion

        #region AutoNav

        /// <summary>
        /// (Extension) Serializes a string
        /// </summary>
        public static string Serialize(this AutoNav.clsAutoNavSaveList AutoNavSettings)
        {
            return Serialize_ToString(AutoNavSettings, typeof(AutoNav.clsAutoNavSaveList));
        }

        /// <summary>
        /// (Extension) XML Deserializes a string
        /// </summary>
        /// <param name="strSerial">the string to decrypt and deserialize</param>
        /// <returns></returns>
        public static AutoNav.clsAutoNavSaveList DeserializeAutoNav(this string strSerial)
        {
            return Deserialize_FromString(strSerial, typeof(AutoNav.clsAutoNavSaveList)) as AutoNav.clsAutoNavSaveList;
        }

        // AutoNav
        #endregion

        #region CombatSettings

        /// <summary>
        /// (Extension) Serializes a string
        /// </summary>
        public static string Serialize(this clsCombatSettings CombatSettings)
        {
            return Serialize_ToString(CombatSettings, typeof(clsCombatSettings));
        }

        /// <summary>
        /// (Extension) XML Deserializes a string
        /// </summary>
        /// <param name="strSerial">the string to decrypt and deserialize</param>
        /// <returns></returns>
        public static clsCombatSettings DeserializeCombatSettings(this string strSerial)
        {
            return Deserialize_FromString(strSerial, typeof(clsCombatSettings)) as clsCombatSettings;
        }

        // CombatSettings
        #endregion

        // Serialize - Extension
        #endregion

        #region Serialize - NEW

        /// <summary>
        /// Serializes an object
        /// </summary>
        public static byte[] Serialize_ToByteA(object ObjectToSerialize, Type ObjectType)
        {
            // http://www.codeproject.com/KB/dotnet/mscompression.aspx

            // skip if no object
            if (ObjectToSerialize == null)
                return new byte[0];

            MemoryStream ms = new MemoryStream();

            // create the serialization object
            System.Xml.Serialization.XmlSerializer xSerializer = new System.Xml.Serialization.XmlSerializer(ObjectType);

            // serialize it
            xSerializer.Serialize(ms, ObjectToSerialize);

            // compress the encrypted data
            MemoryStream cms = new MemoryStream();
            byte[] buffer = ms.ToArray();
            Int32 size = buffer.Length;

            // Prepend the compressed data with the length of the uncompressed data (firs 4 bytes)
            BinaryWriter writer = new BinaryWriter(cms, System.Text.Encoding.ASCII);
            writer.Write(size);

            // compress it
            BZip2OutputStream bzCompressed = new BZip2OutputStream(cms);
            bzCompressed.Write(buffer, 0, size);
            bzCompressed.Close();

            // return it
            return cms.ToArray();
        }

        /// <summary>
        /// XML Deserializes a string
        /// </summary>
        /// <param name="strSerial">the string to decrypt and deserialize</param>
        /// <returns></returns>
        public static object Deserialize_FromByteA(byte[] strSerial, Type ObjectType)
        {
            // skip if no data
            if ((strSerial == null) || (strSerial.Length <= 1))
                return null;

            // decompress first
            MemoryStream cms = new MemoryStream(strSerial);
            // read final uncompressed string size stored in first 4 bytes
            BinaryReader reader = new BinaryReader(cms, System.Text.Encoding.ASCII);
            Int32 size = reader.ReadInt32() * 4;
            BZip2InputStream bzStream = new BZip2InputStream(cms);
            byte[] buffer = new byte[size];
            int bytesRead = bzStream.Read(buffer, 0, size);
            bzStream.Close();
            cms.Close();

            // remove all bad 0's at the end
            MemoryStream ms = new MemoryStream(buffer);

            // create the serialization object
            System.Xml.Serialization.XmlSerializer xSerializer = new System.Xml.Serialization.XmlSerializer(ObjectType);

            // deserialize it
            return xSerializer.Deserialize(ms);
        }

        // Serialize - NEW
        #endregion
    }
}
