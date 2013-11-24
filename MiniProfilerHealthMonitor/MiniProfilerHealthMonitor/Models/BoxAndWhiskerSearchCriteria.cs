using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MiniProfilerHealthMonitor.Models
{
    public class BoxAndWhiskerSearchCriteria
    {
        [DisplayName("Begin Date")]
        [Required]
        public DateTime BeginDate { get; set; }

        [DisplayName("End Date")]
        [Required]
        public DateTime EndDate { get; set; }
    }
}