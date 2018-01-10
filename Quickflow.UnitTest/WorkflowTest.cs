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
using ExpressionEvaluator;
using System.Collections;
using Newtonsoft.Json.Linq;
using DotNetToolkit;

namespace Quickflow.UnitTest
{
    [TestClass]
    public class WorkflowTest : TestEssential
    {
        [TestMethod]
        public void DecisionActivityTest()
        {
            var wf = new WorkflowEngine
            {
                WorkflowId = "db2f768c-f6f1-41e1-a869-c3c9e587ea74",
                TransactionId = Guid.NewGuid().ToString()
            };

            dc.DbTran(async () =>
            {
                var result = await wf.Run(dc, new { Name = "Haiping Chen" });
                Assert.IsTrue(result.Data.GetValue("Name").ToString() == "Haiping Chen");
            });
        }

        [TestMethod]
        public void EmailInAwsActivityTest()
        {
            var wf = new WorkflowEngine
            {
                WorkflowId = "a25ea38c-ee44-4acc-8bdf-a8e83c133803",
                TransactionId = Guid.NewGuid().ToString()
            };

            dc.DbTran(async () =>
            {
                var result = await wf.Run(dc, new { Name = "Haiping Chen" });
            });
        }

        [TestMethod]
        public void DataMappingActivityTest()
        {
            var wf = new WorkflowEngine
            {
                WorkflowId = "3bf71bdb-254d-4ad0-9de4-a64dcfe47770",
                TransactionId = Guid.NewGuid().ToString()
            };

            var result = wf.Run(dc, new { FullName = "Haiping Chen", Email = "haiping008@gmail.com" });

            Assert.IsTrue(result.Result.Data.GetValue("ToAddresses").ToString() == "haiping008@gmail.com");
        }

        [TestMethod]
        public void DbRecordUpdateActivityTest()
        {
            var wf = new WorkflowEngine
            {
                WorkflowId = "e256fef6-b0fa-4b25-85ef-635444d3826d",
                TransactionId = Guid.NewGuid().ToString()
            };
            
            dc.DbTran(() =>
            {
                var result = wf.Run(dc, new { Name = "Haiping", Total = new Random().Next(1000), UpdatedTime = DateTime.Parse("1/10/2018 2:21:30 PM") });

                Assert.IsTrue(result.Result.Data.GetValue("Name").ToString() == "Haiping");
            });

        }
    }
}
