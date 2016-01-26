// *************************************************************
// P99 Auctions Community Team
// -----------
// Solution:     Project 1999 Auction Tracker
// File:         AuctionBatchEventArgs.cs
// Email:        info@p99auctions.com
// Website:      www.p99auctions.com
// Last Update:  01/25/2016 10:02 PM
// *************************************************************

using System.Collections.Generic;

namespace P99Auctions.Client.Watchers
{
    /// <summary>
    ///     Class AuctionBatchEventArgs.
    /// </summary>
    public class AuctionBatchEventArgs : AuctionLogFileEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuctionBatchEventArgs" /> class.
        /// </summary>
        /// <param name="lines">The lines.</param>
        public AuctionBatchEventArgs(List<string> lines)
        {
            this.Batch = lines;
        }

        /// <summary>
        /// Gets or sets the batch.
        /// </summary>
        /// <value>The batch.</value>
        public List<string> Batch { get; private set; }
    }
}