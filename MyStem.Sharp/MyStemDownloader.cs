using System;
using System.IO;
using System.IO.Compression;
using System.Net;

namespace MyStem.Sharp
{
    public class MyStemDownloader
    {
        private readonly Uri _url;
        private readonly string _baseFolder;

        public MyStemDownloader(string url = @"http://download.cdn.yandex.net/mystem/mystem-3.0-win7-64bit.zip")
        {
            _url = new Uri(url, UriKind.Absolute);
            _baseFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        }

        public string GetLocalPath()
        {
            var directory = Path.Combine(_baseFolder, "mystem");
            if (Directory.Exists(directory) == false)
                Directory.CreateDirectory(directory);

            var myStemExePath = Path.Combine(directory, "mystem.exe");
            if (File.Exists(myStemExePath))
                return myStemExePath;

            var myStemZipPath = Path.Combine(directory, "mystem.zip");
            using (var webClient = new WebClient())
            {
                webClient.DownloadFile(_url, myStemZipPath);
                ZipFile.ExtractToDirectory(myStemZipPath, directory);
                File.Delete(myStemZipPath);
            }

            return myStemExePath;
        }
    }
}