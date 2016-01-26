// *************************************************************
// P99 Auctions Community Team
// -----------
// Solution:     Project 1999 Auction Tracker
// File:         MessageSeverity.cs
// Email:        info@p99auctions.com
// Website:      www.p99auctions.com
// Last Update:  01/25/2016 10:02 PM
// *************************************************************

namespace P99Auctions.Client.Models
{
    /// <summary>
    /// Enum MessageSeverity
    /// </summary>
    public enum MessageSeverity
    {
        Informational,
        Caution,
        Error,
        Success,
        Disabled,
        NotSet
    }

    /// <summary>
    /// Class MessageSeverityExtensions.
    /// </summary>
    public static class MessageSeverityExtensions
    {
        /// <summary>
        /// Determines whether the specified severity is set.
        /// </summary>
        /// <param name="severity">The severity.</param>
        /// <returns><c>true</c> if the specified severity is set; otherwise, <c>false</c>.</returns>
        public static bool IsSet(this MessageSeverity severity)
        {
            return severity != MessageSeverity.NotSet;
        }
    }
}