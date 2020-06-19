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
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly ApplicationDbContext _context;

        public RequestResultRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<RequestResult> GetByIdAsync(int id)
        {
            return await _context.RequestResults.FirstOrDefaultAsync(_ => _.Id == id);             
        }

        public async Task<RequestResult> GetByUrlAsync(string url)
        {
            return await _context.RequestResults.FirstOrDefaultAsync(_ => _.Url == url);            
        }

        public async Task<IEnumerable<RequestResult>> GetAllAsync()
        {
            return await _context.RequestResults.ToListAsync();
        }
        
        public async Task<RequestResult> AddAsync(RequestResult entity)
        {
            try
            {
                var adding = _context.RequestResults.Add(entity);
                await _context.SaveChangesAsync();
                return adding;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var deletion = await GetByIdAsync(id);

            if (deletion != null)
            {
                try
                {
                    _context.RequestResults.Remove(deletion);
                    await _context.SaveChangesAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message);
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
