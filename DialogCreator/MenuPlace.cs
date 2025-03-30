using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Windows.Controls;

namespace DialogCreator
{
    class MenuPlace
    {
        public static PlacementMode GetPlacement(DependencyObject obj) { return (PlacementMode)obj.GetValue(PlacementProperty); }
        public static void SetPlacement(DependencyObject obj, PlacementMode value) { obj.SetValue(PlacementProperty, value); }
        public static readonly DependencyProperty PlacementProperty = DependencyProperty.RegisterAttached("Placement", typeof(PlacementMode), typeof(MenuPlace), new PropertyMetadata(PlacementMode.Bottom, changed));
        private static void changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MenuItem menu)
                menu.Loaded += delegate
                {
                    Popup popup = menu.Template.FindName("PART_Popup", menu) as Popup;
                    popup.Placement = (PlacementMode)e.NewValue;
                };
        }
    }
}
