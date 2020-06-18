using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using PerformanceEvaluating.Business.Interfaces;
using PerformanceEvaluating.Data;
using PerformanceEvaluating.Data.Models;

namespace PerformanceEvaluating.Business.Repositories
{
    public class RequestResultRepository : IRequestResultRepository
    {
        private readonly ApplicationDbContext _context;

        public RequestResultRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<RequestResult> GetByIdAsync(int id)
        {
            var result = await _context.RequestResults.FirstOrDefaultAsync(_ => _.Id == id);
            return result;
        }

        public async Task<RequestResult> GetByUrlAsync(string url)
        {
            var result = await _context.RequestResults.FirstOrDefaultAsync(_ => _.Url == url);
            return result;
        }

        public async Task<IEnumerable<RequestResult>> GetAllAsync()
        {
            return await _context.RequestResults.ToListAsync();
        }

        public async Task<bool> UpdateAsync(RequestResult entity)
        {
            throw new NotImplementedException();
        }

        public async Task<RequestResult> AddAsync(RequestResult entity)
        {
            try
            {
                var result = _context.RequestResults.Add(entity);
                await _context.SaveChangesAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var result = await GetByIdAsync(id);

            if (result != null)
            {
                try
                {
                    _context.RequestResults.Remove(result);
                    await _context.SaveChangesAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }

            throw new NullReferenceException($"Result with id={id} not found");
        }


        public async Task<long> GetMaxValueByUrlAsync(string url)
        {
            var results = await _context.RequestResults.Where(_ => _.Url == url).ToListAsync();
            var attempt = results.Select(_ => _.Attempt).Max();
            return attempt;
        }

        public async Task<long> GetMinValueByUrlAsync(string url)
        {
            var results = await _context.RequestResults.Where(_ => _.Url == url).ToListAsync();
            var attempt = results.Select(_ => _.Attempt).Min();
            return attempt;
        }
        public async Task<IEnumerable<long>> GetAllAttemptsAsync(string url)
        {
            var results = await _context.RequestResults.Where(_ => _.Url == url).ToListAsync();
            var attempts = results.Select(_ => _.Attempt);
            return attempts;
        }

        public async Task<IEnumerable<RequestResult>> GetAllByUrlAsync(string url)
        {
            return await _context.RequestResults.Where(_ => _.Url == url).ToListAsync();
        }

        public async Task<bool> DeleteAllByUrlAsync(string url)
        {
            var results = await GetAllByUrlAsync(url);
            if (results.Any())
            {
                _context.RequestResults.RemoveRange(results);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
