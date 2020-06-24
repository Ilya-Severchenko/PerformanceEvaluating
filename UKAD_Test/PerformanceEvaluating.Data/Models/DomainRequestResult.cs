using System.Collections.Generic;

namespace PerformanceEvaluating.Data.Models
{
    public class DomainRequestResult
    {        
        public int Id { get; set; }
        public string Url { get; set; }
        public IEnumerable<ChildRequestResult> ChildRequestResults { get; set; }
        public int StatusCode { get; set; }
    }
}
