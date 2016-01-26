// *************************************************************
// P99 Auctions Community Team
// -----------
// Solution:     Project 1999 Auction Tracker
// File:         BalloonController.cs
// Email:        info@p99auctions.com
// Website:      www.p99auctions.com
// Last Update:  01/25/2016 10:01 PM
// *************************************************************

using System;
using System.Threading;
using System.Windows.Controls.Primitives;
using Hardcodet.Wpf.TaskbarNotification;
using P99Auctions.Client.Balloons;
using P99Auctions.Client.Interfaces;

namespace P99Auctions.Client.Controllers
{
    /// <summary>
    /// A helper class for handling the display of balloons into the WPF UI thread
    /// </summary>
    public class BalloonHelper : IBalloonController
    {
        private BalloonContainer _container = null;
        private TaskbarIcon _tbi;

        /// <summary>
        /// Initializes a new instance of the <see cref="BalloonHelper"/> class.
        /// </summary>
        /// <param name="tbi">The tbi.</param>
        public BalloonHelper(TaskbarIcon tbi)
        {
            _tbi = tbi;
        }

        /// <summary>
        /// Internal method for balloon creation
        /// </summary>
        /// <param name="msg">The MSG.</param>
        private void DisplayBalloon(IBalloonMessage msg)
        {
            _tbi.Dispatcher.Invoke(() =>
            {
                if (_container == null)
                {
                    _container = new BalloonContainer();
                    _tbi.ShowCustomBalloon(_container, PopupAnimation.None, null);
                    _tbi.CustomBalloon.Closed += this.CustomBalloon_Closed;
                }

                _container.AddBalloon(msg);
            });
        }

        /// <summary>
        /// Handles the Closed event of the CustomBalloon control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void CustomBalloon_Closed(object sender, EventArgs e)
        {
            _container = null;
        }

        /// <summary>
        /// Adds a baloon to the container so it can be displayed
        /// </summary>
        /// <param name="msg">The MSG.</param>
        /// <exception cref="System.InvalidOperationException">No TaskbarIcon owner set</exception>
        public void ShowBalloonMessage(IBalloonMessage msg)
        {
            if (_tbi == null)
                throw new InvalidOperationException("No TaskbarIcon owner set");

            var thread = new Thread(() => this.DisplayBalloon(msg));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
        }
    }
}