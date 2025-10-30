using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Trees.Painters;
using TheTechIdea.Beep.Winform.Controls.Trees.Models;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Configs;
using System.Collections.Generic;
using System.Windows.Forms;

namespace BenchmarkSuite1
{
 [SimpleJob(RuntimeMoniker.Net70)]
 [MemoryDiagnoser]
 public class TreesRenderBenchmark
 {
 private BeepTree _tree;
 private Graphics _graphics;
 private Bitmap _bitmap;
 private List<NodeInfo> _nodes;
 private ITreePainter _painter;
 private IBeepTheme _theme;

 [GlobalSetup]
 public void Setup()
 {
 // Create an off-screen bitmap and graphics for painting
 _bitmap = new Bitmap(1024,768);
 _graphics = Graphics.FromImage(_bitmap);

 // Create a sample BeepTree and populate visible nodes
 _tree = new BeepTree();
 _theme = TheTechIdea.Beep.Vis.Modules.BeepThemesManager.GetDefaultTheme();

 // Create painter
 _painter = TheTechIdea.Beep.Winform.Controls.Trees.Painters.BeepTreePainterFactory.CreatePainter(TheTechIdea.Beep.Winform.Controls.Trees.Models.TreeStyle.Standard, _tree, _theme);

 // Build fake nodes
 _nodes = new List<NodeInfo>();
 for (int i =0; i <1000; i++)
 {
 NodeInfo ni = new NodeInfo
 {
 Item = new TheTechIdea.Beep.Winform.Controls.Models.SimpleItem { Text = "Node " + i },
 Level = i %5,
 Y = i *24,
 RowHeight =24,
 RowWidth =800,
 TextSize = new Size(100,16),
 RowRectContent = new Rectangle(0, i *24,800,24),
 ToggleRectContent = new Rectangle(4, i *24 +4,16,16),
 CheckRectContent = new Rectangle(24, i *24 +4,16,16),
 IconRectContent = new Rectangle(44, i *24 +4,16,16),
 TextRectContent = new Rectangle(68, i *24 +4,732,16)
 };
 _nodes.Add(ni);
 }

 // Set visible nodes via reflection
 var visibleField = typeof(BeepTree).GetField("_visibleNodes", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
 if (visibleField != null)
 {
 visibleField.SetValue(_tree, _nodes);
 }
 }

 [GlobalCleanup]
 public void Cleanup()
 {
 _graphics.Dispose();
 _bitmap.Dispose();
 }

 [Benchmark]
 public void PaintTree()
 {
 var painter = _painter as TheTechIdea.Beep.Winform.Controls.Trees.Painters.BaseTreePainter;
 if (painter != null)
 {
 painter.Paint(_graphics, _tree, new Rectangle(0,0,800,768));
 }
 }
 }
}
