using System.Diagnostics;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Search
{
    public sealed class RibbonSearchBenchmarkQueryResult
    {
        public string Query { get; set; } = string.Empty;
        public long DurationMs { get; set; }
        public int ResultCount { get; set; }
        public string TopCommandKey { get; set; } = string.Empty;
    }

    public sealed class RibbonSearchBenchmarkReport
    {
        public DateTime ExecutedAtUtc { get; set; } = DateTime.UtcNow;
        public int CandidateCount { get; set; }
        public int QueryCount { get; set; }
        public long TotalDurationMs { get; set; }
        public double AverageDurationMs { get; set; }
        public double P95DurationMs { get; set; }
        public double AverageResultCount { get; set; }
        public List<RibbonSearchBenchmarkQueryResult> QueryResults { get; set; } = [];
    }

    public static class RibbonSearchBenchmark
    {
        public static RibbonSearchBenchmarkReport Run(
            IEnumerable<string> queries,
            IEnumerable<SimpleItem> candidates,
            IReadOnlyDictionary<string, int>? commandUsage = null,
            int maxResults = 12,
            Func<SimpleItem, int>? scoreBoostProvider = null)
        {
            var queryList = queries?
                .Where(q => !string.IsNullOrWhiteSpace(q))
                .Select(q => q.Trim())
                .ToList() ?? [];

            var candidateList = candidates?
                .Where(c => c != null)
                .ToList() ?? [];

            var report = new RibbonSearchBenchmarkReport
            {
                ExecutedAtUtc = DateTime.UtcNow,
                CandidateCount = candidateList.Count,
                QueryCount = queryList.Count
            };

            if (queryList.Count == 0 || candidateList.Count == 0)
            {
                return report;
            }

            var durations = new List<long>(queryList.Count);
            int totalResults = 0;
            foreach (var query in queryList)
            {
                var sw = Stopwatch.StartNew();
                var matches = RibbonSearchIndex.BuildMatches(query, candidateList, commandUsage, maxResults, scoreBoostProvider);
                sw.Stop();

                long duration = Math.Max(0, sw.ElapsedMilliseconds);
                durations.Add(duration);
                totalResults += matches.Count;

                report.QueryResults.Add(new RibbonSearchBenchmarkQueryResult
                {
                    Query = query,
                    DurationMs = duration,
                    ResultCount = matches.Count,
                    TopCommandKey = matches.Count > 0 ? matches[0].CommandKey : string.Empty
                });
            }

            report.TotalDurationMs = durations.Sum();
            report.AverageDurationMs = durations.Count == 0 ? 0d : durations.Average();
            report.P95DurationMs = ComputePercentile(durations, 0.95);
            report.AverageResultCount = queryList.Count == 0 ? 0d : (double)totalResults / queryList.Count;
            return report;
        }

        private static double ComputePercentile(List<long> values, double percentile)
        {
            if (values == null || values.Count == 0)
            {
                return 0d;
            }

            var sorted = values.OrderBy(v => v).ToArray();
            double p = Math.Clamp(percentile, 0d, 1d);
            int index = (int)Math.Ceiling((sorted.Length * p) - 1);
            index = Math.Clamp(index, 0, sorted.Length - 1);
            return sorted[index];
        }
    }
}
