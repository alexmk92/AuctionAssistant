// *************************************************************
// P99 Auctions Community Team
// -----------
// Solution:     Project 1999 Auction Tracker
// File:         DataDispatchResult.cs
// Email:        info@p99auctions.com
// Website:      www.p99auctions.com
// Last Update:  01/25/2016 10:02 PM
// *************************************************************

namespace P99Auctions.Client.Web
{
    /// <summary>
    /// A result of a data dispatch
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DataDispatchResult<T> where T : class
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="DataDispatchResult{T}"/> is successful.
        /// </summary>
        /// <value><c>true</c> if successful; otherwise, <c>false</c>.</value>
        public bool Successful { get; set; }

        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        /// <value>The result.</value>
        public T Result { get; set; }
    }
}