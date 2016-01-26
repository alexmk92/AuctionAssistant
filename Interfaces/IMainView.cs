// *************************************************************
// P99 Auctions Community Team
// -----------
// Solution:     Project 1999 Auction Tracker
// File:         IMainView.cs
// Email:        info@p99auctions.com
// Website:      www.p99auctions.com
// Last Update:  01/25/2016 10:02 PM
// *************************************************************

using System;
using System.ComponentModel;

namespace P99Auctions.Client.Interfaces
{
    /// <summary>
    /// Interface IMainView
    /// </summary>
    public interface IMainView
    {
        event EventHandler EditSettings;
        event EventHandler CloseExplicit;
        event EventHandler<CancelEventArgs> CloseImplied;
        event EventHandler ViewLog;
        event EventHandler Help;

        /// <summary>
        /// Shows this instance.
        /// </summary>
        void Show();

        /// <summary>
        /// Closes this instance.
        /// </summary>
        void Close();

        /// <summary>
        /// Gets or sets the data context.
        /// </summary>
        /// <value>The data context.</value>
        object DataContext { get; set; }
    }
}