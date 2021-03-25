namespace AnimeDownloader
{
    public class Startup
    {
        private AnimeManager _manager;
        private bool _continuesMod;

        public Startup()
        {
            _manager = new AnimeManager();
            _continuesMod = false;
        }

        public void Start(string[] args) => CheckArgs(args);

        private void CheckArgs(string[] args)
        {
            if (args.Length == 0)
            {
                _continuesMod = true;
                Logger.Instance.Write("\n=> ", toConsole: true, toLog: false);
                args = System.Console.ReadLine().Split("\"");
                args[0] = args[0].Replace(" ", "");
            }

            switch (args[0])
            {
                case "download": // Downloads anime
                    DownloadAnime(args);
                    break;
                
                case "add": // Add new anime to the list
                    AddAnime(args);
                    break;

                case "remove":  // Remove a anime from the list
                    RemoveAnime(args);
                    break;
                
                case "set": // Set episode for a anime
                    SetAnime(args);
                    break;

                case "list":    // Prints all Anime on the Console
                    PrintAnimes();
                    break;

                case "resolution":  // Change or print resolution for anime
                    Resolution(args);
                    break;
                
                case "submitter":  // Change or print submitter for anime
                    Submitter(args);
                    break;

                case "help":
                    PrintInfo();
                    break;

                case "exit":
                    _continuesMod = false;
                    break;
                
                default:
                    Logger.Instance.Write(
                        $"Unknown command. Try 'help' for help.\n", toConsole: true, toLog: false);
                    break;
            }

            if (_continuesMod == true)
            {
                CheckArgs(new string[] { });
            }
        }
    
        private void DownloadAnime(string[] args)
        {
            if (args.Length == 1)
            {
                if (_manager.AnimeCount == 0)
                {
                    Logger.Instance.Write("No anime added to the list.\n", toConsole: true, toLog: false);
                }
                // Download all new episodes of the added anime
                else
                {
                    _manager.DownloadNewEpisodes();
                }
            }
            else if (args.Length > 1)
            {
                Anime anime = new Anime() { Title = args[1] };
                int episode = -1;

                if (args.Length > 2 && int.TryParse(args[2], out episode) && episode != -1)
                {
                    // Download the specific episode
                    anime.Episode = episode;
                    if (!_manager.DownloadAnime(anime))
                    {
                        Logger.Instance.Write("Cannot find Anime or Episode.\n"+
                        $"[Title: {anime.Title}][Episode: {anime.Episode}]"+
                        $"[Submitter: {Anime.Submitter}][Resolution: {Anime.Resolution}]\n", toConsole: true);
                    }
                }
                else 
                {
                    // Downloads all episodes from the anime
                    anime.Episode = 1;
                    _manager.DownloadAllEpisodes(anime);
                }
            }
        }

        private void PrintInfo()
        {
            Logger.Instance.Write(
@"download                          => auto download new episodes from added animes
download [anime] [episode]          => manually download episode
download [anime]                    => download all episodes
add [anime] [episode]=(1)           => add new anime
remove [anime]                      => remove anime
set [anime] [episode]               => set episode of anime
list                                => output all anime and episodes
resolution [number]                 => if given value, change the resolution, else it prints it out
submitter [name]                    => if given value, change the submitter, else it prints it out"
, toConsole: true, toLog: false);
        }

        private void AddAnime(string[] args)
        {
            if (args.Length < 2) return;

            Anime newAnime = new Anime { Title = args[1] };
            int episode = 1;

            if (args.Length > 2)
            {
                int.TryParse(args[2], out episode);
            }

            newAnime.Episode = episode;

            if (_manager.AddAnime(newAnime))
            {
                Logger.Instance.Write(
                    $"{newAnime.Title} was added successfully!\n", toConsole: true);
            }
            else
            {
                Logger.Instance.Write(
                    $"{newAnime.Title} is already added.\n", toConsole: true);
            }
        }
    
        private void RemoveAnime(string[] args)
        {
            if (args.Length > 1)
            {
                Anime anime = new Anime { Title = args[1] };
                if (_manager.RemoveAnime(anime))
                {
                    Logger.Instance.Write(
                    $"{anime.Title} was removed successfully!\n", toConsole: true);
                }
                else
                {
                    Logger.Instance.Write(
                        $"{anime.Title} does not exist.\n", toConsole: true);
                }
            }
        }
    
        private void SetAnime(string[] args)
        {
            if (args.Length > 2)
            {
                Anime anime = new Anime { Title = args[1] };
                int episode = 1;
                
                if (int.TryParse(args[2], out episode))
                {
                    anime.Episode = episode;
                    if (_manager.SetAnimeEpisode(anime))
                    {
                        Logger.Instance.Write(
                            $"{anime.Title} was set to episode {anime.Episode}\n", toConsole: true);
                    }
                    else
                    {
                        Logger.Instance.Write(
                            $"{anime.Title} does not exist.\n", toConsole: true);
                    }
                }
                else
                {
                    Logger.Instance.Write(
                        $"Episode is not a number.\n", toConsole: true);
                }
            }
            else
            {
                Logger.Instance.Write(
                    $"set [anime] [episode]\n", toConsole: true, toLog: false);
            }

        }
    
        private void PrintAnimes()
        {
            var animes = _manager.GetAllAnime();

            if (animes.Length == 0)
            {
                Logger.Instance.Write("List is empty. No anime added.\n", toConsole: true, toLog: false);
            }
            else
            {
                foreach (var anime in animes)
                {
                    Logger.Instance.Write(
                        $"Title: {anime.Title}, Episode: {anime.Episode}\n", toConsole: true, toLog: false);
                }

                Logger.Instance.Write("All Anime printed on the Console!");
            }
        }
    
        private void Resolution(string[] args)
        {
            if (args.Length > 1) // Change resolution
            {
                int resolution = 1080;
                if (int.TryParse(args[1], out resolution))
                {
                    _manager.ChangeResolution(resolution);

                    Logger.Instance.Write(
                        $"Resolution changed to {Anime.Resolution}!\n", toConsole: true);
                }
            }
            else    // Only print on the console the resolution
            {
                Logger.Instance.Write(
                    $"Resolution: {Anime.Resolution}\n", toConsole: true, toLog: false);
            }
        }

        private void Submitter(string[] args)
        {
            if (args.Length > 1) // Change submitter
            {
                _manager.ChangeSubmitter(args[1]);

                Logger.Instance.Write(
                    $"Submitter changed to {Anime.Submitter}!\n", toConsole: true);
            }
            else    // Only print on the console the submitter
            {
                Logger.Instance.Write(
                    $"Submitter: {Anime.Submitter}\n", toConsole: true, toLog: false);
            }
        }
    }
}