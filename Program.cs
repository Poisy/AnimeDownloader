namespace AnimeDownloader
{
    class Program
    {
        static void Main(string[] args)
        {
            Startup startup = new Startup();

            startup.Start(args);
        }
    }
}
