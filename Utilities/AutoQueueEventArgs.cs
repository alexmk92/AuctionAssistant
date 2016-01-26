// *************************************************************
// P99 Auctions Community Team
// -----------
// Solution:     Project 1999 Auction Tracker
// File:         AutoQueueEventArgs.cs
// Email:        info@p99auctions.com
// Website:      www.p99auctions.com
// Last Update:  01/25/2016 10:02 PM
// *************************************************************

using System;

namespace P99Auctions.Client.Utilities
{
    /// <summary>
    /// Class AutoQueueEventArgs.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AutoQueueEventArgs<T> : EventArgs
        where T : IAutoQueueItem
    {
        /// <summary>
        /// Gets or sets the item.
        /// </summary>
        /// <value>The item.</value>
        public T Item { get; set; }
    }
}