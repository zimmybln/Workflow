using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Designer.Contracts;

namespace BuildActivities.Designs
{
    public class BuildDesignerOptionsFactory : IDesignerOptionsUiFactory
    {
        private DialogResources res = new DialogResources();

        public object GetOptionsUi()
        {
            return  res["BuildOptionsTemplate"] as DataTemplate;
        }
    }
}
