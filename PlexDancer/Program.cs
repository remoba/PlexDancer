namespace ConsoleApp2;

using System.Text.RegularExpressions;

public class Program
{
    public static void Main(string[] args)
    {
        string folderPath = @"P:\Yu-Gi-Oh! VRAINS";

        Regex regex = new("(Yu-Gi-Oh! VRAINS Episode )(\\d{1,3})(.*)");
        string newPrefix = "Yu-Gi-Oh! VRAINS ";
        List<int> episodeCounts = new() { 46, 56, 18 };
        string fileExtension = "mkv";
        bool dryRun = false;

        try
        {
            string[] files = Directory.GetFiles(folderPath, $"*.{fileExtension}");

            int index = 0;

            foreach (string filePath in files)
            {
                index++;

                string fileName = Path.GetFileName(filePath);

                Match match = regex.Match(fileName);

                if (!match.Success)
                {
                    throw new Exception($"{fileName} didn't match regex");
                }

                Episode episode = new(
                    Prefix: match.Groups[1].Value,
                    AbsoluteNumber: int.Parse(match.Groups[2].Value),
                    Suffix: match.Groups[3].Value);

                (int season, int localEpisodeNumber) = LocalizeEpisodeNumber(episode.AbsoluteNumber, episodeCounts);

                string localizedEpisodeIndex = $"S{season:00}E{localEpisodeNumber:000}";

                string newFileName = $"{newPrefix}{localizedEpisodeIndex}{episode.Suffix}";

                string newFilePath = Path.Combine(folderPath, newFileName);

                if (newFilePath == filePath)
                {
                    Console.WriteLine($"Skipped: {Path.GetFileName(filePath)} {index} / {files.Length}");
                    continue;
                }
                
                if (!dryRun)
                {
                    File.Move(filePath, newFilePath);
                }

                Console.WriteLine($"Renamed: {Path.GetFileName(filePath)} -> {Path.GetFileName(newFilePath)} {index} / {files.Length}");
            }

            Console.WriteLine("Renaming completed.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex}");
        }
    }

    private static (int Season, int LocalNumber) LocalizeEpisodeNumber(int absoluteNumber, List<int> episodeCounts)
    {
        int season = 1;
        int localNumber = absoluteNumber;

        while (localNumber > episodeCounts[season - 1])
        {
            localNumber -= episodeCounts[season - 1];
            season++;
        }

        return (Season: season, LocalNumber: localNumber);
    }

    private record Episode(string Prefix, int AbsoluteNumber, string Suffix);
}