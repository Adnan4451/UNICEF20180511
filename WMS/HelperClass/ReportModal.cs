using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WMS.HelperClass
{
    public class VMTimeAverage
    {
        public string Totals { get; set; }
        public string Actuals { get; set; }
        public string Time { get; set; }
        public TimeSpan TimeStart { get; set; }
        public TimeSpan TimeEnd { get; set; }
    }
    public class VMTimeAverageDeptWise
    {
        public string DeptActuals { get; set; }
        public string Time { get; set; }
        public string DeptName { get; set; }
        public string DeptTotals { get; set; }
        public TimeSpan TimeStart { get; set; }
        public TimeSpan TimeEnd { get; set; }
    }
}