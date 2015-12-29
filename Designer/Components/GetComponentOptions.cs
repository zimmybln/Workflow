using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Designer.Components
{
    public enum ComponentVersion
    {
        Default,

        Latest
    }

    /// <summary>
    /// Ermöglicht das Ermitteln von Komponenten mit speziellen
    /// Optionen.
    /// </summary>
    public class GetComponentOptions
    {
        public GetComponentOptions()
        {
            Version = ComponentVersion.Default;
        }

        public ComponentVersion Version { get; set; }

    }
}
