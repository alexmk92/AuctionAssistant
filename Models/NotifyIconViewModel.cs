// *************************************************************
// P99 Auctions Community Team
// -----------
// Solution:     Project 1999 Auction Tracker
// File:         NotifyIconViewModel.cs
// Email:        info@p99auctions.com
// Website:      www.p99auctions.com
// Last Update:  01/25/2016 10:02 PM
// *************************************************************

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Timers;
using System.Windows.Input;
using P99Auctions.Client.Interfaces;

namespace P99Auctions.Client.Models
{
    /// <summary>
    ///     Provides bindable properties and commands for the NotifyIcon. In this sample, the
    ///     view model is assigned to the NotifyIcon in XAML. Alternatively, the startup routing
    ///     in App.xaml.cs could have created this view model, and assigned it to the NotifyIcon.
    /// </summary>
    public class NotifyIconViewModel : INotifyIconView, INotifyPropertyChanged
    {
        private string _iconImage;
        private string _iconStatusMessage;
        private Timer timer;


        public event EventHandler ShowAuctionTracker;
        public event EventHandler ExitApplication;

        public NotifyIconViewModel()
        {
            this.UpdateActivityMonitor(ActivityState.Idle, "P99 Auction House Assistant");
        }

        /// <summary>
        /// Updates the activity monitor.
        /// </summary>
        /// <param name="newState">The new state.</param>
        /// <param name="newMessage">The new message.</param>
        /// <param name="fallBackMessage">The fall back message.</param>
        /// <param name="resetToIdleAfterXMilliseconds">The monitor state is automatically reverted to idle after X milliseconds (0
        /// = never revert).</param>
        public void UpdateActivityMonitor(ActivityState newState, string newMessage, string fallBackMessage = "", double resetToIdleAfterXMilliseconds = 0)
        {
            var strImage = "";
            switch (newState)
            {
                case ActivityState.AuctionFound:
                    strImage = "/Images/auctionPlus.ico";
                    break;
                case ActivityState.Error:
                    strImage = "/Images/auctionAlert.ico";

                    break;
                case ActivityState.Transmitting:
                    strImage = "/Images/auctionArrow.ico";
                    break;
                case ActivityState.Disabled:
                    strImage = "/Images/auctionDisabled.ico";
                    break;
                default:
                    strImage = "/Images/auction.ico";
                    break;
            }

            this.IconStatusMessage = $"P99 Auction House - {newMessage}";
            if (!string.IsNullOrWhiteSpace(strImage))
            {
                this.IconImage = strImage;

                if (resetToIdleAfterXMilliseconds > 0)
                {
                    if (string.IsNullOrWhiteSpace(fallBackMessage))
                        fallBackMessage = "P99 Auction House";
                    else
                        fallBackMessage = $"P99 Auction House - {fallBackMessage}";

                    if (timer == null || timer.Enabled == false)
                    {
                        timer = new Timer();
                        timer.Elapsed += (sender, args) =>
                        {
                            this.UpdateActivityMonitor(ActivityState.Idle, fallBackMessage);
                            timer.Stop();
                        };
                        timer.AutoReset = false;
                        timer.Interval = resetToIdleAfterXMilliseconds;
                        timer.Start();
                    }
                }
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public NotifyIconViewModel GetModel()
        {
            return this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Shows a window, if none is already open.
        /// </summary>
        public ICommand ShowTrackerWindow
        {
            get
            {
                return new DelegateCommand
                {
                    CanExecuteFunc = () => true,
                    CommandAction = () => { ShowAuctionTracker?.Invoke(this, new EventArgs()); }
                };
            }
        }


        public ICommand ExitApplicationCommand
        {
            get
            {
                return new DelegateCommand
                {
                    CanExecuteFunc = () => true,
                    CommandAction = () => { ExitApplication?.Invoke(this, new EventArgs()); }
                };
            }
        }

        public string IconStatusMessage
        {
            get { return _iconStatusMessage; }
            set
            {
                _iconStatusMessage = value;
                this.OnPropertyChanged();
            }
        }

        public string IconImage
        {
            get { return _iconImage; }
            set
            {
                _iconImage = value;
                this.OnPropertyChanged();
            }
        }
    }
}