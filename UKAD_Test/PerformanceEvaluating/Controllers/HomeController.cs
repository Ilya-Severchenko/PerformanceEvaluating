using PerformanceEvaluating.Business.Interfaces;
using PerformanceEvaluating.Data.Models;
using PerformanceEvaluating.ViewModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PerformanceEvaluating.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRequestResultRepository _requestResultRepository;

        public HomeController(IRequestResultRepository requestResultRepository)
        {
            _requestResultRepository = requestResultRepository;
        }

        public async Task<ActionResult> Index()
        {
            var results = await _requestResultRepository.GetAllAsync();

            var groups = results.Select(_ => _.Url).Distinct();
            var sortedResults = new List<RequestResultViewModel>();
            foreach (var res in groups)
            {
                var vm = new RequestResultViewModel()
                {
                    Url = res,
                    Min = await _requestResultRepository.GetMinValueByUrlAsync(res),
                    Max = await _requestResultRepository.GetMaxValueByUrlAsync(res)
                };
                sortedResults.Add(vm);
            }

            return View(sortedResults.OrderBy(_ => _.Min));
        }

        [HttpPost]
        public async Task<ActionResult> Evaluate(string url)
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

            return RedirectToAction("Index");
        }
        public async Task<ActionResult> DeleteAsync(string url)
        {
            await _requestResultRepository.DeleteAllByUrlAsync(url);

            return RedirectToAction("Index");
        }

        public async Task<ActionResult> ShowDetails(string url)
        {
            var results = await _requestResultRepository.GetAllByUrlAsync(url);

            var viewResults = new List<RequestResult>();
            foreach (var res in results)
            {
                var vm = new RequestResult()
                {
                    Url = url,
                    Attempt = res.Attempt,
                    StatusCode = res.StatusCode
                };
                viewResults.Add(vm);
            }

            return View(viewResults);
        }

    }
}