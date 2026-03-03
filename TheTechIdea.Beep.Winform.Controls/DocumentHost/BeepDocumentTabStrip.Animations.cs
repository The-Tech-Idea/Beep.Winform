// BeepDocumentTabStrip.Animations.cs
// Tab open/close animations — width morphing driven by a 16 ms timer.
// AnimationDuration (ms) controls speed; 0 = instant (no animation).
// ─────────────────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    public partial class BeepDocumentTabStrip
    {
        // ── Animation state ───────────────────────────────────────────────────
        // _openingTabs: tabId → progress [0 .. 1]  (0 = zero-width, 1 = full-width)

        private readonly Dictionary<string, float> _openingTabs = new Dictionary<string, float>();
        private System.Windows.Forms.Timer? _tabAnimTimer;

        // ─────────────────────────────────────────────────────────────────────
        // Public / internal entry-points
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Starts a width-open animation for the tab with the given id.
        /// No-op when <see cref="AnimationDuration"/> is 0.
        /// </summary>
        internal void StartOpenAnimation(string tabId)
        {
            if (_animationDuration <= 0) return;
            _openingTabs[tabId] = 0f;
            EnsureTabAnimTimer();
            _tabAnimTimer!.Start();
        }

        // ─────────────────────────────────────────────────────────────────────
        // Width helper used by CalculateTabLayout
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Returns the animated effective width for a tab.
        /// If the tab is currently opening, width is lerped from 0 to <paramref name="targetWidth"/>.
        /// Otherwise returns <paramref name="targetWidth"/> unchanged.
        /// </summary>
        internal int GetAnimatedWidth(string tabId, int targetWidth)
        {
            if (_openingTabs.TryGetValue(tabId, out float progress))
                return Math.Max(1, (int)Math.Round(targetWidth * progress));
            return targetWidth;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Timer
        // ─────────────────────────────────────────────────────────────────────

        private void EnsureTabAnimTimer()
        {
            if (_tabAnimTimer != null) return;
            _tabAnimTimer = new System.Windows.Forms.Timer { Interval = 16 };
            _tabAnimTimer.Tick += OnTabAnimTick;
        }

        private void OnTabAnimTick(object? sender, EventArgs e)
        {
            if (_animationDuration <= 0)
            {
                _openingTabs.Clear();
                _tabAnimTimer?.Stop();
                return;
            }

            float step = 16f / _animationDuration;
            bool  anyActive = false;

            foreach (var key in _openingTabs.Keys.ToList())
            {
                float next = _openingTabs[key] + step;
                if (next >= 1f)
                    _openingTabs.Remove(key);
                else
                {
                    _openingTabs[key] = next;
                    anyActive = true;
                }
            }

            if (!anyActive)
                _tabAnimTimer?.Stop();

            CalculateTabLayout();
            Invalidate();
        }

        // ─────────────────────────────────────────────────────────────────────
        // Badge pulse animation (7.4)
        // Scale oscillates: 1.0 → 1.25 → 1.0 over ~300 ms then stops.
        // ─────────────────────────────────────────────────────────────────────

        // tabId → phase [0 .. 1]; 0 = just started, 1 = finished
        internal readonly Dictionary<string, float> _badgePulseScale = new Dictionary<string, float>();
        private System.Windows.Forms.Timer? _badgeAnimTimer;

        /// <summary>
        /// Starts a one-shot badge pulse (scale 1→1.25→1) for the given document id.
        /// Call this whenever <c>BadgeText</c> changes.
        /// </summary>
        internal void StartBadgePulse(string tabId)
        {
            _badgePulseScale[tabId] = 0f;
            EnsureBadgeTimer();
            _badgeAnimTimer!.Start();
        }

        private void EnsureBadgeTimer()
        {
            if (_badgeAnimTimer != null) return;
            _badgeAnimTimer = new System.Windows.Forms.Timer { Interval = 16 };
            _badgeAnimTimer.Tick += OnBadgePulseTick;
        }

        private void OnBadgePulseTick(object? sender, EventArgs e)
        {
            const float duration = 300f; // ms
            float step    = 16f / duration;
            bool  anyLeft = false;

            foreach (var key in _badgePulseScale.Keys.ToList())
            {
                float phase = _badgePulseScale[key] + step;
                if (phase >= 1f)
                {
                    _badgePulseScale.Remove(key);
                }
                else
                {
                    _badgePulseScale[key] = phase;
                    anyLeft = true;
                }
            }

            if (!anyLeft) _badgeAnimTimer?.Stop();
            Invalidate();
        }
    }
}
