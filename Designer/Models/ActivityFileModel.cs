using System;
using System.Activities;
using System.Activities.XamlIntegration;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xaml;
using Designer.Components.Models;

namespace Designer.Models
{
    public class ActivityFileModel : StorageModel<Activity>
    {
        public ActivityFileModel(IStorageUi ui, IDocumentProvider<Activity> documentProvider)
            : base(ui, documentProvider)
        {

        }
        
        protected override Activity TransferStreamToDocument(Stream stream)
        {
            StreamReader sr = new StreamReader(stream, Encoding.UTF8, true, 1024, true);
            var reader = ActivityXamlServices.CreateBuilderReader(new XamlXmlReader(sr, new XamlSchemaContext()));
            var workflow = XamlServices.Load(reader) as Activity;
            return workflow;
        }

        protected override void TransferDocumentIntoStream(Activity document, Stream documentStream)
        {
            StreamWriter sw = new StreamWriter(documentStream, Encoding.UTF8, 1024, true);
            var writer = ActivityXamlServices.CreateBuilderWriter(new XamlXmlWriter(sw, new XamlSchemaContext()));
            XamlServices.Save(writer, document);
            sw.Close();
        }


    }
}
