using EntityFrameworkCore.BootKit;
using System;
using System.Collections.Generic;
using System.Text;

namespace Quickflow.UnitTest
{
    public class DataTable4Test : DbRecord, IDbRecord
    {
        public String Name { get; set; }

        public Int32 Total { get; set; }

        public Decimal? Amount { get; set; }
    }
}
