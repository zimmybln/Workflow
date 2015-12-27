using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Designer.Components
{
    public class ActionAdapter<T>
        where T : EventArgs
    {
        private readonly Action<object, T> mv_actEventAction;

        public ActionAdapter(Action<object, T> Action)
        {
            mv_actEventAction = Action;
        }

        public Action<object, T> Action
        {
            get { return mv_actEventAction; }
        }
    }
}
