using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PerformanceEvaluating.ViewModel
{
    public class RequestResultViewModel
    {
        public string Url { get; set; }
        public long Min { get; set; }
        public long Max { get; set; }
        public int StatusCode { get; set; }
    }
}