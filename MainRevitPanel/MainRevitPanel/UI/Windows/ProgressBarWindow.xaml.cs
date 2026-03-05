using System;
using System.Windows;
using System.Windows.Controls;

using System.Windows.Threading;

namespace MainRevitPanel.UI.Windows
{
    /// <summary>
    /// Логика взаимодействия для ProgressBarWindow.xaml
    /// </summary>
    public partial class ProgressBarWindow : Window
    {
        private int _currentValue;
        private int _maxValue;
        private DispatcherTimer _timer;

        public int CurrentValue
        {
            get { return _currentValue; }
            set
            {
                _currentValue = value;
                UpdateProgress();
            }
        }

        public int MaxValue
        {
            get { return _maxValue; }
            set
            {
                _maxValue = value;
                UpdateProgress();
            }
        }

        public ProgressBarWindow()
        {
            InitializeComponent();
            this.Title = "Прогресс выполнения";

            _currentValue = 0;
            _maxValue = 100;

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(50);
            _timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                if (_maxValue > 0)
                {
                    double percentage = (_currentValue * 100.0) / _maxValue;
                    ProgressBar.Value = percentage;
                    PercentageText.Text = $"{percentage:F1}%";
                    StatusText.Text = $"Обработано: {_currentValue} из {_maxValue}";
                }
            });
        }

        public void UpdateProgress()
        {
            if (!_timer.IsEnabled)
            {
                _timer.Start();
            }
        }

        public void SetStatus(string status)
        {
            Dispatcher.Invoke(() =>
            {
                StatusText.Text = status;
            });
        }

        protected override void OnClosed(EventArgs e)
        {
            _timer.Stop();
            base.OnClosed(e);
        }
    }
}
