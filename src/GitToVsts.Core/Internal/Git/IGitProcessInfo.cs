using System.Diagnostics;

namespace GitToVsts.Core.Internal.Git;

/// <summary>
///     Interface for ProcessStartInfo of git.exe.
/// </summary>
public interface IGitProcessInfo : IValue<ProcessStartInfo>;