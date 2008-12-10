using System;
using LavishVMAPI;

namespace ISXBotHelper
{
    /// <summary>
    /// Functions for locking/unlocking frames
    /// </summary>
    public class clsFrameLock
    {
        #region LockBuffer

        /// <summary>
        /// Creates a lock buffer that can be used in using{} blocks
        /// </summary>
        public class LockBuffer: IDisposable
        {
            /// <summary>
            /// Creates the class and locks the frame. Frame is unlocked on disposing
            /// </summary>
            public LockBuffer()
            {
                // lock the frame
                Frame.Lock();
            }

            #region IDisposable Members

            public void Dispose()
            {
                // unlock the frame
                Frame.Unlock();
            }

            #endregion
        }

        // LockBuffer
        #endregion
    }
}
