using System.Windows.Media.Imaging;
using GitToVsts.Internal.Models;

namespace GitToVsts.Internal.Git
{
    public interface IGitAvatar
    {
        BitmapImage For(GitUser gitUser);
    }
}