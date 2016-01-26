// *************************************************************
// P99 Auctions Community Team
// -----------
// Solution:     Project 1999 Auction Tracker
// File:         ClientSettings.cs
// Email:        info@p99auctions.com
// Website:      www.p99auctions.com
// Last Update:  01/25/2016 10:02 PM
// *************************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using P99Auctions.Client.Interfaces;

namespace P99Auctions.Client.Models
{
    /// <summary>
    ///     Class ClientSettings.
    /// </summary>
    [Serializable]
    public class ClientSettings : IClientSettings, IIgnorableCharacterProvider
    {
        private string _apiKey;
        private bool _enableToasts;
        private string _eqFolder;

        private List<string> _ignoreList;
        private bool _minimizeToTray;
        private bool _startWithWindows;
        private int _toastDisplayForSeconds;
        private int _todaysAuctions;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ClientSettings" /> class.
        /// </summary>
        private ClientSettings()
        {
            _ignoreList = new List<string>();
            this.EnableToasts = true;
            this.StartWithWindows = true;
            this.MinimizeToTray = true;
            this.TrackStateInSystemTray = true;
            this.ToastDisplayForSeconds = 30;
        }

        /// <summary>
        /// Gets the file path.
        /// </summary>
        /// <returns>System.String.</returns>
        private static string GetFilePath()
        {
            var dirPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            return Path.Combine(dirPath, "client.config");
        }

        /// <summary>
        /// Loads this instance.
        /// </summary>
        /// <returns>ClientSettings.</returns>
        public static ClientSettings Load()
        {
            try
            {
                var serializer = new XmlSerializer(typeof (ClientSettings));
                using (var reader = new StreamReader(GetFilePath()))
                {
                    var settings = (ClientSettings) serializer.Deserialize(reader);
                    settings.ZeroOutTodaysAuctionsOnDayChange();
                    return settings;
                }
            }
            catch
            {
            }
            return new ClientSettings();
        }

        /// <summary>
        /// Saves the specified settings.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static void Save(ClientSettings settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));

            try
            {
                var serializer = new XmlSerializer(typeof (ClientSettings));
                using (var writer = new StreamWriter(GetFilePath()))
                    serializer.Serialize(writer, settings);
            }
            catch
            {
            }
        }

        /// <summary>
        ///     Zeroes the out todays auctions on day change.
        /// </summary>
        private void ZeroOutTodaysAuctionsOnDayChange()
        {
            if (!this.LastDateCounted.HasValue || this.LastDateCounted.Value != DateTime.Now.Date)
            {
                this.LastDateCounted = DateTime.Now.Date;
                this.TodayAuctionCount = 0;
            }
        }

        /// <summary>
        /// Sets the ignore list.
        /// </summary>
        /// <param name="value">The value.</param>
        private void SetIgnoreList(string value)
        {
            _ignoreList.Clear();
            if (!string.IsNullOrWhiteSpace(value))
                _ignoreList.AddRange(value.Trim().Split(',').Select(x => x.Trim()));
        }

        private string CreateIgnoreList()
        {
            return string.Join(", ", _ignoreList);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Retreives the validation errors.
        /// </summary>
        /// <returns>List&lt;System.String&gt;.</returns>
        public List<string> RetreiveValidationErrors()
        {
            var lst = new List<string>();

            if (string.IsNullOrWhiteSpace(this.EQFolder))
                lst.Add("Invalid Everquest Directory");

            else
            {
                var dir = new DirectoryInfo(this.EQFolder);
                if (!dir.Exists)
                    lst.Add("Invalid Everquest Directory");
                else
                {
                    if (dir.GetFiles("eqgame.exe").Length == 0)
                        lst.Add("Unable to validate your everquest directory.\r\n Ensure you are selecting the top level folder (with eqgame.exe) and not the log folder itself.");
                }
            }

            if (!string.IsNullOrWhiteSpace(this.ApiKey) && this.ApiKey.Length > 35)
                lst.Add("Invalid Account Key. Your Account Key must be less than 35 characters long");

            if (!string.IsNullOrWhiteSpace(this.IgnoreList) && this.IgnoreList.Length > 150)
                lst.Add("Your ignore list can be at most 150 character long. Sorry about that");

            if (this.ToastDisplayForSeconds < 1 || this.ToastDisplayForSeconds > 180)
                lst.Add("Auction alerts can only between displayed for 1-60 seconds.");

            return lst;
        }

        /// <summary>
        ///     Adds a single auction to the client counters
        /// </summary>
        public void AddAuctionCounter()
        {
            this.ZeroOutTodaysAuctionsOnDayChange();
            this.TodayAuctionCount += 1;
            this.LifetimeAuctionCount += 1;
        }

        /// <summary>
        ///     Gets or sets the eq folder to watch
        /// </summary>
        /// <value>The eq folder.</value>
        public string EQFolder
        {
            get { return _eqFolder; }
            set
            {
                _eqFolder = value?.Trim();
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Gets or sets the API key.
        /// </summary>
        /// <value>The API key.</value>
        public string ApiKey
        {
            get { return _apiKey; }
            set
            {
                _apiKey = value?.Trim();
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether [enable toasts].
        /// </summary>
        /// <value><c>true</c> if [enable toasts]; otherwise, <c>false</c>.</value>
        public bool EnableToasts
        {
            get { return _enableToasts; }
            set
            {
                _enableToasts = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Gets or sets the toast display for seconds.
        /// </summary>
        /// <value>The toast display for seconds.</value>
        public int ToastDisplayForSeconds
        {
            get { return _toastDisplayForSeconds; }
            set
            {
                _toastDisplayForSeconds = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether [start with windows].
        /// </summary>
        /// <value><c>true</c> if [start with windows]; otherwise, <c>false</c>.</value>
        public bool StartWithWindows
        {
            get { return _startWithWindows; }
            set
            {
                _startWithWindows = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Gets or sets the lifetime auction count.
        /// </summary>
        /// <value>The lifetime auction count.</value>
        public int LifetimeAuctionCount { get; set; }

        /// <summary>
        ///     Gets or sets today's auction count.
        /// </summary>
        /// <value>The today auction count.</value>
        public int TodayAuctionCount
        {
            get { return _todaysAuctions; }
            set { _todaysAuctions = value; }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether to minimize to the system tray when the applicaiton is minimized
        /// </summary>
        /// <value><c>true</c> if [minimize to tray]; otherwise, <c>false</c>.</value>
        public bool MinimizeToTray
        {
            get { return _minimizeToTray; }
            set
            {
                _minimizeToTray = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Gets or sets the list of ignored characters
        /// </summary>
        /// <value>The ignore list.</value>
        public string IgnoreList
        {
            get { return this.CreateIgnoreList(); }
            set
            {
                this.SetIgnoreList(value);
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the system tray icon is updated to reflect the current state of the
        ///     applicationb
        /// </summary>
        /// <value><c>true</c> if state changes will be reflected in the system tray; otherwise, <c>false</c>.</value>
        public bool TrackStateInSystemTray { get; set; }

        public object Clone()
        {
            var settings = new ClientSettings
            {
                ApiKey = this.ApiKey,
                EQFolder = this.EQFolder,
                IgnoreList = this.IgnoreList,
                MinimizeToTray = this.MinimizeToTray,
                EnableToasts = this.EnableToasts,
                ToastDisplayForSeconds = this.ToastDisplayForSeconds,
                StartWithWindows = this.StartWithWindows,
                TodayAuctionCount = this.TodayAuctionCount,
                LifetimeAuctionCount = this.LifetimeAuctionCount,
                LastDateCounted = this.LastDateCounted,
                TrackStateInSystemTray = this.TrackStateInSystemTray
            };

            return settings;
        }

        public void Merge(IClientSettings settingsToMergeIn)
        {
            this.ApiKey = settingsToMergeIn.ApiKey?.Trim();
            this.EQFolder = settingsToMergeIn.EQFolder?.Trim();
            this.IgnoreList = settingsToMergeIn.IgnoreList?.Trim();
            this.MinimizeToTray = settingsToMergeIn.MinimizeToTray;
            this.EnableToasts = settingsToMergeIn.EnableToasts;
            this.ToastDisplayForSeconds = settingsToMergeIn.ToastDisplayForSeconds;
            this.StartWithWindows = settingsToMergeIn.StartWithWindows;
            this.TrackStateInSystemTray = settingsToMergeIn.TrackStateInSystemTray;
        }

        /// <summary>
        ///     Determines whether the specified character name is ignored.
        /// </summary>
        /// <param name="characterName">Name of the character.</param>
        /// <returns><c>true</c> if the specified character name is ignored; otherwise, <c>false</c>.</returns>
        public bool IsIgnored(string characterName)
        {
            return _ignoreList.Any(x => x.Equals(characterName, StringComparison.InvariantCultureIgnoreCase));
        }

        public DateTime? LastDateCounted { get; set; }
    }
}