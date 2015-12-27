using System;
using System.Activities.Presentation.Validation;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Designer.Controls.Services
{
    public class ValidationErrorService : IValidationErrorService
    {
        private readonly ObservableCollection<EditorMessage> _designerMessages;

        public ValidationErrorService(ObservableCollection<EditorMessage> Messages)
        {
            if (Messages == null)
                throw new ArgumentNullException("Messages");

            _designerMessages = Messages;
            _designerMessages.Clear();
        }

        public void ShowValidationErrors(IList<ValidationErrorInfo> errors)
        {
            _designerMessages.Clear();
            errors.ToList().ForEach(err => _designerMessages.Add(
                new EditorMessage()
                {
                    Message = err.Message,
                    IsWarning = err.IsWarning,
                    PropertyName = err.PropertyName
                })
                );
        }
    }
}
