using BranchManager.Core;
using Spectre.Console;
using Core = BranchManager.Core;

internal class Program
{
    private static void Main(string[] args)
    {
        var repositories = Core.BranchManager.GetBranches(new DirectoryInfo(""));
        var multiSelection = GenerateMultiSelection(repositories);
        var selected = AnsiConsole.Prompt(multiSelection);

        Core.BranchManager.DeleteBranches(selected);
    }

    static MultiSelectionPrompt<Branch> GenerateMultiSelection(List<Branch> branches)
    {
        var padding = branches.Select(b => b.DirInfo.Name.Length).Max();
        var prompt = new MultiSelectionPrompt<Branch>()
            .Title("Select branches to delete")
            .UseConverter(b => $"{b.DirInfo.Name.PadRight(padding)} -> {b.LocalName}");

        prompt.AddChoices(branches);
        return prompt;
    }
}