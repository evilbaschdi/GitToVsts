using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using EvilBaschdi.Core;
using GitToVsts.Model;

namespace GitToVsts.Internal.Git
{
    /// <summary>
    ///     Interface for classes that extract the a git avatar by GitUser.
    /// </summary>
    public interface IGitAvatar : IValueFor<GitUser, Task<BitmapImage>>
    {
    }
}