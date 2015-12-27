using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Designer.Components
{
    public class ModelBase : INotifyPropertyChanged
    {
        private LoadedAdapter _loadedadapter;

        public ModelBase()
        {
            _loadedadapter = new LoadedAdapter(OnLoaded);
        }
        
        public LoadedAdapter LoadedAdapter
        {
            get { return _loadedadapter; }
        }

        protected virtual void OnLoaded()
        {

        }


        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged([CallerMemberName] string name = "")
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                return;
            }

            var p = PropertyChanged;

            if (p != null)
            {
                p(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion

    }
}
