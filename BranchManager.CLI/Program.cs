using BranchManager.Core;
using Spectre.Console;
using Core = BranchManager.Core;

internal class Program
{
    private static void Main(string[] args)
    {
        var repositories = Core.BranchManager.GetBranches(new DirectoryInfo(""));
        var prompt = GeneratePrompt(repositories);
        var selected = AnsiConsole.Prompt(prompt);

        Core.BranchManager.DeleteBranches(selected);
    }

    static MultiSelectionPrompt<Branch> GeneratePrompt(List<Branch> branches)
    {
        var padding = branches.Max(b => b.DirInfo.Name.Length);
        var prompt = new MultiSelectionPrompt<Branch>()
            .Title("Select branches to delete")
            .UseConverter(b => $"{b.DirInfo.Name.PadRight(padding)} -> {b.LocalName}");

        prompt.AddChoices(branches);
        return prompt;
    }
}