// *************************************************************
// P99 Auctions Community Team
// -----------
// Solution:     Project 1999 Auction Tracker
// File:         IMessageMonitor.cs
// Email:        info@p99auctions.com
// Website:      www.p99auctions.com
// Last Update:  01/25/2016 10:02 PM
// *************************************************************

using System;
using P99Auctions.Client.Watchers;

namespace P99Auctions.Client.Interfaces
{
    /// <summary>
    /// Interface IMessageMonitor
    /// </summary>
    public interface IMessageMonitor
    {
        event EventHandler<MessageRecievedEventArgs> MessageReceived;

        /// <summary>
        /// Starts this instance.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops this instance.
        /// </summary>
        void Stop();
    }
}