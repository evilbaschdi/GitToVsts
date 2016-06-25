namespace GitToVsts.Internal.TeamServices
{
    public interface IMigrationConfiguration
    {
        string VsTemplate { get; set; }
        string VsProject { get; set; }
    }
}