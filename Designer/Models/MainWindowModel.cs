﻿using Designer.Components;
using System;
using System.Activities;
using System.Activities.Core.Presentation.Factories;
using System.Activities.Statements;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Designer.Models
{
    public class MainWindowModel : ModelBase
    {
        private Activity _currentworkflow = null;

        public MainWindowModel()
        {
            ToolboxItems.Add(new ToolboxItemDescriptor("Steuerstrukturen", typeof(If)));

        }


        protected override void OnLoaded()
        {
            CurrentWorkflow = CreateDefaultWorkflow();

            

            //ToolboxItems.Add.AddRange(new[]
            //{
            //        new ToolboxItemDescriptor(resCourse, typeof(Sequence)),
            //        new ToolboxItemDescriptor(resCourse, typeof(If)),
            //        new ToolboxItemDescriptor(resCourse, typeof(System.Activities.Statements.Parallel)),
            //        new ToolboxItemDescriptor(resCourse, typeof(While)),
            //        new ToolboxItemDescriptor(resCourse, typeof(DoWhile)),
            //        new ToolboxItemDescriptor(resCourse, typeof(Assign)),
            //        // Delay kann erst wieder eingeblendet werden, wenn wir eine Lösung für das 
            //        // zeitgesteuerte Wiedererwecken von persisitierten Workflows gefunden haben
            //        //new ToolboxItemDescriptor(resCourse, typeof(Delay)),
            //        new ToolboxItemDescriptor(resCourse, typeof(TerminateWorkflow)),
            //        new ToolboxItemDescriptor(resCourse, typeof(ForEachWithBodyFactory<>)),
            //        new ToolboxItemDescriptor(resCourse, typeof(Switch<>)),
            //        new ToolboxItemDescriptor(resCourse, typeof(Pick)),
            //        new ToolboxItemDescriptor(resCourse, typeof(PickBranch)),
            //        new ToolboxItemDescriptor(resCourse, typeof(InvokeMethod))
            //    });

            ToolboxItems.Add(new ToolboxItemDescriptor("test", typeof(Sequence)));
        }

        private Activity CreateDefaultWorkflow()
        {
            return new Flowchart() { DisplayName = "Default" };
        }


        #region Properties

        public ToolboxItemDescriptorCollection ToolboxItems { get; private set; } = new ToolboxItemDescriptorCollection();

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
