using System.Collections.Generic;
using EvilBaschdi.Core;
using GitToVsts.Model;

namespace GitToVsts.Internal.Git
{
    /// <summary>
    ///     Interface for classes that return a list of GitRepositories
    /// </summary>
    public interface IGitRepositories : IValue<List<GitRepository>>
    {
    }
}