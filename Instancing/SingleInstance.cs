// *************************************************************
// P99 Auctions Community Team
// -----------
// Solution:     Project 1999 Auction Tracker
// File:         SingleInstance.cs
// Email:        info@p99auctions.com
// Website:      www.p99auctions.com
// Last Update:  01/25/2016 10:01 PM
// *************************************************************

using System.Reflection;
using System.Threading;

namespace P99Auctions.Client.Instancing
{
    /// <summary>
    /// Unique program info to identify the application in windows (used in a global mutex)
    /// </summary>
    public static class ProgramInfo
    {
        public static string AssemblyGuid
        {
            get
            {
                object[] attributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof (System.Runtime.InteropServices.GuidAttribute), false);
                if (attributes.Length == 0)
                    return "{62852D3B-9713-478A-A876-AD1C7284F441}";
                return ((System.Runtime.InteropServices.GuidAttribute) attributes[0]).Value;
            }
        }
    }

    /// <summary>
    /// Static singleton for regulating multiple tracker copies being loaded
    /// </summary>
    public static class SingleInstance
    {
        static Mutex mutex;

        /// <summary>
        /// Starts this instance.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool Start()
        {
            bool onlyInstance = false;
            string mutexName = $"Local\\{ProgramInfo.AssemblyGuid}";

            // if you want your app to be limited to a single instance
            // across ALL SESSIONS (multiple users & terminal services), then use the following line instead:
            // string mutexName = String.Format("Global\\{0}", ProgramInfo.AssemblyGuid);

            mutex = new Mutex(true, mutexName, out onlyInstance);
            return onlyInstance;
        }

        public static void Stop()
        {
            mutex.ReleaseMutex();
        }
    }
}