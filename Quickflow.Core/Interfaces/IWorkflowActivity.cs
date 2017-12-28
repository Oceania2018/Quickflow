using EntityFrameworkCore.BootKit;
using Quickflow.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Quickflow.Core.Interfacess
{
    public interface IWorkflowActivity
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc">data context</param>
        /// <param name="wf">workflow definition</param>
        /// <param name="activity">current activity</param>
        /// <param name="preActivity">previous acivity</param>
        /// <returns></returns>
        Task Run(Database dc, Workflow wf, ActivityInWorkflow activity, ActivityInWorkflow preActivity);
    }
}
