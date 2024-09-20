using System.Reflection;
using System.Net;
using System.Diagnostics;

namespace YouTubeConverter
{
    class Program
    {
        static string outputDirectory;
        static async Task Main(string[] args)
        {
            Console.Clear();

            string currentVersion = "1.0.0"; // Current version of your app
            string versionUrl = "https://github.com/Hard2Find-dev/youtube-converter/version.txt"; // URL to check the latest version
            string updateUrl = "https://example.com/YourAppInstaller.exe";

            bool isFFmpegInstalled = await Converter.CheckFFmpegInstallation();

            if (!isFFmpegInstalled)
            {
                Console.WriteLine("FFmpeg is not installed. Please install FFmpeg from the following URL and then restart the application:");
                Console.WriteLine("https://ffmpeg.org/download.html");
            }

            string profileDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            outputDirectory = Path.Combine(profileDirectory, "Music");

            Assembly assembly = Assembly.GetExecutingAssembly();
            string resource = "youtube-converter.Text.txt";


            while (true)
            {

                using (Stream stream = assembly.GetManifestResourceStream(resource))
                using (StreamReader reader = new StreamReader(stream))
                {
                    string content = reader.ReadToEnd();
                    Console.WriteLine(content);  // Output the content of the file
                }
                Console.Write($"Download Folder: {outputDirectory}\n");
                Console.Write($"Hard2Find Development Company 2024\n\n");

                Console.Write("[1] Download Youtube Video MP3\n");
                Console.Write("[2] Download Youtube Video MP4\n");
                Console.Write("[3] Edit Metadata For MP3\n");
                Console.Write("[4] Exit\n\n");

                Console.Write(">> ");
                string input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        Console.Write("Enter YouTube video or playlist URL: ");
                        string videoUrl = Console.ReadLine();
                        await Downloader.DownloadYouTubeVideoAsMP3(videoUrl, outputDirectory);
                        Console.WriteLine("Download completed!");
                        break;

                    case "2":
                        Console.Write("Enter YouTube video or playlist URL: ");
                        string videoUrlMp4 = Console.ReadLine();
                        await Downloader.DownloadYouTubeVideoAsMP4(videoUrlMp4, outputDirectory);
                        Console.WriteLine("Download completed!");
                        break;

                    case "3":
                        Console.WriteLine("Enter metadata details");
                        Console.Write("Name: ");
                        string name = Console.ReadLine();
                        Console.Write("Artist: ");
                        string artist = Console.ReadLine();
                        Console.Write("Genre: ");
                        string genre = Console.ReadLine();
                        Console.Write("Year: ");
                        int year = Convert.ToInt32(Console.ReadLine());
                        Console.Write("Album: ");
                        string album = Console.ReadLine();
                        Console.Write("Bitrate: ");
                        int bitrate = Convert.ToInt32(Console.ReadLine());

                        Metadata metadata = new(name, artist, genre, year, album, bitrate);

                        Console.Write("Enter audio file to edit metadata: ");
                        string audioFilePath = Console.ReadLine();

                        string audioFile = $"{outputDirectory}{audioFilePath}";
                        if (File.Exists(audioFile))
                        {
                            Console.WriteLine($"Editing metadata for file: {audioFile}");
                            await Metadata.EditMetadataAndConvert(audioFile, metadata, ".mp3");
                        }
                        else
                        {
                            Console.WriteLine("File not found. Please enter a valid file path.");
                        }
                        break;
                    case "4":
                        return;
                    default:
                        Console.Clear();
                        Console.WriteLine("Invalid input. Please enter a valid option.");
                        break;
                }
            }
        }
        void CheckUpdate()
        {
            string currentVersion = "1.0.0"; // Current version of your app
            string versionUrl = "https://github.com/Hard2Find-dev/youtube-converter/version.txt"; // URL to check the latest version
            string updateUrl = "https://example.com/YourAppInstaller.exe"; // URL for the new version installer

            using (var client = new WebClient())
            {
                try
                {
                    // Fetch the latest version from the server
                    string latestVersion = client.DownloadString(versionUrl).Trim();

                    // Compare the versions
                    if (latestVersion != currentVersion)
                    {
                        Console.WriteLine($"New version available: {latestVersion}. Downloading update...");

                        // Download the new installer
                        string installerPath = Path.Combine(Path.GetTempPath(), "YourAppInstaller.exe");
                        client.DownloadFile(updateUrl, installerPath);

                        // Run the installer (this will replace the old version)
                        Process.Start(installerPath);

                        // Optionally close the current application after triggering the update
                        Environment.Exit(0);
                    }
                    else
                    {
                        Console.WriteLine("Your app is up-to-date.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error checking for updates: {ex.Message}");
                }
            }
        }
    }
}