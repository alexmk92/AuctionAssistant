// *************************************************************
// P99 Auctions Community Team
// -----------
// Solution:     Project 1999 Auction Tracker
// File:         LoggerConfiguration.cs
// Email:        info@p99auctions.com
// Website:      www.p99auctions.com
// Last Update:  01/25/2016 10:01 PM
// *************************************************************

using System.IO;
using System.Reflection;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Util;
using P99Auctions.Client.Interfaces;

namespace P99Auctions.Client.Helpers
{
    /// <summary>
    /// Configruation class for Log4net
    /// </summary>
    public static class LoggerConfiguration
    {
        /// <summary>
        /// Setups the logger using global settings
        /// </summary>
        /// <param name="globalSettings">The global settings.</param>
        public static void Setup(IGlobalSettings globalSettings)
        {
            bool blnConfigured = false;
            try
            {
                //if an xml file is present use that
                var executingFile = new FileInfo(Assembly.GetExecutingAssembly().Location);
                if (executingFile.Directory != null)
                {
                    var file = new FileInfo(Path.Combine(executingFile.Directory.FullName, "log.config"));
                    if (file.Exists)
                    {
                        log4net.Config.XmlConfigurator.Configure(file);
                        blnConfigured = true;
                    }
                }
            }
            catch
            {
            }

            //if no configruation file was found via global settings (or it failed)
            //use fallback default 
            if (blnConfigured == false)
            {
                var patternLayout = new PatternLayout
                {
                    Header = new PatternString("[START MODE=%property{StartMode} DATE=%date{yyyy-MM-dd HH:mm:ss}]\r\n").Format(),
                    Footer = "[END]\r\n",
                    ConversionPattern = "[%date{yyyy-MM-dd HH:mm:ss}] - %message%newline%exception"
                };


                var roller = new RollingFileAppender
                {
                    Name = "P99AuctionLogger",
                    AppendToFile = true,
                    File = globalSettings.LogFileName,
                    Layout = patternLayout,
                    MaxSizeRollBackups = 5,
                    MaximumFileSize = "10MB",
                    RollingStyle = RollingFileAppender.RollingMode.Size
                };


                var hierarchy = (log4net.Repository.Hierarchy.Hierarchy) LogManager.GetRepository();
                var root = hierarchy.Root;
                var attachable = root as IAppenderAttachable;
                root.Level = Level.Warn;

                if (attachable != null)
                {
                    attachable.AddAppender(roller);
                    roller.ActivateOptions();
                }

                hierarchy.Configured = true;
            }
        }
    }
}