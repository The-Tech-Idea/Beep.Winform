namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepRibbonControl
    {
        private void RefreshKeyTips()
        {
            _keyTips.Clear();
            _keyTipLookup.Clear();

            if (!_enableKeyTips)
            {
                return;
            }

            _keyTips[_backstageButton] = "F";
            _keyTipLookup["F"] = _backstageButton;

            int qIndex = 1;
            foreach (ToolStripItem item in _quickAccess.Items)
            {
                if (!CanAssignKeyTip(item) || item == _backstageButton) continue;
                if (qIndex > 9) break;
                string keyTip = qIndex.ToString();
                _keyTips[item] = keyTip;
                _keyTipLookup[keyTip] = item;
                qIndex++;
            }

            var tab = _tabStrip.SelectedTab;
            if (tab?.ContentPanel == null) return;
            var panel = tab.ContentPanel;

            int alphaIndex = 0;
            foreach (var group in panel.Controls.OfType<BeepRibbonGroup>())
            {
                // Commands in a group are controls now, so this walks the control tree rather than a
                // ToolStripItemCollection. It also means the overflowed commands are reachable: they
                // live in the overflow button's menu, which the button itself carries a key tip for.
                foreach (var item in group.ItemControls)
                {
                    if (!CanAssignKeyTip(item)) continue;
                    string keyTip = GetAlphaKeyTip(alphaIndex++);
                    _keyTips[item] = keyTip;
                    _keyTipLookup[keyTip] = item;
                }
            }
        }

        private static bool CanAssignKeyTip(object target) => target switch
        {
            ToolStripItem item => item is not ToolStripSeparator &&
                                  item.Available &&
                                  item.Visible &&
                                  item.Enabled &&
                                  item is not ToolStripTextBox,

            // Control has no Available; the equivalent question is whether it and its parent are both
            // showing, which is exactly what ToolStripItem.Available answers for an item.
            Control control => control.Visible &&
                               control.Enabled &&
                               control.Parent is { Visible: true },

            _ => false
        };

        private static string GetAlphaKeyTip(int index)
        {
            index = Math.Max(0, index);
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            if (index < chars.Length)
            {
                return chars[index].ToString();
            }

            int first = index / chars.Length - 1;
            int second = index % chars.Length;
            first = Math.Clamp(first, 0, chars.Length - 1);
            return $"{chars[first]}{chars[second]}";
        }

        /// <summary>Where a key tip badge is anchored, and on which control it is shown.</summary>
        private static bool TryGetKeyTipAnchor(object target, out Control owner, out Rectangle bounds)
        {
            switch (target)
            {
                case ToolStripItem item when item.Owner != null:
                    owner = item.Owner;
                    bounds = item.Bounds;
                    return true;

                case Control control when control.Parent != null:
                    owner = control.Parent;
                    bounds = control.Bounds;
                    return true;

                default:
                    owner = null!;
                    bounds = Rectangle.Empty;
                    return false;
            }
        }

        private void ShowKeyTips()
        {
            if (!_enableKeyTips) return;
            RefreshKeyTips();
            if (_keyTips.Count == 0) return;

            _keyTipsVisible = true;
            _keyTipInputBuffer = string.Empty;
            foreach (var kv in _keyTips)
            {
                if (!TryGetKeyTipAnchor(kv.Key, out var owner, out var bounds)) continue;
                var point = new Point(bounds.Left + Math.Max(2, bounds.Width / 2 - 8), Math.Max(0, bounds.Top - 18));
                _keyTipToolTip.Show(kv.Value, owner, point, 30000);
            }
        }

        private void HideKeyTips()
        {
            if (!_keyTipsVisible) return;
            var owners = new List<Control>();
            foreach (var target in _keyTips.Keys)
            {
                if (!TryGetKeyTipAnchor(target, out var owner, out _)) continue;
                if (!owners.Contains(owner)) owners.Add(owner);
            }

            foreach (var owner in owners)
            {
                _keyTipToolTip.Hide(owner);
            }

            _keyTipInputBuffer = string.Empty;
            _keyTipsVisible = false;
        }

        private void RefreshKeyTipsVisibility()
        {
            if (!_keyTipsVisible) return;
            HideKeyTips();
            ShowKeyTips();
        }

        private bool TryInvokeKeyTip(Keys keyData)
        {
            string token = NormalizeKeyToken(keyData);
            if (string.IsNullOrWhiteSpace(token))
            {
                return false;
            }

            if ((DateTime.UtcNow - _lastKeyTipInput).TotalSeconds > 1.6)
            {
                _keyTipInputBuffer = string.Empty;
            }

            _lastKeyTipInput = DateTime.UtcNow;
            _keyTipInputBuffer += token;

            if (_keyTipLookup.TryGetValue(_keyTipInputBuffer, out var exactTarget))
            {
                InvokeKeyTipTarget(exactTarget);
                HideKeyTips();
                return true;
            }

            bool hasPrefix = _keyTipLookup.Keys.Any(k => k.StartsWith(_keyTipInputBuffer, StringComparison.OrdinalIgnoreCase));
            if (hasPrefix)
            {
                return true;
            }

            _keyTipInputBuffer = token;
            if (_keyTipLookup.TryGetValue(_keyTipInputBuffer, out exactTarget))
            {
                InvokeKeyTipTarget(exactTarget);
                HideKeyTips();
                return true;
            }

            _keyTipInputBuffer = string.Empty;
            return false;
        }

        private static string NormalizeKeyToken(Keys keyData)
        {
            Keys keyCode = keyData & Keys.KeyCode;
            if (keyCode >= Keys.A && keyCode <= Keys.Z)
            {
                return keyCode.ToString();
            }

            if (keyCode >= Keys.D0 && keyCode <= Keys.D9)
            {
                return ((int)(keyCode - Keys.D0)).ToString();
            }

            if (keyCode >= Keys.NumPad0 && keyCode <= Keys.NumPad9)
            {
                return ((int)(keyCode - Keys.NumPad0)).ToString();
            }

            return string.Empty;
        }

        private void InvokeKeyTipTarget(object target)
        {
            switch (target)
            {
                // A ribbon command with a menu opens it from its own OnClick, so PerformClick is the
                // one call that covers both a plain command and a drop-down.
                case BeepButton button:
                    button.PerformClick();
                    break;
                case ToolStripDropDownButton dropDownButton when dropDownButton.HasDropDownItems:
                    dropDownButton.ShowDropDown();
                    break;
                case ToolStripItem item:
                    item.PerformClick();
                    break;
                case Control control:
                    control.Focus();
                    break;
            }
        }
    }
}
