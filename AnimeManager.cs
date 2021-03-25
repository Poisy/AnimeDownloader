using System.Collections.Generic;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace AnimeDownloader
{
    public class AnimeManager
    {
        private Scrapper _scrapper;
        private Downloader _downloader;
        private FileManager _fileManager;
        private List<Anime> _animes { get; set; } = new List<Anime>();
        public int AnimeCount { get => _animes.Count; }

        public AnimeManager()
        {
            _fileManager = new FileManager();

            ReadAnimeList();
            ReadConfig();

            _scrapper = new Scrapper(Anime.Submitter);
            _downloader = new Downloader();
            
        }

        public bool AddAnime(Anime anime)
        {
            bool isAnimeNotInList = _animes.TrueForAll(a => a.Title != anime.Title );
            
            if (isAnimeNotInList)
            {
                _animes.Add(anime);
                WriteAnimeList();
                return true; 
            }

            return false;
        }

        public bool RemoveAnime(Anime anime)
        {
            Anime animeToRemove = _animes.Find(a => a.Title == anime.Title);

            if (animeToRemove != null)
            {
                _animes.Remove(animeToRemove);
                WriteAnimeList();
                return true;
            }

            return false;
        }

        public bool SetAnimeEpisode(Anime anime)
        {
            bool isAnimeChanged = false;

            _animes.ForEach(a => 
            {
                if (a.Title == anime.Title)
                {
                    a.Episode = anime.Episode;
                    isAnimeChanged = true;
                    return;
                }
            });

            if (isAnimeChanged)
            {
                WriteAnimeList();
                return true;
            }

            return false;
        }

        public Anime[] GetAllAnime() => new List<Anime>(_animes).ToArray();

        public bool DownloadAnime(Anime anime)
        {
            if (_scrapper.SetTorrentURL(anime))
            {
                List<Anime> animes = new List<Anime>();
                animes.Add(anime);
                _downloader.DownloadAll(animes);
                return true;
            }
            return false;
        }

        public void DownloadNewEpisodes()
        {
            foreach (var anime in _animes)
            {
                anime.Episode++;
                if (!DownloadAnime(anime))
                {
                    anime.Episode--;
                    Logger.Instance.Write($"No new episodes for {anime.Title}.\n" + 
                    $"Current episode {anime.Episode}, waiting for the next.\n", toConsole: true);
                }
            }

            WriteAnimeList();
        }

        public void DownloadAllEpisodes(Anime anime)
        {
            List<Anime> animes = new List<Anime>();
            while (_scrapper.SetTorrentURL(anime))
            {
                animes.Add(new Anime { Title = anime.Title, Episode = anime.Episode, TorrentUrl = anime.TorrentUrl });
                anime.Episode++;
            }
            _downloader.DownloadAll(animes);
        }

        public void ChangeResolution(int resoultion)
        {
            Anime.Resolution = resoultion;

            WriteConfig();
        }

        public void ChangeSubmitter(string submitter)
        {
            Anime.Submitter = submitter;

            WriteConfig();
        }

        private void ReadAnimeList()
        {
            string json = _fileManager.ReadFile(_fileManager.AnimeListPath);
            
            Dictionary<string, int> animes = new Dictionary<string, int>();

            try
            {
                animes = JsonConvert.DeserializeObject<Dictionary<string, int>>(json);
            }
            catch (Exception)
            {
                Logger.Instance.Write("Error: Couldn't read from 'anime_list.txt'.",
                    toConsole: true);
                return;
            }

            if (animes == null) return;

            foreach (var anime in animes)
            {
                _animes.Add(new Anime 
                { 
                    Title = anime.Key, 
                    Episode = anime.Value 
                });
            }
        }

        private void WriteAnimeList()
        {
            Dictionary<string, int> animes = new Dictionary<string, int>();

            foreach (var anime in _animes)
            {
                animes.Add(anime.Title, anime.Episode);
            }

            string json = JsonConvert.SerializeObject(animes);

            _fileManager.WriteFile(json, _fileManager.AnimeListPath);
        }
    
        private void ReadConfig()
        {
            string json = _fileManager.ReadFile(_fileManager.ConfigPath);

            Dictionary<string, string> config = new Dictionary<string, string>();

            try
            {
                config = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            }
            catch (Exception)
            {
                Logger.Instance.Write("Error: Couldn't read from 'config.txt'.",
                    toConsole: true);
                return;
            }

            if (config == null) return;

            if (config.ContainsKey("resolution"))
            {
                int resolution = 1080;
                if (int.TryParse(config["resolution"], out resolution))
                {
                    Anime.Resolution = resolution;
                }
            }
            if (config.ContainsKey("submitter")) Anime.Submitter = config["submitter"];
        }

        private void WriteConfig()
        {
            Dictionary<string, string> config = new Dictionary<string, string>();

            config.Add("submitter", Anime.Submitter);
            config.Add("resolution", Anime.Resolution.ToString());

            string json = JsonConvert.SerializeObject(config);

            _fileManager.WriteFile(json, _fileManager.ConfigPath);
        }
    }
}