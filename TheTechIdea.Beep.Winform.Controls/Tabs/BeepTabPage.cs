using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Tabs.Hosts;
using TheTechIdea.Beep.Winform.Controls.Tabs.Models;
using TheTechIdea.Beep.Winform.Controls.Layouts.Helpers;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(false)]
    [DesignerCategory("Code")]
    [DefaultProperty(nameof(Text))]
    public class BeepTabPage : BaseControl
    {
        protected override Size DefaultSize => BeepLayoutMetrics.TabPage;
        internal BeepTabItem TabMetadata { get; private set; }

        protected override bool IsContainerControl => true;
        protected override bool UseBaseMouseInputRouting => false;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams createParams = base.CreateParams;
                createParams.ExStyle &= ~0x20;
                return createParams;
            }
        }

        public BeepTabPage()
        {
            AccessibleRole = AccessibleRole.Pane;
            Margin = Padding.Empty;
            Padding = Padding.Empty;
            Dock = DockStyle.None;
            TabStop = false;
            CanBeHovered = false;
            CanBePressed = false;
            CanBeSelected = false;
            HitAreaEventOn = false;
            AutoDrawHitListComponents = false;
            ResetTabMetadata();
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            TabMetadata.Title = Text ?? string.Empty;
            NotifyTabsMetadataChanged();
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            TabMetadata.IsEnabled = Enabled;
            NotifyTabsMetadataChanged();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            PaintPageSurface(e.Graphics);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            PaintPageForeground(e.Graphics);
        }

        private void PaintPageSurface(Graphics graphics)
        {
            if (graphics == null || ClientRectangle.Width <= 0 || ClientRectangle.Height <= 0)
            {
                return;
            }

            Color fillColor = BackColor;
            if (fillColor == Color.Empty || fillColor.A == 0)
            {
                fillColor = Parent?.BackColor ?? SystemColors.Control;
            }

            using Brush brush = new SolidBrush(fillColor);
            graphics.FillRectangle(brush, ClientRectangle);
        }

        private void PaintPageForeground(Graphics graphics)
        {
            if (graphics == null || ClientRectangle.Width <= 0 || ClientRectangle.Height <= 0)
            {
                return;
            }

            if (Focused)
            {
                ControlPaint.DrawFocusRectangle(graphics, ClientRectangle);
            }
        }

        [Browsable(true)]
        [Category("Tabs")]
        [Description("Path or resource key for the tab header icon.")]
        [DefaultValue("")]
        public string TabIconPath
        {
            get => TabMetadata.IconPath;
            set
            {
                string normalizedValue = value ?? string.Empty;
                if (string.Equals(TabMetadata.IconPath, normalizedValue, StringComparison.Ordinal))
                {
                    return;
                }

                TabMetadata.IconPath = normalizedValue;
                NotifyTabsMetadataChanged();
            }
        }

        private bool ShouldSerializeTabIconPath() => !string.IsNullOrEmpty(TabIconPath);
        private void ResetTabIconPath() => TabIconPath = string.Empty;

        [Browsable(true)]
        [Category("Tabs")]
        [Description("Optional secondary label shown in the tab header.")]
        [DefaultValue("")]
        public string TabSubText
        {
            get => TabMetadata.SubText;
            set
            {
                string normalizedValue = value ?? string.Empty;
                if (string.Equals(TabMetadata.SubText, normalizedValue, StringComparison.Ordinal))
                {
                    return;
                }

                TabMetadata.SubText = normalizedValue;
                NotifyTabsMetadataChanged();
            }
        }

        private bool ShouldSerializeTabSubText() => !string.IsNullOrEmpty(TabSubText);
        private void ResetTabSubText() => TabSubText = string.Empty;

        [Browsable(true)]
        [Category("Tabs")]
        [Description("Text shown inside the tab badge.")]
        [DefaultValue("")]
        public string TabBadgeText
        {
            get => TabMetadata.BadgeText;
            set
            {
                string normalizedValue = value ?? string.Empty;
                if (string.Equals(TabMetadata.BadgeText, normalizedValue, StringComparison.Ordinal))
                {
                    return;
                }

                TabMetadata.BadgeText = normalizedValue;
                NotifyTabsMetadataChanged();
            }
        }

        private bool ShouldSerializeTabBadgeText() => !string.IsNullOrEmpty(TabBadgeText);
        private void ResetTabBadgeText() => TabBadgeText = string.Empty;

        [Browsable(true)]
        [Category("Tabs")]
        [Description("Visual kind of the tab badge.")]
        [DefaultValue(BeepTabBadgeKind.None)]
        public BeepTabBadgeKind TabBadgeKind
        {
            get => TabMetadata.BadgeKind;
            set
            {
                if (TabMetadata.BadgeKind == value)
                {
                    return;
                }

                TabMetadata.BadgeKind = value;
                NotifyTabsMetadataChanged();
            }
        }

        private bool ShouldSerializeTabBadgeKind() => TabBadgeKind != BeepTabBadgeKind.None;
        private void ResetTabBadgeKind() => TabBadgeKind = BeepTabBadgeKind.None;

        [Browsable(true)]
        [Category("Tabs")]
        [Description("Allows this page to show a close button in the tab header.")]
        [DefaultValue(true)]
        public bool TabCanClose
        {
            get => TabMetadata.CanClose;
            set
            {
                if (TabMetadata.CanClose == value)
                {
                    return;
                }

                TabMetadata.CanClose = value;
                NotifyTabsMetadataChanged();
            }
        }

        private bool ShouldSerializeTabCanClose() => !TabCanClose;
        private void ResetTabCanClose() => TabCanClose = true;

        [Browsable(true)]
        [Category("Tabs")]
        [Description("Allows this page to be selected from the tab header.")]
        [DefaultValue(true)]
        public bool TabCanSelect
        {
            get => TabMetadata.CanSelect;
            set
            {
                if (TabMetadata.CanSelect == value)
                {
                    return;
                }

                TabMetadata.CanSelect = value;
                NotifyTabsMetadataChanged();
            }
        }

        private bool ShouldSerializeTabCanSelect() => !TabCanSelect;
        private void ResetTabCanSelect() => TabCanSelect = true;

        [Browsable(true)]
        [Category("Tabs")]
        [Description("Allows this page to participate in tab reordering.")]
        [DefaultValue(true)]
        public bool TabCanReorder
        {
            get => TabMetadata.CanReorder;
            set
            {
                if (TabMetadata.CanReorder == value)
                {
                    return;
                }

                TabMetadata.CanReorder = value;
                NotifyTabsMetadataChanged();
            }
        }

        private bool ShouldSerializeTabCanReorder() => !TabCanReorder;
        private void ResetTabCanReorder() => TabCanReorder = true;

        [Browsable(true)]
        [Category("Tabs")]
        [Description("Marks the tab as busy so the header can show activity feedback.")]
        [DefaultValue(false)]
        public bool TabIsBusy
        {
            get => TabMetadata.IsBusy;
            set
            {
                if (TabMetadata.IsBusy == value)
                {
                    return;
                }

                TabMetadata.IsBusy = value;
                NotifyTabsMetadataChanged();
            }
        }

        private bool ShouldSerializeTabIsBusy() => TabIsBusy;
        private void ResetTabIsBusy() => TabIsBusy = false;

        [Browsable(true)]
        [Category("Tabs")]
        [Description("Overrides the host-level close-button visibility for this page. Null means inherit.")]
        [DefaultValue(null)]
        public bool? TabCloseVisible
        {
            get => TabMetadata.CloseVisible;
            set
            {
                if (TabMetadata.CloseVisible == value)
                {
                    return;
                }

                TabMetadata.CloseVisible = value;
                NotifyTabsMetadataChanged();
            }
        }

        private bool ShouldSerializeTabCloseVisible() => TabCloseVisible.HasValue;
        private void ResetTabCloseVisible() => TabCloseVisible = null;

        [Browsable(true)]
        [Category("Tabs")]
        [Description("Pins this page ahead of the normal workspace tab run.")]
        [DefaultValue(false)]
        public bool TabIsPinned
        {
            get => TabMetadata.WorkspaceState.IsPinned;
            set
            {
                if (TabMetadata.WorkspaceState.IsPinned == value)
                {
                    return;
                }

                TabMetadata.WorkspaceState.IsPinned = value;
                NotifyTabsMetadataChanged();
            }
        }

        private bool ShouldSerializeTabIsPinned() => TabIsPinned;
        private void ResetTabIsPinned() => TabIsPinned = false;

        [Browsable(true)]
        [Category("Tabs")]
        [Description("Marks this page as the current preview tab.")]
        [DefaultValue(false)]
        public bool TabIsPreview
        {
            get => TabMetadata.WorkspaceState.IsPreview;
            set
            {
                if (TabMetadata.WorkspaceState.IsPreview == value)
                {
                    return;
                }

                TabMetadata.WorkspaceState.IsPreview = value;
                NotifyTabsMetadataChanged();
            }
        }

        private bool ShouldSerializeTabIsPreview() => TabIsPreview;
        private void ResetTabIsPreview() => TabIsPreview = false;

        [Browsable(true)]
        [Category("Tabs")]
        [Description("Marks this page as dirty so the tab header can show unsaved state.")]
        [DefaultValue(false)]
        public bool TabIsDirty
        {
            get => TabMetadata.WorkspaceState.IsDirty;
            set
            {
                if (TabMetadata.WorkspaceState.IsDirty == value)
                {
                    return;
                }

                TabMetadata.WorkspaceState.IsDirty = value;
                NotifyTabsMetadataChanged();
            }
        }

        private bool ShouldSerializeTabIsDirty() => TabIsDirty;
        private void ResetTabIsDirty() => TabIsDirty = false;

        [Browsable(true)]
        [Category("Tabs")]
        [Description("Controls whether this preview tab should reuse the shared preview slot.")]
        [DefaultValue(true)]
        public bool TabReusePreviewSlot
        {
            get => TabMetadata.WorkspaceState.ReusePreviewSlot;
            set
            {
                if (TabMetadata.WorkspaceState.ReusePreviewSlot == value)
                {
                    return;
                }

                TabMetadata.WorkspaceState.ReusePreviewSlot = value;
                NotifyTabsMetadataChanged();
            }
        }

        private bool ShouldSerializeTabReusePreviewSlot() => !TabReusePreviewSlot;
        private void ResetTabReusePreviewSlot() => TabReusePreviewSlot = true;

        [Browsable(true)]
        [Category("Tabs")]
        [Description("Optional grouping hint for workspace/document integrations.")]
        [DefaultValue("")]
        public string TabGroupKey
        {
            get => TabMetadata.WorkspaceState.GroupKey;
            set
            {
                string normalizedValue = value ?? string.Empty;
                if (string.Equals(TabMetadata.WorkspaceState.GroupKey, normalizedValue, StringComparison.Ordinal))
                {
                    return;
                }

                TabMetadata.WorkspaceState.GroupKey = normalizedValue;
                NotifyTabsMetadataChanged();
            }
        }

        private bool ShouldSerializeTabGroupKey() => !string.IsNullOrEmpty(TabGroupKey);
        private void ResetTabGroupKey() => TabGroupKey = string.Empty;

        public void ResetSerializedTabMetadata()
        {
            ResetTabMetadata();
        }

        internal void ResetTabMetadata()
        {
            TabMetadata = new BeepTabItem
            {
                Name = Name ?? string.Empty,
                Title = Text ?? string.Empty,
                Content = this,
                IsEnabled = Enabled,
                IsVisible = true,
                CanClose = true,
                CanSelect = true,
                CanReorder = true
            };

            NotifyTabsMetadataChanged();
        }

        private void NotifyTabsMetadataChanged()
        {
            FindTabsOwner()?.NotifyHostedPageMetadataChanged(this);
        }

        private BeepTabs? FindTabsOwner()
        {
            if (Parent is BeepTabs tabs)
            {
                return tabs;
            }

            if (Parent is BeepTabContentHost contentHost && contentHost.Parent is BeepTabs hostedTabs)
            {
                return hostedTabs;
            }

            return null;
        }

    }
}