using CkeditorSample.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace CkeditorSample.Helpers
{
    public static class CkeditorHtmlHelpers
    {
        /// <summary>
        /// Returns html to render Ckeditor with the specified form name
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="name"></param>
        /// <param name="config">A Ckeditor config object which can contain any setting mentioned here: http://docs.ckeditor.com/#!/api/CKEDITOR.config e.g. new { height = 500 }</param>
        /// <returns></returns>
        public static MvcHtmlString Ckeditor(this HtmlHelper htmlHelper, string name, object config = null)
        {
            return htmlHelper.Editor(name, "Ckeditor", new { Config = config });
        }

        /// <summary>
        /// Returns html to render Ckeditor for the specified model property
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <param name="config">A Ckeditor config object which can contain any setting mentioned here: http://docs.ckeditor.com/#!/api/CKEDITOR.config e.g. new { height = 500 }</param>
        /// <returns></returns>
        public static MvcHtmlString CkeditorFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object config = null)
        {
            return htmlHelper.EditorFor(expression, "Ckeditor", new { Config = config });
        }

        public static MvcHtmlString HubHtmlTextEditorFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string toolbar = null, string filebrowserImageUrl = null, string removePlugin = null, string extraPlugin = null)
        {
            var htmlEditor = new HubHtmlTextEditor<TModel, TProperty>(htmlHelper, expression, toolbar, filebrowserImageUrl, removePlugin, extraPlugin);
            return htmlEditor.GetHtml();
        }


        public class HubHtmlTextEditor<TModel, TProperty> : BaseHubField<TModel, TProperty>
        {
            private string JsPrefix { get; }
            public string Toolbar { get; set; }
            public string FilebrowserImageUrl { get; set; }
            public string RemovePlugin { get; set; }

            public string ExtraPlugin { get; set; }
            private TProperty DisplayValue { get; }

            public HubHtmlTextEditor(HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression,
                                        string toolbar = null, string filebrowserImageUrl = null, string removePlugin = null, string extraPlugin = null) : base(htmlHelper, expression)
            {
                var defaultToolbar = @"Source,Preview,SelectAll,Scayt,Styles,Format,Bold,Italic,Image,Table,HorizontalRule,SpecialChar,Link,Unlink,NumberedList,BulletedList,Outdent,Indent,Blockquote,Maximize,customimage";
                var defaultfilebrowserImageUrl = "Image/Imagebrowser";
                JsPrefix = string.IsNullOrWhiteSpace(FieldName) ? string.Empty :
                    $"{TagBuilder.CreateSanitizedId(FieldName)}";
                var customToolbar = !string.IsNullOrWhiteSpace(toolbar) ? toolbar : defaultToolbar;
                Toolbar = JsonConvert.SerializeObject(customToolbar.Split(','));
                if (removePlugin != null) RemovePlugin = removePlugin;
                if (extraPlugin != null) ExtraPlugin = extraPlugin;
                FilebrowserImageUrl = string.IsNullOrWhiteSpace(filebrowserImageUrl) ? defaultfilebrowserImageUrl : filebrowserImageUrl;
                var model = htmlHelper.ViewData.Model;
                var func = expression.Compile();
                DisplayValue = func(model);
            }

            public override MvcHtmlString GetFieldHtml()
            {
                var attributes = new Dictionary<string, object> { { "id", JsPrefix } };
                if (DisplayValue != null)
                    attributes.Add("Value", DisplayValue.ToString());
                var textBox = HtmlHelper.TextAreaFor(Expression, attributes);
                string html = $@"
                             <script src=""/CKEditor/ckeditor.js"" type=""text/javascript""></script>
                             <link href=""/Content/bootstrap.min.css"" rel=""stylesheet"" />
                             <script src = ""/Scripts/jquery-1.9.1.min.js"" type=""text/javascript"" ></script >
                             <script src = ""/Scripts/bootstrap.min.js"" type=""text/javascript""></script >
                             <style>
                                .modal-vertical-centered {{
                                  transform: translate(0, 50%) !important;
                                  -ms-transform: translate(0, 50%) !important; /* IE 9 */
                                  -webkit-transform: translate(0, 50%) !important; /* Safari and Chrome */
                                }}
                                .modal-body {{
                                    position: relative;
                                    padding: 15px;
                                    border: 1px solid #E5E5E5;
                                    margin: 10px;
                                    min-height: 350px;
                                }}
                             </style>
                             <script type=""text/javascript"" language=""javascript"">
                             function showMyDialog(e) {{
                             $(""#myModal"").modal('show');}}
                            function updateValue(id, value) {{
                                                              document.getElementById(id).value = value;
                                                            }}
                             (
                              function(){{
                                          CKEDITOR.replace('{JsPrefix}');
                                             
                                          CKEDITOR.editorConfig = function (config) 
                                          {{
                                           config.toolbar = [{{ name: 'toolbar', items: {Toolbar}}}]; 
                                           config.height = '250px';
                                           config.filebrowserImageBrowseUrl = '{FilebrowserImageUrl}';
                                           config.removePlugins = '{RemovePlugin}';
                                           config.extraPlugins  = '{ExtraPlugin}';
                                            config.autoGrow_onStartup = false;
                                            config.resize_enabled = false;
                                            config.filebrowserBrowseUrl = '{FilebrowserImageUrl}';
                                            config.filebrowserImageBrowseUrl = '{FilebrowserImageUrl}';
                                            config.filebrowserFlashBrowseUrl = '/Content/Js/kcfinder-3.20/browse.php?opener=ckeditor&type=flash';
                                            config.filebrowserUploadUrl = '/Content/Js/kcfinder-3.20/upload.php?opener=ckeditor&type=files';
                                            config.filebrowserImageUploadUrl = '/Content/Js/kcfinder-3.20/upload.php?opener=ckeditor&type=images';
                                            config.filebrowserFlashUploadUrl = '/Content/Js/kcfinder-3.20/upload.php?opener=ckeditor&type=flash';
 
                                          }};
                                        }}
                             )();
                             </script>
                           ";
                string modelHtml = @"<div id=""myModal"" class=""modal fade"">
                                     <div class=""modal-dialog modal-vertical-centered"" role=""document"">
                                     <div class=""modal-content"" style=""width:650px;height:500px"">
                                     <div class=""modal-header"">
                                     <button type = ""button"" class=""close"" data-dismiss=""modal"" aria-label=""Close"" >
                                     <span aria-hidden=""true"" >&times;</span></button>
                                     <h4 class=""modal-title"" id=""myModalLabel"" >Image</h4>
                                     </div>
                                     <div class=""modal-body"" >
                                     </div>
                                     <div class=""modal-footer"" >
                                     <button type = ""button"" class=""btn btn-default"" data-dismiss=""modal"" >Close</button>
                                     <button type = ""button"" class=""btn btn-primary"">Save changes</button>
                                     </div>
                                     </div>
                                     </div>
                                     </div>";
                return new MvcHtmlString(textBox+ modelHtml + html );
            }
        }

        public static MvcHtmlString File(this HtmlHelper html, string name)
        {
            var tb = new TagBuilder("input");
            tb.Attributes.Add("type", "file");
            tb.Attributes.Add("name", name);
            tb.GenerateId(name);
            return MvcHtmlString.Create(tb.ToString(TagRenderMode.SelfClosing));
        }

        public static MvcHtmlString FileFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression)
        {
            string name = GetFullPropertyName(expression);
            return html.File(name);
        }

        static string GetFullPropertyName<T, TProperty>(Expression<Func<T, TProperty>> exp)
        {
            MemberExpression memberExp;

            if (!TryFindMemberExpression(exp.Body, out memberExp))
                return string.Empty;

            var memberNames = new Stack<string>();

            do
            {
                memberNames.Push(memberExp.Member.Name);
            }
            while (TryFindMemberExpression(memberExp.Expression, out memberExp));

            return string.Join(".", memberNames.ToArray());
        }

        static bool TryFindMemberExpression(Expression exp, out MemberExpression memberExp)
        {
            memberExp = exp as MemberExpression;

            if (memberExp != null)
                return true;

            if (IsConversion(exp) && exp is UnaryExpression)
            {
                memberExp = ((UnaryExpression)exp).Operand as MemberExpression;

                if (memberExp != null)
                    return true;
            }

            return false;
        }

        static bool IsConversion(Expression exp)
        {
            return (exp.NodeType == ExpressionType.Convert || exp.NodeType == ExpressionType.ConvertChecked);
        }


    }
}
