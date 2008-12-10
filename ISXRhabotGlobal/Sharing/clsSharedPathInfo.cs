using System;

namespace ISXBotHelper.Sharing
{
    /// <summary>
    /// Summary description for clsSharedPathInfo
    /// </summary>
    public class clsSharedPathInfo
    {
        /// <summary>
        /// Original UserGUID
        /// </summary>
        public string UG { get; set; }

        public string GroupName { get; set; }
        public int PathLevel { get; set; }
        public bool IsPaid { get; set; }
    }
}