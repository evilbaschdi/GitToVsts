using EvilBaschdi.Core.DotNetExtensions;
using GitToVsts.Model;

namespace GitToVsts.Internal.Git
{
    /// <summary>
    ///     Interface for classes that returns a GitUser.
    /// </summary>
    public interface IGitUser : IValue<GitUser>
    {
    }
}