// *************************************************************
// P99 Auctions Community Team
// -----------
// Solution:     Project 1999 Auction Tracker
// File:         MessageRetrievalPackage.cs
// Email:        info@p99auctions.com
// Website:      www.p99auctions.com
// Last Update:  01/25/2016 10:02 PM
// *************************************************************

using P99Auctions.Client.Interfaces;

namespace P99Auctions.Client.Web
{
    /// <summary>
    /// Class MessageRetrievalPackage.
    /// </summary>
    public class MessageRetrievalPackage : ISubmissionPackage
    {
        /// <summary>
        /// Gets or sets the stream identifier.
        /// </summary>
        /// <value>The stream identifier.</value>
        public string StreamId { get; set; }

        /// <summary>
        /// Gets or sets the application version.
        /// </summary>
        /// <value>The application version.</value>
        public string ApplicationVersion { get; set; }
    }
}