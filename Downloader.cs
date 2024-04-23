
using YoutubeExplode;

namespace YouTubeConverter
{
    class Downloader
    {
        public static async Task DownloadYouTubeVideoAsMP3(string url, string outputDirectory)
        {
            var youtube = new YoutubeClient();

            if (url.Contains("list="))
            {
                var playlistId = url.Split(new[] { "list=", "&" }, StringSplitOptions.RemoveEmptyEntries)[1];

                var playlist = await youtube.Playlists.GetAsync(playlistId);

                await foreach (var video in youtube.Playlists.GetVideosAsync(playlistId))
                {
                    Console.WriteLine($"\nDownloading From {playlist.Title} Playlist");
                    await ConvertToMP3(video.Url, outputDirectory);
                }

                Console.WriteLine("Playlist download completed!");
            }
            else
            {
                var video = await youtube.Videos.GetAsync(url);
                Console.WriteLine($"Downloading video: {video.Title}");
                await ConvertToMP3(url, outputDirectory);
                Console.WriteLine("Video download completed!");
            }
        }
        public static async Task DownloadYouTubeVideoAsMP4(string url, string outputDirectory)
        {
            var youtube = new YoutubeClient();

            if (url.Contains("list="))
            {
                var playlistId = url.Split("list=").Last();
                var playlist = await youtube.Playlists.GetAsync(playlistId);


                await foreach (var video in youtube.Playlists.GetVideosAsync(playlistId))
                {
                    Console.WriteLine($"\nDownloading From {playlist.Title} Playlist");
                    await KeepAsMP4(video.Url, outputDirectory);
                }

                Console.WriteLine("Playlist download completed!");
            }
            else
            {
                var video = await youtube.Videos.GetAsync(url);
                Console.WriteLine($"Downloading video: {video.Title}");
                await KeepAsMP4(url, outputDirectory);
                Console.WriteLine("Video download completed!");
            }
        }
        public static async Task ConvertToMP3(string videoUrl, string outputDirectory)
        {
            var youtube = new YoutubeClient();
            var video = await youtube.Videos.GetAsync(videoUrl);

            string sanitizedTitle = string.Join("_", video.Title.Split(Path.GetInvalidFileNameChars()));

            Console.WriteLine($"\nDownloading {sanitizedTitle}");

            var streamManifest = await youtube.Videos.Streams.GetManifestAsync(video.Id);
            var muxedStreams = streamManifest.GetMuxedStreams().OrderByDescending(s => s.VideoQuality).ToList();

            if (muxedStreams.Any())
            {
                var streamInfo = muxedStreams.First();

                using var httpClient = new HttpClient();
                using var response = await httpClient.GetAsync(streamInfo.Url, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                string outputFilePath = Path.Combine(outputDirectory, $"{sanitizedTitle}.{streamInfo.Container}");

                using (var fileStream = File.Create(outputFilePath))
                {
                    using var stream = await response.Content.ReadAsStreamAsync();

                    byte[] buffer = new byte[8192];
                    int bytesRead;

                    while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        await fileStream.WriteAsync(buffer, 0, bytesRead);
                    }
                }

                Console.WriteLine("\nDownload completed!");
                Console.WriteLine($"Video saved as: {outputFilePath}");

                await Converter.ConvertMp4ToMp3(outputFilePath, outputDirectory);
            }
            else
            {
                Console.WriteLine($"No suitable video stream found for {video.Title}.");
            }
        }
        public static async Task KeepAsMP4(string videoUrl, string outputDirectory)
        {
            var youtube = new YoutubeClient();
            var video = await youtube.Videos.GetAsync(videoUrl);

            string sanitizedTitle = string.Join("_", video.Title.Split(Path.GetInvalidFileNameChars()));

            Console.WriteLine($"\nDownloading {sanitizedTitle}");

            var streamManifest = await youtube.Videos.Streams.GetManifestAsync(video.Id);
            var muxedStreams = streamManifest.GetMuxedStreams().OrderByDescending(s => s.VideoQuality).ToList();

            if (muxedStreams.Any())
            {
                var streamInfo = muxedStreams.First();

                using var httpClient = new HttpClient();
                using var response = await httpClient.GetAsync(streamInfo.Url, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                string outputFilePath = Path.Combine(outputDirectory, $"{sanitizedTitle}.{streamInfo.Container}");

                using (var fileStream = File.Create(outputFilePath))
                {
                    using var stream = await response.Content.ReadAsStreamAsync();

                    byte[] buffer = new byte[8192];
                    int bytesRead;

                    while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        await fileStream.WriteAsync(buffer, 0, bytesRead);
                    }
                }

                Console.WriteLine("\nDownload completed!");
                Console.WriteLine($"Video saved as: {outputFilePath}");
            }
            else
            {
                Console.WriteLine($"No suitable video stream found for {video.Title}.");
            }
        }
    }
}