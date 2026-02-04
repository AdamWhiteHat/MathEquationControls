using System;
using System.Reflection;
using System.Threading;

namespace MathEquationControls
{
    /// <summary>
    /// A thread-safe interlock mechanism that allows multiple threads to coordinate access to a shared resource.
    /// </summary>
    public class ThreadsafeInterlock
    {
        /// <summary>
        /// Gets a value indicating whether the resource is currently locked.
        /// </summary>
        public bool IsLocked
        {
            get
            {
                return _internalState.IsOccupied;
            }
        }

        private InternalState _internalState = null;

        public ThreadsafeInterlock()
        {
            _internalState = new InternalState();
        }

        /// <summary>
        /// Gets a disposable lock token that represents a lock on the interlock.
        /// Dispose of the token to release the lock.
        /// Use within a using statement to ensure proper disposal.
        /// </summary>
        /// <returns>A disposable InterlockToken, intended to be used within a using statement.</returns>
        public InterlockToken GetLockToken()
        {
            return new InterlockToken(this);
        }

        /// <summary>
        /// A disposable class that takes a lock on creation and releases it upon disposal.
        /// This class is designed to be used within a using statement, 
        /// so it is disposed of automatically upon leaving the using block scope.
        /// </summary>
        public class InterlockToken : IDisposable
        {
            private bool _isDisposed = true;
            private ThreadsafeInterlock _owner;

            /// <summary>
            /// Initializes a new instance of the <see cref="InterlockToken"/> class and enters the protected section.
            /// </summary>
            /// <remarks>Use this constructor to create a token that manages entry into a critical
            /// section or protected resource.  The token should be disposed when no longer needed to ensure proper
            /// release of the protected section.</remarks>
            internal InterlockToken(ThreadsafeInterlock owner)
            {
                _isDisposed = false;
                _owner = owner;
                _owner._internalState.EnterSection();
            }

            /// <summary>
            /// Disposes the interlock token, releasing the lock.
            /// </summary>
            public void Dispose()
            {
                if (!_isDisposed)
                {
                    _isDisposed = true;
                    _owner._internalState.ExitSection();
                }
            }
        }

        /// <summary>
        /// Tracks internal state of the interlock.
        /// The initial state of the internal counter is zero.
        /// EnterSection increments an internal counter as an atomic operation.
        /// ExitSection decrements the internal counter as an atomic operation.
        /// When the internal counter is zero, the interlock is considered not occupied.
        /// </summary>
        private class InternalState
        {
            /// <summary>
            /// Gets a value indicating whether the resource is currently occupied.
            /// </summary>
            public bool IsOccupied
            {
                get
                {
                    return TestIsOccupied();
                }
            }

            private long _incrementValue = 0;

            internal InternalState()
            {
            }

            private bool TestIsOccupied()
            {
                return !(Interlocked.Read(ref _incrementValue) == 0);
            }

            /// <summary>
            /// Marks the entry into a protected section, incrementing the internal section counter.
            /// </summary>
            public void EnterSection()
            {
                long currentValue = Interlocked.Read(ref _incrementValue);

                if (Interlocked.Increment(ref _incrementValue) > 0)
                {
                    if (currentValue == 0)
                    {
                        //RaiseStateChanged(true);
                    }
                }
            }

            /// <summary>
            /// Signals that the current thread has exited a protected section, decrementing the internal section counter.
            /// </summary>
            public void ExitSection()
            {
                if (Interlocked.Decrement(ref _incrementValue) == 0)
                {
                    //RaiseStateChanged(false);
                }
            }
        }
    }
}
