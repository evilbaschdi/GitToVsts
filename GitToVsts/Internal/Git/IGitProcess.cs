namespace GitToVsts.Internal.Git
{
    public interface IGitProcess
    {
        void Run(string arguments, string directory);
    }
}