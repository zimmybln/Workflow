using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Commands;

namespace Designer.Components.Models
{
    public class DocumentInfo
    {
        public Stream Content { get; set; }

        public DocumentIdentification Identification { get; set; }
    }

    public class DocumentIdentification
    {
        public DocumentIdentification(string name)
        {
            Name = name;
        }
        
        public string Name { get; }

        public override string ToString()
        {
            return Name;
        }
    }

    /// <summary>
    /// This interface describes the communication with the user interface
    /// for all kinds of interaction, like selecting a file or errormessages.
    /// </summary>
    public interface IStorageUi
    {
        bool? PromptForSaveWhenDocumentChanged(DocumentInfo document);

        DocumentInfo PromptForOpenDocument();

        DocumentInfo PromptForSaveDocument(DocumentInfo document);

        DocumentInfo OpenDocument(DocumentInfo document);

        DocumentInfo ResetDocument(DocumentInfo document);

        void PromptException(Exception exception);
    }
     
    /// <summary>
    /// This interface describes the communication with the container of the
    /// document (in most cases the main window).
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDocumentProvider<T>
    {
        T GetDefaultDocument();

        void SetDocument(T document);

        T GetDocument();

        bool GetDocumentChanged();

        void ActionPerformed();
    }


    public abstract class StorageModel<T> : INotifyPropertyChanged
    {
        private string _documentname = null;
        private DocumentInfo _currentdocument = null;
        private readonly IStorageUi _storageUi;
        private readonly IDocumentProvider<T> _documentProvider; 

        protected StorageModel(IStorageUi ui, IDocumentProvider<T> documentProvider)
        {
            if (ui == null)
                throw new ArgumentNullException(nameof(ui));

            if (documentProvider == null)
                throw new ArgumentNullException(nameof(documentProvider));

            _storageUi = ui;
            _documentProvider = documentProvider;

            NewCommand = new DelegateCommand<string>(OnNewCommand);
            OpenCommand = new DelegateCommand<DocumentInfo>(OnOpenCommand);
            SaveCommand = new DelegateCommand<object>(OnSaveCommand);
            SaveAsCommand = new DelegateCommand<object>(OnSaveAsCommand);

        }

        #region abstract interface

        protected abstract T TransferStreamToDocument(Stream stream);

        protected abstract void TransferDocumentIntoStream(T document, Stream contentStream);

        #endregion

        /// <summary>
        /// Performs the processing for a new document.
        /// </summary>
        /// <param name="parameter">Reserved for further usage. Might be a template.</param>
        private void OnNewCommand(string parameter)
        {
            // check if the document changed 
            try
            {
                if (!SaveWhenChanged())
                    return;
            }
            catch (Exception ex)
            {
                _storageUi.PromptException(ex);
                return;
            }
            
            T defaultdocument = _documentProvider.GetDefaultDocument();

            _documentProvider.SetDocument(defaultdocument);

            _currentdocument = null;
            DocumentName = String.Empty;

            _documentProvider.ActionPerformed();
        }

        /// <summary>
        /// Performs the processing when opening a document from storage
        /// </summary>
        /// <param name="parameter">a document might be open without dialog</param>
        private void OnOpenCommand(DocumentInfo parameter)
        {
            // if so, prompt for saving ...
            if (!SaveWhenChanged())
                return;

            // create the document stream
            DocumentInfo info = null;
            T document = default(T);

            try
            {
                info = parameter != null
                    ? _storageUi.OpenDocument(parameter)
                    : _storageUi.PromptForOpenDocument();

                if (info == null)
                    return;

                // transfer stream to document instance
                document = TransferStreamToDocument(info.Content);

                info.Content.Close();
            }
            catch (Exception ex)
            {
                _storageUi.PromptException(ex);
                return;
            }
            
            // finally set the created document ...
            _documentProvider.SetDocument(document);

            // ... and the current document name
            _currentdocument = info;
            DocumentName = info.Identification.ToString();

            _documentProvider.ActionPerformed();
        }


        private void OnSaveCommand(object parameter)
        {
            DocumentInfo info = null;

            try
            {
                // prompt for storage name when no document is available
                info = _currentdocument == null
                    ? _storageUi.PromptForSaveDocument(null)
                    : _storageUi.ResetDocument(_currentdocument);

                if (info == null)
                    return;

                // get to current document
                T document = _documentProvider.GetDocument();


                // transfer it to stream
                TransferDocumentIntoStream(document, info.Content);
            }
            catch (Exception ex)
            {
                _storageUi.PromptException(ex);
                return;
            }
            finally
            {
                info?.Content?.Flush();
                info?.Content?.Close();
            }

            // set the documentname
            _currentdocument = info;
            DocumentName = info.Identification.ToString();
            
            _documentProvider.ActionPerformed();
        }

        private void OnSaveAsCommand(object parameter)
        {
            DocumentInfo info = null;

            try
            {
                // prompt for storage name when no document is available
                info = _storageUi.PromptForSaveDocument(_currentdocument);

                if (info == null)
                    return;

                // get to current document
                T document = _documentProvider.GetDocument();

                // transfer it to stream
                TransferDocumentIntoStream(document, info.Content);
            }
            catch (Exception ex)
            {
                _storageUi.PromptException(ex);
                return;
            }
            finally
            {
                info?.Content?.Flush();
                info?.Content?.Close();
            }

            // set the documentname
            _currentdocument = info;
            DocumentName = info.Identification.ToString();

            _documentProvider.ActionPerformed();
        }

        /// <summary>
        /// Check whether the document has changed and save it if neccessary.
        /// </summary>
        /// <returns>
        /// True when the process succeeded, otherwise false.
        /// </returns>
        private bool SaveWhenChanged()
        {
            if (_documentProvider.GetDocumentChanged())
            {
                // aks the user whether the document should be saved
                var save = _storageUi.PromptForSaveWhenDocumentChanged(_currentdocument);

                // when here is no value the user canceled the request
                if (!save.HasValue)
                    return false;

                // when true the document should be saved
                if (save.Value)
                {
                    DocumentInfo info = null;

                    info = _currentdocument != null
                        ? _storageUi.ResetDocument(_currentdocument)
                        : _storageUi.PromptForSaveDocument(null);

                    // check whether the user cancelled the saving
                    if (info == null)
                    {
                        return false;
                    }

                    T document = _documentProvider.GetDocument();

                    TransferDocumentIntoStream(document, info.Content);

                    info.Content.Close();
                }
            }
            
            return true;
        }

        
        public string DocumentName
        {
            get { return _documentname;}
            set
            {
                if (!string.Equals(_documentname, value))
                {
                    _documentname = value;
                    RaisePropertyChanged();
                }
            }
        }

        public ICommand NewCommand { get; set; }

        public ICommand OpenCommand { get; set; }

        public ICommand SaveCommand { get; set; }

        public ICommand SaveAsCommand { get; set; }
        
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
