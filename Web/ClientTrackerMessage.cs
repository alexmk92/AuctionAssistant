// *************************************************************
// P99 Auctions Community Team
// -----------
// Solution:     Project 1999 Auction Tracker
// File:         ClientTrackerMessage.cs
// Email:        info@p99auctions.com
// Website:      www.p99auctions.com
// Last Update:  01/25/2016 10:02 PM
// *************************************************************

namespace P99Auctions.Client.Web
{
    /// <summary>
    /// A message recieved by the client tracker
    /// </summary>
    public class ClientTrackerMessage
    {
        /// <summary>
        /// Gets or sets the type of the message.
        /// </summary>
        /// <value>The type of the message.</value>
        public ClientTrackerMessageType MessageType { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>The message.</value>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public string Url { get; set; }
    }

    public enum ClientTrackerMessageType
    {
        Info = 0,
        Error = 1,
        Warning = 2,
        Success = 3,
        ItemForSale = 4,
        ItemBeingBought = 5
    }
}