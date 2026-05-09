using TheTechIdea.Beep.Winform.Controls.Accessibility;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepRibbonControl
    {
        public int AddContextualGroup(string name, Color color)
        {
            var grp = new ContextualGroup { Name = name, Color = color, Visible = false };
            _contextGroups.Add(grp);
            return _contextGroups.Count - 1;
        }

        public TabPage AddContextualTab(int groupId, string title)
        {
            if (groupId < 0 || groupId >= _contextGroups.Count) throw new ArgumentOutOfRangeException(nameof(groupId));
            var grp = _contextGroups[groupId];
            var page = new TabPage(title) { BackColor = _theme.TabActiveBack };
            grp.Pages.Add(page);
            _pageToGroup[page] = grp;
            if (grp.Visible)
            {
                _tabs.TabPages.Add(page);
            }
            _contextHeader.Invalidate();
            return page;
        }

        public void SetContextualGroupVisible(int groupId, bool visible)
        {
            if (groupId < 0 || groupId >= _contextGroups.Count) return;
            var grp = _contextGroups[groupId];
            if (grp.Visible == visible) return;
            grp.Visible = visible;
            if (visible)
            {
                foreach (var p in grp.Pages)
                {
                    if (!_tabs.TabPages.Contains(p)) _tabs.TabPages.Add(p);
                }
            }
            else
            {
                foreach (var p in grp.Pages)
                {
                    if (_tabs.TabPages.Contains(p)) _tabs.TabPages.Remove(p);
                }
            }
            StartContextTransition();
            _contextHeader.Invalidate();
        }

        public void RegisterContextualRule(string contextKey, int groupId)
        {
            if (string.IsNullOrWhiteSpace(contextKey)) return;
            if (groupId < 0 || groupId >= _contextGroups.Count) return;
            _contextualRuleMap[contextKey] = groupId;
        }

        public void ActivateContext(string? contextKey)
        {
            string key = contextKey?.Trim() ?? string.Empty;
            _activeContextKey = key;

            if (_contextGroups.Count == 0)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(key))
            {
                for (int i = 0; i < _contextGroups.Count; i++)
                {
                    SetContextualGroupVisible(i, false);
                }
                return;
            }

            bool matched = false;
            if (_contextualRuleMap.TryGetValue(key, out int mappedGroup))
            {
                for (int i = 0; i < _contextGroups.Count; i++)
                {
                    bool visible = i == mappedGroup;
                    SetContextualGroupVisible(i, visible);
                    if (visible) matched = true;
                }
            }

            if (!matched)
            {
                for (int i = 0; i < _contextGroups.Count; i++)
                {
                    bool visible = _contextGroups[i].Name.Equals(key, StringComparison.OrdinalIgnoreCase);
                    SetContextualGroupVisible(i, visible);
                    if (visible) matched = true;
                }
            }

            if (!matched)
            {
                for (int i = 0; i < _contextGroups.Count; i++)
                {
                    SetContextualGroupVisible(i, false);
                }
            }
        }

        private void ContextHeader_Paint(object? sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(_theme.Background);

            for (int gi = 0; gi < _contextGroups.Count; gi++)
            {
                var grp = _contextGroups[gi];
                if (!grp.Visible || grp.Pages.Count == 0) continue;

                Rectangle? left = null;
                Rectangle? right = null;
                for (int i = 0; i < _tabs.TabCount; i++)
                {
                    var page = _tabs.TabPages[i];
                    if (!_pageToGroup.TryGetValue(page, out var gref) || gref != grp) continue;
                    var r = _tabs.GetTabRect(i);
                    if (!left.HasValue || r.Left < left.Value.Left) left = r;
                    if (!right.HasValue || r.Right > right.Value.Right) right = r;
                }

                if (!left.HasValue || !right.HasValue) continue;
                var band = new Rectangle(left.Value.Left, 0, right.Value.Right - left.Value.Left, _contextHeader.Height - 1);
                int alpha = Math.Clamp((int)(120 * _contextTransitionProgress), 30, 180);
                using var b = new SolidBrush(Color.FromArgb(alpha, grp.Color));
                using var p = new Pen(grp.Color);
                using var textBrush = new SolidBrush(_theme.Text);
                g.FillRectangle(b, band);
                g.DrawRectangle(p, band);
                g.DrawString(grp.Name, _contextHeader.Font, textBrush, new PointF(band.Left + 6, 2));
            }
        }

        private void ContextTransitionTimer_Tick(object? sender, EventArgs e)
        {
            _contextTransitionProgress += _contextTransitionTimer.Interval / (float)Math.Max(1, _contextTransitionEffectiveDurationMs);
            if (_contextTransitionProgress >= 1f)
            {
                _contextTransitionProgress = 1f;
                _contextTransitionTimer.Stop();
            }

            _contextHeader.Invalidate();
        }

        private void StartContextTransition()
        {
            if (!ShouldAnimateTransitions() || !_enableContextTransitions)
            {
                _contextTransitionProgress = 1f;
                _contextHeader.Invalidate();
                return;
            }

            _contextTransitionEffectiveDurationMs = GetEffectiveTransitionDurationMs(_contextTransitionDurationMs, forBackstage: false);
            _contextTransitionTimer.Interval = Math.Clamp(_contextTransitionEffectiveDurationMs / 12, 10, 24);
            _contextTransitionProgress = 0f;
            _contextTransitionTimer.Stop();
            _contextTransitionTimer.Start();
        }

        private bool ShouldAnimateTransitions()
        {
            if (_reducedMotion)
            {
                return false;
            }

            if (_respectSystemReducedMotion)
            {
                try
                {
                    if (TryGetSystemAnimationPreference(out bool animationsEnabled) && !animationsEnabled)
                    {
                        return false;
                    }
                }
                catch
                {
                    // no-op: default to enabled
                }
            }

            return true;
        }

        private static bool TryGetSystemAnimationPreference(out bool animationsEnabled)
        {
            animationsEnabled = true;

            // Different WinForms target frameworks expose different animation-related properties.
            // Probe common names via reflection to keep this ribbon control compile-safe everywhere.
            var propertyCandidates = new[]
            {
                "IsMinimizeAnimationEnabled",
                "MinimizeAnimation",
                "IsMenuAnimationEnabled",
                "UIEffects"
            };

            var type = typeof(SystemInformation);
            foreach (var propertyName in propertyCandidates)
            {
                try
                {
                    var prop = type.GetProperty(propertyName);
                    if (prop == null || prop.PropertyType != typeof(bool))
                    {
                        continue;
                    }

                    var value = prop.GetValue(null);
                    if (value is bool enabled)
                    {
                        animationsEnabled = enabled;
                        return true;
                    }
                }
                catch
                {
                    // continue trying other candidates
                }
            }

            return false;
        }

        private int GetEffectiveTransitionDurationMs(int configuredDurationMs, bool forBackstage)
        {
            int baseDuration = Math.Max(50, configuredDurationMs);
            if (!_adaptiveTransitionTiming)
            {
                return baseDuration;
            }

            float densityFactor = _density switch
            {
                RibbonDensity.Compact => 0.86f,
                RibbonDensity.Touch => 1.16f,
                _ => 1f
            };

            float presetFactor = _resolvedStylePreset switch
            {
                RibbonStylePreset.FluentLight => 1.06f,
                RibbonStylePreset.FluentDark => 1.08f,
                RibbonStylePreset.OfficeLight => 0.94f,
                RibbonStylePreset.OfficeDark => 0.96f,
                RibbonStylePreset.HighContrast => 0.78f,
                _ => 1f
            };

            float typeFactor = forBackstage ? 1.08f : 0.94f;
            int effective = (int)Math.Round(baseDuration * densityFactor * presetFactor * typeFactor);
            return Math.Clamp(effective, 40, 420);
        }
    }
}
