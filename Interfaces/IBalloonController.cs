// *************************************************************
// P99 Auctions Community Team
// -----------
// Solution:     Project 1999 Auction Tracker
// File:         IBalloonController.cs
// Email:        info@p99auctions.com
// Website:      www.p99auctions.com
// Last Update:  01/25/2016 10:02 PM
// *************************************************************

namespace P99Auctions.Client.Interfaces
{
    /// <summary>
    /// Interface IBalloonController
    /// </summary>
    public interface IBalloonController
    {
        /// <summary>
        /// Shows the balloon message.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        void ShowBalloonMessage(IBalloonMessage msg);
    }
}