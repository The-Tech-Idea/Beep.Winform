using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Images;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepTabs
    {
        private void InitializeControlDefaults()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            UpdateStyles();

            AllowDrop = true;
            TabStop = true;
            ItemSize = new Size(120, 30);
            Padding = new Padding(5);
        }

        private void InitializeRuntimeAssets()
        {
            InitializeStyleTransitionTimer();
            InitializeCloseIcon();
            InitializeHeaderRuntimeSurface();
        }

        private void InitializeCloseIcon()
        {
            closeIcon = new BeepImage
            {
                ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.close.svg",
                ScaleMode = ImageScaleMode.KeepAspectRatio,
                ApplyThemeOnImage = false,
                Size = new Size(GetScaledCloseButtonSize(), GetScaledCloseButtonSize())
            };
        }

        private void InitializeHeaderRuntimeSurface()
        {
            _headerHost.AttachTabsOwner(this);
        }

        private void InitializeAccessibilityMetadata()
        {
            AccessibleRole = AccessibleRole.PageTabList;
            AccessibleName = "Beep Tabs";
        }

        private void WireControlEvents()
        {
            HandleCreated += BeepTabs_HandleCreated;
            MouseClick += BeepTabs_MouseClick;
            MouseDoubleClick += BeepTabs_MouseDoubleClick;
            MouseLeave += BeepTabs_MouseLeave;
            SelectedIndexChanged += BeepTabs_SelectedIndexChanged;
            MouseDown += BeepTabs_MouseDown;
            MouseMove += BeepTabs_MouseMove;
            MouseUp += BeepTabs_MouseUp;
            DragEnter += BeepTabs_DragEnter;
            DragOver += BeepTabs_DragOver;
            DragDrop += BeepTabs_DragDrop;
            DragLeave += BeepTabs_DragLeave;
        }

        private void BeepTabs_HandleCreated(object sender, EventArgs e)
        {
            closeIcon.Size = new Size(GetScaledCloseButtonSize(), GetScaledCloseButtonSize());

            // At runtime, project any designer-authored pages that are still parented to
            // BeepTabs.Controls into BeepTabContentHost so the runtime content-host
            // presentation path is fully active before the first layout/paint cycle.
            // OnControlAdded handles projection per-page as InitializeComponent runs,
            // but this sweep seals the state and covers unusual initialisation orders.
            if (!IsInHostedContentDesignMode())
            {
                ProjectDesignerPagesToContentHost();
            }

            UpdateLayout();
            UpdateItemSize();
            _headerHost.SyncSnapshot();
            InitializeUnderlineTimer();
            StartUnderlineAnimation();
        }
    }
}