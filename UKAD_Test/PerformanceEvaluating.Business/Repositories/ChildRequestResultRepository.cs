using PerformanceEvaluating.Business.Interfaces;
using PerformanceEvaluating.Data;
using PerformanceEvaluating.Data.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceEvaluating.Business.Repositories
{
    public class ChildRequestResultRepository : IChildRequesResult
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly ApplicationDbContext _context;

        public ChildRequestResultRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ChildRequestResult> GetByIdAsync(int id)
        {
            return await _context.ChildRequestResults.FirstOrDefaultAsync(_ => _.Id == id);
        }

        public async Task<ChildRequestResult> AddAsync(ChildRequestResult entity)
        {
            try
            {
                var adding = _context.ChildRequestResults.Add(entity);
                await _context.SaveChangesAsync();
                return adding;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<ChildRequestResult>> GetAllAsync()
        {
            return await _context.ChildRequestResults.ToListAsync();
        }

        public async Task<ChildRequestResult> GetByUrlAsync(string url)
        {
            return await _context.ChildRequestResults.FirstOrDefaultAsync(_ => _.Url == url);
        }

        public async Task<IEnumerable<ChildRequestResult>> GetAllByUrlAsync(string url)
        {
            return await _context.ChildRequestResults.Where(_ => _.Url == url).ToListAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var deletion = await GetByIdAsync(id);

            if (deletion != null)
            {
                try
                {
                    _context.ChildRequestResults.Remove(deletion);
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

        public async Task<bool> DeleteAllByUrlAsync(string url)
        {
            var results = await GetAllByUrlAsync(url);
            if (results.Any())
            {
                _context.ChildRequestResults.RemoveRange(results);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<long>> GetAllAttemptsAsync(string url)
        {
            var results = await _context.ChildRequestResults.Where(_ => _.Url == url).ToListAsync();
            var attempts = results.Select(_ => _.Attempt);
            return attempts;
        }

        public async Task<long> GetMinValueByUrlAsync(string url)
        {
            var results = await _context.ChildRequestResults.Where(_ => _.Url == url).ToListAsync();
            var attempt = results.Select(_ => _.Attempt).Min();
            return attempt;
        }

        public async Task<long> GetMaxValueByUrlAsync(string url)
        {
            var results = await _context.ChildRequestResults.Where(_ => _.Url == url).ToListAsync();
            var attempt = results.Select(_ => _.Attempt).Max();
            return attempt;
        }

    }
}
