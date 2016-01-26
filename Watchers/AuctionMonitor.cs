// *************************************************************
// P99 Auctions Community Team
// -----------
// Solution:     Project 1999 Auction Tracker
// File:         AuctionMonitor.cs
// Email:        info@p99auctions.com
// Website:      www.p99auctions.com
// Last Update:  01/25/2016 10:02 PM
// *************************************************************

using System;
using System.IO;
using System.Text.RegularExpressions;
using P99Auctions.Client.Models;

namespace P99Auctions.Client.Watchers
{
    /// <summary>
    ///     A monitor for extracting auction text from a single log file
    /// </summary>
    public class AuctionMonitor
    {
        private readonly string _characterName;
        private FileInfo _fi;
        private volatile int _secondsBeforeClose;

        private DateTime dtLastRead = DateTime.MinValue;
        private long lastMaxOffset = long.MaxValue;

        /// <summary>
        ///     Occurs when an auction is read from the data stream
        /// </summary>
        public event EventHandler<AuctionReadEventArgs> AuctionRead;


        /// <summary>
        ///     Initializes a new instance of the <see cref="AuctionMonitor" /> class.
        /// </summary>
        /// <param name="clientLogger">The client logger.</param>
        /// <param name="fileName">The log file to watch</param>
        /// <param name="secondsBeforeClose">
        ///     The number of seconds that must elapse without a log entry before the log is
        ///     considered 'closed' and monitoring stops.
        /// </param>
        public AuctionMonitor(string fileName, int secondsBeforeClose = 10)
        {
            if (fileName == null) throw new ArgumentNullException(nameof(fileName));

            _fi = new FileInfo(fileName);
            if (!_fi.Exists)
                throw new InvalidOperationException($"Invalid or nonexistant file: {_fi.Name}");

            if (!Regex.IsMatch(_fi.Name, LoggingConstants.P99FileFilterRegex))
                throw new InvalidOperationException($"Invalid P99 Blue log file format: {_fi.Name}");

            _secondsBeforeClose = secondsBeforeClose;

            _characterName = ExtractCharacterName(_fi.Name);

            if (_fi.Exists)
                lastMaxOffset = _fi.Length;
        }

        /// <summary>
        ///     Called when a batch of auction lines is ready for transmittal
        /// </summary>
        /// <param name="line">The line.</param>
        protected virtual void OnAuctionRead(string line)
        {
            var e = new AuctionReadEventArgs(line)
            {
                CharacterName = _characterName,
                FileName = _fi.Name
            };
            AuctionRead?.Invoke(this, e);
        }

        /// <summary>
        ///     Extracts the name of the character from a filename
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>System.String.</returns>
        public static string ExtractCharacterName(string fileName)
        {
            return fileName.Replace(LoggingConstants.P99FilePrefix, string.Empty).Replace(LoggingConstants.P99FileSuffix, string.Empty);
        }


        /// <summary>
        ///     Begins the monitor.
        /// </summary>
        public void ScanNewContent()
        {
            try
            {
                using (var reader = new StreamReader(new FileStream(this.File.FullName,
                    FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                {
                    //if the file has been truncated edited or somehow changed since 
                    //the last read, skip any potential adds and seek to the end of the file
                    //if the file has grown in size though we can resonably assume 
                    //we're starting from the point of last read
                    if (lastMaxOffset > reader.BaseStream.Length)
                        lastMaxOffset = reader.BaseStream.Length;

                    //seek to the last max offset
                    reader.BaseStream.Seek(lastMaxOffset, SeekOrigin.Begin);

                    //read out of the file until the EOF
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (this.IsAuctionLine(line))
                        {
                            line = line.Replace("You auction, '", $"{_characterName} auctions, '");
                            this.OnAuctionRead(line);
                        }
                    }

                    //update the last max offset and scan time
                    lastMaxOffset = reader.BaseStream.Position;
                    dtLastRead = DateTime.Now;
                }
            }
            catch
            {
            }
        }

        /// <summary>
        ///     Determines whether [is auction line] [the specified line].
        /// </summary>
        /// <param name="line">The line.</param>
        /// <returns>System.Boolean.</returns>
        private bool IsAuctionLine(string line)
        {
            //all auction lines start with:  [Sat Nov 28 04:16:07 2015] SoAndSo auctions, '

            //ensure the line is long neough to contain a date a name and auction line
            if (string.IsNullOrWhiteSpace(line))
                return false;
            line = line.Trim();

            if (!line.Contains("'"))
                return false;

            if (line.Length < 41) //length of date + brackets + spaces on side of name + auctions phrase + comma + apostrophe
                return false;

            if (line.Length < line.IndexOf("'", StringComparison.Ordinal) + 1)
                return false;

            var sub = line.Substring(0, line.IndexOf("'", StringComparison.Ordinal) + 1);
            return sub.StartsWith("[") && (sub.EndsWith("auctions, '") || sub.EndsWith("You auction, '"));
        }

        /// <summary>
        ///     Gets or sets the log file reference
        /// </summary>
        /// <value>The name of the file.</value>
        public FileInfo File => _fi;

        /// <summary>
        ///     Gets or sets the seconds to wait for a file change before close the file monitor
        /// </summary>
        /// <value>The seconds before close.</value>
        public int SecondsBeforeClose => _secondsBeforeClose;

        /// <summary>
        ///     Gets or sets the last scan date.
        /// </summary>
        /// <value>The last scan date.</value>
        public DateTime LastScanDate => dtLastRead;

        /// <summary>
        /// Gets the name of the character.
        /// </summary>
        /// <value>The name of the character.</value>
        public string CharacterName
        {
            get { return _characterName; }
        }
    }
}