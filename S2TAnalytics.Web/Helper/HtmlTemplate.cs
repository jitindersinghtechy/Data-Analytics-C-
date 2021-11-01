using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace S2TAnalytics.Web.Helper
{
    public class HtmlTemplate
    {        
        public HtmlTemplate()
        {

        }
        public string Render(string htmlTemplate, object values)
        {
            string output = htmlTemplate;
            foreach (var p in values.GetType().GetProperties())
                output = output.Replace("{{" + p.Name + "}}", (p.GetValue(values, null) as string) ?? string.Empty);
            return output;
        }
    }
}