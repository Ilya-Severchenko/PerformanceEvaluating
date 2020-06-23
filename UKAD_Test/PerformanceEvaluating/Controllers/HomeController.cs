using PerformanceEvaluating.Business.Interfaces;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PerformanceEvaluating.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDomainRequestResultRepository _requestResultRepository;
        private readonly IPerformanceEvaluatingService _performanceEvaluatingService;

        public HomeController(IDomainRequestResultRepository requestResultRepository, IPerformanceEvaluatingService performanceEvaluatingService)
        {
            _requestResultRepository = requestResultRepository;
            _performanceEvaluatingService = performanceEvaluatingService;
        }

        public async Task<ActionResult> Index()
        {
            var table = await _performanceEvaluatingService.SortedMainTableAsync();
            
            return View(table);
        }

        [HttpPost]
        public async Task<ActionResult> Evaluate(string url)
        {
            await _performanceEvaluatingService.EvaluateAsync(url);
            
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Delete(string url)
        {
            await _requestResultRepository.DeleteAllByUrlAsync(url);

            return RedirectToAction("Index");
        }

        public async Task<ActionResult> ShowDetails(string url)
        {
            var details = await _performanceEvaluatingService.ShowDetailsAsync(url);
            
            return View(details);
        }

        public async Task<ActionResult> GraphOutput()
        {
            var stream = await _performanceEvaluatingService.GraphOutputAsync();

            return File(stream.GetBuffer(), @"image/png");
        }

    }
}