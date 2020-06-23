using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceEvaluating.Data.Models
{
    public class ChildRequestResult
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public long Attempt { get; set; }
        public int StatusCode { get; set; }
    }
}
