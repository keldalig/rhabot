using System;
using System.Threading;

namespace ISXBotHelper.Threading
{
    #region NOTES

    /*
        How does the Spin-Trap work?

        A Spin-Trap has, similar to a Spin-Lock, an Enter() and and Exit() method which mark the 
     Spin-Trap region. However, the thread which enters a Spin-Trap region does not try to acquire 
     a lock, but increments Spin-Trap's EnteredThreadCount counter at Enter() which it decrements 
     at Exit(). This ensures, that the Spin-Trap class will always know how many threads are 
     currently within all of its Enter-Exit-blocks.

        Lock-like behaviour comes with traps, that can be passivated or activated. A Trap() marks 
     defined states within a region of code which can be traversed by all threads if 
     passivated (no-lock) or capture any thread in a spin-lock state if activated (lock). 
     Once trapped, a thread will increment the Spin-Trap's TrappedThreadCount. If the traps 
     get deactivated, all trapped threads will exit and each will decrease TrappedThreadCount 
     by one. TrappedThreadCount can be never greater than EnteredThreadCount.

        Any thread can activate all traps by using the AcquireLock() method from within an Enter-Exit 
     region. This function behaves just like a spin-lock: if a thread tries to acquire the lock 
     while another thread owns the lock, it will be spin-locked in a trap-state. ReleaseLock() 
     releases the lock and will cause all threads to exit from their traps. Threads which are 
     trapped inside an AcquireLock() function will try to exit the trap and to acquire the lock.

        All this would not be of much use without the Wait() function which allows the thread which 
     acquired the lock to wait until each thread except the one which owns the lock either leaves 
     the Enter-Exit blocks or gets trapped in a defined state. Wait() will know when all threads 
     are captured in traps if TrappedThreadCount equals EnteredThreadCount. It is important to 
     note, that the number of captured threads is not a constant after Wait() and before 
     ReleaseLock() as more threads can enter and be trapped. This means that all code between 
     a Enter and a Trap can still execute after a Wait() and before the ReleaseLock(). A simple 
     solution to this problem is to place all traps just after the entrance of the spin-trap 
     regions.

        In summary: the Spin-Trap provides a mechanism to trap ongoing (concurrent) operations in 
     defined states in order to perform a more extensive operation which requires mutual exclusion 
     and a defined state of the object. As an example, one may think of an lock-free array-based 
     self-resizing Queue where a) enqueue, dequeue operations are concurrent lock-free operations 
     until b) a array resize has to take place. It is clear that a) and b) will interfere severely 
     until b) can lock all concurrent threads into defined states. It is also clear, that b) can 
     only be executed in a mutual exclusive manner and requires a lock around its code. 
    */

    // NOTES
    #endregion

    public struct ThreadSpin
    {
        // http://www.codeproject.com/useritems/spintrap.asp

        #region Properties

        private int m_enteredThreadCount;
        public int EnteredThreadCount
        {
            get { return Thread.VolatileRead(ref m_enteredThreadCount); }
        }

        private int m_trappedThreadCount;
        public int TrappedThreadCount
        {
            get { return Thread.VolatileRead(ref m_trappedThreadCount); }
        }

        private int m_lockState;
        public int LockState
        {
            get { return Thread.VolatileRead(ref m_lockState); }
        }

        // Properties
        #endregion

        /// <summary>
        /// Enter spin-trap region.
        /// </summary>
        public void Enter()
        {
            Thread.BeginCriticalRegion();
            Interlocked.Increment(ref m_enteredThreadCount);
        }

        /// <summary>
        /// Try to take lock, otherwise trap thread and retry once lock becomes free.
        /// </summary>
        public void EnterAcquire()
        {
            Thread.BeginCriticalRegion();
            Interlocked.Increment(ref m_enteredThreadCount);
            AcquireLock();
        }

        /// <summary>
        /// Try to take the lock, or trap thread till a lock becomes available.
        /// </summary>
        public void AcquireLock()
        {
            while (true)
            {
                // take lock and return
                if (Interlocked.Exchange(ref m_lockState, 1) == 0)
                {
                    return;
                }
                Trap();
            }
        }

        /// <summary>
        /// Release the lock for others to take.
        /// </summary>
        public void ReleaseLock()
        {
            // release lock
            Interlocked.Exchange(ref m_lockState, 0); 
        }

        /// <summary>
        /// Release the lock and exit the spin-trap region.
        /// </summary>
        public void ExitRelease()
        {
            // release lock
            Interlocked.Exchange(ref m_lockState, 0);
            Interlocked.Decrement(ref m_enteredThreadCount);
            Thread.EndCriticalRegion();
        }

        /// <summary>
        /// Wait for all threads to either exit the spin-trap regions or be trapped.
        /// </summary>
        public void Wait()
        {
            int it = 0;
            Interlocked.Increment(ref m_trappedThreadCount);
            while (true)
            {
                int trappedCount = Thread.VolatileRead(ref this.m_trappedThreadCount);
                int enteredCount = Thread.VolatileRead(ref this.m_enteredThreadCount);
                if (trappedCount == enteredCount) break;
                if (trappedCount > enteredCount) throw new Exception("trappedCount > enteredCount");
                StallThread(it++);
            }
            Interlocked.Decrement(ref m_trappedThreadCount);
        }

        /// <summary>
        /// Setup a trap.
        /// </summary>
        public void Trap()
        {
            if (Thread.VolatileRead(ref m_lockState) == 1)
            {
                int it = 0;
                Thread.MemoryBarrier();
                Interlocked.Increment(ref m_trappedThreadCount);
                while (Thread.VolatileRead(ref m_lockState) == 1)
                {
                    StallThread(it++);

                    // reset it if we are close to hitting max value
                    if (it >= int.MaxValue - 100)
                        it = 0;
                }
                Interlocked.Decrement(ref m_trappedThreadCount);
            }
        }

        /// <summary>
        /// Exit spin-trap region.
        /// </summary>
        public void Exit()
        {
            Interlocked.Decrement(ref m_enteredThreadCount);
            Thread.EndCriticalRegion();
        }

        private static readonly bool c_isSingleCpuMachine = (Environment.ProcessorCount == 1);

        private static void StallThread(int it)
        {
            if (c_isSingleCpuMachine || (it % 100) == 0)
            {
                // need to occassionally use 1 so we don't 
                // get into a priority race
                // 0 forces a context switch, BUT: starves low priority threads.
                Thread.Sleep((it%10) == 0 ? 1 : 0); 
            }
            else
            {
                // busy wait on multi-cpu machines
                Thread.SpinWait(20); 
            }
        }
    }
}