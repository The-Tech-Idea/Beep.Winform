using System;
using System.Collections.Generic;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.StatusCards.Painters;

namespace TheTechIdea.Beep.Winform.Controls.StatusCards
{
    public partial class BeepStatCard
    {
        private readonly Dictionary<StatCardPainterKind, IStatCardPainter> _painters = new();
        private StatCardPainterKind _painterKind = StatCardPainterKind.SimpleKpi;
        private Dictionary<string, object> _parameters = new();

        public StatCardPainterKind PainterKind
        {
            get => _painterKind;
            set { _painterKind = value; Invalidate(); }
        }

        public Dictionary<string, object> Parameters
        {
            get => _parameters;
            set { _parameters = value ?? new Dictionary<string, object>(); Invalidate(); }
        }

        public static readonly string ParamHeader = "Header";
        public static readonly string ParamValue = "Value";
        public static readonly string ParamDelta = "Delta";
        public static readonly string ParamInfo = "Info";
        public static readonly string ParamLabels = "Labels";
        public static readonly string ParamSeries = "Series";
        public static readonly string ParamSeries2 = "Series2";
        public static readonly string ParamSpark = "Spark";

        private void EnsureDefaultPainters()
        {
            if (_painters.Count > 0) return;
            RegisterPainter(StatCardPainterKind.SimpleKpi, new SimpleKpiPainter());
            RegisterPainter(StatCardPainterKind.HeartRate, new HeartRatePainter());
            RegisterPainter(StatCardPainterKind.EnergyActivity, new EnergyActivityPainter());
            RegisterPainter(StatCardPainterKind.Performance, new PerformancePainter());
        }

        public void RegisterPainter(StatCardPainterKind kind, IStatCardPainter painter)
        {
            if (painter == null) return;
            _painters[kind] = painter;
        }

        private IStatCardPainter? GetActivePainter()
        {
            EnsureDefaultPainters();
            return _painters.TryGetValue(_painterKind, out var p) ? p : null;
        }
    }
}
