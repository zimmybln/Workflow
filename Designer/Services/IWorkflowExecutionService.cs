﻿using System;
using System.Activities;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Designer.Components;

namespace Designer.Services
{
    public class WorkflowExecutionOptions
    {
        public List<TextWriter> TraceListeners { get; } = new List<TextWriter>(); 
    }


    public interface IWorkflowExecutionService : IService
    {
        void Execute(Activity activity);

        void Execute(Activity activity, WorkflowExecutionOptions options);
    }
}
