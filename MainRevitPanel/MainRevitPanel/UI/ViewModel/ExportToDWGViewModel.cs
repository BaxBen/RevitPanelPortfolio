using MainRevitPanel.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MainRevitPanel.UI.ViewModel
{
    public class ExportToDWGViewModel : ViewModelBase
    {
        public List<string> _listFilter;
        public List<string> _listParameters;
        public List<string> ListFilters
        { 
            get=> _listFilter;
            set { SetProperty(ref _listFilter, value); }
        }

        public List<string> ListParameters {
            get=> _listParameters;
            set { SetProperty(ref _listFilter, value); }
        }

        public ExportToDWGViewModel()
        {
            ListFilters = new List<string>();
            ListParameters = new List<string>();

        }

        public void LoadData(List<string> listFilters, List<string> listParameters)
        {

            ListFilters.Clear();
            ListParameters.Clear();

            foreach (var row in listFilters.Where(r => r != null))
            {
                ListFilters.Add(row);
            }

            foreach (var row in listParameters.Where(r => r != null))
            {
                ListParameters.Add(row);
            }
        }
    }
}
