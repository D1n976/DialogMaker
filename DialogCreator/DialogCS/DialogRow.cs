using DialogCreator;
using javax.print;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace DialogCreatorLibrary
{
    public class LocalizadTextRow : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public LocalizadTextRow() { CharacterName = ""; Text = ""; culture = ""; }
        public LocalizadTextRow(LocalizadTextRow localizadTextRow) { Culture = localizadTextRow.Culture; CharacterName = localizadTextRow.CharacterName; Text = localizadTextRow.Text; }

        private string culture;
        public string Culture { get => culture; set { culture = value; OnPropertyChanged("Culture"); } }

        private string characterName;
        public string CharacterName { get => characterName; set { characterName = value; OnPropertyChanged("CharacterName"); } }

        private string text;
        public string Text { get => text; set { text = value; OnPropertyChanged("Text"); } }
    }
    public class DialogRow : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private string rowName;
        public string RowName { get => rowName; set { rowName = value; OnPropertyChanged("RowName"); } }

        private string commandEventsString;

        public string CommandEventsString { get => commandEventsString; set { commandEventsString = value; OnPropertyChanged("CommandEventsString"); } }

        public List<LocalizadTextRow> LocalizadTexts;

        [JsonIgnore]
        public LocalizadTextRow LocalizadText
        {
            get
            {
                foreach (var LocalizadText in LocalizadTexts)
                    if (LocalizadText.Culture == DialogCreatorViewModel.CurrentCulture)
                        return LocalizadText;
                return null;
            }
        }
        public List<Character> Characters { get; set; }

        public DialogRow() { RowName = ""; CommandEventsString = "";
            LocalizadTexts = new List<LocalizadTextRow>();
            Characters = new List<Character> { };
        }
        public DialogRow(DialogRow dialogRow)
        {
            RowName = dialogRow.RowName;
            CommandEventsString = dialogRow.CommandEventsString;

            Characters= new List<Character>();
            foreach (var c in dialogRow.Characters)
                Characters.Add(new Character(c));

            LocalizadTexts = new List<LocalizadTextRow>();
            foreach (var cult in dialogRow.LocalizadTexts)
            {
                LocalizadTexts.Add(new LocalizadTextRow(cult));
            }
        }
        public void AddLocolizationText()
        {
            LocalizadTexts = new List<LocalizadTextRow>();
            foreach (var cult in DialogCreatorViewModel.Cultures)
                LocalizadTexts.Add(new LocalizadTextRow() { Culture = cult });
        }
    }
}
