// *************************************************************
// P99 Auctions Community Team
// -----------
// Solution:     Project 1999 Auction Tracker
// File:         MessageRecievedEventArgs.cs
// Email:        info@p99auctions.com
// Website:      www.p99auctions.com
// Last Update:  01/25/2016 10:02 PM
// *************************************************************

using System;
using P99Auctions.Client.Web;

namespace P99Auctions.Client.Watchers
{
    /// <summary>
    /// Class MessageRecievedEventArgs.
    /// </summary>
    public class MessageRecievedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageRecievedEventArgs"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public MessageRecievedEventArgs(ClientTrackerMessage message)
        {
            this.Message = message;
        }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>The message.</value>
        public ClientTrackerMessage Message { get; set; }
    }
}