using com.sun.rowset.@internal;
using DialogCreatorLibrary;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;

namespace DialogCreator
{
    public class DialogCreatorViewModel : INotifyPropertyChanged, INotifyCollectionChanged
    {
        public DialogCreatorViewModel()
        {
            CurrentDialogRows = new ObservableCollection<DialogRow>(currentDialogRows);
            Dialogs = new ObservableCollection<Dialog>(dialogs);
        }

        public static string[] Cultures { get; set; } = { "ru", "eu", "jp" };
        public static string CurrentCulture { get; set; } = Cultures[0];

        //all dialogs frompath
        private ObservableCollection<Dialog> dialogs = new ObservableCollection<Dialog>();
        public ObservableCollection<Dialog> Dialogs { get => dialogs; set { dialogs = value; OnPropertyChanged("Dialogs"); } }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public void OnCollectionChanged(NotifyCollectionChangedEventArgs e) => CollectionChanged?.Invoke(this, e);

        //to change dialogrow
        private ObservableCollection<DialogRow> currentDialogRows = new ObservableCollection<DialogRow>();
        public ObservableCollection<DialogRow> CurrentDialogRows { get => currentDialogRows; set { currentDialogRows = value; OnPropertyChanged("CurrentDialogRows"); } }

        //to change one dialog
        public DialogRow currentDialogRow = new DialogRow();
        public DialogRow CurrentDialogRow { get { return currentDialogRow; } set { currentDialogRow = value; OnPropertyChanged("CurrentDialogRow"); } }

        public CharacterController characterController = new CharacterController();
        public CharacterController CharacterController { get { return characterController; } set { characterController = value; OnPropertyChanged("CharacterController"); } }

        public bool? rememberCurrentCharacterParams = true;
        public bool? IsRememberCurrentCharacterParams { get { return rememberCurrentCharacterParams; } set { rememberCurrentCharacterParams = value; OnPropertyChanged("IsRememberCurrentCharacterParams"); } }

        private float opacityButtonValue = 0.1f;
        public float OpacityButtonValue { get => opacityButtonValue; set { opacityButtonValue = value; OnPropertyChanged("OpacityButtonValue"); } }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        internal void ChooseDialog(Dialog selectedItem)
        {
            if (selectedItem == null)
                return;

            CurrentDialogRows = selectedItem.DialogRows;
        }
        internal bool ChooseRow(int rowIndex)
        {
            if (rowIndex >= 0 && rowIndex < currentDialogRows.Count)
            {
                CurrentDialogRow = CurrentDialogRows[rowIndex];
                return true;
            }
            return false;
        }
        internal void ChooseRow(DialogRow row)
        {
            CurrentDialogRow = row;
        }
        public void AddDialog(string v)
        {
            Dialogs.Add(new Dialog() { DialogName = v });
        }

        internal void RemoveDialog(Dialog dialog)
        {
            Dialogs.Remove(dialog);
            CurrentDialogRows = null;
            CurrentDialogRow = null;   
        }


        internal DialogRow AddRow(bool? isRowChangeToDefault)
        {
            if (CurrentDialogRows == null)
                return null;
            DialogRow dialogRow = new DialogRow();
            if (isRowChangeToDefault == true)
                dialogRow.RowName = $"Row{CurrentDialogRows.Count}";
            dialogRow.AddLocolizationText();

            CurrentDialogRows.Add(dialogRow);
            RefrashDialogNames();

            return dialogRow;
        }
        internal void DublicateRow(DialogRow row)
        {
            if(row == null) 
                return;

            DialogRow fromCopyDialog = CurrentDialogRows.Where(x => x == row).FirstOrDefault();
            if (fromCopyDialog != null)
                CurrentDialogRows.Insert(CurrentDialogRows.IndexOf(row) + 1, new DialogRow(fromCopyDialog));
            RefrashDialogNames();
        }

        internal DialogRow RemoveRow(DialogRow row)
        {
            if(CurrentDialogRows == null || row == null)
                return null;
            int index = CurrentDialogRows.IndexOf(row);

            CurrentDialogRows.Remove(row);
            RefrashDialogNames();
            return index >= 0 && index < CurrentDialogRows.Count ? CurrentDialogRows[index] : null;
        }

        internal void SwapRows(DialogRow fromDialog, DialogRow toDialog)
        {
            if (fromDialog == null || toDialog == null || CurrentDialogRows == null)
                return;

            int from = CurrentDialogRows.IndexOf(fromDialog);
            int to = CurrentDialogRows.IndexOf(toDialog);
            if (IsRowValid(from) && IsRowValid(to))
                (CurrentDialogRows[from], CurrentDialogRows[to]) = (CurrentDialogRows[to], CurrentDialogRows[from]);
            RefrashDialogNames();
        }

        private bool IsRowValid(int from)
        {
            return from >= 0 && from < CurrentDialogRows.Count;
        }

        internal DialogRow InsertRow(DialogRow copiedDialogRow, DialogRow row)
        {

            if (copiedDialogRow == null || row == null || CurrentDialogRows == null)
                return null;
            DialogRow dialogRow = new DialogRow(copiedDialogRow);
            CurrentDialogRows.Insert(CurrentDialogRows.IndexOf(row) + 1, dialogRow);
            RefrashDialogNames();

            return dialogRow;
        }

        internal DialogRow InsertNewRow(DialogRow row, bool? isRowChangeToDefault)
        {
            if(row == null || CurrentDialogRows == null) 
                return null;
            int rowIndex = CurrentDialogRows.IndexOf(row) + 1;
            if (IsRowValid(rowIndex))
            {
                DialogRow dialogRow = new DialogRow();
                if (isRowChangeToDefault == true)
                    dialogRow.RowName = $"Row{CurrentDialogRows.Count}";
                CurrentDialogRows.Insert(rowIndex, dialogRow);
                RefrashDialogNames();
            }
            else
                return AddRow(true);

            return CurrentDialogRows[rowIndex];
        }

        internal void RefrashDialogNames()
        {
            if (CurrentDialogRows != null)
                RefrashDialogNames(CurrentDialogRows);
        }
        public static void RefrashDialogNames(ObservableCollection<DialogRow> dialogRows)
        {
            for (int i = 0; i < dialogRows.Count; i++)
                dialogRows[i].RowName = "Row" + i.ToString();
        }

        internal void ChangeDialogCulture(int selectedIndex)
        {
            var temp = CurrentDialogRows;
            CurrentDialogRows = null;
            CurrentDialogRows = temp;
        }
    }
}