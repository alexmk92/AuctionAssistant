// *************************************************************
// P99 Auctions Community Team
// -----------
// Solution:     Project 1999 Auction Tracker
// File:         ISubmissionPackage.cs
// Email:        info@p99auctions.com
// Website:      www.p99auctions.com
// Last Update:  01/25/2016 10:02 PM
// *************************************************************

namespace P99Auctions.Client.Interfaces
{
    /// <summary>
    /// Interface ISubmissionPackage
    /// </summary>
    public interface ISubmissionPackage
    {
        /// <summary>
        /// Gets or sets the stream identifier.
        /// </summary>
        /// <value>The stream identifier.</value>
        string StreamId { get; set; }

        /// <summary>
        /// Gets or sets the application version.
        /// </summary>
        /// <value>The application version.</value>
        string ApplicationVersion { get; set; }
    }
}