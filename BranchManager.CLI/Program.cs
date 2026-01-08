using BranchManager.Core;
using Spectre.Console;
using Core = BranchManager.Core;
using System.IO;

internal class Program
{
    private static void Main(string[] args)
    {
        var repositories = Core.BranchManager.GetBranches(new DirectoryInfo(GetDirectoryPath()));
        var prompt = GeneratePrompt(repositories);
        var selected = AnsiConsole.Prompt(prompt);

        Core.BranchManager.DeleteBranches(selected);
    }

    static MultiSelectionPrompt<Branch> GeneratePrompt(List<Branch> branches)
    {
        var padding = branches.Max(b => b.DirInfo.Name.Length);
        var prompt = new MultiSelectionPrompt<Branch>()
            .Title("Select branches to delete")
            .UseConverter(b => $"{b.DirInfo.Name.PadRight(padding)} -> {b.LocalName} \"{b.LatestCommitMessage}\"");

        prompt.AddChoices(branches);
        return prompt;
    }

    static string GetDirectoryPath()
    {
        var configFile = "config.txt";
        if (!File.Exists(configFile)) File.Create(configFile).Dispose();

        var storedPath = File.ReadAllText(configFile).Trim();

        if (Directory.Exists(storedPath))
            return storedPath;

        var desiredPath = AnsiConsole.Ask<string>("What's the root directory to look in?");
        File.WriteAllText(configFile, desiredPath);
        return File.ReadAllText(configFile).Trim();
    }
}