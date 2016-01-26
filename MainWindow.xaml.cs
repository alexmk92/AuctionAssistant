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
        public MainWindow()
        {
            this.InitializeComponent();
        }

        private void MenuFileSettings_Click(object sender, RoutedEventArgs e)
        {
            EditSettings?.Invoke(this, new EventArgs());
        }


        private void MenuFileClose_Click(object sender, RoutedEventArgs e)
        {
            CloseExplicit?.Invoke(this, new EventArgs());
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CloseImplied?.Invoke(this, e);
        }

        private void ViewLog_Click(object sender, RoutedEventArgs e)
        {
            ViewLog?.Invoke(this, new EventArgs());
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            this.UpdateLayout();
        }

        private void MenuHelpOnline_Click(object sender, RoutedEventArgs e)
        {
            Help?.Invoke(this, new EventArgs());
        }

        public event EventHandler EditSettings;
        public event EventHandler CloseExplicit;
        public event EventHandler<CancelEventArgs> CloseImplied;
        public event EventHandler ViewLog;
        public event EventHandler Help;
    }
}