using MainRevitPanel.Base;
using MainRevitPanel.UI.EventsArgs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace MainRevitPanel.UI.ViewModel
{
    public class ExportToIFCViewModel: ViewModelBase
    {
        public string _pathSave { get; set; }
        public string PathSave {  
            get => _pathSave;
            set
            {
                _pathSave = value;
                OnPropertyChanged();
            }
        }
        public List<string> Settings { get; set; }
        public string SelectedSettings { get; set; }
        public Window _window { get; set; }
        public Dictionary<string, bool> CreateModel { get; set; }
        public bool SelectedCreateModel { get; set; }
        public string Name { get; set; }
        public event EventHandler<ExportToIFCRequestedEventArgs> ExportToIFCRequested;

        public RelayCommand ClosedCommand { get; }
        public RelayCommand ApplyCommand { get; }
        public RelayCommand PathCommand { get; }

        public ExportToIFCViewModel()
        {
            CreateModel = new Dictionary<string, bool> { ["Да"] = true, ["Нет"] = false };
            ClosedCommand = new RelayCommand(ExecuteClose, CanExecuteClose);
            ApplyCommand = new RelayCommand(ExecuteApply, CanExecuteApply);
            PathCommand = new RelayCommand(ExecutePath, CanExecutePath);
        }
        private bool CanExecuteClose(object parameter)
        {
            return _window != null;
        }
        private void ExecuteClose(object parameter)
        {
            _window?.Close();
        }
        private bool CanExecuteApply(object parameter)
        {
            return (SelectedSettings != null && PathSave!=null);
        }
        private void ExecuteApply(object parameter)
        {
            OnExportToIFCRequested(new ExportToIFCRequestedEventArgs(SelectedSettings, SelectedCreateModel, PathSave));
            _window?.Close();
        }
        private bool CanExecutePath(object parameter)
        {
            return true;
        }
        private void ExecutePath(object parameter)
        {
            var dialog = new System.Windows.Forms.SaveFileDialog
            {
                Title = "Выберите файл",
                Filter = "Файлы IFC (*.ifc)|*.ifc",
                FileName = Name
            };

            if(dialog.ShowDialog() == DialogResult.OK)
            {
                PathSave = dialog.FileName;
            }
        }

        protected virtual void OnExportToIFCRequested(ExportToIFCRequestedEventArgs e)
        {
            ExportToIFCRequested?.Invoke(this, e);
        }

        public void LoadData(Window window, List<string> settings, string pathSave, string name)
        {
            _window = window;
            Settings = settings;
            PathSave = pathSave;
            Name = name;
        }
    }
}
