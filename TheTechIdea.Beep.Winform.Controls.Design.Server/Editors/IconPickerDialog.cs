using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Icons;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Editors
{
    /// <summary>
    /// Dialog for browsing and selecting icons from the SvgsUI, Svgs, and SvgsDatasources libraries
    /// Provides categorization, search, favorites, and recent icons
    /// </summary>
    public class IconPickerDialog : Form
    {
        private TabControl _tabControl;
        private ListView _categoryList;
        private ListView _iconList;
        private Panel _previewPanel;
        private TextBox _searchBox;
        private ComboBox _sourceFilter;
        private Button _okButton;
        private Button _cancelButton;
        private Label _iconNameLabel;
        private Label _iconPathLabel;
        private CheckBox _showRecentCheckBox;
        
        private List<IconInfo> _allIcons;
        private List<string> _recentIcons = new();
        private HashSet<string> _favoriteIcons = new();
        
        public string? SelectedIconPath { get; private set; }
        public string? SelectedIconName { get; private set; }

        private class IconInfo
        {
            public string Name { get; set; } = "";
            public string Path { get; set; } = "";
            public string Category { get; set; } = "";
            public string Source { get; set; } = ""; // "UI", "General", "DataSources"
        }

        public IconPickerDialog(string? currentIcon)
        {
            SelectedIconPath = currentIcon;
            InitializeComponents();
            LoadAllIcons();
            LoadCategories();
            
            if (!string.IsNullOrEmpty(currentIcon))
            {
                SelectIcon(currentIcon);
            }
        }

        private void InitializeComponents()
        {
            Text = "Icon Picker - Select from Icon Library";
            Size = new Size(900, 650);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            // Tab control for different icon libraries
            _tabControl = new TabControl
            {
                Location = new Point(12, 12),
                Size = new Size(876, 24),
                Appearance = TabAppearance.Buttons
            };
            _tabControl.TabPages.Add("UI Icons");
            _tabControl.TabPages.Add("General Icons");
            _tabControl.TabPages.Add("DataSource Icons");
            _tabControl.TabPages.Add("Favorites");
            _tabControl.TabPages.Add("Recent");
            _tabControl.SelectedIndexChanged += TabControl_SelectedIndexChanged;

            // Search box
            _searchBox = new TextBox
            {
                Location = new Point(12, 44),
                Size = new Size(680, 24),
                PlaceholderText = "Search icons... (e.g., 'check', 'arrow', 'user')"
            };
            _searchBox.TextChanged += SearchBox_TextChanged;

            _sourceFilter = new ComboBox
            {
                Location = new Point(700, 44),
                Size = new Size(188, 24),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _sourceFilter.Items.AddRange(new object[] { "All Sources", "UI", "General", "DataSources" });
            _sourceFilter.SelectedIndex = 0;
            _sourceFilter.SelectedIndexChanged += (s, e) =>
            {
                if (_tabControl.SelectedIndex < 3)
                {
                    LoadCategories();
                }
            };

            // Show recent checkbox
            _showRecentCheckBox = new CheckBox
            {
                Text = "Show only recent",
                Location = new Point(700, 74),
                Size = new Size(180, 24),
                Visible = false
            };
            _showRecentCheckBox.CheckedChanged += (s, e) => RefreshIconList();

            // Category list
            _categoryList = new ListView
            {
                Location = new Point(12, 104),
                Size = new Size(180, 400),
                View = View.List,
                FullRowSelect = true,
                HideSelection = false
            };
            _categoryList.SelectedIndexChanged += CategoryList_SelectedIndexChanged;

            // Icon list
            _iconList = new ListView
            {
                Location = new Point(200, 104),
                Size = new Size(320, 400),
                View = View.Details,
                FullRowSelect = true,
                HideSelection = false,
                MultiSelect = false
            };
            _iconList.Columns.Add("Icon Name", 300);
            _iconList.SelectedIndexChanged += IconList_SelectedIndexChanged;
            _iconList.DoubleClick += (s, e) => DialogResult = DialogResult.OK;

            // Preview panel
            _previewPanel = new Panel
            {
                Location = new Point(528, 104),
                Size = new Size(360, 300),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };
            _previewPanel.Paint += PreviewPanel_Paint;

            // Icon name label
            _iconNameLabel = new Label
            {
                Location = new Point(528, 412),
                Size = new Size(360, 24),
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Text = "No icon selected"
            };

            // Icon path label
            _iconPathLabel = new Label
            {
                Location = new Point(528, 440),
                Size = new Size(360, 64),
                BorderStyle = BorderStyle.FixedSingle,
                TextAlign = ContentAlignment.TopLeft,
                Padding = new Padding(4),
                Font = new Font("Consolas", 8),
                Text = "Icon path will appear here"
            };

            // Buttons
            _okButton = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Location = new Point(732, 560),
                Size = new Size(75, 32),
                Enabled = false
            };

            _cancelButton = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Location = new Point(813, 560),
                Size = new Size(75, 32)
            };

            Controls.AddRange(new Control[] { _tabControl, _searchBox, _sourceFilter, _showRecentCheckBox, 
                _categoryList, _iconList, _previewPanel, _iconNameLabel, _iconPathLabel, 
                _okButton, _cancelButton });

            AcceptButton = _okButton;
            CancelButton = _cancelButton;
        }

        private void LoadAllIcons()
        {
            _allIcons = IconCatalog.GetAllEntries()
                .Select(e => new IconInfo
                {
                    Name = e.Name,
                    Path = e.Path,
                    Category = e.Category,
                    Source = e.Source
                })
                .ToList();
        }

        private void LoadCategories()
        {
            _categoryList.Items.Clear();
            
            var currentSource = GetCurrentSource();
            var selectedSource = _sourceFilter.SelectedItem as string;
            var icons = _allIcons.AsEnumerable();

            if (!string.Equals(selectedSource, "All Sources", StringComparison.OrdinalIgnoreCase))
            {
                icons = icons.Where(i => string.Equals(i.Source, selectedSource, StringComparison.OrdinalIgnoreCase));
            }
            else if (!string.IsNullOrWhiteSpace(currentSource))
            {
                icons = icons.Where(i => i.Source == currentSource);
            }

            var materialized = icons.ToList();
            
            var categories = materialized.Select(i => i.Category).Distinct().OrderBy(c => c).ToList();
            
            _categoryList.Items.Add(new ListViewItem("All") { Tag = "All" });
            
            foreach (var category in categories)
            {
                _categoryList.Items.Add(new ListViewItem($"{category} ({materialized.Count(i => i.Category == category)})") 
                    { Tag = category });
            }

            if (_categoryList.Items.Count > 0)
            {
                _categoryList.Items[0].Selected = true;
            }
        }

        private string GetCurrentSource()
        {
            var source = _tabControl.SelectedIndex switch
            {
                0 => "UI",
                1 => "General",
                2 => "DataSources",
                _ => string.Empty
            };

            return source;
        }

        private void TabControl_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (_tabControl.SelectedIndex == 3) // Favorites
            {
                LoadFavorites();
            }
            else if (_tabControl.SelectedIndex == 4) // Recent
            {
                LoadRecent();
            }
            else
            {
                LoadCategories();
            }
        }

        private void CategoryList_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (_categoryList.SelectedItems.Count == 0) return;

            var selected = _categoryList.SelectedItems[0];
            var category = selected.Tag as string;

            RefreshIconList(category);
        }

        private void RefreshIconList(string? category = null)
        {
            _iconList.Items.Clear();

            var currentSource = GetCurrentSource();
            var selectedSource = _sourceFilter.SelectedItem as string;
            IEnumerable<IconInfo> icons = _allIcons;

            if (!string.Equals(selectedSource, "All Sources", StringComparison.OrdinalIgnoreCase))
            {
                icons = icons.Where(i => string.Equals(i.Source, selectedSource, StringComparison.OrdinalIgnoreCase));
            }
            else if (!string.IsNullOrWhiteSpace(currentSource))
            {
                icons = icons.Where(i => i.Source == currentSource);
            }

            if (category != null && category != "All")
            {
                icons = icons.Where(i => i.Category == category);
            }

            var searchText = _searchBox.Text.ToLowerInvariant();
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                icons = icons.Where(i => i.Name.ToLowerInvariant().Contains(searchText));
            }

            foreach (var icon in icons.OrderBy(i => i.Name))
            {
                var item = new ListViewItem(icon.Name) { Tag = icon };
                _iconList.Items.Add(item);
            }
        }

        private void LoadFavorites()
        {
            _categoryList.Items.Clear();
            _iconList.Items.Clear();

            var favoriteIcons = _allIcons.Where(i => _favoriteIcons.Contains(i.Path)).ToList();
            
            foreach (var icon in favoriteIcons.OrderBy(i => i.Name))
            {
                var item = new ListViewItem($"{icon.Name} ({icon.Source})") { Tag = icon };
                _iconList.Items.Add(item);
            }
        }

        private void LoadRecent()
        {
            _categoryList.Items.Clear();
            _iconList.Items.Clear();

            var recentIcons = _allIcons.Where(i => _recentIcons.Contains(i.Path)).ToList();
            
            foreach (var iconPath in _recentIcons)
            {
                var icon = _allIcons.FirstOrDefault(i => i.Path == iconPath);
                if (icon != null)
                {
                    var item = new ListViewItem($"{icon.Name} ({icon.Source})") { Tag = icon };
                    _iconList.Items.Add(item);
                }
            }
        }

        private void SearchBox_TextChanged(object? sender, EventArgs e)
        {
            if (_tabControl.SelectedIndex >= 3) return; // Don't search in Favorites/Recent tabs

            RefreshIconList(_categoryList.SelectedItems.Count > 0 
                ? _categoryList.SelectedItems[0].Tag as string 
                : null);
        }

        private void IconList_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (_iconList.SelectedItems.Count == 0) return;

            var selected = _iconList.SelectedItems[0];
            if (selected.Tag is IconInfo icon)
            {
                SelectedIconPath = icon.Path;
                SelectedIconName = icon.Name;
                
                _iconNameLabel.Text = $"Icon: {icon.Name}";
                _iconPathLabel.Text = $"Path:\n{icon.Path}\n\nSource: {icon.Source}\nCategory: {icon.Category}";
                
                _previewPanel.Invalidate();
                _okButton.Enabled = true;
            }
        }

        private void SelectIcon(string iconPath)
        {
            var icon = _allIcons.FirstOrDefault(i => i.Path == iconPath);
            if (icon != null)
            {
                _sourceFilter.SelectedItem = icon.Source;
                // Select the appropriate tab
                _tabControl.SelectedIndex = icon.Source switch
                {
                    "UI" => 0,
                    "General" => 1,
                    "DataSources" => 2,
                    _ => 0
                };

                // Find and select the icon in the list
                var item = _iconList.Items.Cast<ListViewItem>()
                    .FirstOrDefault(i => i.Tag is IconInfo info && info.Path == iconPath);
                
                if (item != null)
                {
                    item.Selected = true;
                    item.EnsureVisible();
                }
            }
        }

        private void PreviewPanel_Paint(object? sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(Color.White);

            if (string.IsNullOrEmpty(SelectedIconPath))
            {
                // Draw placeholder
                using (var font = new Font("Segoe UI", 12))
                using (var brush = new SolidBrush(Color.Gray))
                {
                    var format = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    g.DrawString("Select an icon\nto preview", font, brush, _previewPanel.ClientRectangle, format);
                }
                return;
            }

            // Try to render the icon
            try
            {
                // For now, just show a large icon glyph and the name
                var rect = _previewPanel.ClientRectangle;
                rect.Inflate(-20, -20);

                // Draw a large icon placeholder
                using (var font = new Font("Segoe UI", 72))
                using (var brush = new SolidBrush(Color.FromArgb(100, 150, 200)))
                {
                    var format = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    g.DrawString("⭐", font, brush, rect, format);
                }

                // Draw icon name
                using (var font = new Font("Segoe UI", 10, FontStyle.Bold))
                using (var brush = new SolidBrush(Color.Black))
                {
                    var textRect = new Rectangle(rect.X, rect.Bottom - 30, rect.Width, 30);
                    var format = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    g.DrawString(SelectedIconName, font, brush, textRect, format);
                }
            }
            catch
            {
                // Draw error message
                using (var font = new Font("Segoe UI", 10))
                using (var brush = new SolidBrush(Color.Red))
                {
                    var format = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    g.DrawString("Preview not available", font, brush, _previewPanel.ClientRectangle, format);
                }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (DialogResult == DialogResult.OK && !string.IsNullOrEmpty(SelectedIconPath))
            {
                // Add to recent icons
                _recentIcons.Remove(SelectedIconPath);
                _recentIcons.Insert(0, SelectedIconPath);
                if (_recentIcons.Count > 20)
                {
                    _recentIcons.RemoveAt(20);
                }
            }

            base.OnFormClosing(e);
        }
    }
}

