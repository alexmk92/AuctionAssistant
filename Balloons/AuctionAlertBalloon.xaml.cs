// *************************************************************
// P99 Auctions Community Team
// -----------
// Solution:     Project 1999 Auction Tracker
// File:         AuctionAlertBalloon.xaml.cs
// Email:        info@p99auctions.com
// Website:      www.p99auctions.com
// Last Update:  01/25/2016 10:02 PM
// *************************************************************

using System;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Hardcodet.Wpf.TaskbarNotification;
using P99Auctions.Client.Interfaces;

namespace P99Auctions.Client.Balloons
{
    /// <summary>
    /// Interaction logic for AuctionAlertBalloon.xaml
    /// </summary>
    public partial class AuctionAlertBalloon : UserControl
    {
        // This defines the custom event
        public static readonly RoutedEvent HoveredProperty = EventManager.RegisterRoutedEvent(
            "HoveredProperty", // Event name
            RoutingStrategy.Bubble,
            typeof (RoutedEventHandler),
            typeof (AuctionAlertBalloon));

        public static readonly RoutedEvent ClosingProperty = EventManager.RegisterRoutedEvent(
            "ClosedProperty", // Event name
            RoutingStrategy.Bubble,
            typeof (RoutedEventHandler),
            typeof (AuctionAlertBalloon));

        public static readonly RoutedEvent ClickedProperty = EventManager.RegisterRoutedEvent(
            "ClickedProperty", // Event name
            RoutingStrategy.Bubble,
            typeof (RoutedEventHandler),
            typeof (AuctionAlertBalloon));


        Timer _timer = new Timer();


        public event RoutedEventHandler Hovered
        {
            add { this.AddHandler(HoveredProperty, value); }
            remove { this.RemoveHandler(HoveredProperty, value); }
        }


        public event RoutedEventHandler Closing
        {
            add { this.AddHandler(ClosingProperty, value); }
            remove { this.RemoveHandler(ClosingProperty, value); }
        }

        public event RoutedEventHandler Clicked
        {
            add { this.AddHandler(ClickedProperty, value); }
            remove { this.RemoveHandler(ClickedProperty, value); }
        }

        public AuctionAlertBalloon()
        {
            this.InitializeComponent();
            this.Cursor = Cursors.Hand;
            _timer.Interval = 5000;
            _timer.AutoReset = false;
            _timer.Elapsed += this.Timer_Elapsed;
            DataContextChanged += this.AuctionAlertBalloon_DataContextChanged;
        }

        private void AuctionAlertBalloon_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var balloonData = e.NewValue as IBalloonMessage;
            if (balloonData != null)
            {
                _timer.Interval = balloonData.ShowForMilliseconds;
                _timer.Start();
            }
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(() => this.RaiseEvent(new RoutedEventArgs(ClosingProperty)));
        }

        /// <summary>
        /// Resolves the <see cref="TaskbarIcon"/> that displayed
        /// the balloon and requests a close action.
        /// </summary>
        private void imgClose_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //handle the evnt to prevent higher level clicks
            e.Handled = true;
            this.IsClosing = true;

            this.RaiseEvent(new RoutedEventArgs(ClosingProperty));
            //the tray icon assigned this attached property to simplify access
            //TaskbarIcon taskbarIcon = TaskbarIcon.GetParentTaskbarIcon(this);
            //taskbarIcon.CloseBalloon();
        }

        /// <summary>
        /// If the users hovers over the balloon, we don't close it.
        /// </summary>
        private void grid_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!this.IsClosing)
            {
                if (_timer.Enabled)
                    _timer.Stop();
                this.RaiseEvent(new RoutedEventArgs(HoveredProperty));
            }
        }


        /// <summary>
        /// Closes the popup once the fade-out animation completed.
        /// The animation was triggered in XAML through the attached
        /// BalloonClosing event.
        /// </summary>
        private void OnFadeOutCompleted(object sender, EventArgs e)
        {
            //Popup pp = (Popup) this.Parent;
            //pp.IsOpen = false;
        }

        private void Grid_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.IsClosing)
                return;

            this.RaiseEvent(new RoutedEventArgs(ClickedProperty));
        }

        private void Grid_OnMouseLeave(object sender, MouseEventArgs e)
        {
            if (!_timer.Enabled)
                _timer.Start();
        }

        public bool IsClosing { get; set; }
    }
}