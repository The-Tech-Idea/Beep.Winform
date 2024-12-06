using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Winform.Controls
{// We assume BeepButton, BeepTheme, and related classes are in your namespace.
 // using TheTechIdea.Beep.Winform.Controls; // Adjust namespace as needed

    // A TabControl that hides the default tab headers
    public class TabControlWithoutTabs : TabControl
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct RECT { public int Left; public int Top; public int Right; public int Bottom; }
        // Custom event that notifies when TabPages change
        public event EventHandler TabPagesChanged;
        protected override void WndProc(ref Message m)
        {
            // TCM_ADJUSTRECT message = 0x1328
            if (m.Msg == 0x1328 && !DesignMode)
            {
                // Return zero rect to hide tabs
                m.Result = IntPtr.Zero;
                return;
            }
            base.WndProc(ref m);
        }
        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            // A tab (TabPage) was added
            TabPagesChanged?.Invoke(this, EventArgs.Empty);
        }

        protected override void OnControlRemoved(ControlEventArgs e)
        {
            base.OnControlRemoved(e);
            // A tab (TabPage) was removed
            TabPagesChanged?.Invoke(this, EventArgs.Empty);
        }
    }
    [DefaultProperty("TabPages")]
    public class BeepTabs : BeepControl
    {
        private FlowLayoutPanel _headerPanel;
        private TabControlWithoutTabs _tabControl;
       
        private int _headerButtonSize = 30;
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int HeaderButtonSize
        {
            get => _headerButtonSize;
            set
            {
                _headerButtonSize = value;
                RefreshHeaders();
            }
        }


        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public TabControl.TabPageCollection TabPages => _tabControl.TabPages;

        public TabPage SelectedTab
        {
            get => _tabControl.SelectedTab;
            set => _tabControl.SelectedTab = value;
        }

        public int SelectedIndex
        {
            get => _tabControl.SelectedIndex;
            set => _tabControl.SelectedIndex = value;
        }

        public BeepTabs()
        {
            _headerPanel = new FlowLayoutPanel
            {
                Top= DrawingRect.Top,
                Left = DrawingRect.Left,
                Width = DrawingRect.Width,
                Height = _headerButtonSize,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                BackColor = SystemColors.Control
            };

            _tabControl = new TabControlWithoutTabs
            {
                //Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                //Bounds = new Rectangle(DrawingRect.Left, _headerPanel.Height+DrawingRect.Top, DrawingRect.Width, DrawingRect.Height - _headerPanel.Height)

            };
            // Hook into the TabPagesChanged event
           _tabControl.TabPagesChanged += (s, e) => RefreshHeaders();

            Controls.Add(_tabControl);
            Controls.Add(_headerPanel);

            this.MinimumSize = new Size(200, 100);
           
        }
        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);

            // Ensure DrawingRect has been updated by BeepControl
            // DrawingRect is accessible because BeepTabs inherits from BeepControl
            var rect = this.DrawingRect;
            if (rect == Rectangle.Empty) return;

            // Position header panel at the top of DrawingRect
            _headerPanel.SetBounds(rect.X, rect.Y, rect.Width, _headerButtonSize);

            // Position tab control below the header panel, also within DrawingRect
            int tabControlHeight = rect.Height - _headerPanel.Height;
            if (tabControlHeight < 0) tabControlHeight = 0;
            _tabControl.SetBounds(rect.X, rect.Y + _headerPanel.Height, rect.Width, tabControlHeight);
        }
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            RefreshHeaders(); // Ensure headers are built at runtime
        }

        public void RefreshHeaders()
        {
            _headerPanel.Controls.Clear();

            for (int i = 0; i < _tabControl.TabPages.Count; i++)
            {
                var page = _tabControl.TabPages[i];
                var btn = CreateTabButton(page, i);
                _headerPanel.Controls.Add(btn);
            }

            // Highlight the selected tab button
            HighlightButtonAt(_tabControl.SelectedIndex);
        }

        private BeepButton CreateTabButton(TabPage page, int index)
        {
            // Create a BeepButton for each tab
            // measure the width of the text
            int textwidth = TextRenderer.MeasureText(page.Text, page.Font).Width;

            var btn = new BeepButton
            {
                Text = page.Text,
                Margin = new Padding(2),
                SavedGuidID = index.ToString(),
                HideText = false, // Show text on the tab button
                IsChild=false,
                ShowAllBorders=true,
                ShowShadow = false,
                IsRounded =false,
                IsRoundedAffectedByTheme=false,
                IsBorderAffectedByTheme = true,
                IsShadowAffectedByTheme = false,
                IsSelectedAuto = false,
                Size = new Size(textwidth, _headerButtonSize-4),
                ImagePath = "" // Set an image path if you want icons on your tabs
               
               
            };

            btn.Click += (s, e) => SelectTab(index);

            // Apply theme if requested

                btn.Theme = Theme;


            return btn;
        }

        public void SelectTab(int index)
        {
            if (index >= 0 && index < _tabControl.TabPages.Count)
            {
                _tabControl.SelectedIndex = index;
                HighlightButtonAt(index);
            }
        }

        private void HighlightButtonAt(int index)
        {
            for (int i = 0; i < _headerPanel.Controls.Count; i++)
            {
                if (_headerPanel.Controls[i] is BeepButton btn)
                {
                    bool isSelected = (i == index);
                    btn.IsSelected = isSelected;
                    btn.Invalidate();
                }
            }
        }

        public override void ApplyTheme()
        {
            if (Theme != null)
            {
                this.BackColor = _currentTheme.PanelBackColor;
                _headerPanel.BackColor = _currentTheme.PanelBackColor; // Or another suitable property

                // Re-apply theme to all buttons
                foreach (Control ctrl in _headerPanel.Controls)
                {
                    if (ctrl is BeepButton btn)
                    {
                        btn.Theme = Theme;
                        btn.ApplyTheme();
                    }
                }
            }
            else
            {
                // If no theme, revert to defaults
                this.BackColor = SystemColors.Control;
                _headerPanel.BackColor = SystemColors.Control;
            }

            Invalidate();
        }
    }
}