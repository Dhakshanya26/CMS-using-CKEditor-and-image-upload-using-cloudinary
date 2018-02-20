using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudinaryService
{
  
    public class CloudinaryManager : ICloudinaryManager
    {
        private readonly Cloudinary _cloudinary;
        public CloudinaryManager()
        {
            Account account = new Account("red-letter-days", "643618489799757", "s1vwBrbxShXp6t0O0E-DcsDi9H0");
            _cloudinary = new Cloudinary(account);
        }

        public List<string> GetImageUrlList(string publicId = null)
        {
            var s = _cloudinary.GetResource("").SecureUrl;
            var resources = GetAllResults((cursor) => _cloudinary.ListResources(cursor));

            var folder = GetFolderList();
            return resources.ToList().Select(x => x.SecureUri.OriginalString).ToList();
        }

        public List<string> GetFolderList()
        {
            var s = _cloudinary.RootFolders();
            //   var resources = GetAllResults((cursor) => _cloudinary.ListResources(cursor));
            //  return resources.ToList().Select(x => x.SecureUri.OriginalString).ToList();
            return new List<string>();
        }

        public List<Resource> GetResourceList(string publicId = null)
        {
            var resources = GetAllResults((cursor) => _cloudinary.ListResources(cursor));
            return resources.ToList();
        }

        private IEnumerable<Resource> GetAllResults(Func<String, ListResourcesResult> list)
        {
            ListResourcesResult current = list(null);
            IEnumerable<Resource> resources = current.Resources;
            for (; resources != null && current.NextCursor != null; current = list(current.NextCursor))
            {
                resources = resources.Concat(current.Resources);
            }
            return resources;
        }
    }
}
