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
        public const int ContentTypeFilters = 10;
        public const int ContentTypePortfolioThemes = 11;

        private static string _strProduct = null;
        private static string _strCopyright = null;
        private static string _strTitle = null;
        private static Version _version = null;
        private static string _strRootDirectory = null;
        private static readonly AggregateCatalog _objComponentCatalog;
        private static CompositionContainer _objComponentComposition;
        private static readonly Dictionary<Type, object> _dictReplacements;
        private static bool _fIsBeginRegistered = false;

        static ApplicationServices()
        {
            _objComponentCatalog = new AggregateCatalog();
            _dictReplacements = new Dictionary<Type, object>();
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

        private static CompositionContainer Container
        {
            get { return _objComponentComposition; }
        }

        #endregion

        #region Methoden zum Instanzmanagement

        /*
         *      Im Instanzmanagement werden unterschiedliche Modultypen verwaltet. Obwohl die grundsätzliche Funktionsweise
         *      gleich ist, sollte mit der Zielsetzung der Typensicherheit dennoch zwischen den Typen unterschieden werden.
         *      Folgende Typen werden hier berücksichtigt:
         *      - Dienste (Basisschnittstelle IService)
         *      - Contextmodels (Basisschnittstelle IContextModelBase
         *      - Models (Basistyp ModelBase)
         * 
         *      Für jeden Typ können die Instanzen nach folgenden Kriterien ermittelt werden
         *      - ursprüngliche Implementierung (älteste)
         *      - jüngste Implementierung
         */

        /// <summary>
        /// Ermittelt die Instanz eines Dienstes
        /// </summary>
        public static T GetService<T>()
            where T : class, IService
        {
            return GetService<T>(new GetComponentOptions() { Version = ComponentVersion.Default });
        }

        /// <summary>
        /// Ermittelt die Instanz eines Dienstes mit der Angabe von 
        /// Charakteristiken.
        /// </summary>
        public static T GetService<T>(GetComponentOptions Options)
            where T : class, IService
        {
            // Wenn es eine Stellvertreterinstanz gibt, dann diese zurückliefern
            if (_dictReplacements.ContainsKey(typeof(T)))
                return _dictReplacements[typeof(T)] as T;

            return GetComponent<T>(Options);
        }

        ///// <summary>
        ///// Ermittelt alle Implementierungen eines bestimmten Typs.
        ///// </summary>
        public static T GetLatestItem<T>()
            where T : class
        {
            var item = InternalGetLatestItem(typeof(T)) as T
                    ?? Container.GetExportedValueOrDefault<T>();

            return item;
        }

        public static object GetLatestItem(Type Type)
        {
            var item = InternalGetLatestItem(Type);

            if (item == null && Container != null)
            {
                var inneritem = Container.GetExports(Type, null, null).FirstOrDefault();

                if (inneritem != null)
                    item = inneritem.Value;
            }

            return item;
        }

        public static IEnumerable<T> GetItems<T>()
            where T : class
        {
            return Container.GetExportedValues<T>();
        }

        /// <summary>
        /// Ermittelt die Instanz einer Komponente.
        /// </summary>
        private static T GetComponent<T>(GetComponentOptions Options)
            where T : class
        {
            ComponentVersion enmComponentVersion = ComponentVersion.Default;

            // Auswertung der Erstellungsparameter
            if (Options != null)
            {
                enmComponentVersion = Options.Version;
            }

            T svr = default(T);

            // Ermitteln der jüngsten Version
            if (enmComponentVersion == ComponentVersion.Latest)
                svr = InternalGetLatestItem(typeof(T)) as T;

            // Ermitteln der aktuellen Version
            if (svr == null)
                svr = _objComponentComposition.GetExportedValue<T>();

            return svr;
        }

        /// <summary>
        /// Ermittelt die Implementierung eines Typs, mit der höchsten
        /// Versionsnummer. Dabei wird das Attribut ComponentInfo 
        /// herangezogen.
        /// </summary>
        private static object InternalGetLatestItem(Type Type)
        {
            if (Container == null)
                return null;

            var lstItems = Container.GetExports(Type, typeof(IComponentInfo), null);

            Lazy<object, object> latestitem = null;
            Version latestversion = null;

            // Durchlaufe alle Einträge
            foreach (var l in lstItems)
            {
                var m = l.Metadata as IComponentInfo;

                if (m == null) continue;

                Version v;

                if (!Version.TryParse(m.Version, out v))
                    continue;

                if (latestversion == null)
                {
                    latestitem = l;
                    latestversion = v;
                }
                else if (v.CompareTo(latestversion) > 0)
                {
                    latestitem = l;
                    latestversion = v;
                }
            }

            if (latestitem != null)
                return latestitem.Value;

            return null;
        }

        #endregion

        /// <summary>
        /// Registriert ein Assembly als Anwendungsbestandteil.
        /// </summary>
        public static void RegisterModule(Assembly Assembly)
        {
            // Gültigkeitsüberprüfungen
            if (!_fIsBeginRegistered)
                throw new InvalidMethodSequence("BeginRegister");

            if (Assembly == null)
                throw new ArgumentNullException(nameof(Assembly));

            var asmCatalog = new AssemblyCatalog(Assembly);

            _objComponentCatalog.Catalogs.Add(asmCatalog);
        }

        /// <summary>
        /// Registriert eine Assemblydatei als Anwendungsbestandteil.
        /// </summary>
        public static void RegisterModule(string AssemblyCodeBase)
        {
            // Gültigkeitsüberprüfungen
            if (!_fIsBeginRegistered)
                throw new InvalidMethodSequence("BeginRegister");

            if (String.IsNullOrEmpty(AssemblyCodeBase))
                throw new ArgumentException(AssemblyCodeBase);

            _objComponentCatalog.Catalogs.Add(new AssemblyCatalog(AssemblyCodeBase));
        }

        /// <summary>
        /// Registriert ein Verzeichnis als einen Standard mit
        /// Assemblies als Anwendungsbestandteilen.
        /// </summary>
        public static void RegisterModuleDirectory(string Path)
        {
            // Gültigkeitsüberprüfungen
            if (!_fIsBeginRegistered)
                throw new InvalidMethodSequence("BeginRegister");

            _objComponentCatalog.Catalogs.Add(new DirectoryCatalog(Path));
        }

        /// <summary>
        /// Registriert die Ersetzung eines Typs im Catalog.
        /// </summary>
        /// <remarks>Wenn die Instanz eines Typs angefragt wird, wird normalerweise der MEF Katalog 
        /// befragt. Für das Testen ist es jedoch notwendig, dass nicht die Instanzen, die während der
        /// Ausführung der Anwendung ermittelt werden, benutzt werden, sondern spezialisierte Implementierungen.
        /// Diese stellen dann Daten auf eine für den Testfall zugeschnittene Art zur Verfügung.
        /// 
        /// Wenn hier ein Typ registriert ist, wird grundsätzlich dieser zurückgeliefert. Unabhängig davon, ob
        /// eine spezielle Version erfragt wird.
        /// </remarks>
        public static void RegisterReplacement(Type TypeIdentifier, object Instance)
        {
            if (_dictReplacements.ContainsKey(TypeIdentifier))
                _dictReplacements.Remove(TypeIdentifier);

            if (Instance != null)
                _dictReplacements.Add(TypeIdentifier, Instance);
        }

        /// <summary>
        /// Entfernt einen Typ-Stellvertreter aus dem Katalog.
        /// </summary>
        public static void UnregisterReplacement(Type TypeIdentifier)
        {
            if (_dictReplacements.ContainsKey(TypeIdentifier))
                _dictReplacements.Remove(TypeIdentifier);
        }

        /// <summary>
        /// Beginnt die Registrierung der beteiligten Komponenten
        /// </summary>
        /// <remarks>Momentan hat diese Methode keine Funktionalität. Sie dient jedoch
        /// als Merker, um mit späterer Funktionalität kompatibel zu sein.</remarks>
        public static void BeginRegister()
        {
            _fIsBeginRegistered = true;
        }

        /// <summary>
        /// Schließt die Registrierung der an der Anwendung beteiligten Module
        /// ab. Auf Dienste kann anschließend mit der Methode GetService zugegriffen
        /// werden.
        /// </summary>
        public static void EndRegister()
        {
            _objComponentComposition = new CompositionContainer(_objComponentCatalog);
        }
    }
}
