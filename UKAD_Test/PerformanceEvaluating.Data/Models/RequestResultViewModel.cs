﻿namespace PerformanceEvaluating.Data.Models
{
    public class RequestResultViewModel
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public long Min { get; set; }
        public long Max { get; set; }
        public int StatusCode { get; set; }
    }
}