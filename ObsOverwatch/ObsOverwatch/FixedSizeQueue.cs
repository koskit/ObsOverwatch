using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace ObsOverwatch
{
    /// <summary>
    /// Class that has a fixed size queue.
    /// </summary>
    /// <typeparam name="T">The type of the element the queue will hold.</typeparam>
    public class FixedSizeQueue<T> : IReadOnlyCollection<T>
    {
        #region Private Fields/Properties

        private ConcurrentQueue<T> Queue = new ConcurrentQueue<T>();
        private int _Count;

        #endregion Private Fields/Properties

        #region Public Properties

        /// <summary>
        /// The count of the elements in the queue.
        /// </summary>
        public int Count => _Count;

        /// <summary>
        /// Gets a bool that indicates if the queue is full (i.e. if the count of the queue reached the limit.
        /// </summary>
        public bool IsFull => _Count == Limit;

        /// <summary>
        /// The limit of the queue that was set at construction time.
        /// </summary>
        public int Limit { get; private set; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Clears the queue.
        /// </summary>
        public void Clear()
        {
            Queue = new ConcurrentQueue<T>();
            _Count = 0;
        }

        /// <summary>
        /// Enqueues a new element into the queue. If the limit is reached, dequeues the last item.
        /// </summary>
        /// <param name="obj">The item to add to the queue.</param>
        public void Enqueue(T obj)
        {
            Queue.Enqueue(obj);
            Interlocked.Increment(ref _Count);

            // Calculate the number of items to be removed by this thread in a thread safe manner
            int currentCount;
            int finalCount;
            do
            {
                currentCount = _Count;
                finalCount = Math.Min(currentCount, Limit);
            } while (currentCount != Interlocked.CompareExchange(ref _Count, finalCount, currentCount));

            while (currentCount > finalCount && Queue.TryDequeue(out _)) currentCount--;
        }

        #endregion Public Methods

        #region GetEnumerator

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="FixedSizeQueue{T}"/>.
        /// </summary>
        /// <returns>Returns an enumerator that iterates through the <see cref="FixedSizeQueue{T}"/>.</returns>
        public IEnumerator<T> GetEnumerator() => Queue.GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="FixedSizeQueue{T}"/>.
        /// </summary>
        /// <returns>Returns an enumerator that iterates through the <see cref="FixedSizeQueue{T}"/>.</returns>
        IEnumerator IEnumerable.GetEnumerator() => Queue.GetEnumerator();

        #endregion GetEnumerator

        #region Constructor

        /// <summary>
        /// Creates a new instance of the class and sets the limit of the queue.
        /// </summary>
        /// <param name="limit">The maximum number of elements that the queue can hold.</param>
        public FixedSizeQueue(int limit) => Limit = limit;

        #endregion Constructor
    }
}
