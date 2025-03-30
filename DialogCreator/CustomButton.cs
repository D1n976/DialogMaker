using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DialogCreator
{
    internal class CustomButton : Button
    {
        public ClothType ClothType;

        public delegate void OnTypeChanged(string ClothTypeName, bool toIncrease);
        public OnTypeChanged OnImageChanged;
        public bool To_increase { get; set; }
        public void OnClick(object sender, EventArgs e)
        {
            OnImageChanged.Invoke(ClothType.Name, To_increase);
        }
    }
}
