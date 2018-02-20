using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;

namespace CkeditorSample.Utilities
{
    public class BaseHelper<TModel>
    {
        protected List<string> JavaScriptOutput
        {
            get { return GetViewData("JavaScriptOutput"); }
            set { SetViewData("JavaScriptOutput", value); }
        }

        protected List<string> JavaScriptImport
        {
            get { return GetViewData("JavaScriptImport"); }
            set { SetViewData("JavaScriptImport", value); }
        }

        private void SetViewData(string name, object value)
        {
            this.HtmlHelper.ViewContext.Controller.ViewData[name] = value;
        }

        private List<string> GetViewData(string name)
        {
            return (this.HtmlHelper.ViewContext.Controller.ViewData[name] ?? (this.HtmlHelper.ViewContext.Controller.ViewData[name] = new List<string>())) as List<string>;
        }

        protected HtmlHelper<TModel> HtmlHelper { get; set; }

        public BaseHelper(HtmlHelper<TModel> htmlHelper)
        {
            this.HtmlHelper = htmlHelper;
        }

        protected void HtmlOptimizeString(StringBuilder builder)
        {
            builder.Replace(Environment.NewLine, " ");
            builder.Replace("\n", " ");
            builder.Replace("\r", " ");
            builder.Replace("\t", " ");
            while (builder.ToString().Contains("  "))
            {
                builder.Replace("  ", " ");
            }
            builder.Replace("> <", "><");
            builder.Replace("> ", ">");
            builder.Replace(" <", "<");
        }
    }
}