using Autodesk.Revit.DB;
using MainRevitPanel.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MainRevitPanel.UI.ViewModel
{
    public class GipVisionViewModel
    {
        public List<GipVisionModel> Items { get; set; }
        public GipVisionModel SelectedItem { get; set; }
        private Window _window { get; set; }


        public RelayCommand CloseCommand { get; }
        public RelayCommand CopyUrlCommand { get; }
        public RelayCommand CopyPinCommand { get; }

        public GipVisionViewModel() 
        {
            CloseCommand = new RelayCommand(ExecuteClose, CanExecuteClose);
            CopyUrlCommand = new RelayCommand(ExecuteUrl, CanExecuteUrl);
            CopyPinCommand = new RelayCommand(ExecutePin, CanExecutePin);
        }

        private bool CanExecuteClose(object parameter)
        {
            return _window != null;
        }
        private void ExecuteClose(object parameter)
        {
            _window?.Close();
        }


        private bool CanExecuteUrl(object parameter)
        {
            return SelectedItem != null;
        }
        private void ExecuteUrl(object parameter)
        {
            Clipboard.SetText(SelectedItem.URL.ToString());
        }


        private bool CanExecutePin(object parameter)
        {
            return SelectedItem != null;
        }
        private void ExecutePin(object parameter)
        {
            Clipboard.SetText(SelectedItem.PIN_CODE.ToString());
        }

        public void LoadData(Window window, List<GipVisionModel> list)
        {
            _window = window;
            Items = list;
        }
    }
}
