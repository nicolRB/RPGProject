using System;
using System.IO;

namespace RPGProject.Utils
{
    public static class AsciiArtLoader
    {
        private static readonly string basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "ASCII");

        public static string Load(string name)
        {
            string path = Path.Combine(basePath, name + ".txt");
            if (!File.Exists(path))
                return $"[ASCII {name} não encontrado]";
            return File.ReadAllText(path);
        }
    }
}