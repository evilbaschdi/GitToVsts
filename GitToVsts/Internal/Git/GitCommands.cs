namespace GitToVsts.Internal.Git
{
    public class GitCommands : IGitCommands
    {
        public string Clone { get; } = "clone --mirror";

        public string Config { get; } = "config --local --bool core.bare false";

        public string Reset { get; } = "reset --hard HEAD";

        public string RemoteAdd { get; } = "remote add vsts";

        public string PushAll { get; } = "push --all vsts";

        public string PushTags { get; } = "push --tags vsts";
    }
}