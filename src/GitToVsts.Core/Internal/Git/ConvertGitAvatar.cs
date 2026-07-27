namespace GitToVsts.Core.Internal.Git;

/// <summary>
///     Class that extracts the avatar for a git user.
/// </summary>
public class ConvertGitAvatar : IGitAvatar
{
    private readonly IHttpClientFactory _httpClientFactory;

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="httpClientFactory"></param>
    public ConvertGitAvatar(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    /// <summary>
    ///     Contains a byte array for given GitUser.
    /// </summary>
    /// <param name="gitUser">GitUser to extract avatar for.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"><paramref name="gitUser" /> is <see langword="null" />.</exception>
    public async Task<byte[]> ValueFor(GitUser gitUser)
    {
        ArgumentNullException.ThrowIfNull(gitUser);

        if (string.IsNullOrWhiteSpace(gitUser.Avatar_Url))
        {
            return Array.Empty<byte>();
        }

        var pictureUri = new Uri(gitUser.Avatar_Url, UriKind.Absolute);

        using var httpClient = _httpClientFactory.CreateClient();
        var imageBytes = await httpClient.GetByteArrayAsync(pictureUri).ConfigureAwait(false);

        return imageBytes;
    }
}