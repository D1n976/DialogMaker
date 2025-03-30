using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Media;

namespace DialogCreator
{
    public class ResizePanel : ContentControl
    {
        public static double DefaultResizeGripHeight = 12.0;

        public static readonly DependencyProperty ResizeGripHeightProperty = DependencyProperty.Register(
            "ResizeGripHeight", typeof(double), typeof(ResizePanel), new PropertyMetadata(DefaultResizeGripHeight));

        public static readonly DependencyProperty ResizeGripBackgroundProperty = DependencyProperty.Register(
            "ResizeGripBackground", typeof(Brush), typeof(ResizePanel), new PropertyMetadata(Brushes.White));

        public static readonly DependencyProperty ResizeGripOnHoverBackgroundProperty = DependencyProperty.Register(
            "ResizeGripOnHoverBackground", typeof(Brush), typeof(ResizePanel), new PropertyMetadata(Brushes.LightGray));

        public static readonly DependencyProperty ResizeGripOnDragBackgroundProperty = DependencyProperty.Register(
            "ResizeGripOnDragBackground", typeof(Brush), typeof(ResizePanel), new PropertyMetadata(Brushes.DarkGray));

        private Thumb _dragHandle;

        static ResizePanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(ResizePanel),
                new FrameworkPropertyMetadata(typeof(ResizePanel)));
        }

        public double ResizeGripHeight
        {
            get => (double)GetValue(ResizeGripHeightProperty);
            set => SetValue(ResizeGripHeightProperty, value);
        }

        public Brush ResizeGripBackground
        {
            get => (Brush)GetValue(ResizeGripBackgroundProperty);
            set => SetValue(ResizeGripBackgroundProperty, value);
        }

        public Brush ResizeGripOnHoverBackground
        {
            get => (Brush)GetValue(ResizeGripOnHoverBackgroundProperty);
            set => SetValue(ResizeGripOnHoverBackgroundProperty, value);
        }

        public Brush ResizeGripOnDragBackground
        {
            get => (Brush)GetValue(ResizeGripOnDragBackgroundProperty);
            set => SetValue(ResizeGripOnDragBackgroundProperty, value);
        }

        public override void OnApplyTemplate()
        {
            _dragHandle = (Thumb)Template.FindName("PART_DragHandle", this);
            _dragHandle.DragDelta += OnDragDelta;
        }

        private void OnDragDelta(object sender, DragDeltaEventArgs e)
        {
            var dy = ActualHeight + e.VerticalChange;

            Height = dy >= _dragHandle.ActualHeight
                ? dy
                : _dragHandle.ActualHeight;
        }
    }
}