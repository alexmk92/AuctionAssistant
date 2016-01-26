// *************************************************************
// P99 Auctions Community Team
// -----------
// Solution:     Project 1999 Auction Tracker
// File:         ClientController.cs
// Email:        info@p99auctions.com
// Website:      www.p99auctions.com
// Last Update:  01/25/2016 10:01 PM
// *************************************************************

using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Timers;
using log4net;
using P99Auctions.Client.Balloons;
using P99Auctions.Client.Helpers;
using P99Auctions.Client.Interfaces;
using P99Auctions.Client.Models;
using P99Auctions.Client.Utilities;
using P99Auctions.Client.Watchers;
using P99Auctions.Client.Web;

namespace P99Auctions.Client.Controllers
{
    /// <summary>
    /// Primary controller for handling the the processing of the client data
    /// </summary>
    public class ClientController
    {
        /// <summary>
        /// check flag for starting minimized in the tray
        /// </summary>
        public const string StartMinimizedArgumentFlag = "silent";

        /// <summary>
        /// Generic catch all error message
        /// </summary>
        private const string MessageCheckLog = "Error: Check Log for details";

        /// <summary>
        /// Status message when scanning
        /// </summary>
        private const string MessageScanningForAuctions = "Scanning for new Auctions";

        /// <summary>
        /// Status message when idle
        /// </summary>
        private const string MessageNoCharactersLoggedIn = "-Ready-";

        /// <summary>
        /// Default time to hold the client state before processing the next queued message
        /// </summary>
        private const int holdClientStateForXMilliseconds = 2000;


        /// <summary>
        /// A reference to global client settings 
        /// </summary>
        private readonly IGlobalSettings _globalSettings;

        /// <summary>
        /// A reference to the logger used by the controller
        /// </summary>
        private readonly ILog _logger;

        /// <summary>
        /// View model consumed by the notification icon
        /// </summary>
        private readonly NotifyIconViewModel _notificationModel;

        /// <summary>
        /// Primary view model 
        /// </summary>
        private readonly MainWindowViewModel _trackerWindowModel;

        /// <summary>
        /// IoC of view creator
        /// </summary>
        private readonly IViewResolver _viewResolver;

        /// <summary>
        /// A global list of characters being watched by the various monitors
        /// </summary>
        private readonly ConcurrentDictionary<string, ActiveCharacter> dicCharactersBeingWatched = new ConcurrentDictionary<string, ActiveCharacter>();

        /// <summary>
        /// A refernece to teh auctions stored before being shipped to teh server
        /// </summary>
        private IAuctionQueue _auctionQueue;

        /// <summary>
        /// the balloon sub controller
        /// </summary>
        private IBalloonController _balloonController;

        /// <summary>
        /// a flag indicating the applicaiton is shutting down
        /// </summary>
        private bool _blnShuttingDown;

        /// <summary>
        /// The general client controlable settings (File -> Settings)
        /// </summary>
        private IClientSettings _clientSettings;


        /// <summary>
        /// An interface to the dispatching object that communicates formatted messages to teh server
        /// </summary>
        private IDataDispatcher _dataDispatcher;

        /// <summary>
        /// A timer for counting how long to disable the client after a series of failed attempts
        /// </summary>
        private Timer _disableTimer;

        /// <summary>
        /// The priamry WPF view when the app is not minimized
        /// </summary>
        private IMainView _mainView;

        /// <summary>
        /// An interface to the component that watches for new messages from the server
        /// </summary>
        private IMessageMonitor _messageMonitor;

        /// <summary>
        /// The queued list messages that need to be displayed on the UI
        /// </summary>
        private UiMessageQueue _uiMessageQueue;

        /// <summary>
        /// The phsyical file monitor watching for log changes
        /// </summary>
        private EQLogFolderWatcher _watcher;

        /// <summary>
        /// Occurs when the controller initiates a full shut down of the application
        /// </summary>
        public event EventHandler ApplicationShutDownRequested;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientController" /> class.
        /// </summary>
        /// <param name="clientSettings">The client settings.</param>
        /// <param name="globalSettings">The global settings.</param>
        /// <param name="balloonController">The balloon controller.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="viewResolver">The view resolver.</param>
        /// <exception cref="System.ArgumentNullException">
        /// </exception>
        public ClientController(IClientSettings clientSettings, IGlobalSettings globalSettings, IBalloonController balloonController, ILog logger, IViewResolver viewResolver)
        {
            if (clientSettings == null) throw new ArgumentNullException(nameof(clientSettings));
            if (globalSettings == null) throw new ArgumentNullException(nameof(globalSettings));
            if (balloonController == null) throw new ArgumentNullException(nameof(balloonController));
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            if (viewResolver == null) throw new ArgumentNullException(nameof(viewResolver));

            _balloonController = balloonController;
            _clientSettings = clientSettings;
            _globalSettings = globalSettings;
            _viewResolver = viewResolver;
            _logger = logger;

            //setup the view model for the notification window (sys tray)
            _notificationModel = new NotifyIconViewModel();
            _notificationModel.ExitApplication += this.MainWindow_CloseExplicit;
            _notificationModel.ShowAuctionTracker += this.NotificationView_ShowAuctionTracker;

            //data Dispatcher
            _dataDispatcher = new DataDispatcher(_logger, _globalSettings.DispatchRetryCount, _clientSettings.ApiKey, _globalSettings.ServiceUrlBase);
            _messageMonitor = new MessageMonitor(_dataDispatcher, _globalSettings.ToastCheckInterval);
            _messageMonitor.MessageReceived += this.MessageMonitor_MessageReceived;
            //Primary window view model
            _trackerWindowModel = new MainWindowViewModel();

            //status message switcher
            _uiMessageQueue = new UiMessageQueue(3000);
            _uiMessageQueue.ActiveItemChanged += this.UiMessageQueue_ActiveItemChanged;
        }

        /// <summary>
        /// Starts the application monitors and timers
        /// </summary>
        /// <param name="silentStartup">if set to <c>true</c> [silent startup].</param>
        public void StartApplication(bool silentStartup)
        {
            //validate the settings from disk
            var clientSettingValid = this.ValidateClientSettings();
            if (!clientSettingValid)
            {
                //allow the user to correct them
                this.ShowApplicationWindow();
                var result = this.UpdateSettings();
                if (!result)
                {
                    //not corrected / canceled (shut down the app)
                    _logger.Warn("Invalid Client Settings");
                    _logger.Warn($"EQ Folder: {_clientSettings.EQFolder}");
                    this.ForceApplicationShutdown();
                    return;
                }
            }

            //hook the dispatcher for view updates
            _dataDispatcher.StatusChanged += this.DataDispatcher_StatusChanged;

            _logger.Warn($"EQ Folder: {_clientSettings.EQFolder}");
            this.RestartAuctionWatch();

            _trackerWindowModel.UpdateTransmissionStatus(null);
            _uiMessageQueue.QueueItem(MessageNoCharactersLoggedIn, MessageSeverity.Informational, ActivityState.Idle);
            _trackerWindowModel.UpdateAuctionCounts(_clientSettings.TodayAuctionCount, _clientSettings.LifetimeAuctionCount);

            //if the settings weren't valid and thus need to be fixed, show the app window 
            //OR if they dont want it minimized 
            //OR if ti was explicitly launched by the user (not via a windows startup process)
            if (!clientSettingValid || !_clientSettings.MinimizeToTray || !silentStartup)
                this.ShowApplicationWindow();
        }

        /// <summary>
        /// public method for requsting the close of the application
        /// </summary>
        public void CloseApplication()
        {
            this.DisableApplication("Application shutting down", 0);
        }

        /// <summary>
        /// Shows the application window and wires the event hooks
        /// </summary>
        private void ShowApplicationWindow()
        {
            if (_mainView == null)
            {
                _mainView = _viewResolver.CreateMainView();
                _mainView.DataContext = _trackerWindowModel;
                _mainView.CloseExplicit += this.MainWindow_CloseExplicit;
                _mainView.CloseImplied += this.MainWindow_CloseImplied;
                _mainView.EditSettings += this.MainWindow_EditSettings;
                _mainView.ViewLog += this.MainWindow_ViewLog;
                _mainView.Help += this.MainWindow_Help;
                _mainView.Show();
            }
        }


        /// <summary>
        /// Closes the application window
        /// </summary>
        private void CloseApplicationWindow()
        {
            //minimize to system tray 
            if (_mainView != null)
            {
                try
                {
                    _mainView.Close();
                }
                catch (Exception e)
                {
                    _logger.Error(e);
                }
                _mainView = null;
            }
        }

        /// <summary>
        /// Forces the application shutdown, closing it entirely (as opposed to minimzing to system tray)
        /// </summary>
        private void ForceApplicationShutdown()
        {
            this.CloseApplicationWindow();
            ApplicationShutDownRequested?.Invoke(this, new EventArgs());
        }


        /// <summary>
        /// Integrates the applicaiton with windows startup registry entries (or removes a previous registration)
        /// </summary>
        /// <param name="startWithWindows">The start with windows.</param>
        private void IntegrateWithWindowsStartup(bool startWithWindows)
        {
            _logger.Info($"Setting Windows Startup Mode: {startWithWindows}");
            var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (key == null)
                return;

            Assembly startingExecutable = Assembly.GetExecutingAssembly();
            var name = startingExecutable.GetName().Name;

            if (startWithWindows)
            {
                try
                {
                    key.SetValue(name, $"{startingExecutable.Location} {StartMinimizedArgumentFlag}");
                }
                catch (Exception e)
                {
                    _logger.Error(e);
                }
            }
            else
            {
                try
                {
                    if (key.GetValueNames().Contains(name))
                        key.DeleteValue(name);
                }
                catch (Exception e)
                {
                    _logger.Error(e);
                }
            }
        }


        /// <summary>
        ///     Validates the client settings are correct for proper program execution
        /// </summary>
        /// <returns>System.Boolean.</returns>
        private bool ValidateClientSettings()
        {
            return _clientSettings.RetreiveValidationErrors().Count == 0;
        }

        /// <summary>
        ///     Shows the settings window to allow the user to make changes to the client editable settings
        /// </summary>
        /// <returns>System.Boolean.</returns>
        private bool UpdateSettings()
        {
            var settingsView = _viewResolver.CreateSettingsView();
            settingsView.Setup(_clientSettings);

            var result = settingsView.ShowDialog(_mainView);
            if (result.HasValue && result.Value)
            {
                //merge real client settings with changed ones
                _clientSettings.Merge(settingsView.ClientSettings);
                this.IntegrateWithWindowsStartup(_clientSettings.StartWithWindows);
            }

            return result.HasValue && result.Value;
        }

        /// <summary>
        ///     Restarts the auction watcher.
        /// </summary>
        private void RestartAuctionWatch()
        {
            _logger.Info($"Interface Status: {!_trackerWindowModel.IsDisabled}");
            if (_trackerWindowModel.IsDisabled)
                return;

            //start the data dispatcher for server/client communications
            _dataDispatcher.UpdateClientApikey(_clientSettings.ApiKey);
            _dataDispatcher.Reset();
            if (_clientSettings.EnableToasts && !string.IsNullOrWhiteSpace(_clientSettings.ApiKey))
                _messageMonitor.Start();
            else
                _messageMonitor.Stop();

            //start processing the message queue
            _uiMessageQueue.Start();
            _logger.Info("UI Mesage Queue Started");

            //stop the file watcher if its started (so we can rebuild it with new settings set by the user if need be)
            if (_watcher != null && _watcher.ScanningEnabled)
                _watcher.EndWatch();

            //ensure eq Folder is set
            if (string.IsNullOrWhiteSpace(_clientSettings.EQFolder))
            {
                _uiMessageQueue.QueueItem("No EQ Folder. Check File -> Settings.", MessageSeverity.Error, ActivityState.Error);
                return;
            }

            //rebuild the file watcher
            _watcher = new EQLogFolderWatcher(_clientSettings.EQFolder,
                _clientSettings as IIgnorableCharacterProvider,
                _globalSettings.InactivityTimeout,
                _globalSettings.DispatchDelay,
                _globalSettings.MaxAuctionsPerBatch);

            //hook teh file watcher
            _watcher.AuctionMonitorStarted += this.LogWatcher_AuctionMonitorStarted;
            _watcher.AuctionMonitorEnded += this.LogWatcher_AuctionMonitorEnded;
            _watcher.AuctionRead += this.LogWatcher_AuctionRead;

            //start watching for log files changes
            var beginWatch = _watcher.BeginWatch();
            if (beginWatch)
            {
                //notify listeners of successful start
                _logger.Info("Auction service initialized");
                _logger.InfoFormat("API Key: {0}", string.IsNullOrWhiteSpace(_clientSettings.ApiKey) ? "-Not Set-" : _clientSettings.ApiKey);
                _uiMessageQueue.QueueItem(MessageNoCharactersLoggedIn, MessageSeverity.Informational, ActivityState.Idle);
                _trackerWindowModel.UpdateCharacterMonitorStatus(dicCharactersBeingWatched.Values);
            }
            else
            {
                //unable to monitor log files
                //Error out and ask for update of file folder if needed (show settings window)
                this.ShowApplicationWindow();
                if (!Directory.Exists(_watcher.LogFolder))
                    _uiMessageQueue.QueueItem("Invalid Log Folder. Check Settings", MessageSeverity.Error, ActivityState.Error);
                else
                    _uiMessageQueue.QueueItem(MessageCheckLog, MessageSeverity.Error, ActivityState.Error);
            }
        }

        /// <summary>
        ///     Disables the application entirely, stopping all application processes
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="numberOfMinsToLockFor">The number of mins to lock the app (0 = indefinite lock).</param>
        private void DisableApplication(string errorMessage, int numberOfMinsToLockFor = 0)
        {
            //try and flush the auction and message queues
            _auctionQueue?.Flush();
            _uiMessageQueue?.Stop();
            _uiMessageQueue?.Flush();

            //stop server communications
            _dataDispatcher.Stop();
            _messageMonitor.Stop();

            //ensure the file watcher is stopped 
            if (_watcher != null && _watcher.ScanningEnabled)
                _watcher.EndWatch();

            //disable the display of the window data (auction counts) an display a "this is disabled" message
            _trackerWindowModel.Disable(errorMessage);
            _logger.Info("Auction service disabled");
            _notificationModel.UpdateActivityMonitor(ActivityState.Disabled, "Auction Services Disabled");

            //if the timer is already ticking (for some reason) for how long to disable
            //stop that timer so it can be rstarted
            if (_disableTimer != null && _disableTimer.Enabled)
                _disableTimer.Stop();

            //if we are locking for a set amount of time begin the count down
            if (numberOfMinsToLockFor > 0)
            {
                if (_disableTimer == null)
                {
                    _disableTimer = new Timer();
                    _disableTimer.Elapsed += this.DisableTimer_Elapsed;
                }
                if (_disableTimer.Enabled)
                    _disableTimer.Stop();
                _disableTimer.AutoReset = false;
                _disableTimer.Interval = numberOfMinsToLockFor*60*1000;
                _disableTimer.Start();
                _logger.Info($"Auction service will resume in {numberOfMinsToLockFor}min(s)");
            }
        }


        /// <summary>
        /// An event fired after the "disable for X time" counter has elapsed
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Timers.ElapsedEventArgs" /> instance containing the event data.</param>
        private void DisableTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _trackerWindowModel.Enable();
            this.RestartAuctionWatch();
        }


        /// <summary>
        /// An event fired when the ui message dequeues an item that then needs to be displayed (status bar and priamry window items)
        /// </summary>
        /// <param name="o">The o.</param>
        /// <param name="e">The e.</param>
        private void UiMessageQueue_ActiveItemChanged(object o, AutoQueueEventArgs<ApplicationStatusMessage> e)
        {
            //update the progress bar with the message
            _trackerWindowModel.UpdateTransmissionStatus(e.Item.OperationPercentComplete);

            //update the status bar
            if (e.Item.Severity.IsSet() && e.Item.MinimumShowTime >= holdClientStateForXMilliseconds)
                _trackerWindowModel.UpdateStatusMessage(e.Item.Severity, e.Item.Message);

            //system tray updates
            if (_clientSettings.TrackStateInSystemTray)
            {
                if (e.Item.ActivityState.IsSet())
                    _notificationModel.UpdateActivityMonitor(e.Item.ActivityState, e.Item.Message, resetToIdleAfterXMilliseconds: e.Item.MinimumShowTime);
            }
        }

        /// <summary>
        /// An event fired when the data dispatcher experiences a state change
        /// </summary>
        /// <param name="o">The o.</param>
        /// <param name="e">The <see cref="P99Auctions.Client.Web.AuctionDispatchEventArgs" /> instance containing the event data.</param>
        private void DataDispatcher_StatusChanged(object o, AuctionDispatchEventArgs e)
        {
            switch (e.Code)
            {
                case DispatcherStatus.InvalidServiceAddress:
                case DispatcherStatus.ConfigurationError:
                    this.DisableApplication(e.Message);
                    break;
                case DispatcherStatus.RetryFailure:
                    this.DisableApplication($"Unable to contact the P99 Auction Services, next scheduled attempt at {DateTime.Now.AddMinutes(_globalSettings.DisabledCommunicationsInterval).ToShortTimeString()}", _globalSettings.DisabledCommunicationsInterval);
                    break;
                case DispatcherStatus.Error:
                case DispatcherStatus.Failure:
                    _uiMessageQueue.QueueItem(e.Message, 5000, MessageSeverity.Error, ActivityState.Error);
                    break;
                case DispatcherStatus.Complete:
                    _uiMessageQueue.QueueItem(e.Message, 2000, MessageSeverity.Success, ActivityState.Success, e.PercentComplete);
                    _uiMessageQueue.QueueItem(dicCharactersBeingWatched.Count > 0 ? MessageScanningForAuctions : MessageNoCharactersLoggedIn, MessageSeverity.Success, ActivityState.Idle);
                    break;
                case DispatcherStatus.Idle:
                    _uiMessageQueue.QueueItem(dicCharactersBeingWatched.Count > 0 ? MessageScanningForAuctions : MessageNoCharactersLoggedIn, MessageSeverity.Success, ActivityState.Idle);
                    break;
                case DispatcherStatus.Transmitting:
                case DispatcherStatus.Connecting:
                    _uiMessageQueue.QueueItem(e.Message, 1500, MessageSeverity.Caution, ActivityState.Transmitting, e.PercentComplete);
                    break;
            }
        }

        /// <summary>
        /// Handler for when teh user requests to view the main window
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        private void NotificationView_ShowAuctionTracker(object sender, EventArgs e)
        {
            this.ShowApplicationWindow();
        }


        /// <summary>
        /// Event fired when the log watcher releases a hook on a character log file
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="P99Auctions.Client.Watchers.AuctionLogFileEventArgs" /> instance containing the event data.</param>
        private void LogWatcher_AuctionMonitorEnded(object sender, AuctionLogFileEventArgs e)
        {
            _logger.Info($"Auction Monitor Ended: {e.CharacterName}");
            ActiveCharacter stats;
            if (dicCharactersBeingWatched.TryRemove(e.CharacterName, out stats))
                _trackerWindowModel.UpdateCharacterMonitorStatus(dicCharactersBeingWatched.Values);

            if (dicCharactersBeingWatched.Count == 0)
                _uiMessageQueue.QueueItem(MessageNoCharactersLoggedIn, MessageSeverity.Informational, ActivityState.Idle);
        }

        /// <summary>
        /// Event fired when the log watcher begins watching a log file
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="P99Auctions.Client.Watchers.AuctionLogFileEventArgs" /> instance containing the event data.</param>
        private void LogWatcher_AuctionMonitorStarted(object sender, AuctionLogFileEventArgs e)
        {
            _logger.Info($"Auction Monitor Started: {e.CharacterName}");
            var addSuccess = dicCharactersBeingWatched.TryAdd(e.CharacterName, new ActiveCharacter
            {
                CharacterName = e.CharacterName,
                FileName = e.FileName
            });

            if (addSuccess)
                _trackerWindowModel.UpdateCharacterMonitorStatus(dicCharactersBeingWatched.Values);
            _uiMessageQueue.QueueItem(MessageScanningForAuctions, MessageSeverity.Success, ActivityState.Idle);
        }

        /// <summary>
        /// Event fired by the auction watcher when an auction is read from the log file
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="P99Auctions.Client.Watchers.AuctionReadEventArgs" /> instance containing the event data.</param>
        private void LogWatcher_AuctionRead(object sender, AuctionReadEventArgs e)
        {
            _logger.Debug($"Auction Queuing: [{e.CharacterName}] {e.Line}");
            _clientSettings.AddAuctionCounter();

            //update hte global counts and queue a message for UI status pieces to be updated
            _trackerWindowModel.UpdateAuctionCounts(_clientSettings.TodayAuctionCount, _clientSettings.LifetimeAuctionCount);
            _uiMessageQueue.QueueItem("Enqueuing Auction", 300, activityState: ActivityState.AuctionFound);
            _uiMessageQueue.QueueItem(MessageScanningForAuctions, activityState: ActivityState.Idle);

            //has the auction queue been created yet? if not do so
            if (_auctionQueue == null)
                _auctionQueue = new AuctionQueue(_dataDispatcher, _globalSettings.DispatchDelay, _globalSettings.MaxAuctionsPerBatch);

            //queue the found auction for dispatch
            _auctionQueue.EnqueueAuction(e.Line);
        }


        /// <summary>
        /// Even fired when the monitor recieves an alert/message from teh server
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="P99Auctions.Client.Watchers.MessageRecievedEventArgs" /> instance containing the event data.</param>
        private void MessageMonitor_MessageReceived(object sender, MessageRecievedEventArgs e)
        {
            //create the alert 
            var msg = new BalloonMessage
            {
                Message = e.Message.Message,
                Title = e.Message.Title,
                Url = e.Message.Url,
                MessageColor = BalloonMessage.CreateColorFromMessageType(e.Message.MessageType),
                ShowForMilliseconds = _clientSettings.ToastDisplayForSeconds*1000
            };

            //show the alert to the user
            _balloonController.ShowBalloonMessage(msg);
        }

        /// <summary>
        /// Handler when the user requests to see the client side log
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        private void MainWindow_ViewLog(object sender, EventArgs e)
        {
            try
            {
                _logger.Info($"Showing Log File: {_globalSettings.LogFileName}");
                if (!File.Exists(_globalSettings.LogFileName))
                {
                    var stream = File.Create(_globalSettings.LogFileName);
                    stream.Close();
                }

                ProcessStartInfo psi = new ProcessStartInfo(_globalSettings.LogFileName) {UseShellExecute = true};
                Process.Start(psi);
            }
            catch (FileNotFoundException)
            {
                _logger.Error($"Log File Not Found {_globalSettings.LogFileName}");
                _uiMessageQueue.QueueItem("Unable to launch the Log File", 4000, MessageSeverity.Caution);
            }
            catch (Exception ex)
            {
                _logger.Error($"Unable to launch the log file  {_globalSettings.LogFileName}", ex);
                _uiMessageQueue.QueueItem("Unable to launch the Log File", 4000, MessageSeverity.Caution);
            }
        }

        /// <summary>
        /// Handler when the user requests to update their settings
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        private void MainWindow_EditSettings(object sender, EventArgs e)
        {
            _logger.Info("Edit Settings Requested");
            var result = this.UpdateSettings();
            if (result)
            {
                _logger.Info("Client Settings Updated Successfully");
                this.RestartAuctionWatch();
            }
            else
                _logger.Error("Client Settings Update Failed");
        }

        /// <summary>
        /// Handler when the user fires the "implied closed" command. Settings based action is taken (minimize or close)
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs" /> instance containing the event data.</param>
        private void MainWindow_CloseImplied(object sender, CancelEventArgs e)
        {
            //shrink to system tray if allowed or exit the application
            if (!_blnShuttingDown)
            {
                _logger.Info($"Implied Shutdown Initiated. Minimize={_clientSettings.MinimizeToTray}");
                if (_clientSettings.MinimizeToTray)
                    this.CloseApplicationWindow();
                else
                    this.ForceApplicationShutdown();
            }
        }

        /// <summary>
        /// Handler when the explicit close command is fired (the application is closed completely)
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        private void MainWindow_CloseExplicit(object sender, EventArgs e)
        {
            //exit the application 
            _logger.Info("Explicit Shutdown Initiated");
            _blnShuttingDown = true;
            this.ForceApplicationShutdown();
        }

        /// <summary>
        /// Handler for launching hte help page on p99auctions
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        private void MainWindow_Help(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.p99auctions.com/Help");
        }


        /// <summary>
        ///     Gets the client settings.
        /// </summary>
        /// <value>The client settings.</value>
        public IClientSettings ClientSettings
        {
            get { return _clientSettings; }
        }

        /// <summary>
        ///     Gets the notification model.
        /// </summary>
        /// <value>The notification model.</value>
        public NotifyIconViewModel NotificationModel
        {
            get { return _notificationModel; }
        }
    }
}