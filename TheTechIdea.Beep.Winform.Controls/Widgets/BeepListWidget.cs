using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Widgets.Helpers;
using System.Data;
using TheTechIdea.Beep.Editor;

namespace TheTechIdea.Beep.Winform.Controls.Widgets
{
    public enum ListWidgetStyle
    {
        ActivityFeed,     // Timeline-Style activities
        DataTable,        // Structured data table
        RankingList,      // Ordered ranking list
        StatusList,       // Items with status indicators
        ProfileList,      // User/profile listings
        TaskList          // Checklist/todo Style
    }

    [ToolboxItem(true)]
    [DisplayName("Beep List Widget")]
    [Category("Beep Widgets")]
    [Description("A list/table widget with multiple display styles.")]
    public class BeepListWidget : BaseControl
    {
        #region Fields
        private ListWidgetStyle _style = ListWidgetStyle.ActivityFeed;
        private IWidgetPainter _painter;
        private string _title = "List Title";
        private List<Dictionary<string, object>> _items = new List<Dictionary<string, object>>();
        private List<string> _columns = new List<string> { "Name", "Value", "Status" };
        private Color _accentColor = Color.FromArgb(33, 150, 243);
        private Color _itemBackColor = Color.White;
        private Color _selectedItemBackColor = Color.FromArgb(240, 240, 240);
        private Color _hoverItemBackColor = Color.FromArgb(245, 245, 245);
        private Color _disabledItemBackColor = Color.FromArgb(250, 250, 250);
        private Color _itemForeColor = Color.Black;
        private Color _disabledItemForeColor = Color.Gray;
        private Color _borderColor = Color.FromArgb(200, 200, 200);
        private bool _showHeader = true;
        private bool _allowSelection = true;
        private int _selectedIndex = -1;
        private int _maxVisibleItems = 10;

        // Events
        public event EventHandler<BeepEventDataArgs> ItemClicked;
        public event EventHandler<BeepEventDataArgs> ItemSelected;
        public event EventHandler<BeepEventDataArgs> HeaderClicked;
        #endregion

        #region Constructor
        public BeepListWidget() : base()
        {
            IsChild = false;
            Padding = new Padding(5);
            this.Size = new Size(300, 250);
            ApplyThemeToChilds = false;
            InitializeSampleData();
            ApplyTheme();
            CanBeHovered = true;
            InitializePainter();
        }

        private void InitializeSampleData()
        {
            _items.AddRange(new[]
            {
                new Dictionary<string, object> { ["Name"] = "John Doe", ["Value"] = "Manager", ["Status"] = "Active", ["Time"] = "2 hours ago" },
                new Dictionary<string, object> { ["Name"] = "Jane Smith", ["Value"] = "Developer", ["Status"] = "Online", ["Time"] = "5 minutes ago" },
                new Dictionary<string, object> { ["Name"] = "Bob Wilson", ["Value"] = "Designer", ["Status"] = "Busy", ["Time"] = "1 hour ago" }
            });
        }

        private void InitializePainter()
        {
            switch (_style)
            {
                case ListWidgetStyle.ActivityFeed:
                    _painter = new ActivityFeedPainter();
                    break;
                case ListWidgetStyle.DataTable:
                    _painter = new DataTablePainter();
                    break;
                case ListWidgetStyle.RankingList:
                    _painter = new RankingListPainter();
                    break;
                case ListWidgetStyle.StatusList:
                    _painter = new StatusListPainter();
                    break;
                case ListWidgetStyle.ProfileList:
                    _painter = new ProfileListPainter();
                    break;
                case ListWidgetStyle.TaskList:
                    _painter = new TaskListPainter();
                    break;
                default:
                    _painter = new ActivityFeedPainter();
                    break;
            }
            _painter?.Initialize(this, _currentTheme);
        }
        #endregion

        #region Properties
        [Category("List")]
        [Description("Visual Style of the list widget.")]
        public ListWidgetStyle Style
        {
            get => _style;
            set { _style = value; InitializePainter(); Invalidate(); }
        }

        [Category("List")]
        [Description("Title of the list.")]
        public string Title
        {
            get => _title;
            set { _title = value; Invalidate(); }
        }

        [Category("List")]
        [Description("Data items for the list.")]
        public List<Dictionary<string, object>> Items
        {
            get => _items;
            set { _items = value ?? new List<Dictionary<string, object>>(); Invalidate(); }
        }

        [Category("List")]
        [Description("Column names for table-Style lists.")]
        public List<string> Columns
        {
            get => _columns;
            set { _columns = value ?? new List<string>(); Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Primary accent color for the list.")]
        public Color AccentColor
        {
            get => _accentColor;
            set { _accentColor = value; Invalidate(); }
        }

        [Category("List")]
        [Description("Whether to show the header.")]
        public bool ShowHeader
        {
            get => _showHeader;
            set { _showHeader = value; Invalidate(); }
        }

        [Category("List")]
        [Description("Whether items can be selected.")]
        public bool AllowSelection
        {
            get => _allowSelection;
            set { _allowSelection = value; Invalidate(); }
        }

        [Category("List")]
        [Description("Index of the selected item.")]
        public int SelectedIndex
        {
            get => _selectedIndex;
            set { _selectedIndex = value; Invalidate(); }
        }

        [Category("List")]
        [Description("Maximum number of visible items.")]
        public int MaxVisibleItems
        {
            get => _maxVisibleItems;
            set { _maxVisibleItems = value; Invalidate(); }
        }
        #endregion

        #region Drawing
        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);

            var ctx = new WidgetContext
            {
                DrawingRect = DrawingRect,
                Title = _title,
                Labels = _columns,
                AccentColor = _accentColor,
                ShowHeader = _showHeader,
                IsInteractive = true,
                CornerRadius = BorderRadius,
                CustomData = new Dictionary<string, object>
                {
                    ["Items"] = _items,
                    ["AllowSelection"] = _allowSelection,
                    ["SelectedIndex"] = _selectedIndex,
                    ["MaxVisibleItems"] = _maxVisibleItems
                }
            };

            _painter?.Initialize(this, _currentTheme);
            ctx = _painter?.AdjustLayout(DrawingRect, ctx) ?? ctx;

            _painter?.DrawBackground(g, ctx);
            _painter?.DrawContent(g, ctx);
            _painter?.DrawForegroundAccents(g, ctx);

            RefreshHitAreas(ctx);
            _painter?.UpdateHitAreas(this, ctx, (name, rect) => { });
        }

        private void RefreshHitAreas(WidgetContext ctx)
        {
            ClearHitList();

            if (ctx.ShowHeader && !ctx.HeaderRect.IsEmpty)
            {
                AddHitArea("Header", ctx.HeaderRect, null, () =>
                {
                    HeaderClicked?.Invoke(this, new BeepEventDataArgs("HeaderClicked", this));
                });
            }

            if (!ctx.ContentRect.IsEmpty)
            {
                AddHitArea("Content", ctx.ContentRect, null, () =>
                {
                    ItemClicked?.Invoke(this, new BeepEventDataArgs("ItemClicked", this));
                });
            }
        }
        #endregion

        public override void ApplyTheme()
        {
            base.ApplyTheme();
            if (_currentTheme == null) return;

            // Apply list-specific theme colors
            BackColor = _currentTheme.BackColor;
            ForeColor = _currentTheme.ForeColor;
            
            // Update item background colors
            _itemBackColor = _currentTheme.SurfaceColor;
            _selectedItemBackColor = _currentTheme.HighlightBackColor;
            _hoverItemBackColor = _currentTheme.ButtonHoverBackColor;
            _disabledItemBackColor = _currentTheme.DisabledBackColor;
            
            // Update text colors
            _itemForeColor = _currentTheme.ForeColor;
            _disabledItemForeColor = _currentTheme.DisabledForeColor;
            
            // Update accent and border colors
            _accentColor = _currentTheme.AccentColor;
            _borderColor = _currentTheme.BorderColor;
            
            InitializePainter();
            Invalidate();
        }
    }
}