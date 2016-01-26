// *************************************************************
// P99 Auctions Community Team
// -----------
// Solution:     Project 1999 Auction Tracker
// File:         BalloonMessage.cs
// Email:        info@p99auctions.com
// Website:      www.p99auctions.com
// Last Update:  01/25/2016 10:02 PM
// *************************************************************

using System;
using System.Drawing;
using P99Auctions.Client.Interfaces;
using P99Auctions.Client.Web;

namespace P99Auctions.Client.Balloons
{
    /// <summary>
    /// A displayable message show to the user
    /// </summary>
    public class BalloonMessage : IBalloonMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BalloonMessage"/> class.
        /// </summary>
        public BalloonMessage()
        {
            this.Id = Guid.NewGuid().ToString("N");
        }

        /// <summary>
        /// Conversion method to create the display color (of the balloon shade) from the message type
        /// </summary>
        /// <param name="severity">The severity.</param>
        /// <returns>Color.</returns>
        public static Color CreateColorFromMessageType(ClientTrackerMessageType severity)
        {
            switch (severity)
            {
                case ClientTrackerMessageType.Info:
                case ClientTrackerMessageType.ItemBeingBought:
                    return Color.LightBlue;
                case ClientTrackerMessageType.Success:
                case ClientTrackerMessageType.ItemForSale:
                    return Color.ForestGreen;
                case ClientTrackerMessageType.Error:
                    return Color.Red;
                case ClientTrackerMessageType.Warning:
                    return Color.Gold;
                default:
                    return Color.White;
            }
        }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public string Id { get; private set; }

        /// <summary>
        /// Gets or sets the title of the balloon
        /// </summary>
        /// <value>The title.</value>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the message shown 
        /// </summary>
        /// <value>The message.</value>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the URL launched if the user clicks the balloon
        /// </summary>
        /// <value>The URL.</value>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets how long the balloon will show
        /// </summary>
        /// <value>The show for milliseconds.</value>
        public int ShowForMilliseconds { get; set; }

        /// <summary>
        /// Gets or sets the color of the message.
        /// </summary>
        /// <value>The color of the message.</value>
        public Color MessageColor { get; set; }
    }
}