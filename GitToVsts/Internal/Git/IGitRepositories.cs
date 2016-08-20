using System.Collections.Generic;
using GitToVsts.Core;
using GitToVsts.Internal.Models;

namespace GitToVsts.Internal.Git
{
    /// <summary>
    ///     Interface for classes that return a list of GitRepositories
    /// </summary>
    public interface IGitRepositories : IValue<List<GitRepository>>
    {
    }
}