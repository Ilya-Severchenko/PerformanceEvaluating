namespace PerformanceEvaluating.Data.Models
{
    public class ChildRequestResult
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public long Attempt { get; set; }
        public int DomainRequestResultId { get; set; }
        public DomainRequestResult DomainRequestResult { get; set; }
        public int StatusCode { get; set; }
    }
}
