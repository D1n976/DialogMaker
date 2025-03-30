using com.sun.org.apache.xml.@internal.resolver.helpers;
using com.sun.xml.@internal.ws.api.model;
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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Shell;

namespace DialogCreator
{
    /// <summary>
    /// Логика взаимодействия для OpenImportPrincessWindow.xaml
    /// </summary>
    public partial class OpenImportPrincessWindow : CustomWindow
    {
        public PrincessImportViewModel PrincessImportViewModel_ { get; set; }

        public OpenImportPrincessWindow(CharacterController princess)
        {
            InitializeComponent();
            PrincessImportViewModel_ = new PrincessImportViewModel() { CharacterController = princess };
            DataContext = PrincessImportViewModel_;
            LoadWindowSettings(Windows.ImportPrincessWindow);
        }
        //*** ACTIONS WITH CHARACTERS ***
        Character currentCharacter;
        private void ChooseCharacter(object sender, MouseButtonEventArgs e) => currentCharacter = ((ListBoxItem)(sender)).DataContext as Character;
        private void addCharacter(object sender, RoutedEventArgs e)
        {
            PrincessImportViewModel_.addCharacter();
        }
        private void removeCharacter(object sender, RoutedEventArgs e)
        {
            PrincessImportViewModel_.removeCharacter();
        }
        //*** ACTIONS WITH CHARACTERS ***

        //*** ACTIONS WITH POSES ***
        Pose CurrentPose;
        private void ChoosePose(object sender, MouseButtonEventArgs e) => CurrentPose = ((ListBoxItem)(sender)).DataContext as Pose;
        private void AddPose(object sender, RoutedEventArgs e) => PrincessImportViewModel_.AddPose();
        private void RemovePose(object sender, RoutedEventArgs e) => PrincessImportViewModel_.RemovePose();

        //*** ACTIONS WITH CLOTHES ***
        //CharacterAttire CurrentAttire;
        //private void ChooseAttire(object sender, MouseButtonEventArgs e) => CurrentAttire = ((ListBoxItem)(sender)).DataContext as CharacterAttire;

        //*** ACTIONS WITH TYPE ***
        CharacterClothType currentPrincessClothType;
        private void ChooseType(object sender, MouseButtonEventArgs e) => currentPrincessClothType = ((ListBoxItem)(sender)).DataContext as CharacterClothType;
        //private void AddType(object sender, RoutedEventArgs e) => PrincessImportViewModel_.AddType(CurrentAttire);
        private void AddStaticType(object sender, RoutedEventArgs e) => PrincessImportViewModel_.AddStaticPoseType(CurrentPose);
        private void RemoveType(object sender, RoutedEventArgs e)
        {
            PrincessImportViewModel_.RemoveStaticType(currentPrincessClothType);
        }
        private void ImportImages(object sender, RoutedEventArgs e)
        {
            PrincessImportViewModel_.ImportImages(currentPrincessClothType);
            Paint();
        }

        private void RemoveImages(object sender, RoutedEventArgs e)
        {
            PrincessImportViewModel_.RemoveImages(currentPrincessClothType);
            Paint();
        }

        void s_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            if (e.ClickCount == 2)
            {
                if (sender is ListBoxItem)
                {
                    ListBoxItem draggedItem = sender as ListBoxItem;
                    DragDrop.DoDragDrop(draggedItem, draggedItem.DataContext, System.Windows.DragDropEffects.Move);
                    draggedItem.IsSelected = true;
                }
            }
        }
        void listbox1_Drop(object sender, System.Windows.DragEventArgs e)
        {
            CharacterClothType droppedData = e.Data.GetData(typeof(CharacterClothType)) as CharacterClothType;
            CharacterClothType target = ((ListBoxItem)(sender)).DataContext as CharacterClothType;
            PrincessImportViewModel_.DropTypes(droppedData, target);
        }
        private void currentCharacterChanged(object sender, SelectionChangedEventArgs e)
        {
            Paint();
        }
        private void Paint()
        {
            if (PrincessImportViewModel_ != null && PrincessImportViewModel_.CharacterController.CharacterIndex >= 0 && PrincessImportViewModel_.CharacterController.CharacterIndex < PrincessImportViewModel_.CharacterController.Characters.Count)
            {
                int poseIndex = PrincessImportViewModel_.CharacterController.Characters[PrincessImportViewModel_.CharacterController.CharacterIndex].PoseIndex;
                if (poseIndex >= 0 && poseIndex < PrincessImportViewModel_.CharacterController.Characters[PrincessImportViewModel_.CharacterController.CharacterIndex].Poses.Count)
                    PaintCharacter(PrincessImportViewModel_.CharacterController.Characters[PrincessImportViewModel_.CharacterController.CharacterIndex].Poses[poseIndex].GetCharacterClothes(), PrincessViewGrid);
            }
        }

        public static void PaintCharacter(string[] paths, Grid grid)
        {
            grid.Children.Clear();
            foreach (string path in paths)
            {
                Image image = new Image();
                PrincessImportViewModel.ChangeImageSourceFromPath(image, path);
                grid.Children.Add(image);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PrincessImportViewModel_.CharacterController.Serialize();
        }

        private void CustomWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveWindowSettings(Windows.ImportPrincessWindow, this.RestoreBounds);
        }

    }
}
