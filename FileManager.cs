using System.IO;

namespace AnimeDownloader
{
    public class FileManager
    {
        public readonly string AnimeListPath  = "./anime_list";
        public readonly string LogPath = "./log";
        public readonly string ConfigPath = "./config";

        public void WriteFile(string text, string path)
        {
            if (!File.Exists(path))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine("{}");
                    Logger.Instance.Write($"Creating '{path}'!",
                        toLog: false, toConsole: true);
                }	
            }

            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.WriteLine(text);
            }
        }

        public string ReadFile(string path)
        {
            if (!File.Exists(path))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine("{}");
                    Logger.Instance.Write($"Creating '{path}'!",
                        toLog: false, toConsole: true);
                }

                return "";	
            }
            else
            {
                using (StreamReader sr = File.OpenText(path))
                {
                    return sr.ReadToEnd();
                }
            }
        }
    }
}