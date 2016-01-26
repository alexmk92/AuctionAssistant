// *************************************************************
// P99 Auctions Community Team
// -----------
// Solution:     Project 1999 Auction Tracker
// File:         ItemDequeuedEventHandler.cs
// Email:        info@p99auctions.com
// Website:      www.p99auctions.com
// Last Update:  01/25/2016 10:02 PM
// *************************************************************

namespace P99Auctions.Client.Utilities
{
    /// <summary>
    /// Delegate ItemDequeuedEventHandler
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="o">The o.</param>
    /// <param name="eventArgs">The event arguments.</param>
    public delegate void ItemDequeuedEventHandler<T>(object o, AutoQueueEventArgs<T> eventArgs) where T : IAutoQueueItem;
}