using System;
using System.Collections.Generic;
using ISXWoW;

namespace ISXBotHelper
{
    public class clsBlacklist
    {
        #region Properties

        private PathListInfo.PathPoint m_Location = null;
        /// <summary>
        /// The location of this item
        /// </summary>
        public PathListInfo.PathPoint Location
        {
            get { return m_Location; }
            set { m_Location = value; }
        }

        private string m_GUID = string.Empty;
        /// <summary>
        /// The unit's guid
        /// </summary>
        public string GUID
        {
            get { return m_GUID; }
            set { m_GUID = value; }
        }

        private string m_ZoneText = string.Empty;
        /// <summary>
        /// The text of the zone this object resides in
        /// </summary>
        public string ZoneText
        {
            get { return m_ZoneText; }
            set { m_ZoneText = value; }
        }

        private string m_UnitName = string.Empty;
        /// <summary>
        /// Unit's name
        /// </summary>
        public string UnitName
        {
            get { return m_UnitName; }
            set { m_UnitName = value; }
        }

        // Properties
        #endregion

        #region Init

        /// <summary>
        /// Initializes a new instance of the clsBlacklist class.
        /// </summary>
        public clsBlacklist()
        {
        }

        /// <summary>
        /// Initializes a new instance of the clsBlacklist class.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="gUID"></param>
        /// <param name="zoneText"></param>
        /// <param name="unitName"></param>
        public clsBlacklist(PathListInfo.PathPoint location, string gUID, string zoneText, string unitName)
        {
            m_Location = location;
            m_GUID = gUID;
            m_ZoneText = zoneText;
            m_UnitName = unitName;
        }

        /// <summary>
        /// Initializes a new instance of the clsBlacklist class.
        /// </summary>
        public clsBlacklist(WoWUnit BlacklistUnit)
        {
            // add the unit's properties
            m_Location = clsPath.GetUnitLocation(BlacklistUnit);
            m_GUID = BlacklistUnit.GUID;
            m_ZoneText = clsCharacter.ZoneText;
            m_UnitName = BlacklistUnit.Name;

            // log it
            clsSettings.Logging.AddToLogFormatted("Blacklisting", "Blacklisting unit: {0} - Zone: {1} - Location: {2}",
                BlacklistUnit.Name, m_ZoneText, m_Location.ToString());
        }

        // Init
        #endregion

        #region Functions

        /// <summary>
        /// Checks if unit is the unit stored in the blacklist
        /// </summary>
        /// <param name="unit">the unit to compare</param>
        //public bool UnitMatch(WoWUnit unit)
        public bool UnitMatch(WoWObject unit)
        {
            try
            {
                using (new clsFrameLock.LockBuffer())
                {
                    // if we have no properties, exit
                    if ((string.IsNullOrEmpty(GUID)) || (Location == null) || (string.IsNullOrEmpty(ZoneText)) || (string.IsNullOrEmpty(UnitName)))
                        return false;

                    // if the zones don't match, exit
                    if (ZoneText != clsCharacter.ZoneText)
                        return false;

                    // see if the guid's match
                    if (unit.GUID == GUID)
                        return true;

                    // see if the location's are close, if so, it is probably blacklisted
                    if ((UnitName == unit.Name) && (Location.Distance(new PathListInfo.PathPoint(unit.Location)) <= 5))
                        return true;
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "clsBlacklist.UnitMatch");
            }

            // no matches, so exit false
            return false;
        }

        /// <summary>
        /// searches the blacklist to see if the unit exists in it
        /// </summary>
        /// <param name="blacklist">the blacklist to search</param>
        /// <param name="unit">the unit to find</param>
        public static bool IsBlacklisted(List<clsBlacklist> blacklist, WoWUnit unit)
        {
            try
            {
                // loop through the list
                using (new clsFrameLock.LockBuffer())
                {
                    foreach (clsBlacklist item in blacklist)
                    {
                        if (item.UnitMatch(unit))
                            return true;
                    }
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "IsBlacklisted");
            }
            
            // no matches
            return false;
        }

        // Functions
        #endregion
    }
}
