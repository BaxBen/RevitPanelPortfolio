using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainRevitPanel.Services
{
    public class RevitService : IRevitService
    {
        private readonly UIDocument _uiDoc;

        public RevitService(UIDocument uiDoc)
        {
            _uiDoc = uiDoc;
        }
        public void HighlightElements(List<ElementId> ids)
        {
            _uiDoc.Selection.SetElementIds(ids);
        }

        public void ShowElements(List<ElementId> ids)
        {
            _uiDoc.ShowElements(ids);
        }
    }
}
