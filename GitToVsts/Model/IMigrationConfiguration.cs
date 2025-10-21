namespace GitToVsts.Model;

/// <summary>
///     Interface for migration configuration.
/// </summary>
public interface IMigrationConfiguration
{
    /// <summary>
    ///     Project to create in.
    /// </summary>
    string DevOpsProject { get; }

    /// <summary>
    ///     Template to create with.
    /// </summary>
    string DevOpsTemplate { get; }
}