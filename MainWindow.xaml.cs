// *************************************************************
// P99 Auctions Community Team
// -----------
// Solution:     Project 1999 Auction Tracker
// File:         MainWindow.xaml.cs
// Email:        info@p99auctions.com
// Website:      www.p99auctions.com
// Last Update:  01/25/2016 10:02 PM
// *************************************************************

using System;
using System.ComponentModel;
using System.Windows;
using P99Auctions.Client.Interfaces;

namespace P99Auctions.Client
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IMainView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Handles the Click event of the MenuFileSettings control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void MenuFileSettings_Click(object sender, RoutedEventArgs e)
        {
            EditSettings?.Invoke(this, new EventArgs());
        }


        /// <summary>
        /// Handles the Click event of the MenuFileClose control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void MenuFileClose_Click(object sender, RoutedEventArgs e)
        {
            CloseExplicit?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Handles the Closing event of the MainWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CloseImplied?.Invoke(this, e);
        }

        /// <summary>
        /// Handles the Click event of the ViewLog control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void ViewLog_Click(object sender, RoutedEventArgs e)
        {
            ViewLog?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Handles the OnLoaded event of the MainWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            this.UpdateLayout();
        }

        /// <summary>
        /// Handles the Click event of the MenuHelpOnline control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void MenuHelpOnline_Click(object sender, RoutedEventArgs e)
        {
            Help?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Handles the Click event of the MenuAbout control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void MenuAbout_Click(object sender, RoutedEventArgs e)
        {
            About?.Invoke(this, new EventArgs());
        }

        public event EventHandler EditSettings;
        public event EventHandler CloseExplicit;
        public event EventHandler<CancelEventArgs> CloseImplied;
        public event EventHandler ViewLog;
        public event EventHandler Help;
        public event EventHandler About;
    }
}