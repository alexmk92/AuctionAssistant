// *************************************************************
// P99 Auctions Community Team
// -----------
// Solution:     Project 1999 Auction Tracker
// File:         IViewResolver.cs
// Email:        info@p99auctions.com
// Website:      www.p99auctions.com
// Last Update:  01/25/2016 10:02 PM
// *************************************************************

namespace P99Auctions.Client.Interfaces
{
    /// <summary>
    ///     Interface IViewResolver
    /// </summary>
    public interface IViewResolver
    {
        /// <summary>
        ///     Creates the main view.
        /// </summary>
        /// <returns>IMainView.</returns>
        IMainView CreateMainView();

        /// <summary>
        ///     Creates the settings view.
        /// </summary>
        /// <returns>ISettingsView.</returns>
        ISettingsView CreateSettingsView();
    }
}