using PlexDancer;
using System.Text.RegularExpressions;

try
{
    FileHandler.RenameToPlexStandard(
        inputDirectory: @"P:\Yu-Gi-Oh! VRAINS",
        fileNameRegex: new Regex("(Yu-Gi-Oh! VRAINS Episode )(\\d{1,3})(.*)"),
        newPrefix: "Yu-Gi-Oh! VRAINS ",
        seasonEpisodeCount: new[] { 46, 56, 18 },
        fileExtension: "mkv",
        dryRun: false);
}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred: {ex}");
}
