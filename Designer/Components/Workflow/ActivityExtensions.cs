using System;
using System.Activities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xaml;

namespace Designer.Components.Workflow
{
    public static class ActivityExtensions
    {
        public static string GetDefinition(this Activity activity)
        {
            StringWriter writer = new StringWriter();
            XamlXmlWriter xamlWriter = new XamlXmlWriter(writer, new XamlSchemaContext());
            XamlServices.Save(xamlWriter, activity);

            return writer.ToString();
        }

        public static Activity Clone(this Activity activity)
        {
            StringWriter writer = new StringWriter();
            XamlXmlWriter xamlWriter = new XamlXmlWriter(writer, new XamlSchemaContext());
            XamlServices.Save(xamlWriter, activity);

            StringReader reader = new StringReader(writer.ToString());
            XamlXmlReader xamlReader = new XamlXmlReader(reader);

            return XamlServices.Load(xamlReader) as Activity;
        }

    }
}
