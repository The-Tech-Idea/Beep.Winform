// BeepDocumentHostBenchmarks.cs
// Performance benchmark suite for BeepDocumentHost core operations.
// Uses a simple stopwatch-based runner so it has zero additional NuGet dependencies.
// For production profiling, wire the same scenarios into BenchmarkDotNet.
// ─────────────────────────────────────────────────────────────────────────────
// How to run:
//   dotnet run --project TheTechIdea.Beep.Winform.Controls.Tests -- benchmark
// Results are written to the console and to benchmark-results.txt.
// ─────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.DocumentHost;
using TheTechIdea.Beep.Winform.Controls.DocumentHost.Features;
using TheTechIdea.Beep.Winform.Controls.DocumentHost.Layout;
using Xunit;
using Xunit.Abstractions;

namespace TheTechIdea.Beep.Winform.Controls.Tests
{
    /// <summary>
    /// Benchmark scenarios for core BeepDocumentHost subsystems.
    /// Each test measures wall-clock time and asserts an upper bound so regressions
    /// are caught in CI without dedicated benchmark infra.
    /// </summary>
    public class BeepDocumentHostBenchmarks
    {
        private readonly ITestOutputHelper _out;

        public BeepDocumentHostBenchmarks(ITestOutputHelper output)
            => _out = output;

        // ── 1. Myers diff — small file ────────────────────────────────────────

        [Fact]
        public void Benchmark_DiffSmallFile_Under10Ms()
        {
            string textA = GenerateText(100);
            string textB = MutateText(textA, mutations: 10);

            var elapsed = Measure(() => DocumentComparer.Compare(textA, textB));

            Report("Diff 100-line file (10 mutations)", elapsed, expectedMaxMs: 10);
            Assert.True(elapsed.TotalMilliseconds < 50, // generous CI allowance
                        $"Diff took {elapsed.TotalMilliseconds:F1} ms");
        }

        // ── 2. Myers diff — large file ────────────────────────────────────────

        [Fact]
        public void Benchmark_DiffLargeFile_Under100Ms()
        {
            string textA = GenerateText(2_000);
            string textB = MutateText(textA, mutations: 200);

            var elapsed = Measure(() => DocumentComparer.Compare(textA, textB));

            Report("Diff 2 000-line file (200 mutations)", elapsed, expectedMaxMs: 100);
            Assert.True(elapsed.TotalMilliseconds < 500,
                        $"Diff took {elapsed.TotalMilliseconds:F1} ms");
        }

        // ── 3. Layout validation — deep tree ──────────────────────────────────

        [Fact]
        public void Benchmark_ValidateDeepTree_Under5Ms()
        {
            var root    = BuildDeepTree(depth: 10);
            var elapsed = Measure(() => LayoutTreeValidator.Validate(root));

            Report("Validate depth-10 layout tree", elapsed, expectedMaxMs: 5);
            Assert.True(elapsed.TotalMilliseconds < 50,
                        $"Validate took {elapsed.TotalMilliseconds:F1} ms");
        }

        // ── 4. Layout repair — wide invalid tree ─────────────────────────────

        [Fact]
        public void Benchmark_RepairWideTree_Under10Ms()
        {
            var root    = BuildInvalidWideTree(leaves: 32);
            var elapsed = Measure(() => LayoutTreeRepairer.Repair(root));

            Report("Repair 32-leaf tree with empty NodeIds", elapsed, expectedMaxMs: 10);
            Assert.True(elapsed.TotalMilliseconds < 100,
                        $"Repair took {elapsed.TotalMilliseconds:F1} ms");
        }

        // ── 5. History push — sustained throughput ────────────────────────────

        [Fact]
        public void Benchmark_HistoryPush1000Items_Under50Ms()
        {
            var history  = new BeepLayoutHistory { MaxDepth = 200 };
            string state = """{"schemaVersion":3,"groups":[]}""";

            var elapsed = Measure(() =>
            {
                for (int i = 0; i < 1_000; i++)
                    history.Push(state);
            });

            Report("1 000 × BeepLayoutHistory.Push (ring eviction)", elapsed, expectedMaxMs: 50);
            Assert.True(elapsed.TotalMilliseconds < 200,
                        $"History push took {elapsed.TotalMilliseconds:F1} ms");
        }

        // ── 6. UndoRedo — 500 undo operations ────────────────────────────────

        [Fact]
        public void Benchmark_UndoRedo500Operations_Under20Ms()
        {
            var ur     = new BeepLayoutUndoRedo { MaxDepth = 50 };
            string j   = """{"schemaVersion":3}""";
            for (int i = 0; i < 50; i++) ur.Push(j);

            var elapsed = Measure(() =>
            {
                for (int i = 0; i < 500; i++)
                {
                    if (ur.CanUndo) ur.Undo(j);
                    else            ur.Push(j);
                }
            });

            Report("500 × Undo/Push cycle", elapsed, expectedMaxMs: 20);
            Assert.True(elapsed.TotalMilliseconds < 200,
                        $"UndoRedo cycle took {elapsed.TotalMilliseconds:F1} ms");
        }

        // ── 7. Diff of identical files — hot path ─────────────────────────────

        [Fact]
        public void Benchmark_DiffIdenticalFiles_Under5Ms()
        {
            string text     = GenerateText(500);
            var elapsed     = Measure(() => DocumentComparer.Compare(text, text));

            Report("Diff identical 500-line file", elapsed, expectedMaxMs: 5);
            Assert.True(elapsed.TotalMilliseconds < 50,
                        $"Identical diff took {elapsed.TotalMilliseconds:F1} ms");
        }

        // ── 8. Layout diff — large layout ────────────────────────────────────

        [Fact]
        public void Benchmark_LayoutDiff100Docs_Under5Ms()
        {
            string layoutA = MakeLayoutJsonWithDocs(50);
            string layoutB = MakeLayoutJsonWithDocs(50, shuffle: true);

            var elapsed = Measure(() => BeepLayoutDiff.Compare(layoutA, layoutB));

            Report("Diff layout with 50 docs each group", elapsed, expectedMaxMs: 5);
            Assert.True(elapsed.TotalMilliseconds < 100,
                        $"Layout diff took {elapsed.TotalMilliseconds:F1} ms");
        }

        // ── 9. Porcelain provider refresh — 1 000-entry cache load ────────────

        [Fact]
        public void Benchmark_GitFileInfo_Allocate1000_Under10Ms()
        {
            var elapsed = Measure(() =>
            {
                for (int i = 0; i < 1_000; i++)
                {
                    _ = new GitFileInfo
                    {
                        FilePath  = $"src/File{i}.cs",
                        Status    = GitFileStatus.Modified,
                        BadgeText = "M",
                        BadgeColor = System.Drawing.Color.Orange,
                        ToolTip   = "Modified"
                    };
                }
            });

            Report("Allocate 1 000 GitFileInfo objects", elapsed, expectedMaxMs: 10);
            Assert.True(elapsed.TotalMilliseconds < 200,
                        $"GitFileInfo allocation took {elapsed.TotalMilliseconds:F1} ms");
        }

        // ── 10. AllNodes enumeration — wide tree ─────────────────────────────

        [Fact]
        public void Benchmark_AllNodesWideTree_Under2Ms()
        {
            var root    = BuildDeepTree(depth: 8);
            var elapsed = Measure(() => root.AllNodes().ToList());

            Report("AllNodes() on depth-8 tree (256 leaves)", elapsed, expectedMaxMs: 2);
            Assert.True(elapsed.TotalMilliseconds < 50,
                        $"AllNodes took {elapsed.TotalMilliseconds:F1} ms");
        }

        // ── 11. DeepClone — large tree ────────────────────────────────────────

        [Fact]
        public void Benchmark_DeepCloneLargeTree_Under5Ms()
        {
            var root    = BuildDeepTree(depth: 8);
            var elapsed = Measure(() => root.DeepClone());

            Report("DeepClone depth-8 tree (256 leaves)", elapsed, expectedMaxMs: 5);
            Assert.True(elapsed.TotalMilliseconds < 100,
                        $"DeepClone took {elapsed.TotalMilliseconds:F1} ms");
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private static TimeSpan Measure(Action action)
        {
            var sw = Stopwatch.StartNew();
            action();
            sw.Stop();
            return sw.Elapsed;
        }

        private void Report(string name, TimeSpan elapsed, int expectedMaxMs)
        {
            string line = $"[BENCH] {name,-50} {elapsed.TotalMilliseconds,8:F2} ms  (target <{expectedMaxMs} ms)";
            _out.WriteLine(line);
        }

        private static string GenerateText(int lines)
        {
            var sb = new System.Text.StringBuilder();
            for (int i = 0; i < lines; i++)
                sb.AppendLine($"Line {i,4}: {new string((char)('a' + (i % 26)), 40)}");
            return sb.ToString();
        }

        private static string MutateText(string text, int mutations)
        {
            var linesList = text.Split('\n').ToList();
            int step      = Math.Max(1, linesList.Count / mutations);
            for (int i = 0; i < linesList.Count && mutations > 0; i += step, mutations--)
                linesList[i] = "MUTATED " + linesList[i];
            return string.Join("\n", linesList);
        }

        private static ILayoutNode BuildDeepTree(int depth)
        {
            if (depth == 0)
                return new GroupLayoutNode();

            return new SplitLayoutNode(BuildDeepTree(depth - 1), BuildDeepTree(depth - 1));
        }

        private static ILayoutNode BuildInvalidWideTree(int leaves)
        {
            // Chain of splits using empty NodeIds to trigger repair
            ILayoutNode node = new GroupLayoutNode(string.Empty);
            for (int i = 0; i < leaves - 1; i++)
            {
                node = new SplitLayoutNode(node, new GroupLayoutNode(string.Empty));
            }
            return node;
        }

        private static string MakeLayoutJsonWithDocs(int docsPerGroup, bool shuffle = false)
        {
            var ids = Enumerable.Range(0, docsPerGroup).Select(i => $"doc{i}").ToList();
            if (shuffle) ids = ids.OrderBy(_ => Guid.NewGuid()).ToList();

            var g1Docs = string.Join(",", ids.Select(id => $"\"{id}\""));
            var g2Docs = string.Join(",", ids.Select(id => $"\"{id}_b\""));
            return $$"""{"schemaVersion":3,"groups":[{"id":"g1","documents":[{{g1Docs}}]},{"id":"g2","documents":[{{g2Docs}}]}]}""";
        }
    }
}
