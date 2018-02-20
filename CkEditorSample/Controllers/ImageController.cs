using CkeditorSample.ServiceWrapper;
using CkEditorSample.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Drawing;
using System.IO;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace CkEditorSample.Controllers
{
    public class ImageController : Controller
    {
        private readonly CloudinaryManager _cloudinaryManager;
        public ImageController()
        {
            _cloudinaryManager = new CloudinaryManager();
        }
        // GET: Image
        public ActionResult ImageBrowser()
        {

            var response = _cloudinaryManager.GetImageUrlList();
            var folderList = _cloudinaryManager.GetFolderList();
            var images = MapImageList(response);
            var imageModel = new ImageModel()
            {
                Images = images,
                Folders = new SelectList(folderList, dataTextField: "Name", dataValueField: "Path")
            };


            return View(imageModel);
        }

        [HttpPost]
        public ActionResult GetImageListByFolderName(string folderName)
        {
            if (!string.IsNullOrEmpty(folderName) && string.Equals("All", folderName))
                folderName = null;
            var imageList = _cloudinaryManager.GetImageUrlListByPrefix(folderName);
            var response = MapImageList(imageList);
            var imageModel = new ImageModel();
            imageModel.Images = response;
            return PartialView("_ImageViewer", imageModel);
        }

        private List<Models.Image> MapImageList(List<string> response)
        {
            var images = new List<Models.Image>();
            response.ForEach(x => images.Add(new Models.Image()
            {
                ImageUrl = x,
                ImageName = x.Split('/').Last()
            }));

            return images;
        }
        public ActionResult UploadImage()
        {
            var folderList = _cloudinaryManager.GetFolderList();
            var imageModel = new UploadImageModel()
            {
                Folders = new SelectList(folderList, dataTextField: "Name", dataValueField: "Path")
            };
            return View(imageModel);
        }

        [HttpPost]
        public ActionResult UploadImage(UploadImageModel model)
        {
            var isUploadSuccessful = false;
            if (model.File != null)
            {
                Bitmap original = null;
                var name = "newimagefile";
                var errorField = string.Empty;

                if (model.File != null) // model.IsFile
                {
                    errorField = "File";
                    name = Path.GetFileNameWithoutExtension(model.File.FileName);
                    original = System.Drawing.Image.FromStream(model.File.InputStream) as Bitmap;
                }

                if (original != null)
                {
                    var img = CreateImage(original, model.X, model.Y, model.Width, model.Height);
                    var ms = new MemoryStream();
                    img.Save(ms, ImageFormat.Jpeg);

                    using (MemoryStream memoryStream = new MemoryStream(ms.ToArray()))
                    {
                        isUploadSuccessful = _cloudinaryManager.UploadImage("streamed", memoryStream, model.ChangeHeight, model.ChangeWidth,model.ImageRadius, model.FileName,
                                                       string.Equals(model.FolderName,"Root") ? null : model.FolderName );
                    }
                    if(isUploadSuccessful)
                    //Redirect to index
                    return RedirectToAction("ImageBrowser");
                }
                else //Otherwise we add an error and return to the (previous) view with the model data
                    ModelState.AddModelError(errorField, "Your upload did not seem valid. Please try again using only correct images!");
            }
            return View(model);
        }

        Bitmap CreateImage(Bitmap original, int x, int y, int width= 200, int height=200)
        {
            var img = new Bitmap(width, height);

            using (var g = Graphics.FromImage(img))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(original, new Rectangle(0, 0, width, height), x, y, width, height, GraphicsUnit.Pixel);
            }

            return img;
        }
    }
}