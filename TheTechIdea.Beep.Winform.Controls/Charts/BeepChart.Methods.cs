using System.ComponentModel;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Charts.Helpers;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls.Charts
{
    public partial class BeepChart
    {
        #region Public Methods
        public void StartAnimation(int durationMs = 600)
        {
            _seriesOptions.AnimationProgress = 0f;
            _animTimer ??= new System.Windows.Forms.Timer();
            _animTimer.Interval = 16;
            int elapsed = 0;
            
            void Tick(object s, EventArgs e)
            {
                elapsed += _animTimer.Interval;
                _seriesOptions.AnimationProgress = Math.Min(1f, elapsed / (float)durationMs);
                Invalidate();
                if (_seriesOptions.AnimationProgress >= 1f)
                {
                    _animTimer.Stop();
                    _animTimer.Tick -= Tick;
                }
            }
            
            _animTimer.Tick -= Tick;
            _animTimer.Tick += Tick;
            _animTimer.Start();
        }

        public object ConvertXValue(ChartDataPoint point)
        {
            return BeepChartDataHelper.ConvertXValue(this, point);
        }

        public object ConvertYValue(ChartDataPoint point)
        {
            return BeepChartDataHelper.ConvertYValue(this, point);
        }
        #endregion

        #region Internal Methods
        internal void ToggleSeriesByIndex(int index)
        {
            if (index >= 0 && index < _dataSeries.Count)
            {
                _dataSeries[index].Visible = !_dataSeries[index].Visible;
                Invalidate();
            }
        }

        internal void ShowTooltip(ChartDataPoint point)
        {
            if (_dataPointToolTip != null)
            {
                string tooltipText = point.ToolTip ?? $"{point.X}, {point.Y}";
                _dataPointToolTip.Show(tooltipText, this, PointToClient(MousePosition), 3000);
            }
        }
        #endregion

        #region Private Initialization Methods
        private void InitializeDefaultSettings()
        {
            if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
            {
                _dataPointToolTip = new ToolTip();
            }
            BeepChartViewportHelper.UpdateChartDrawingRectBase(this);
            BeepChartViewportHelper.AutoScaleViewport(this);
            var t = BeepThemesManager.CurrentTheme;
            ChartBackColor         = t.ChartBackColor;
            ChartLineColor         = t.ChartLineColor;
            ChartFillColor         = t.ChartFillColor;
            ChartAxisColor         = t.ChartAxisColor;
            ChartTitleColor        = t.ChartTitleColor;
            ChartTitleForeColor    = t.ChartTitleColor;
            ChartTextColor         = t.ChartTextColor;
            ChartLegendBackColor   = t.ChartLegendBackColor;
            ChartLegendTextColor   = t.ChartLegendTextColor;
            ChartLegendShapeColor  = t.ChartLegendShapeColor;
            ChartGridLineColor     = t.ChartGridLineColor;
            ChartDefaultSeriesColors = new List<Color>(t.ChartDefaultSeriesColors);
            ChartTitleFont    = BeepThemesManager.ToFont(t.ChartTitleFont   ?? t.TitleStyle) ?? SystemFonts.DefaultFont;
            ChartValueFont    = BeepThemesManager.ToFont(t.ChartSubTitleFont ?? t.GetBlockHeaderFont()) ?? SystemFonts.DefaultFont;
            ChartSubtitleFont = BeepThemesManager.ToFont(t.GetBlockTextFont()) ?? SystemFonts.DefaultFont;
        }

        private void InitializeDesignTimeSampleData()
        {
            // DesignMode property is always false inside a constructor,
            // so use LicenseManager.UsageMode which works at construction time.
            bool isDesignTime = LicenseManager.UsageMode == LicenseUsageMode.Designtime
                                || DesignMode
                                || (Site != null && Site.DesignMode);
            if (isDesignTime)
            {
                _dataSeries = new List<ChartDataSeries>
                {
                    new ChartDataSeries
                    {
                        Name = "Sample Series",
                        ChartType = ChartType.Line,
                        ShowLine = true,
                        ShowPoint = true,
                        Visible = true,
                        Points = new List<ChartDataPoint>
                        {
                            new ChartDataPoint("1", "5", 10f, "A", Color.Red),
                            new ChartDataPoint("2", "15", 15f, "B", Color.Green),
                            new ChartDataPoint("3", "8", 20f, "C", Color.Blue)
                        }
                    }
                };
                BeepChartDataHelper.DetectAxisTypes(this);
                BeepChartViewportHelper.UpdateChartDrawingRectBase(this);
                BeepChartViewportHelper.AutoScaleViewport(this);
            }
        }

        internal void InitializePainter()
        {
            switch (_surfaceStyle)
            {
                case ChartSurfaceStyle.Card:
                    _chartpainter = new CardChartPainter();
                    break;
                case ChartSurfaceStyle.Outline:
                    _chartpainter = new OutlineChartPainter();
                    break;
                case ChartSurfaceStyle.Glass:
                    _chartpainter = new GlassChartPainter2();
                    break;
                case ChartSurfaceStyle.Classic:
                default:
                    _chartpainter = new CardChartPainter();
                    break;
            }
            _chartpainter?.Initialize(this, _currentTheme);

            _axisPainter = new CartesianAxisPainter();

            // Series painter will be set dynamically based on chart type
            _lastSeriesType = ChartType;
            _seriesPainter = SeriesPainterFactory.GetPainter(_lastSeriesType);
            _seriesPainter.Initialize(this);
        }
        #endregion
    }
}