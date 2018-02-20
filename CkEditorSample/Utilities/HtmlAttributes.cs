using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Mvc;
using System.Web.Routing;

namespace CkeditorSample.Utilities
{
    public static class HtmlAttributes
    {
        public static RouteValueDictionary AnonymousObjectToHtmlAttributes(this HtmlHelper htmlHelper, object htmlAttributes)
        {	
            if (htmlAttributes is RouteValueDictionary)
                return (RouteValueDictionary)htmlAttributes;

            if (htmlAttributes is IDictionary<string, object>)
                return new RouteValueDictionary((IDictionary<string, object>)htmlAttributes);

            RouteValueDictionary result = new RouteValueDictionary();
            if (htmlAttributes != null)
            {
                foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(htmlAttributes))
                {
                    result.Add(property.Name.Replace('_', '-'), property.GetValue(htmlAttributes));
                }
            }
            return result;
        }
    }
}
