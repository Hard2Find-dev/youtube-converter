using System.Diagnostics;

namespace YouTubeConverter
{
    class Converter
    {
        public static async Task ConvertMp4ToMp3(string inputFilePath, string outputDirectory)
        {
            try
            {
                Console.WriteLine("Converting To MP3");
                int bitrate = 320;

                string outputFilePath = Path.Combine(outputDirectory, $"{Path.GetFileNameWithoutExtension(inputFilePath)}.mp3");
                string ffmpegCommand = $"-i \"{inputFilePath}\" -vn -b:a {bitrate}k -acodec libmp3lame \"{outputFilePath}\"";

                Process process = new Process();
                process.StartInfo.FileName = "ffmpeg";
                process.StartInfo.Arguments = ffmpegCommand;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;

                process.Start();  // Start the process before reading streams

                // Asynchronously read the output and error streams
                var outputTask = process.StandardOutput.ReadToEndAsync();
                var errorTask = process.StandardError.ReadToEndAsync();

                // Wait for FFmpeg to finish
                await process.WaitForExitAsync();

                // Ensure output/error streams are read completely
                string output = await outputTask;
                string error = await errorTask;

                Console.WriteLine("FFmpeg process completed.");
                Console.WriteLine($"FFmpeg Output: {output}");
                Console.WriteLine($"FFmpeg Error: {error}");

                if (process.ExitCode == 0)
                {
                    Console.WriteLine("Conversion completed!");
                    Console.WriteLine($"MP3 saved as: {outputFilePath}");
                    File.Delete(inputFilePath);
                    Console.WriteLine($"Deleted MP4 file: {inputFilePath}");
                }
                else
                {
                    Console.WriteLine("FFmpeg encountered an error during conversion.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during conversion: {ex.Message}");
            }
        }

        public static async Task<bool> CheckFFmpegInstallation()
        {
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = "ffmpeg";
                process.StartInfo.Arguments = "-version";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;

                process.Start();  // Start the process before reading streams

                string output = await process.StandardOutput.ReadToEndAsync();
                process.WaitForExit();

                return output.Contains("ffmpeg version");
            }
            catch
            {
                return false;
            }
        }
    }
}
