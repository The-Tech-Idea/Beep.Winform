using System.ComponentModel;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Charts.Helpers;

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
                BeepChartViewportHelper.UpdateChartDrawingRectBase(this);
                BeepChartViewportHelper.AutoScaleViewport(this);
            }
        }

        private void InitializeDesignTimeSampleData()
        {
            if (DesignMode)
            {
                DataSeries = new List<ChartDataSeries>
                {
                    new ChartDataSeries
                    {
                        Name = "Sample Series",
                        ChartType = ChartType.Line,
                        ShowLine = true,
                        ShowPoint = true,
                        Points = new List<ChartDataPoint>
                        {
                            new ChartDataPoint("1", "5", 10f, "A", Color.Red),
                            new ChartDataPoint("2", "15", 15f, "B", Color.Green),
                            new ChartDataPoint("3", "8", 20f, "C", Color.Blue)
                        }
                    }
                };
                BeepChartViewportHelper.UpdateChartDrawingRectBase(this);
                BeepChartViewportHelper.AutoScaleViewport(this);
            }
        }

        internal void InitializePainter()
        {
            switch (_surfaceStyle)
            {
                case ChartSurfaceStyle.Card:
                    _painter = new CardChartPainter();
                    break;
                case ChartSurfaceStyle.Outline:
                    _painter = new OutlineChartPainter();
                    break;
                case ChartSurfaceStyle.Glass:
                    _painter = new GlassChartPainter2();
                    break;
                case ChartSurfaceStyle.Classic:
                default:
                    _painter = new CardChartPainter();
                    break;
            }
            _painter?.Initialize(this, _currentTheme);

            _axisPainter = new CartesianAxisPainter();

            // Series painter will be set dynamically based on chart type
            _lastSeriesType = ChartType;
            _seriesPainter = SeriesPainterFactory.GetPainter(_lastSeriesType);
            _seriesPainter.Initialize(this);
        }
        #endregion
    }
}