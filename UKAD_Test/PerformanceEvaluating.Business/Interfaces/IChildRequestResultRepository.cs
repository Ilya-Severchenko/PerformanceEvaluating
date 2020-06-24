using PerformanceEvaluating.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PerformanceEvaluating.Business.Interfaces
{
    public interface IChildRequestResultRepository
    {
        Task<ChildRequestResult> GetByIdAsync(int id);
        Task<IEnumerable<ChildRequestResult>> GetAllByParentIdAsync(int parentId);
        Task<ChildRequestResult> GetByUrlAsync(string url);
        Task<IEnumerable<ChildRequestResult>> GetAllAsync();
        Task<ChildRequestResult> AddAsync(ChildRequestResult entity);
        Task<bool> DeleteAsync(int id);
        Task<bool> DeleteAllByUrlAsync(string url);
        Task<long> GetMaxValueByUrlAsync(string url);
        Task<long> GetMinValueByUrlAsync(string url);
        Task<IEnumerable<long>> GetAllAttemptsAsync(string url);
        Task<IEnumerable<ChildRequestResult>> GetAllByUrlAsync(string url);
    }
}
