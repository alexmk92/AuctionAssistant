// *************************************************************
// P99 Auctions Community Team
// -----------
// Solution:     Project 1999 Auction Tracker
// File:         EQLogFolderWatcher.cs
// Email:        info@p99auctions.com
// Website:      www.p99auctions.com
// Last Update:  01/25/2016 10:02 PM
// *************************************************************

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Timers;
using P99Auctions.Client.Interfaces;
using P99Auctions.Client.Models;

namespace P99Auctions.Client.Watchers
{
    /// <summary>
    /// Delegate LogFileUpdatedHandler
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="fileName">Name of the file.</param>
    public delegate void LogFileUpdatedHandler(object sender, string fileName);

    /// <summary>
    /// A folder watcher for monitoring a log files
    /// </summary>
    public class EQLogFolderWatcher
    {
        private readonly IIgnorableCharacterProvider _ignorableCharacterProvider;
        private readonly int _maxAuctionLinesPerBatch;
        private readonly int _noActivitySeconds;
        private readonly int _secondsBetweenAuctionBatches;
        private ConcurrentDictionary<string, AuctionMonitor> _dicMonitors = new ConcurrentDictionary<string, AuctionMonitor>();
        private FileSystemWatcher _fsw;
        private System.Timers.Timer _timerFileChecker;
        private bool _watching;

        public event EventHandler<AuctionReadEventArgs> AuctionRead;
        public event EventHandler<AuctionLogFileEventArgs> AuctionMonitorStarted;
        public event EventHandler<AuctionLogFileEventArgs> AuctionMonitorEnded;

        /// <summary>
        ///     Initializes a new instance of the <see cref="EQLogFolderWatcher" /> class.
        /// </summary>
        /// <param name="logFolder">The log folder.</param>
        /// <param name="ignorableCharacterProvider">The ignorable character provider.</param>
        /// <param name="noActivitySeconds">
        ///     The number of seconds of no activity on a file before the logger notifies listeners
        ///     that its changed
        /// </param>
        /// <param name="secondsBetweenAuctionBatches">The seconds between auction batches.</param>
        /// <param name="maxAuctionLinesPerBatch">The maximum auction lines per batch.</param>
        public EQLogFolderWatcher(string logFolder, IIgnorableCharacterProvider ignorableCharacterProvider, int noActivitySeconds = 15, int secondsBetweenAuctionBatches = 30, int maxAuctionLinesPerBatch = 150)
        {
            if (logFolder == null) throw new ArgumentNullException(nameof(logFolder));
            if (ignorableCharacterProvider == null) throw new ArgumentNullException(nameof(ignorableCharacterProvider));

            _ignorableCharacterProvider = ignorableCharacterProvider;
            _noActivitySeconds = noActivitySeconds;
            _secondsBetweenAuctionBatches = secondsBetweenAuctionBatches;
            _maxAuctionLinesPerBatch = maxAuctionLinesPerBatch;
            this.LogFolder = logFolder;
        }

        /// <summary>
        /// Called when [auction queued].
        /// </summary>
        /// <param name="monitor">The monitor.</param>
        /// <param name="line">The line.</param>
        protected virtual void OnAuctionQueued(AuctionMonitor monitor, string line)
        {
            var evtArgs = new AuctionReadEventArgs(line)
            {
                CharacterName = monitor.CharacterName,
                FileName = monitor.File.Name
            };
            AuctionRead?.Invoke(this, evtArgs);
        }

        /// <summary>
        /// Called when [auction monitor started].
        /// </summary>
        /// <param name="monitor">The monitor.</param>
        protected virtual void OnAuctionMonitorStarted(AuctionMonitor monitor)
        {
            var evtArgs = new AuctionLogFileEventArgs
            {
                CharacterName = monitor.CharacterName,
                FileName = monitor.File.Name
            };
            AuctionMonitorStarted?.Invoke(this, evtArgs);
        }

        /// <summary>
        /// Called when [auction monitor ended].
        /// </summary>
        /// <param name="monitor">The monitor.</param>
        protected virtual void OnAuctionMonitorEnded(AuctionMonitor monitor)
        {
            var evtArgs = new AuctionLogFileEventArgs
            {
                CharacterName = monitor.CharacterName,
                FileName = monitor.File.Name
            };
            AuctionMonitorEnded?.Invoke(this, evtArgs);
        }

        /// <summary>
        ///     Finalizes an instance of the <see cref="EQLogFolderWatcher" /> class.
        /// </summary>
        ~EQLogFolderWatcher()
        {
            this.EndWatch();
        }

        /// <summary>
        ///     Begins the watch.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool BeginWatch()
        {
            var dir = new DirectoryInfo(this.LogFolder);
            if (!dir.Exists)
                return false;

            //eq will always right to a "logs" folder, if the currnet folder doesn't end in "logs" we might be bound 
            //to the eq directory itself, so check the log folder inside it
            if (dir.Name.ToLowerInvariant() != "logs")
            {
                var logDir = dir.GetDirectories("logs");
                if (logDir.Length != 1)
                    return false;
                dir = logDir[0];
            }

            this.EndWatch();
            _watching = true;
            _fsw = new FileSystemWatcher
            {
                Path = dir.FullName,
                Filter = LoggingConstants.P99FileFilterPattern
            };
            _fsw.Changed += this.watcher_FileChanged;
            _fsw.InternalBufferSize = 8192; //8kb buffer
            _fsw.IncludeSubdirectories = false;
            _fsw.EnableRaisingEvents = true;

            //setup the file timer
            if (_timerFileChecker != null && _timerFileChecker.Enabled)
                _timerFileChecker.Stop();

            _timerFileChecker = new System.Timers.Timer
            {
                AutoReset = true,
                Interval = _noActivitySeconds*1000
            };
            _timerFileChecker.Elapsed += this.TimerFileChecker_Elapsed;
            return true;
        }

        /// <summary>
        ///     Handles the Elapsed event of the TimerFileChecker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ElapsedEventArgs" /> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void TimerFileChecker_Elapsed(object sender, ElapsedEventArgs e)
        {
            //any file that hasnt been scanned in the period indicated should be flushed and the scanner dropped
            DateTime minScanDate = DateTime.Now.AddSeconds(-1*_noActivitySeconds);

            var lstRemove = new List<string>();

            foreach (var mon in _dicMonitors)
            {
                if (mon.Value.LastScanDate < minScanDate)
                    lstRemove.Add(mon.Key);
            }

            foreach (var monKey in lstRemove)
            {
                AuctionMonitor monOut;
                if (_dicMonitors.TryRemove(monKey, out monOut))
                    this.OnAuctionMonitorEnded(monOut);
            }

            if (_dicMonitors.Count == 0)
                _timerFileChecker.Stop();
        }

        /// <summary>
        ///     Watcher_s the file changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.IO.FileSystemEventArgs" /> instance containing the event data.</param>
        private void watcher_FileChanged(object sender, FileSystemEventArgs e)
        {
            if (_ignorableCharacterProvider.IsIgnored(AuctionMonitor.ExtractCharacterName(e.Name)))
                return;

            AuctionMonitor monitor;
            bool blnFound = false, blnNewlyAdded = false;
            blnFound = _dicMonitors.TryGetValue(e.FullPath, out monitor);
            if (!blnFound)
            {
                monitor = new AuctionMonitor(e.FullPath, _noActivitySeconds);
                blnFound = _dicMonitors.TryAdd(e.FullPath, monitor);
                blnNewlyAdded = true;
            }

            if (blnFound)
            {
                if (blnNewlyAdded)
                {
                    monitor.AuctionRead += this.Monitor_AuctionQueued;
                    this.OnAuctionMonitorStarted(monitor);
                }
                if (!_timerFileChecker.Enabled)
                    _timerFileChecker.Start();
                monitor.ScanNewContent();
            }
        }

        /// <summary>
        /// Fired when an auction is read
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="P99Auctions.Client.Watchers.AuctionReadEventArgs" /> instance containing the event data.</param>
        private void Monitor_AuctionQueued(object sender, AuctionReadEventArgs e)
        {
            var monitor = sender as AuctionMonitor;
            if (monitor != null)
            {
                //_clientLogger.WriteLine("[{1}] Transmitting {0} Lines", e.Batch.Lines.Count(), monitor.File.Name);
                this.OnAuctionQueued(monitor, e.Line);
            }
        }

        /// <summary>
        ///     Ends the watch on the log directory
        /// </summary>
        public void EndWatch()
        {
            //stop the file monitors
            if (_timerFileChecker != null && _timerFileChecker.Enabled)
                _timerFileChecker?.Stop();

            if (_fsw != null)
            {
                _fsw.EnableRaisingEvents = false;
                _fsw.Dispose();
                _fsw = null;
            }

            //end all monitors
            var lstRemove = new List<string>();
            foreach (var mon in _dicMonitors)
                lstRemove.Add(mon.Key);

            foreach (var monKey in lstRemove)
            {
                AuctionMonitor monOut;
                if (_dicMonitors.TryRemove(monKey, out monOut))
                    this.OnAuctionMonitorEnded(monOut);
            }

            _watching = false;
        }

        /// <summary>
        ///     Gets or sets the log folder.
        /// </summary>
        /// <value>The log folder.</value>
        public string LogFolder { get; private set; }


        /// <summary>
        ///     Gets a value indicating of files are being actively scanned
        /// </summary>
        /// <value>The watching f iles.</value>
        public bool ScanningEnabled
        {
            get { return _watching; }
        }
    }
}