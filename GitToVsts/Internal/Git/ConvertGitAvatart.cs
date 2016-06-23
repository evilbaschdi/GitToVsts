using System;
using System.IO;
using System.Net;
using System.Windows.Media.Imaging;
using GitToVsts.Internal.Models;

namespace GitToVsts.Internal.Git
{
    public class ConvertGitAvatart : IGitAvatar
    {
        public BitmapImage For(GitUser gitUser)
        {
            var image = new BitmapImage();
            int BytesToRead = 100;

            if (!string.IsNullOrWhiteSpace(gitUser.Avatar_Url))
            {
                var pictureUri = new Uri(gitUser.Avatar_Url, UriKind.Absolute);
                WebRequest request = WebRequest.Create(pictureUri);
                request.Timeout = -1;
                WebResponse response = request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                if (responseStream != null)
                {
                    BinaryReader reader = new BinaryReader(responseStream);
                    MemoryStream memoryStream = new MemoryStream();

                    byte[] bytebuffer = new byte[BytesToRead];
                    int bytesRead = reader.Read(bytebuffer, 0, BytesToRead);

                    while (bytesRead > 0)
                    {
                        memoryStream.Write(bytebuffer, 0, bytesRead);
                        bytesRead = reader.Read(bytebuffer, 0, BytesToRead);
                    }

                    image.BeginInit();
                    memoryStream.Seek(0, SeekOrigin.Begin);

                    image.StreamSource = memoryStream;
                }
                image.EndInit();
            }
            return image;
        }
    }
}