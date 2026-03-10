using MainRevitPanel.Base;
using MainRevitPanel.UI;
using MainRevitPanel.UI.EventsArgs;
using MainRevitPanel.UI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;


public class NearFarCraneViewModel : ViewModelBase
{
    public RelayCommand CloseCommand { get; }
    public RelayCommand HighlightCommand { get; }
    private Window _window { get; set; }
    public List<SystemMEP> Items { get; set; }
    public SystemMEP SelectedItem { get; set; }

    public event EventHandler<ZoomRequestedEventArgs> ZoomRequested;


    public NearFarCraneViewModel()
    {
        CloseCommand = new RelayCommand(ExecuteClose, CanExecuteClose);
        HighlightCommand = new RelayCommand(ExecuteHighlight, CanExecuteHighlight);
    }

    private bool CanExecuteClose(object parameter)
    {
        return _window != null;
    }
    private void ExecuteClose(object parameter)
    {
        _window?.Close();
    }

    private bool CanExecuteHighlight(object parameter)
    {
        return SelectedItem != null;
    }

    private void ExecuteHighlight(object parameter)
    {
        OnZoomRequested(new ZoomRequestedEventArgs(SelectedItem.ListPipe));
    }

    protected virtual void OnZoomRequested(ZoomRequestedEventArgs e)
    {
        ZoomRequested?.Invoke(this, e);
    }

    public void LoadData(Window window, List<SystemMEP> listReturn)
    {
        _window = window;
        Items = listReturn;

    }
}