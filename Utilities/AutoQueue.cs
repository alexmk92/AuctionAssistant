// *************************************************************
// P99 Auctions Community Team
// -----------
// Solution:     Project 1999 Auction Tracker
// File:         AutoQueue.cs
// Email:        info@p99auctions.com
// Website:      www.p99auctions.com
// Last Update:  01/25/2016 10:02 PM
// *************************************************************

using System;
using System.Collections.Concurrent;
using System.Timers;

namespace P99Auctions.Client.Utilities
{
    /// <summary>
    /// A queue with an auto dequeue feature for regulating message consumption
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AutoQueue<T> where T : IAutoQueueItem
    {
        T _currentItem;
        DateTime _dtLastPop = DateTime.MinValue;
        ConcurrentQueue<T> _queue = new ConcurrentQueue<T>();
        Timer _timer;

        public event ItemDequeuedEventHandler<T> ActiveItemChanged;


        /// <summary>
        ///     Initializes a new instance of the <see cref="AutoQueue{T}" /> class.
        /// </summary>
        public AutoQueue()
        {
        }


        /// <summary>
        ///     Called when [active item changed].
        /// </summary>
        /// <param name="item">The item.</param>
        private void OnActiveItemChanged(T item)
        {
            ActiveItemChanged?.Invoke(this, new AutoQueueEventArgs<T>
            {
                Item = item
            });
        }


        /// <summary>
        ///     Starts this instance.
        /// </summary>
        /// <returns>System.Boolean.</returns>
        public bool Start()
        {
            this.Stop();
            _timer = new Timer();
            _timer.Interval = 200;
            _timer.Elapsed += this.Timer_Elapsed;
            _timer.AutoReset = true;
            _timer.Start();

            return _timer.Enabled;
        }

        /// <summary>
        ///     Stops this instance.
        /// </summary>
        public void Stop()
        {
            if (_timer != null && _timer.Enabled)
            {
                _timer.Stop();
                _timer.AutoReset = false;
            }
        }

        /// <summary>
        ///     Flushes this instance and clears any pending items
        /// </summary>
        public void Flush()
        {
            _queue = new ConcurrentQueue<T>();
        }

        /// <summary>
        /// Timer_s the elapsed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Timers.ElapsedEventArgs" /> instance containing the event data.</param>
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_queue.Count > 0)
            {
                var elapsed = DateTime.Now.Subtract(_dtLastPop);
                if (_currentItem == null || elapsed.TotalMilliseconds > _currentItem.MinimumShowTime)
                {
                    T item;
                    if (_queue.TryDequeue(out item))
                    {
                        _currentItem = item;
                        this.OnActiveItemChanged(item);
                    }
                }
            }
        }


        /// <summary>
        ///     Finalizes an instance of the <see cref="AutoQueue{T}" /> class.
        /// </summary>
        ~AutoQueue()
        {
            if (_timer != null && _timer.Enabled)
                _timer.Stop();
            _timer = null;
        }

        /// <summary>
        ///     Queues the message.
        /// </summary>
        /// <param name="T">The t.</param>
        public void QueueItem(T item)
        {
            _queue.Enqueue(item);
        }

        /// <summary>
        ///     Gets the active item.
        /// </summary>
        /// <value>The active item.</value>
        public T ActiveItem
        {
            get { return _currentItem; }
        }
    }
}