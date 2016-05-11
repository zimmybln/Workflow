using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Designer.Contracts;

namespace Designer.Components.Workflow
{
    public class DesignerOptions : IDesignerOptionsExtension
    {
        private readonly SortedDictionary<Type,object> _options = new SortedDictionary<Type, object>();


        public void SomeMethod()
        {
            var method = new System.Diagnostics.StackTrace().GetFrame(1).GetMethod();

            MessageBox.Show(method.DeclaringType.Assembly.FullName + " " +  method.DeclaringType.Name + " " + method.Name);
        }

        public void AddOption(Type type, object optionsData)
        {
            _options.Add(type, optionsData);
        }

        public T GetOptions<T>()
            where T : class 
        {
            if (_options.ContainsKey(typeof(T)))
            {
                return _options[typeof (T)] as T;
            }


            return default(T);
        }
    }
}
