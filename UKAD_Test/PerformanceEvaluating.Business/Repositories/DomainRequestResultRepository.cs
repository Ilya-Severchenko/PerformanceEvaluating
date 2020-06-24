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
    public class DomainRequestResultRepository : IDomainRequestResultRepository
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly ApplicationDbContext _context;

        public DomainRequestResultRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DomainRequestResult> GetByIdAsync(int id)
        {
            return await _context.DomainRequestResults.FirstOrDefaultAsync(_ => _.Id == id);             
        }

        public async Task<DomainRequestResult> GetByUrlAsync(string url)
        {
            return await _context.DomainRequestResults.FirstOrDefaultAsync(_ => _.Url == url);            
        }

        public async Task<IEnumerable<DomainRequestResult>> GetAllAsync()
        {
            return await _context.DomainRequestResults.ToListAsync();
        }
        
        public async Task<DomainRequestResult> AddAsync(DomainRequestResult entity)
        {
            try
            {
                var adding = _context.DomainRequestResults.Add(entity);
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
                    _context.DomainRequestResults.Remove(deletion);
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

        public async Task<IEnumerable<DomainRequestResult>> GetAllByUrlAsync(string url)
        {
            return await _context.DomainRequestResults.Where(_ => _.Url == url).ToListAsync();
        }

        public async Task<bool> DeleteAllByUrlAsync(string url)
        {
            var results = await GetAllByUrlAsync(url);
            if (results.Any())
            {
                _context.DomainRequestResults.RemoveRange(results);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
