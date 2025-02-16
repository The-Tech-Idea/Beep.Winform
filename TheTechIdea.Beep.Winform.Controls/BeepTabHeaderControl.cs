﻿using System;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.Drawing.Design;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using TheTechIdea.Beep.Desktop.Common;
using TheTechIdea.Beep.Winform.Controls.Converters;
using TheTechIdea.Beep.Winform.Controls.Design;

namespace TheTechIdea.Beep.Winform.Controls
{
    public enum HeaderDockPosition
    {
        Top,
        Bottom
    }

    [ToolboxItem(true)]
    [DisplayName("Beep Tab Header Control")]
    [Category("Beep Controls")]
    public class BeepTabHeaderControl : BeepControl
    {
        public TabControlWithoutHeader _targetTabControl;
        private bool _isUpdating; // a guard to prevent re-entrant loops
        private TabPage _selectedTab;
        public BeepTabHeaderControl()
        {
            AutoSize = true;
            HeaderPanel = new FlowLayoutPanel
            {
                AutoSize = false,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Dock = DockStyle.Fill,
                Padding = new Padding(1),

            };
            Controls.Add(HeaderPanel);
            RebuildHeader();
        

        }
        [Localizable(true)]
        [MergableProperty(false)]

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        private  FlowLayoutPanel HeaderPanel { get; set; }
        protected override Size DefaultSize => new Size(200, 30);
        private HeaderDockPosition _dockPosition = HeaderDockPosition.Top;
        [Localizable(true)]
        [MergableProperty(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<SimpleItem> Tabs { get; } = new List<SimpleItem>();
       
        /// <summary>
        /// Choose whether this header is docked above or below the target tab control.
        /// </summary>
        [Browsable(true)]
        [Category("Layout")]
        [DefaultValue(HeaderDockPosition.Top)]
        public HeaderDockPosition DockPosition
        {
            get => _dockPosition;
            set
            {
                if (_dockPosition != value)
                {
                    _dockPosition = value;
                   Reposition();
                }
            }
        }
        /// <summary>
        /// The TabControlWithoutHeader that this header will sync with.
        /// </summary>
      //  [Browsable(true)]
      //  [Category("Behavior")]
      //  [Description("Select which TabControlWithoutHeader this header belongs to.")]
      //  [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    ///    [TypeConverter(typeof(ReferenceConverter))]
        public TabControlWithoutHeader TargetTabControl
        {
            get => _targetTabControl;
            set
            {
                if (value==null)
                {
                    // If user cleared it, detach
                    _targetTabControl = null;
                    return;
                }

                // Unsubscribe from old target's events
                if (_targetTabControl != null)
                {
                    _targetTabControl.ControlAdded -= OnTabControl_ControlAdded;
                    _targetTabControl.ControlRemoved -= OnTabControl_ControlRemoved;
                    _targetTabControl.SelectedIndexChanged -= OnTabControl_SelectedIndexChanged;

                    _targetTabControl.LocationChanged -= Target_LocationOrSizeChanged;
                    _targetTabControl.SizeChanged -= Target_LocationOrSizeChanged;

                    // Also unsubscribe from our own location/size events if desired
                    this.LocationChanged -= Header_LocationOrSizeChanged;
                    this.SizeChanged -= Header_LocationOrSizeChanged;
                }

                _targetTabControl = value;

                if (_targetTabControl != null)
                {
                    // Ensure both are in the same parent container
                    var parentContainer = _targetTabControl.Parent;
                    if (parentContainer != null && parentContainer != this.Parent)
                    {
                        this.Parent?.Controls.Remove(this);
                        parentContainer.Controls.Add(this);
                    }

                    // Subscribe to add/remove/selection events
                    _targetTabControl.ControlAdded += OnTabControl_ControlAdded;
                    _targetTabControl.ControlRemoved += OnTabControl_ControlRemoved;
                    _targetTabControl.SelectedIndexChanged += OnTabControl_SelectedIndexChanged;

                    // Subscribe to location/size events for visual sync
                    _targetTabControl.LocationChanged += Target_LocationOrSizeChanged;
                    _targetTabControl.SizeChanged += Target_LocationOrSizeChanged;

                    // Watch our own location/size changes (for two-way sync, if desired)
                    this.LocationChanged += Header_LocationOrSizeChanged;
                    this.SizeChanged += Header_LocationOrSizeChanged;

                    // Build the header buttons for existing TabPages
                    RebuildHeader();

                    // Initial reposition
                    Reposition();
                    // Initialize our selected tab (if any)
                    if (_targetTabControl.SelectedTab != null)
                    {
                        _selectedTab = _targetTabControl.SelectedTab;
                    }
                    else if (_targetTabControl.TabPages.Count > 0)
                    {
                        _selectedTab = _targetTabControl.TabPages[0];
                        _targetTabControl.SelectedTab = _selectedTab;
                    }
                }
                else
                {
                    // Clear if no tab control
                    Tabs.Clear();
                    HeaderPanel.Controls.Clear();
                }
            }
        }
        /// <summary>
        /// A design-time friendly property that sets which TabPage is active.
        /// Setting this changes the TargetTabControl.SelectedTab, 
        /// and thus we can drag & drop controls onto that tab at design time.
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Gets or sets which tab page is currently selected in the target tab control.")]
        [TypeConverter(typeof(TabPageListConverter))]
        public TabPage SelectedTab
        {
            get => _selectedTab;
            set
            {
                if (value != null && _targetTabControl != null)
                {
                    if (_targetTabControl.TabPages.Contains(value))
                    {
                        _selectedTab = value;
                        _targetTabControl.SelectedTab = value;
                        UpdateButtonSelection();
                    }
                }
            }
        }
        #region "Design-Time Property"
        private string _targetTabControlName;
        /// <summary>
        /// A string-based property that references the Name of a TabControlWithoutHeader.
        /// The designer will persist a line like:
        ///     this.beepTabHeaderControl1.TargetTabControlName = "tabControlWithoutHeader1";
        /// in Designer.cs.
        /// </summary>
        //[Browsable(true)]
        //[Category("Behavior")]
        //[Description("The Name of the TabControlWithoutHeader this header control is attached to.")]
        //[TypeConverter(typeof(TabControlNameConverter))]
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        //public string TargetTabControlName
        //{
        //    get => _targetTabControlName;
        //    set
        //    {
        //        if (_targetTabControlName == value)
        //            return;

        //        _targetTabControlName = value;

        //        // Attempt immediate lookup if possible
        //        if (!string.IsNullOrEmpty(_targetTabControlName) && this.Parent != null)
        //        {
        //            var foundControl = FindControlByName<TabControlWithoutHeader>(this.Parent, _targetTabControlName);
        //            AttachToTabControl(foundControl);
        //        }
        //        else
        //        {
        //            // If empty string or no parent, detach
        //            DetachFromTabControl();
        //        }
        //    }
        //}

        #endregion
        #region "Attach/Detach Logic"

        /// <summary>
        /// Subscribes to relevant events of the newly found TabControlWithoutHeader, 
        /// unsubscribes from the old one, rebuilds the header UI, 
        /// etc.
        /// </summary>
        private void AttachToTabControl(TabControlWithoutHeader newTabControl)
        {
            if (_targetTabControl == newTabControl)
                return;

            // Unsubscribe from old
            if (_targetTabControl != null)
            {
                _targetTabControl.ControlAdded -= OnTabControl_ControlAdded;
                _targetTabControl.ControlRemoved -= OnTabControl_ControlRemoved;
                _targetTabControl.SelectedIndexChanged -= OnTabControl_SelectedIndexChanged;

                _targetTabControl.LocationChanged -= Target_LocationOrSizeChanged;
                _targetTabControl.SizeChanged -= Target_LocationOrSizeChanged;

                // If you had a 2-way coupling for the header
                this.LocationChanged -= Header_LocationOrSizeChanged;
                this.SizeChanged -= Header_LocationOrSizeChanged;
            }

            _targetTabControl = newTabControl;

            if (_targetTabControl != null)
            {
                // Subscribe to new
                _targetTabControl.ControlAdded += OnTabControl_ControlAdded;
                _targetTabControl.ControlRemoved += OnTabControl_ControlRemoved;
                _targetTabControl.SelectedIndexChanged += OnTabControl_SelectedIndexChanged;

                // If you want to sync location/size
                _targetTabControl.LocationChanged += Target_LocationOrSizeChanged;
                _targetTabControl.SizeChanged += Target_LocationOrSizeChanged;

                // If you want 2-way coupling (optional)
                this.LocationChanged += Header_LocationOrSizeChanged;
                this.SizeChanged += Header_LocationOrSizeChanged;
            }

            // Now rebuild the header UI
            RebuildHeader();
            // If you want to position the header near the tab control
            Reposition();
        }

        /// <summary>
        /// Detaches from any previously attached TabControlWithoutHeader.
        /// Unsubscribes from events, clears the header UI, etc.
        /// </summary>
        private void DetachFromTabControl()
        {
            if (_targetTabControl != null)
            {
                _targetTabControl.ControlAdded -= OnTabControl_ControlAdded;
                _targetTabControl.ControlRemoved -= OnTabControl_ControlRemoved;
                _targetTabControl.SelectedIndexChanged -= OnTabControl_SelectedIndexChanged;
                _targetTabControl.LocationChanged -= Target_LocationOrSizeChanged;
                _targetTabControl.SizeChanged -= Target_LocationOrSizeChanged;

                this.LocationChanged -= Header_LocationOrSizeChanged;
                this.SizeChanged -= Header_LocationOrSizeChanged;
            }
            _targetTabControl = null;

            Controls.Clear();
        }

        #endregion
        // When the target tab control moves or resizes, reposition the header
        private void Target_LocationOrSizeChanged(object sender, EventArgs e)
        {
            if (_isUpdating) return; // Guard against re-entrant calls

            _isUpdating = true;
            try
            {
                // This repositions the header so it stays above/below the tab control
                Reposition();
            }
            finally
            {
                _isUpdating = false;
            }
        }

        // If the user drags the header in the designer or resizes it,
        // we can optionally move or resize the tab control, or do nothing.
        private void Header_LocationOrSizeChanged(object sender, EventArgs e)
        {
            if (_targetTabControl == null)
                return;
            if (this.Parent == null)
                return;

            // Avoid infinite loops: if we are already updating, skip
            if (_isUpdating)
                return;

            try
            {
                _isUpdating = true;

                // Reposition the tab control based on the header's position/size
                switch (_dockPosition)
                {
                    case HeaderDockPosition.Top:
                        // If the header is at Y=someValue, then the tab control's top should start at header.Bottom.
                        // That means:
                        _targetTabControl.Left = this.Left;
                        _targetTabControl.Width = this.Width;
                        _targetTabControl.Top = this.Bottom;
                        break;

                    case HeaderDockPosition.Bottom:
                        // If the header is at Y=someValue, then the tab control's bottom should sit at header.Top.
                        // That means: tabControl.Top = header.Top - tabControl.Height
                        _targetTabControl.Left = this.Left;
                        _targetTabControl.Width = this.Width;
                        _targetTabControl.Top = this.Top - _targetTabControl.Height;
                        break;
                }
            }
            finally
            {
                _isUpdating = false;
            }
        }
        /// <summary>
        /// Positions this header relative to the target tab control.
        /// </summary>
        private void Reposition()
        {
            if (_targetTabControl == null || _targetTabControl.Parent == null)
                return;

            // Make sure we're in the same parent as the target
            if (this.Parent != _targetTabControl.Parent)
            {
                this.Parent?.Controls.Remove(this);
                _targetTabControl.Parent.Controls.Add(this);
            }

            // Align our width to match the tab control
            this.Width = _targetTabControl.Width;

            // Left-align or match the tab control's X position
            this.Left = _targetTabControl.Left;

            // Now place it above or below
            switch (_dockPosition)
            {
                case HeaderDockPosition.Top:
                    // The header's bottom edge is at the tab control's top edge
                    // so we do: header.Top = tabControl.Top - header.Height
                    this.Top = _targetTabControl.Top - this.Height;
                    break;

                case HeaderDockPosition.Bottom:
                    // The header's top edge is at the tab control's bottom edge
                    this.Top = _targetTabControl.Bottom;
                    break;
            }
        }
        /// <summary>
        /// Example method to position this header relative to the tab control
        /// (above or below or same X coordinate, etc.).
        /// </summary>
        private void RepositionHeader()
        {
            if (_targetTabControl == null || _targetTabControl.Parent == null)
                return;

            // Example: keep the same width as tab control
            this.Width = _targetTabControl.Width;
            this.Left = _targetTabControl.Left;

            // Place it above the tab control
            this.Top = _targetTabControl.Top - this.Height;
        }
        /// <summary>
        /// Clears and rebuilds the header buttons based on the current TabPages.
        /// </summary>
        private void RebuildHeader()
        {
            HeaderPanel.Controls.Clear();

            if (_targetTabControl == null)
                return;

            // Create a button for each TabPage
            for (int i = 0; i < _targetTabControl.TabPages.Count; i++)
            {
                var page = _targetTabControl.TabPages[i];
                var button = CreateTabButton(page);
              //  Console.WriteLine("Button Created");
                HeaderPanel.Controls.Add(button);
               // Console.WriteLine("Button Added");
            }

            // Highlight the currently selected tab
            UpdateButtonSelection();
        }

        /// <summary>
        /// Creates a button to represent one TabPage.
        /// </summary>
        private BeepExtendedButton CreateTabButton(TabPage page)
        {
            
            var btn = new BeepExtendedButton
            {
                Text = string.IsNullOrEmpty(page.Text) ? page.Name: page.Text,
                Tag = page, // store reference to TabPage
                AutoSize = false,
                Margin = new Padding(1),
                UseScaledFont = true,
                ExtendButtonImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.close.svg"
            };
           // btn.TextFont = new Font("Arial", 10, FontStyle.Regular);
            btn.ButtonClick += (s, e) =>
            {
                Console.WriteLine("Button Clicked");
                if (_targetTabControl != null)
                {   Console.WriteLine("TargetTab is not null");
                    _targetTabControl.SelectedTab = (TabPage)btn.Tag;
                    Console.WriteLine("Select Tab Done");
                }
            };
            btn.ExtendButtonClick += (s, e) =>
            {
                if (_targetTabControl != null)
                {
                    _targetTabControl.TabPages.Remove((TabPage)((BeepButton)s).Tag);
                }
                // On click.close current tab
                

            };
            // check if tabs has been added for this button by comparing Guidid
            SimpleItem item = Tabs.FirstOrDefault(x => x.Name == page.Name);
            if (item != null)
            {
                item.Text = page.Text;
            }
            else
            {
                Tabs.Add(new SimpleItem { Name = page.Name, Text = page.Text });
            }
            if (item != null)
            {
                if (item.ImagePath != null)
                {
                    btn.ImagePath = item.ImagePath;
                }
            }
      
          
            return btn;
        }

        /// <summary>
        /// Updates button states (e.g., bold the selected one).
        /// </summary>
        private void UpdateButtonSelection()
        {
            if (_targetTabControl == null)
                return;

            foreach (Control ctrl in HeaderPanel.Controls)
            {
                if (ctrl is BeepExtendedButton btn && btn.Tag is TabPage page)
                {
                    bool isSelected = (_targetTabControl.SelectedTab == page);
                    btn.IsSelected = isSelected;
                }
            }
        }
       
        // Event handlers to keep the header in sync:

        private void OnTabControl_ControlAdded(object sender, ControlEventArgs e)
        {
            if (e.Control is TabPage page)
            {
                var button = CreateTabButton(page);
                HeaderPanel.Controls.Add(button);
                UpdateButtonSelection();
            }
        }

        private void OnTabControl_ControlRemoved(object sender, ControlEventArgs e)
        {
            if (e.Control is TabPage pageToRemove)
            {
                // Find the corresponding button
                BeepExtendedButton found = null;
                foreach (Control c in HeaderPanel.Controls)
                {
                    if (c is BeepExtendedButton btn && btn.Tag == pageToRemove)
                    {
                        found = btn;
                        break;
                    }
                }
                if (found != null)
                {
                    HeaderPanel.Controls.Remove(found);
                    found.Dispose();
                }
            }
            UpdateButtonSelection();
        }

        private void OnTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_targetTabControl.SelectedTab != null)
            {
                _selectedTab = _targetTabControl.SelectedTab;
            }
            UpdateButtonSelection();
        }
        public override void ApplyTheme()
        {

            BackColor = _currentTheme.ButtonBackColor;
            ForeColor = _currentTheme.ButtonForeColor;
            HoverBackColor = _currentTheme.ButtonHoverBackColor;
            HoverForeColor = _currentTheme.ButtonHoverForeColor;
            ActiveBackColor = _currentTheme.ButtonActiveBackColor;
            for (int i = 0; i < HeaderPanel.Controls.Count; i++)
            {
                if (HeaderPanel.Controls[i] is BeepExtendedButton btn)
                {
                    btn.BackColor = _currentTheme.ButtonBackColor;
                    btn.ForeColor = _currentTheme.ButtonForeColor;
                    btn.HoverBackColor = _currentTheme.ButtonHoverBackColor;
                    btn.HoverForeColor = _currentTheme.ButtonHoverForeColor;
                    btn.ActiveBackColor = _currentTheme.ButtonActiveBackColor;
                    btn.Theme = Theme;
                }
            }
            Invalidate();  // Trigger repaint
        }

        /// <summary>
        /// Utility to find a control by 'name' in a given parent's .Controls collection.
        /// Optional recursion.
        /// </summary>
        private TControl FindControlByName<TControl>(Control parent, string name) where TControl : Control
        {
            // Direct child search
            foreach (Control c in parent.Controls)
            {
                if (c is TControl typed && c.Name == name)
                {
                    return typed;
                }
            }

            // If you want to search nested children (recursively), uncomment:
            //foreach (Control c in parent.Controls)
            //{
            //    var result = FindControlByName<TControl>(c, name);
            //    if (result != null) return result;
            //}
            return null;
        }

        /// <summary>
        /// If the parent changes, we might want to attempt the name->control lookup again.
        /// This helps if the header is initially placed in the form at design time 
        /// but the tab control isn't fully created or in the same container yet.
        /// </summary>
        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            if (!string.IsNullOrEmpty(_targetTabControlName) && this.Parent != null)
            {
                var foundControl = FindControlByName<TabControlWithoutHeader>(this.Parent, _targetTabControlName);
                AttachToTabControl(foundControl);
            }
        }
    }
}
