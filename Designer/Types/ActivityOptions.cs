using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace Designer.Types
{
    public class ActivityOption
    {
        public string Name { get; set; }

        public object Content { get; set; }

        public object Data { get; set; }

        public string DataTypeName { get; set; }

        public string GroupName { get; set; }
    }


    public class ActivityOptionGroup
    {
        public ActivityOptionGroup(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public List<ActivityOption> Items { get; } = new List<ActivityOption>(); 
    }
}
