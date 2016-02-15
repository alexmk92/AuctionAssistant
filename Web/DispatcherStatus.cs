// *************************************************************
// P99 Auctions Community Team
// -----------
// Solution:     Project 1999 Auction Tracker
// File:         DispatcherStatus.cs
// Email:        info@p99auctions.com
// Website:      www.p99auctions.com
// Last Update:  01/25/2016 10:02 PM
// *************************************************************

namespace P99Auctions.Client.Web
{
    /// <summary>
    /// Enum DispatcherStatus
    /// </summary>
    public enum DispatcherStatus
    {
        Idle,
        Connecting,
        Transmitting,
        Complete,
        Error,
        RetryFailure,
        InvalidServiceAddress,
        ConfigurationError,
        Disconnected,
        Closed
    }

    /// <summary>
    /// Class DispatcherStatusExtensions.
    /// </summary>
    public static class DispatcherStatusExtensions
    {
        /// <summary>
        ///     Determines whether the specified status is a transmission state status
        /// </summary>
        /// <param name="status">The status.</param>
        /// <returns><c>true</c> if the specified status is transmitting; otherwise, <c>false</c>.</returns>
        public static bool IsTransmitting(this DispatcherStatus status)
        {
            return status == DispatcherStatus.Connecting || status == DispatcherStatus.Transmitting;
        }
    }
}