// *************************************************************
// P99 Auctions Community Team
// -----------
// Solution:     Project 1999 Auction Tracker
// File:         AuctionServicesTransmissionEventArgs.cs
// Email:        info@p99auctions.com
// Website:      www.p99auctions.com
// Last Update:  01/25/2016 10:02 PM
// *************************************************************

namespace P99Auctions.Client.Web
{
    /// <summary>
    ///     Class AuctionServicesTransmissionEventArgs.
    /// </summary>
    public class AuctionDispatchEventArgs
    {
        /// <summary>
        ///     Gets or sets the message.
        /// </summary>
        /// <value>The message.</value>
        public string Message { get; set; }

        /// <summary>
        ///     Gets or sets the code.
        /// </summary>
        /// <value>The code.</value>
        public DispatcherStatus Code { get; set; }

        /// <summary>
        ///     Gets or sets the percent complete. (0-100)
        /// </summary>
        /// <value>The percent complete.</value>
        public int PercentComplete { get; set; }
    }
}