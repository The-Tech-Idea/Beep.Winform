using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    /// <summary>
    /// Dialog for selecting a BeepControlStyle from all 56+ available styles
    /// Provides categorization, search, and live preview
    /// </summary>
    public class StyleSelectorDialog : Form
    {
        private ListView _categoryList;
        private ListView _styleList;
        private Panel _previewPanel;
        private TextBox _searchBox;
        private Button _okButton;
        private Button _cancelButton;
        private Label _descriptionLabel;
        
        public BeepControlStyle SelectedStyle { get; private set; }

        // Style categories for organization
        private readonly Dictionary<string, List<BeepControlStyle>> _categories = new()
        {
            ["Modern Web"] = new() { BeepControlStyle.Material3, BeepControlStyle.MaterialYou, BeepControlStyle.iOS15, 
                BeepControlStyle.AntDesign, BeepControlStyle.ChakraUI, BeepControlStyle.TailwindCard, 
                BeepControlStyle.NotionMinimal, BeepControlStyle.VercelClean, BeepControlStyle.StripeDashboard },
            
            ["Microsoft"] = new() { BeepControlStyle.Fluent2, BeepControlStyle.Fluent, BeepControlStyle.Windows11Mica, 
                BeepControlStyle.Metro, BeepControlStyle.Metro2, BeepControlStyle.Office },
            
            ["Apple"] = new() { BeepControlStyle.Apple, BeepControlStyle.MacOSBigSur },
            
            ["Linux Desktop"] = new() { BeepControlStyle.Gnome, BeepControlStyle.Kde, BeepControlStyle.Cinnamon, 
                BeepControlStyle.Elementary, BeepControlStyle.Ubuntu, BeepControlStyle.ArcLinux },
            
            ["Minimal & Clean"] = new() { BeepControlStyle.Minimal, BeepControlStyle.Brutalist, BeepControlStyle.NeoBrutalist, 
                BeepControlStyle.NotionMinimal, BeepControlStyle.VercelClean, BeepControlStyle.Paper },
            
            ["Effects & Glass"] = new() { BeepControlStyle.Glassmorphism, BeepControlStyle.GlassAcrylic, 
                BeepControlStyle.Neumorphism, BeepControlStyle.GradientModern },
            
            ["Gaming & Neon"] = new() { BeepControlStyle.Gaming, BeepControlStyle.Neon, BeepControlStyle.Cyberpunk, 
                BeepControlStyle.DarkGlow, BeepControlStyle.Holographic },
            
            ["Theme Inspired"] = new() { BeepControlStyle.Dracula, BeepControlStyle.Nord, BeepControlStyle.Nordic, 
                BeepControlStyle.Tokyo, BeepControlStyle.OneDark, BeepControlStyle.GruvBox, 
                BeepControlStyle.Solarized },
            
            ["Fun & Creative"] = new() { BeepControlStyle.Cartoon, BeepControlStyle.ChatBubble, BeepControlStyle.Retro },
            
            ["Other"] = new() { BeepControlStyle.Bootstrap, BeepControlStyle.FigmaCard, BeepControlStyle.PillRail, 
                BeepControlStyle.Material, BeepControlStyle.WebFramework, BeepControlStyle.Effect, 
                BeepControlStyle.HighContrast, BeepControlStyle.Terminal, BeepControlStyle.DiscordStyle }
        };

        // Style descriptions
        private readonly Dictionary<BeepControlStyle, string> _descriptions = new()
        {
            [BeepControlStyle.Material3] = "Google's Material Design 3 (Material You) - Modern, colorful, adaptive",
            [BeepControlStyle.iOS15] = "Apple iOS 15 design language - Clean, minimal, rounded",
            [BeepControlStyle.Fluent2] = "Microsoft Fluent Design 2 - Modern, acrylic effects",
            [BeepControlStyle.AntDesign] = "Ant Design (Alibaba) - Enterprise-grade, professional",
            [BeepControlStyle.Minimal] = "Minimal design - Clean lines, no ornamentation",
            [BeepControlStyle.Brutalist] = "Brutalist design - Raw, bold, unapologetic",
            [BeepControlStyle.Neumorphism] = "Neumorphic (soft UI) - Subtle 3D depth, soft shadows",
            [BeepControlStyle.Glassmorphism] = "Glassmorphism - Frosted glass effect, transparency",
            [BeepControlStyle.Nord] = "Nord theme - Arctic, clean, blue-tinted palette",
            [BeepControlStyle.Dracula] = "Dracula theme - Dark, vampiric, purple accents",
            [BeepControlStyle.Tokyo] = "Tokyo Night theme - Dark, neon accents, cyberpunk",
            [BeepControlStyle.Cyberpunk] = "Cyberpunk aesthetic - Neon, futuristic, high-tech",
            [BeepControlStyle.Gaming] = "Gaming UI - Bold, animated, high-energy",
            [BeepControlStyle.Neon] = "Neon style - Bright glows, vibrant colors",
            [BeepControlStyle.Cartoon] = "Cartoon style - Playful, bold outlines, fun"
        };

        public StyleSelectorDialog(BeepControlStyle currentStyle)
        {
            SelectedStyle = currentStyle;
            InitializeComponents();
            LoadCategories();
            SelectCurrentStyle(currentStyle);
        }

        private void InitializeComponents()
        {
            Text = "Select Control Style";
            Size = new Size(800, 600);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            // Search box
            _searchBox = new TextBox
            {
                Location = new Point(12, 12),
                Size = new Size(776, 24),
                PlaceholderText = "Search styles..."
            };
            _searchBox.TextChanged += SearchBox_TextChanged;

            // Category list
            _categoryList = new ListView
            {
                Location = new Point(12, 44),
                Size = new Size(180, 440),
                View = View.List,
                FullRowSelect = true,
                HideSelection = false
            };
            _categoryList.SelectedIndexChanged += CategoryList_SelectedIndexChanged;

            // Style list
            _styleList = new ListView
            {
                Location = new Point(200, 44),
                Size = new Size(250, 440),
                View = View.Details,
                FullRowSelect = true,
                HideSelection = false,
                MultiSelect = false
            };
            _styleList.Columns.Add("Style", 230);
            _styleList.SelectedIndexChanged += StyleList_SelectedIndexChanged;
            _styleList.DoubleClick += (s, e) => DialogResult = DialogResult.OK;

            // Preview panel
            _previewPanel = new Panel
            {
                Location = new Point(458, 44),
                Size = new Size(330, 300),
                BorderStyle = BorderStyle.FixedSingle
            };
            _previewPanel.Paint += PreviewPanel_Paint;

            // Description label
            _descriptionLabel = new Label
            {
                Location = new Point(458, 352),
                Size = new Size(330, 132),
                BorderStyle = BorderStyle.FixedSingle,
                TextAlign = ContentAlignment.TopLeft,
                Padding = new Padding(8),
                Text = "Select a style to see description"
            };

            // Buttons
            _okButton = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Location = new Point(632, 520),
                Size = new Size(75, 28)
            };

            _cancelButton = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Location = new Point(713, 520),
                Size = new Size(75, 28)
            };

            Controls.AddRange(new Control[] { _searchBox, _categoryList, _styleList, 
                _previewPanel, _descriptionLabel, _okButton, _cancelButton });

            AcceptButton = _okButton;
            CancelButton = _cancelButton;
        }

        private void LoadCategories()
        {
            _categoryList.Items.Clear();
            _categoryList.Items.Add(new ListViewItem("All Styles") { Tag = "All" });
            
            foreach (var category in _categories.Keys)
            {
                _categoryList.Items.Add(new ListViewItem(category) { Tag = category });
            }

            _categoryList.Items[0].Selected = true;
        }

        private void CategoryList_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (_categoryList.SelectedItems.Count == 0) return;

            var selected = _categoryList.SelectedItems[0];
            var category = selected.Tag as string;

            LoadStyles(category);
        }

        private void LoadStyles(string? category)
        {
            _styleList.Items.Clear();

            List<BeepControlStyle> styles;
            if (category == "All")
            {
                styles = Enum.GetValues(typeof(BeepControlStyle))
                    .Cast<BeepControlStyle>()
                    .Where(s => s != BeepControlStyle.None)
                    .ToList();
            }
            else if (category != null && _categories.ContainsKey(category))
            {
                styles = _categories[category];
            }
            else
            {
                return;
            }

            foreach (var style in styles.OrderBy(s => s.ToString()))
            {
                var item = new ListViewItem(style.ToString()) { Tag = style };
                _styleList.Items.Add(item);
            }
        }

        private void SelectCurrentStyle(BeepControlStyle style)
        {
            // Find category containing this style
            foreach (var kvp in _categories)
            {
                if (kvp.Value.Contains(style))
                {
                    var categoryItem = _categoryList.Items.Cast<ListViewItem>()
                        .FirstOrDefault(i => i.Tag as string == kvp.Key);
                    if (categoryItem != null)
                    {
                        categoryItem.Selected = true;
                        break;
                    }
                }
            }

            // Select the style in the list
            var styleItem = _styleList.Items.Cast<ListViewItem>()
                .FirstOrDefault(i => i.Tag is BeepControlStyle s && s == style);
            if (styleItem != null)
            {
                styleItem.Selected = true;
                styleItem.EnsureVisible();
            }
        }

        private void StyleList_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (_styleList.SelectedItems.Count == 0) return;

            var selected = _styleList.SelectedItems[0];
            if (selected.Tag is BeepControlStyle style)
            {
                SelectedStyle = style;
                UpdateDescription(style);
                _previewPanel.Invalidate();
            }
        }

        private void UpdateDescription(BeepControlStyle style)
        {
            if (_descriptions.TryGetValue(style, out var description))
            {
                _descriptionLabel.Text = $"{style}\n\n{description}";
            }
            else
            {
                _descriptionLabel.Text = $"{style}\n\n(No description available)";
            }
        }

        private void SearchBox_TextChanged(object? sender, EventArgs e)
        {
            string search = _searchBox.Text.ToLowerInvariant();
            
            if (string.IsNullOrWhiteSpace(search))
            {
                LoadCategories();
                return;
            }

            // Search all styles
            _styleList.Items.Clear();
            var allStyles = Enum.GetValues(typeof(BeepControlStyle))
                .Cast<BeepControlStyle>()
                .Where(s => s != BeepControlStyle.None && s.ToString().ToLowerInvariant().Contains(search))
                .OrderBy(s => s.ToString());

            foreach (var style in allStyles)
            {
                var item = new ListViewItem(style.ToString()) { Tag = style };
                _styleList.Items.Add(item);
            }
        }

        private void PreviewPanel_Paint(object? sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(Color.White);

            // Draw a simple preview of the selected style
            var rect = new Rectangle(20, 20, _previewPanel.Width - 40, _previewPanel.Height - 40);
            
            // Draw text showing the style name
            using (var font = new Font("Segoe UI", 12, FontStyle.Bold))
            using (var brush = new SolidBrush(Color.Black))
            {
                var format = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                g.DrawString($"{SelectedStyle}\n\nPreview", font, brush, rect, format);
            }

            // Draw a sample rounded rectangle with the style's radius
            int radius = StyleBorders.GetRadius(SelectedStyle);
            var sampleRect = new Rectangle(rect.X + 40, rect.Bottom - 60, rect.Width - 80, 40);
            
            // Simple rectangle drawing (avoiding GraphicsExtensions dependency)
            using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                sampleRect, Color.FromArgb(100, 150, 200), Color.FromArgb(70, 120, 170), 
                System.Drawing.Drawing2D.LinearGradientMode.Vertical))
            using (var pen = new Pen(Color.FromArgb(50, 100, 150), 2))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.FillRectangle(brush, sampleRect);
                g.DrawRectangle(pen, sampleRect);
            }

            // Draw radius indicator
            using (var font = new Font("Segoe UI", 8))
            using (var brush = new SolidBrush(Color.Gray))
            {
                g.DrawString($"Border Radius: {radius}px", font, brush, sampleRect.X, sampleRect.Bottom + 4);
            }
        }
    }
}

