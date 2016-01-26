// *************************************************************
// P99 Auctions Community Team
// -----------
// Solution:     Project 1999 Auction Tracker
// File:         LoggingConstants.cs
// Email:        info@p99auctions.com
// Website:      www.p99auctions.com
// Last Update:  01/25/2016 10:02 PM
// *************************************************************

namespace P99Auctions.Client.Models
{
    class LoggingConstants
    {
        public const string P99FilePrefix = "eqlog_";
        public const string P99FileSuffix = "_project1999.txt";
        public const string P99FileFilterPattern = "eqlog_*_project1999.txt";
        public static readonly string P99FileFilterRegex = $"^{P99FilePrefix}[a-zA-z]+{P99FileSuffix}$";
    }
}