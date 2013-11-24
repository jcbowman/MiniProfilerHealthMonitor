using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MiniProfilerHealthMonitor.Models
{
    public class ActivityQueryDto
    {
        public string Id { get; set; }
        public string Command { get; set; }
        public decimal Duration { get; set; }
    }
}