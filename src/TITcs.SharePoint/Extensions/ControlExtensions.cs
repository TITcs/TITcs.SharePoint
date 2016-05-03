using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TITcs.SharePoint.Extensions
{
    public static class ControlExtensions
    {
        public static void Bind(this ListControl control, IDictionary<string, string> data)
        {
            if (data == null)
                return;

            foreach (var d in data)
            {
                if (control.Items.FindByValue(d.Value) == null)
                    control.Items.Add(new ListItem(d.Key, d.Value));
            }
        }

        public static void Bind(this Repeater control, object data)
        {
            control.DataSource = data;
            control.DataBind();
        }

        public static string Evaluate<T>(this IDataItemContainer itemContainer, Func<T, object> expression)
        {
            var result = expression((T)itemContainer.DataItem);
            if (result != null)
                return result.ToString();
            return null;
        }

        public static TOutput Evaluate<T, TOutput>(this IDataItemContainer itemContainer, Func<T, TOutput> expression)
        {
            return expression((T)itemContainer.DataItem);
        }
    }
}
