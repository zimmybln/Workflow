using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Designer.Components
{
    public static class ApplicationServices
    {
        private static string _strProduct = null;
        private static string _strCopyright = null;
        private static string _strTitle = null;
        private static string _strRootDirectory = null;
        private static Version _version = null;

        static ApplicationServices()
        {

        }


        #region Eigenschaften

        /// <summary>
        /// Ermittelt das Verzeichnis der Anwendung.
        /// </summary>
        public static string RootDirectory
        {
            get
            {
                if (String.IsNullOrEmpty(_strRootDirectory))
                {
                    var uri = new Uri(Assembly.GetEntryAssembly().CodeBase);
                    _strRootDirectory = Path.GetDirectoryName(uri.LocalPath);
                }

                return _strRootDirectory;
            }
        }

        /// <summary>
        /// Liefert den Produktnamen der Anwendung.
        /// </summary>
        public static string Product
        {
            get
            {
                if (String.IsNullOrEmpty(_strProduct))
                {
                    System.Reflection.Assembly asm = System.Reflection.Assembly.GetEntryAssembly() ?? System.Reflection.Assembly.GetCallingAssembly();

                    var attrProduct = (from a in asm.GetCustomAttributes(
                            typeof(System.Reflection.AssemblyProductAttribute), false)
                                       select a).FirstOrDefault();

                    _strProduct = attrProduct != null ? ((System.Reflection.AssemblyProductAttribute)attrProduct).Product : String.Empty;
                }
                return _strProduct;
            }
        }

        /// <summary>
        /// Liefert das Copyright der Anwendung.
        /// </summary>
        public static string Copyright
        {
            get
            {
                if (String.IsNullOrEmpty(_strCopyright))
                {
                    System.Reflection.Assembly asm = System.Reflection.Assembly.GetEntryAssembly() ?? System.Reflection.Assembly.GetCallingAssembly();

                    var attrCopyright = (from a in asm.GetCustomAttributes(
                                                   typeof(System.Reflection.AssemblyCopyrightAttribute), false)
                                         select a).FirstOrDefault();

                    _strCopyright = attrCopyright != null ? ((System.Reflection.AssemblyCopyrightAttribute)attrCopyright).Copyright : String.Empty;
                }
                return _strCopyright;
            }
        }

        /// <summary>
        /// Liefert die Versionsnummer der Anwendung.
        /// </summary>
        public static Version Version
        {
            get
            {
                if (_version == null)
                {
                    Assembly asm = Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly();

                    var attrFileVersion =
                        (from a in asm.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false) select a)
                            .FirstOrDefault();

                    if (attrFileVersion != null)
                    {
                        _version = new Version(((AssemblyFileVersionAttribute)attrFileVersion).Version);
                    }
                }
                return _version;
            }
        }

        /// <summary>
        /// Liefert den Titel der Anwendung.
        /// </summary>
        public static string Title
        {
            get
            {
                if (String.IsNullOrEmpty(_strTitle))
                {
                    System.Reflection.Assembly asm = System.Reflection.Assembly.GetEntryAssembly() ?? System.Reflection.Assembly.GetCallingAssembly();

                    var attrTitle = (from a in asm.GetCustomAttributes(
                                                 typeof(System.Reflection.AssemblyTitleAttribute), false)
                                     select a).FirstOrDefault();

                    _strTitle = attrTitle != null ? ((System.Reflection.AssemblyTitleAttribute)attrTitle).Title : String.Empty;
                }
                return _strTitle;
            }
        }

        #endregion

 
    }
}
