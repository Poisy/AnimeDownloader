using System;

namespace AnimeDownloader
{
    public class Logger
    {
        private static Logger _logger;
        private FileManager _fileManager;
        private string _text;

        private Logger()
        {   
            _fileManager = new FileManager();
            _text = _fileManager.ReadFile(_fileManager.LogPath);
        }
        
        public static Logger Instance
        {
            get {
                if (_logger == null) _logger = new Logger();

                return _logger;        	
            }
        }

        public void Write(string text, bool toConsole = false, bool toLog = true)
        {
            if (toConsole)
            {
                System.Console.Write(text);
            }

            if (toLog)
            {
                string date = DateTime.Now.ToString("MM/dd/yyyy HH:mm");
                _text += $"---[{date}]--->   {text}";

                _fileManager.WriteFile(_text, _fileManager.LogPath);
            }
            
        }
    }
}
