// *************************************************************
// P99 Auctions Community Team
// -----------
// Solution:     Project 1999 Auction Tracker
// File:         IDataDispatcher.cs
// Email:        info@p99auctions.com
// Website:      www.p99auctions.com
// Last Update:  01/25/2016 10:02 PM
// *************************************************************

using System;
using System.Threading.Tasks;
using P99Auctions.Client.Watchers;
using P99Auctions.Client.Web;

namespace P99Auctions.Client.Interfaces
{
    /// <summary>
    ///     A dispatcher takes a batch of auction data and dispatches it to the underlying carrier
    /// </summary>
    public interface IDataDispatcher
    {
        /// <summary>
        /// Occurs when the status fo the dispatcher is changed
        /// </summary>
        event EventHandler<DataDispatchEventArgs> StatusChanged;

        event EventHandler<MessageRecievedEventArgs> MessageReceived;


        /// <summary>
        /// Sends the auction line to the server
        /// </summary>
        /// <param name="line">The line.</param>
        void SendAuctionLine(string line);

        /// <summary>
        /// Starts this instance, connecting to the server and waiting for information
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool Start();

        /// <summary>
        /// Stops this instance, closing the connect
        /// </summary>
        void Stop();

        /// <summary>
        /// Resets this instance.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool Reset();

        /// <summary>
        /// Gets or sets the account key to be transmitted with each request
        /// </summary>
        /// <value>The account key.</value>
        string ApiKey { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is enabled and connected to the server
        /// </summary>
        /// <value><c>true</c> if this instance is enabled; otherwise, <c>false</c>.</value>
        bool IsEnabled { get; }

        bool WatchingForMessages { get; }

        /// <summary>
        /// Ensures the dispatcher has been started, starts it if it was not already.
        /// </summary>
        bool EnsureStart(string apiKey);
        
    }
}