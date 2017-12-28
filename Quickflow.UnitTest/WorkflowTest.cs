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
                Assert.IsTrue(JObject.FromObject(result.Data)["Name"].ToString() == "Haiping Chen");
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
            var jObject = JObject.FromObject(result.Result.Data);

            Assert.IsTrue(jObject["ToAddresses"].ToString() == "haiping008@gmail.com");
        }
    }
}
