using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DialogCreator
{
    public class Paths
    {
        public static readonly string DataPath = "DialogsData";

        public static readonly string DialogPath = $"{DataPath}\\Dialogs";
        public static readonly string ImagePath = $"{DataPath}\\Images";
        public static readonly string JsonCharactersPathFile = $"{DataPath}\\characters";
        public static void CreateFolder(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
        public static string GetPrincessImagePath(string fileName)
        {
            return System.IO.Path.Combine(Directory.GetCurrentDirectory(), $"{Paths.ImagePath}\\{System.IO.Path.GetFileName(fileName)}");
        }

        internal static string GetUnrealPath(string from_file)
        {
            string newImagePath = System.IO.Path.GetFileName(from_file);
            int index_of_extention = newImagePath.IndexOf('.');

            if (index_of_extention >= 0)
                newImagePath = newImagePath.Substring(0, index_of_extention);
            return $"/Script/Engine.Texture2D'/Game/{DataPath}/Images/{newImagePath}.{newImagePath}'";
        }
    }
}
