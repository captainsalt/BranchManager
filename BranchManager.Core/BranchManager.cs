using LibGit2Sharp;

namespace BranchManager.Core;

public record Branch(DirectoryInfo DirInfo, string LocalName, string? RemoteName, string LatestCommitMessage);

public static class BranchManager
{
    public static List<Branch> GetBranches(DirectoryInfo rootDir)
    {
        var validRepositories = rootDir.EnumerateDirectories()
            .Where(d => Repository.IsValid(d.FullName));

        var branches = new List<Branch>();

        foreach (var dirInfo in validRepositories)
        {
            using var repo = new Repository(dirInfo.FullName);
            var ignoredBranches = new List<string>(["main", "master"]);

            var repoBranches = repo.Branches
                .Where(b => !ignoredBranches.Contains(b.FriendlyName))
                .Where(b => !b.IsRemote)
                .Select(b => new Branch(dirInfo, b.FriendlyName, b.RemoteName, b.Commits.First().MessageShort))
                .ToList();

            branches.AddRange(repoBranches);
        }

        return branches;
    }

    public static void DeleteBranches(List<Branch> branches)
    {
        foreach (var branch in branches)
        {
            using var repo = new Repository(branch.DirInfo.FullName);
            var mainBranch = repo.Branches["master"]
                ?? repo.Branches["main"]
                ?? throw new Exception("Could not switch to main/master branch");

            // TODO: Do this once per repository
            Commands.Checkout(repo, mainBranch);
            repo.Branches.Remove(branch.LocalName);
        }
    }
}
