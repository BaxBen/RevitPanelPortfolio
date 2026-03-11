using Autodesk.Revit.DB;
using MainRevitPanel.Base;
using MainRevitPanel.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MainRevitPanel.UI.ViewModel
{
    public class AddKeyGipVisionViewModel : ViewModelBase
    {
        private string _textInfo;
        public string TextInfo
        {
            get => _textInfo;
            set
            {
                _textInfo = value;
                OnPropertyChanged();
            }
        }
        public string SelectedText { get; set; }
        public RelayCommand CheckKeyCommand { get; }
        public Window _window {  get; set; }
        public GipVision _gip {  get; set; }

        public AddKeyGipVisionViewModel()
        {
            CheckKeyCommand = new RelayCommand(ExecuteCheckKey, CanExecuteCheckKey);
            TextInfo = "вставьте ключ от GIP VISION";
        }
        private bool CanExecuteCheckKey(object parameter)
        {
            return true;
        }
        private void ExecuteCheckKey(object parameter)
        {
            if (_gip.CheckAPIKey(SelectedText) == 200)
            {
                _gip.apiToken = SelectedText;
                _window.Close();
            }
            else
            {
                TextInfo = "Некорректный ключ";
            }
        }

        public void LoadData(Window window, GipVision gip)
        {
            _window = window;
            _gip = gip;
        }
    }
}
