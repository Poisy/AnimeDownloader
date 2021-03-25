// Using WebTorrent to download torrents
const WebTorrent = require('webtorrent')
const fs = require('fs')

const client = new WebTorrent()

require('events').EventEmitter.prototype._maxListeners = 100;

// Getting all animes from a argument as a JSON
const animes = JSON.parse(process.argv[2].replace(/_/g, ' '))
var size = 0;
var torrentCounter = 0;

animes.forEach(anime => {
  var x = true
  client.torrents.forEach(t => {
    x = t.magnetURI != anime.TorrentUrl && x
  })
  if (x) {
    process.stdout.write(`\nDownloading ${anime.Title} Episode ${anime.Episode} ...\n`)
    client.add(anime.TorrentUrl, { path: "./Anime/" }, function (torrent) {
      torrentCounter++;
      if (!fs.existsSync("./Anime/" + torrent.name))
      {
        size += torrent.length;
        torrent.on('done', function () {
          process.stdout.write(`\n${torrent.name} Downloaded!\n`)
          torrentCounter--;
          if (torrentCounter == 0) process.exit(0);
        })
        torrent.on('download', function (bytes) {
          if (!torrent.done)
          {
            process.stdout.write("\r\x1b[K")
            process.stdout.write(`Downloaded ${Math.round(client.progress*100)}% <=>` +
            `Speed ${Math.round(client.downloadSpeed/100000)/10}MB/s <=> Size ${Math.round(size/100000000)/10} GB`)
          }
        })
        torrent.on('error', function(err) {
          process.stdout.write('\n'+err+'\n');
        })
      }
      else
      {
        torrent.destroy();
        process.stdout.write('\n'+"Downloading Error: The episode is already downloaded."+'\n');
        torrentCounter--;
        if (torrentCounter == 0) process.exit(0);
      }
    })
  }
})