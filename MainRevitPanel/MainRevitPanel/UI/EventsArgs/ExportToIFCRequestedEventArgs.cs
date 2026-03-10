using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainRevitPanel.UI.EventsArgs
{
    public class ExportToIFCRequestedEventArgs:EventArgs
    {
        public string SelectedSettings { get; }
        public bool? SelectedCreateModel { get; }
        public string PathSave { get; }
        public ExportToIFCRequestedEventArgs(string selectedSettings, bool? selectedCreateModel, string pathSave) 
        {
            SelectedSettings = selectedSettings;
            SelectedCreateModel = selectedCreateModel;
            PathSave = pathSave;
        }
    }
}
