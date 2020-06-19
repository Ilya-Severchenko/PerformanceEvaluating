
using System.Collections.Generic;
using System.Threading.Tasks;
using PerformanceEvaluating.Data.Models;

namespace PerformanceEvaluating.Business.Interfaces
{
    public interface IRequestResultRepository
    {
        Task<RequestResult> GetByIdAsync(int id);
        Task<RequestResult> GetByUrlAsync(string url);
        Task<IEnumerable<RequestResult>> GetAllAsync();
        Task<RequestResult> AddAsync(RequestResult entity);
        Task<bool> DeleteAsync(int id);
        Task<bool> DeleteAllByUrlAsync(string url);
        Task<long> GetMaxValueByUrlAsync(string url);
        Task<long> GetMinValueByUrlAsync(string url);
        Task<IEnumerable<long>> GetAllAttemptsAsync(string url);
        Task<IEnumerable<RequestResult>> GetAllByUrlAsync(string url);
    }
}
