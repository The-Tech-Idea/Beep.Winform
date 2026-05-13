using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.DocumentHost;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    internal sealed class DocumentHostLayoutAssistantDialog : Form
    {
        private readonly BindingList<DocumentLayoutAssistantItem> _documents = new();
        private readonly ListBox _presetList;
        private readonly Panel _previewPanel;
        private readonly Label _descriptionLabel;
        private readonly Label _minimumCountLabel;
        private readonly NumericUpDown _documentCount;
        private readonly DataGridView _documentGrid;

        public LayoutPreset SelectedPreset
            => _presetList.SelectedItem is LayoutPreset preset ? preset : LayoutPreset.Single;

        public IReadOnlyList<DocumentLayoutAssistantItem> Documents
            => _documents.Select(item => item.Clone()).ToList();

        public DocumentHostLayoutAssistantDialog(IEnumerable<DocumentDescriptor> existingDocuments)
        {
            Text = "DocumentHost Layout Assistant";
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(760, 520);
            Size = new Size(920, 620);
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = Color.White;
            Font = new Font("Segoe UI", 9f);

            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 78,
                Padding = new Padding(18, 18, 18, 10),
                BackColor = Color.White
            };
            var titleLabel = new Label
            {
                Dock = DockStyle.Top,
                Height = 28,
                Font = new Font("Segoe UI Semibold", 13f, FontStyle.Bold),
                Text = "Layout Assistant"
            };
            var subtitleLabel = new Label
            {
                Dock = DockStyle.Fill,
                ForeColor = Color.FromArgb(92, 98, 112),
                Text = "Choose a docking pattern and the document surfaces you want the designer to create and persist."
            };
            headerPanel.Controls.Add(subtitleLabel);
            headerPanel.Controls.Add(titleLabel);

            var body = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                Padding = new Padding(18, 0, 18, 0)
            };
            body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42f));
            body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58f));

            var layoutGroup = new GroupBox
            {
                Dock = DockStyle.Fill,
                Text = "Layout Pattern",
                Padding = new Padding(12)
            };
            var layoutPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 3
            };
            layoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 45f));
            layoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 40f));
            layoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 15f));

            _presetList = new ListBox
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                IntegralHeight = false
            };
            _presetList.Items.AddRange(GetPresetOrder().Cast<object>().ToArray());
            _presetList.Format += (s, e) => e.Value = GetPresetDisplayName((LayoutPreset)e.ListItem!);
            _presetList.SelectedIndexChanged += (s, e) =>
            {
                UpdatePresetDetails();
            };

            _previewPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 12, 0, 8),
                BackColor = Color.FromArgb(248, 249, 252)
            };
            _previewPanel.Paint += PreviewPanel_Paint;

            _descriptionLabel = new Label
            {
                Dock = DockStyle.Fill,
                ForeColor = Color.FromArgb(92, 98, 112),
                TextAlign = ContentAlignment.TopLeft
            };

            layoutPanel.Controls.Add(_presetList, 0, 0);
            layoutPanel.Controls.Add(_previewPanel, 0, 1);
            layoutPanel.Controls.Add(_descriptionLabel, 0, 2);
            layoutGroup.Controls.Add(layoutPanel);

            var documentsGroup = new GroupBox
            {
                Dock = DockStyle.Fill,
                Text = "Document Surfaces",
                Padding = new Padding(12)
            };
            var documentsPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 3
            };
            documentsPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            documentsPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            documentsPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var controlsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Margin = new Padding(0, 0, 0, 10)
            };
            controlsPanel.Controls.Add(new Label
            {
                AutoSize = true,
                Text = "Document count:",
                Padding = new Padding(0, 6, 0, 0)
            });
            _documentCount = new NumericUpDown
            {
                Minimum = 1,
                Maximum = 12,
                Value = 1,
                Width = 72,
                Margin = new Padding(8, 0, 16, 0)
            };
            _documentCount.ValueChanged += (s, e) => SyncDocumentCount((int)_documentCount.Value);
            controlsPanel.Controls.Add(_documentCount);

            _minimumCountLabel = new Label
            {
                AutoSize = true,
                ForeColor = Color.FromArgb(92, 98, 112),
                Padding = new Padding(0, 6, 0, 0)
            };
            controlsPanel.Controls.Add(_minimumCountLabel);

            _documentGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                DataSource = _documents
            };
            _documentGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(DocumentLayoutAssistantItem.Title),
                HeaderText = "Document Title",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 70f
            });
            _documentGrid.Columns.Add(new DataGridViewComboBoxColumn
            {
                DataPropertyName = nameof(DocumentLayoutAssistantItem.InitialContent),
                HeaderText = "Initial Content",
                DataSource = Enum.GetValues(typeof(DocumentInitialContent)),
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 30f
            });

            var documentButtons = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Margin = new Padding(0, 10, 0, 0)
            };
            var resetTitlesButton = new Button
            {
                AutoSize = true,
                Text = "Reset Titles"
            };
            resetTitlesButton.Click += (s, e) => ResetDocumentTitles();
            documentButtons.Controls.Add(resetTitlesButton);
            documentButtons.Controls.Add(new Label
            {
                AutoSize = true,
                ForeColor = Color.FromArgb(92, 98, 112),
                Text = "Existing document ids are preserved when possible.",
                Padding = new Padding(10, 7, 0, 0)
            });

            documentsPanel.Controls.Add(controlsPanel, 0, 0);
            documentsPanel.Controls.Add(_documentGrid, 0, 1);
            documentsPanel.Controls.Add(documentButtons, 0, 2);
            documentsGroup.Controls.Add(documentsPanel);

            body.Controls.Add(layoutGroup, 0, 0);
            body.Controls.Add(documentsGroup, 1, 0);

            var footerPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 54,
                Padding = new Padding(18, 10, 18, 14)
            };
            var okButton = new Button
            {
                Text = "Apply Layout",
                DialogResult = DialogResult.OK,
                Anchor = AnchorStyles.Right | AnchorStyles.Top,
                Width = 120,
                Height = 30,
                Left = 640,
                Top = 10
            };
            var cancelButton = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Anchor = AnchorStyles.Right | AnchorStyles.Top,
                Width = 100,
                Height = 30,
                Left = 770,
                Top = 10
            };
            footerPanel.Resize += (s, e) =>
            {
                cancelButton.Left = footerPanel.Width - cancelButton.Width;
                okButton.Left = cancelButton.Left - okButton.Width - 10;
            };
            footerPanel.Controls.Add(okButton);
            footerPanel.Controls.Add(cancelButton);

            AcceptButton = okButton;
            CancelButton = cancelButton;

            Controls.Add(body);
            Controls.Add(footerPanel);
            Controls.Add(headerPanel);

            SeedDocuments(existingDocuments);
            _presetList.SelectedItem = PickInitialPreset(_documents.Count);
            UpdatePresetDetails();
        }

        private void SeedDocuments(IEnumerable<DocumentDescriptor> existingDocuments)
        {
            var descriptors = existingDocuments
                .Where(descriptor => !string.IsNullOrWhiteSpace(descriptor.Id))
                .ToList();

            if (descriptors.Count == 0)
            {
                _documents.Add(new DocumentLayoutAssistantItem
                {
                    Title = "Document 1",
                    InitialContent = DocumentInitialContent.Empty
                });
            }
            else
            {
                foreach (DocumentDescriptor descriptor in descriptors)
                {
                    _documents.Add(new DocumentLayoutAssistantItem
                    {
                        Title = string.IsNullOrWhiteSpace(descriptor.Title) ? $"Document {_documents.Count + 1}" : descriptor.Title,
                        InitialContent = descriptor.InitialContent
                    });
                }
            }

            _documentCount.Value = Math.Max(1, _documents.Count);
        }

        private void SyncDocumentCount(int requestedCount)
        {
            int requiredCount = Math.Max(GetMinimumDocumentCount(SelectedPreset), requestedCount);
            if ((int)_documentCount.Value != requiredCount)
            {
                _documentCount.Value = requiredCount;
                return;
            }

            while (_documents.Count < requiredCount)
            {
                _documents.Add(new DocumentLayoutAssistantItem
                {
                    Title = $"Document {_documents.Count + 1}",
                    InitialContent = DocumentInitialContent.Empty
                });
            }

            while (_documents.Count > requiredCount)
            {
                _documents.RemoveAt(_documents.Count - 1);
            }
        }

        private void ResetDocumentTitles()
        {
            for (int index = 0; index < _documents.Count; index++)
            {
                _documents[index].Title = $"Document {index + 1}";
            }

            _documentGrid.Refresh();
        }

        private void UpdatePresetDetails()
        {
            int minimumCount = GetMinimumDocumentCount(SelectedPreset);
            _descriptionLabel.Text = GetPresetDescription(SelectedPreset);
            _minimumCountLabel.Text = $"Minimum for this layout: {minimumCount}";
            if (_documentCount.Value < minimumCount)
            {
                _documentCount.Value = minimumCount;
            }

            SyncDocumentCount((int)_documentCount.Value);
            _previewPanel.Invalidate();
        }

        private void PreviewPanel_Paint(object? sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.Clear(_previewPanel.BackColor);

            var bounds = new Rectangle(18, 18, Math.Max(80, _previewPanel.Width - 36), Math.Max(80, _previewPanel.Height - 36));
            using var frameBrush = new SolidBrush(Color.White);
            using var shadowBrush = new SolidBrush(Color.FromArgb(18, 32, 44, 64));
            e.Graphics.FillRoundedRect(shadowBrush, new Rectangle(bounds.X + 6, bounds.Y + 6, bounds.Width, bounds.Height), 10);
            e.Graphics.FillRoundedRect(frameBrush, bounds, 10);
            DrawLayoutSketch(e.Graphics, new Rectangle(bounds.X + 14, bounds.Y + 14, bounds.Width - 28, bounds.Height - 28), SelectedPreset);
        }

        private static LayoutPreset PickInitialPreset(int documentCount)
            => documentCount switch
            {
                <= 1 => LayoutPreset.Single,
                2 => LayoutPreset.SideBySide,
                3 => LayoutPreset.ThreeWay,
                4 => LayoutPreset.FourUp,
                _ => LayoutPreset.FiveWay
            };

        private static IReadOnlyList<LayoutPreset> GetPresetOrder()
            => new[]
            {
                LayoutPreset.Single,
                LayoutPreset.SideBySide,
                LayoutPreset.Stacked,
                LayoutPreset.ThreeWay,
                LayoutPreset.ThreeWayNested,
                LayoutPreset.FourUp,
                LayoutPreset.ThreeColumn,
                LayoutPreset.FiveWay
            };

        private static string GetPresetDisplayName(LayoutPreset preset)
            => preset switch
            {
                LayoutPreset.SideBySide => "Side-by-Side",
                LayoutPreset.ThreeWay => "Three-Way",
                LayoutPreset.ThreeWayNested => "Three-Way Nested",
                LayoutPreset.FourUp => "Four-Up",
                LayoutPreset.ThreeColumn => "Three Column",
                LayoutPreset.FiveWay => "Five-Way",
                _ => preset.ToString()
            };

        private static string GetPresetDescription(LayoutPreset preset)
            => preset switch
            {
                LayoutPreset.Single => "One document group fills the host. Good for a standard tabbed MDI start.",
                LayoutPreset.SideBySide => "Two groups split left and right. Useful for compare and review workflows.",
                LayoutPreset.Stacked => "Two groups split top and bottom. Useful for editor plus output or preview.",
                LayoutPreset.ThreeWay => "Three groups in an L-pattern. The left side stays dominant while the right is stacked.",
                LayoutPreset.ThreeWayNested => "Three groups with a main left pane and nested top/bottom panes on the right.",
                LayoutPreset.FourUp => "A 2 x 2 grid of groups for dashboards, multi-file inspection, or data tools.",
                LayoutPreset.ThreeColumn => "Three equal columns across the host for explorer, editor, and preview shells.",
                LayoutPreset.FiveWay => "A dominant left pane plus a 2 x 2 right grid for commercial IDE-style layouts.",
                _ => string.Empty
            };

        private static int GetMinimumDocumentCount(LayoutPreset preset)
            => preset switch
            {
                LayoutPreset.Single => 1,
                LayoutPreset.SideBySide or LayoutPreset.Stacked => 2,
                LayoutPreset.ThreeWay or LayoutPreset.ThreeWayNested or LayoutPreset.ThreeColumn => 3,
                LayoutPreset.FourUp => 4,
                LayoutPreset.FiveWay => 5,
                _ => 1
            };

        private static void DrawLayoutSketch(Graphics g, Rectangle bounds, LayoutPreset preset)
        {
            using var framePen = new Pen(Color.FromArgb(104, 122, 146), 1.2f);
            using var fillBrush = new SolidBrush(Color.FromArgb(214, 228, 248));

            switch (preset)
            {
                case LayoutPreset.Single:
                    g.FillRectangle(fillBrush, bounds);
                    g.DrawRectangle(framePen, bounds);
                    break;

                case LayoutPreset.SideBySide:
                {
                    int half = bounds.Width / 2 - 1;
                    var left = new Rectangle(bounds.X, bounds.Y, half, bounds.Height);
                    var right = new Rectangle(bounds.X + half + 2, bounds.Y, bounds.Width - half - 2, bounds.Height);
                    g.FillRectangle(fillBrush, left);
                    g.DrawRectangle(framePen, left);
                    g.FillRectangle(fillBrush, right);
                    g.DrawRectangle(framePen, right);
                    break;
                }

                case LayoutPreset.Stacked:
                {
                    int half = bounds.Height / 2 - 1;
                    var top = new Rectangle(bounds.X, bounds.Y, bounds.Width, half);
                    var bottom = new Rectangle(bounds.X, bounds.Y + half + 2, bounds.Width, bounds.Height - half - 2);
                    g.FillRectangle(fillBrush, top);
                    g.DrawRectangle(framePen, top);
                    g.FillRectangle(fillBrush, bottom);
                    g.DrawRectangle(framePen, bottom);
                    break;
                }

                case LayoutPreset.ThreeWay:
                case LayoutPreset.ThreeWayNested:
                {
                    int leftWidth = bounds.Width * 6 / 10;
                    int rightWidth = bounds.Width - leftWidth - 2;
                    int topHeight = bounds.Height / 2 - 1;
                    var left = new Rectangle(bounds.X, bounds.Y, leftWidth, bounds.Height);
                    var rightTop = new Rectangle(bounds.X + leftWidth + 2, bounds.Y, rightWidth, topHeight);
                    var rightBottom = new Rectangle(bounds.X + leftWidth + 2, bounds.Y + topHeight + 2, rightWidth, bounds.Height - topHeight - 2);
                    g.FillRectangle(fillBrush, left);
                    g.DrawRectangle(framePen, left);
                    g.FillRectangle(fillBrush, rightTop);
                    g.DrawRectangle(framePen, rightTop);
                    g.FillRectangle(fillBrush, rightBottom);
                    g.DrawRectangle(framePen, rightBottom);
                    break;
                }

                case LayoutPreset.FourUp:
                {
                    int halfWidth = bounds.Width / 2 - 1;
                    int halfHeight = bounds.Height / 2 - 1;
                    Rectangle[] rectangles =
                    {
                        new Rectangle(bounds.X, bounds.Y, halfWidth, halfHeight),
                        new Rectangle(bounds.X + halfWidth + 2, bounds.Y, bounds.Width - halfWidth - 2, halfHeight),
                        new Rectangle(bounds.X, bounds.Y + halfHeight + 2, halfWidth, bounds.Height - halfHeight - 2),
                        new Rectangle(bounds.X + halfWidth + 2, bounds.Y + halfHeight + 2, bounds.Width - halfWidth - 2, bounds.Height - halfHeight - 2)
                    };
                    foreach (Rectangle rectangle in rectangles)
                    {
                        g.FillRectangle(fillBrush, rectangle);
                        g.DrawRectangle(framePen, rectangle);
                    }
                    break;
                }

                case LayoutPreset.ThreeColumn:
                {
                    int third = bounds.Width / 3 - 1;
                    var first = new Rectangle(bounds.X, bounds.Y, third, bounds.Height);
                    var second = new Rectangle(bounds.X + third + 2, bounds.Y, third, bounds.Height);
                    var thirdRect = new Rectangle(bounds.X + third * 2 + 4, bounds.Y, bounds.Width - third * 2 - 4, bounds.Height);
                    g.FillRectangle(fillBrush, first);
                    g.DrawRectangle(framePen, first);
                    g.FillRectangle(fillBrush, second);
                    g.DrawRectangle(framePen, second);
                    g.FillRectangle(fillBrush, thirdRect);
                    g.DrawRectangle(framePen, thirdRect);
                    break;
                }

                case LayoutPreset.FiveWay:
                {
                    int leftWidth = bounds.Width * 3 / 10;
                    int rightWidth = bounds.Width - leftWidth - 2;
                    int rightHalfWidth = rightWidth / 2 - 1;
                    int halfHeight = bounds.Height / 2 - 1;
                    var left = new Rectangle(bounds.X, bounds.Y, leftWidth, bounds.Height);
                    var topLeft = new Rectangle(bounds.X + leftWidth + 2, bounds.Y, rightHalfWidth, halfHeight);
                    var topRight = new Rectangle(bounds.X + leftWidth + rightHalfWidth + 4, bounds.Y, bounds.Width - leftWidth - rightHalfWidth - 4, halfHeight);
                    var bottomLeft = new Rectangle(bounds.X + leftWidth + 2, bounds.Y + halfHeight + 2, rightHalfWidth, bounds.Height - halfHeight - 2);
                    var bottomRight = new Rectangle(bounds.X + leftWidth + rightHalfWidth + 4, bounds.Y + halfHeight + 2, bounds.Width - leftWidth - rightHalfWidth - 4, bounds.Height - halfHeight - 2);
                    Rectangle[] rectangles = { left, topLeft, topRight, bottomLeft, bottomRight };
                    foreach (Rectangle rectangle in rectangles)
                    {
                        g.FillRectangle(fillBrush, rectangle);
                        g.DrawRectangle(framePen, rectangle);
                    }
                    break;
                }
            }
        }
    }

    internal sealed class DocumentLayoutAssistantItem
    {
        public string Title { get; set; } = string.Empty;
        public DocumentInitialContent InitialContent { get; set; } = DocumentInitialContent.Empty;

        public DocumentLayoutAssistantItem Clone()
            => new DocumentLayoutAssistantItem
            {
                Title = Title,
                InitialContent = InitialContent
            };
    }
}