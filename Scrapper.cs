using System;
using System.Collections.Generic;
using ScrapySharp.Extensions;
using ScrapySharp.Network;
using System.Text.RegularExpressions;

namespace AnimeDownloader
{
    public class Scrapper
    {
        private ScrapingBrowser _browser = new ScrapingBrowser();
        private string _url;

        public string Submitter { get; private set; }

        public Scrapper(string author = "Erai-raws")
        {
            Submitter = author;
            _url = $"https://nyaa.si/?f=0&c=0_0&q=";
        }

        public bool SetTorrentURL(List<Anime> animes) 
        {
            bool isEverythingCorrect = true;

            foreach (var anime in animes)
            {
                isEverythingCorrect = SetTorrentURL(anime) && isEverythingCorrect;
            }
            
            return isEverythingCorrect;
        }

        public bool SetTorrentURL(Anime anime) 
        {
            WebPage web;

            try
            {
                Submitter = Submitter == "" ? Submitter : Submitter + "+";
                string url = _url+Submitter+anime.Title.Replace(" ", "+")+"+"+
                $"\"{anime.Episode} \""+"+"+Anime.Resolution;
                web = _browser.NavigateToPage(new Uri(url));
            }
            catch (AggregateException)
            {
                Logger.Instance.Write("Unknown Author.\n", toConsole: true, toLog: false);
                return false;
            }
            
            var rows = web.Html.CssSelect("tr");
            foreach (var row in rows)
            {
                if (!row.Attributes.Contains("class")) continue;
                if (row.Attributes["class"].Value.Contains("success") || row.Attributes["class"].Value.Contains("default"))
                {
                    string rowTitle = row.ChildNodes[3].LastChild.PreviousSibling.Attributes["title"].Value;
                    bool matchTitle = Regex.Match(rowTitle, $"\\s0{{0,2}}{anime.Episode}\\s").Success;
                    string rowSeeds = row.ChildNodes[11].InnerText;
                    bool matchSeeds = Regex.Match(rowSeeds, "[^0]").Success;
                    string rowSize = row.ChildNodes[7].InnerText;
                    bool matchSize = Regex.Match(rowSize, "^[0-1].\\d GiB|\\d+.\\d MiB").Success;

                    if (matchSeeds && matchSize && matchTitle)
                    {
                        var links = row.CssSelect("a");
                        if (links == null) continue;
                        foreach (var link in links)
                        {
                            if (link.Attributes["href"].Value.Contains("magnet:"))
                            {
                                anime.TorrentUrl = link.Attributes["href"].Value;
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
    }
}