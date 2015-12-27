using Designer.Components;
using System;
using System.Activities;
using System.Activities.Statements;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Designer.Models
{
    public class MainWindowModel : ModelBase
    {
        private Activity _currentworkflow = null;


        protected override void OnLoaded()
        {
            CurrentWorkflow = CreateDefaultWorkflow();
        }

        private Activity CreateDefaultWorkflow()
        {
            return new Flowchart() { DisplayName = "Default" };
        }
        

        #region Properties

        public Activity CurrentWorkflow
        {
            get { return _currentworkflow; }
            set
            {
                if (!object.Equals(value, _currentworkflow))
                {
                    _currentworkflow = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion



    }
}
