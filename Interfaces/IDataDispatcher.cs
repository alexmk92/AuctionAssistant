// *************************************************************
// P99 Auctions Community Team
// -----------
// Solution:     Project 1999 Auction Tracker
// File:         IDataDispatcher.cs
// Email:        info@p99auctions.com
// Website:      www.p99auctions.com
// Last Update:  01/25/2016 10:02 PM
// *************************************************************

using System.Threading.Tasks;
using P99Auctions.Client.Web;

namespace P99Auctions.Client.Interfaces
{
    /// <summary>
    ///     A dispatcher takes a batch of auction data and dispatches it to the underlying carrier
    /// </summary>
    public interface IDataDispatcher
    {
        /// <summary>
        ///     Occurs when dispatcher status changes states
        /// </summary>
        event AuctionDispatchEventHandler StatusChanged;

        /// <summary>
        /// A non-blocking call to dispatche the auction data.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <param name="submissionPackage">The submission package.</param>
        /// <returns>Task&lt;DataDispatchResult&gt;.</returns>
        Task<DataDispatchResult<T>> ExecuteWebRequest<T>(string endPoint, ISubmissionPackage submissionPackage) where T : class;

        /// <summary>
        /// Resets the dispatcher
        /// </summary>
        void Reset();

        /// <summary>
        /// Stops the dispatcher from communicating
        /// </summary>
        void Stop();

        /// <summary>
        /// Updates the client apikey this dispatcher will append to all outgoing requests
        /// </summary>
        /// <param name="apiKey">The API key.</param>
        void UpdateClientApikey(string apiKey);


        /// <summary>
        ///     Gets a value indicating whether this instance is enabled and can dispatch auction data
        /// </summary>
        /// <value><c>true</c> if this instance is enabled; otherwise, <c>false</c>.</value>
        bool IsEnabled { get; }
    }
}