using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using ISXRhabotGlobal.SerializedProperties;
using ISXBotHelper.Talents;
using ISXBotHelper.AutoNav;
using ISXBotHelper.Items;
using ISXBotHelper;

namespace ISXBotHelper.Settings.Settings
{
    [Serializable]
    public class clsGlobalSettings : IXmlSerializable
    {
        #region Properties

        private bool m_LogoutOnStuck = true;
        /// <summary>
        /// When true, the bot will logout if stuck for StuckTimeout seconds
        /// </summary>
        public bool LogoutOnStuck
        {
            get { return m_LogoutOnStuck; }
            set { m_LogoutOnStuck = value; }
        }

        private int m_StuckTimeout = 180;
        /// <summary>
        /// Time, in seconds, to allow us to remain stuck before raising a stuck error
        /// </summary>
        public int StuckTimeout
        {
            get { return m_StuckTimeout; }
            set { m_StuckTimeout = value; }
        }

        private bool m_DeclineGuildInvite = true;
        /// <summary>
        /// When true, guild invites are declined
        /// </summary>
        public bool DeclineGuildInvite
        {
            get { return m_DeclineGuildInvite; }
            set { m_DeclineGuildInvite = value; }
        }

        private bool m_DeclineGroupInvite = true;
        /// <summary>
        /// When true, group invites are declined
        /// </summary>
        public bool DeclineGroupInvite
        {
            get { return m_DeclineGroupInvite; }
            set { m_DeclineGroupInvite = value; }
        }

        private bool m_DeclineDuelInvite = true;
        /// <summary>
        /// When true, duels are declined
        /// </summary>
        public bool DeclineDuelInvite
        {
            get { return m_DeclineDuelInvite; }
            set { m_DeclineDuelInvite = value; }
        }

        private string m_Character_MailTo = string.Empty;
        /// <summary>
        /// The character to mail items to. Leave blank for no mail
        /// </summary>
        public string Character_MailTo
        {
            get { return m_Character_MailTo; }
            set { m_Character_MailTo = value; }
        }

        #region MSN Chat

        private string m_MSN_Username = string.Empty;
        /// <summary>
        /// MSN Chat Username
        /// </summary>
        public string MSN_Username
        {
            get { return m_MSN_Username; }
            set { m_MSN_Username = value; }
        }

        private string m_MSN_Password = string.Empty;
        /// <summary>
        /// MSN Chat Password
        /// </summary>
        public string MSN_Password
        {
            get { return m_MSN_Password; }
            set { m_MSN_Password = value; }
        }

        private bool m_MSN_UseMSNChat = false;
        /// <summary>
        /// When true, the MSN chat panel will load and send/receive commands via msn
        /// </summary>
        public bool MSN_UseMSNChat
        {
            get { return m_MSN_UseMSNChat; }
            set { m_MSN_UseMSNChat = value; }
        }

        // MSN Chat
        #endregion

        #region Email Settings

        /// <summary>
        /// List of email addresses to send when errors are received
        /// </summary>
        public List<string> Comm_Email_List = new List<string>();

        private string m_Comm_Email_SMTPServer = string.Empty;
        /// <summary>
        /// Email server address (like smtp.somewhere.com)
        /// </summary>
        public string Comm_Email_SMTPServer
        {
            get { return m_Comm_Email_SMTPServer; }
            set { m_Comm_Email_SMTPServer = value; }
        }

        private int m_Comm_Email_SMTPPort = 25;
        /// <summary>
        /// SMTP Server port
        /// </summary>
        public int Comm_Email_SMTPPort
        {
            get { return m_Comm_Email_SMTPPort; }
            set { m_Comm_Email_SMTPPort = value; }
        }

        private string m_Comm_Email_Usename = string.Empty;
        /// <summary>
        /// Username for logging into smtp server
        /// </summary>
        public string Comm_Email_Usename
        {
            get { return m_Comm_Email_Usename; }
            set { m_Comm_Email_Usename = value; }
        }

        private string m_Comm_Email_Password = string.Empty;
        /// <summary>
        /// Password for logging into smtp server. when setting from the GUI, need to encrypt first
        /// </summary>
        public string Comm_Email_Password
        {
            get { return m_Comm_Email_Password; }
            set { m_Comm_Email_Password = value; }
        }

        // Email/MSN Settings
        #endregion

        #region HealBot

        private string m_HealBot_HealSpell = string.Empty;
        /// <summary>
        /// Healing spell to use
        /// </summary>
        public string HealBot_HealSpell
        {
            get { return m_HealBot_HealSpell; }
            set { m_HealBot_HealSpell = value; }
        }

        /// <summary>
        /// Percent at which unit should be healed
        /// </summary>
        private int m_HealBot_HealPercent = 50;
        public int HealBot_HealPercent
        {
            get
            {
                if (m_HealBot_HealPercent < 10)
                    return 50;
                return m_HealBot_HealPercent;
            }
            set { m_HealBot_HealPercent = value; }
        }

        /// <summary>
        /// List of buff's to maintain on unit
        /// </summary>
        public List<string> HealBot_BuffList = new List<string>();

        private string m_HealBot_RemoveDisease = string.Empty;
        /// <summary>
        /// Spell to remove disease
        /// </summary>
        public string HealBot_RemoveDisease
        {
            get { return m_HealBot_RemoveDisease; }
            set { m_HealBot_RemoveDisease = value; }
        }

        private string m_HealBot_RemoveCurse = string.Empty;
        /// <summary>
        /// Spell to remove curses
        /// </summary>
        public string HealBot_RemoveCurse
        {
            get { return m_HealBot_RemoveCurse; }
            set { m_HealBot_RemoveCurse = value; }
        }

        private string m_HealBot_RemovePoison = string.Empty;
        /// <summary>
        /// Spell to remove poison
        /// </summary>
        public string HealBot_RemovePoison
        {
            get { return m_HealBot_RemovePoison; }
            set { m_HealBot_RemovePoison = value; }
        }
        
        private string m_HealBot_Target = string.Empty;
        /// <summary>
        /// Unit to follow/heal
        /// </summary>
        public string HealBot_Target
        {
            get { return m_HealBot_Target; }
            set { m_HealBot_Target = value; }
        }

        // HealBot
        #endregion

        #region AutoBuff

        private string m_AutoBuff_Heal = string.Empty;
        /// <summary>
        /// Healing spell to use for auto buff
        /// </summary>
        public string AutoBuff_Heal
        {
            get { return m_AutoBuff_Heal; }
            set { m_AutoBuff_Heal = value; }
        }

        private int m_AutoBuff_HealPercent = 75;
        /// <summary>
        /// Health percent of player before healing is cast
        /// </summary>
        public int AutoBuff_HealPercent
        {
            get { return m_AutoBuff_HealPercent; }
            set { m_AutoBuff_HealPercent = value; }
        }

        /// <summary>
        /// List of buffs to randomly add to players
        /// </summary>
        public List<string> AutoBuff_BuffList = new List<string>();


        // AutoBuff
        #endregion

        #region Auto Equip

        /// <summary>
        /// List of AutoEquip requirements
        /// </summary>
        public List<ISXBotHelper.Items.clsAutoEquipItem> AutoEquipList = new List<ISXBotHelper.Items.clsAutoEquipItem>();

        // Auto Equip
        #endregion

        #region Buy/Sell List

        [Serializable]
        public class clsItemColorInfo : IXmlSerializable
        {
            public bool Grey = false;
            public bool White = false;
            public bool Green = false;
            public bool Blue = false;
            public bool Purple = false;

            public const string GreyFilter = "-rarity 0";
            public const string WhiteFilter = "-rarity 1";
            public const string GreenFilter = "-rarity 2";
            public const string BlueFilter = "-rarity 3";
            public const string PurpleFilter = "-rarity 4";

            #region IXmlSerializable Members

            public System.Xml.Schema.XmlSchema GetSchema()
            {
                return null;
            }

            public void ReadXml(System.Xml.XmlReader reader)
            {
                Grey = reader.GetAttribute("Grey").ConvertToBool(false);
                White = reader.GetAttribute("White").ConvertToBool(false);
                Green = reader.GetAttribute("Green").ConvertToBool(false);
                Blue = reader.GetAttribute("Blue").ConvertToBool(false);
                Purple = reader.GetAttribute("Purple").ConvertToBool(false);
            }

            public void WriteXml(System.Xml.XmlWriter writer)
            {
                writer.WriteAttributeString("Grey", Grey.ToString());
                writer.WriteAttributeString("White", White.ToString());
                writer.WriteAttributeString("Green", Green.ToString());
                writer.WriteAttributeString("Blue", Blue.ToString());
                writer.WriteAttributeString("Purple", Purple.ToString());
            }

            #endregion
        }

        /// <summary>
        /// List of items to sell when vendor is visited
        /// </summary>
        public List<string> ItemSellList = new List<string>();

        private clsItemColorInfo m_ItemSellColors = new clsItemColorInfo();
        /// <summary>
        /// Color rules for selling to vendor
        /// </summary>
        public clsItemColorInfo ItemSellColors
        {
            get { return m_ItemSellColors; }
            set { m_ItemSellColors = value; }
        }

        /// <summary>
        /// List of items to buy when vendor is visited
        /// </summary>
        public List<string> ItemBuyList = new List<string>();

        /// <summary>
        /// List of items that can be deleted
        /// </summary>
        public List<string> ItemJunkList = new List<string>();

        private clsItemColorInfo m_ItemJunkColors = new clsItemColorInfo();
        /// <summary>
        /// Color rules for deleting
        /// </summary>
        public clsItemColorInfo ItemJunkColors
        {
            get { return m_ItemJunkColors; }
            set { m_ItemJunkColors = value; }
        }

        /// <summary>
        /// List of items to be mailed to mule
        /// </summary>
        public List<string> ItemMailList = new List<string>();

        private clsItemColorInfo m_ItemMailColors = new clsItemColorInfo();
        /// <summary>
        /// Color rules for mailing items
        /// </summary>
        public clsItemColorInfo ItemMailColors
        {
            get { return m_ItemMailColors; }
            set { m_ItemMailColors = value; }
        }

        /// <summary>
        /// List of items to Disenchant
        /// </summary>
        public List<string> ItemDisenchantList = new List<string>();

        private clsItemColorInfo m_ItemDisenchantColors = new clsItemColorInfo();
        /// <summary>
        /// Color rules for Disenchanting items
        /// </summary>
        public clsItemColorInfo ItemDisenchantColors
        {
            get { return m_ItemDisenchantColors; }
            set { m_ItemDisenchantColors = value; }
        }

        /// <summary>
        /// List of items to keep
        /// </summary>
        public List<string> ItemKeepList = new List<string>();

        /// <summary>
        /// List of items to open
        /// </summary>
        public List<string> ItemOpenList = new List<string>();

        // Buy/Sell List
        #endregion

        #region Talents

        public List<clsPTalent> TalentList = new List<clsPTalent>();

        // Talents
        #endregion

        // Properties
        #endregion

        #region init

        /// <summary>
        /// Initializes a new instance of the clsGlobalSettings class.
        /// </summary>
        public clsGlobalSettings()
        {
        }

        // init
        #endregion

        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            LogoutOnStuck = reader.GetAttribute("LogoutOnStuck").ConvertToBool(true);
            StuckTimeout = reader.GetAttribute("StuckTimeout").ConvertToInt(180);
            DeclineGuildInvite = reader.GetAttribute("DeclineGuildInvite").ConvertToBool(true);
            DeclineGroupInvite = reader.GetAttribute("DeclineGroupInvite").ConvertToBool(true);
            DeclineDuelInvite = reader.GetAttribute("DeclineDuelInvite").ConvertToBool(true);
            Character_MailTo = reader.GetAttribute("Character_MailTo");

            MSN_Username = reader.GetAttribute("MSN_Username");
            MSN_Password = reader.GetAttribute("MSN_Password");
            MSN_UseMSNChat = reader.GetAttribute("MSN_UseMSNChat").ConvertToBool(false);

            Comm_Email_SMTPServer = reader.GetAttribute("Comm_Email_SMTPServer");
            Comm_Email_SMTPPort = reader.GetAttribute("Comm_Email_SMTPPort").ConvertToInt(25);
            Comm_Email_Usename = reader.GetAttribute("Comm_Email_Usename");
            Comm_Email_Password = reader.GetAttribute("Comm_Email_Password");

            HealBot_HealSpell = reader.GetAttribute("HealBot_HealSpell");
            HealBot_HealPercent = reader.GetAttribute("HealBot_HealPercent").ConvertToInt(75);
            HealBot_RemoveDisease = reader.GetAttribute("HealBot_RemoveDisease");
            HealBot_RemoveCurse = reader.GetAttribute("HealBot_RemoveCurse");
            HealBot_RemovePoison = reader.GetAttribute("HealBot_RemovePoison");
            HealBot_Target = reader.GetAttribute("HealBot_Target");

            AutoBuff_Heal = reader.GetAttribute("AutoBuff_Heal");
            AutoBuff_HealPercent = reader.GetAttribute("AutoBuff_HealPercent").ConvertToInt(75);

            // lists
            Comm_Email_List = clsXMLSerialList.ReadXMLList<string>("Comm_Email_List", typeof(string), reader);
            HealBot_BuffList = clsXMLSerialList.ReadXMLList<string>("HealBot_BuffList", typeof(string), reader);
            AutoBuff_BuffList = clsXMLSerialList.ReadXMLList<string>("AutoBuff_BuffList", typeof(string), reader);
            AutoEquipList = clsXMLSerialList.ReadXMLList<clsAutoEquipItem>("AutoEquipList", typeof(clsAutoEquipItem), reader);
            TalentList = clsXMLSerialList.ReadXMLList<clsPTalent>("TalentList", typeof(clsPTalent), reader);

            // items
            ItemSellList = clsXMLSerialList.ReadXMLList<string>("ItemSellList", typeof(string), reader);
            ItemBuyList = clsXMLSerialList.ReadXMLList<string>("ItemBuyList", typeof(string), reader);
            ItemJunkList = clsXMLSerialList.ReadXMLList<string>("ItemJunkList", typeof(string), reader);
            ItemMailList = clsXMLSerialList.ReadXMLList<string>("ItemMailList", typeof(string), reader);
            ItemDisenchantList = clsXMLSerialList.ReadXMLList<string>("ItemDisenchantList", typeof(string), reader);
            ItemKeepList = clsXMLSerialList.ReadXMLList<string>("ItemKeepList", typeof(string), reader);
            ItemOpenList = clsXMLSerialList.ReadXMLList<string>("ItemOpenList", typeof(string), reader);

            // item colors
            ItemSellColors = reader.GetAttribute("ItemSellColors").Deserialize(typeof(clsItemColorInfo)) as clsItemColorInfo;
            ItemJunkColors = reader.GetAttribute("ItemJunkColors").Deserialize(typeof(clsItemColorInfo)) as clsItemColorInfo;
            ItemMailColors = reader.GetAttribute("ItemMailColors").Deserialize(typeof(clsItemColorInfo)) as clsItemColorInfo;
            ItemDisenchantColors = reader.GetAttribute("ItemDisenchantColors").Deserialize(typeof(clsItemColorInfo)) as clsItemColorInfo;
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString("LogoutOnStuck", LogoutOnStuck.ToString());
            writer.WriteAttributeString("StuckTimeout", StuckTimeout.ToString().Trim());
            writer.WriteAttributeString("DeclineGuildInvite", DeclineGuildInvite.ToString());
            writer.WriteAttributeString("DeclineGroupInvite", DeclineGroupInvite.ToString());
            writer.WriteAttributeString("DeclineDuelInvite", DeclineDuelInvite.ToString());
            writer.WriteAttributeString("Character_MailTo", Character_MailTo);

            writer.WriteAttributeString("MSN_Username", MSN_Username);
            writer.WriteAttributeString("MSN_Password", MSN_Password);
            writer.WriteAttributeString("MSN_UseMSNChat", MSN_UseMSNChat.ToString());

            writer.WriteAttributeString("Comm_Email_SMTPServer", Comm_Email_SMTPServer);
            writer.WriteAttributeString("Comm_Email_SMTPPort", Comm_Email_SMTPPort.ToString().Trim());
            writer.WriteAttributeString("Comm_Email_Usename", Comm_Email_Usename);
            writer.WriteAttributeString("Comm_Email_Password", Comm_Email_Password);

            writer.WriteAttributeString("HealBot_HealSpell", HealBot_HealSpell);
            writer.WriteAttributeString("HealBot_HealPercent", HealBot_HealPercent.ToString().Trim());
            writer.WriteAttributeString("HealBot_RemoveDisease", HealBot_RemoveDisease);
            writer.WriteAttributeString("HealBot_RemoveCurse", HealBot_RemoveCurse);
            writer.WriteAttributeString("HealBot_RemovePoison", HealBot_RemovePoison);
            writer.WriteAttributeString("HealBot_Target", HealBot_Target);

            writer.WriteAttributeString("AutoBuff_Heal", AutoBuff_Heal);
            writer.WriteAttributeString("AutoBuff_HealPercent", AutoBuff_HealPercent.ToString().Trim());

            // Lists
            clsXMLSerialList.WriteXMLList<string>("Comm_Email_List", Comm_Email_List, typeof(string), ref writer);
            clsXMLSerialList.WriteXMLList<string>("HealBot_BuffList", HealBot_BuffList, typeof(string), ref writer);
            clsXMLSerialList.WriteXMLList<string>("AutoBuff_BuffList", AutoBuff_BuffList, typeof(string), ref writer);
            clsXMLSerialList.WriteXMLList<clsAutoEquipItem>("AutoEquipList", AutoEquipList, typeof(clsAutoEquipItem), ref writer);
            clsXMLSerialList.WriteXMLList<clsPTalent>("TalentList", TalentList, typeof(clsPTalent), ref writer);

            clsXMLSerialList.WriteXMLList<string>("ItemSellList", ItemSellList, typeof(string), ref writer);
            clsXMLSerialList.WriteXMLList<string>("ItemBuyList", ItemBuyList, typeof(string), ref writer);
            clsXMLSerialList.WriteXMLList<string>("ItemJunkList", ItemJunkList, typeof(string), ref writer);
            clsXMLSerialList.WriteXMLList<string>("ItemMailList", ItemMailList, typeof(string), ref writer);
            clsXMLSerialList.WriteXMLList<string>("ItemDisenchantList", ItemDisenchantList, typeof(string), ref writer);
            clsXMLSerialList.WriteXMLList<string>("ItemKeepList", ItemKeepList, typeof(string), ref writer);
            clsXMLSerialList.WriteXMLList<string>("ItemOpenList", ItemOpenList, typeof(string), ref writer);

            // item colors
            writer.WriteAttributeString("ItemSellColors", ItemSellColors.Serialize(typeof(clsItemColorInfo)));
            writer.WriteAttributeString("ItemJunkColors", ItemJunkColors.Serialize(typeof(clsItemColorInfo)));
            writer.WriteAttributeString("ItemMailColors", ItemMailColors.Serialize(typeof(clsItemColorInfo)));
            writer.WriteAttributeString("ItemDisenchantColors", ItemDisenchantColors.Serialize(typeof(clsItemColorInfo)));
        }

        #endregion
    }
}
