// *************************************************************
// P99 Auctions Community Team
// -----------
// Solution:     Project 1999 Auction Tracker
// File:         UiMessageQueue.cs
// Email:        info@p99auctions.com
// Website:      www.p99auctions.com
// Last Update:  01/25/2016 10:01 PM
// *************************************************************

using P99Auctions.Client.Models;
using P99Auctions.Client.Utilities;

namespace P99Auctions.Client.Helpers
{
    /// <summary>
    ///     A message queue that regulates the speed of Ui Messages to the client. Used to slow the speed
    ///     of status updates on the UI so they are visible to the user for a period of "readable time"
    /// </summary>
    public class UiMessageQueue : AutoQueue<ApplicationStatusMessage>
    {
        private readonly int _minShowTime;

        /// <summary>
        ///     Initializes a new instance of the <see cref="UiMessageQueue" /> class.
        /// </summary>
        /// <param name="minShowTime">The minimum a status message must remain as the active item, in milliseconds.</param>
        public UiMessageQueue(int minShowTime)
        {
            _minShowTime = minShowTime;
        }


        /// <summary>
        /// Queues the item.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="severity">The severity.</param>
        /// <param name="activityState">State of the activity.</param>
        public void QueueItem(string message, MessageSeverity severity = MessageSeverity.NotSet, ActivityState activityState = ActivityState.NotSet)
        {
            this.QueueItem(message, _minShowTime, severity, activityState);
        }

        /// <summary>
        /// Queues the item.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="minShowTime">The minimum show time.</param>
        /// <param name="severity">The severity.</param>
        /// <param name="activityState">State of the activity.</param>
        public void QueueItem(string message, int minShowTime, MessageSeverity severity = MessageSeverity.NotSet, ActivityState activityState = ActivityState.NotSet)
        {
            base.QueueItem(new ApplicationStatusMessage
            {
                Severity = severity,
                ActivityState = activityState,
                Message = message,
                MinimumShowTime = minShowTime
            });
        }
    }
}