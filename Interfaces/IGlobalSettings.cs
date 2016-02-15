// *************************************************************
// P99 Auctions Community Team
// -----------
// Solution:     Project 1999 Auction Tracker
// File:         IGlobalSettings.cs
// Email:        info@p99auctions.com
// Website:      www.p99auctions.com
// Last Update:  01/25/2016 10:02 PM
// *************************************************************

namespace P99Auctions.Client.Interfaces
{
    /// <summary>
    /// An interface for the global, non-user controlled settings
    /// </summary>
    public interface IGlobalSettings
    {
        /// <summary>
        /// Gets the url the dispatcher connects to
        /// </summary>
        /// <value>The service URL base.</value>
        string ServiceUrlBase { get; }

        /// <summary>
        /// Gets the amount of time, in seconds, between auction dispatches
        /// </summary>
        /// <value>The dispatch delay.</value>
        int DispatchDelay { get; }

        /// <summary>
        /// Gets the maximum auctions per batch sent to the server.
        /// </summary>
        /// <value>The maximum auctions per batch.</value>
        int MaxAuctionsPerBatch { get; }

        /// <summary>
        /// Gets the name of the log file  to be written to
        /// </summary>
        /// <value>The name of the log file.</value>
        string LogFileName { get; }

        /// <summary>
        /// Gets the inactivity timeout, in seconds, before a watched file is closed
        /// </summary>
        /// <value>The inactivity timeout.</value>
        int InactivityTimeout { get; }

        /// <summary>
        /// Gets the check interval, in seconds, when looking for auction messages
        /// </summary>
        /// <value>The toast check interval.</value>
        int ToastCheckInterval { get; }

        /// <summary>
        /// Gets the number of times(lines) before the dispatcher will cease attempting to try to send data
        /// </summary>
        /// <value>The dispatch retry count.</value>
        int DispatchRetryCount { get; }

        /// <summary>
        /// Gets the interval, in seconds, that the application will stay disabled if communications are unavailable
        /// </summary>
        /// <value>The disabled communications interval.</value>
        int DisabledCommunicationsInterval { get; }

        /// <summary>
        /// Gets a value indicating whether to use a presistant connection or, if false, periodic polling
        /// </summary>
        /// <value><c>true</c> if [use presistant connection]; otherwise, <c>false</c>.</value>
        bool UsePersistantConnection { get; }
    }
}