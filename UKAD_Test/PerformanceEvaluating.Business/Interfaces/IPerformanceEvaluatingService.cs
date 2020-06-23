using PerformanceEvaluating.Data.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PerformanceEvaluating.Business.Interfaces
{
    public interface IPerformanceEvaluatingService
    {
        Task EvaluateAsync(string url);
        Task<List<RequestResultViewModel>> SortedMainTableAsync();
        Task<List<DomainRequestResult>> ShowDetailsAsync(string url);
        Task<MemoryStream> GraphOutputAsync();
    }
}
