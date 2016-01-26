// *************************************************************
// P99 Auctions Community Team
// -----------
// Solution:     Project 1999 Auction Tracker
// File:         MessageMonitor.cs
// Email:        info@p99auctions.com
// Website:      www.p99auctions.com
// Last Update:  01/25/2016 10:02 PM
// *************************************************************

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using P99Auctions.Client.Interfaces;
using P99Auctions.Client.Web;

namespace P99Auctions.Client.Watchers
{
    /// <summary>
    /// Monitors and records incomming messages dispatching them to listeners for processing
    /// </summary>
    public class MessageMonitor : IMessageMonitor
    {
        private const string MessageDataServiceEndPoint = "/Auctions/Messages";

        private const int queueCheckInterval = 5;
        private readonly IDataDispatcher _dataDispatcher;

        private readonly int _dispatchDelay;

        private readonly Timer _timer;
        private ConcurrentQueue<string> _queue;
        private int timeSinceLastDispatch = 0;


        /// <summary>
        /// Initializes a new instance of the <see cref="MessageMonitor"/> class.
        /// </summary>
        /// <param name="dataDispatcher">The _data dispatcher.</param>
        /// <param name="dispatchDelay">The dispatch delay.</param>
        public MessageMonitor(IDataDispatcher dataDispatcher, int dispatchDelay)
        {
            _dataDispatcher = dataDispatcher;
            _dispatchDelay = dispatchDelay;

            _queue = new ConcurrentQueue<string>();

            _timer = new Timer
            {
                Interval = queueCheckInterval*1000,
                AutoReset = true
            };
            _timer.Elapsed += this.DispatchDelay_Elapsed;
        }

        /// <summary>
        ///     Finalizes an instance of the <see cref="AuctionQueue" /> class.
        /// </summary>
        ~MessageMonitor()
        {
            if (_timer != null && _timer.Enabled)
                _timer.Stop();
        }

        /// <summary>
        ///     Handles the Elapsed event of the DispatchDelay control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ElapsedEventArgs" /> instance containing the event data.</param>
        private void DispatchDelay_Elapsed(object sender, ElapsedEventArgs e)
        {
            timeSinceLastDispatch += queueCheckInterval;
            if (timeSinceLastDispatch >= _dispatchDelay)
            {
                timeSinceLastDispatch = 0;
                _timer.Stop();
                if (_dataDispatcher.IsEnabled)
                {
                    //look for new messages
                    Task.Run(() => this.PollForNewMessages());
                }
                _timer.Start();
            }
        }

        /// <summary>
        /// Polls for new messages.
        /// </summary>
        private async void PollForNewMessages()
        {
            var package = new MessageRetrievalPackage();
            //send teh data
            var result = await _dataDispatcher.ExecuteWebRequest<List<ClientTrackerMessage>>(MessageDataServiceEndPoint, package);

            //if the send failed, requeue the data
            if (result.Successful)
            {
                foreach (var message in result.Result)
                    this.OnMessageReceived(message);
            }
        }

        /// <summary>
        /// Called when [message received].
        /// </summary>
        /// <param name="message">The message.</param>
        protected virtual void OnMessageReceived(ClientTrackerMessage message)
        {
            MessageReceived?.Invoke(this, new MessageRecievedEventArgs(message));
        }


        public event EventHandler<MessageRecievedEventArgs> MessageReceived;


        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start()
        {
            if (_timer != null && !_timer.Enabled)
                _timer.Start();
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            if (_timer != null && _timer.Enabled)
                _timer.Stop();
        }
    }
}