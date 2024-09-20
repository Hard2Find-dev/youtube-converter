using System.Reflection;

namespace YouTubeConverter
{
    class Program
    {
        static string outputDirectory;
        static async Task Main(string[] args)
        {
            Console.Clear();



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
                await CheckUpdate();
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
        static async Task CheckUpdate()
        {
            string currentVersion = "1.0.0";
            string versionUrl = "https://raw.githubusercontent.com/Hard2Find-dev/youtube-converter/refs/heads/main/version.txt";
                               //https://raw.githubusercontent.com/Hard2Find-dev/youtube-converter/refs/heads/main/version.txt
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(versionUrl);
                    response.EnsureSuccessStatusCode();
                    string latestVersion = await response.Content.ReadAsStringAsync();

                    latestVersion = latestVersion.Trim();  // Trim to remove any extra spaces, newlines, etc.

                    // Compare versions
                    if (!string.Equals(currentVersion, latestVersion, StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine($"Version: {currentVersion}. New version available: {latestVersion}\n");
                    }
                    else
                    {
                        Console.WriteLine($"Version: {currentVersion} is up to date.\n");
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