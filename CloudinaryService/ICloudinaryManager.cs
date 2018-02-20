using CloudinaryDotNet.Actions;
using System.Collections.Generic;

namespace CloudinaryService
{
    public interface ICloudinaryManager
    {
        List<string> GetImageUrlList(string publicId = null);

        List<string> GetFolderList();
        List<Resource> GetResourceList(string publicId = null);
    }
}
