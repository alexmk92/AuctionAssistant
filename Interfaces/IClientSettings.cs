// *************************************************************
// P99 Auctions Community Team
// -----------
// Solution:     Project 1999 Auction Tracker
// File:         IClientSettings.cs
// Email:        info@p99auctions.com
// Website:      www.p99auctions.com
// Last Update:  01/25/2016 10:01 PM
// *************************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace P99Auctions.Client.Interfaces
{
    /// <summary>
    /// Client settings interface (global settings managed by the user)
    /// </summary>
    public interface IClientSettings : INotifyPropertyChanged, ICloneable
    {
        /// <summary>
        /// Retreives the validation errors, if any
        /// </summary>
        /// <returns>List&lt;System.String&gt;.</returns>
        List<string> RetreiveValidationErrors();

        /// <summary>
        ///     Adds a single auction to the client counters
        /// </summary>
        void AddAuctionCounter();

        /// <summary>
        /// Merges the specified settings to into this instance
        /// </summary>
        /// <param name="settingsToMergeIn">The settings to merge in.</param>
        void Merge(IClientSettings settingsToMergeIn);

        /// <summary>
        /// Gets or sets the API key/Account Key/Stream Id
        /// </summary>
        /// <value>The API key.</value>
        string ApiKey { get; set; }

        /// <summary>
        /// Gets or sets the eq folder.
        /// </summary>
        /// <value>The eq folder.</value>
        string EQFolder { get; set; }

        /// <summary>
        /// Gets or sets the list of characters to ignore and not watch
        /// </summary>
        /// <value>The ignore list.</value>
        string IgnoreList { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to minimize to the tray when closed
        /// </summary>
        /// <value><c>true</c> if [minimize to tray]; otherwise, <c>false</c>.</value>
        bool MinimizeToTray { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether toasts/balloons are shown
        /// </summary>
        /// <value><c>true</c> if [enable toasts]; otherwise, <c>false</c>.</value>
        bool EnableToasts { get; set; }

        /// <summary>
        /// Gets or sets the amount of time a toast is displayed before being automatically dismissed
        /// </summary>
        /// <value>The toast display for seconds.</value>
        int ToastDisplayForSeconds { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the application should start with windows
        /// </summary>
        /// <value><c>true</c> if [start with windows]; otherwise, <c>false</c>.</value>
        bool StartWithWindows { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the system tray icon should update as actions happen
        /// </summary>
        /// <value><c>true</c> if [track state in system tray]; otherwise, <c>false</c>.</value>
        bool TrackStateInSystemTray { get; set; }
        
        /// <summary>
        /// Gets the lifetime auction count.
        /// </summary>
        /// <value>The lifetime auction count.</value>
        int LifetimeAuctionCount { get; }

        /// <summary>
        /// Gets the today auction count.
        /// </summary>
        /// <value>The today auction count.</value>
        int TodayAuctionCount { get; }
    }
}