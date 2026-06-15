using TheTechIdea.Beep.Winform.Controls.Helpers;

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
                foreach (ToolStripItem item in group.Items)
                {
                    if (!CanAssignKeyTip(item)) continue;
                    string keyTip = GetAlphaKeyTip(alphaIndex++);
                    _keyTips[item] = keyTip;
                    _keyTipLookup[keyTip] = item;
                }
            }
        }

        private static bool CanAssignKeyTip(ToolStripItem item)
        {
            return item is not ToolStripSeparator &&
                   item.Available &&
                   item.Visible &&
                   item.Enabled &&
                   item is not ToolStripTextBox;
        }

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

        private void ShowKeyTips()
        {
            if (!_enableKeyTips) return;
            RefreshKeyTips();
            if (_keyTips.Count == 0) return;

            _keyTipsVisible = true;
            _keyTipInputBuffer = string.Empty;
            foreach (var kv in _keyTips)
            {
                var item = kv.Key;
                var owner = item.Owner;
                if (owner == null) continue;
                var bounds = item.Bounds;
                var point = new Point(bounds.Left + Math.Max(2, bounds.Width / 2 - 8), Math.Max(0, bounds.Top - 18));
                _keyTipToolTip.Show(kv.Value, owner, point, 30000);
            }
        }

        private void HideKeyTips()
        {
            if (!_keyTipsVisible) return;
            var owners = _keyTips.Keys
                .Select(k => k.Owner)
                .Where(o => o != null)
                .Distinct()
                .ToList();
            foreach (var owner in owners)
            {
                _keyTipToolTip.Hide(owner!);
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

            if (_keyTipLookup.TryGetValue(_keyTipInputBuffer, out var exactItem))
            {
                InvokeToolStripItem(exactItem);
                HideKeyTips();
                return true;
            }

            bool hasPrefix = _keyTipLookup.Keys.Any(k => k.StartsWith(_keyTipInputBuffer, StringComparison.OrdinalIgnoreCase));
            if (hasPrefix)
            {
                return true;
            }

            _keyTipInputBuffer = token;
            if (_keyTipLookup.TryGetValue(_keyTipInputBuffer, out exactItem))
            {
                InvokeToolStripItem(exactItem);
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

        private void InvokeToolStripItem(ToolStripItem item)
        {
            switch (item)
            {
                case ToolStripButton button:
                    button.PerformClick();
                    break;
                case ToolStripMenuItem menuItem:
                    menuItem.PerformClick();
                    break;
                case ToolStripDropDownButton dropDownButton:
                    if (dropDownButton.HasDropDownItems)
                    {
                        dropDownButton.ShowDropDown();
                    }
                    else
                    {
                        dropDownButton.PerformClick();
                    }
                    break;
                default:
                    item.PerformClick();
                    break;
            }
        }
    }
}
