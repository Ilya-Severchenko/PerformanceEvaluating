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
using System.Xml;
using System;
using NLog;

namespace PerformanceEvaluating.Business.Services
{
    public class PerformanceEvaluatingService : IPerformanceEvaluatingService
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly IDomainRequestResultRepository _domainRequestResultRepository;
        private readonly IChildRequestResultRepository _childRequestResultRepository;

        public PerformanceEvaluatingService(IDomainRequestResultRepository domainRequestResultRepository, IChildRequestResultRepository childRequesResultRepository)
        {
            _domainRequestResultRepository = domainRequestResultRepository;
            _childRequestResultRepository = childRequesResultRepository;
        }

        public async Task EvaluateAsync(string url)
        {
            if (!url.StartsWith("http"))
            {
                url = $"http://{url}";
            }
            string sitemapUrl;
            if (!url.EndsWith("sitemap.xml"))
            {
                sitemapUrl = $"{url}/sitemap.xml";
            }
            else
            {
                sitemapUrl = url;
            }

            var httpClient = new HttpClient();
            var stopwatch = new Stopwatch();

            try
            {
                var domenResponse = await httpClient.GetAsync(url);
                var domainRequestResult = await _domainRequestResultRepository.AddAsync(new DomainRequestResult
                {
                    Url = url,
                    StatusCode = (int)domenResponse.StatusCode,
                });

            
                var sitemap = await httpClient.GetStringAsync(sitemapUrl);

                XmlDocument urldoc = new XmlDocument();
                urldoc.LoadXml(sitemap);

                var xnList = urldoc.LastChild.ChildNodes;

                if (xnList.Count > 0)
                {
                    foreach (XmlNode node in xnList)
                    {
                        var childUrl = node["loc"].InnerText;
                        if (!string.IsNullOrEmpty(childUrl))
                        {
                            stopwatch.Restart();
                            stopwatch.Start();
                            var childResponse = await httpClient.GetAsync(node["loc"].InnerText);
                            stopwatch.Stop();
                            await _childRequestResultRepository.AddAsync(new ChildRequestResult
                            {
                                Url = node["loc"].InnerText,
                                Attempt = stopwatch.ElapsedMilliseconds,
                                DomainRequestResultId = domainRequestResult.Id,
                                StatusCode = (int)childResponse.StatusCode,
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);                
            }
        }

        public async Task<List<RequestResultViewModel>> ShowDetailsAsync(int id)
        {
            var results = await _childRequestResultRepository.GetAllByParentIdAsync(id);

            var sortedResults = new List<RequestResultViewModel>();
            foreach (var res in results)
            {
                var viewModel = new RequestResultViewModel()
                {
                    Url = res.Url,
                    Min = await _childRequestResultRepository.GetMinValueByUrlAsync(res.Url),
                    Max = await _childRequestResultRepository.GetMaxValueByUrlAsync(res.Url),
                    StatusCode = res.StatusCode
                };
                sortedResults.Add(viewModel);
            }
            return sortedResults.OrderBy(_ => _.Min).ToList();
        }

        public async Task<List<RequestResultViewModel>> SortedMainTableAsync()
        {
            var results = await _domainRequestResultRepository.GetAllAsync();


            var groups = results.GroupBy(x => x.Url).Select(x => x.First());

            var viewResults = new List<RequestResultViewModel>();

            foreach (var res in groups)
            {
                var viewModel = new RequestResultViewModel()
                {
                    Id = res.Id,
                    Url = res.Url
                };
                viewResults.Add(viewModel);
            }

            return viewResults;
        }

        public async Task<MemoryStream> GraphOutputAsync(int parentId)
        {
            var sortedResults = await ShowDetailsAsync(parentId);

            Chart chart = new Chart();
            chart.Width = 6000;
            chart.Height = 1500;

            var area = new ChartArea();

            area.BackColor = Color.FromArgb(211, 223, 240);
            area.Name = "Result Chart";
            area.AxisX.IsLabelAutoFit = true;
            area.AxisY.IsLabelAutoFit = true;
            area.AxisX.Title = "URL address";
            area.AxisX.TitleFont = new Font("Verdana,Arial,Helvetica,sans-serif", 20F, FontStyle.Bold);
            area.AxisY.Title = "Response time, ms";
            area.AxisY.TitleFont = new Font("Verdana,Arial,Helvetica,sans-serif", 20F, FontStyle.Bold);
            area.AxisX.LabelStyle.Font = new Font("Verdana,Arial,Helvetica,sans-serif", 16F, FontStyle.Regular);
            area.AxisY.LabelStyle.Font = new Font("Verdana,Arial,Helvetica,sans-serif", 16F, FontStyle.Regular);
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
            legends.Font = new Font("Verdana,Arial,Helvetica,sans-serif", 20F, FontStyle.Bold);

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
            chart.ImageType = ChartImageType.Jpeg;
            chart.SaveImage(returnStream);
            returnStream.Position = 0;

            return returnStream;
        }
    }
}
