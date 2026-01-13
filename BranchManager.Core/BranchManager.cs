using LibGit2Sharp;

namespace BranchManager.Core;

public record Branch(DirectoryInfo DirInfo, string LocalName, string? RemoteName, string LatestCommitMessage);

public class CannotDeleteBranch(string message, Exception ex) : Exception(message, ex)
{
}

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

    public static void DeleteBranches(List<Branch> branches, bool ignoreBranchDeletionExceptions = false)
    {
        foreach (var branch in branches)
        {
            try
            {
                using var repo = new Repository(branch.DirInfo.FullName);
                repo.Branches.Remove(branch.LocalName);
            }
            catch (LibGit2SharpException ex)
            {
                if (ex.Message.StartsWith($"cannot delete branch"))
                {
                    if (ignoreBranchDeletionExceptions) 
                        continue;

                    throw new CannotDeleteBranch($"Cannot delete branch {branch}", ex);
                }

                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
