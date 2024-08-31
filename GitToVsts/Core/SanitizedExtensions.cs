using System.IO;

namespace GitToVsts.Core;

/// <summary>
/// </summary>
public static class SanitizedExtensions
{
    /// <summary>
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static bool SanitizedDirectoryExists(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            throw new ArgumentNullException(nameof(path));
        }

        try
        {
            string canonicalPath = Path.GetFullPath(path);
            return Directory.Exists(canonicalPath);
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static bool SanitizedFileExists(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            throw new ArgumentNullException(nameof(path));
        }

        try
        {
            string canonicalPath = Path.GetFullPath(path);
            return File.Exists(canonicalPath);
        }
        catch (Exception)
        {
            return false;
        }
    }
}