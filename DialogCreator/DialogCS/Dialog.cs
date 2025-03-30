using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DialogCreatorLibrary
{
    public class Dialog
    {
        public Dialog() { DialogRows = new ObservableCollection<DialogRow>();  }
        public string DialogName { get; set; }
        public ObservableCollection<DialogRow> DialogRows { get; set;  }

    }
}
