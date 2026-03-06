using MainRevitPanel.Base;
using MainRevitPanel.Services;
using MainRevitPanel.UI.RelayCommands;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;


public class NearFarCraneViewModel : ViewModelBase
{
    private List<string> _listBoxLeft;
    private List<string> _listBoxRight;
    private string _selectedConnector;
    private readonly IRevitService _revitService;

    public RelayCommand CloseCommand { get; }
    public RelayCommand HighlightCommand { get; }
    public string SelectedElement { get;set; }
    private Window _window { get; set; }
    public List<string> ListBoxLeft
    {
        get => _listBoxLeft;
        set
        {
            _listBoxLeft = value;
            OnPropertyChanged();
        }
    }

    public List<string> ListBoxRight
    {
        get => _listBoxRight;
        set
        {
            _listBoxRight = value;
            OnPropertyChanged();
        }
    }

    public string SelectedConnector
    {
        get => _selectedConnector;
        set
        {
            _selectedConnector = value;
            OnPropertyChanged();
        }
    }

    public NearFarCraneViewModel()
    {
        CloseCommand = new RelayCommand(ExecuteClose, CanExecuteClose);
        HighlightCommand = new RelayCommand(ExecuteHighlight, CanExecuteHighlight);
        ListBoxLeft = new List<string>();
        ListBoxRight = new List<string>();
    }

    private bool CanExecuteClose(object parameter)
    {
        if (_window == null) return false;
        else return true;
    }
    private bool CanExecuteHighlight(object parameter)
    {
        if (_window == null) return false;
        else return true;
    }
    private void ExecuteHighlight(object parameter)
    {
        //ОСтановился здесь!!!!!!!!
        //TaskDialog.Show("Внимание", "Функция в разработке, юудет доступна позже");

        //var idsToSelect = SelectedElement;
        //_revitService.HighlightElements(idsToSelect);
    }
    private void ExecuteClose(object parameter)
    {
        _window.Close();
    }

    public void LoadData(Window window, string selectedElement, List<string> listElementLeft, List<string> listElementRight)
    {
        _window = window;
        SelectedConnector = selectedElement;

        ListBoxLeft.Clear();
        ListBoxRight.Clear();

        foreach (var row in listElementLeft.Where(r => r != null))
        {
            ListBoxLeft.Add(row);
        }

        foreach (var row in listElementRight.Where(r => r != null))
        {
            ListBoxRight.Add(row);
        }
    }
}