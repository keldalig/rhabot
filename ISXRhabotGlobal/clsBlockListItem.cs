using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using LavishScriptAPI;
using System.Runtime.Serialization;

namespace ISXBotHelper.AutoNav
{
    [Serializable]
    public class clsPathPoint : ISerializable
    {
        #region Properties

        private double m_X = 0;
        public double X
        {
            get { return m_X; }
            set { m_X = value; }
        }

        private double m_Y = 0;
        public double Y
        {
            get { return m_Y; }
            set { m_Y = value; }
        }

        private double m_Z = 0;
        public double Z
        {
            get { return m_Z; }
            set { m_Z = value; }
        }

        // Properties
        #endregion

        #region Init

        /// <summary>
        /// Initializes a new instance of the PathPoint class.
        /// </summary>
        public clsPathPoint()
        {
        }

        /// <summary>
        /// Initializes a new instance of the PathPoint class.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public clsPathPoint(double x, double y, double z)
        {
            m_X = x;
            m_Y = y;
            m_Z = z;
        }

        /// <summary>
        /// Initializes a new instance of the PathPoint class.
        /// </summary>
        public clsPathPoint(Point3f point3f)
        {
            X = point3f.X;
            Y = point3f.Y;
            Z = point3f.Z;
        }

        protected clsPathPoint(SerializationInfo info, StreamingContext context)
        {
            m_X = info.GetByte("m_X");
            m_Y = info.GetByte("m_Y");
            m_Z = info.GetByte("m_Z");
        }

        // Init
        #endregion

        #region ISerializable Members

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("m_X", m_X);
            info.AddValue("m_Y", m_Y);
            info.AddValue("m_Z", m_Z);
        }

        #endregion
    }

    [Serializable]
    public class clsBlockListItem : ISerializable
    {
        #region Enums

        /// <summary>
        /// Holds the list of node types
        /// </summary>
        public enum ENodeType
        {
            NONE = 0,

            // Main Block

            /// <summary>
            /// goes to and opens vendor panel, will sell
            /// </summary>
            Go_To_Vendor_X_City_Y, 

            /// <summary>
            /// goes to and opens flightmaster panel
            /// </summary>
            Go_To_Flightmaster_X_City_Y,

            /// <summary>
            /// goes to and opens training panel. TRAINS
            /// </summary>
            Go_To_Trainer_X_City_Y, 

            /// <summary>
            /// takes you to the dock
            /// </summary>
            Go_To_Boat_X_City_Y, 

            /// <summary>
            /// puts you on the boat and takes you off the boat onto the new dock
            /// </summary>
            Take_Boat_To_City_X, 

            /// <summary>
            /// run a specific path
            /// </summary>
            Run_Rhabot_Path, 

            /// <summary>
            /// Runs a full Rhabot path (found in the Rhabot level folders). 
            /// Searches for units, herbs/mines/chests, and vendor runs
            /// </summary>
            Run_Rhabot_Path_Full,

            /// <summary>
            /// Goes to innkeeper and opens panel
            /// </summary>
            Go_To_Innkeeper_X_City_Y,

            /// <summary>
            /// Hearthstones
            /// </summary>
            Stone_Home,

            /// <summary>
            /// Goes to repair vendor and opens panel. Will sell/repair
            /// </summary>
            Go_To_Repair_Vendor_X_City_Y,

            /// <summary>
            /// Goes to mailbox, opens and mails items
            /// </summary>
            Go_To_Mailbox_X_City_Y,

            /// <summary>
            /// Speaks to a person
            /// </summary>
            Speak_To_Person_X_At_Y,

            /// <summary>
            /// Goes to a questgiver and opens dialog
            /// </summary>
            Go_To_QuestGiver_X_At_Y,

            /// <summary>
            /// Fishes at XYZ
            /// </summary>
            Fish_At_XYZ,

            /// <summary>
            /// Condition of Fish. Stop fishing after X minutes (X in quantity)
            /// </summary>
            Fish_Until_X_Time,

            /// <summary>
            /// Condition of Fish. Stop fishing after X of Y item (fish) captured
            /// </summary>
            Fish_Capture_X_of_Y,

            /// <summary>
            /// Specifies a new block (has no main block requirements)
            /// </summary>
            NEW_BLOCK,
            

            // Sub Block

            /// <summary>
            /// MUST be at vendor already
            /// </summary>
            Purchase_X_of_Y, 

            /// <summary>
            /// MUST be at flight master
            /// </summary>
            Take_Flight_X_To_Y, 

            /// <summary>
            /// MUST be at innkeeper
            /// </summary>
            Make_Inn_Home,

            /// <summary>
            /// Picks up the quest named X
            /// </summary>
            Pick_Up_Quest_X,

            /// <summary>
            /// Turns in the quest named Y. Automatically selects reward (random slot)
            /// </summary>
            Turn_In_Quest_Y,

            /// <summary>
            /// fights mobs in your zone that are within level range
            /// </summary>
            Fight_Mobs_Zone_X,

            /// <summary>
            /// Fights mobs that have this name
            /// </summary>
            Fight_Mobs_Zone_X_Named_Y,

            /// <summary>
            /// fights elite mobs in your zone within level range
            /// </summary>
            Fight_Elite_Mobs_Zone_X,

            /// <summary>
            /// fights elite mobs that have this name
            /// </summary>
            Fight_Elite_Mobs_Zone_X_Named_Y,

            /// <summary>
            /// Don't fight mobs with this name
            /// </summary>
            Exclude_Mobs_Zone_X_Named_Y,

            /// <summary>
            /// Pick herbs in zone X
            /// </summary>
            Find_Herbs_Zone_X,

            /// <summary>
            /// Pick mines in zone X
            /// </summary>
            Find_Mines_Zone_X,

            /// <summary>
            /// Find chests in zone X
            /// </summary>
            Find_Chests_Zone_X,

            /// <summary>
            /// Picks up 1 item with name X in zone Y (MAIN/SUB)
            /// </summary>
            Pickup_Item_Named_X_Zone_Y,

            /// <summary>
            /// Picks up multiple quantities of item X in zone Y
            /// </summary>
            Pickup_Item_Named_X_Zone_Y_MULTI,

            /// <summary>
            /// Use item in your backs named X
            /// </summary>
            Use_Item_X,

            /// <summary>
            /// Go to this location
            /// </summary>
            Go_To_XYZ,

            /// <summary>
            /// Choose the gossip option ID X
            /// </summary>
            Choose_Gossip_Option_X,

            /// <summary>
            /// SPECIAL CONDITION. Returns to a vendor when item named X has Y quantity. 
            /// You can add a sub to this of purchase X of Y. Path restarts from vendor
            /// </summary>
            Return_To_Vendor_When_X_Has_Quantity_Y,


            // Conditions
            // All AND's must be met for a block to complete
            // Any OR can be met for a block to complete
            // OR's take precedence over AND's

            /// <summary>
            /// OR. Block finishes when bags are full
            /// </summary>
            Continue_Until_Bags_Full,

            /// <summary>
            /// OR. Continue until your durability reaches X percent
            /// </summary>
            Continue_Durability_X_Percent,

            /// <summary>
            /// AND. Continue until you have X of Y items in your bags
            /// </summary>
            Continue_Until_X_Item_Y_Quantity,

            /// <summary>
            /// AND. Continue until you have killed X of mob named Y
            /// </summary>
            Continue_Until_X_Mob_Y_Killed,

            /// <summary>
            /// AND. Continue until character reaches level X
            /// </summary>
            Continue_Until_Character_Is_Level_X
        }

        // Enums
        #endregion

        #region Properties

        /// <summary>
        /// The node type
        /// </summary>
        public ENodeType NodeType = ENodeType.NONE;

        /// <summary>
        /// How many of this item to kill/loot
        /// </summary>
        public int Quantity = 0;

        /// <summary>
        /// Destination point (vendor location, mailbox location, etc)
        /// </summary>
        public clsPathPoint DestinationPoint = null;

        /// <summary>
        /// Destination/Quest/Item name (if not using DestPoint)
        /// </summary>
        public string DestName = "";

        /// <summary>
        /// Zone Name - REQUIRED for sub blocks
        /// </summary>
        public string ZoneName = "";

        /// <summary>
        /// True when this portion of the path uses a flying mount
        /// </summary>
        public bool PathCanFly = false;

        // Properties
        #endregion

        #region Init

        /// <summary>
        /// Initializes a new instance of the clsBlockListItem class.
        /// </summary>
        public clsBlockListItem()
        {
        }

        /// <summary>
        /// Initializes a new instance of the clsBlockListItem class.
        /// </summary>
        /// <param name="nodeType"></param>
        /// <param name="goToName"></param>
        /// <param name="quantity"></param>
        /// <param name="destinationPoint"></param>
        /// <param name="destName"></param>
        public clsBlockListItem(ENodeType nodeType, int quantity, clsPathPoint destinationPoint, string destName)
        {
            NodeType = nodeType;
            Quantity = quantity;
            DestinationPoint = destinationPoint;
            DestName = destName;
        }

        protected clsBlockListItem(SerializationInfo info, StreamingContext context)
        {
            NodeType = (ENodeType)info.GetValue("NodeType", typeof(ENodeType));
            Quantity = info.GetInt32("Quantity");
            DestinationPoint = (clsPathPoint)info.GetValue("DestinationPoint", typeof(clsPathPoint));
            DestName = info.GetString("DestName");
            ZoneName = info.GetString("ZoneName");
            PathCanFly = info.GetBoolean("PathCanFly");
        }

        // Init
        #endregion

        #region Clone

        /// <summary>
        /// Clones this item
        /// </summary>
        public clsBlockListItem Clone()
        {
            clsBlockListItem retItem = new clsBlockListItem();

            // clone it
            retItem.NodeType = NodeType;
            retItem.Quantity = Quantity;
            retItem.DestinationPoint = new clsPathPoint(DestinationPoint.X, DestinationPoint.Y, DestinationPoint.Z);
            retItem.DestName = (string)DestName.Clone();

            // return the cloned item
            return retItem;
        }

        // Clone
        #endregion

        #region ToString

        /// <summary>
        /// Returns Destination Name
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return DestName;
        }

        // ToString
        #endregion

        #region ISerializable Members

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("NodeType", NodeType);
            info.AddValue("Quantity", Quantity);
            info.AddValue("DestinationPoint", DestinationPoint);
            info.AddValue("DestName", DestName);
            info.AddValue("ZoneName", ZoneName);
            info.AddValue("PathCanFly", PathCanFly);
        }

        #endregion
    }
}
