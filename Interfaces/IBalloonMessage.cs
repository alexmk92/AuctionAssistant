// *************************************************************
// P99 Auctions Community Team
// -----------
// Solution:     Project 1999 Auction Tracker
// File:         IBalloonMessage.cs
// Email:        info@p99auctions.com
// Website:      www.p99auctions.com
// Last Update:  01/25/2016 10:02 PM
// *************************************************************

namespace P99Auctions.Client.Interfaces
{
    /// <summary>
    /// Interface IBalloonMessage
    /// </summary>
    public interface IBalloonMessage
    {
        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        string Id { get; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        string Title { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>The message.</value>
        string Message { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        string Url { get; set; }

        /// <summary>
        /// Gets or sets the amount of time to show the message in milliseconds.
        /// </summary>
        /// <value>The show for milliseconds.</value>
        int ShowForMilliseconds { get; set; }
    }
}