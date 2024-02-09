using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System;

namespace GoServer
{
    public class ServiceViewModel : INotifyPropertyChanged
    {
        public Service D2DBS { get; set; } = new Service();
        public Service D2CS { get; set; } = new Service();
        public Service D2GS { get; set; } = new Service();
        public Service PVPGN { get; set; } = new Service();
        public Service Store { get; set; } = new Service();

        public ServiceViewModel()
        {
            var settings = AppSettings.LoadSettings("config.json");

            D2DBS.Path = settings.D2DBSPath;
            D2CS.Path = settings.D2CSPath;
            D2GS.Path = settings.D2GSPath;
            PVPGN.Path = settings.PVPGNPath;
            Store.Path = settings.StorePath;
        }
        public string AllServicesStatus
        {
            get
            {
                bool allRunning = D2DBS.IsRunning && D2CS.IsRunning && D2GS.IsRunning && PVPGN.IsRunning;
                bool anyRunning = D2DBS.IsRunning || D2CS.IsRunning || D2GS.IsRunning || PVPGN.IsRunning;

                if (allRunning) return "모든 서비스 실행 중";
                else if (!anyRunning) return "모든 서비스 중지됨";
                else return "일부 서비스 실행 중";
            }
        }

        public void ToggleAllServices()
        {
            bool allRunning = D2DBS.IsRunning && D2CS.IsRunning && D2GS.IsRunning && PVPGN.IsRunning;

            if (allRunning)
            {
                StopAllServices();
            }
            else
            {
                StartAllServices();
            }

            NotifyServiceStatusChanged();
        }

        public void StartService(string serviceName)
        {
            try
            {
                StartServiceInternal(serviceName);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting service {serviceName}: {ex.Message}");
            }

            NotifyServiceStatusChanged();
        }

        private void StartServiceInternal(string serviceName)
        {
            Process process = new Process();
            bool started = false;

            switch (serviceName)
            {
                case "D2DBS":
                    if (!string.IsNullOrEmpty(D2DBS.Path))
                    {
                        //process.StartInfo.FileName = D2DBS.Path;
                        started = true;
                        //started = process.Start();
                        D2DBS.IsRunning = started;
                    }
                    break;
                case "D2CS":
                    if (!string.IsNullOrEmpty(D2CS.Path))
                    {
                        //process.StartInfo.FileName = D2CS.Path;
                        started = true;
                        //started = process.Start();
                        D2CS.IsRunning = started;
                    }
                    break;
                case "D2GS":
                    if (!string.IsNullOrEmpty(D2GS.Path))
                    {
                        //process.StartInfo.FileName = D2GS.Path;
                        started = true;
                        //started = process.Start();
                        D2GS.IsRunning = started;
                    }
                    break;
                case "PVPGN":
                    if (!string.IsNullOrEmpty(PVPGN.Path))
                    {
                        //process.StartInfo.FileName = PVPGN.Path;
                        started = true;
                        //started = process.Start();
                        PVPGN.IsRunning = started;
                    }
                    break;
                case "Store":
                    if (!string.IsNullOrEmpty(Store.Path))
                    {
                        //process.StartInfo.FileName = Store.Path;
                        started = true;
                        //started = process.Start();
                        Store.IsRunning = started;
                    }
                    break;
            }
        }

        public void StopService(string serviceName)
        {
            switch (serviceName)
            {
                case "D2DBS":
                    D2DBS.IsRunning = false;
                    break;
                case "D2CS":
                    D2CS.IsRunning = false;
                    break;
                case "D2GS":
                    D2GS.IsRunning = false;
                    break;
                case "PVPGN":
                    PVPGN.IsRunning = false;
                    break;
                case "Store":
                    Store.IsRunning = false;
                    break;
            }

            NotifyServiceStatusChanged();
        }

        public void StartAllServices()
        {
            try
            {
                StartService("D2DBS");
                StartService("D2CS");
                StartService("D2GS");
                StartService("PVPGN");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting all services: {ex.Message}");
            }

            NotifyServiceStatusChanged();
        }

        public void StopAllServices()
        {
            try
            {
                StopService("D2DBS");
                StopService("D2CS");
                StopService("D2GS");
                StopService("PVPGN");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error stopping all services: {ex.Message}");
            }

            NotifyServiceStatusChanged();
        }

        private void NotifyServiceStatusChanged()
        {
            OnPropertyChanged(nameof(D2DBS.IsRunning));
            OnPropertyChanged(nameof(D2CS.IsRunning));
            OnPropertyChanged(nameof(D2GS.IsRunning));
            OnPropertyChanged(nameof(PVPGN.IsRunning));
            OnPropertyChanged(nameof(AllServicesStatus));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}