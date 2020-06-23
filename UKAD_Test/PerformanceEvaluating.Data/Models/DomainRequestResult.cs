namespace PerformanceEvaluating.Data.Models
{
    public class DomainRequestResult
    {        
        public int Id { get; set; }
        public string Url { get; set; }
        public ChildRequestResult Child { get; set; }
        //public long Attempt { get; set; }
        public int StatusCode { get; set; }
    }
}
