using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainRevitPanel.UI.Models
{
    public class SystemMEP
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string StringPipe { get; set; }
        public List<int> ListPipe { get; set; }
        public double Length { get; set; }
    }
}
