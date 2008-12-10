using System.Threading;

namespace ISXBotHelper.Threading
{
    public class ThreadBase 
    {
        #region Variables

        /// <summary>
        /// Used for Spin Trap locking
        /// </summary>
        public ThreadSpin SpinTrap = new ThreadSpin();

        /// <summary>
        /// Used for locking
        /// </summary>
        public object LockObj = new object();

        // Variables
        #endregion

        #region Properties

        private int m_Shutdown = 0; // 0 = false, 1 = true
        /// <summary>
        /// Set to true when the thread needs to shutdown
        /// </summary>
        public bool Shutdown
        {
            get { return Thread.VolatileRead(ref m_Shutdown) == 1; }
            set { Thread.VolatileWrite(ref m_Shutdown, value ? 1 : 0); }
        }

        private int m_IsSleep = 0; // 0 = false, 1 = true
        /// <summary>
        /// When true, time to sleep
        /// </summary>
        private bool DoSleep
        {
            set { Thread.VolatileWrite(ref m_IsSleep, value ? 1 : 0); }
        }

        /// <summary>
        /// Returns true if thread should be sleeping
        /// </summary>
        public bool IsSleep
        {
            get { return Thread.VolatileRead(ref m_IsSleep) == 1; }
        }

        // Properties
        #endregion\\

        #region Functions

        /// <summary>
        /// Tells the thread to go to sleep
        /// </summary>
        public void Sleep()
        {
            DoSleep = true;
        }

        /// <summary>
        /// Tells the thread to wake up
        /// </summary>
        public void Resume()
        {
            DoSleep = false;
        }

        // Functions
        #endregion
    }
}
