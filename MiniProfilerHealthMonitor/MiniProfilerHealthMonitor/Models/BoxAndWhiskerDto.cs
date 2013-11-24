using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MiniProfilerHealthMonitor.Models
{
    public class BoxAndWhiskerDto
    {
        public string Name { get; set; }
        public decimal FirstQuartileMinimum { get; set; }
        public decimal SecondQuartileMinimum { get; set; }
        public decimal Median { get; set; }
        public decimal ThirdQuartileMaximum { get; set; }
        public decimal FourthQuartileMaximum { get; set; }

        public decimal AverageDuration { get; set; }
    }
}