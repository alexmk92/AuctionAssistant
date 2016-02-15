// *************************************************************
// P99 Auctions Community Team
// -----------
// Solution:     Project 1999 Auction Tracker
// File:         MainWindowViewModel.cs
// Email:        info@p99auctions.com
// Website:      www.p99auctions.com
// Last Update:  01/25/2016 10:02 PM
// *************************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using P99Auctions.Client.Properties;

namespace P99Auctions.Client.Models
{
    /// <summary>
    /// Class MainWindowViewModel.
    /// </summary>
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private Visibility _applicationWindowVisibility;
        private string _auctionsLifeTime;
        private string _auctionsThisSession;
        private string _characterName;
        private string _disabledMessage;
        private Visibility _disabledWindowVisibility;
        private string _logFileName;
        private bool _progressInfinite;
        private int _progressValue;
        private Visibility _showProgressBar;
        private BitmapImage _statusImage;
        private string _statusText;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public MainWindowViewModel()
        {
            this.ApplicationControlVisibility = Visibility.Visible;
            this.DisabledWindowVisibility = Visibility.Collapsed;
            this.UpdateStatusMessage(MessageSeverity.Informational, "-Unknown-");
            this.UpdateCharacterMonitorStatus(new ActiveCharacter[] {});
        }

        /// <summary>
        /// Updates the status message.
        /// </summary>
        /// <param name="severity">The severity.</param>
        /// <param name="message">The message.</param>
        public void UpdateStatusMessage(MessageSeverity severity, string message)
        {
            this.StatusText = message;
            this.SetModelImage(severity);
        }


        /// <summary>
        /// Updates the character monitor status.
        /// </summary>
        /// <param name="activeCharacters">The active characters.</param>
        public void UpdateCharacterMonitorStatus([NotNull] IEnumerable<ActiveCharacter> activeCharacters)
        {
            var _active = activeCharacters as IList<ActiveCharacter> ?? activeCharacters.ToList();

            if (!_active.Any())
            {
                this.CharacterName = "-Unknown-";
                this.LogFileName = "-Waiting. Do you have logging enabled?-";
            }
            else if (_active.Count == 1)
            {
                this.CharacterName = _active[0].CharacterName;
                this.LogFileName = _active[0].FileName;
            }
            else
            {
                if (_active.Count == 2)
                    this.CharacterName = string.Join(", ", _active.OrderBy(x => x.CharacterName).Select(y => y.CharacterName));
                else
                    this.CharacterName = "-Multiple-";
                this.LogFileName = "-Multiple-";
            }
        }

        /// <summary>
        /// Updates the auction counts.
        /// </summary>
        /// <param name="thisSession">The this session.</param>
        /// <param name="lifeTime">The life time.</param>
        public void UpdateAuctionCounts(int thisSession, int lifeTime)
        {
            this.AuctionsThisSession = this.FormatAuctionCountForDisplay(thisSession);
            this.AuctionsLifeTime = this.FormatAuctionCountForDisplay(lifeTime);
        }


        /// <summary>
        /// Formats the auction count for display.
        /// </summary>
        /// <param name="count">The count.</param>
        /// <returns>System.String.</returns>
        private string FormatAuctionCountForDisplay(int count)
        {
            if (count < 0)
                return "0";

            if (count < 99999)
                return count.ToString("##,##0");
            if (count < 1000000)
                return (count/1000).ToString("##0k");

            return (count/1000/1000).ToString("##,###,##0m");
        }

        /// <summary>
        /// Sets the model image.
        /// </summary>
        /// <param name="severity">The severity.</param>
        private void SetModelImage(MessageSeverity severity)
        {
            BitmapImage img = new BitmapImage();
            img.BeginInit();
            switch (severity)
            {
                case MessageSeverity.Caution:
                    img.StreamSource = Application.GetResourceStream(new Uri(@"pack://application:,,,/Images/ledyellow.png"))?.Stream;
                    break;
                case MessageSeverity.Error:
                    img.StreamSource = Application.GetResourceStream(new Uri(@"pack://application:,,,/Images/ledred.png"))?.Stream;
                    break;
                case MessageSeverity.Success:
                    img.StreamSource = Application.GetResourceStream(new Uri(@"pack://application:,,,/Images/ledgreen.png"))?.Stream;
                    break;
                default:
                    img.StreamSource = Application.GetResourceStream(new Uri(@"pack://application:,,,/Images/ledlightblue.png"))?.Stream;
                    break;
            }

            img.EndInit();
            img.Freeze();
            Dispatcher.CurrentDispatcher.Invoke(() => this.StatusImage = img);
        }

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Disables the specified display message.
        /// </summary>
        /// <param name="displayMessage">The display message.</param>
        public void Disable(string displayMessage)
        {
            this.DisabledWindowVisibility = Visibility.Visible;
            this.ApplicationControlVisibility = Visibility.Collapsed;
            this.DisabledMessage = displayMessage;
        }

        /// <summary>
        /// Enables this instance.
        /// </summary>
        public void Enable()
        {
            this.DisabledWindowVisibility = Visibility.Collapsed;
            this.ApplicationControlVisibility = Visibility.Visible;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the status image.
        /// </summary>
        /// <value>The status image.</value>
        public BitmapImage StatusImage
        {
            get { return _statusImage; }
            set
            {
                _statusImage = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the status text.
        /// </summary>
        /// <value>The status text.</value>
        public string StatusText
        {
            get { return _statusText; }
            set
            {
                _statusText = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the show progress bar.
        /// </summary>
        /// <value>The show progress bar.</value>
        public Visibility ShowProgressBar
        {
            get { return _showProgressBar; }
            set
            {
                _showProgressBar = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the progress value.
        /// </summary>
        /// <value>The progress value.</value>
        public int ProgressValue
        {
            get { return _progressValue; }
            set
            {
                _progressValue = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the progress infinite.
        /// </summary>
        /// <value>The progress infinite.</value>
        public bool ProgressInfinite
        {
            get { return _progressInfinite; }
            set
            {
                _progressInfinite = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the name of the character.
        /// </summary>
        /// <value>The name of the character.</value>
        public string CharacterName
        {
            get { return _characterName; }
            set
            {
                _characterName = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the name of the log file.
        /// </summary>
        /// <value>The name of the log file.</value>
        public string LogFileName
        {
            get { return _logFileName; }
            set
            {
                _logFileName = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the auctions this session.
        /// </summary>
        /// <value>The auctions this session.</value>
        public string AuctionsThisSession
        {
            get { return _auctionsThisSession; }
            set
            {
                _auctionsThisSession = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the auctions life time.
        /// </summary>
        /// <value>The auctions life time.</value>
        public string AuctionsLifeTime
        {
            get { return _auctionsLifeTime; }
            set
            {
                _auctionsLifeTime = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the application control visibility.
        /// </summary>
        /// <value>The application control visibility.</value>
        public Visibility ApplicationControlVisibility
        {
            get { return _applicationWindowVisibility; }
            set
            {
                _applicationWindowVisibility = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the disabled window visibility.
        /// </summary>
        /// <value>The disabled window visibility.</value>
        public Visibility DisabledWindowVisibility
        {
            get { return _disabledWindowVisibility; }
            set
            {
                _disabledWindowVisibility = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the disabled message.
        /// </summary>
        /// <value>The disabled message.</value>
        public string DisabledMessage
        {
            get { return _disabledMessage; }
            set
            {
                _disabledMessage = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Gets a value indicating if the interface is disabled
        /// </summary>
        /// <value>The is disabled.</value>
        public bool IsDisabled
        {
            get { return this.ApplicationControlVisibility != Visibility.Visible; }
        }
    }
}