using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.Helpers
{
    public static class HtmlHelpers
    {
        public static string SafeGetString(this SqlDataReader reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex))
                return reader.GetString(colIndex);
            else
                return string.Empty;
        }
        public static string ToQueryString(this IDictionary<string, string> dict)
        {
            if (dict.Count == 0) return string.Empty;

            var buffer = new StringBuilder();
            int count = 0;
            bool end = false;

            foreach (var key in dict.Keys)
            {
                if (count == dict.Count - 1) end = true;

                if (end)
                    buffer.AppendFormat("{0}={1}", key, dict[key]);
                else
                    buffer.AppendFormat("{0}={1}&", key, dict[key]);

                count++;
            }

            return buffer.ToString();
        }


        public static string DisplayTime(this HtmlHelper html, DateTime time, double hoursToAdd,
            USTimeZone timeZone)
        {
            return DisplayTime(html, time.AddHours(hoursToAdd), timeZone, true);
        }
        public static string Truncate(this string value, int maxChars)
        {
            return value.Length <= maxChars ? value : value.Substring(0, maxChars) + " ...";
        }

        public static string DisplayTime(this HtmlHelper html, DateTime time, decimal hoursToAdd,
            USTimeZone timeZone)
        {
            return DisplayTime(html, time, (double)hoursToAdd, timeZone, true);
        }

        public static string DisplayTime(this HtmlHelper html, DateTime time, double hoursToAdd,
            USTimeZone timeZone, bool displayTimezone)
        {
            return DisplayTime(html, time.AddHours(hoursToAdd), timeZone, displayTimezone);
        }

        public static string DisplayTime(this HtmlHelper html, DateTime time, decimal hoursToAdd,
            USTimeZone timeZone, bool displayTimezone)
        {
            return DisplayTime(html, time, (double)hoursToAdd, timeZone, displayTimezone);
        }

        public static string DisplayTime(this HtmlHelper html, DateTime time, USTimeZone timeZone)
        {
            return DisplayTime(html, time, timeZone, true);
        }

        public static string DisplayTime(this HtmlHelper html, DateTime time, USTimeZone timeZone,
            bool displayTimezone)
        {
            return DateTimeHelper.FormatTime(time, timeZone, displayTimezone);
        }

        public static string DisplayDateTime(this HtmlHelper html, DateTime dateTime,
            USTimeZone timeZone)
        {
            return DisplayDateTime(html, dateTime, timeZone, true);
        }

        public static string DisplayDateTime(this HtmlHelper html, DateTime dateTime,
            USTimeZone timeZone, bool displayTimezone)
        {
            return DisplayDate(html, dateTime) + " - " + DisplayTime(
                html, dateTime, timeZone, displayTimezone);
        }

        public static string DisplayDateShort(this HtmlHelper html, DateTime date)
        {
            return DateTimeHelper.FormatDateShort(date);
        }

        public static string DisplayDate(this HtmlHelper html, DateTime date)
        {
            return DateTimeHelper.FormatDate(date);
        }

        public static String CheckBoxList(this HtmlHelper htmlHelper, String name, IEnumerable<SelectListItem> selectList)
        {
            return htmlHelper.CheckBoxList(name, selectList, ((IDictionary<string, object>)null));
        }

        public static String CheckBoxList(this HtmlHelper htmlHelper, String name, IEnumerable<SelectListItem> selectList, Object htmlAttributes)
        {
            return htmlHelper.CheckBoxList(name, selectList, ((IDictionary<string, object>)new RouteValueDictionary(htmlAttributes)));
        }

        public static String CheckBoxList(this HtmlHelper htmlHelper, String name, IEnumerable<SelectListItem> selectList, IDictionary<String, Object> htmlAttributes)
        {
            // Verify arguments  
            if (String.IsNullOrEmpty(name)) throw new ArgumentNullException("name", "Name cannot be null");
            if (selectList == null) throw new ArgumentNullException("selectList", "Select list cannot be null");
            if (selectList.Count() < 1) throw new ArgumentException("Select list must contain at least one value", "selectList");

            // Define list  
            TagBuilder list = new TagBuilder("ul");
            list.MergeAttributes<String, Object>(htmlAttributes);

            // Define items  
            StringBuilder items = new StringBuilder();

            // Loop through items  
            Int32 index = 1;
            foreach (SelectListItem item in selectList)
            {
                // Define input  
                TagBuilder input = new TagBuilder("input");
                input.MergeAttribute("id", String.Concat(name, index));
                input.MergeAttribute("name", name);
                input.MergeAttribute("type", "checkbox");
                input.MergeAttribute("value", item.Value);

                //Set checked/unchecked state
                ModelStateDictionary modelState = htmlHelper.ViewData.ModelState;
                if (modelState[name] != null)
                {
                    string[] keys = modelState[name].Value.AttemptedValue.Split(',');
                    if (keys.Contains(item.Value))
                    {
                        input.MergeAttribute("checked", "checked");
                    }
                }
                else if (item.Selected && modelState.IsValid)
                {
                    input.MergeAttribute("checked", "checked");
                }

                // Define label  
                TagBuilder label = new TagBuilder("label");
                label.MergeAttribute("for", String.Concat(name, index));
                label.InnerHtml = item.Text;

                // Add item  
                items.AppendFormat("<li>{0}{1}</li>", input.ToString(TagRenderMode.Normal), label.ToString(TagRenderMode.Normal));
            }

            // Return list  
            list.InnerHtml = items.ToString();
            return list.ToString();

        }

        public static string GetInputName<TModel, TProperty>(Expression<Func<TModel, TProperty>> expression)
        {
            if (expression.Body.NodeType == ExpressionType.Call)
            {
                MethodCallExpression methodCallExpression = (MethodCallExpression)expression.Body;
                string name = GetInputName(methodCallExpression);
                return name.Substring(expression.Parameters[0].Name.Length + 1);

            }
            return expression.Body.ToString().Substring(expression.Parameters[0].Name.Length + 1);
        }

        private static string GetInputName(MethodCallExpression expression)
        {
            // p => p.Foo.Bar().Baz.ToString() => p.Foo OR throw...
            MethodCallExpression methodCallExpression = expression.Object as MethodCallExpression;
            if (methodCallExpression != null)
            {
                return GetInputName(methodCallExpression);
            }
            return expression.Object.ToString();
        }


        //http://iwantmymvc.com/populate-drop-down-list-mvc-3
        public static MvcHtmlString DropDownList(this HtmlHelper helper,
            string name, Dictionary<int, string> dictionary)
        {

            var selectListItems = new SelectList(dictionary, "Key", "Value");
            return helper.DropDownList(name, selectListItems);
        }

        public static MvcHtmlString EnumDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression) where TModel : class
        {
            string inputName = GetInputName(expression);
            var value = htmlHelper.ViewData.Model == null
                ? default(TProperty)
                : expression.Compile()(htmlHelper.ViewData.Model);

            return htmlHelper.DropDownList(inputName, ToSelectList(typeof(TProperty), value.ToString()));
        }

        public static SelectList ToSelectList(Type enumType, string selectedItem)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            foreach (var item in Enum.GetValues(enumType))
            {
                FieldInfo fi = enumType.GetField(item.ToString());
                var attribute = fi.GetCustomAttributes(typeof(DescriptionAttribute), true).FirstOrDefault();
                var title = attribute == null ? item.ToString() : ((DescriptionAttribute)attribute).Description;
                var listItem = new SelectListItem
                    {
                        Value = ((int)item).ToString(),
                        Text = title,
                        Selected = selectedItem == ((int)item).ToString()
                    };
                items.Add(listItem);
            }
            return new SelectList(items, "Value", "Text");

        }


        // Extension method
        public static MvcHtmlString ActionImage(this HtmlHelper html, string action, object routeValues, string imagePath, string alt)
        {
            var url = new UrlHelper(html.ViewContext.RequestContext);

            // build the <img> tag
            var imgBuilder = new TagBuilder("img");
            imgBuilder.MergeAttribute("src", url.Content(imagePath));
            imgBuilder.MergeAttribute("alt", alt);
            string imgHtml = imgBuilder.ToString(TagRenderMode.SelfClosing);

            // build the <a> tag
            var anchorBuilder = new TagBuilder("a");
            anchorBuilder.MergeAttribute("href", url.Action(action, routeValues));
            anchorBuilder.InnerHtml = imgHtml; // include the <img> tag inside
            string anchorHtml = anchorBuilder.ToString(TagRenderMode.Normal);

            return MvcHtmlString.Create(anchorHtml);
        }

        public static bool IsDebug(this HtmlHelper htmlHelper)
        {
            //http://stackoverflow.com/questions/4696175/razor-view-engine-how-to-enter-preprocessorif-debug
            // adds support for IfDebug compiler directive to razor views
            #if DEBUG
                        return true;
            #else
                              return false;
            #endif
        }

    }
}