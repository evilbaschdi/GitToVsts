namespace GitToVsts.Model
{
    /// <summary>
    ///     Class for migration configuration.
    /// </summary>
    public class Configuration : IMigrationConfiguration
    {
        /// <summary>
        ///     Template to create with.
        /// </summary>
        public string VsTemplate { get; set; }

        /// <summary>
        ///     Project to create in.
        /// </summary>
        public string VsProject { get; set; }
    }
}