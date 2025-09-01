using System;
using System.Drawing;
using System.Text.RegularExpressions;
using Svg;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.BaseImage
{
    public partial class ImagePainter
    {
        public void ApplyThemeToSvg()
        {
            if (_svgDocument == null || _currentTheme == null)
                return;

            Color actualFillColor, actualStrokeColor;

            switch (_imageEmbededin)
            {
                case ImageEmbededin.TabPage:
                    actualFillColor = _currentTheme.TabForeColor;
                    actualStrokeColor = _currentTheme.TabForeColor;
                    break;
                case ImageEmbededin.AppBar:
                    actualFillColor = _currentTheme.AppBarForeColor;
                    actualStrokeColor = _currentTheme.AppBarForeColor;
                    break;
                case ImageEmbededin.Menu:
                case ImageEmbededin.MenuBar:
                    actualFillColor = _currentTheme.MenuForeColor;
                    actualStrokeColor = _currentTheme.MenuForeColor;
                    break;
                case ImageEmbededin.TreeView:
                    actualFillColor = _currentTheme.TreeForeColor;
                    actualStrokeColor = _currentTheme.TreeForeColor;
                    break;
                case ImageEmbededin.SideBar:
                    actualFillColor = _currentTheme.SideMenuForeColor;
                    actualStrokeColor = _currentTheme.SideMenuForeColor;
                    break;
                case ImageEmbededin.ListBox:
                case ImageEmbededin.Form:
                case ImageEmbededin.ListView:
                    actualFillColor = _currentTheme.ListForeColor;
                    actualStrokeColor = _currentTheme.ListForeColor;
                    break;
                case ImageEmbededin.Label:
                    actualFillColor = _currentTheme.LabelForeColor;
                    actualStrokeColor = _currentTheme.LabelForeColor;
                    break;
                case ImageEmbededin.TextBox:
                    actualFillColor = _currentTheme.TextBoxForeColor;
                    actualStrokeColor = _currentTheme.TextBoxForeColor;
                    break;
                case ImageEmbededin.ComboBox:
                    actualFillColor = _currentTheme.ComboBoxForeColor;
                    actualStrokeColor = _currentTheme.ComboBoxForeColor;
                    break;
                case ImageEmbededin.DataGridView:
                    actualFillColor = _currentTheme.GridHeaderForeColor;
                    actualStrokeColor = _currentTheme.GridHeaderForeColor;
                    break;
                case ImageEmbededin.Button:
                default:
                    actualFillColor = _fillColor;
                    actualStrokeColor = _strokeColor;
                    break;
            }

            var fillServer = new SvgColourServer(actualFillColor);
            var strokeServer = new SvgColourServer(actualStrokeColor);

            _svgDocument.StrokeWidth = new SvgUnit(2);

            foreach (var node in _svgDocument.Children)
            {
                node.Fill = fillServer;
                node.Color = fillServer;
                node.Stroke = strokeServer;
                node.StrokeWidth = new SvgUnit(2);
                ProcessNodes(node.Descendants(), fillServer, strokeServer);
            }

            _svgDocument.FlushStyles();
            _stateChanged = true;
            InvalidateCache();
        }

        private void ProcessNodes(System.Collections.Generic.IEnumerable<SvgElement> nodes, SvgPaintServer fillServer, SvgPaintServer strokeServer)
        {
            foreach (var node in nodes)
            {
                node.Fill = fillServer;
                node.Color = fillServer;
                node.Stroke = strokeServer;
                node.StrokeWidth = new SvgUnit(2);
                ProcessNodes(node.Descendants(), fillServer, strokeServer);
            }
        }

        public void ApplyColorToAllElements(Color fillColor)
        {
            if (_svgDocument == null)
                return;

            string hexColor = ColorTranslator.ToHtml(fillColor);

            foreach (var element in _svgDocument.Descendants())
            {
                if (element.CustomAttributes.ContainsKey("style"))
                {
                    string style = element.CustomAttributes["style"];
                    style = Regex.Replace(style, @"fill:[^;]+", $"fill:{hexColor}");
                    element.CustomAttributes["style"] = style;
                }

                element.Fill = new SvgColourServer(fillColor);
            }

            _svgDocument.FlushStyles();
            _stateChanged = true;
            InvalidateCache();
        }
    }
}
