using System.Threading;

namespace ClrCoder.System.Threading
{
    /// <summary>
    /// Collection of synchronization root objects accessable by arbitrary keys.
    /// </summary>
    /// <remarks>
    /// ATTENTION: It is possible that a same root sync root will be used for different keys.
    /// Random locking order with nested locks can lead to deadlock.
    /// </remarks>
    /// <typeparam name="T">Key type.</typeparam>
    public class KeyedMonitor<T>
    {
        //// TODO: Add strict checks. (double dispose for example).
        
        private readonly Token[] _tokens = new Token[0x1000];

        /// <summary>
        /// Initializes a new instance of the class <see cref="KeyedMonitor{T}"/>.
        /// </summary>
        public KeyedMonitor()
        {
            for (int i = 0; i < _tokens.Length; i++)
            {
                _tokens[i] = new Token();
            }
        }

        /// <summary>
        /// Locks a sync root that corresponds to the specified key.
        /// </summary>
        /// <param name="key">Target resource key.</param>
        /// <returns>Lock token, use <see cref="ILockToken.Dispose"/> to release lock.</returns>
        public ILockToken Lock(T key)
        {
            int tokenIndex = key.GetHashCode() & 0xFFF;
            Monitor.Enter(_tokens[tokenIndex]);
            return _tokens[tokenIndex];
        }

        private class Token : ILockToken
        {
            public void Dispose()
            {
                Monitor.Exit(this);
            }
        }
    }
}