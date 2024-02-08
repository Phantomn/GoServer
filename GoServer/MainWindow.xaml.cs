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

        private void LoadSettings_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "JSON Files (*.json)|*.json|All files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (openFileDialog.ShowDialog() == true)
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

                    // ViewModel의 다른 프로퍼티를 필요에 따라 업데이트...
                }
            }
        }

        private void StartAllServices_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as ServiceViewModel)?.StartAllServices();
        }

        private void StopAllServices_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as ServiceViewModel)?.StopAllServices();
        }

        private void ToggleAllServices_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as ServiceViewModel)?.ToggleAllServices();
        }

        private void StartService_Click(object sender, RoutedEventArgs e)
        {
            var serviceName = (sender as Button)?.Content.ToString();
            (DataContext as ServiceViewModel)?.StartService(serviceName);
        }

        private void StopService_Click(object sender, RoutedEventArgs e)
        {
            var serviceName = (sender as Button)?.Content.ToString();
            (DataContext as ServiceViewModel)?.StopService(serviceName);
        }
    }
}