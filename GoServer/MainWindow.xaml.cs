
using System.ComponentModel;
using Newtonsoft.Json;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System;
using System.Diagnostics;

public class AppSettings
{
    public string D2DBSPath { get; set; }
    public string D2CSPath { get; set; }
    public string D2GSPath { get; set; }
    public string PVPGNPath { get; set; }
    public string StorePath { get; set; }

    public static AppSettings LoadSettings(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return new AppSettings();
        }

        string json = File.ReadAllText(filePath);
        return JsonConvert.DeserializeObject<AppSettings>(json);
    }
}
namespace YourApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ServiceViewModel();
        }

        private void ToggleAllServices_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as ServiceViewModel;
            viewModel?.ToggleAllServices();
        }
        private void StartAllServices_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as ServiceViewModel;
            viewModel?.StartAllServices();
        }
        private void StartService_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var serviceName = button?.Content.ToString();
            (DataContext as ServiceViewModel)?.StartService(serviceName);
        }

        private void StopService_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var serviceName = button?.Content.ToString();
            (DataContext as ServiceViewModel)?.StopService(serviceName);
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
                    viewModel.D2DBSPath = settings.D2DBSPath;
                    viewModel.D2CSPath = settings.D2CSPath;
                    viewModel.D2GSPath = settings.D2GSPath;
                    viewModel.PVPGNPath = settings.PVPGNPath;
                    viewModel.StorePath = settings.StorePath;

                    // ViewModel의 다른 프로퍼티를 필요에 따라 업데이트...
                }
            }
        }
    }
}

public class ServiceViewModel : INotifyPropertyChanged
{
    private bool _isD2DBSRunning;
    private bool _isD2CSRunning;
    private bool _isD2GSRunning;
    private bool _isPVPGNRunning;
    private bool _isStoreRunning;

    private string _d2DBSPath;
    private string _d2CSPath;
    private string _d2GSPath;
    private string _pvpgnPath;
    private string _storePath;

    public string D2DBSPath
    {
        get => _d2DBSPath;
        set { _d2DBSPath = value; OnPropertyChanged(); }
    }

    public string D2CSPath
    {
        get => _d2CSPath;
        set { _d2CSPath = value; OnPropertyChanged(); }
    }

    public string D2GSPath
    {
        get => _d2GSPath;
        set { _d2GSPath = value; OnPropertyChanged(); }
    }
    public string PVPGNPath
    {
        get => _pvpgnPath;
        set { _pvpgnPath = value; OnPropertyChanged(); }
    }

    public string StorePath
    {
        get => _storePath;
        set { _storePath = value; OnPropertyChanged(); }
    }

    public bool IsD2DBSRunning
    {
        get => _isD2DBSRunning;
        set
        {
            _isD2DBSRunning = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(D2DBSStatus));
            OnPropertyChanged(nameof(AllServicesStatus));
        }
    }

    public bool IsD2CSRunning
    {
        get => _isD2CSRunning;
        set
        {
            _isD2CSRunning = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(D2CSStatus));
            OnPropertyChanged(nameof(AllServicesStatus));
        }
    }

    public bool IsD2GSRunning
    {
        get => _isD2GSRunning;
        set
        {
            _isD2GSRunning = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(D2GSStatus));
            OnPropertyChanged(nameof(AllServicesStatus));
        }
    }

    public bool IsPVPGNRunning
    {
        get => _isPVPGNRunning;
        set
        {
            _isPVPGNRunning = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(PVPGNStatus));
            OnPropertyChanged(nameof(AllServicesStatus));
        }
    }

    public bool IsStoreRunning
    {
        get => _isStoreRunning;
        set
        {
            _isStoreRunning = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(StoreStatus));
        }
    }

    public string D2DBSStatus => IsD2DBSRunning ? "실행중" : "중지됨";
    public string D2CSStatus => IsD2CSRunning ? "실행중" : "중지됨";
    public string D2GSStatus => IsD2GSRunning ? "실행중" : "중지됨";
    public string PVPGNStatus => IsPVPGNRunning ? "실행중" : "중지됨";
    public string StoreStatus => IsStoreRunning ? "실행중" : "중지됨";

    public string AllServicesStatus
    {
        get
        {
            bool allRunning = IsD2DBSRunning && IsD2CSRunning && IsD2GSRunning && IsPVPGNRunning;
            bool anyRunning = IsD2DBSRunning || IsD2CSRunning || IsD2GSRunning || IsPVPGNRunning;

            if (allRunning) return "모든 서비스 실행 중";
            else if (!anyRunning) return "모든 서비스 중지됨";
            else return "일부 서비스 실행 중";
        }
    }
    public void ToggleAllServices()
    {
        // 모든 서비스가 실행 중인지 확인합니다.
        bool allRunning = IsD2DBSRunning && IsD2CSRunning && IsD2GSRunning && IsPVPGNRunning;

        if (allRunning)
        {
            StopAllServices();

        }
        else
        {
            // 모든 서비스의 상태를 반대로 설정합니다.
            StartAllServices();
        }

        // 상태 변경 알림을 발생시킵니다.
        OnPropertyChanged(nameof(IsD2DBSRunning));
        OnPropertyChanged(nameof(IsD2CSRunning));
        OnPropertyChanged(nameof(IsD2GSRunning));
        OnPropertyChanged(nameof(IsPVPGNRunning));
        OnPropertyChanged(nameof(AllServicesStatus));
    }
    public void StartService(string serviceName)
    {
        Process process = new Process();
        bool started = false;

        try
        {
            switch (serviceName)
            {
                case "D2DBS":
                    if (!string.IsNullOrEmpty(D2DBSPath))
                    {
                        process.StartInfo.FileName = D2DBSPath;
                        started = process.Start();
                        IsD2DBSRunning = started;
                    }
                    break;
                case "D2CS":
                    if (!string.IsNullOrEmpty(D2CSPath))
                    {
                        process.StartInfo.FileName = D2CSPath;
                        started = process.Start();
                        IsD2CSRunning = started;
                    }
                    break;
                case "D2GS":
                    if (!string.IsNullOrEmpty(D2GSPath))
                    {
                        process.StartInfo.FileName = D2GSPath;
                        started = process.Start();
                        IsD2GSRunning = started;
                    }
                    break;
                case "PVPGN":
                    if (!string.IsNullOrEmpty(PVPGNPath))
                    {
                        process.StartInfo.FileName = PVPGNPath;
                        started = process.Start();
                        IsPVPGNRunning = started;
                    }
                    break;
                    // 상점 서비스 시작 로직...
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error starting service {serviceName}: {ex.Message}");
        }

        if (started)
        {
            OnPropertyChanged(nameof(IsD2DBSRunning));
            OnPropertyChanged(nameof(IsD2CSRunning));
            OnPropertyChanged(nameof(IsD2GSRunning));
            OnPropertyChanged(nameof(IsPVPGNRunning));
            OnPropertyChanged(nameof(AllServicesStatus));
        }
    }

    public void StopService(string serviceName)
    {
        // 실제 프로세스 중지 로직은 시스템의 프로세스 목록을 검색하여
        // 해당 서비스에 맞는 프로세스를 종료하는 방식으로 구현해야 합니다.
        // 이 예시에서는 간단히 서비스 상태를 false로 설정합니다.

        switch (serviceName)
        {
            case "D2DBS":
                IsD2DBSRunning = false;
                break;
            case "D2CS":
                IsD2CSRunning = false;
                break;
            case "D2GS":
                IsD2GSRunning = false;
                break;
            case "PVPGN":
                IsPVPGNRunning = false;
                break;
                // 상점 서비스 중지 로직...
        }

        OnPropertyChanged(nameof(IsD2DBSRunning));
        OnPropertyChanged(nameof(IsD2CSRunning));
        OnPropertyChanged(nameof(IsD2GSRunning));
        OnPropertyChanged(nameof(IsPVPGNRunning));
        OnPropertyChanged(nameof(AllServicesStatus));
    }
    public void StartAllServices()
    {
        try
        {
            StartService("D2DBS");
            StartService("D2CS");
            StartService("D2GS");
            StartService("PVPGN");
            OnPropertyChanged(nameof(AllServicesStatus));
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error starting all services: {ex.Message}");
        }
    }
    public void StopAllServices()
    {
        try
        {
            StopService("D2DBS");
            StopService("D2CS");
            StopService("D2GS");
            StopService("PVPGN");
            OnPropertyChanged(nameof(AllServicesStatus));
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error starting all services: {ex.Message}");
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

