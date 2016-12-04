using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Designer.Contracts;

namespace BuildActivities.Test.Extensions
{
    public class TestDesignerOptionsExtensions : IDesignerOptionsExtension
    {
        private Dictionary<Type, object> _dictionaryExtensions = new Dictionary<Type, object>();
         
        public void AddExtension(object extension)
        {
            _dictionaryExtensions.Add(extension.GetType(), extension);    
        }
        
        public T GetOptions<T>() where T : class
        {
            return _dictionaryExtensions[typeof (T)] as T;
        }
    }
}
