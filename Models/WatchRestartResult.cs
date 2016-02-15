using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P99Auctions.Client.Models
{
    public class WatchRestartResult
    {
        public bool Successful { get; set; }
        public string ErrorMessage { get; set; }

        public bool TryAgainLater { get; set; }
        public static WatchRestartResult Fail
        {
            get
            {
                return new WatchRestartResult
                {
                    ErrorMessage = "",
                    Successful = false,
                    TryAgainLater = false
                };
            }
        }

        public static WatchRestartResult Success
        {
            get
            {
                return new WatchRestartResult
                {
                    ErrorMessage = "",
                    Successful = true,
                    TryAgainLater = true
                };
            }
        }
    }
}
