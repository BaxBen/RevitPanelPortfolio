using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainRevitPanel.UI.EventsArgs
{
    public class ApplRequestedEventArgs : EventArgs
    {
        public bool Parameter { get; }
        public List<bool> Position { get; }
        public ApplRequestedEventArgs(List<object> parameter)
        {
            Parameter = (bool)parameter.First();
            Position = (List<bool>)parameter.Last();
        }
    }
}
