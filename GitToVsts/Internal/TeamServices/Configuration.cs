namespace GitToVsts.Internal.TeamServices
{
    public class Configuration : IMigrationConfiguration
    {
        public string VsTemplate { get; set; }

        public string VsProject { get; set; }
    }
}