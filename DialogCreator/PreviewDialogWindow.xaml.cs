using DialogCreatorLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DialogCreator
{
    /// <summary>
    /// Логика взаимодействия для PreviewDialogWindow.xaml
    /// </summary>
    public partial class PreviewDialogWindow : CustomWindow
    {
        private int currentRowIndex = 0;
        DialogCreatorViewModel DialogCreatorViewModel_;
        public PreviewDialogWindow(int selectedRowIndex, DialogCreatorViewModel dialogCreatorViewModel)
        {
            InitializeComponent();
            currentRowIndex = selectedRowIndex;
            DialogCreatorViewModel_ = dialogCreatorViewModel;
            DataContext = DialogCreatorViewModel_;
            if (!DialogCreatorViewModel_.ChooseRow(currentRowIndex))
            {
                DialogCreatorViewModel_.ChooseRow(0);
                currentRowIndex = 0;
            }

            PaintCharacters();
            //InitForeibleTypes();
            //InitPosesPanel();

            LoadWindowSettings(Windows.PreviewWindow);

            imgButtons = new List<Button>()
            {
                openCenterImgButton,
                openFullImgButton,
                openLeftImgButton,

                removeCenterImgButton,
                removeFullImgButton,
                removeLeftImgButton,

                //openRightRightFirstImgButton,
                //openRightSecondImgButton,
                //openRightThirdImgButton,
                //openRightFourthImgButton,

                //removeRightFirstImgButton,
                //removeRightSecondImgButton,
                //removeRightThirdImgButton,
                //removeRightFourthImgButton
            };
            SetVisibilityOfButtons(imgButtons, Visibility.Hidden);
            UpdateOpacity();
        }
        List<Button> imgButtons;

        private void NextRow(object sender, ExecutedRoutedEventArgs e)
        {
            SwitchToNextRow(sender, e);
        }

        private void BackRow(object sender, ExecutedRoutedEventArgs e)
        {
            SwitchToBackRow(sender, e);
        }
        private void SwitchToBackRow(object sender, RoutedEventArgs e)
        {
            int nextRowIndex = currentRowIndex - 1;
            if (nextRowIndex >= 0 && nextRowIndex < DialogCreatorViewModel_.CurrentDialogRows.Count)
            {
                DialogCreatorViewModel_.ChooseRow(nextRowIndex);
                currentRowIndex = nextRowIndex;
                PaintCharacters();
                //InitForeibleTypes();
                //InitPosesPanel();
            }
        }

        private void SwitchToNextRow(object sender, RoutedEventArgs e)
        {
            int nextRowIndex = currentRowIndex + 1;
            if (nextRowIndex >= 0 && nextRowIndex < DialogCreatorViewModel_.CurrentDialogRows.Count)
            {
                List<Character> previos_characters = DialogCreatorViewModel_.CurrentDialogRow.Characters;
                DialogCreatorViewModel_.ChooseRow(nextRowIndex);
                currentRowIndex = nextRowIndex;
                //InitForeibleTypes();
                //InitPosesPanel();
                if(IsinitCharactersCheckBox.IsChecked == true)
                {
                    DialogCreatorViewModel_.CurrentDialogRow.Characters = new List<Character>();
                    previos_characters.ForEach(x => DialogCreatorViewModel_.CurrentDialogRow.Characters.Add(new Character(x)));
                }
                PaintCharacters();
            }
        }

        private void InitPosesPanel(StackPanel characterPanel)
        {
            characterPanel.Children.Clear();
            if(SelectedCharacter == null)
                return;

            if (DialogCreatorViewModel_.CharacterController.Characters.FirstOrDefault() != null && DialogCreatorViewModel_.CharacterController.CharacterIndex >= 0 && DialogCreatorViewModel_.CharacterController.CharacterIndex < DialogCreatorViewModel_.CharacterController.Characters.Count)
            {
                //Initialize character
                //Получаем всех персов
                CharacterController characterController = DialogCreatorViewModel_.CharacterController;

                //Инициализируем индексы всех персонажей индексами нашего 
                foreach (var character in characterController.Characters)
                {
                    if (character.Name == SelectedCharacter.Name)
                    {
                        characterController.CharacterIndex = characterController.Characters.IndexOf(character);
                    }
                }

                //Дополняем панель именами персонажей
                characterPanel.Children.Add(CreateCustomPanel(
                    (s, e) =>
                {
                    if (characterController.CharacterIndex + 1 >= 0 && characterController.CharacterIndex + 1 < characterController.Characters.Count)
                    {
                        characterController.CharacterIndex++;
                        SaveCharacter();
                        InitPosesPanel(characterPanel);
                    }
                },
                    (s, e) =>
                {
                    if (characterController.CharacterIndex - 1 >= 0 && characterController.CharacterIndex - 1 < characterController.Characters.Count)
                    {
                        characterController.CharacterIndex--;
                        SaveCharacter();
                        InitPosesPanel(characterPanel);
                    }
                },
                characterController.Characters[characterController.CharacterIndex].Name, (SolidColorBrush)new BrushConverter().ConvertFrom("#7971c6")));

                //Получаем индекс персонажа и все позы для него
                int char_index = characterController.CharacterIndex;
                ObservableCollection<Pose> characterControllerPoses = characterController.Characters[char_index].Poses;

                if (characterControllerPoses != null && characterControllerPoses.Count != 0)
                {
                    //Инициализируем индекс текущей позы 
                    foreach (var pose in characterControllerPoses)
                    {
                        if (pose.PoseName == SelectedCharacter.Poses[SelectedCharacter.PoseIndex].PoseName)
                            characterController.Characters[char_index].PoseIndex = characterControllerPoses.IndexOf(pose);
                        {
                            //Дополняем панель именами доступных поз
                            characterPanel.Children.Add(CreateCustomPanel(
                               (s, e) =>
                               {
                                   if (characterController.Characters[char_index].PoseIndex + 1 >= 0 && characterController.Characters[char_index].PoseIndex + 1 < characterController.Characters[char_index].Poses.Count)
                                   {
                                       characterController.Characters[char_index].PoseIndex++;
                                       SaveCharacter();
                                       InitPosesPanel(characterPanel);
                                   }
                               },
                               (s, e) =>
                               {
                                   if (characterController.Characters[char_index].PoseIndex - 1 >= 0 && characterController.Characters[char_index].PoseIndex - 1 < characterController.Characters[char_index].Poses.Count)
                                   {
                                       characterController.Characters[char_index].PoseIndex--;
                                       SaveCharacter();
                                       InitPosesPanel(characterPanel);
                                   }
                               },
                           characterControllerPoses[characterController.Characters[char_index].PoseIndex].PoseName, Brushes.White));
                            break;
                        }
                    }
                    int character_controller_pose_index = characterController.Characters[char_index].PoseIndex;
                    ObservableCollection<CharacterClothType> characterControllerClothes = characterControllerPoses[character_controller_pose_index].StaticCharactersClothes;
                    //Инициализируем индексы одежды текуще выбранной одеждой, если таковая есть
                    foreach (CharacterClothType clothType in characterControllerClothes)
                    {
                        foreach (CharacterClothType curClothType in SelectedCharacter.Poses[SelectedCharacter.PoseIndex].StaticCharactersClothes)
                        {
                            if (curClothType.ClothType == clothType.ClothType)
                            {
                                if (curClothType.StaticPathIndex >= 0 && curClothType.StaticPathIndex < clothType.Clothes.Count)
                                    clothType.StaticPathIndex = curClothType.StaticPathIndex;
                                else
                                    clothType.StaticPathIndex = 0;
                            }
                        }
                    }
                    //Дополняем панель одеждой
                    foreach (CharacterClothType clothType in characterControllerClothes)
                    {
                        if (clothType.Clothes.Count > 1)
                            characterPanel.Children.Add(CreateCustomPanel(
                                  (s, e) =>
                                  {
                                      if (clothType.StaticPathIndex + 1 >= 0 && clothType.StaticPathIndex + 1 < clothType.Clothes.Count)
                                      {
                                          clothType.StaticPathIndex++;
                                          SaveCharacter();
                                          InitPosesPanel(characterPanel);
                                      }
                                  },
                                  (s, e) =>
                                  {
                                      if (clothType.StaticPathIndex - 1 >= 0 && clothType.StaticPathIndex - 1 < clothType.Clothes.Count)
                                      {
                                          clothType.StaticPathIndex--;
                                          SaveCharacter();
                                          InitPosesPanel(characterPanel);
                                      }
                                  }, clothType.Clothes[clothType.StaticPathIndex].ImagePath, Brushes.White));
                    }
                }
            }
            PaintCharacters();
        }

        StackPanel CreateCustomPanel(RoutedEventHandler BtnDownClick, RoutedEventHandler BtnUpClick, string TextBlockText, Brush foregroundBrush)
        {
            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Horizontal;

            Button btnFirstPose = CreateButton("←");
            btnFirstPose.Click += BtnUpClick;

            TextBlock textBlock = new TextBlock();
            textBlock.Text = TextBlockText;
            textBlock.TextAlignment = TextAlignment.Center;
            textBlock.VerticalAlignment = VerticalAlignment.Center;
            textBlock.Foreground = foregroundBrush;
            textBlock.Width = 100;

            Button btnBackPose = CreateButton("→");
            btnBackPose.Click += BtnDownClick;

            stackPanel.Children.Add(btnFirstPose);
            stackPanel.Children.Add(textBlock);
            stackPanel.Children.Add(btnBackPose);
            return stackPanel;
        }
        Button CreateButton(string TextToView)
        {
            Button button = new Button();
            button.Content = new ContentControl() { Content = new TextBlock() { Text = TextToView, FontSize = 18 } };
            button.Width = 50;
            button.Height = 50;
            button.Style = this.FindResource("DarkButton") as Style; ;
            return button;
        }

        Character SelectedCharacter { get; set; }
        private void OpenImagesWindow(Positions position)
        {
            SelectedCharacter = DialogCreatorViewModel_.CurrentDialogRow.Characters.Where(x => x.Position == position).FirstOrDefault();
            if (SelectedCharacter == null)
            {
                SelectedCharacter = new Character(DialogCreatorViewModel_.characterController.Characters.FirstOrDefault());
                SelectedCharacter.Position = position;
                DialogCreatorViewModel_.CurrentDialogRow.Characters.Add(SelectedCharacter);
            }
            InitPosesPanel(changeCharacterPanel);
        }

        private void RemoveImages(Positions position)
        {
            Character characterToRemove = DialogCreatorViewModel_.CurrentDialogRow.Characters.Where(x => x.Position == position).FirstOrDefault();
            if (characterToRemove != null)
                DialogCreatorViewModel_.CurrentDialogRow.Characters.Remove(characterToRemove);
            PaintCharacters();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveWindowSettings(Windows.PreviewWindow, this.RestoreBounds);
        }

        private void SaveCharacter()
        {
            if (SelectedCharacter != null && DialogCreatorViewModel_.CharacterController.CharacterIndex >= 0 && DialogCreatorViewModel_.CharacterController.CharacterIndex < DialogCreatorViewModel_.CharacterController.Characters.Count)
            {
                Positions oldPositon = SelectedCharacter.Position;
                SelectedCharacter.Copy(DialogCreatorViewModel_.CharacterController.Characters[DialogCreatorViewModel_.CharacterController.CharacterIndex]);
                SelectedCharacter.Position = oldPositon;    
            }
            PaintCharacters();
        }

        private void PaintCharacters()
        {
            leftImageGrid.Children.Clear();
            centerImageGrid.Children.Clear();
            fullImageGrid.Children.Clear();

            rightFirstImageGrid.Children.Clear();
            rightSecondImageGrid.Children.Clear();
            rightThirdImageGrid.Children.Clear();
            rightFourthImageGrid.Children.Clear();

            if (DialogCreatorViewModel_.CurrentDialogRow.Characters != null)
                foreach (var character in DialogCreatorViewModel_.CurrentDialogRow.Characters)
                {
                    PaintCharacter(character);
                }
        }
        void PaintCharacter(Character character)
        {
            foreach (var ch in DialogCreatorViewModel_.CurrentDialogRow.Characters)
            {
                if (character.PoseIndex >= 0 && character.PoseIndex < character.Poses.Count)
                    character.Poses[character.PoseIndex].GetCharacterClothes().ToList().ForEach(x =>
                    {
                        Image img_to_view = new Image();
                        PrincessImportViewModel.ChangeImageSourceFromPath(img_to_view, x);
                        switch (character.Position)
                        {
                            case Positions.Left:
                                leftImageGrid.Children.Add(img_to_view);
                                break;
                            case Positions.Center:
                                centerImageGrid.Children.Add(img_to_view);
                                break;
                            case Positions.FullScreen:
                                fullImageGrid.Children.Add(img_to_view);
                                break;

                            case Positions.RightFirst:
                                rightFirstImageGrid.Children.Add(img_to_view);
                                break;
                            case Positions.RightSecond:
                                rightSecondImageGrid.Children.Add(img_to_view);
                                break;
                            case Positions.RightThird:
                                rightThirdImageGrid.Children.Add(img_to_view);
                                break;
                            case Positions.RightFourth:
                                rightFourthImageGrid.Children.Add(img_to_view);
                                break;
                        };
                    });
            }
        }

        private void OpenLeftImages(object sender, RoutedEventArgs e) => OpenImagesWindow(Positions.Left);
        private void OpenCenterImages(object sender, RoutedEventArgs e) => OpenImagesWindow(Positions.Center);
        private void OpenFullScreenImages(object sender, RoutedEventArgs e) => OpenImagesWindow(Positions.FullScreen);

        private void RemoveLeftImage(object sender, RoutedEventArgs e) => RemoveImages(Positions.Left);
        private void RemoveCenterImages(object sender, RoutedEventArgs e) => RemoveImages(Positions.Center);
        private void RemoveFullImage(object sender, RoutedEventArgs e) => RemoveImages(Positions.FullScreen);

        private void Button_Click_1(object sender, RoutedEventArgs e) => OpenImagesWindow(Positions.RightFourth);
        private void Button_Click_2(object sender, RoutedEventArgs e) => OpenImagesWindow(Positions.RightSecond);
        private void Button_Click_3(object sender, RoutedEventArgs e) => OpenImagesWindow(Positions.RightThird);
        private void Button_Click_4(object sender, RoutedEventArgs e) => OpenImagesWindow(Positions.RightFirst);

        private void removeRightFirstImgButton_Click(object sender, RoutedEventArgs e) => RemoveImages(Positions.RightFirst);
        private void removeRightSecondImgButton_Click(object sender, RoutedEventArgs e) => RemoveImages(Positions.RightSecond);
        private void removeRightThirdImgButton_Click(object sender, RoutedEventArgs e) => RemoveImages(Positions.RightThird);
        private void removeRightFourthImgButton_Click(object sender, RoutedEventArgs e) => RemoveImages(Positions.RightFourth);

        private void ShowButtons(object sender, RoutedEventArgs e) => SetVisibilityOfButtons(imgButtons, Visibility.Visible);
        private void UnshowButtons(object sender, RoutedEventArgs e) => SetVisibilityOfButtons(imgButtons, Visibility.Hidden);

        private void IncreaseOpacityButtonValue(object sender, RoutedEventArgs e)
        {
            float opacity = DialogCreatorViewModel_.OpacityButtonValue + 0.1f;
            if(opacity > 0 && opacity < 1)
                DialogCreatorViewModel_.OpacityButtonValue = opacity;
            UpdateOpacity();
        }

        private void DecreaseOpacityButtonValue(object sender, RoutedEventArgs e)
        {
            float opacity = DialogCreatorViewModel_.OpacityButtonValue - 0.1f;
            if (opacity > 0 && opacity < 1)
                DialogCreatorViewModel_.OpacityButtonValue = opacity;
            UpdateOpacity();
        }
        private void UpdateOpacity()
        {
            foreach (var btn in imgButtons)
                btn.Opacity = DialogCreatorViewModel_.OpacityButtonValue;
        }
        private void SetVisibilityOfButtons(List<Button> buttons, Visibility visibility) 
            => buttons.ForEach(x => x.Visibility = visibility);

    }
}
