using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Designer.Contracts.Attributes
{
    public class DesignerActivityOptionsAttribute : Attribute
    {
        public DesignerActivityOptionsAttribute(Type designerOptionsUiFactory, Type designerOptionsDataContextType)
        {
            DesignerOptionsUiFactoryType = designerOptionsUiFactory;
            DesignerOptionsDataContextType = designerOptionsDataContextType;
        }

        public Type DesignerOptionsUiFactoryType { get; }

        public Type DesignerOptionsDataContextType { get; }
    }
}
