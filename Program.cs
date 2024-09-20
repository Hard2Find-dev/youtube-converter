using System.Runtime.InteropServices;

namespace YouTubeConverter
{
    class Program
    {
        static string outputDirectory; 
        static readonly string filePath = "./ascii-Art.txt";
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


            string directory = AppDomain.CurrentDomain.BaseDirectory;

            string fullPath = Path.Combine(directory, filePath);
            string fileContent = File.ReadAllText(fullPath);

            while (true)
            {

                Console.Write(fileContent);
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
    }
}