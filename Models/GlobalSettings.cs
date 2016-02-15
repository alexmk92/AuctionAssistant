// *************************************************************
// P99 Auctions Community Team
// -----------
// Solution:     Project 1999 Auction Tracker
// File:         GlobalSettings.cs
// Email:        info@p99auctions.com
// Website:      www.p99auctions.com
// Last Update:  01/25/2016 10:02 PM
// *************************************************************

using System.Configuration;
using System.IO;
using P99Auctions.Client.Interfaces;

namespace P99Auctions.Client.Models
{
    /// <summary>
    ///     Class GlobalSettings.
    /// </summary>
    public class GlobalSettings : IGlobalSettings
    {
        /// <summary>
        ///     Gets the service URL.
        /// </summary>
        /// <value>The service URL.</value>
        public string ServiceUrlBase
        {
            get
            {
                try
                {
                    var str = ConfigurationManager.AppSettings["ServiceUrlBase"];
                    if (!string.IsNullOrWhiteSpace(str))
                        return str.Trim();
                }
                catch
                {
                }

                return null;
            }
        }

        /// <summary>
        ///     Gets the transmit delay seconds.
        /// </summary>
        /// <value>The transmit delay seconds.</value>
        public int DispatchDelay
        {
            get
            {
                var str = "";
                try
                {
                    str = ConfigurationManager.AppSettings["DispatchDelay"];
                }
                catch
                {
                }

                int intDelay;
                if (!int.TryParse(str, out intDelay))
                    intDelay = 60;
                if (intDelay > 600)
                    intDelay = 600;
                if (intDelay < 15)
                    intDelay = 15;
                return intDelay;
            }
        }

        /// <summary>
        ///     Gets the m ax auctions per batch.
        /// </summary>
        /// <value>The m ax auctions per batch.</value>
        public int MaxAuctionsPerBatch
        {
            get
            {
                var str = "";
                try
                {
                    str = ConfigurationManager.AppSettings["MaxAuctionsPerBatch"];
                }
                catch
                {
                }

                int intValue;
                if (!int.TryParse(str, out intValue))
                    intValue = 50;
                if (intValue > 100)
                    intValue = 100;
                if (intValue < 15)
                    intValue = 15;
                return intValue;
            }
        }

        /// <summary>
        ///     Gets the name of the log file.
        /// </summary>
        /// <value>The name of the log file.</value>
        public string LogFileName
        {
            get
            {
                var str = string.Empty;
                try
                {
                    str = ConfigurationManager.AppSettings["LogFileName"];
                }
                catch
                {
                }


                if (string.IsNullOrWhiteSpace(str))
                    str = "p99AuctionsLog.txt";

                str = str.Trim();
                if (str.Contains("/") || str.Contains("\\"))
                    str = "p99AuctionsLog.txt";

                if (str.IndexOfAny(Path.GetInvalidFileNameChars()) > 0)
                    str = "p99AuctionsLog.txt";

                if (!str.EndsWith(".txt"))
                    str += ".txt";

                return str;
            }
        }

        /// <summary>
        ///     The number of seconds before a log monitor will shut down if no changes are detected on the log
        /// </summary>
        /// <value>The inactivity timeout.</value>
        public int InactivityTimeout
        {
            get
            {
                var str = "";
                try
                {
                    str = ConfigurationManager.AppSettings["InactivityTimeout"];
                }
                catch
                {
                }

                int intTimeout;
                if (!int.TryParse(str, out intTimeout))
                    intTimeout = 60;
                if (intTimeout > 120)
                    intTimeout = 120;
                if (intTimeout < 15)
                    intTimeout = 15;
                return intTimeout;
            }
        }


        /// <summary>
        ///     The number of seconds between toast server checks when in a long polling mode
        /// </summary>
        /// <value>The toast check interval.</value>
        public int ToastCheckInterval
        {
            get
            {
                var str = "";
                try
                {
                    str = ConfigurationManager.AppSettings["ToastCheckInterval"];
                }
                catch
                {
                }

                int intInterval;
                if (!int.TryParse(str, out intInterval))
                    intInterval = 30;
                if (intInterval > 120)
                    intInterval = 120;
                if (intInterval < 30)
                    intInterval = 5;
                return intInterval;
            }
        }

        /// <summary>
        ///     The number of attempts to retry sending auction data before a complete failure is recorded
        /// </summary>
        /// <value>The transmit retry count.</value>
        public int DispatchRetryCount
        {
            get
            {
                var str = "";
                try
                {
                    str = ConfigurationManager.AppSettings["TransmitRetryCount"];
                }
                catch
                {
                }

                int intInterval;
                if (!int.TryParse(str, out intInterval))
                    intInterval = 10;
                if (intInterval > 20)
                    intInterval = 20;
                if (intInterval < 3)
                    intInterval = 3;
                return intInterval;
            }
        }


        /// <summary>
        /// Gets the disabled communications interval. The length of time, in minutes, to disable all communications 
        /// after repeated connection failures before trying again.
        /// </summary>
        /// <value>The disabled communications interval.</value>
        public int DisabledCommunicationsInterval
        {
            get
            {
                var str = "";
                try
                {
                    str = ConfigurationManager.AppSettings["DisabledCommunicationsInterval"];
                }
                catch
                {
                }

                int intInterval;
                if (!int.TryParse(str, out intInterval))
                    intInterval = 5;
                if (intInterval > 60)
                    intInterval = 60;
                if (intInterval < 1)
                    intInterval = 1;
                return intInterval;
            }
        }

        /// <summary>
        /// Gets a value indicating whether to use a presistant connection or, if false, periodic polling
        /// </summary>
        /// <value><c>true</c> if [use presistant connection]; otherwise, <c>false</c>.</value>
        public bool UsePersistantConnection
        {
            get
            {
                var str = "";
                try
                {
                    str = ConfigurationManager.AppSettings["UsePersistantConnection"];
                }
                catch
                {
                }

                bool blnCnn;
                if (string.IsNullOrWhiteSpace(str) || !bool.TryParse(str, out blnCnn))
                    blnCnn = true;
                
                return blnCnn;
            }
        }
    }
}