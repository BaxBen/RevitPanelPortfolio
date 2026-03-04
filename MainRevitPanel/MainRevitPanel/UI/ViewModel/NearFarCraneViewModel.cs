using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;


public class NearFarCraneViewModel : INotifyPropertyChanged
{
    private List<string> _listBoxLeft;
    private List<string> _listBoxRight;
    private string _selectedConnector;

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
        ListBoxLeft = new List<string>();
        ListBoxRight = new List<string>();
    }

    public void LoadData(string selectedElement, List<string> listElementLeft, List<string> listElementRight)
    {
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

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}