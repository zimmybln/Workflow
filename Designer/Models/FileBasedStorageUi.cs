using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Designer.Components.Models;
using Microsoft.Win32;

namespace Designer.Models
{
    public class FileBasedStorageUi : IStorageUi
    {
        public bool? PromptForSaveWhenDocumentChanged(DocumentInfo document)
        {
            var result = MessageBox.Show("Das Dokument hat sich geändert. Soll es gespeichert werden?", "",
                    MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            
            if (result == MessageBoxResult.Yes)
            {
                return true;
            }

            if (result == MessageBoxResult.No)
            {
                return false;
            }
            
            return null;
        }

        public DocumentInfo PromptForOpenDocument()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Workflow Definition File (*.wdef)|*.wdef|All files|*.*";
            dlg.FilterIndex = 0;
            dlg.AddExtension = true;
            dlg.CheckPathExists = true;
            dlg.CheckFileExists = true;
            dlg.DefaultExt = "wdef";

            if (dlg.ShowDialog() != true)
                return null;

            return new DocumentInfo()
            {
                Content = new FileStream(dlg.FileName, FileMode.Open),
                Identification = new DocumentIdentification(dlg.FileName)
            };
        }

        public DocumentInfo PromptForSaveDocument(DocumentInfo document)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Workflow Definition File (*.wdef)|*.wdef|All files|*.*";
            dlg.FilterIndex = 0;
            dlg.OverwritePrompt = true;
            dlg.AddExtension = true;
            dlg.CheckPathExists = true;
            dlg.DefaultExt = "wdef";

            if (dlg.ShowDialog() != true)
                return null;

            // save the current workflow
            if (File.Exists(dlg.FileName))
                File.Delete(dlg.FileName);

            return new DocumentInfo()
            {
                Content = new FileStream(dlg.FileName, FileMode.OpenOrCreate),
                Identification = new DocumentIdentification(dlg.FileName)
            };
        }

        public DocumentInfo OpenDocument(DocumentInfo document)
        {
            if (String.IsNullOrEmpty(document?.ToString()))
                return null;

            return new DocumentInfo()
            {
                Content = new FileStream(document.ToString(), FileMode.Open),
                Identification = new DocumentIdentification(document.ToString())
            };
        }

        public DocumentInfo ResetDocument(DocumentInfo document)
        {
            if (String.IsNullOrEmpty(document?.ToString()))
                return null;

            if (File.Exists(document.ToString()))
                File.Delete(document.ToString());

            return new DocumentInfo()
            {
                Content = new FileStream(document.ToString(), FileMode.OpenOrCreate),
                Identification = new DocumentIdentification(document.ToString())
            };
        }

        public void PromptException(Exception exception)
        {
            string message = null;

            if (exception is InvalidOperationException)
            { 
              message = "Unable to open the file";
            }

            if (message != null)
            {
                MessageBox.Show(message);
            }
            
        }

        public string DefaultExtension { get; set; }

        public string FormatName { get; set; }

        public string Title { get; set; }


    }
}
