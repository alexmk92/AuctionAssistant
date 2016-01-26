// *************************************************************
// P99 Auctions Community Team
// -----------
// Solution:     Project 1999 Auction Tracker
// File:         INotifyIconView.cs
// Email:        info@p99auctions.com
// Website:      www.p99auctions.com
// Last Update:  01/25/2016 10:02 PM
// *************************************************************

using P99Auctions.Client.Models;

namespace P99Auctions.Client.Interfaces
{
    /// <summary>
    /// Interface INotifyIconView
    /// </summary>
    public interface INotifyIconView
    {
        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <returns>NotifyIconViewModel.</returns>
        NotifyIconViewModel GetModel();
    }
}