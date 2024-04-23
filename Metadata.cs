using System.Diagnostics;

namespace YouTubeConverter
{
    public class Metadata
    {
        public string Name { get; set; }
        public string Artist { get; set; }
        public string Genre { get; set; }
        public int Year { get; set; }
        public string Album { get; set; }
        public int Bitrate { get; set; }

        public Metadata(string name, string artist, string genre, int year, string album, int bitrate)
        {
            Name = name;
            Artist = artist;
            Genre = genre;
            Year = year;
            Album = album;
            Bitrate = bitrate;
        }
        public static async Task EditMetadataAndConvert(string audioFilePath, Metadata metadata, string outputFormat)
        {
            try
            {
                if (!File.Exists(audioFilePath))
                {
                    throw new ArgumentException("Audio file not found.", nameof(audioFilePath));
                }
                string arguments = $"-i \"{audioFilePath}\" " +
                                   $"-metadata title=\"{metadata.Name}\" " +
                                   $"-metadata artist=\"{metadata.Artist}\" " +
                                   $"-metadata genre=\"{metadata.Genre}\" " +
                                   $"-metadata date=\"{metadata.Year}\" " +
                                   $"-metadata album=\"{metadata.Album}\" " +
                                   $"-b:a {metadata.Bitrate}k " +
                                   $"\"{audioFilePath}.mp3\"";

                Process process = new();

                process.StartInfo.FileName = "ffmpeg";
                process.StartInfo.Arguments = arguments;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;

                process.Start();
                process.WaitForExit();

                await process.WaitForExitAsync();

                Console.WriteLine("Metadata editing and conversion completed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

    }
}