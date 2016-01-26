// *************************************************************
// P99 Auctions Community Team
// -----------
// Solution:     Project 1999 Auction Tracker
// File:         App.xaml.cs
// Email:        info@p99auctions.com
// Website:      www.p99auctions.com
// Last Update:  01/25/2016 10:02 PM
// *************************************************************

using System;
using System.Linq;
using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;
using log4net;
using P99Auctions.Client.Controllers;
using P99Auctions.Client.Helpers;
using P99Auctions.Client.Instancing;
using P99Auctions.Client.Interfaces;
using P99Auctions.Client.Models;

namespace P99Auctions.Client
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, IViewResolver
    {
        IBalloonController _balloonController;
        private ClientController _controller;

        ILog _logger;
        private TaskbarIcon notifyIcon;

        public App()
        {
            this.DispatcherUnhandledException += this.App_DispatcherUnhandledException;
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if (_logger != null)
            {
                _logger.Fatal("A fatal error occured after start up", e.Exception);
                e.Handled = true;
                MessageBox.Show("An fatal error occured, P99 Auction House Assistant will now close. See the log for details.", "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);
                System.Environment.Exit(1);
            }
        }

        private void App_Start(object sender, StartupEventArgs e)
        {
            if (!SingleInstance.Start())
            {
                System.Windows.MessageBox.Show("The P99 Auction Assistant is already running.");
                Application.Current?.Shutdown();
                return;
            }

            var launchedByWindows = e.Args.Any(arg => arg.Equals(ClientController.StartMinimizedArgumentFlag, StringComparison.CurrentCultureIgnoreCase));

            //global settings
            IGlobalSettings _globalSettings = new GlobalSettings();

            //load logging
            log4net.GlobalContext.Properties["StartMode"] = launchedByWindows ? "SILENT" : "USER";
            LoggerConfiguration.Setup(_globalSettings);
            _logger = log4net.LogManager.GetLogger("P99AuctionAssistant");

            //client changable settings
            IClientSettings _clientSettings = ClientSettings.Load();

            //setup the logging utility
            try
            {
                notifyIcon = (TaskbarIcon) this.FindResource("NotifyIcon");
                _balloonController = new BalloonHelper(notifyIcon);
                //create the controller and start the application

                _controller = new ClientController(_clientSettings, _globalSettings, _balloonController, _logger, this);
                _controller.ApplicationShutDownRequested += this.Controller_ApplicationShutDownRequested;

                //wireup the notify icon
                if (notifyIcon != null)
                    notifyIcon.DataContext = _controller.NotificationModel;

                _controller.StartApplication(launchedByWindows);
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.Fatal("A fatal error occured during primary execution", ex);
                    MessageBox.Show("A fatal error occured, the Auction Assistant will now close. See the log for details.", "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                System.Environment.Exit(1);
            }
        }

        /// <summary>
        ///     Handles the event when the controller explicity requests that the environment shut down the application
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        private void Controller_ApplicationShutDownRequested(object sender, System.EventArgs e)
        {
            Application.Current?.Shutdown();
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
            _controller?.CloseApplication();
            if (_controller?.ClientSettings != null)
                ClientSettings.Save(_controller.ClientSettings as ClientSettings);

            notifyIcon?.Dispose();
            SingleInstance.Stop();
        }

        /// <summary>
        ///     Creates the main view.
        /// </summary>
        /// <returns>P99Auctions.Client.Data.IMainView.</returns>
        public IMainView CreateMainView()
        {
            return new MainWindow();
        }

        /// <summary>
        ///     Creates the settings view.
        /// </summary>
        /// <returns>P99Auctions.Client.Data.ISettingsView.</returns>
        public ISettingsView CreateSettingsView()
        {
            return new Settings();
        }
    }
}