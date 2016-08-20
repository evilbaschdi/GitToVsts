namespace GitToVsts.Internal.TeamServices
{
    /// <summary>
    ///     Interface for migration configuration.
    /// </summary>
    public interface IMigrationConfiguration
    {
        /// <summary>
        ///     Template to create with.
        /// </summary>
        string VsTemplate { get; set; }

        /// <summary>
        ///     Project to create in.
        /// </summary>
        string VsProject { get; set; }
    }
}