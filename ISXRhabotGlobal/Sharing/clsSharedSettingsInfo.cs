using System;

namespace ISXBotHelper.Sharing
{
    /// <summary>
    /// Summary description for clsSharedSettingsInfo
    /// </summary>
    public class clsSharedSettingsInfo
    {
        /// <summary>
        /// Original UserGUID
        /// </summary>
        public string UG { get; set; }

        public string SettingsName { get; set; }
        public bool IsPaid { get; set; }
        public int MinLevel { get; set; }
        public int MaxLevel { get; set; }
    }
}