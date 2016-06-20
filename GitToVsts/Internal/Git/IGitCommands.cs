namespace GitToVsts.Internal.Git
{
    public interface IGitCommands
    {
        string Clone { get; }
        string Config { get; }
        string Reset { get; }
        string RemoteAdd { get; }
        string PushAll { get; }
        string PushTags { get; }
    }
}