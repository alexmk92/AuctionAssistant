// *************************************************************
// P99 Auctions Community Team
// -----------
// Solution:     Project 1999 Auction Tracker
// File:         IAutoQueueItem.cs
// Email:        info@p99auctions.com
// Website:      www.p99auctions.com
// Last Update:  01/25/2016 10:02 PM
// *************************************************************

namespace P99Auctions.Client.Utilities
{
    /// <summary>
    /// Interface IAutoQueueItem
    /// </summary>
    public interface IAutoQueueItem
    {
        /// <summary>
        /// Gets or sets the minimum show time.
        /// </summary>
        /// <value>The minimum show time.</value>
        int MinimumShowTime { get; set; }
    }
}