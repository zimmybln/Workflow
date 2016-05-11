using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using BuildActivities.Annotations;

namespace BuildActivities.Designs
{
    [Serializable]
    public class BuildDesignerOptions : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _executable;

        public BuildDesignerOptions()
        {
            
        }

        public string Executable
        {
            get { return _executable;}
            set
            {
                if (String.Compare(_executable, value, StringComparison.InvariantCulture) != 0)
                {
                    _executable = value;
                    RaisePropertyChanged();
                }
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
