using PerformanceEvaluating.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PerformanceEvaluating.Business.Interfaces
{
    public interface IServiceForHomeController
    {
        Task Evaluate(string url);
        Task<List<RequestResultViewModel>> SortedMainTable();
        Task<List<RequestResult>> ShowDetails(string url);
    }
}
