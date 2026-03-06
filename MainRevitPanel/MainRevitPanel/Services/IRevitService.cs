using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainRevitPanel.Services
{
    public interface IRevitService
    {
        void HighlightElements(List<ElementId> ids);
        void ShowElements(List<ElementId> ids);
    }
}
