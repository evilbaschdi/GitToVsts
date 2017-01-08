using System.Windows.Media.Imaging;
using EvilBaschdi.Core.DotNetExtensions;
using GitToVsts.Model;

namespace GitToVsts.Internal.Git
{
    /// <summary>
    ///     Interfcae for classes that extract the a git avatar by GitUser.
    /// </summary>
    public interface IGitAvatar : IValueFor<GitUser, BitmapImage>
    {
    }
}