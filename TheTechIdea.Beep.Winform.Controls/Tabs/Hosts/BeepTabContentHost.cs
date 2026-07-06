using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Layouts.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Tabs.Hosts
{
    [ToolboxItem(false)]
    [Category("Beep Controls")]
    [DisplayName("Beep Tab Content Host")]
    [Description("Container host for premium tab content presentation.")]
    public class BeepTabContentHost : ContainerControl
    {
        protected override Size DefaultSize => BeepLayoutMetrics.TabHost;
        private Control? _hostedContent;
        private bool _applyingPageLayout;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Control? HostedContent => _hostedContent;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Theme { get; set; } = string.Empty;

        public BeepTabContentHost()
        {
            AccessibleRole = AccessibleRole.Pane;
            AccessibleName = "Beep Tab Content Host";
            Padding = Padding.Empty;
            TabStop = false;
            DoubleBuffered = true;
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);

            if (_applyingPageLayout || _hostedContent == null)
            {
                return;
            }

            SetSelectedPage(_hostedContent, ClientRectangle);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (ClientRectangle.Width <= 0 || ClientRectangle.Height <= 0)
            {
                return;
            }

            Color fillColor = BackColor;
            if (fillColor == Color.Empty || fillColor.A == 0)
            {
                fillColor = Parent?.BackColor ?? SystemColors.Control;
            }

            using Brush brush = new SolidBrush(fillColor);
            e.Graphics.FillRectangle(brush, ClientRectangle);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // Intentionally empty â€“ children paint themselves.
        }

        public void AddPage(Control page)
        {
            if (page == null)
            {
                throw new ArgumentNullException(nameof(page));
            }

            if (page.Parent == this && Controls.Contains(page))
            {
                return;
            }

            SuspendLayout();
            try
            {
                if (page.Parent != null && page.Parent != this)
                {
                    page.Parent.Controls.Remove(page);
                }

                page.Dock = DockStyle.None;
                if (!ReferenceEquals(_hostedContent, page))
                {
                    page.Visible = false;
                }

                if (!Controls.Contains(page))
                {
                    Controls.Add(page);
                }
            }
            finally
            {
                ResumeLayout(false);
            }
        }

        public void RemovePage(Control? page)
        {
            if (page == null)
            {
                return;
            }

            SuspendLayout();
            try
            {
                if (ReferenceEquals(_hostedContent, page))
                {
                    _hostedContent = null;
                }

                if (page.Parent == this)
                {
                    Controls.Remove(page);
                }

                page.Dock = DockStyle.None;
                page.Visible = false;
            }
            finally
            {
                ResumeLayout(false);
            }
        }

        public void ClearPages()
        {
            SuspendLayout();
            try
            {
                _hostedContent = null;
                for (int index = Controls.Count - 1; index >= 0; index--)
                {
                    Control child = Controls[index];
                    child.Dock = DockStyle.None;
                    child.Visible = false;
                    Controls.RemoveAt(index);
                }
            }
            finally
            {
                ResumeLayout(false);
            }
        }

        public bool ContainsPage(Control? page)
        {
            return page != null && page.Parent == this && Controls.Contains(page);
        }

        public void SetPageOrder(IReadOnlyList<Control> pages)
        {
            if (pages == null)
            {
                throw new ArgumentNullException(nameof(pages));
            }

            SuspendLayout();
            try
            {
                for (int index = 0; index < pages.Count; index++)
                {
                    Control page = pages[index];
                    if (!ContainsPage(page))
                    {
                        AddPage(page);
                    }

                    if (Controls.Contains(page) && Controls.GetChildIndex(page, false) != index)
                    {
                        Controls.SetChildIndex(page, index);
                    }
                }
            }
            finally
            {
                ResumeLayout(false);
            }
        }

        public void SetSelectedPage(Control? page)
        {
            SetSelectedPage(page, ClientRectangle);
        }

        public void SetSelectedPage(Control? page, Rectangle bounds)
        {
            if (_applyingPageLayout)
            {
                return;
            }

            if (page != null && !ContainsPage(page))
            {
                AddPage(page);
            }

            Rectangle pageBounds = new Rectangle(0, 0, Math.Max(0, bounds.Width), Math.Max(0, bounds.Height));
            bool showSelectedPage = page != null && pageBounds.Width > 0 && pageBounds.Height > 0;

            SuspendLayout();
            _applyingPageLayout = true;
            try
            {
                _hostedContent = page;
                Control? selectedControl = null;

                for (int index = 0; index < Controls.Count; index++)
                {
                    Control child = Controls[index];
                    child.Dock = DockStyle.None;

                    if (ReferenceEquals(child, page))
                    {
                        selectedControl = child;
                        if (child.Bounds != pageBounds)
                        {
                            child.Bounds = pageBounds;
                        }

                        if (child.Visible != showSelectedPage)
                        {
                            child.Visible = showSelectedPage;
                        }
                    }
                    else if (child.Visible)
                    {
                        child.Visible = false;
                    }
                }

                if (selectedControl != null && showSelectedPage && Controls.GetChildIndex(selectedControl, false) != 0)
                {
                    selectedControl.BringToFront();
                }
            }
            finally
            {
                try
                {
                    ResumeLayout(false);
                }
                finally
                {
                    _applyingPageLayout = false;
                }
            }
        }

        public void UpdatePageBounds(Rectangle bounds)
        {
            SetSelectedPage(_hostedContent, bounds);
        }

        public void SetContent(Control? content)
        {
            SetSelectedPage(content, ClientRectangle);
        }
    }
}
