using System.Diagnostics;
using EvilBaschdi.Core;

namespace GitToVsts.Internal.Git;

/// <summary>
///     Interface for ProcessStartInfo of git.exe.
/// </summary>
public interface IGitProcessInfo : IValue<ProcessStartInfo>
{
}