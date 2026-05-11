using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Tabs.Helpers;
using TheTechIdea.Beep.Winform.Controls.Tabs.Painters;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepTabs
    {
        private bool _dontresize;

        public override Rectangle DisplayRectangle
        {
            get
            {
                int scaledHeaderHeight = GetScaledHeaderHeight();
                switch (_headerPosition)
                {
                    case TabHeaderPosition.Top:
                        return new Rectangle(0, scaledHeaderHeight, Math.Max(0, Width), Math.Max(0, ClientSize.Height - scaledHeaderHeight));
                    case TabHeaderPosition.Bottom:
                        return new Rectangle(0, 0, Math.Max(0, ClientSize.Width), Math.Max(0, ClientSize.Height - scaledHeaderHeight));
                    case TabHeaderPosition.Left:
                        return new Rectangle(scaledHeaderHeight, 0, Math.Max(0, ClientSize.Width - scaledHeaderHeight), Math.Max(0, ClientSize.Height));
                    case TabHeaderPosition.Right:
                        return new Rectangle(0, 0, Math.Max(0, ClientSize.Width - scaledHeaderHeight), Math.Max(0, ClientSize.Height));
                    default:
                        return base.DisplayRectangle;
                }
            }
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            if (e.Control is BeepTabPage page)
            {
                if (_suspendHostedSourceControlTreeSync)
                {
                    return;
                }

                // Guard: AddHostedSourcePage registers in _hostedPages BEFORE Controls.Add,
                // so the page may already be tracked.  Only register when it arrives from
                // an external source such as the WinForms designer.
                if (!_hostedPages.Contains(page))
                {
                    page.Dock = DockStyle.None;
                    page.Visible = false;
                    _hostedPages.Add(page);
                    GetOrCreateHostedTabMetadata(page);
                    // Auto-select first page when the control has no selection yet
                    if (_selectedHostedPage == null)
                        SetSelectedHostedPage(page, raiseSelectionChanged: false);
                }
                if (_currentTheme != null)
                {
                    page.BackColor = TabThemeHelpers.GetTabControlBackgroundColor(_currentTheme, true);
                    page.ForeColor = TabThemeHelpers.GetTabTextColor(_currentTheme, true);
                }

                if (UseContentHostPresentation() && page.Parent == this)
                {
                    EnsureHostedPageAttachedToContentHost(EnsureContentHost(), page);
                }

                UpdateLayout();
                UpdateItemSize();
            }
        }

        protected override void OnControlRemoved(ControlEventArgs e)
        {
            base.OnControlRemoved(e);
            if (e.Control is BeepTabPage page)
            {
                if (_suspendHostedSourceControlTreeSync)
                {
                    return;
                }

                // Only deregister if this removal originated externally (e.g. designer).
                // RemoveHostedSourcePage already removes from Controls, so _hostedPages
                // will already be clean when that path is used.
                if (_hostedPages.Contains(page))
                {
                    _hostedPages.Remove(page);
                    RemoveHostedPageFromMru(page);
                    if (ReferenceEquals(_selectedHostedPage, page))
                    {
                        BeepTabPage? next = _hostedPages.Count > 0 ? _hostedPages[0] : null;
                        SetSelectedHostedPage(next, raiseSelectionChanged: true);
                    }
                }
                UpdateLayout();
                UpdateItemSize();
            }
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);
            UpdateLayout();
        }

        private void UpdateLayout()
        {
            if (_dontresize)
                return;

            SyncHostedPageOrderFromOwnerControlTree();

            Rectangle rect = DisplayRectangle;
            ApplyHostedSourceContentBounds(rect);
        }

        private void UpdateItemSize()
        {
            if (_dontresize)
                return;

            int itemCount = GetHostedSourceItemCount();
            if (itemCount == 0)
            {
                return;
            }

            // Guard: CreateGraphics() on a control without a window handle forces
            // premature handle creation; defer until the handle exists.
            if (!IsHandleCreated)
            {
                return;
            }

            int headerHeight = GetScaledHeaderHeight();
            int maxWidth = GetScaledMinTabWidth();

            using (Graphics graphics = CreateGraphics())
            {
                Font fontToUse = TabFontHelpers.ResolveSafeFont(_textFont ?? Font, this);
                ITabPainter activePainter = _painter ?? GetPainter(_tabStyle);
                for (int i = 0; i < itemCount; i++)
                {
                    SizeF size = activePainter.MeasureTab(graphics, i, fontToUse);
                    maxWidth = Math.Max(maxWidth, (int)Math.Ceiling(size.Width));
                }
            }

            if (_showCloseButtons)
            {
                maxWidth += GetScaledCloseButtonSize() + GetScaledCloseButtonPadding() * 2;
            }

            maxWidth = Math.Min(maxWidth, GetScaledMaxTabWidth());

            int itemHeight = Math.Max(headerHeight, GetScaledMinTabHeight());
            itemHeight = Math.Min(itemHeight, GetScaledMaxTabHeight());

            Size newSize = new Size(maxWidth, itemHeight);
            if (ItemSize != newSize)
            {
                ItemSize = newSize;
            }
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
            Size baseSize = base.GetPreferredSize(proposedSize);
            switch (HeaderPosition)
            {
                case TabHeaderPosition.Top:
                case TabHeaderPosition.Bottom:
                    baseSize.Height += GetScaledHeaderHeight();
                    break;
                case TabHeaderPosition.Left:
                case TabHeaderPosition.Right:
                    baseSize.Width += GetScaledHeaderHeight();
                    break;
            }

            return baseSize;
        }

        private float[] CalculateTabSizes(Graphics graphics, bool vertical)
        {
            int itemCount = GetHostedSourceItemCount();
            float[] sizes = new float[itemCount];
            Font fontToUse = TabFontHelpers.ResolveSafeFont(_textFont ?? Font, this);
            ITabPainter activePainter = _painter ?? GetPainter(_tabStyle);
            for (int i = 0; i < itemCount; i++)
            {
                SizeF tabSize = activePainter.MeasureTab(graphics, i, fontToUse);
                sizes[i] = vertical
                    ? Math.Max(GetScaledMinTabHeight(), Math.Min(GetScaledMaxTabHeight(), tabSize.Height))
                    : Math.Max(GetScaledMinTabWidth(), Math.Min(GetScaledMaxTabWidth(), tabSize.Width));
            }

            return sizes;
        }

        internal float[] GetDesiredHeaderTabSizes(Graphics graphics)
        {
            return CalculateTabSizes(graphics, _headerPosition == TabHeaderPosition.Left || _headerPosition == TabHeaderPosition.Right);
        }

        internal float GetDesiredHeaderRunExtent(Graphics graphics)
        {
            float totalExtent = 0f;
            foreach (float size in GetDesiredHeaderTabSizes(graphics))
            {
                totalExtent += size;
            }

            return totalExtent;
        }

        internal List<RectangleF> GetCurrentHeaderTabRects()
        {
            if (!IsHandleCreated)
            {
                return new List<RectangleF>();
            }

            using (Graphics graphics = CreateGraphics())
            {
                return GetCurrentHeaderTabRects(graphics);
            }
        }

        internal List<RectangleF> GetCurrentHeaderTabRects(Graphics graphics)
        {
            int itemCount = GetHostedSourceItemCount();
            List<RectangleF> tabRects = new List<RectangleF>(itemCount);
            if (itemCount == 0)
            {
                _cachedHeaderTabRects = tabRects;
                return tabRects;
            }

            float[] tabSizes = GetDesiredHeaderTabSizes(graphics);
            int scaledHeaderHeight = GetScaledHeaderHeight();
            Rectangle headerBounds = BeepTabLayoutHelper.GetHeaderBounds(this);
            int reservedActionExtent = GetHeaderActionReservedExtent(headerBounds, GetHeaderActionSlots(graphics));

            switch (_headerPosition)
            {
                case TabHeaderPosition.Top:
                    {
                        float currentX = headerBounds.Left;
                        float availableRight = Math.Max(headerBounds.Left, headerBounds.Right - reservedActionExtent);
                        for (int i = 0; i < itemCount; i++)
                        {
                            float width = Math.Max(0f, Math.Min(tabSizes[i], availableRight - currentX));
                            tabRects.Add(new RectangleF(currentX, headerBounds.Top, width, scaledHeaderHeight));
                            currentX += width;
                        }
                        break;
                    }
                case TabHeaderPosition.Bottom:
                    {
                        float currentX = headerBounds.Left;
                        float availableRight = Math.Max(headerBounds.Left, headerBounds.Right - reservedActionExtent);
                        for (int i = 0; i < itemCount; i++)
                        {
                            float width = Math.Max(0f, Math.Min(tabSizes[i], availableRight - currentX));
                            tabRects.Add(new RectangleF(currentX, headerBounds.Top, width, scaledHeaderHeight));
                            currentX += width;
                        }
                        break;
                    }
                case TabHeaderPosition.Left:
                    {
                        float currentY = headerBounds.Top;
                        float availableBottom = Math.Max(headerBounds.Top, headerBounds.Bottom - reservedActionExtent);
                        for (int i = 0; i < itemCount; i++)
                        {
                            float height = Math.Max(0f, Math.Min(tabSizes[i], availableBottom - currentY));
                            tabRects.Add(new RectangleF(headerBounds.Left, currentY, headerBounds.Width, height));
                            currentY += height;
                        }
                        break;
                    }
                case TabHeaderPosition.Right:
                    {
                        float currentY = headerBounds.Top;
                        float availableBottom = Math.Max(headerBounds.Top, headerBounds.Bottom - reservedActionExtent);
                        for (int i = 0; i < itemCount; i++)
                        {
                            float height = Math.Max(0f, Math.Min(tabSizes[i], availableBottom - currentY));
                            tabRects.Add(new RectangleF(headerBounds.Left, currentY, headerBounds.Width, height));
                            currentY += height;
                        }
                        break;
                    }
            }

            _cachedHeaderTabRects = tabRects;
            return tabRects;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            _cachedHeaderTabRects.Clear();
            RefreshHeaderLayoutState(updateItemSize: false);
        }

        private void RefreshHeaderLayoutState(bool updateItemSize = true)
        {
            UpdateLayout();
            if (updateItemSize)
            {
                UpdateItemSize();
            }

            StartUnderlineAnimation();
            Invalidate();
        }

        public void SuspendFormLayout()
        {
            SuspendLayout();
            _dontresize = true;
            IReadOnlyList<Control> children = GetHostedSourceSelectedContentChildren();
            if (children.Count == 0)
            {
                return;
            }

            foreach (Control control in children)
            {
                control.SuspendLayout();
                if (control is IBeepUIComponent component)
                {
                    component.SuspendFormLayout();
                }
            }
        }

        public void ResumeFormLayout()
        {
            Control? selectedContent = GetHostedSourceSelectedContent();

            ResumeLayout(true);
            _dontresize = false;
            UpdateLayout();

            if (selectedContent == null)
            {
                Invalidate();
                return;
            }

            Size selectedContentSize = GetHostedSourceSelectedContentClientSize();
            foreach (Control control in GetHostedSourceSelectedContentChildren())
            {
                if (control is IBeepUIComponent component)
                {
                    component.ResumeFormLayout();
                }

                control.ResumeLayout(true);
                if (control.Dock == DockStyle.Fill)
                {
                    control.Size = selectedContentSize;
                }
                else if (control.Anchor != AnchorStyles.None)
                {
                    control.Refresh();
                }
            }

            InvalidateHostedSourceSelectedContent();
            Invalidate();
        }
    }
}