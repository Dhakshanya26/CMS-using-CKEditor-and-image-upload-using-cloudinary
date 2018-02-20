using System;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace CkeditorSample.Utilities
{
    public abstract class BaseHubFieldForReferenceType<TModel, TProperty> : BaseHubField<TModel, TProperty>
        where TModel : class
    {
        protected TProperty FieldValue
        {
            get { return GetFieldValue(this.HtmlHelper, this.Expression); }
        }

        protected static TProperty GetFieldValue(HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            TModel model = htmlHelper.ViewData.Model;
            if (model == null)
            {
                return default(TProperty);
            }
            Func<TModel, TProperty> func = expression.Compile();
            return func(model);
        }       

        protected BaseHubFieldForReferenceType(HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
            : base(htmlHelper, expression) { }
    }
}