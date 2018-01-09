using EntityFrameworkCore.BootKit;
using System;
using System.Collections.Generic;
using System.Text;

namespace Quickflow.UnitTest
{
    public class DataTable4Test : DbRecord, IDbRecord
    {
        public String Name { get; set; }

        public DateTime UpdatedTime { get; set; }
    }
}
