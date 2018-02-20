using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CkEditorSample.Models
{
    public class ImageModel
    {
        public string HtmlText { get; set; }

        public List<Image> Images { get; set; }

        public string FolderName { get; set; }
        public SelectList Folders { get; set; }
    }
    public class UploadImageModel
    {
        public UploadImageModel()
        {
            Height = 200;
            Width = 200;
           
        }
        [Display(Name = "Internet URL")]
        public string Url { get; set; }

        public bool IsUrl { get; set; }

        [Display(Name = "Flickr image")]
        public string Flickr { get; set; }

        public bool IsFlickr { get; set; }

        [Display(Name = "Local file")]
        public HttpPostedFileBase File { get; set; }

        public bool IsFile { get; set; }

        [Range(0, int.MaxValue)]
        public int X { get; set; }

        [Range(0, int.MaxValue)]
        public int Y { get; set; }

        [Range(1, int.MaxValue)]
        public int Width { get; set; }

        [Range(1, int.MaxValue)]
        public int Height { get; set; }

        [Range(1, int.MaxValue)]
        public int ChangeWidth { get; set; }

        [Range(1, int.MaxValue)]
        public int ChangeHeight { get; set; }

        [Range(0, int.MaxValue)]
        public int ImageRadius { get; set; }

        public string FileName { get; set; }

        public string FolderName { get; set; }
        public SelectList Folders { get; set; }
    }
    public class Image
    {
        public string ImageName { get; set; }

        public string ImageUrl { get; set; }
    }

    public class FolderModel
    {
        public string FolderName { get; set; }

        public string Path { get; set; }
    }
}
