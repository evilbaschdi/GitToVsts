namespace GitToVsts.Internal.Git
{
    public class GitCommands
    {
        public string Clone = "clone --mirror";
        public string Config = "config --local --bool core.bare false";
        public string Reset = "reset --hard HEAD";
        public string RemoteAdd = "remote add vsts";
        public string PushAll = "push --all vsts";
        public string PushTags = "push --tags vsts";
    }
}