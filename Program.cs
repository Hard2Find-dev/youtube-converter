using System.Runtime.InteropServices;

namespace YouTubeConverter
{
    class Program
    {
        static async Task Main(string[] args)
        {

            while (true)
            {
                string outputDirectory;
                bool isFFmpegInstalled = await Converter.CheckFFmpegInstallation();

                if (!isFFmpegInstalled)
                {
                    Console.WriteLine("FFmpeg is not installed. Please install FFmpeg from the following URL and then restart the application:");
                    Console.WriteLine("https://ffmpeg.org/download.html");
                }
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    string username = Environment.UserName;
                    outputDirectory = $"/home/{username}/Music/";
                    Console.WriteLine(outputDirectory);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    string profileDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                    outputDirectory = Path.Combine(profileDirectory, "Music");
                    Console.WriteLine(outputDirectory);
                }
                else
                {
                    throw new NotSupportedException("Unsupported operating system.");
                }

                Console.Write("Enter Music Single/Playlist URL (or type 'exit' to quit): ");
                string playlistUrl = Console.ReadLine();

                if (playlistUrl.Equals("exit", StringComparison.CurrentCultureIgnoreCase))
                    break;

                try
                {
                    await Downloader.DownloadYouTubePlaylistOrVideo(playlistUrl, outputDirectory);
                    Console.WriteLine("Download completed!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred while downloading the playlist: " + ex.Message);
                }
            }
        }
    }
}