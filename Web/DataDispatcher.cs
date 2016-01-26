// *************************************************************
// P99 Auctions Community Team
// -----------
// Solution:     Project 1999 Auction Tracker
// File:         DataDispatcher.cs
// Email:        info@p99auctions.com
// Website:      www.p99auctions.com
// Last Update:  01/25/2016 10:02 PM
// *************************************************************

using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using log4net;
using Newtonsoft.Json;
using P99Auctions.Client.Interfaces;

namespace P99Auctions.Client.Web
{
    public delegate void AuctionDispatchEventHandler(object o, AuctionDispatchEventArgs e);

    /// <summary>
    /// The dispatcher that communicates with the server
    /// </summary>
    public class DataDispatcher : IDataDispatcher
    {
        private const string DefaultUrlBase = "http://api.p99auctions.com/";
        private readonly int _dispatchRetryCount;
        private readonly ILog _logger;
        private readonly string _serviceUrlBase;
        private string _apiKey;
        private int _failedAttempts;
        private string clientKey = $"ANON-{Guid.NewGuid().ToString().Replace("-", "").Replace("{", "").Replace("}", "")}";

        /// <summary>
        ///     Initializes a new instance of the <see cref="DataDispatcher" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="dispatchRetryCount">The dispatch retry count.</param>
        /// <param name="apiKey">The API key.</param>
        /// <param name="serviceUrlBase">The service URL base.</param>
        public DataDispatcher(ILog logger, int dispatchRetryCount, string apiKey, string serviceUrlBase)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            _logger = logger;
            _dispatchRetryCount = dispatchRetryCount;
            _apiKey = apiKey;
            _serviceUrlBase = serviceUrlBase;
            this.IsEnabled = true;
        }


        /// <summary>
        ///     Called when [status changed].
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="percentComplete">The percent complete.</param>
        /// <param name="message">The message.</param>
        protected virtual void OnStatusChanged(DispatcherStatus code, int percentComplete, string message)
        {
            StatusChanged?.Invoke(this, new AuctionDispatchEventArgs
            {
                Code = code,
                PercentComplete = percentComplete,
                Message = message
            });
        }

        /// <summary>
        ///     Makes the qualified URL or nothing.
        /// </summary>
        /// <param name="serviceBase">The service base.</param>
        /// <param name="endPoint">The end point.</param>
        /// <returns>System.String.</returns>
        private string MakeQualifiedUrlOrNothing(string serviceBase, string endPoint)
        {
            if (String.IsNullOrWhiteSpace(serviceBase))
                serviceBase = DefaultUrlBase;

            Uri urlBase;
            if (Uri.TryCreate(serviceBase, UriKind.Absolute, out urlBase))
            {
                Uri completeUri;
                if (Uri.TryCreate(urlBase, endPoint, out completeUri))
                    return completeUri.ToString();
            }
            return string.Empty;
        }

        public void Reset()
        {
            _failedAttempts = 0;
            this.IsEnabled = true;
        }

        public void Stop()
        {
            this.IsEnabled = false;
        }

        public event AuctionDispatchEventHandler StatusChanged;

        public async Task<DataDispatchResult<T>> ExecuteWebRequest<T>(string endPoint, ISubmissionPackage submissionPackage)
            where T : class
        {
            if (!this.IsEnabled)
                return new DataDispatchResult<T> {Successful = false};

            this.OnStatusChanged(DispatcherStatus.Connecting, 0, "Connecting...");
            var url = this.MakeQualifiedUrlOrNothing(_serviceUrlBase, endPoint);
            if (string.IsNullOrWhiteSpace(url))
            {
                this.OnStatusChanged(DispatcherStatus.InvalidServiceAddress, 0, "Invalid Service Address");
                this.IsEnabled = false;
                return new DataDispatchResult<T> {Successful = false};
            }

            if (string.IsNullOrWhiteSpace(_apiKey))
                submissionPackage.StreamId = clientKey;
            else
                submissionPackage.StreamId = _apiKey;

            submissionPackage.ApplicationVersion = System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString();

            //pacakage the data
            var preppedPackage = JsonConvert.SerializeObject(submissionPackage);
            var httpContent = new StringContent(preppedPackage, Encoding.UTF8, "application/json");

            var dd = new DataDispatchResult<T> {Successful = false};
            using (var httpClient = new HttpClient())
            {
                // send the actual request
                this.OnStatusChanged(DispatcherStatus.Transmitting, 40, "Sending Data...");

                try
                {
                    //wait for a response
                    var httpResponse = await httpClient.PostAsync(url, httpContent);
                    this.OnStatusChanged(DispatcherStatus.Transmitting, 70, "Receiving Response...");

                    //parse the result
                    switch (httpResponse.StatusCode)
                    {
                        case HttpStatusCode.OK:
                            var result = await httpResponse.Content.ReadAsStringAsync();
                            dd.Successful = true;
                            dd.Result = JsonConvert.DeserializeObject<T>(result);
                            _failedAttempts = 0;
                            this.OnStatusChanged(DispatcherStatus.Complete, 100, "Operation Complete...");
                            break;
                        case HttpStatusCode.NotFound:
                            this.OnStatusChanged(DispatcherStatus.Error, 100, "Service Not Found...");
                            _failedAttempts++;
                            _logger.Error($"Auction Dispatch Failure: Service Not Found {url}");
                            break;
                        default:
                            this.OnStatusChanged(DispatcherStatus.Failure, 100, "An Unknown Error Occured...");
                            _logger.Error($"Dispatch Failure: Unknown Error");
                            _failedAttempts++;
                            break;
                    }
                }
                catch (Exception e)
                {
                    _failedAttempts++;
                    _logger.Error($" Dispatch Critical Failure", e);
                    this.OnStatusChanged(DispatcherStatus.Failure, 0, "Auction Dispatcher Failed");
                }

                //if we've reached our threshold for failed attempt fail out and stop trying
                if (!dd.Successful && _failedAttempts >= _dispatchRetryCount)
                {
                    this.IsEnabled = false;
                    this.OnStatusChanged(DispatcherStatus.RetryFailure, 0, $"Maximum number of dispatch failures reached ({_dispatchRetryCount}");
                }
            }

            return dd;
        }

        /// <summary>
        ///     Gets the is enabled.
        /// </summary>
        /// <value>The is enabled.</value>
        public bool IsEnabled { get; private set; }

        /// <summary>
        /// Updates the client apikey.
        /// </summary>
        /// <param name="apiKey">The API key.</param>
        public void UpdateClientApikey(string apiKey)
        {
            _apiKey = apiKey;
        }
    }
}