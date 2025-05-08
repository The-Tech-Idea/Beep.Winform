// SvgExtensions.cs
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Svg;

namespace TheTechIdea.Beep.Winform.Controls.Helpers
{
    public static class SvgExtensions
    {
        /// <summary>
        /// Promotes every CSS declaration in a <c>style="…"</c> attribute to the
        /// corresponding strongly-typed property on the element, then removes the
        /// attribute so it can no longer override you later.
        /// Works for visual elements *and* gradient stops.
        /// </summary>
        public static void FlattenStyles(this SvgDocument doc)
        {
            if (doc == null) return;

            foreach (var el in doc.Descendants())
            {
                if (!el.CustomAttributes.TryGetValue("style", out string style) ||
                    string.IsNullOrWhiteSpace(style))
                    continue;

                foreach (string pair in style.Split(new[] { ';' },
                                                    StringSplitOptions.RemoveEmptyEntries))
                {
                    var kv = pair.Split(new[] { ':' }, 2);
                    if (kv.Length != 2) continue;

                    string key = kv[0].Trim().ToLowerInvariant();
                    string val = kv[1].Trim();

                    ApplyStyle(el, key, val);
                }

                el.CustomAttributes.Remove("style");
            }
        }

        // ────────────────────────────────────────────────────────────────────────
        #region  Style → Property mapper
        private static void ApplyStyle(SvgElement el, string key, string val)
        {
            switch (key)
            {
                /* ─── Colour & Paint ───────────────────────────────────────────*/
                case "fill":
                    SetPaint(el, val, fill: true);
                    break;

                case "stroke":
                    SetPaint(el, val, fill: false);
                    break;

                case "color":
                    if (el is SvgVisualElement v1)
                        v1.Color = ParsePaint(val);
                    break;

                case "stop-color":
                    if (el is SvgGradientStop s1)
                        s1.StopColor = ParsePaint(val);
                    break;

                /* ─── Opacity ──────────────────────────────────────────────────*/
                case "fill-opacity":
                    if (el is SvgVisualElement v2 &&
                        TryParseFloat(val, out float fo))
                        v2.FillOpacity = fo;
                    break;

                case "stroke-opacity":
                    if (el is SvgVisualElement v3 &&
                        TryParseFloat(val, out float so))
                        v3.StrokeOpacity = so;
                    break;

                case "stop-opacity":
                    if (el is SvgGradientStop s2 &&
                        TryParseFloat(val, out float spo))
                        s2.StopOpacity = spo;
                    break;

                case "opacity":
                    if (el is SvgVisualElement v4 &&
                        TryParseFloat(val, out float op))
                        v4.Opacity = op;
                    break;

                /* ─── Stroke geometry ──────────────────────────────────────────*/
                case "stroke-width":
                    if (el is SvgVisualElement v5 &&
                        TryParseFloat(val, out float sw))
                        v5.StrokeWidth = sw;
                    break;

                case "stroke-linecap":
                    if (el is SvgVisualElement v6 &&
                        EnumTryParse<SvgStrokeLineCap>(val, out var cap))
                        v6.StrokeLineCap = cap;
                    break;

                case "stroke-linejoin":
                    if (el is SvgVisualElement v7 &&
                        EnumTryParse<SvgStrokeLineJoin>(val, out var join))
                        v7.StrokeLineJoin = join;
                    break;

                case "stroke-miterlimit":
                    if (el is SvgVisualElement v8 &&
                        TryParseFloat(val, out float miter))
                        v8.StrokeMiterLimit = miter;
                    break;

                case "stroke-dasharray":
                    if (el is SvgVisualElement v9)
                        v9.StrokeDashArray = ParseDashArray(val);
                    break;

                case "stroke-dashoffset":
                    if (el is SvgVisualElement v10 &&
                        TryParseFloat(val, out float off))
                        v10.StrokeDashOffset = new SvgUnit(off);
                    break;

                /* ─── Fill rule ────────────────────────────────────────────────*/
                case "fill-rule":
                    if (el is SvgVisualElement v11 &&
                        EnumTryParse<SvgFillRule>(val, out var rule))
                        v11.FillRule = rule;
                    break;
            }
        }
        #endregion
        // ────────────────────────────────────────────────────────────────────────

        #region  Helpers
        /// <summary>
        /// Rewrites every &lt;style&gt; block in the SVG so that
        /// <c>fill:</c> / <c>stroke:</c> declarations adopt the theme colours.
        /// </summary>
        public static void ReplaceColoursInStyleElements(
                this SvgDocument doc,
                Color newFill,
                Color newStroke)
        {
            if (doc == null) return;

            string hexFill = ColorTranslator.ToHtml(newFill);
            string hexStroke = ColorTranslator.ToHtml(newStroke);

            // cache the reflection lookup once
            PropertyInfo elementNameProp =
                typeof(SvgElement).GetProperty("ElementName",
                                               BindingFlags.Instance |
                                               BindingFlags.NonPublic |
                                               BindingFlags.Public);

            foreach (var el in doc.Descendants())
            {
                // ── identify <style> elements, even though ElementName is internal ──
                string tag = elementNameProp?.GetValue(el) as string;
                if (!string.Equals(tag, "style", StringComparison.OrdinalIgnoreCase))
                    continue;

                // the raw CSS lives in the Content property
                string css = el.Content;
                if (string.IsNullOrWhiteSpace(css)) continue;

                // replace fill / stroke colours inside the CSS text
                css = Regex.Replace(css,
                                    @"fill\s*:[^;}\n\r]+",
                                    $"fill:{hexFill}",
                                    RegexOptions.IgnoreCase);

                css = Regex.Replace(css,
                                    @"stroke\s*:[^;}\n\r]+",
                                    $"stroke:{hexStroke}",
                                    RegexOptions.IgnoreCase);

                el.Content = css;
            }
        }
        public static SvgDocument DeepCopyDocument(this SvgDocument doc)
        {
            if (doc == null) return null;

            try
            {
                using (var ms = new MemoryStream())
                {
                    using (var writer = System.Xml.XmlWriter.Create(ms))
                    {
                        doc.Write(writer);
                    }

                    ms.Position = 0;
                    return SvgDocument.Open<SvgDocument>(ms);
                }
            }
            catch
            {
                return null; // Return null if copy fails
            }
        }
        private static void SetPaint(SvgElement el, string css, bool fill)
        {
            var paint = ParsePaint(css);

            if (el is SvgVisualElement v)
            {
                if (fill) v.Fill = paint;
                else v.Stroke = paint;
            }
            else if (el is SvgGradientStop s)
            {
                if (fill) s.StopColor = paint;   // gradients are always “fill”
            }
        }

        private static SvgPaintServer ParsePaint(string css)
        {
            if (string.Equals(css, "none", StringComparison.OrdinalIgnoreCase))
                return SvgPaintServer.None;

            // try rgb(…), rgba(…), #RGB, #RRGGBB, #AARRGGBB, or named colours
            try
            {
                return new SvgColourServer(ColorTranslator.FromHtml(css));
            }
            catch
            {
                return SvgPaintServer.None;
            }
        }

        private static bool TryParseFloat(string txt, out float val) =>
            float.TryParse(txt, NumberStyles.Float, CultureInfo.InvariantCulture, out val);

        private static bool EnumTryParse<T>(string txt, out T value) where T : struct =>
            Enum.TryParse(txt, true, out value);

        private static SvgUnitCollection ParseDashArray(string css)
        {
            if (string.Equals(css, "none", StringComparison.OrdinalIgnoreCase))
                return new SvgUnitCollection(); // empty

            var col = new SvgUnitCollection();

            foreach (string part in css.Split(new[] { ',', ' ', '\t' },
                                              StringSplitOptions.RemoveEmptyEntries))
            {
                if (TryParseFloat(part, out float f))
                    col.Add(new SvgUnit(f));
            }
            return col;
        }
        #endregion
    }
}
