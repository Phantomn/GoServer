using System.Windows.Controls;
using System.Windows;
using System;

namespace GoServer
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ServiceViewModel();
        }

        private ServiceViewModel ViewModel => DataContext as ServiceViewModel;

        private void LoadSettings_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "JSON Files (*.json)|*.json|All files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    var settings = AppSettings.LoadSettings(openFileDialog.FileName);
                    var viewModel = DataContext as ServiceViewModel;
                    if (viewModel != null)
                    {
                        viewModel.D2DBS.Path = settings.D2DBSPath;
                        viewModel.D2CS.Path = settings.D2CSPath;
                        viewModel.D2GS.Path = settings.D2GSPath;
                        viewModel.PVPGN.Path = settings.PVPGNPath;
                        viewModel.Store.Path = settings.StorePath;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to load settings: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        private void StartAllServices_Click(object sender, RoutedEventArgs e) => ViewModel?.StartAllServices();

        private void StopAllServices_Click(object sender, RoutedEventArgs e) => ViewModel?.StopAllServices();

        private void ToggleAllServices_Click(object sender, RoutedEventArgs e) => ViewModel?.ToggleAllServices();

        private void ToggleService_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.CommandParameter is string serviceName)
            {
                ViewModel?.ToggleService(serviceName);
                UpdateButtonContent(button, serviceName);
            }
        }

        private void UpdateButtonContent(Button button, string serviceName)
        {
            var isRunning = ViewModel?.IsServiceRunning(serviceName) ?? false;
        }
    }
}