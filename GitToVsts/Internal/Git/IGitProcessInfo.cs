using System.Diagnostics;
using EvilBaschdi.Core.DotNetExtensions;

namespace GitToVsts.Internal.Git
{
    /// <summary>
    ///     Interface for ProcessStartInfo of git.exe.
    /// </summary>
    public interface IGitProcessInfo : IValue<ProcessStartInfo>
    {
    }
}