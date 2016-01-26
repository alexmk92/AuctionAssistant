// *************************************************************
// P99 Auctions Community Team
// -----------
// Solution:     Project 1999 Auction Tracker
// File:         ApplicationStatusMessage.cs
// Email:        info@p99auctions.com
// Website:      www.p99auctions.com
// Last Update:  01/25/2016 10:01 PM
// *************************************************************

using P99Auctions.Client.Models;
using P99Auctions.Client.Utilities;

namespace P99Auctions.Client.Helpers
{
    /// <summary>
    /// An informational object for ui messages shown in the application
    /// </summary>
    public class ApplicationStatusMessage : IAutoQueueItem
    {
        /// <summary>
        ///     Gets or sets the minimum show time.
        /// </summary>
        /// <value>The minimum show time.</value>
        public int MinimumShowTime { get; set; }

        /// <summary>
        ///     Gets or sets the message.
        /// </summary>
        /// <value>The message.</value>
        public string Message { get; set; }

        /// <summary>
        ///     Gets or sets the severity.
        /// </summary>
        /// <value>The severity.</value>
        public MessageSeverity Severity { get; set; }

        /// <summary>
        ///     Gets or sets the state of the activity.
        /// </summary>
        /// <value>The state of the activity.</value>
        public ActivityState ActivityState { get; set; }

        /// <summary>
        ///     Gets or sets the operation percent complete.
        /// </summary>
        /// <value>The operation percent complete.</value>
        public int? OperationPercentComplete { get; set; }
    }
}