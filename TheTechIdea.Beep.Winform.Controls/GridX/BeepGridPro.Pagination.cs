using System;
using System.ComponentModel;

namespace TheTechIdea.Beep.Winform.Controls.GridX
{
    public partial class BeepGridPro
    {
        private int _pageSize;

        /// <summary>
        /// Rows per page for navigator page indicators. 0 means "use the
        /// current viewport capacity". This is navigation paging over the
        /// current view; it does not replace the data source's paging.
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(0)]
        [Description("Rows per page for navigator page indicators. 0 = current viewport capacity.")]
        public int PageSize
        {
            get => _pageSize;
            set
            {
                int next = Math.Max(0, value);
                if (_pageSize == next) return;
                _pageSize = next;
                SafeInvalidate(Layout.NavigatorRect.IsEmpty ? Rectangle.Empty : Layout.NavigatorRect);
                SafeInvalidate();
                if (Data.Rows.Count > 0)
                    GoToFirstPage();
            }
        }

        [Browsable(false)]
        public int CurrentPage => Render.GetCurrentPage(this);

        [Browsable(false)]
        public int TotalPages => Render.GetTotalPages(this);

        public void GoToPage(int pageNumber) => Render.GoToPage(this, pageNumber);
        public void GoToFirstPage() => Render.GoToFirstPage(this);
        public void GoToPreviousPage() => Render.GoToPreviousPage(this);
        public void GoToNextPage() => Render.GoToNextPage(this);
        public void GoToLastPage() => Render.GoToLastPage(this);

        /// <summary>
        /// Shows the page-size menu (right-click the navigator status text).
        /// </summary>
        public void ShowPageSizeMenu(System.Drawing.Point screenLocation)
        {
            var menu = new System.Windows.Forms.ContextMenuStrip();
            foreach (int size in new[] { 0, 10, 25, 50, 100 })
            {
                var item = new System.Windows.Forms.ToolStripMenuItem(
                    size == 0 ? "Auto (viewport)" : $"{size} rows")
                {
                    Checked = PageSize == size
                };
                int chosen = size;
                item.Click += (_, __) =>
                {
                    PageSize = chosen;
                    if (Data.Rows.Count > 0) GoToFirstPage();
                    SafeInvalidate();
                };
                menu.Items.Add(item);
            }

            menu.Items.Add(new System.Windows.Forms.ToolStripSeparator());
            var goTo = new System.Windows.Forms.ToolStripMenuItem("Go to page...");
            goTo.Click += (_, __) => ShowGoToPageDialog();
            menu.Items.Add(goTo);

            menu.Show(screenLocation);
        }

        private void ShowGoToPageDialog()
        {
            using var form = new System.Windows.Forms.Form
            {
                Text = "Go to page",
                Width = 320,
                Height = 150,
                FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                ShowInTaskbar = false,
                StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
            };

            var layout = new System.Windows.Forms.TableLayoutPanel
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2,
                Padding = new System.Windows.Forms.Padding(12)
            };
            layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 55));
            layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 45));
            layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28));
            layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38));

            var label = new BeepLabel
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
                Text = $"Page (1–{TotalPages}):",
                Theme = Theme
            };
            var input = new BeepTextBox
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
                Text = CurrentPage.ToString(),
                Theme = Theme
            };
            var buttons = new System.Windows.Forms.FlowLayoutPanel
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
                FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft
            };
            var go = new BeepButton { Text = "Go", Width = 70, Theme = Theme };
            var cancel = new BeepButton { Text = "Cancel", Width = 80, Theme = Theme };

            go.Click += (_, __) =>
            {
                if (int.TryParse(input.Text, out int page))
                {
                    form.Tag = page;
                    form.DialogResult = System.Windows.Forms.DialogResult.OK;
                    form.Close();
                }
                else
                {
                    input.Text = CurrentPage.ToString();
                }
            };
            cancel.Click += (_, __) =>
            {
                form.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                form.Close();
            };

            buttons.Controls.Add(cancel);
            buttons.Controls.Add(go);
            layout.Controls.Add(label, 0, 0);
            layout.SetColumnSpan(label, 2);
            layout.Controls.Add(input, 0, 1);
            layout.Controls.Add(buttons, 1, 1);
            form.Controls.Add(layout);
            form.AcceptButton = go;
            form.CancelButton = cancel;

            var owner = FindForm();
            var result = owner != null ? form.ShowDialog(owner) : form.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK && form.Tag is int pageNumber)
                GoToPage(pageNumber);
        }
    }
}
