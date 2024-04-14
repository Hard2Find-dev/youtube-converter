
using YoutubeExplode;

namespace YouTubeConverter
{
    class Downloader
    {
        public static async Task DownloadYouTubePlaylistOrVideo(string url, string outputDirectory)
        {
            var youtube = new YoutubeClient();

            if (url.Contains("list="))
            {
                var playlistId = url.Split("list=").Last();
                var playlist = await youtube.Playlists.GetAsync(playlistId);
                Console.WriteLine($"\nDownloading From {playlist.Title} Playlist");

                await foreach (var video in youtube.Playlists.GetVideosAsync(playlistId))
                {
                    await DownloadYouTubeVideo(video.Url, outputDirectory);
                }

                Console.WriteLine("Playlist download completed!");
            }
            else
            {
                var video = await youtube.Videos.GetAsync(url);
                Console.WriteLine($"Downloading video: {video.Title}");
                await DownloadYouTubeVideo(url, outputDirectory);
                Console.WriteLine("Video download completed!");
            }
        }
        public static async Task DownloadYouTubeVideo(string videoUrl, string outputDirectory)
        {
            var youtube = new YoutubeClient();
            var video = await youtube.Videos.GetAsync(videoUrl);


            string sanitizedTitle = string.Join("_", video.Title.Split(Path.GetInvalidFileNameChars()));

            Console.Write($"\nDownloading {sanitizedTitle}");
            var streamManifest = await youtube.Videos.Streams.GetManifestAsync(video.Id);
            var muxedStreams = streamManifest.GetMuxedStreams().OrderByDescending(s => s.VideoQuality).ToList();

            if (muxedStreams.Any())
            {
                var streamInfo = muxedStreams.First();
                using var httpClient = new HttpClient();
                var stream = await httpClient.GetStreamAsync(streamInfo.Url);

                string outputFilePath = Path.Combine(outputDirectory, $"{sanitizedTitle}.{streamInfo.Container}");
                using var outputStream = File.Create(outputFilePath);
                await stream.CopyToAsync(outputStream);

                Console.WriteLine("\nDownload completed!");
                Console.WriteLine($"Video saved as: {outputFilePath}");

                await Converter.ConvertMp4ToMp3(outputFilePath, outputDirectory);
            }
            else
            {
                Console.WriteLine($"No suitable video stream found for {video.Title}.");
            }
        }
    }
}