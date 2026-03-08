using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainRevitPanel.UI.EventsArgs
{
    public class ZoomRequestedEventArgs : EventArgs
    {
        public object Parameter { get; }
        public ZoomRequestedEventArgs(object parameter) => Parameter = parameter;
    }
}
