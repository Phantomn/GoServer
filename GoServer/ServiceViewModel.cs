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
            bool anyServiceRunning = new[] { D2DBS, D2CS, D2GS, PVPGN }.Any(service => service.IsRunning);

            if (anyServiceRunning)
            {
                // 하나 이상의 서비스가 실행 중이면 모든 서비스 중지
                StopAllServices();
            }
            else
            {
                // 모든 서비스가 중지 상태면 모든 서비스 시작
                StartAllServices();
            }
        }


        public void ToggleService(string serviceName)
        {
            var service = GetServiceByName(serviceName);
            if (service != null)
            {
                if (service.IsRunning)
                    service.Stop();
                else
                    service.Start();

                NotifyServiceStatusChanged();
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
            foreach (var service in new[] { D2DBS, D2CS, D2GS, PVPGN })
            {
                service.Start();
            }
            NotifyServiceStatusChanged();
        }

        public void StopAllServices()
        {
            foreach (var service in new[] { D2DBS, D2CS, D2GS, PVPGN })
            {
                service.Stop();
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
