// *************************************************************
// P99 Auctions Community Team
// -----------
// Solution:     Project 1999 Auction Tracker
// File:         AuctionQueue.cs
// Email:        info@p99auctions.com
// Website:      www.p99auctions.com
// Last Update:  01/25/2016 10:02 PM
// *************************************************************

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using P99Auctions.Client.Interfaces;
using P99Auctions.Client.Web;

namespace P99Auctions.Client.Watchers
{
    /// <summary>
    ///     A queueing mechanism for batching auction data from multiple sources
    /// </summary>
    public class AuctionQueue : IAuctionQueue
    {
        private const string AuctionDataServiceEndPoint = "/Auctions/Upload";

        private const int queueCheckInterval = 5;
        private readonly IDataDispatcher _auctionDispatcher;

        private readonly int _dispatchDelay;
        private readonly int _maxAuctionsPerBatch;

        private readonly Timer _timer;
        private ConcurrentQueue<string> _queue;
        private int timeSinceLastDispatch = 0;

        /// <summary>
        ///     Initializes a new instance of the <see cref="AuctionQueue" /> class.
        /// </summary>
        /// <param name="_auctionDispatcher">The _auction dispatcher.</param>
        /// <param name="dispatchDelay">The dispatch delay.</param>
        /// <param name="maxAuctionsPerBatch">The maximum auctions per batch.</param>
        public AuctionQueue(IDataDispatcher _auctionDispatcher, int dispatchDelay, int maxAuctionsPerBatch)
        {
            this._auctionDispatcher = _auctionDispatcher;
            this._dispatchDelay = dispatchDelay;
            this._maxAuctionsPerBatch = maxAuctionsPerBatch;

            _queue = new ConcurrentQueue<string>();

            _timer = new Timer
            {
                Interval = queueCheckInterval*1000,
                AutoReset = true
            };
            _timer.Elapsed += this.DispatchDelay_Elapsed;
            _timer.Start();
        }

        /// <summary>
        ///     Finalizes an instance of the <see cref="AuctionQueue" /> class.
        /// </summary>
        ~AuctionQueue()
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
            if (timeSinceLastDispatch >= _dispatchDelay || _queue.Count >= _maxAuctionsPerBatch)
            {
                timeSinceLastDispatch = 0;
                _timer.Stop();
                if (_auctionDispatcher.IsEnabled)
                {
                    var lstBatch = new List<string>();
                    while (_queue.Count > 0 && lstBatch.Count < _maxAuctionsPerBatch)
                    {
                        string str;
                        if (_queue.TryDequeue(out str))
                            lstBatch.Add(str);
                        else
                            break;
                    }

                    //only call out if any data is actually ready to go1wx
                    if (lstBatch.Count > 0)
                        Task.Run(() => this.DispatchAuctionData(lstBatch));
                }
                _timer.Start();
            }
        }

        /// <summary>
        /// Dispatches the auction data.
        /// </summary>
        /// <param name="auctionLines">The auction lines.</param>
        private async void DispatchAuctionData(List<string> auctionLines)
        {
            var package = new AuctionSubmissionPackage
            {
                AuctionLines = auctionLines
            };

            //send teh data
            var result = await _auctionDispatcher.ExecuteWebRequest<string>(AuctionDataServiceEndPoint, package);

            //if the send failed, requeue the data
            if (!result.Successful)
                auctionLines.ForEach(x => _queue.Enqueue(x));
        }

        /// <summary>
        /// Flushes this instance.
        /// </summary>
        public void Flush()
        {
            _queue = new ConcurrentQueue<string>();
        }

        /// <summary>
        ///     Enqueues an auction for batched dispatching
        /// </summary>
        /// <param name="auctionLine">The auction line.</param>
        public void EnqueueAuction(string auctionLine)
        {
            _queue.Enqueue(auctionLine);
        }
    }
}