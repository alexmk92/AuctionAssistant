// *************************************************************
// P99 Auctions Community Team
// -----------
// Solution:     Project 1999 Auction Tracker
// File:         AuctionSubmissionPackage.cs
// Email:        info@p99auctions.com
// Website:      www.p99auctions.com
// Last Update:  01/25/2016 10:02 PM
// *************************************************************

using System.Collections.Generic;
using P99Auctions.Client.Interfaces;

namespace P99Auctions.Client.Web
{
    /// <summary>
    ///     The final package sent to the p99 auction tracker
    /// </summary>
    public class AuctionSubmissionPackage : ISubmissionPackage
    {
        /// <summary>
        ///     Gets or sets the stream identifier.
        /// </summary>
        /// <value>The stream identifier.</value>
        public string StreamId { get; set; }

        /// <summary>
        /// Gets or sets the application version.
        /// </summary>
        /// <value>The application version.</value>
        public string ApplicationVersion { get; set; }

        /// <summary>
        ///     Gets or sets the auction lines.
        /// </summary>
        /// <value>The auction lines.</value>
        public List<string> AuctionLines { get; set; }
    }
}