using com.sun.org.apache.bcel.@internal.generic;
using com.sun.tools.sjavac.comp;
using com.sun.xml.@internal.ws.runtime.config;
using DialogCreatorLibrary;
using javax.management.openmbean;
using Newtonsoft.Json;
using sun.java2d.loops;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DialogCreator
{
    public class ClothType
    {
        public string Name { get; set; }
        public ClothType(string name)
        {
            Name = name;
        }
    }
    public class CharacterCloth
    {
        public string ImagePath { get; set; } = "";
        public string UnrealTexturePath { get; set; } = "";
        public string SpriteInfoName { get; set; } = "";
        public CharacterCloth() { }

        public CharacterCloth(CharacterCloth CharacterCloth)
        {
            ImagePath = CharacterCloth.ImagePath;
            SpriteInfoName = CharacterCloth.SpriteInfoName;
            UnrealTexturePath = CharacterCloth.UnrealTexturePath;
        }
    }
    public class CharacterClothType
    {
        public CharacterClothType() { ClothType = new ClothType(""); Clothes = new ObservableCollection<CharacterCloth>(); }
        public ClothType ClothType { get; set; }
        public ObservableCollection<CharacterCloth> Clothes { get; set; }

        public int StaticPathIndex { get; set; } = 0;
        public CharacterClothType(CharacterClothType ClothType)
        {
            this.ClothType = new ClothType(ClothType.ClothType.Name); 
            Clothes = new ObservableCollection<CharacterCloth>();
            foreach(var Cl in ClothType.Clothes)
                Clothes.Add(new CharacterCloth(Cl));
            StaticPathIndex = ClothType.StaticPathIndex;
        }
    }
    public class Pose
    {
        public string PoseName { get; set; } = "";
        public ObservableCollection<CharacterClothType> StaticCharactersClothes { get; set; }
        public int StaticCharacterClothesIndex { get; set; }
        public Pose(Pose Pose)
        {
            StaticCharactersClothes = new ObservableCollection<CharacterClothType>();
            foreach (var sPC in Pose.StaticCharactersClothes)
                StaticCharactersClothes.Add(new CharacterClothType(sPC));
            StaticCharacterClothesIndex = Pose.StaticCharacterClothesIndex;
            PoseName = Pose.PoseName;

        }
        public Pose()
        {
            StaticCharactersClothes = new ObservableCollection<CharacterClothType>();
        }
        public string[] GetCharacterClothes()
        {
            List<string> paths = new List<string>();

            foreach (var StaticPrincessCloth in StaticCharactersClothes)
            {
                if (StaticPrincessCloth.StaticPathIndex >= 0 && StaticPrincessCloth.StaticPathIndex < StaticPrincessCloth.Clothes.Count)
                {
                    paths.Add(Path.Combine(Directory.GetCurrentDirectory(), Paths.ImagePath, StaticPrincessCloth.Clothes[StaticPrincessCloth.StaticPathIndex].ImagePath));
                }
            }
            return paths.ToArray();
        }
    }
    public class Character
    {
        public string Name { get; set; } = "";
        public ObservableCollection<Pose> Poses { get; set; }
        public int PoseIndex { get; set; }
        public Positions Position { get; set; }
        public Character(Character Characer)
        {
            Copy(Characer);
        }
        public Character()
        {
            Poses = new ObservableCollection<Pose>();
        }
        public void Copy(Character new_character)
        {
            Poses = new ObservableCollection<Pose>();
            foreach (var Pose in new_character.Poses)
            {
                Poses.Add(new Pose(Pose));
            }
            PoseIndex = new_character.PoseIndex;
            Name = new_character.Name;
            Position = new_character.Position;
        }
    }
    public class CharacterController
    {

        public ObservableCollection<Character> Characters { get; set; }
        public CharacterController()
        {
            Characters = new ObservableCollection<Character>();
        }

        public int CharacterIndex { get; set; } = 0;
        public async void Serialize() => await Serialize(System.IO.Path.ChangeExtension(System.IO.Path.Combine(Directory.GetCurrentDirectory(), Paths.DataPath, "characterController"), ".json"));
        public async Task Serialize(string filePath)
        {
            //Remove old save file
            if (File.Exists(filePath))
                File.Delete(filePath);

            using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                string js_str = JsonConvert.SerializeObject(Characters);
                var buffer = Encoding.UTF8.GetBytes(js_str);
                await fs.WriteAsync(buffer, 0, buffer.Length);
            }
        }
        public static CharacterController Deserialize()
        {
            CharacterController characterController = new CharacterController();
            try
            {
                string js_str = File.ReadAllText(System.IO.Path.Combine(Directory.GetCurrentDirectory(), Paths.DataPath, "characterController.json"));
                ObservableCollection<Character> Characters = JsonConvert.DeserializeObject<ObservableCollection<Character>>(js_str);
                if(Characters != null)
                    characterController.Characters = Characters;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating directory: {ex.Message}");
            }
            return characterController;
        }
    }
}