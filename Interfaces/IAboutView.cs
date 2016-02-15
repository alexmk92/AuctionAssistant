using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P99Auctions.Client.Interfaces
{
    /// <summary>
    /// A view for a window wishing to provider "About" info for the assistant
    /// </summary>
    public interface IAboutView
    {
        /// <summary>
        /// Shows the view as a dialog.
        /// </summary>
        /// <param name="owner">The owner.</param>
        void ShowDialog(IMainView owner);
    }
}
