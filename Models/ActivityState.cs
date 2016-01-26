// *************************************************************
// P99 Auctions Community Team
// -----------
// Solution:     Project 1999 Auction Tracker
// File:         ActivityState.cs
// Email:        info@p99auctions.com
// Website:      www.p99auctions.com
// Last Update:  01/25/2016 10:02 PM
// *************************************************************

namespace P99Auctions.Client.Models
{
    /// <summary>
    /// Enum ActivityState
    /// </summary>
    public enum ActivityState
    {
        Idle,
        Transmitting,
        AuctionFound,
        Error,
        NotSet,
        Success,
        Disabled
    }

    /// <summary>
    /// Class ActivityStateExtensions.
    /// </summary>
    public static class ActivityStateExtensions
    {
        /// <summary>
        /// Determines whether the specified state is set.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns><c>true</c> if the specified state is set; otherwise, <c>false</c>.</returns>
        public static bool IsSet(this ActivityState state)
        {
            return state != ActivityState.NotSet;
        }
    }
}