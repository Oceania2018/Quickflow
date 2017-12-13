using CustomEntityFoundation;
using CustomEntityFoundation.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quickflow.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Quickflow.UnitTest
{
    [TestClass]
    public class WorkflowTest
    {
        [TestMethod]
        public void Workflow1()
        {
            var dc = Database.GetDatabase();

            var wf = new WorkflowEngine { WorkflowId = "87647613-df6e-435c-b13c-b0f42397cbc0" };

            dc.Transaction<IDbRecord>(async delegate
            {
                await wf.Run(dc, new { });
            });
        }
    }
}
