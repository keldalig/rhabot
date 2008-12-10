using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace ISXBotHelper.AutoNav
{
    [Serializable]
    public class clsBlockList : ISerializable
    {
        #region Properties

        /// <summary>
        /// List of sub blocks in this list. Sub blocks are processed first
        /// </summary>
        public List<clsBlockList> SubBlocks = new List<clsBlockList>();

        /// <summary>
        /// This block's node
        /// </summary>
        public clsBlockListItem BlockItem = null;

        // Properties
        #endregion

        #region Clone

        public clsBlockList Clone()
        {
            clsBlockList retItem = new clsBlockList();
            retItem.BlockItem = BlockItem.Clone();

            // subblocks
            foreach (clsBlockList sBlock in SubBlocks)
                retItem.SubBlocks.Add(sBlock.Clone());

            // return the clone
            return retItem;
        }

        // Clone
        #endregion

        #region Init

        protected clsBlockList(SerializationInfo info, StreamingContext context)
        {
            BlockItem = (clsBlockListItem)info.GetValue("BlockItem", typeof(clsBlockListItem));

            // add sub block count
            int count = SubBlocks.Count;
            info.AddValue("SubBlocksCount", count);

            // add subblocks
            for (int i = 0; i < count; i++)
                info.AddValue(string.Format("SubBlocks_{0}", i.ToString().Trim()), SubBlocks[i]);
        }


        /// <summary>
        /// Initializes a new instance of the clsBlockList class.
        /// </summary>
        public clsBlockList()
        {
        }

        // Init
        #endregion

        #region ISerializable Members

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            BlockItem = (clsBlockListItem)info.GetValue("BlockItem", typeof(clsBlockListItem));
            SubBlocks = new List<clsBlockList>();

            // sub blocks
            int count = info.GetInt32("SubBlocksCount");
            
            // get sub blocks
            for (int i = 0; i < count; i++)
                SubBlocks.Add((clsBlockList) info.GetValue(string.Format("SubBlocks_{0}", i.ToString().Trim()), typeof(clsBlockList)));
        }

        #endregion
    }
}
