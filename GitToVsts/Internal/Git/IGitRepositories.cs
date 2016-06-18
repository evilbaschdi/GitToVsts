using System.Collections.Generic;
using GitToVsts.Core;
using GitToVsts.Internal.Models;

namespace GitToVsts.Internal.Git
{
    public interface IGitRepositories : IValue<List<GitRepository>>
    {
    }
}