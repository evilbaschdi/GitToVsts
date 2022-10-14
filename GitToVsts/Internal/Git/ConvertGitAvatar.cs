using System.IO;
using System.Net.Http;
using System.Windows.Media.Imaging;
using GitToVsts.Model;

namespace GitToVsts.Internal.Git;

/// <summary>
///     Class that extracts the a git avatar by GitUser.
/// </summary>
public class ConvertGitAvatar : IGitAvatar
{
    /// <summary>
    ///     Contains a BitmapImage for given GitUser.
    /// </summary>
    /// <param name="gitUser">GitUser to extract avatar for.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"><paramref name="gitUser" /> is <see langword="null" />.</exception>
    public async Task<BitmapImage> ValueFor(GitUser gitUser)
    {
        if (gitUser == null)
        {
            throw new ArgumentNullException(nameof(gitUser));
        }

        var image = new BitmapImage();

        if (string.IsNullOrWhiteSpace(gitUser.Avatar_Url))
        {
            return image;
        }

        var pictureUri = new Uri(gitUser.Avatar_Url, UriKind.Absolute);

        using var httpClient = new HttpClient();
        var imageBytes = await httpClient.GetByteArrayAsync(pictureUri);

        image.BeginInit();
        image.StreamSource = new MemoryStream(imageBytes);

        image.EndInit();
        return image;
    }
}