using PerformanceEvaluating.Business.Interfaces;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PerformanceEvaluating.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRequestResultRepository _requestResultRepository;
        private readonly IServiceForHomeController _serviceForHomeController;
        public HomeController(IRequestResultRepository requestResultRepository, IServiceForHomeController serviceForHomeController)
        {
            _requestResultRepository = requestResultRepository;
            _serviceForHomeController = serviceForHomeController;
        }

        public async Task<ActionResult> Index()
        {
            var table = await _serviceForHomeController.SortedMainTable();
            
            return View(table);
        }

        [HttpPost]
        public async Task<ActionResult> Evaluate(string url)
        {
            await _serviceForHomeController.Evaluate(url);
            
            return RedirectToAction("Index");
        }
        public async Task<ActionResult> DeleteAsync(string url)
        {
            await _requestResultRepository.DeleteAllByUrlAsync(url);

            return RedirectToAction("Index");
        }

        public async Task<ActionResult> ShowDetails(string url)
        {
            var details = await _serviceForHomeController.ShowDetails(url);
            
            return View(details);
        }

    }
}