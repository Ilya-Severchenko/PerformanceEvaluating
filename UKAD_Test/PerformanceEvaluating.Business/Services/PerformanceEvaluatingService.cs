using PerformanceEvaluating.Business.Interfaces;
using PerformanceEvaluating.Data.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.UI.DataVisualization.Charting;
using System.Drawing;
using System.IO;

namespace PerformanceEvaluating.Business.Services
{
    public class PerformanceEvaluatingService : IPerformanceEvaluatingService
    {
        private readonly IRequestResultRepository _requestResultRepository;

        public PerformanceEvaluatingService(IRequestResultRepository requestResultRepository)
        {
            _requestResultRepository = requestResultRepository;
        }

        public async Task EvaluateAsync(string url)
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

        public async Task<List<RequestResult>> ShowDetailsAsync(string url)
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

        public async Task<List<RequestResultViewModel>> SortedMainTableAsync()
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

        public async Task<MemoryStream> GraphOutputAsync()
        {
            var sortedResults = await SortedMainTableAsync();
           
            Chart chart = new Chart();
            chart.Width = 1000;
            chart.Height = 250;

            var area = new ChartArea();

            area.BackColor = Color.FromArgb(211, 223, 240);
            area.Name = "Result Chart";
            area.AxisX.IsLabelAutoFit = true;
            area.AxisY.IsLabelAutoFit = true;
            area.AxisX.Title = "URL address";
            area.AxisX.TitleFont = new Font("Verdana,Arial,Helvetica,sans-serif", 10F, FontStyle.Bold);
            area.AxisY.Title = "Response time, ms";
            area.AxisY.TitleFont = new Font("Verdana,Arial,Helvetica,sans-serif", 10F, FontStyle.Bold);
            area.AxisX.LabelStyle.Font = new Font("Verdana,Arial,Helvetica,sans-serif", 8F, FontStyle.Regular);
            area.AxisY.LabelStyle.Font = new Font("Verdana,Arial,Helvetica,sans-serif", 8F, FontStyle.Regular);
            area.AxisY.LineColor = Color.FromArgb(64, 64, 64, 64);
            area.AxisX.LineColor = Color.FromArgb(64, 64, 64, 64);
            area.AxisY.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);
            area.AxisX.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);
            area.AxisX.Interval = 1;
            area.AxisY.MajorGrid.Interval = 200;

            StripLine stripLine = new StripLine();
            stripLine.Interval = 50;
            stripLine.BorderColor = Color.FromArgb(64, 64, 64, 64);
            area.AxisY.StripLines.Add(stripLine);

            chart.ChartAreas.Add(area);

            var legends = new Legend();
            legends.IsDockedInsideChartArea = true;
            legends.Font = new Font("Verdana,Arial,Helvetica,sans-serif", 10F, FontStyle.Bold);

            chart.Legends.Add(legends);

            var seriesMin = new Series();
            var seriesMax = new Series();

            seriesMin.ChartType = SeriesChartType.Column;
            foreach (var item in sortedResults)
            {
                seriesMin.Points.AddXY(item.Url, item.Min);
                seriesMax.Points.AddY(item.Max);
            }
            seriesMin.Color = Color.Blue;
            seriesMax.Color = Color.Red;

            seriesMin.IsValueShownAsLabel = true;
            seriesMax.IsValueShownAsLabel = true;
            seriesMin.SmartLabelStyle.AllowOutsidePlotArea = LabelOutsidePlotAreaStyle.Yes;
            seriesMax.SmartLabelStyle.AllowOutsidePlotArea = LabelOutsidePlotAreaStyle.Yes;
            seriesMin.LabelForeColor = Color.Blue;
            seriesMax.LabelForeColor = Color.Red;

            seriesMin.LegendText = "MinValue";
            seriesMax.LegendText = "MaxValue";

            chart.Series.Add(seriesMin);
            chart.Series.Add(seriesMax);

            var returnStream = new MemoryStream();
            chart.ImageType = ChartImageType.Png;
            chart.SaveImage(returnStream);
            returnStream.Position = 0;

            return returnStream;
        }
    }
}
