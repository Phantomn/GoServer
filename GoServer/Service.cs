using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;

namespace GoServer
{
    public class Service : INotifyPropertyChanged
    {
        private bool _isRunning;
        private string _path;
        private Process _process;
        public string Path
        {
            get => _path;
            set
            {
                if (_path != value)
                {
                    _path = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsRunning
        {
            get => _isRunning;
            set
            {
                if (_isRunning != value)
                {
                    _isRunning = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Status));
                }
            }
        }

        public Process Process
        {
            get => _process;
            set
            {
                if (_process != value)
                {
                    _process = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Status => IsRunning ? "실행중" : "중지됨";

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Start()
        {
            if (string.IsNullOrEmpty(Path)) return;

            if (_process == null || _process.HasExited)
            {
                try
                {
                    _process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = Path,
                            UseShellExecute = false
                        },
                        EnableRaisingEvents = true
                    };

                    _process.Exited += (sender, e) =>
                    {
                        IsRunning = false;
                    };
                    _process.Start();
                    IsRunning = true;
                }
                catch(Exception ex) {
                    MessageBox.Show($"프로세스 시작에 실패했습니다: {ex.Message}");
                    IsRunning = false;
                }
            }
        }

        public void Stop()
        {
            if (_process != null && !_process.HasExited)
            {
                try
                {
                    // 프로세스에게 종료 신호를 보냅니다. GUI 애플리케이션의 경우 이 메서드가 윈도우를 닫으려고 시도합니다.
                    _process.CloseMainWindow();

                    // 프로세스가 종료될 시간을 주기 위해 잠시 대기합니다.
                    // 이 값은 애플리케이션의 종료 시간에 따라 조정될 수 있습니다.
                    if (!_process.WaitForExit(1000)) // 5초 대기
                    {
                        // 프로세스가 지정된 시간 내에 종료되지 않은 경우 강제로 종료합니다.
                        _process.Kill();
                        _process.WaitForExit(); // Kill 호출 후 프로세스가 완전히 종료될 때까지 대기
                    }
                }
                catch (Exception ex)
                {
                    // 프로세스 종료 시도 중 발생할 수 있는 예외를 처리합니다.
                    Console.WriteLine($"프로세스 종료 중 예외 발생: {ex.Message}");
                }
                finally
                {
                    // 프로세스와 관련된 리소스를 정리합니다.
                    _process.Dispose();
                    _process = null;
                    IsRunning = false;
                }
            }
        }

    }
}
