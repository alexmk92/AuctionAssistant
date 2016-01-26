// *************************************************************
// P99 Auctions Community Team
// -----------
// Solution:     Project 1999 Auction Tracker
// File:         IAuctionQueue.cs
// Email:        info@p99auctions.com
// Website:      www.p99auctions.com
// Last Update:  01/25/2016 10:02 PM
// *************************************************************

namespace P99Auctions.Client.Interfaces
{
    /// <summary>
    /// Interface IAuctionQueue
    /// </summary>
    public interface IAuctionQueue
    {
        /// <summary>
        ///     Enqueues an auction for batched dispatching
        /// </summary>
        /// <param name="auctionLine">The auction line.</param>
        void EnqueueAuction(string auctionLine);

        /// <summary>
        ///     Flushes this instance.
        /// </summary>
        void Flush();
    }
}