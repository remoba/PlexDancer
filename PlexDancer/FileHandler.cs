namespace PlexDancer;

using System.Text.RegularExpressions;

public static class FileHandler
{
    /// <summary>
    /// Renames episode files in a given directory from a given naming format to a Plex-supported one by localizing absolute episode numbers.
    /// </summary>
    /// <param name="inputDirectory">The directory that contains the files to change.</param>
    /// <param name="fileNameRegex">The regex to capture the file name's prefix, absolute episode number, and suffix.</param>
    /// <param name="newPrefix">The prefix to set for the renamed files.</param>
    /// <param name="seasonEpisodeCount">The number of episodes per season (index 0 is treated as season 1). Should match TVDB, for instance https://thetvdb.com/series/yu-gi-oh-vrains#seasons.</param>
    /// <param name="fileExtension">The extension of the episode files.</param>
    /// <param name="dryRun">If true will only print out the renames and not actually make them. Recommended to set this to true first to test the inputs.</param>
    public static void RenameToPlexStandard(
        string inputDirectory,
        Regex fileNameRegex,
        string newPrefix,
        int[] seasonEpisodeCount,
        string fileExtension,
        bool dryRun)
    {
        string[] files = Directory.GetFiles(inputDirectory, $"*.{fileExtension}");

        int index = 0;

        foreach (string filePath in files)
        {
            index++;

            string fileName = Path.GetFileName(filePath);

            Match match = fileNameRegex.Match(fileName);

            if (!match.Success)
            {
                throw new Exception($"{fileName} didn't match regex");
            }

            Episode episode = new(
                Prefix: match.Groups[1].Value,
                AbsoluteNumber: int.Parse(match.Groups[2].Value),
                Suffix: match.Groups[3].Value);

            (int season, int localEpisodeNumber) = LocalizeEpisodeNumber(episode.AbsoluteNumber, seasonEpisodeCount);

            string localizedEpisodeIndex = $"S{season:00}E{localEpisodeNumber:000}";

            string newFileName = $"{newPrefix}{localizedEpisodeIndex}{episode.Suffix}";

            string newFilePath = Path.Combine(inputDirectory, newFileName);

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

    private static (int Season, int LocalNumber) LocalizeEpisodeNumber(int absoluteNumber, int[] seasonEpisodeCount)
    {
        int season = 1;
        int localNumber = absoluteNumber;

        while (localNumber > seasonEpisodeCount[season - 1])
        {
            localNumber -= seasonEpisodeCount[season - 1];
            season++;
        }

        return (Season: season, LocalNumber: localNumber);
    }

    private record Episode(string Prefix, int AbsoluteNumber, string Suffix);
}
