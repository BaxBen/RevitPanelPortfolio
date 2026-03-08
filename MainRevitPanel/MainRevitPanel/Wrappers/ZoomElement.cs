using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainRevitPanel.Wrapper
{
    public class ZoomElement : IExternalEventHandler
    {
        private List<int> _listIds {  get; set; }
        public void Execute(UIApplication app)
        {
            if (_listIds != null)
            {
                UIDocument uidoc = app.ActiveUIDocument;
                Document doc = uidoc.Document;
                List<ElementId> listElementIds = _listIds.Select(x => new ElementId(x)).ToList();

                uidoc.Selection.SetElementIds(listElementIds);
                uidoc.ShowElements(listElementIds);
            }
        }
        public void SetParameters(object parameters)
        {
            if (parameters != null)
            {
                List<int> elementIds = parameters as List<int>;
                _listIds = elementIds?.ToList();
            }
        }

        public string GetName()
        {
            return "Zoom Element";
        }
    }
}
