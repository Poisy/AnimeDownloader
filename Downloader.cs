using System;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;

namespace AnimeDownloader
{
    public class Downloader
    {
        public string DownloadPath { get; set; }

        public void DownloadAll(List<Anime> animes)
        {
            var json = JsonConvert.SerializeObject(animes);
            json = JsonConvert.ToString(json.Replace(" ", "_"));
            Process webTorrentProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "node",
                    Arguments = "./bin/webtorrent/start " + json,
                    UseShellExecute = false
                }
            };
            
            try
            {
                if (webTorrentProcess.Start())
                {
                    Logger.Instance.Write("Start Downloading!", toConsole: true, toLog: false);
                    string text = "";
                    foreach (var anime in animes)
                    {
                        text += $"([Title: {anime.Title}][Episode: {anime.Episode}]"+
                        $"[Submitter: {Anime.Submitter}][Resolution: {Anime.Resolution}]),";
                    }
                    Logger.Instance.Write("Downloading => " + text);
                }

                webTorrentProcess.WaitForExit();
            }
            catch (Exception)
            {
                Logger.Instance.Write("Something went wrong. Make sure you do not rename or move file and folders!",
                    toConsole: true);
            }
        }
    }
}
