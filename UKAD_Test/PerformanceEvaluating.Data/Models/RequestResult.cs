
namespace PerformanceEvaluating.Data.Models
{
    public class RequestResult
    {        
        public int Id { get; set; }
        public string Url { get; set; }
        public long Attempt { get; set; }
        public int StatusCode { get; set; }
    }
}
