using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using Svg;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Images.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Images
{
    public partial class BeepImage
    {
        #region Theme Handling

        public override void ApplyTheme()
        {
            if (_currentTheme == null) return;
            base.ApplyTheme();
            if (IsChild && Parent != null)
            {
                BackColor = Parent.BackColor;
                ParentBackColor = Parent.BackColor;
            }
            else
            {
                BackColor = _currentTheme.ButtonBackColor;
            }
            ForeColor = _currentTheme.ButtonForeColor;
            BorderColor = _currentTheme.ButtonBorderColor;
            if (_applyThemeOnImage)
            {
                ApplyThemeToSvg();
            }
            else
            {
                // Apply theme to the regular image if needed
                if (regularImage != null)
                {
                    // Potential place for non-SVG theming, if needed
                }
            }
            // MenuStyle changed -> ensure cache regenerates even if rect hasn't changed
            _stateChanged = true;
            _cachedRenderedImage?.Dispose();
            _cachedRenderedImage = null;
            Invalidate();
        }

        public void ApplyThemeToSvg()
        {
            if (svgDocument == null)
            {
                return;
            }

            // Use ImageThemeHelpers for consistent color retrieval
            Color actualFillColor = ImageThemeHelpers.GetImageFillColor(_currentTheme, _imageEmbededin, fillColor != Color.Empty ? fillColor : (Color?)null);
            Color actualStrokeColor = ImageThemeHelpers.GetImageStrokeColor(_currentTheme, _imageEmbededin, strokeColor != Color.Empty ? strokeColor : (Color?)null);
            Color actualbackcolor = ImageThemeHelpers.GetImageBackgroundColor(_currentTheme, _imageEmbededin);

            // Compute a simple signature for the theme/colors to avoid reprocessing
            string themeSignature = $"{actualFillColor.ToArgb()}_{actualStrokeColor.ToArgb()}_{actualbackcolor.ToArgb()}_{_imageEmbededin}";
            if (!string.IsNullOrEmpty(_lastSvgThemeSignature) && _lastSvgThemeSignature == themeSignature)
            {
                // Theme already applied; skip expensive processing
                return;
            }

            // Create SvgColourServer instances for fill and stroke
            var fillServer = new SvgColourServer(actualFillColor);
            var strokeServer = new SvgColourServer(actualStrokeColor);
            var backgroundServer = new SvgColourServer(actualbackcolor);

            svgDocument.StrokeWidth = new SvgUnit(2); // Optional: set stroke width

            foreach (var node in svgDocument.Children)
            {
                // Update color properties.
                // You can check the properties if you want to preserve "None" values, or update unconditionally.
                node.Fill = fillServer;
                node.Color = fillServer;

                node.Stroke = strokeServer;
                node.StrokeWidth = new SvgUnit(2); // Optional: set stroke width

                // Recurse into child nodes.
                ProcessNodes(node.Descendants(), fillServer, strokeServer);
            }
            svgDocument.FlushStyles();

            // Record theme signature and clear raster cache
            _lastSvgThemeSignature = themeSignature;
            try
            {
                foreach (var kv in _rasterizedSvgCache)
                {
                    kv.Value.Dispose();
                }
                _rasterizedSvgCache.Clear();
            }
            catch { }

            // Trigger a redraw and invalidate cache since colors changed
            try
            {
                _stateChanged = true;
                _cachedRenderedImage?.Dispose();
                _cachedRenderedImage = null;
                Invalidate();
                Refresh();
            }
            catch (Exception ex)
            {
                // Error handling
            }
        }

        /// <summary>
        /// Recursively updates the given SVG nodes with the provided fill and stroke color servers.
        /// </summary>
        private void ProcessNodes(IEnumerable<SvgElement> nodes, SvgPaintServer fillServer, SvgPaintServer strokeServer)
        {
            foreach (var node in nodes)
            {
                // Update color properties.
                // You can check the properties if you want to preserve "None" values, or update unconditionally.
                node.Fill = fillServer;
                node.Color = fillServer;

                node.Stroke = strokeServer;

                node.StrokeWidth = new SvgUnit(2); // Optional: set stroke width

                // Recurse into child nodes.
                ProcessNodes(node.Descendants(), fillServer, strokeServer);
            }
        }

        public void ApplyColorToAllElements(Color fillColor)
        {
            if (svgDocument == null)
                return;

            string hexColor = ColorTranslator.ToHtml(fillColor);

            // First attempt - direct Style replacement for all elements (works for cancel.svg)
            foreach (var element in svgDocument.Descendants())
            {
                // Fix inline styles with a direct regex replacement
                if (element.CustomAttributes.ContainsKey("Style"))
                {
                    string style = element.CustomAttributes["Style"];
                    style = Regex.Replace(style, @"fill:[^;]+", $"fill:{hexColor}");
                    element.CustomAttributes["Style"] = style;
                }

                // Also set the Fill property as a backup approach
                element.Fill = new SvgColourServer(fillColor);
            }

            // Flush all styles
            svgDocument.FlushStyles();
            _stateChanged = true;
            _cachedRenderedImage?.Dispose();
            _cachedRenderedImage = null;
            Invalidate();
        }

        #endregion
    }
}
