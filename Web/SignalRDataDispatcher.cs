using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup.Localizer;
using log4net;
using Microsoft.AspNet.SignalR.Client;
using P99Auctions.Client.Interfaces;
using P99Auctions.Client.Watchers;
using static System.String;

namespace P99Auctions.Client.Web
{
    
    /// <summary>
    /// Data dispatcher for use with signalR
    /// </summary>
    public class SignalRDataDispatcher : IDataDispatcher
    {
        private const string DefaultUrlBase = "http://api.p99auctions.com/";
        private const string HEADER_APIKEY = "X-P99Auctions-ClientApiKey";
        private const string HEADER_VERSION = "X-P99Auctions-AssistantVersion";
        private const string HEADER_USER_TYPE = "X-P99Auctions-UserType";
        private const string USER_TYPE_CONTRIBUTOR = "contributor";

        public event EventHandler<DataDispatchEventArgs> StatusChanged;
        public event EventHandler<MessageRecievedEventArgs> MessageReceived;

        private readonly ILog _logger;
        private readonly string _serviceUrlBase;
        private readonly int _maxFailures;

        private HubConnection serverConnection;
        private IHubProxy _hub;
        private IDisposable _messageReceivedHandler;
        private int _failedAttempts;
        private string _apiKey;
        private bool manualDisconnect = false;

        private string clientKey = $"ANON-{Guid.NewGuid().ToString().Replace("-", "").Replace("{", "").Replace("}", "")}";


        /// <summary>
        /// Initializes a new instance of the <see cref="DataDispatcher" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="apiKey">The API key.</param>
        /// <param name="serviceUrlBase">The service URL base.</param>
        /// <param name="maxFailures">The maximum failures.</param>
        public SignalRDataDispatcher(ILog logger, string apiKey, string serviceUrlBase = "", int maxFailures = 3)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            _logger = logger;
            this.ApiKey = apiKey;
            _serviceUrlBase = serviceUrlBase;
            _maxFailures = maxFailures;
        }


        /// <summary>
        /// Called when [status changed].
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="message">The message.</param>
        protected virtual void OnStatusChanged(DispatcherStatus code, string message)
        {
            StatusChanged?.Invoke(this, new DataDispatchEventArgs
            {
                Code = code,
                Message = message
            });
        }


        /// <summary>
        /// Called when [message received].
        /// </summary>
        /// <param name="message">The message.</param>
        protected virtual void OnMessageReceived(ClientTrackerMessage message)
        {
            MessageReceived?.Invoke(this, new MessageRecievedEventArgs(message));
        }

        /// <summary>
        /// Sends the auction line to the server
        /// </summary>
        /// <param name="line">The line.</param>
        public void SendAuctionLine(string line)
        {
            try
            {
                _hub.Invoke<bool>("ReceiveAuctionLine", line)
                    .ContinueWith(this.AuctionLineSendOperationComplete);
            }
            catch (Exception e)
            {
                this.AuctionLineSendFailure(e);
            }
        }

        /// <summary>
        /// A call back fired when the auctionline has been processed by the server and result received
        /// </summary>
        /// <param name="task">The task.</param>
        private void AuctionLineSendOperationComplete(Task<bool> task)
        {
            if (task.Status != TaskStatus.RanToCompletion)
                this.AuctionLineSendFailure(new Exception("A line send operation failed to complete"));
            else if (task.Result != true)
                this.AuctionLineSendFailure(new Exception("The server rejected an auction line"));
            else
            {
                this.OnStatusChanged(DispatcherStatus.Complete, $"Operation Complete");
                _failedAttempts = 0;
            }
        }

        /// <summary>
        /// A call back when an auction line fails to send
        /// </summary>
        /// <param name="e">The e.</param>
        private void AuctionLineSendFailure(Exception e)
        {
            _logger.Error(e);
            _failedAttempts++;
            this.OnStatusChanged(DispatcherStatus.Error, $"Auction Line Failed to Send ({_failedAttempts} of {_maxFailures})");

            if (_failedAttempts >= _maxFailures)
                this.OnStatusChanged(DispatcherStatus.RetryFailure, $"Maximum failured send attempts reached ({_failedAttempts} of {_maxFailures}");
        }

        /// <summary>
        /// Makes a formatted connection URL or returns nothing.
        /// </summary>
        /// <returns>System.String.</returns>
        private string MakeFormattedConnectionUrlOrNothing()
        {
            var serviceUrl = _serviceUrlBase;
            if (IsNullOrWhiteSpace(serviceUrl))
                serviceUrl = DefaultUrlBase;

            Uri urlBase;
            return Uri.TryCreate(serviceUrl, UriKind.Absolute, out urlBase) ? urlBase.ToString() : string.Empty;
        }

        /// <summary>
        /// Determines the client version.
        /// </summary>
        /// <returns>System.String.</returns>
        private string DetermineClientVersion()
        {
            return System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString();
        }

        /// <summary>
        /// Starts this instance, connecting to the server and listening for incoming messages
        /// </summary>
        /// <returns>System.Boolean.</returns>
        public bool Start()
        {
            if (serverConnection != null)
                return true;

            manualDisconnect = false;
            //ensure a valid url
            var cnnString = this.MakeFormattedConnectionUrlOrNothing();
            if (string.IsNullOrWhiteSpace(cnnString))
            {
                _logger.Error($"Unable to connect to the P99 auction service. The url '{_serviceUrlBase}' is invalid.");
                return false;
            }

            //attempt to connect
            serverConnection = new HubConnection(cnnString);
            serverConnection.Closed += this.ServerConnection_Closed;
            _hub = serverConnection.CreateHubProxy("AuctionCenter");


            //identify the connected client to the server
            var keyToUse = this.ApiKey;
            if (string.IsNullOrWhiteSpace(keyToUse))
                keyToUse = clientKey;

            
            serverConnection.Headers.Add(HEADER_APIKEY, keyToUse);
            serverConnection.Headers.Add(HEADER_VERSION, this.DetermineClientVersion());
            serverConnection.Headers.Add(HEADER_USER_TYPE, USER_TYPE_CONTRIBUTOR);
            _hub.On<ClientTrackerMessage>("ProcessServerMessage", this.ProcessServerMessage);
            try
            {
                //****START THE CONNECTION******
                this.OnStatusChanged(DispatcherStatus.Connecting, $"Connecting to P99 Auctions");
                var completed = serverConnection.Start().Wait(30000);

                if (!completed)
                {
                    _logger.Error($"Client Connection Failed. Connection timed out or was forcibly closed before the connection was completed.");
                    this.OnStatusChanged(DispatcherStatus.Error, $"Auction Services failed to connect");
                }
                else
                {
                    //finalize
                    _logger.Info($"Client Connected. Real time communications enabled.");
                    this.OnStatusChanged(DispatcherStatus.Idle, $"Auction Services Connected");
                    this.IsEnabled = true;
                    _failedAttempts = 0;
                }
                return completed;
            }
            catch (AggregateException ae)
            {
                _logger.Error("An error occured when connecting to the P99 auction service");
                //from the aggregated errors find the httpRequestException (if any)
                //and record that as the problem
                foreach (var ex in ae.InnerExceptions)
                {
                    _logger.Error($"{ex.Message}:{ex.GetType().Name}");
                }
            }
            catch (Exception e)
            {
                _logger.Error($"{e.Message}:{e.GetType().Name}");
            }

            this.Stop();
            this.IsEnabled = false;
            return false;
        }




        /// <summary>
        /// Event fired when the connection is closed
        /// </summary>
        private void ServerConnection_Closed()
        {
            _hub = null;
            serverConnection = null;
            this.IsEnabled = false;
            _messageReceivedHandler?.Dispose();
            _messageReceivedHandler = null;
            if(manualDisconnect)
                this.OnStatusChanged(DispatcherStatus.Closed, "Your connection to the server has been closed.");
            else
                this.OnStatusChanged(DispatcherStatus.Disconnected, "You have been disconnected from the server.");
        }

        /// <summary>
        /// Stops this instance, closing the server connection
        /// </summary>
        public void Stop()
        {
            if (serverConnection != null && serverConnection.State == ConnectionState.Connected)
            {
                manualDisconnect = true;
                serverConnection.Stop();
            }
            else
            {
                _hub = null;
                serverConnection = null;
                this.IsEnabled = false;
                _messageReceivedHandler?.Dispose();
                _messageReceivedHandler = null;
            }
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        /// <returns>System.Boolean.</returns>
        public bool Reset()
        {
            this.Stop();
            return this.Start();
        }

        public bool WatchingForMessages { get { return this.IsEnabled && !string.IsNullOrWhiteSpace(this.ApiKey); } }

        /// <summary>
        /// Ensures the dispatcher has been started
        /// </summary>
        public bool EnsureStart(string apiKey)
        {
            if (apiKey != this.ApiKey)
            {
                this.ApiKey = apiKey;
                return this.Reset();
            }

            return this.IsEnabled || this.Reset();
        }


        /// <summary>
        /// Processes the message received from the server
        /// </summary>
        /// <param name="message">The message.</param>
        private void ProcessServerMessage(ClientTrackerMessage message)
        {
            if (message != null)
                this.OnMessageReceived(message);
        }

        /// <summary>
        /// Gets or sets the account key.
        /// </summary>
        /// <value>The account key.</value>
        public string ApiKey
        {
            get { return _apiKey; }
            set
            {
                _apiKey = value;
                if (this.IsEnabled && serverConnection != null)
                {
                    serverConnection.Headers[HEADER_APIKEY] = value;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating if this instance is enabled and communicating with the server
        /// </summary>
        /// <value>The is enabled.</value>
        public bool IsEnabled { get; private set; }

        
    }
}
