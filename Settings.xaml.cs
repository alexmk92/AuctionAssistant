// *************************************************************
// P99 Auctions Community Team
// -----------
// Solution:     Project 1999 Auction Tracker
// File:         Settings.xaml.cs
// Email:        info@p99auctions.com
// Website:      www.p99auctions.com
// Last Update:  01/25/2016 10:02 PM
// *************************************************************

using System.Windows;
using System.Windows.Forms;
using P99Auctions.Client.Interfaces;
using MessageBox = System.Windows.MessageBox;

namespace P99Auctions.Client
{
    /// <summary>
    ///     Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window, ISettingsView
    {
        bool _blnIsDialog = false;

        public Settings()
        {
            this.InitializeComponent();
        }


        private void BtnOK_OnClick(object sender, RoutedEventArgs e)
        {
            var settings = this.DataContext as IClientSettings;
            if (settings == null)
            {
                MessageBox.Show("Invalid or corrupted user settings detected. Please close the program and try again.");
                if (_blnIsDialog)
                    this.DialogResult = false;
                this.Close();
                return;
            }

            var errors = settings.RetreiveValidationErrors();
            if (errors.Count == 0)
            {
                if (_blnIsDialog)
                    this.DialogResult = true;
                this.Close();
                return;
            }

            MessageBox.Show(string.Join("\r\n", errors), "An Error Occured", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void btnFolderSelect_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            if (string.IsNullOrWhiteSpace(this.ClientSettings.EQFolder))
                dialog.SelectedPath = this.ClientSettings.EQFolder;
            var result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
                this.ClientSettings.EQFolder = dialog.SelectedPath;
        }

        private void BtnCancel_OnClick(object sender, RoutedEventArgs e)
        {
            if (!_blnIsDialog)
                this.Close();
        }

        public bool? ShowDialog(IMainView owner)
        {
            _blnIsDialog = true;
            this.Owner = owner as Window;
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            return this.ShowDialog();
        }

        public void Show(bool showInTaskbar)
        {
            this.ShowInTaskbar = showInTaskbar;
            this.Show();
        }

        public void Setup(IClientSettings settings)
        {
            this.DataContext = settings.Clone();
        }

        public IClientSettings ClientSettings
        {
            get { return this.DataContext as IClientSettings; }
        }
    }
}