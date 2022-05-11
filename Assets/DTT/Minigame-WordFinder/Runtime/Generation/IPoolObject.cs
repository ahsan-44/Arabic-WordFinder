using System;

namespace DTT.MiniGame.WordFinder
{
    /// <summary>
    /// Implement to make the object poolable.
    /// </summary>
    internal interface IPoolObject
    {
        /// <summary>
        /// Invoke this once the object has been reset.
        /// </summary>
        event Action Reset;

        /// <summary>
        /// Called when the object is pulled out of the pool.
        /// </summary>
        void ResetObject();
    }
}