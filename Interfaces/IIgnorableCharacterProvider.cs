// *************************************************************
// P99 Auctions Community Team
// -----------
// Solution:     Project 1999 Auction Tracker
// File:         IIgnorableCharacterProvider.cs
// Email:        info@p99auctions.com
// Website:      www.p99auctions.com
// Last Update:  01/25/2016 10:02 PM
// *************************************************************

namespace P99Auctions.Client.Interfaces
{
    /// <summary>
    /// Data provider for checking if a character is ignored
    /// </summary>
    public interface IIgnorableCharacterProvider
    {
        /// <summary>
        /// Determines whether the specified character name is ignored.
        /// </summary>
        /// <param name="characterName">Name of the character.</param>
        /// <returns><c>true</c> if the specified character name is ignored; otherwise, <c>false</c>.</returns>
        bool IsIgnored(string characterName);
    }
}