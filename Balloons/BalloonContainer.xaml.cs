// *************************************************************
// P99 Auctions Community Team
// -----------
// Solution:     Project 1999 Auction Tracker
// File:         BalloonContainer.xaml.cs
// Email:        info@p99auctions.com
// Website:      www.p99auctions.com
// Last Update:  01/25/2016 10:02 PM
// *************************************************************

using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Hardcodet.Wpf.TaskbarNotification;
using P99Auctions.Client.Interfaces;

namespace P99Auctions.Client.Balloons
{
    /// <summary>
    /// Interaction logic for BalloonContainer.xaml
    /// </summary>
    public partial class BalloonContainer : UserControl
    {
        public static readonly DependencyProperty BalloonListProperty =
            DependencyProperty.Register("BalloonList",
                typeof (ObservableCollection<IBalloonMessage>),
                typeof (BalloonContainer),
                new FrameworkPropertyMetadata(new ObservableCollection<IBalloonMessage>()));


        public BalloonContainer()
        {
            this.InitializeComponent();
        }

        public void AddBalloon(IBalloonMessage balloon)
        {
            var lst = this.BalloonList;
            lst.Add(balloon);
        }

        private void BalloonContainer_OnHovered(object sender, RoutedEventArgs e)
        {
            //the tray icon assigned this attached property to simplify access
            TaskbarIcon taskbarIcon = TaskbarIcon.GetParentTaskbarIcon(this);
            taskbarIcon.ResetBalloonCloseTimer();
        }

        private void BalloonContainer_OnClicked(object sender, RoutedEventArgs e)
        {
            var balloon = e.OriginalSource as AuctionAlertBalloon;
            var bMessage = balloon?.DataContext as IBalloonMessage;
            if (bMessage != null)
            {
                //launch the url
                if (!balloon.IsClosing)
                    System.Diagnostics.Process.Start(bMessage.Url);
            }
        }

        private void BalloonContainer_OnClosing(object sender, RoutedEventArgs e)
        {
            var balloon = e.OriginalSource as AuctionAlertBalloon;
            var bMessage = balloon?.DataContext as IBalloonMessage;
            e.Handled = true; //suppresses the popup from being closed immediately
            if (bMessage != null)
            {
                var lst = this.BalloonList;
                lst.Remove(bMessage);
                if (lst.Count == 0)
                {
                    TaskbarIcon taskbarIcon = TaskbarIcon.GetParentTaskbarIcon(this);
                    taskbarIcon.CloseBalloon();
                }
            }
        }

        public ObservableCollection<IBalloonMessage> BalloonList
        {
            get { return (ObservableCollection<IBalloonMessage>) this.GetValue(BalloonListProperty); }
            set { this.SetValue(BalloonListProperty, value); }
        }
    }
}