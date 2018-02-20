using System;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;

namespace CkeditorSample.Utilities
{
    public abstract class BaseHubField<TModel, TProperty> : BaseHelper<TModel>
    {
        protected Expression<Func<TModel, TProperty>> Expression {get; set;}

        protected string FieldName
        {
            get { return (base.HtmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix + "." + GetFieldName(this.Expression)).Trim('.'); }
        }

        protected BaseHubField(HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression) : base(htmlHelper)
        {
            this.Expression = expression;
        }

        public abstract MvcHtmlString GetFieldHtml();

        public static string GetFieldName(Expression<Func<TModel, TProperty>> expression)
        {
            return ExpressionHelper.GetExpressionText(expression);
        }

        public MvcHtmlString GetHtml()
        {
            StringBuilder builder = new StringBuilder(GetFieldHtml().ToString());

            HtmlOptimizeString(builder);

            return new MvcHtmlString(builder.ToString());
        }

        protected string MakeJsVarSafe(string value)
        {
            return value.Replace("'", "\'");
        }
    }
}