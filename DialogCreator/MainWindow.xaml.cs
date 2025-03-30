using com.sun.istack.@internal.localization;
using DialogCreatorLibrary;
using java.nio.channels;
using javax.swing.border;
using Microsoft.Win32;
using Newtonsoft.Json;
using org.omg.CORBA;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace DialogCreator
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : CustomWindow
    {
        public DialogCreatorViewModel DialogCreatorViewModel_ { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            DialogCreatorViewModel_ = new DialogCreatorViewModel();
            DataContext = DialogCreatorViewModel_;
            defaultRowsCheckBox.IsChecked = true;
            rowNameTextBlock.IsEnabled = false;

            var menuDropAlignmentField = typeof(SystemParameters).GetField("_menuDropAlignment", BindingFlags.NonPublic | BindingFlags.Static);
            Action setAlignmentValue = () =>
            {
                if (SystemParameters.MenuDropAlignment && menuDropAlignmentField != null) menuDropAlignmentField.SetValue(null, false);
            };
            setAlignmentValue();

            SystemParameters.StaticPropertyChanged += (sender, e) => { setAlignmentValue(); };

            LoadWindowSettings(Windows.MainWindow);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //create folders
            try
            {
                Paths.CreateFolder(System.IO.Path.Combine(Directory.GetCurrentDirectory(), Paths.DataPath));
                Paths.CreateFolder(System.IO.Path.Combine(Directory.GetCurrentDirectory(), Paths.DialogPath));
                Paths.CreateFolder(System.IO.Path.Combine(Directory.GetCurrentDirectory(), Paths.ImagePath));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating directory: {ex.Message}");
            }
            LoadDialogs();

            //Load character Controller from file
            DialogCreatorViewModel_.CharacterController = CharacterController.Deserialize();
        }

        private void AddDialog(object sender, RoutedEventArgs e) => DialogCreatorViewModel_.AddDialog("");
        private void RemoveDialog(object sender, RoutedEventArgs e)
        {
            var dialogResult = System.Windows.MessageBox.Show("Do u want delete this dialog?\nU cant return it after save", "Sure", MessageBoxButton.YesNo);
            if (dialogResult == MessageBoxResult.Yes)
            {
                DialogCreatorViewModel_.RemoveDialog((Dialog)dialogsDataGrid.SelectedItem);
                RemoveOldFilesOfDialogs();
            }
        }

        private void ChooseDialog(object sender, RoutedEventArgs e) => DialogCreatorViewModel_.ChooseDialog((Dialog)dialogsDataGrid.SelectedItem);
        private void AddRow(object sender, RoutedEventArgs e)
        {
            rowsDataGridd.SelectedItem = DialogCreatorViewModel_.AddRow(defaultRowsCheckBox.IsChecked);
            DialogCreatorViewModel_.ChooseRow((DialogRow)rowsDataGridd.SelectedItem);
        }

        private void RemoveRow(object sender, RoutedEventArgs e)
        {
            rowsDataGridd.SelectedItem = DialogCreatorViewModel_.RemoveRow((DialogRow)rowsDataGridd.SelectedItem);
            DialogCreatorViewModel_.ChooseRow((DialogRow)rowsDataGridd.SelectedItem);
        }

        private void ChooseRow(object sender, SelectionChangedEventArgs e)
        {
            DialogCreatorViewModel_.ChooseRow((DialogRow)rowsDataGridd.SelectedItem);
        }

        private void DublicateRow(object sender, RoutedEventArgs e) => DialogCreatorViewModel_.DublicateRow((DialogRow)rowsDataGridd.SelectedItem);

        DialogRow copiedDialogRow = null;
        private void InsertRow(object sender, RoutedEventArgs e)
        {
            rowsDataGridd.SelectedItem = DialogCreatorViewModel_.InsertNewRow((DialogRow)rowsDataGridd.SelectedItem, defaultRowsCheckBox.IsChecked);
            DialogCreatorViewModel_.ChooseRow((DialogRow)rowsDataGridd.SelectedItem);
        }

        private void CopyRow(object sender, RoutedEventArgs e) => copiedDialogRow = DialogCreatorViewModel_.CurrentDialogRow;
        private void PasteRow(object sender, RoutedEventArgs e)
        {
            rowsDataGridd.SelectedItem = DialogCreatorViewModel_.InsertRow(copiedDialogRow, (DialogRow)rowsDataGridd.SelectedItem);
            DialogCreatorViewModel_.ChooseRow((DialogRow)rowsDataGridd.SelectedItem);
        }

        private void CopyRowExecute(object sender, ExecutedRoutedEventArgs e) => copiedDialogRow = DialogCreatorViewModel_.CurrentDialogRow;
        private void PasteRowExecute(object sender, ExecutedRoutedEventArgs e)
        {
            rowsDataGridd.SelectedItem = DialogCreatorViewModel_.InsertRow(copiedDialogRow, (DialogRow)rowsDataGridd.SelectedItem);
            DialogCreatorViewModel_.ChooseRow((DialogRow)rowsDataGridd.SelectedItem);
        }

        private void CutRow_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            rowsDataGridd.SelectedItem = DialogCreatorViewModel_.RemoveRow((DialogRow)rowsDataGridd.SelectedItem);
            DialogCreatorViewModel_.ChooseRow((DialogRow)rowsDataGridd.SelectedItem);
        }

        private void DeleteRow_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            rowsDataGridd.SelectedItem = DialogCreatorViewModel_.RemoveRow((DialogRow)rowsDataGridd.SelectedItem);
            DialogCreatorViewModel_.ChooseRow((DialogRow)rowsDataGridd.SelectedItem);
        }

        private async void SaveDialogs_Executed(object sender, ExecutedRoutedEventArgs e) => await SaveDialogs();
        private void AddRow_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            rowsDataGridd.SelectedItem = DialogCreatorViewModel_.AddRow(defaultRowsCheckBox.IsChecked);
            DialogCreatorViewModel_.ChooseRow((DialogRow)rowsDataGridd.SelectedItem);
        }

        private void RowsDataGridd_CopyingRowClipboardContent(object sender, DataGridRowClipboardEventArgs e) => copiedDialogRow = DialogCreatorViewModel_.CurrentDialogRow;
        private async void SaveDialogs(object sender, RoutedEventArgs e) => await SaveDialogs();
        private async Task SaveDialogs()
        {
            for (int i = 0; i < DialogCreatorViewModel_.Dialogs.Count; i++)
            {
                string filePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), $"{Paths.DialogPath}\\{DialogCreatorViewModel_.Dialogs[i].DialogName}");

                if (File.Exists(filePath))
                    File.Delete(filePath);
                await SerializeDialog(DialogCreatorViewModel_.Dialogs[i], filePath);
            }
            RemoveOldFilesOfDialogs();
        }
        private async Task SerializeDialog(Dialog dialog, string file_path)
        {
            using (FileStream fs = new FileStream(file_path, FileMode.OpenOrCreate))
            {
                string js_str = JsonConvert.SerializeObject(dialog.DialogRows);
                var buffer = Encoding.UTF8.GetBytes(js_str);
                await fs.WriteAsync(buffer, 0, buffer.Length);
            }

        }
        private void RemoveOldFilesOfDialogs()
        {
            List<string> filePaths = Directory.GetFiles(System.IO.Path.Combine(Directory.GetCurrentDirectory(), Paths.DialogPath)).ToList();
            foreach (var filePath in filePaths)
            {
                if (!DialogCreatorViewModel_.Dialogs.Any(x => x.DialogName == System.IO.Path.GetFileName(filePath)))
                    File.Delete(filePath);
            }
        }
        private void LoadDialogs()
        {
            List<string> fileNames = Directory.GetFiles(System.IO.Path.Combine(Directory.GetCurrentDirectory(), Paths.DialogPath)).ToList();
            foreach (string file in fileNames)
            {
                string js_str = File.ReadAllText(file);
                try
                {
                    List<DialogRow> rows = JsonConvert.DeserializeObject<List<DialogRow>>(js_str);
                    string dialogName = System.IO.Path.GetFileName(file);
                    DialogCreatorViewModel_.Dialogs.Add(new Dialog() { DialogName = dialogName, DialogRows = new ObservableCollection<DialogRow>(rows) });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating directory: {ex.Message}");
                }
            }
        }
        private void OpenImportPrincessWindow_(object sender, RoutedEventArgs e)
        {
            OpenImportPrincessWindow openImportPrincessWindow = new OpenImportPrincessWindow(DialogCreatorViewModel_.CharacterController);
            openImportPrincessWindow.ShowDialog();
        }
        private void changeRowAddState(object sender, RoutedEventArgs e)
        {
            if (defaultRowsCheckBox.IsChecked == true)
                rowNameTextBlock.IsEnabled = false;
            else
                rowNameTextBlock.IsEnabled = true;
        }

        private void ShowPredictWindow(object sender, RoutedEventArgs e)
        {
            if (DialogCreatorViewModel_.CurrentDialogRows == null || DialogCreatorViewModel_.CurrentDialogRows.Count == 0)
                return;

            PreviewDialogWindow previewDialogWindow = new PreviewDialogWindow(rowsDataGridd.SelectedIndex, DialogCreatorViewModel_);
            previewDialogWindow.ShowDialog();
        }
        private async void Export(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //Create new folders for export
                try
                {
                    Paths.CreateFolder(System.IO.Path.Combine(fbd.SelectedPath, Paths.DataPath));
                    Paths.CreateFolder(System.IO.Path.Combine(fbd.SelectedPath, Paths.DialogPath));

                    Paths.CreateFolder(System.IO.Path.Combine(fbd.SelectedPath, Paths.ImagePath));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating directory: {ex.Message}");
                }
                //get images from path
                //copy images
                foreach (var fromFilePath in Directory.GetFiles(System.IO.Path.Combine(Directory.GetCurrentDirectory(), Paths.ImagePath)))
                {
                    //copy image
                    string to_path = System.IO.Path.Combine(fbd.SelectedPath, $"{Paths.ImagePath}\\{System.IO.Path.GetFileName(fromFilePath)}");
                    if (!File.Exists(to_path))
                    {
                        File.Copy(fromFilePath, to_path);
                    }
                }

                List<string> SelectedPathDialogs = new List<string>();

                foreach(var d in System.IO.Directory.GetFiles(System.IO.Path.Combine(fbd.SelectedPath, $"{Paths.DialogPath}")))
                    System.IO.Path.GetFileName(d);

                foreach (var dialog in DialogCreatorViewModel_.Dialogs)
                {
                    DialogRow[] CopyDialogRows = new DialogRow[dialog.DialogRows.Count];
                    dialog.DialogRows.ToList().CopyTo(CopyDialogRows);

                    Dialog copyDialog = new Dialog
                    {
                        DialogName = dialog.DialogName,
                        DialogRows = new ObservableCollection<DialogRow>(CopyDialogRows),
                    };
                    if (SelectedPathDialogs.Contains(dialog.DialogName))
                        File.Delete(System.IO.Path.Combine(fbd.SelectedPath, $"{Paths.DialogPath}\\{dialog.DialogName}"));

                    await SerializeDialog(copyDialog, System.IO.Path.ChangeExtension(System.IO.Path.Combine(fbd.SelectedPath, $"{Paths.DialogPath}\\{dialog.DialogName}"), ".json"));
                }
            }
            foreach (var fromFilePath in Directory.GetFiles(System.IO.Path.Combine(Directory.GetCurrentDirectory(), Paths.ImagePath)))
            {
                //copy image
                string to_path = System.IO.Path.Combine(fbd.SelectedPath, $"{Paths.ImagePath}\\{System.IO.Path.GetFileName(fromFilePath)}");
                if (!File.Exists(to_path))
                {
                    File.Copy(fromFilePath, to_path);
                }
            }
        }
        private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            await SaveDialogs();

            SaveWindowSettings(Windows.MainWindow, this.RestoreBounds);
            // Сохранение настроек.
            Properties.Settings.Default.Save();
        }

        private void OnRowChanged(object sender, TextChangedEventArgs e)
        {
            if (DialogCreatorViewModel_ != null)
            {
                int oldIndex = rowsDataGridd.SelectedIndex;
            }
        }

        int dragIndex = -1;
        int dropIndex = -1;
        private void MouseDrag(object sender, System.Windows.Input.MouseEventArgs e)
        {
            TextBlock cellUnderMouse = sender as TextBlock;
            if (cellUnderMouse != null && e.LeftButton == MouseButtonState.Pressed)
            {
                dragIndex = FindRowInTable(cellUnderMouse);
                DragDrop.DoDragDrop(cellUnderMouse, rowsDataGridd, System.Windows.DragDropEffects.Move);
            }
            else if (cellUnderMouse != null && e.LeftButton == MouseButtonState.Released)
            {
                dropIndex = FindRowInTable(cellUnderMouse);

                ObservableCollection<DialogRow> productCollection = DialogCreatorViewModel_.CurrentDialogRows;
                if (dragIndex < 0 || dragIndex >= productCollection.Count)
                    return;

                DialogRow changedProduct = productCollection[dragIndex];
                productCollection.RemoveAt(dragIndex);
                productCollection.Insert(dropIndex, changedProduct);
                dragIndex = -1;
                dropIndex = -1;
                DialogCreatorViewModel_.RefrashDialogNames();
            };
        }
        private int FindRowInTable(TextBlock textCell)
        {
            var asGridRow = textCell.BindingGroup.Owner as DataGridRow;
            if (asGridRow != null)
                return asGridRow.GetIndex();
            return -1;
        }

        private void rowNameTextBlock_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var textbox = (sender as System.Windows.Controls.TextBox);
            if (textbox != null && DialogCreatorViewModel_.CurrentDialogRow != null)
                DialogCreatorViewModel_.CurrentDialogRow.RowName = textbox.Text;
        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => DialogCreatorViewModel_.ChangeDialogCulture(CulturesComboBox.SelectedIndex);

        private void ChangeLanguageUp(object sender, ExecutedRoutedEventArgs e)
        {
            if (CulturesComboBox.SelectedIndex - 1 >= 0 && CulturesComboBox.SelectedIndex - 1 < CulturesComboBox.Items.Count)
            {
                CulturesComboBox.SelectedIndex--;
                DialogCreatorViewModel_.ChangeDialogCulture(CulturesComboBox.SelectedIndex);
            }
        }

        private void ChangeLanguageDown(object sender, ExecutedRoutedEventArgs e)
        {
            if (CulturesComboBox.SelectedIndex + 1 >= 0 && CulturesComboBox.SelectedIndex + 1 < CulturesComboBox.Items.Count)
            {
                CulturesComboBox.SelectedIndex++;
                DialogCreatorViewModel_.ChangeDialogCulture(CulturesComboBox.SelectedIndex);
            }
        }

        private void TextBox_KeyDown_2(object sender, TextChangedEventArgs e)
        {
            var textbox = (sender as System.Windows.Controls.TextBox);
            if (textbox != null && DialogCreatorViewModel_.CurrentDialogRow != null)
                DialogCreatorViewModel_.CurrentDialogRow.LocalizadText.CharacterName = textbox.Text;

        }

        private void TextBox_KeyDown_1(object sender, TextChangedEventArgs e)
        {
            var textbox = (sender as System.Windows.Controls.TextBox);
            if (textbox != null && DialogCreatorViewModel_.CurrentDialogRow != null)
            {
                DialogCreatorViewModel_.CurrentDialogRow.CommandEventsString = textbox.Text;
            }
        }

        private void TextBox_KeyDown(object sender, TextChangedEventArgs e)
        {
            var textbox = (sender as System.Windows.Controls.TextBox);
            if (textbox != null && DialogCreatorViewModel_.CurrentDialogRow != null)
            {
                DialogCreatorViewModel_.CurrentDialogRow.LocalizadText.Text = textbox.Text;
            }
        }
    }
}