using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace GoServer
{
    public class ServiceViewModel : INotifyPropertyChanged
    {
        public Service D2DBS { get; } = new Service();
        public Service D2CS { get; } = new Service();
        public Service D2GS { get; } = new Service();
        public Service PVPGN { get; } = new Service();
        public Service Store { get; } = new Service();

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
                bool allRunning = new[] { D2DBS, D2CS, D2GS, PVPGN }.All(service => service.IsRunning);
                bool anyRunning = new[] { D2DBS, D2CS, D2GS, PVPGN }.Any(service => service.IsRunning);

                if (allRunning) return "모든 서비스 실행 중";
                else if (!anyRunning) return "모든 서비스 중지됨";
                else return "일부 서비스 실행 중";
            }
        }

        public void ToggleAllServices()
        {
            bool allRunning = new[] { D2DBS, D2CS, D2GS, PVPGN }.All(service => service.IsRunning);

            if (allRunning)
                StopAllServices();
            else
                StartAllServices();

            NotifyServiceStatusChanged();
        }

        public void StartService(string serviceName) => UpdateServiceStatus(serviceName, true);

        public void StopService(string serviceName) => UpdateServiceStatus(serviceName, false);

        private void UpdateServiceStatus(string serviceName, bool isRunning)
        {
            try
            {
                var service = GetServiceByName(serviceName);
                if (service != null)
                {
                    service.IsRunning = isRunning;

                    if (isRunning)
                    {
                        switch (serviceName)
                        {
                            case "D2DBS":
                                Process.Start(D2DBS.Path); break;
                            case "D2CS":
                                Process.Start(D2CS.Path); break;
                            case "D2GS":
                                Process.Start(D2GS.Path); break;
                            case "PVPGN":
                                Process.Start(PVPGN.Path); break;
                            case "Store":
                                Process.Start(Store.Path); break;
                        }
                    }
                }
                NotifyServiceStatusChanged();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating service {serviceName}: {ex.Message}");
            }
        }
        public void ToggleService(string serviceName)
        {
            var service = GetServiceByName(serviceName);
            if (service != null)
            {
                if (service.IsRunning)
                    StopService(serviceName);
                else
                    StartService(serviceName);
            }
        }
        public bool IsServiceRunning(string serviceName)
        {
            var service = GetServiceByName(serviceName);
            return service != null && service.IsRunning;
        }
        private Service GetServiceByName(string serviceName)
        {
            switch (serviceName)
            {
                case "D2DBS": return D2DBS;
                case "D2CS": return D2CS;
                case "D2GS": return D2GS;
                case "PVPGN": return PVPGN;
                case "Store": return Store;
                default: return null;
            }
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

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
