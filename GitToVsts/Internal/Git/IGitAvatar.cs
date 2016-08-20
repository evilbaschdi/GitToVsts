using System.Windows.Media.Imaging;
using GitToVsts.Internal.Models;

namespace GitToVsts.Internal.Git
{
    /// <summary>
    ///     Interfcae for classes that extract the a git avatar by GitUser.
    /// </summary>
    public interface IGitAvatar
    {
        /// <summary>
        ///     Contains a BitmapImage for given GitUser.
        /// </summary>
        /// <param name="gitUser">GitUser to extract avatar for.</param>
        /// <returns></returns>
        BitmapImage For(GitUser gitUser);
    }
}