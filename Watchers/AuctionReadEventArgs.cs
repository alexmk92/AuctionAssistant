// *************************************************************
// P99 Auctions Community Team
// -----------
// Solution:     Project 1999 Auction Tracker
// File:         AuctionReadEventArgs.cs
// Email:        info@p99auctions.com
// Website:      www.p99auctions.com
// Last Update:  01/25/2016 10:02 PM
// *************************************************************

namespace P99Auctions.Client.Watchers
{
    /// <summary>
    ///     Class AuctionReadEventArgs.
    /// </summary>
    public class AuctionReadEventArgs : AuctionLogFileEventArgs
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="AuctionReadEventArgs" /> class.
        /// </summary>
        /// <param name="auctionLine">The auction line.</param>
        public AuctionReadEventArgs(string auctionLine)
        {
            this.Line = auctionLine;
        }

        /// <summary>
        ///     Gets the line.
        /// </summary>
        /// <value>The line.</value>
        public string Line { get; private set; }
    }
}