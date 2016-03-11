using System;
using System.IO;
using System.IO.Packaging;
using System.Text;

namespace Designer.Models
{
    /// <summary>
    /// This class transfers TextWriter output to another location.
    /// </summary>
    public class TextToUiWriter : TextWriter
    {
        private readonly IWriterAdapter _adapter;

        public TextToUiWriter(IWriterAdapter adapter)
        {
            _adapter = adapter;
        }

        public override Encoding Encoding => Encoding.ASCII;

        public string Prefix { get; set; }

        public override void WriteLine(string value)
        {
            base.WriteLine(value);

            string prefix = String.IsNullOrEmpty(Prefix) ? "" : Prefix;

            _adapter.WriteLine(prefix + value);
        }
    }
}