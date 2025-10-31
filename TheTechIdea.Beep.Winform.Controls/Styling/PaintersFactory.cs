using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Winform.Controls.Styling
{
 /// <summary>
 /// Central factory and cache for commonly used painters (brushes, pens, gradients, paths, rasters).
 /// Use this to avoid allocating transient GDI objects each paint cycle.
 /// Call `ClearCache()` when application/theme unloads to release GDI objects.
 /// </summary>
 public static class PaintersFactory
 {
 private static readonly ConcurrentDictionary<int, SolidBrush> _solidBrushCache = new ConcurrentDictionary<int, SolidBrush>();
 private static readonly ConcurrentDictionary<int, Pen> _penCache = new ConcurrentDictionary<int, Pen>();

 // Gradient brush cache keyed by a hash of bounds + colors + mode/angle
 private static readonly ConcurrentDictionary<long, LinearGradientBrush> _linearGradientCache = new ConcurrentDictionary<long, LinearGradientBrush>();

 // Rounded rectangle GraphicsPath cache keyed by width,height,radius
 private static readonly ConcurrentDictionary<long, GraphicsPath> _roundedPathCache = new ConcurrentDictionary<long, GraphicsPath>();

 // Rasterized background cache keyed by a signature (theme+bounds)
 private static readonly ConcurrentDictionary<string, Bitmap> _rasterCache = new ConcurrentDictionary<string, Bitmap>(StringComparer.Ordinal);

 // Shared font cache
 private static readonly ConcurrentDictionary<string, Font> _fontCache = new ConcurrentDictionary<string, Font>(StringComparer.Ordinal);

 public static SolidBrush GetSolidBrush(Color color)
 {
 int key = color.ToArgb();
 return _solidBrushCache.GetOrAdd(key, k => new SolidBrush(Color.FromArgb(k)));
 }

 public static Pen GetPen(Color color, float width =1f)
 {
 int key = HashCode.Combine(color.ToArgb(), width);
 return _penCache.GetOrAdd(key, k => new Pen(color, width));
 }

 public static LinearGradientBrush GetLinearGradientBrush(RectangleF bounds, Color color1, Color color2, LinearGradientMode mode)
 {
 // Create a stable key based on bounds size and colors (ignore location to allow reuse across positions)
 int k1 = color1.ToArgb();
 int k2 = color2.ToArgb();
 int w = (int)bounds.Width;
 int h = (int)bounds.Height;
 int m = (int)mode;
 long key = ((long)k1 <<32) ^ (long)k2 ^ (long)w <<16 ^ h ^ m;

 return _linearGradientCache.GetOrAdd(key, _ =>
 {
 // Create brush using bounds passed in; callers should reuse brush only for identical sizes
 var brush = new LinearGradientBrush(bounds, color1, color2, mode);
 // Important: set WrapMode to Clamp to prevent tiling artifacts
 //brush.WrapMode = WrapMode.Clamp;
 return brush;
 });
 }

 // New overload to support angle-based gradients
 public static LinearGradientBrush GetLinearGradientBrush(RectangleF bounds, Color color1, Color color2, float angle)
 {
 int k1 = color1.ToArgb();
 int k2 = color2.ToArgb();
 int w = (int)bounds.Width;
 int h = (int)bounds.Height;
 int ang = (int)(angle *1000f);
 long key = ((long)k1 <<32) ^ (long)k2 ^ ((long)w <<16) ^ h ^ ang;

 return _linearGradientCache.GetOrAdd(key, _ =>
 {
 var brush = new LinearGradientBrush(bounds, color1, color2, angle);
 //brush.WrapMode = WrapMode.Clamp;
 return brush;
 });
 }

 /// <summary>
 /// Returns a rounded rectangle GraphicsPath for the given size/radius. The returned path has coordinates starting at (0,0).
 /// Caller should clone and transform as needed to place at desired location.
 /// </summary>
 public static GraphicsPath GetRoundedRectanglePath(int width, int height, int radius)
 {
 long key = ((long)width <<42) ^ ((long)height <<22) ^ (uint)radius;
 return _roundedPathCache.GetOrAdd(key, _ => CreateRoundedPath(width, height, radius));
 }

 private static GraphicsPath CreateRoundedPath(int width, int height, int radius)
 {
 var path = new GraphicsPath();
 if (width <=0 || height <=0)
 {
 path.AddRectangle(new Rectangle(0,0, Math.Max(1, width), Math.Max(1, height)));
 return path;
 }

 int r = Math.Max(0, Math.Min(radius, Math.Min(width, height) /2));
 if (r ==0)
 {
 path.AddRectangle(new Rectangle(0,0, width, height));
 return path;
 }

 path.AddArc(0,0, r *2, r *2,180,90);
 path.AddArc(width - r *2,0, r *2, r *2,270,90);
 path.AddArc(width - r *2, height - r *2, r *2, r *2,0,90);
 path.AddArc(0, height - r *2, r *2, r *2,90,90);
 path.CloseFigure();
 return path;
 }

 /// <summary>
 /// Get or create a rasterized background (bitmap) for complex effects. The generator is invoked when no cached bitmap exists.
 /// </summary>
 public static Bitmap GetOrCreateRaster(string signature, Func<Bitmap> generator)
 {
 return _rasterCache.GetOrAdd(signature, _ => generator());
 }

 /// <summary>
 /// Get or create a cached Font. Keyed by family name, size and style.
 /// Cached fonts are reused across paints to avoid allocations.
 /// </summary>
 public static Font GetFont(string familyName, float size, FontStyle style)
 {
 if (string.IsNullOrEmpty(familyName)) familyName = SystemFonts.DefaultFont.FontFamily.Name;
 var key = $"{familyName}:{size}:{(int)style}";
 return _fontCache.GetOrAdd(key, k =>
 {
 try
 {
 return new Font(familyName, size, style);
 }
 catch
 {
 return SystemFonts.DefaultFont;
 }
 });
 }

 public static Font GetFont(Font prototype)
 {
 if (prototype == null) return SystemFonts.DefaultFont;
 return GetFont(prototype.FontFamily.Name, prototype.Size, prototype.Style);
 }

 public static void ClearCache()
 {
 foreach (var kv in _solidBrushCache)
 {
 try { kv.Value.Dispose(); } catch { }
 }
 _solidBrushCache.Clear();

 foreach (var kv in _penCache)
 {
 try { kv.Value.Dispose(); } catch { }
 }
 _penCache.Clear();

 foreach (var kv in _linearGradientCache)
 {
 try { kv.Value.Dispose(); } catch { }
 }
 _linearGradientCache.Clear();

 foreach (var kv in _roundedPathCache)
 {
 try { kv.Value.Dispose(); } catch { }
 }
 _roundedPathCache.Clear();

 foreach (var kv in _rasterCache)
 {
 try { kv.Value.Dispose(); } catch { }
 }
 _rasterCache.Clear();

 foreach (var kv in _fontCache)
 {
 try { kv.Value.Dispose(); } catch { }
 }
 _fontCache.Clear();
 }
 }
}
