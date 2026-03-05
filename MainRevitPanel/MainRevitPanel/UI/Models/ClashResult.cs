using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainRevitPanel.UI.Models
{
    public class ClashResult
    {
        public int DuctId { get; set; }
        public int PipeId { get; set; }
        public string DuctName { get; set; }
        public string PipeName { get; set; }
        public double Volume { get; set; }
        public XYZ Location { get; set; }
    }
}
