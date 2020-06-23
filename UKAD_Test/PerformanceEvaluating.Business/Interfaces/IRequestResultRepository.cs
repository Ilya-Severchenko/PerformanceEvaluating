﻿using System.Collections.Generic;
using System.Threading.Tasks;
using PerformanceEvaluating.Data.Models;

namespace PerformanceEvaluating.Business.Interfaces
{
    public interface IRequestResultRepository
    {
        Task<DomainRequestResult> GetByIdAsync(int id);
        Task<DomainRequestResult> GetByUrlAsync(string url);
        Task<IEnumerable<DomainRequestResult>> GetAllAsync();
        Task<DomainRequestResult> AddAsync(DomainRequestResult entity);
        Task<bool> DeleteAsync(int id);
        Task<bool> DeleteAllByUrlAsync(string url);
        Task<long> GetMaxValueByUrlAsync(string url);
        Task<long> GetMinValueByUrlAsync(string url);
        Task<IEnumerable<long>> GetAllAttemptsAsync(string url);
        Task<IEnumerable<DomainRequestResult>> GetAllByUrlAsync(string url);
    }
}
