using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CkeditorSample.ServiceWrapper
{
  
    public class CloudinaryManager : ICloudinaryManager
    {
        private readonly Cloudinary _cloudinary;
        public CloudinaryManager()
        {
            Account account = new Account("cloud name", "api key", "api secret");
            _cloudinary = new Cloudinary(account);
        }

        public List<string> GetImageUrlList(string publicId = null)
        {
            var resources = GetAllResults((cursor) => _cloudinary.ListResources(cursor));
            return resources.ToList().Select(x => x.SecureUri.OriginalString).ToList();
        }

        public List<string> GetImageUrlListByPrefix(string prefix = null,string type="upload")
        {
            var resources = GetAllResults((cursor) => _cloudinary.ListResourcesByPrefix(prefix, type,cursor));
            return resources.ToList().Select(x => x.SecureUri.OriginalString).ToList();
        }

        public List<Folder> GetFolderList()
        {
            var folder = _cloudinary.RootFolders();
            return folder?.Folders?.Select(x => new Folder() { Name = x.Name, Path = x.Path }).ToList();
        }

        public List<Resource> GetResourceList(string publicId = null)
        {
            var resources = GetAllResults((cursor) => _cloudinary.ListResources(cursor));
            return resources.ToList();
        }

        private IEnumerable<Resource> GetAllResults(Func<string, ListResourcesResult> list)
        {
            ListResourcesResult current = list(null);
            IEnumerable<Resource> resources = current.Resources;
            for (; resources != null && current.NextCursor != null; current = list(current.NextCursor))
            {
                resources = resources.Concat(current.Resources);
            }
            return resources;
        }

        public bool UploadImage(string fileName,MemoryStream fileStream, int height, int width,int radius, string publicId = null, string folderName = null, string optionalName = null, bool OptionalBoolParameter = false,
                                       bool uniqueFilename=false,bool useFilename = true,string type= "upload")
        {
            var imageParameters = new ImageUploadParams();
            imageParameters.UniqueFilename = uniqueFilename;
            imageParameters.UseFilename = useFilename;
            imageParameters.Overwrite = true;
            imageParameters.Type = type;
            imageParameters.PublicId = publicId;
            imageParameters.Folder = folderName;
            imageParameters.Transformation = new Transformation().Width(width).Height(height).Radius(radius);
            imageParameters.File = new FileDescription(fileName,fileStream);
            var response = _cloudinary.Upload(imageParameters);
            if (response != null && response.PublicId != null)
            {
                return true;
            }
            return false;
        }

    }
}
