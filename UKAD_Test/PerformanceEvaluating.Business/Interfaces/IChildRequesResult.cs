using PerformanceEvaluating.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceEvaluating.Business.Interfaces
{
    public interface IChildRequesResult
    {
        Task<ChildRequestResult> GetByIdAsync(int id);
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
