using Autodesk.Revit.DB;
using MainRevitPanel.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainRevitPanel.UI.Models
{
    public class SheetData : ViewModelBase
    {
        private bool _isSelected;
        private string _sheetNumber;
        private string _sheetName;
        private ElementId _sheetId;

        public ElementId SheetId
        {
            get => _sheetId;
            set => SetProperty(ref _sheetId, value);
        }

        public string SheetNumber
        {
            get => _sheetNumber;
            set => SetProperty(ref _sheetNumber, value);
        }

        public string SheetName
        {
            get => _sheetName;
            set => SetProperty(ref _sheetName, value);
        }

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
    }
}
