using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using P99Auctions.Client.Interfaces;

namespace P99Auctions.Client
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window, IAboutView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="About"/> class.
        /// </summary>
        public About()
        {
            this.InitializeComponent();

            txtVersion.Content = string.Format("v{0}",System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString());
        }


        /// <summary>
        /// Shows the view as a dialog.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public void ShowDialog(IMainView owner)
        {
            this.Owner = owner as Window;
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            this.ShowDialog();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
