using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Designer.Components
{
    public interface IComponentInfo
    {
        string Version { get; }
    }

    /// <summary>
    /// Definiert das Attribut, mit dem eine Anwendungskomponente Eigenschaften
    /// über sich zur Verfügung stellen kann.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false), MetadataAttribute]
    public class ComponentInfoAttribute : ExportAttribute, IComponentInfo
    {
        public ComponentInfoAttribute(string version)
            : base(typeof(IComponentInfo))
        {
            Version = version;
        }

        /// <summary>
        /// Liefert die Versionsnummer der Komponente
        /// </summary>
        public string Version { get; private set; }
    }
}
