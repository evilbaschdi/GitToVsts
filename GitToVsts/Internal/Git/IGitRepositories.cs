using System.Collections.Generic;
using EvilBaschdi.Core.DotNetExtensions;
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