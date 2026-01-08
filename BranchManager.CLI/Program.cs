using System.IO;
using BranchManager.Core;
using Spectre.Console;
using Core = BranchManager.Core;

internal class Program
{
    private const string CONFIG_FILE = "config.txt";

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
        if (!File.Exists(CONFIG_FILE)) File.Create(CONFIG_FILE).Dispose();

        var repositoryRootDirectory = File.ReadAllText(CONFIG_FILE).Trim();

        if (Directory.Exists(repositoryRootDirectory))
            return repositoryRootDirectory;

        repositoryRootDirectory = AnsiConsole.Ask<string>("Insert the path to the root directory of your repositories");
        File.WriteAllText(CONFIG_FILE, repositoryRootDirectory.Trim());
        return File.ReadAllText(CONFIG_FILE).Trim();
    }
}