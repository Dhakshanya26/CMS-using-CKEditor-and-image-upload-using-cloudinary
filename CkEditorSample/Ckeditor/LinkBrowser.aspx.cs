using System;
using System.Collections.Generic;
using System.IO;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using HttpMethod = CloudinaryDotNet.HttpMethod;

public partial class LinkBrowser : System.Web.UI.Page
{
    private Cloudinary _cloudinary;
    Account account = new Account(
 "red-letter-days",
 "643618489799757",
 "s1vwBrbxShXp6t0O0E-DcsDi9H0");
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            BindDirectoryList();
            ChangeDirectory(null, null);
            NewDirectoryButton.OnClientClick = "var name = prompt('Enter name of folder:'); if (name == null || name == '') return false; document.getElementById('" + NewDirectoryName.ClientID + "').value = name;";
        }
        Page.ClientScript.RegisterStartupScript(this.GetType(), "FocusImageList", "window.setTimeout(\"document.getElementById('" + ImageList.ClientID + "').focus();\", 1);", true);
       

        _cloudinary = new Cloudinary(account);
        //      var uploadParams = new ImageUploadParams()
        //      {
        //          File = new FileDescription(@"D:\Dhakshanya\March_2016\9th Task\MVC\Images\Image1.jpg"),
        //          PublicId = "sample_id",
        //          Transformation = new Transformation().Crop("limit").Width(40).Height(40),
        //          EagerTransforms = new List<Transformation>()
        //{
        //  new Transformation().Width(200).Height(200).Crop("thumb").Gravity("face").
        //    Radius(20).Effect("sepia"),
        //  new Transformation().Width(100).Height(150).Crop("fit").FetchFormat("png")
        //},
        //          Tags = "special, for_homepage"
        //      };

        //      var uploadResult = _cloudinary.Upload(uploadParams);

       var s= _cloudinary.GetResource("").SecureUrl;
        // var res = s.JsonObj["resources"].Select(t => t.ToString()).ToList();
       var  resources = GetAllResults((cursor) => _cloudinary.ListResources(cursor));
       var res=  resources.ToList();
    }

    protected IEnumerable<Resource> GetAllResults(Func<String, ListResourcesResult> list)
    {
        ListResourcesResult current = list(null);
        IEnumerable<Resource> resources = current.Resources;
        for (; resources != null && current.NextCursor != null; current = list(current.NextCursor))
        {
            resources = resources.Concat(current.Resources);
        }

        return resources;
    }
    public GetResourceResult1 GetResource(string publicId = null)
    {
        return GetResource(new GetResourceParams(publicId));
    }
    public const string ADDR_API = "api.cloudinary.com";
    public const string ADDR_RES = "res.cloudinary.com";
    public const string API_VERSION = "v1_1";
    string m_apiAddr = "https://" + ADDR_API;
    public GetResourceResult1 GetResource(GetResourceParams parameters)
    {
     Api m_api = new Api(account);    
    UrlBuilder urlBuilder = new UrlBuilder(
            m_api.ApiUrlV.
            ResourceType("resources").
            Add(Api.GetCloudinaryParam<ResourceType>(parameters.ResourceType)).
            Add(parameters.Type).Add(parameters.PublicId).
            BuildUrl(),
            parameters.ToParamsDictionary());

        using (HttpWebResponse response = m_api.Call(
            HttpMethod.GET, urlBuilder.ToString(), null, null))
        {
            var result = CloudinaryDotNet.Actions.GetResourceResult1.Parse(response);
            return result;
        }
    }

    protected void BindDirectoryList()
    {
        LinkFolder = "";
        string[] directories = Directory.GetDirectories(FileLinkFolder);
        directories = Array.ConvertAll<string, string>(directories, delegate(string directory) { return directory.Substring(directory.LastIndexOf('\\') + 1); });
        DirectoryList.DataSource = directories;
        DirectoryList.DataBind();
        DirectoryList.Items.Insert(0, "Root");
    }

    protected void ChangeDirectory(object sender, EventArgs e)
    {
        DeleteDirectoryButton.Enabled = (DirectoryList.SelectedIndex != 0);
        LinkFolder = (DirectoryList.SelectedIndex == 0) ? "" : DirectoryList.SelectedValue + "/";
        SearchTerms.Text = "";
        BindImageList();
        SelectImage(null, null);
    }

    protected void DeleteFolder(object sender, EventArgs e)
    {
        Directory.Delete(FileLinkFolder, true);
        BindDirectoryList();
        ChangeDirectory(null, null);
    }

    protected void CreateFolder(object sender, EventArgs e)
    {
        string name = UniqueDirectory(NewDirectoryName.Value);
        Directory.CreateDirectory(FileLinkFolderRoot + name);
        BindDirectoryList();
        DirectoryList.SelectedValue = name;
        ChangeDirectory(null, null);
    }

    protected void BindImageList()
    {
        ImageList.Items.Clear();
        string[] files = Directory.GetFiles(FileLinkFolder, "*" + SearchTerms.Text.Replace(" ", "*") + "*");

        foreach (string file in files)
            ImageList.Items.Add(file.Substring(file.LastIndexOf('\\') + 1));

        if (files.Length > 0)
            ImageList.SelectedIndex = 0;
    }

    protected void Search(object sender, EventArgs e)
    {
        BindImageList();
    }

    protected void SelectImage(object sender, EventArgs e)
    {
        int pos = ImageList.SelectedValue.LastIndexOf('.');
        if (pos == -1)
            return;
        RenameImageButton.OnClientClick = "var name = prompt('Enter new filename:','" + ImageList.SelectedValue.Substring(0, pos) + "'); if (name == null || name == '') return false; document.getElementById('" + NewImageName.ClientID + "').value = name + '" + ImageList.SelectedValue.Substring(pos) + "';";

        string link = LinkFolder + ImageList.SelectedValue;
        OkButton.OnClientClick = "window.top.opener.CKEDITOR.dialog.getCurrent().setValueOf('info', 'url', encodeURI('" + link + "')); window.top.close(); window.top.opener.focus();";
    }

    protected void RenameImage(object sender, EventArgs e)
    {
        string filename = UniqueFilename(NewImageName.Value);
        File.Move(FileLinkFolder + ImageList.SelectedValue, FileLinkFolder + filename);
        BindImageList();
        ImageList.SelectedValue = filename;
        SelectImage(null, null);
    }

    protected void DeleteImage(object sender, EventArgs e)
    {
        File.Delete(FileLinkFolder + ImageList.SelectedValue);
        BindImageList();
        SelectImage(null, null);
    }

    protected void Upload(object sender, EventArgs e)
    {
        string filename = UniqueFilename(UploadedImageFile.FileName);
        UploadedImageFile.SaveAs(FileLinkFolder + filename);

        BindImageList();
        ImageList.SelectedValue = filename;
        SelectImage(null, null);
    }

    protected void Clear(object sender, EventArgs e)
    {
        Session.Remove("viewstate");
    }

    protected void SelectPage(object sender, EventArgs e)
    {
        OkButton.OnClientClick = "window.top.opener.SetUrl(encodeURI('" + FileLinkFolder + ImageList.SelectedValue + "')); window.top.close(); window.top.opener.focus();";
    }

    //util methods
    protected string UniqueFilename(string filename)
    {
        string newfilename = filename;

        for (int i = 1; File.Exists(FileLinkFolder + newfilename); i++)
        {
            newfilename = filename.Insert(filename.LastIndexOf('.'), "(" + i + ")");
        }

        return newfilename;
    }

    protected string UniqueDirectory(string directoryname)
    {
        string newdirectoryname = directoryname;

        for (int i = 1; Directory.Exists(FileLinkFolderRoot + newdirectoryname); i++)
        {
            newdirectoryname = directoryname + "(" + i + ")";
        }

        return newdirectoryname;
    }

    //properties
    protected string LinkFolderRoot
    {
        get { return ResolveUrl(ConfigurationManager.AppSettings["FilesRoot"]); }
    }

    protected string FileLinkFolderRoot
    {
        get { return Server.MapPath(LinkFolderRoot); }
    }

    private string LinkFolder
    {
        get { return LinkFolderRoot + ViewState["folder"]; }
        set { ViewState["folder"] = value; }
    }

    private string FileLinkFolder
    {
        get
        {
            if (ViewState["folder"] != null)
                return FileLinkFolderRoot + ViewState["folder"].ToString().Replace("/", "\\");

            return FileLinkFolderRoot;
        }
    }
}
