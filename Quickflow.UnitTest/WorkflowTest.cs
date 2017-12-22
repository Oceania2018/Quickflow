using CustomEntityFoundation;
using CustomEntityFoundation.Entities;
using EntityFrameworkCore.BootKit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quickflow.Core;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Linq.Dynamic.Core;
using CustomEntityFoundation.Bundles;
using ExpressionEvaluator;
using System.Collections;
using Newtonsoft.Json.Linq;

namespace Quickflow.UnitTest
{
    [TestClass]
    public class WorkflowTest : Database
    {
        [TestMethod]
        public void Workflow1Test()
        {
            var wf = new WorkflowEngine { WorkflowId = "db2f768c-f6f1-41e1-a869-c3c9e587ea74" };

            dc.DbTran(async () =>
            {
                await wf.Run(dc, new { Name = "Haiping Chen" });
            });
        }
    }
}
