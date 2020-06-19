using PerformanceEvaluating.Business.Interfaces;
using PerformanceEvaluating.Data.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PerformanceEvaluating.Business.Services
{
    public class ServiceForHomeController : IServiceForHomeController
    {
        private readonly IRequestResultRepository _requestResultRepository;
        public ServiceForHomeController(IRequestResultRepository requestResultRepository)
        {
            _requestResultRepository = requestResultRepository;
        }
        public async Task Evaluate(string url)
        {
            if (!url.StartsWith("http"))
            {
                url = $"http://{url}";
            }

            var httpClient = new HttpClient();
            var stopwatch = new Stopwatch();

            stopwatch.Start();
            var response = await httpClient.GetAsync(url);
            stopwatch.Stop();

            await _requestResultRepository.AddAsync(new RequestResult
            {
                Url = url,
                Attempt = stopwatch.ElapsedMilliseconds,
                StatusCode = (int)response.StatusCode,
            });
        }

        public async Task<List<RequestResult>> ShowDetails(string url)
        {
            var results = await _requestResultRepository.GetAllByUrlAsync(url);

            var viewResults = new List<RequestResult>();
            foreach (var res in results)
            {
                var viewModel = new RequestResult()
                {
                    Url = url,
                    Attempt = res.Attempt,
                    StatusCode = res.StatusCode
                };
                viewResults.Add(viewModel);
            }
            return viewResults;
        }

        public async Task<List<RequestResultViewModel>> SortedMainTable()
        {
            var results = await _requestResultRepository.GetAllAsync();

            var groups = results.Select(_ => _.Url).Distinct();

            var sortedResults = new List<RequestResultViewModel>();

            foreach (var res in groups)
            {
                var viewModel = new RequestResultViewModel()
                {
                    Url = res,
                    Min = await _requestResultRepository.GetMinValueByUrlAsync(res),
                    Max = await _requestResultRepository.GetMaxValueByUrlAsync(res)
                };
                sortedResults.Add(viewModel);
            }

            return sortedResults.OrderBy(_ => _.Min).ToList();
        }
    }
}
