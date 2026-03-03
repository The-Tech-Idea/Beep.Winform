using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.FontManagement;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls.Styling
{
 /// <summary>
 /// Central factory and cache for commonly used painters (brushes, pens, gradients, paths, rasters).
 /// Use this to avoid allocating transient GDI objects each paint cycle.
 /// /// Call `ClearCache()` when application/theme unloads to release GDI objects.
 /// </summary>
 public static class PaintersFactory
 {
 private static readonly ConcurrentDictionary<int, SolidBrush> _solidBrushCache = new ConcurrentDictionary<int, SolidBrush>();
 private static readonly ConcurrentDictionary<int, Pen> _penCache = new ConcurrentDictionary<int, Pen>();

 // Rounded rectangle GraphicsPath cache keyed by width,height,radius
 private static readonly ConcurrentDictionary<int, GraphicsPath> _roundedPathCache = new ConcurrentDictionary<int, GraphicsPath>();

 // Rasterized background cache keyed by a signature (theme+bounds)
 private static readonly ConcurrentDictionary<string, Bitmap> _rasterCache = new ConcurrentDictionary<string, Bitmap>(StringComparer.Ordinal);

 // Shared font cache
 private static readonly ConcurrentDictionary<string, Font> _fontCache = new ConcurrentDictionary<string, Font>(StringComparer.Ordinal);

 /// <summary>
 /// Returns a cached <see cref="SolidBrush"/> for the given color.
 /// Never returns null. If the cached brush was disposed, it is replaced automatically.
 /// </summary>
 public static SolidBrush GetSolidBrush(Color color)
 {
 int key = color.ToArgb();
 try
 {
 var brush = _solidBrushCache.GetOrAdd(key, k => new SolidBrush(Color.FromArgb(k)));
 _ = brush.Color; // Probe: throws if disposed
 return brush;
 }
 catch
 {
 _solidBrushCache.TryRemove(key, out _);
 try
 {
 return _solidBrushCache.GetOrAdd(key, k => new SolidBrush(Color.FromArgb(k)));
 }
 catch
 {
 // Ultimate fallback — uncached, non-null brush
 return new SolidBrush(color == Color.Empty ? Color.Black : color);
 }
 }
 }

 /// <summary>
 /// Returns a cached <see cref="Pen"/> for the given color and width.
 /// Width is clamped to [0.1 .. 100] to prevent GDI+ errors. Never returns null.
 /// </summary>
 public static Pen GetPen(Color color, float width = 1f)
 {
 width = ClampWidth(width);
 int key = HashCode.Combine(color.ToArgb(), width);
 try
 {
 var pen = _penCache.GetOrAdd(key, _ => new Pen(color, width));
 _ = pen.Width; // Probe: throws if disposed
 return pen;
 }
 catch
 {
 _penCache.TryRemove(key, out _);
 try
 {
 return _penCache.GetOrAdd(key, _ => new Pen(color, width));
 }
 catch
 {
 return new Pen(color == Color.Empty ? Color.Black : color, width);
 }
 }
 }

 /// <summary>
 /// Returns a cached <see cref="Pen"/> with line-join and alignment options.
 /// Width is clamped to [0.1 .. 100]. Never returns null.
 /// </summary>
 public static Pen GetStyledPen(
 Color color,
 float width,
 LineJoin lineJoin = LineJoin.Miter,
 PenAlignment alignment = PenAlignment.Center)
 {
 width = ClampWidth(width);
 int key = HashCode.Combine(color.ToArgb(), width, (int)lineJoin, (int)alignment);
 try
 {
 var pen = _penCache.GetOrAdd(key, _ =>
 {
 var p = new Pen(color, width)
 {
 LineJoin = lineJoin,
 Alignment = alignment
 };
 return p;
 });
 _ = pen.Width; // Probe: throws if disposed
 return pen;
 }
 catch
 {
 _penCache.TryRemove(key, out _);
 try
 {
 return _penCache.GetOrAdd(key, _ =>
 {
 var p = new Pen(color, width)
 {
 LineJoin = lineJoin,
 Alignment = alignment
 };
 return p;
 });
 }
 catch
 {
 return new Pen(color == Color.Empty ? Color.Black : color, width);
 }
 }
 }

 [Obsolete("Use CreateLinearGradientBrush(...) and dispose returned brush with using.")]
 public static LinearGradientBrush GetLinearGradientBrush(RectangleF bounds, Color color1, Color color2, LinearGradientMode mode)
 {
 return CreateLinearGradientBrush(bounds, color1, color2, mode);
 }

 [Obsolete("Use CreateLinearGradientBrush(...) and dispose returned brush with using.")]
 public static LinearGradientBrush GetLinearGradientBrush(RectangleF bounds, Color color1, Color color2, float angle)
 {
 return CreateLinearGradientBrush(bounds, color1, color2, angle);
 }

 /// <summary>
 /// Creates a new <see cref="LinearGradientBrush"/>. Caller must dispose with <c>using</c>.
 /// Bounds are clamped to a minimum of 1×1. Colors are sanitized.
 /// Never returns null — falls back to a safe gradient if GDI+ throws.
 /// </summary>
 public static LinearGradientBrush CreateLinearGradientBrush(RectangleF bounds, Color color1, Color color2, LinearGradientMode mode)
 {
 bounds = SafeBounds(bounds);
 color1 = SafeColor(color1);
 color2 = SafeColor(color2);
 try
 {
 var brush = new LinearGradientBrush(bounds, color1, color2, mode);
 brush.WrapMode = WrapMode.Clamp;
 return brush;
 }
 catch
 {
 try
 {
 // Retry with origin-based bounds and same color
 var safeBounds = new RectangleF(0, 0, Math.Max(1f, bounds.Width), Math.Max(1f, bounds.Height));
 var brush = new LinearGradientBrush(safeBounds, color1, color1, LinearGradientMode.Vertical);
 brush.WrapMode = WrapMode.Clamp;
 return brush;
 }
 catch
 {
 // Last resort — 1×1 transparent gradient that won't crash FillPath
 return new LinearGradientBrush(
  new RectangleF(0, 0, 1, 1), Color.Transparent, Color.Transparent, LinearGradientMode.Vertical);
 }
 }
 }

 /// <summary>
 /// Creates a new <see cref="LinearGradientBrush"/> at a custom angle.
 /// Caller must dispose with <c>using</c>. Never returns null.
 /// </summary>
 public static LinearGradientBrush CreateLinearGradientBrush(RectangleF bounds, Color color1, Color color2, float angle)
 {
 bounds = SafeBounds(bounds);
 color1 = SafeColor(color1);
 color2 = SafeColor(color2);
 // Normalize angle to [0..360)
 if (float.IsNaN(angle) || float.IsInfinity(angle)) angle = 0f;
 angle = ((angle % 360f) + 360f) % 360f;
 try
 {
 var brush = new LinearGradientBrush(bounds, color1, color2, angle);
 brush.WrapMode = WrapMode.Clamp;
 return brush;
 }
 catch
 {
 try
 {
 var safeBounds = new RectangleF(0, 0, Math.Max(1f, bounds.Width), Math.Max(1f, bounds.Height));
 var brush = new LinearGradientBrush(safeBounds, color1, color1, 0f);
 brush.WrapMode = WrapMode.Clamp;
 return brush;
 }
 catch
 {
 return new LinearGradientBrush(
  new RectangleF(0, 0, 1, 1), Color.Transparent, Color.Transparent, 0f);
 }
 }
 }

 /// <summary>
 /// Returns a cloned rounded rectangle <see cref="GraphicsPath"/> starting at (0,0).
 /// Thread-safe: if the cached template was disposed, it is recreated automatically.
 /// Never returns null.
 /// </summary>
 public static GraphicsPath GetRoundedRectanglePath(int width, int height, int radius)
 {
 width = Math.Max(1, width);
 height = Math.Max(1, height);
 radius = Math.Max(0, radius);

 int key = HashCode.Combine(width, height, radius);
 try
 {
 var template = _roundedPathCache.GetOrAdd(key, _ => CreateRoundedPath(width, height, radius));
 return (GraphicsPath)template.Clone();
 }
 catch
 {
 // Template was likely disposed (race with ClearPathCache) — recreate
 _roundedPathCache.TryRemove(key, out _);
 try
 {
 var fresh = CreateRoundedPath(width, height, radius);
 _roundedPathCache.TryAdd(key, fresh);
 return (GraphicsPath)fresh.Clone();
 }
 catch
 {
 // Ultimate fallback — simple rectangle path
 var fallback = new GraphicsPath();
 fallback.AddRectangle(new Rectangle(0, 0, width, height));
 return fallback;
 }
 }
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
 /// Get or create a rasterized background bitmap for complex effects.
 /// Returns null only if <paramref name="signature"/> or <paramref name="generator"/> is null,
 /// or if the generator itself produces null.
 /// </summary>
 public static Bitmap GetOrCreateRaster(string signature, Func<Bitmap> generator)
 {
 if (string.IsNullOrEmpty(signature) || generator == null)
 return null;

 try
 {
 return _rasterCache.GetOrAdd(signature, _ =>
 {
 try { return generator(); }
 catch { return null; }
 });
 }
 catch
 {
 try { return generator(); } catch { return null; }
 }
 }

 /// <summary>
 /// Get or create a cached <see cref="Font"/>. Never returns null.
 /// </summary>
 [Obsolete("Use BeepThemesManager.ToFont(...) instead of PaintersFactory.GetFont.")]
 public static Font GetFont(string familyName, float size, FontStyle style)
 {
 if (string.IsNullOrEmpty(familyName)) familyName = "Segoe UI";
 if (size <= 0f || float.IsNaN(size) || float.IsInfinity(size)) size = 9f;

 try
 {
 var weight = style.HasFlag(FontStyle.Bold) ? FontWeight.Bold : FontWeight.Normal;
 return BeepThemesManager.ToFont(familyName, size, weight, style)
 ?? BeepFontManager.DefaultFont
 ?? SystemFonts.DefaultFont;
 }
 catch
 {
 try { return BeepFontManager.DefaultFont; }
 catch { return SystemFonts.DefaultFont; }
 }
 }

 [Obsolete("Use BeepThemesManager.ToFont(...) instead of PaintersFactory.GetFont.")]
 public static Font GetFont(Font prototype)
 {
 if (prototype == null)
 {
 try { return BeepFontManager.DefaultFont; }
 catch { return SystemFonts.DefaultFont; }
 }
 try
 {
 return GetFont(prototype.FontFamily.Name, prototype.Size, prototype.Style);
 }
 catch
 {
 try { return BeepFontManager.DefaultFont; }
 catch { return SystemFonts.DefaultFont; }
 }
 }

 public static void ClearBrushCache()
 {
 // TryRemove each key individually to reduce race window with paint threads
 var keys = _solidBrushCache.Keys;
 foreach (var key in keys)
 {
 if (_solidBrushCache.TryRemove(key, out var brush))
 { try { brush.Dispose(); } catch { } }
 }
 }

 public static void ClearPenCache()
 {
 var keys = _penCache.Keys;
 foreach (var key in keys)
 {
 if (_penCache.TryRemove(key, out var pen))
 { try { pen.Dispose(); } catch { } }
 }
 }

 public static void ClearPathCache()
 {
 var keys = _roundedPathCache.Keys;
 foreach (var key in keys)
 {
 if (_roundedPathCache.TryRemove(key, out var path))
 { try { path.Dispose(); } catch { } }
 }
 }

 public static void ClearRasterCache()
 {
 var keys = _rasterCache.Keys;
 foreach (var key in keys)
 {
 if (_rasterCache.TryRemove(key, out var bmp))
 { try { bmp?.Dispose(); } catch { } }
 }
 }

 public static void ClearFontCache()
 {
 var keys = _fontCache.Keys;
 foreach (var key in keys)
 {
 if (_fontCache.TryRemove(key, out var font))
 { try { font.Dispose(); } catch { } }
 }
 }

 public static void ClearCache()
 {
 ClearBrushCache();
 ClearPenCache();
 ClearPathCache();
 ClearRasterCache();
 ClearFontCache();
 }

 // ────────────────────────── Private helpers ──────────────────────────

 /// <summary>Clamps pen width to a safe GDI+ range [0.1 .. 100].</summary>
 private static float ClampWidth(float width)
 {
 if (float.IsNaN(width) || float.IsInfinity(width) || width < 0.1f) return 0.1f;
 if (width > 100f) return 100f;
 return width;
 }

 /// <summary>Ensures bounds are at least 1×1 and all coordinates are finite.</summary>
 private static RectangleF SafeBounds(RectangleF bounds)
 {
 float x = float.IsNaN(bounds.X) || float.IsInfinity(bounds.X) ? 0f : bounds.X;
 float y = float.IsNaN(bounds.Y) || float.IsInfinity(bounds.Y) ? 0f : bounds.Y;
 float w = float.IsNaN(bounds.Width) || float.IsInfinity(bounds.Width) ? 1f : Math.Max(1f, bounds.Width);
 float h = float.IsNaN(bounds.Height) || float.IsInfinity(bounds.Height) ? 1f : Math.Max(1f, bounds.Height);
 return new RectangleF(x, y, w, h);
 }

 /// <summary>Replaces Color.Empty with Color.Transparent so GDI+ never sees ARGB=0 as "named empty".</summary>
 private static Color SafeColor(Color color)
 {
 return color == Color.Empty ? Color.Transparent : color;
 }
 }
}
