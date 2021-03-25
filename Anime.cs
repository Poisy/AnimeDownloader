namespace AnimeDownloader
{
    public class Anime
    {
        public string Title { get; set; }
        public int Episode { get; set; }
        public string TorrentUrl { get; set; }
        public static int Resolution { get; set; } = 1080;
        public static string Submitter { get; set; } = "Erai-Raws";
    }
}