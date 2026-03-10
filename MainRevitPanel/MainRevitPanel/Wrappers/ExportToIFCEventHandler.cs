using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainRevitPanel.Wrappers
{
    public class ExportToIFCEventHandler : IExternalEventHandler
    {

        public void Execute(UIApplication app)
        {
            Document doc = app.ActiveUIDocument.Document;


        }

        public string GetName()
        {
            return " ";
        }
    }
}
