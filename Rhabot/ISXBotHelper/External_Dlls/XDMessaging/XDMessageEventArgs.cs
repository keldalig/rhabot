using System;

namespace XDMessaging
{
    /// <summary>
    /// The event args used by the message handler. This enables DataGram data 
    /// to be passed to the handler.
    /// </summary>
    internal sealed class XDMessageEventArgs : EventArgs
    {
        /// <summary>
        /// Stores the DataGram containing message and channel data.
        /// </summary>
        private DataGram dataGram;
        /// <summary>
        /// Gets the DataGram associated with this instance.
        /// </summary>
        public DataGram DataGram
        {
            get
            {
                return dataGram;
            }
        }
        /// <summary>
        /// Constructor used to create a new instance from a DataGram struct.
        /// </summary>
        /// <param name="dataGram">The DataGram instance.</param>
        internal XDMessageEventArgs(DataGram dataGram)
        {
            this.dataGram = dataGram;
        }
    }
}
