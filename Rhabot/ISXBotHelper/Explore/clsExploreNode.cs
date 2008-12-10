using System;
using System.Collections.Generic;
using System.Text;
using ISXWoW;
using LavishVMAPI;
using LavishScriptAPI;

namespace ISXBotHelper.Explore
{
    public class clsExploreNode
    {
        #region Propreties

        /// <summary>
        /// The point in this node
        /// </summary>
        public clsPath.PathPoint ExplorePoint = new clsPath.PathPoint();

        /// <summary>
        /// List of units found at this location
        /// </summary>
        public clsUnitInfo unitInfo = new clsUnitInfo();

        /// <summary>
        /// The subzone name this point is in
        /// </summary>
        public string SubZoneName = "";

        /// <summary>
        /// The zone this point is in
        /// </summary>
        public string ZoneName = "";

        // Propreties
        #endregion
    }
}
