using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace MiniProfilerHealthMonitor.Models
{
    public class ActivityDto
    {
        public string Id { get; set; }

        public string Name { get; set; }
        public decimal Duration { get; set; }

        [DisplayName("Begin Time")]
        public DateTime BeginTime { get; set; }
    }
}