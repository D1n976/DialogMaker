using com.sun.org.apache.xml.@internal.resolver.helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DialogCreator
{
    public enum Windows { MainWindow, PreviewWindow, ImportCharactersWindow, ImportPrincessWindow, EditDialogClothesWindow };

    public class CustomWindow : Window
    {
        public CustomWindow()
        {
            DefaultStyleKey = typeof(CustomWindow);
            CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, CloseWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand, MaximizeWindow, CanResizeWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, MinimizeWindow, CanMinimizeWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand, RestoreWindow, CanResizeWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.ShowSystemMenuCommand, ShowSystemMenu));

        }

        public void LoadWindowSettings(Windows window)
        {
            switch (window)
            {
                case Windows.MainWindow:
                    LoadWindowSettings(Properties.Settings.Default.MainWindow);
                    break;
                case Windows.ImportPrincessWindow:
                    LoadWindowSettings(Properties.Settings.Default.ImportPrincessWindow);
                    break;
                case Windows.PreviewWindow:
                    LoadWindowSettings(Properties.Settings.Default.PreviewWindow);
                    break;
                case Windows.ImportCharactersWindow:
                    LoadWindowSettings(Properties.Settings.Default.ImportCharactersWindow);
                    break;
                case Windows.EditDialogClothesWindow:
                    LoadWindowSettings(Properties.Settings.Default.EditDialogClothesWindow);
                    break;
            }
        }
        private void LoadWindowSettings(Rect windowProperty)
        {
            if(windowProperty.IsEmpty)
                return;

            //Restore window size and positions
            // Восстанавливаем позицию на экране.
            Left = windowProperty.Left;
            Top = windowProperty.Top;

            // Востанавливаем размеры окна.

            if (windowProperty.Width != 0)
                Width = windowProperty.Width;
            if (windowProperty.Height != 0)
                Height = windowProperty.Height;
        }
        public void SaveWindowSettings(Windows window, Rect rect)
        {
            // RestoreBounds - Возвращает размер и расположение окна перед тем как оно было свернуто или развернуто.
            switch (window)
            {
                case Windows.MainWindow:
                    Properties.Settings.Default.MainWindow = rect;
                    break;
                case Windows.ImportPrincessWindow:
                    Properties.Settings.Default.ImportPrincessWindow = rect;
                    break;
                case Windows.PreviewWindow:
                    Properties.Settings.Default.PreviewWindow = rect;
                    break;
                case Windows.ImportCharactersWindow:
                    Properties.Settings.Default.ImportCharactersWindow = rect;
                    break;
                case Windows.EditDialogClothesWindow:
                    Properties.Settings.Default.EditDialogClothesWindow = rect;
                    break;
            };
        }

        public event Action CloseWindowEvent;

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            if (SizeToContent == SizeToContent.WidthAndHeight)
                InvalidateMeasure();
        }

        private void CanResizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ResizeMode == ResizeMode.CanResize || ResizeMode == ResizeMode.CanResizeWithGrip;
        }

        private void CanMinimizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ResizeMode != ResizeMode.NoResize;
        }

        private void CloseWindow(object sender, ExecutedRoutedEventArgs e)
        {
            if (null != CloseWindowEvent)
            {
                CloseWindowEvent();
            }

            this.Close();
        }

        private void MaximizeWindow(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MaximizeWindow(this);
        }

        private void MinimizeWindow(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        private void RestoreWindow(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.RestoreWindow(this);
        }

        private void ShowSystemMenu(object sender, ExecutedRoutedEventArgs e)
        {
            var element = e.OriginalSource as FrameworkElement;
            if (element == null)
                return;

            var point = WindowState == WindowState.Maximized ? new Point(0, element.ActualHeight)
                : new Point(Left + BorderThickness.Left, element.ActualHeight + Top + BorderThickness.Top);
            point = element.TransformToAncestor(this).Transform(point);
            SystemCommands.ShowSystemMenu(this, point);
        }
    }
}
