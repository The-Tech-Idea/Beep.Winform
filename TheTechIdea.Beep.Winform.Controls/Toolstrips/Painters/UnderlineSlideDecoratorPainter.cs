using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using Timer = System.Windows.Forms.Timer;

namespace TheTechIdea.Beep.Winform.Controls.Toolstrips.Painters
{
    // Underline slide animator wrapper painter that can wrap any base painter kind.
    internal sealed class UnderlineSlideDecoratorPainter : IToolStripPainter
    {
        private readonly IToolStripPainter _inner;
        private readonly Timer _timer = new Timer { Interval = 16 };
        private float _t; // 0..1 progression
        private int _fromIndex = -1;
        private int _toIndex = -1;
        private Rectangle _fromRect;
        private Rectangle _toRect;

        // Animation config/state
        private BeepToolStrip _owner;
        private int _lastIndex = -1;
        private DateTime _animStart;
        private double _durationMs;                 // when using duration-based animation
        private bool _useDuration;                  // if true, use time-based; else step-based
        private float _speedStep = 0.15f;           // step-based fallback
        private string _easing = "linear";          // easing name

        public string Key => "UnderlineSlide";

        public UnderlineSlideDecoratorPainter(IToolStripPainter inner)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
            _timer.Tick += (s, e) =>
            {
                if (_useDuration)
                {
                    var elapsed = (DateTime.UtcNow - _animStart).TotalMilliseconds;
                    _t = (float)Math.Max(0, Math.Min(1, elapsed / Math.Max(1.0, _durationMs)));
                    if (_t >= 1f) _timer.Stop();
                }
                else
                {
                    _t += _speedStep;
                    if (_t >= 1f) { _t = 1f; _timer.Stop(); }
                }
                _owner?.Invalidate();
            };
        }

        public void Paint(Graphics g, Rectangle bounds, IBeepTheme theme, BeepToolStrip owner, IReadOnlyDictionary<string, object> parameters)
        {
            _owner = owner;
            _inner.Paint(g, bounds, theme, owner, parameters);
            if (owner is null) return;

            int underline = GetInt(parameters, "Underline", 2);

            // Read animation parameters (with sensible defaults)
            // UnderlineDuration (ms). If present > 0 -> duration-based; otherwise fall back to UnderlineSpeed (step per tick)
            _durationMs = GetDouble(parameters, "UnderlineDuration", 0);
            _useDuration = _durationMs > 0;
            _speedStep = (float)GetDouble(parameters, "UnderlineSpeed", 0.15);
            _easing = GetString(parameters, "UnderlineEasing", theme?.AnimationEasingFunction ?? "linear");

            if (owner.SelectedIndex != _lastIndex)
            {
                _fromIndex = _lastIndex;
                _toIndex = owner.SelectedIndex;
                _lastIndex = owner.SelectedIndex;

                if (_inner is TabsBasePainter tb)
                {
                    if (_fromIndex >= 0 && _fromIndex < owner.Buttons.Count && tb.LastItemRects.Count > _fromIndex)
                        _fromRect = tb.LastItemRects[_fromIndex];
                    if (_toIndex >= 0 && _toIndex < owner.Buttons.Count && tb.LastItemRects.Count > _toIndex)
                        _toRect = tb.LastItemRects[_toIndex];
                }

                _t = 0f;
                _animStart = DateTime.UtcNow;
                _timer.Start();
            }

            if (_toIndex >= 0 && _inner is TabsBasePainter tbp && tbp.LastItemRects.Count > _toIndex)
            {
                var target = _toRect;
                float te = ApplyEasing(_t, _easing);
                if (te < 1f && _fromIndex >= 0)
                {
                    int left = (int)(Lerp(_fromRect.Left, _toRect.Left, te));
                    int width = (int)(Lerp(_fromRect.Width, _toRect.Width, te));
                    target = new Rectangle(left, _toRect.Bottom - underline - 1, width, underline);
                }
                else
                {
                    target = new Rectangle(_toRect.Left, _toRect.Bottom - underline - 1, _toRect.Width, underline);
                }

                using var brush = new SolidBrush(theme.MenuMainItemSelectedForeColor);
                g.FillRectangle(brush, target);
            }
        }

        public void UpdateHitAreas(BeepToolStrip owner, Rectangle bounds, IBeepTheme theme, IReadOnlyDictionary<string, object> parameters, Action<string, Rectangle> register)
        {
            _inner.UpdateHitAreas(owner, bounds, theme, parameters, register);
        }

        private static float Lerp(float a, float b, float t) => (float)(a + (b - a) * t);

        private static int GetInt(IReadOnlyDictionary<string, object> p, string key, int fallback)
            => p != null && p.TryGetValue(key, out var v) && v is IConvertible ? Convert.ToInt32(v) : fallback;
        private static double GetDouble(IReadOnlyDictionary<string, object> p, string key, double fallback)
            => p != null && p.TryGetValue(key, out var v) && v is IConvertible ? Convert.ToDouble(v) : fallback;
        private static string GetString(IReadOnlyDictionary<string, object> p, string key, string fallback)
            => p != null && p.TryGetValue(key, out var v) && v is string s ? s : fallback;

        private static float ApplyEasing(float t, string easing)
        {
            t = Math.Max(0f, Math.Min(1f, t));
            if (string.IsNullOrEmpty(easing)) return t;
            switch (easing.Trim().ToLowerInvariant())
            {
                case "linear": return t;
                case "ease-in":
                case "quad-in": return t * t;
                case "ease-out":
                case "quad-out": return 1f - (1f - t) * (1f - t);
                case "ease-in-out":
                case "quad-in-out": return t < 0.5f ? 2f * t * t : 1f - (float)Math.Pow(-2f * t + 2f, 2) / 2f;
                case "cubic-in": return t * t * t;
                case "cubic-out": { var u = 1f - t; return 1f - u * u * u; }
                case "cubic-in-out": return t < 0.5f ? 4f * t * t * t : 1f - (float)Math.Pow(-2f * t + 2f, 3) / 2f;
                case "quint-in": return t * t * t * t * t;
                case "quint-out": { var u = 1f - t; return 1f - u * u * u * u * u; }
                case "quint-in-out": return t < 0.5f ? 16f * t * t * t * t * t : 1f - (float)Math.Pow(-2f * t + 2f, 5) / 2f;
                default: return t;
            }
        }
    }
}
