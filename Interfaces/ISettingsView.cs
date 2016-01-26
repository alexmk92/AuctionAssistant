// *************************************************************
// P99 Auctions Community Team
// -----------
// Solution:     Project 1999 Auction Tracker
// File:         ISettingsView.cs
// Email:        info@p99auctions.com
// Website:      www.p99auctions.com
// Last Update:  01/25/2016 10:02 PM
// *************************************************************

namespace P99Auctions.Client.Interfaces
{
    /// <summary>
    /// Interface ISettingsView
    /// </summary>
    public interface ISettingsView
    {
        /// <summary>
        /// Shows the dialog.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool? ShowDialog(IMainView owner);

        /// <summary>
        /// Shows the specified show in task bar.
        /// </summary>
        /// <param name="ShowInTaskBar">if set to <c>true</c> [show in task bar].</param>
        void Show(bool ShowInTaskBar);

        /// <summary>
        /// Setups the specified settings.
        /// </summary>
        /// <param name="settings">The settings.</param>
        void Setup(IClientSettings settings);

        /// <summary>
        /// Gets the client settings.
        /// </summary>
        /// <value>The client settings.</value>
        IClientSettings ClientSettings { get; }
    }
}