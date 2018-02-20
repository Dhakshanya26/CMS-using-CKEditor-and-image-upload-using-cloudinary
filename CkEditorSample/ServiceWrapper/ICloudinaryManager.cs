using CloudinaryDotNet.Actions;
using System.Collections.Generic;
using System.IO;

namespace CkeditorSample.ServiceWrapper
{
    public interface ICloudinaryManager
    {
        List<string> GetImageUrlList(string publicId = null);
        List<Folder> GetFolderList();
        List<Resource> GetResourceList(string publicId = null);

        List<string> GetImageUrlListByPrefix(string prefix = null, string type = "upload");

        bool UploadImage(string fileName, MemoryStream fileStream, int height, int width, int radius, string publicId = null, string folderName = null, string optionalName = null, bool OptionalBoolParameter = false,
                                       bool uniqueFilename = false, bool useFilename = true, string type = "upload");

    }
}
