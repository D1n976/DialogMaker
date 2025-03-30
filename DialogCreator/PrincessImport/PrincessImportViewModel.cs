using com.sun.org.apache.xerces.@internal.impl.dv.xs;
using com.sun.xml.@internal.ws;
using sun.nio.cs.ext;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shell;

namespace DialogCreator
{
    public class PrincessImportViewModel : INotifyPropertyChanged
    {

        public PrincessImportViewModel()
        {
            CharacterController = new CharacterController();
        }

        private CharacterController characterController;
        public CharacterController CharacterController { get => characterController; set { characterController = value; OnPropertyChanged("Princess"); } }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        internal void addCharacter()
        {
            CharacterController.Characters.Add(new Character());
        }

        internal void removeCharacter()
        {
            if (CharacterController.CharacterIndex >= 0 && CharacterController.CharacterIndex < CharacterController.Characters.Count)
                CharacterController.Characters.RemoveAt(CharacterController.CharacterIndex);
        }
        internal void AddPose()
        {
            if (CharacterController.CharacterIndex >= 0 && CharacterController.CharacterIndex < CharacterController.Characters.Count)
                CharacterController.Characters[CharacterController.CharacterIndex].Poses.Add(new Pose());
            OnPropertyChanged("CharacterController");

        }
        internal void RemovePose()
        {
            if (CharacterController.CharacterIndex >= 0 && CharacterController.CharacterIndex < CharacterController.Characters.Count)
            {
                int poseIndex = CharacterController.Characters[CharacterController.CharacterIndex].PoseIndex;
                if (poseIndex >= 0 && poseIndex < CharacterController.Characters[CharacterController.CharacterIndex].Poses.Count)
                    CharacterController.Characters[CharacterController.CharacterIndex].Poses.RemoveAt(poseIndex);
            }
        }

        internal void ImportImages(CharacterClothType princessClothType)
        {
            if (princessClothType == null)
                return;

            Microsoft.Win32.OpenFileDialog openFileDialog1 = new Microsoft.Win32.OpenFileDialog();
            openFileDialog1.Multiselect = true;
            openFileDialog1.Filter = "All Files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            if (openFileDialog1.ShowDialog() == true)
                Import(openFileDialog1.FileNames, princessClothType.Clothes);
            OnPropertyChanged("Princess");
        }

        private static void Import(string[] files, ObservableCollection<CharacterCloth> cloths)
        {
            foreach (var from_file in files)
            {
                string to_path = Paths.GetPrincessImagePath(from_file);
                string unreal_path = Paths.GetUnrealPath(from_file);

                if (!cloths.Any(x => x.ImagePath == System.IO.Path.GetFileName(from_file)))
                {
                    cloths.Add(new CharacterCloth() { ImagePath = System.IO.Path.GetFileName(from_file), UnrealTexturePath = unreal_path });
                }

                if (!File.Exists(to_path))
                    File.Copy(from_file, to_path);
            }

        }


        public static void ChangeImageSourceFromPath(Image imagem, string path)
        {
            if (!File.Exists(path))
                return;

            System.Windows.Media.Imaging.BitmapImage logo = new System.Windows.Media.Imaging.BitmapImage();
            logo.BeginInit();
            logo.UriSource = new Uri(path);
            logo.EndInit();
            imagem.Source = logo;
        }
        internal void RemoveImages(CharacterClothType currentPrincessClothType)
        {
            if (currentPrincessClothType == null)
                return;

            currentPrincessClothType.Clothes.Clear();
            OnPropertyChanged("Princess");
        }
        internal void DropTypes(CharacterClothType droppedData, CharacterClothType target)
        {
            if (droppedData == target)
            {
                return;
            }
            if (IsDropDataAndTargetNearAtArr(droppedData, target))
            {
                RemoveStaticType(droppedData);
                InsertStaticType(droppedData, target);
            }
        }
        bool IsDropDataAndTargetNearAtArr(CharacterClothType droppedData, CharacterClothType target)
        {
            if (droppedData == null || target == null)
                return false;

            if (CharacterController.CharacterIndex >= 0 && CharacterController.CharacterIndex < CharacterController.Characters.Count)
            {
                for (int pInd = 0; pInd < CharacterController.Characters[CharacterController.CharacterIndex].Poses.Count; pInd++)
                {
                    if (CharacterController.Characters[CharacterController.CharacterIndex].Poses[pInd].StaticCharactersClothes.Contains(droppedData) &&
                        CharacterController.Characters[CharacterController.CharacterIndex].Poses[pInd].StaticCharactersClothes.Contains(target))
                        return true;

                }
            }
            return false;
        }
        internal void RemoveStaticType(CharacterClothType dropData)
        {
            if (dropData == null)
                return;
            if (CharacterController.CharacterIndex >= 0 && CharacterController.CharacterIndex < CharacterController.Characters.Count)
            {
                for (int pInd = 0; pInd < CharacterController.Characters[CharacterController.CharacterIndex].Poses.Count; pInd++)
                {
                    if (CharacterController.Characters[CharacterController.CharacterIndex].Poses[pInd].StaticCharactersClothes.Contains(dropData))
                        CharacterController.Characters[CharacterController.CharacterIndex].Poses[pInd].StaticCharactersClothes.Remove(dropData);

                }
            }
            return;
        }
        internal void InsertStaticType(CharacterClothType dropData, CharacterClothType target)
        {
            if (CharacterController.CharacterIndex >= 0 && CharacterController.CharacterIndex < CharacterController.Characters.Count)
                for (int pInd = 0; pInd < CharacterController.Characters[CharacterController.CharacterIndex].Poses.Count; pInd++)
                {
                    for (int stPrinCl = 0; stPrinCl < CharacterController.Characters[CharacterController.CharacterIndex].Poses[pInd].StaticCharactersClothes.Count; stPrinCl++)
                    {
                        if (CharacterController.Characters[CharacterController.CharacterIndex].Poses[pInd].StaticCharactersClothes[stPrinCl] == target)
                        {
                            if (stPrinCl + 1 < CharacterController.Characters[CharacterController.CharacterIndex].Poses[pInd].StaticCharactersClothes.Count)
                            {
                                CharacterController.Characters[CharacterController.CharacterIndex].Poses[pInd].StaticCharactersClothes.Insert(stPrinCl + 1, dropData);
                            }
                            else
                                CharacterController.Characters[CharacterController.CharacterIndex].Poses[pInd].StaticCharactersClothes.Add(dropData);
                            return;
                        }
                    }

                }
            return;
        }

        internal void AddStaticPoseType(Pose currentPose)
        {
            if (currentPose == null)
                return;
            currentPose.StaticCharactersClothes.Add(new CharacterClothType());
        }

        internal void RemovePoseImages(CharacterClothType princessClothType)
        {
            if (princessClothType == null)
                return;
            princessClothType.Clothes.Clear();
        }

    }
}