using Designer.Components;
using System;
using System.Activities;
using System.Activities.Statements;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Serialization;
using Designer.Components.Models;
using Designer.Components.Workflow;
using Designer.Contracts;
using Designer.Contracts.Attributes;
using Designer.Dialogs;
using Designer.Properties;
using Designer.Services;
using Designer.Types;
using Prism.Commands;
using Prism.Mvvm;

namespace Designer.Models
{

    /// <summary>
    /// This model represents the ShellWindow
    /// </summary>
    [Export]
    public class MainWindowModel : BindableBase, IWriterAdapter, IPartImportsSatisfiedNotification, IDocumentProvider<Activity>
    {
        private Activity _currentworkflow = null;
        private bool _isBackstageOpen;
        private bool _changed = false;
        private readonly Dispatcher _dispatcher;

        private readonly NotificationService _notificationService;

        [Import(AllowRecomposition = true)]
        private IWorkflowExecutionService _executionService = null;
         
        [ImportingConstructor]
        public MainWindowModel(NotificationService notificationService)
        {
            _notificationService = notificationService;
            
            _dispatcher = Dispatcher.CurrentDispatcher;
            
            ExecuteWorkflow = new DelegateCommand(OnExecuteWorkflow);
            CloseCommand = new DelegateCommand(OnCloseCommand);
            ShowTraceDetailsCommand = new DelegateCommand<LoggingEntry>(OnShowTraceDetailsCommand);

            // Initialize model components
            StatusBar = new StatusBarModel(notificationService);

            FileModel = new ActivityFileModel(new FileBasedStorageUi() {DefaultExtension = "wdef"},
                                                   this);
            
            LoadedAdapter = new LoadedAdapter(OnLoaded);

            ToolboxItems.AddRange("Default", typeof(If), typeof(Sequence), typeof(While), typeof(DoWhile), typeof(Assign), typeof(Switch<>), typeof(WriteLine),
                                            typeof(TerminateWorkflow), typeof(Delay), typeof(InvokeMethod), typeof(ForEach<>));
            ToolboxItems.AddRange("ErrorHandling", typeof(Throw), typeof(TryCatch), typeof(Rethrow));

            if (Settings.Default.LoadActivities)
            {
                var types = ToolboxItems.AddFromDirectory("Activities");

                // finding Ui Options factories
                foreach (var t in types)
                {
                    // check if there is a DesignerActivityOptionsAttribute
                    var designeroptions =
                        t.GetCustomAttributes(typeof(DesignerActivityOptionsAttribute), true).FirstOrDefault() as DesignerActivityOptionsAttribute;
                    
                    if (designeroptions == null)
                        break;

                                        
                    var options = new ActivityOption() {Name = t.Name};
                    options.DataTypeName = designeroptions.DesignerOptionsDataContextType.Name;

                    // ui factory
                    IDesignerOptionsUiFactory uifactory = null;
                    try
                    {
                        uifactory = Activator.CreateInstance(designeroptions.DesignerOptionsUiFactoryType) as IDesignerOptionsUiFactory;
                    }
                    catch (Exception)
                    {
                        // ignore exception here
                        // collect the exception here...
                    }
                    
                    options.Content = uifactory?.GetOptionsUi();

                    // prepare the storage of custom properties
                    SettingsProperty property = Settings.Default.Properties[options.DataTypeName];

                    object datacontext = null;

                    if (property == null)
                    {
                        datacontext = Activator.CreateInstance(designeroptions.DesignerOptionsDataContextType);

                        property = new SettingsProperty(options.DataTypeName);
                        property.PropertyType = typeof(string);
                        property.IsReadOnly = false;
                        property.Attributes.Add(typeof(UserScopedSettingAttribute), new UserScopedSettingAttribute());
                        property.Provider = Settings.Default.Providers["Designer.exe"];
                        property.SerializeAs = SettingsSerializeAs.Xml;
                        property.DefaultValue = null;

                        Settings.Default.Properties.Add(property);
                        // to avoid the reload of the settings every single iteration we have to
                        // move the adding of properties
                        Settings.Default.Reload();
                    }

                    var propertyvalue = Settings.Default[options.DataTypeName]?.ToString();

                    // deserialize the property value
                    if (!String.IsNullOrWhiteSpace(propertyvalue))
                    {
                        try
                        {
                            var serializer = new XmlSerializer(designeroptions.DesignerOptionsDataContextType);
                            using (StringReader reader = new StringReader(propertyvalue))
                            {
                                var item = serializer.Deserialize(reader);

                                if (item != null)
                                {
                                    datacontext = item;
                                }
                            }
                        }
                        catch (Exception)
                        {
                            // collect the exception
                        }

                    }

                    options.Data = datacontext;

                    // add the options with group to the internal structure
                    var groupname = t.Assembly.GetName().Name;

                    var group = ActivityOptionGroups.FirstOrDefault(g => g.Name.Equals(groupname));

                    if (group == null)
                    {
                        group = new ActivityOptionGroup(groupname);
                        ActivityOptionGroups.Add(group);
                    }

                    group.Items.Add(options);                  

                }

                
            }
              
            
            Settings.Default.PropertyChanged += OnSettingsPropertyChanged;

            if (_notificationService != null)
            {
                _notificationService.OnNotify += OnNotify;
            }

            Title = ApplicationServices.Title;
        }

        #region IDocumentProvider

        Activity IDocumentProvider<Activity>.GetDefaultDocument()
        {
            return new Flowchart() { DisplayName = "Default" };
        }

        void IDocumentProvider<Activity>.SetDocument(Activity document)
        {
            CurrentWorkflow = document;
        }

        Activity IDocumentProvider<Activity>.GetDocument()
        {
            return CurrentWorkflow;
        }

        bool IDocumentProvider<Activity>.GetDocumentChanged()
        {
            return Changed;
        }

        void IDocumentProvider<Activity>.ActionPerformed()
        {
            Changed = false;
            IsBackstageOpen = false;
        }

        #endregion

        private void OnSettingsPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName.Equals(nameof(Settings.Default.LoadActivities)))
            {
                 
            }
            
        }

        private async void OnExecuteWorkflow()
        { 
            // reset all the UI Output
            TraceMessages.Clear();
            
            Console.SetOut(new TextToUiWriter(this) { Prefix = "Console: " });

            var designeroptions = new DesignerOptions();

            foreach (var group in ActivityOptionGroups)
            {
                foreach (var o in group.Items)
                {
                    designeroptions.AddOption(o.Data.GetType(), o.Data);
                }
            }

            // execute the workflow
            var options = new WorkflowExecutionOptions()
            {
                TraceWriter = this,
                DesignerOptions = designeroptions
            };

            //-- Das wahrscheinlich eher nicht!
            options.TrackingParticipants.Add(new StateChangeTrackingParticipant(this, StateChangeRecords.All) {ExecutionTime = new RelativeExecutionTimeProvider()});

            var executable = CurrentWorkflow.Clone();
            
            await _executionService.Execute(executable, options);

            _notificationService.Notify(new ExecutionFinishedNotification("Execution finished"));
            
        }

        private void OnShowTraceDetailsCommand(LoggingEntry parameter)
        {
            var entry = parameter as LoggingEntry;

            if (entry == null) return;

            var dlg = new LoggingEntryProperties(entry);

            dlg.ShowDialog();
        }

        private void OnCloseCommand()
        {
            // store all properties
            foreach (var group in ActivityOptionGroups)
           {
               foreach (var option in group.Items)
               {              
                   if (option.Data != null)
                   {
                       SettingsPropertyValue value = Settings.Default.PropertyValues[option.DataTypeName];

                       if (value == null)
                       {
                            SettingsProperty property = Settings.Default.Properties[option.DataTypeName];
                            value = new SettingsPropertyValue(property);
                       }

                        var serializer = new XmlSerializer(option.Data.GetType());
                        using (StringWriter writer = new StringWriter())
                        {
                            serializer.Serialize(writer, option.Data);
                            value.PropertyValue = writer.ToString();
                        }
                   }
                }
            }
            
            Application.Current.Shutdown();
        }

        private void OnNotify(object sender, INotification notification) 
        {

        }

        protected void OnLoaded()
        {
            // execute new document when loaded
            FileModel.NewCommand.Execute(null);
        }

        #region IWriterAdapter & Tracing

        void IWriterAdapter.WriteLine(string message)
        {
            if (!_dispatcher.CheckAccess())
            {
                _dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)(() => { AddEntry(new  LoggingEntry(message)); }));
            }
            else
            {
                AddEntry(new LoggingEntry(message));
            }
            
        }

        void IWriterAdapter.WriteEntry(LoggingEntry entry)
        {
            if (!_dispatcher.CheckAccess())
            {
                _dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)(() => { AddEntry(entry); }));
            }
            else
            {
                AddEntry(entry);
            }

        }

        private void AddEntry(LoggingEntry entry)
        {
            TraceMessages.Add(entry);
        }

        #endregion
        
        #region Properties

        public LoadedAdapter LoadedAdapter { get; }

        public ObservableCollection<ActivityOptionGroup> ActivityOptionGroups { get; } = new ObservableCollection<ActivityOptionGroup>();
         
        public ICommand ExecuteWorkflow { get; }

        public ICommand CloseCommand { get; }

        public ICommand ShowTraceDetailsCommand { get; }

        public ToolboxItemDescriptorCollection ToolboxItems { get; } = new ToolboxItemDescriptorCollection();

        public ObservableCollection<string> TemplateItems { get; } = new ObservableCollection<string>(); 
        
        public ObservableCollection<LoggingEntry> TraceMessages { get; } = new ObservableCollection<LoggingEntry>();

        public Activity CurrentWorkflow
        {
            get { return _currentworkflow; }
            set { SetProperty(ref _currentworkflow, value);}
        }

        public bool IsBackstageOpen
        {
            get { return _isBackstageOpen;}
            set { SetProperty(ref this._isBackstageOpen, value); }
        }

        public bool Changed
        {
            get { return _changed; }
            set { SetProperty(ref this._changed, value); }
        }

        private string _title;

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref this._title, value);}
        }

        public StatusBarModel StatusBar { get; }

        public StorageModel<Activity> FileModel { get; } 

        #endregion

        public void OnImportsSatisfied()
        {

        }



    }
}
