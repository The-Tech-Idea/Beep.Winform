using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Trees.Helpers;
using TheTechIdea.Beep.Winform.Controls.Trees.Models;
using TheTechIdea.Beep.Winform.Controls.Trees.Painters;
using TheTechIdea.Beep.Winform.Controls.Editors;
using System.Drawing.Design;

namespace TheTechIdea.Beep.Winform.Controls
{
	/// <summary>
	/// BeepTree - Core partial: fields, events, helpers, constructor, and common utilities.
	/// </summary>
	public partial class BeepTree
	{
		#region Fields
		// Style and painter
		private TreeStyle _treeStyle = TreeStyle.Standard;
		private ITreePainter _currentPainter;

		// Data and layout
		private List<SimpleItem> _nodes = new();
		private readonly List<NodeInfo> _visibleNodes = new();
		private BeepTreeHelper _treeHelper;
		private BeepTreeLayoutHelper _layoutHelper;
		private BeepTreeHitTestHelper _treeHitTestHelper;

		// Scrollbars and offsets
		private BeepScrollBar _verticalScrollBar;
		private BeepScrollBar _horizontalScrollBar;
		private bool _showVerticalScrollBar = true;
		private bool _showHorizontalScrollBar = true;
		private int _yOffset = 0;
		private int _xOffset = 0;
		private Size _virtualSize = Size.Empty;
		private int _totalContentHeight = 0;

		// Selection and hover state
		private List<SimpleItem> _selectedNodes = new();
		private SimpleItem _lastSelectedNode;
		private SimpleItem _lastClickedNode;
		private SimpleItem _lastHoveredItem = null;
		private Rectangle _lastHoveredRect = Rectangle.Empty;

		// Appearance
		private bool _useScaledFont = false;
		private bool _useThemeFont = true;
		private Font _textFont = new Font("Arial", 10);
		private TextAlignment _textAlignment = TextAlignment.Left;
		private bool _showCheckBox = true;

		// Behavior/performance
		private bool _allowMultiSelect = false;
		private bool _virtualizeLayout = true;
		private int _virtualizationBufferRows = 100;

		// Render helpers (basic placeholders for theme application)
		private BeepButton _toggleRenderer = new BeepButton();
		private BeepCheckBoxBool _checkRenderer = new BeepCheckBoxBool();
		private BeepImage _iconRenderer = new BeepImage();
		private BeepButton _buttonRenderer = new BeepButton();

		// Context menu
	private BindingList<SimpleItem> _currentMenuItems;

		// Timers
		private System.Windows.Forms.Timer _resizeTimer;
		#endregion

		#region Events
		public event EventHandler<BeepMouseEventArgs> LeftButtonClicked;
		public event EventHandler<BeepMouseEventArgs> NodeRightClicked;
		public event EventHandler<BeepMouseEventArgs> NodeMiddleClicked;
		public event EventHandler<BeepMouseEventArgs> NodeDoubleClicked;
		public event EventHandler<BeepMouseEventArgs> NodeSelected;
		public event EventHandler<BeepMouseEventArgs> NodeDeselected;
		public event EventHandler<BeepMouseEventArgs> NodeExpanded;
		public event EventHandler<BeepMouseEventArgs> NodeCollapsed;
		public event EventHandler<BeepMouseEventArgs> NodeChecked;
		public event EventHandler<BeepMouseEventArgs> NodeUnchecked;
	public event EventHandler<BeepMouseEventArgs> NodeAdded;
	public event EventHandler<BeepMouseEventArgs> NodeDeleted;
		public event EventHandler<BeepMouseEventArgs> NodeMouseEnter;
		public event EventHandler<BeepMouseEventArgs> NodeMouseLeave;
		public event EventHandler<BeepMouseEventArgs> NodeMouseHover;
		public event EventHandler<BeepMouseEventArgs> NodeMouseWheel;
		public event EventHandler<BeepMouseEventArgs> NodeMouseUp;
		public event EventHandler<BeepMouseEventArgs> NodeMouseDown;
		public event EventHandler<BeepMouseEventArgs> NodeMouseMove;
		public event EventHandler<SelectedItemChangedEventArgs> MenuItemSelected;

		public event EventHandler<SelectedItemChangedEventArgs> SelectedItemChanged;
		protected virtual void OnSelectedItemChanged(SimpleItem selectedItem)
		{
			SelectedItemChanged?.Invoke(this, new SelectedItemChangedEventArgs(selectedItem));
		}
		#endregion

		#region Constructor
		public BeepTree()
		{
			// Control configuration
			SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.Opaque, true);
			UpdateStyles();
			DoubleBuffered = true;

			// Initialize helpers
			_treeHelper = new BeepTreeHelper(this);
			_layoutHelper = new BeepTreeLayoutHelper(this, _treeHelper);
			_treeHitTestHelper = new BeepTreeHitTestHelper(this, _layoutHelper);

			// Initialize render helpers
			_toggleRenderer.IsChild = true;
			_toggleRenderer.HideText = true;
			_toggleRenderer.ImageAlign = ContentAlignment.MiddleCenter;
			_checkRenderer.IsChild = true;
			_iconRenderer.IsChild = true;
			_buttonRenderer.IsChild = true;

			// Setup mouse events wiring to Events.cs handlers
			MouseDown += OnMouseDownHandler;
			MouseUp += OnMouseUpHandler;
			MouseMove += OnMouseMoveHandler;
			MouseDoubleClick += OnMouseDoubleClickHandler;
			MouseHover += OnMouseHoverHandler;
			MouseEnter += (s, e) => NodeMouseEnter?.Invoke(this, new BeepMouseEventArgs("MouseEnter", null));
			MouseLeave += (s, e) => NodeMouseLeave?.Invoke(this, new BeepMouseEventArgs("MouseLeave", null));
			MouseWheel += (s, e) => NodeMouseWheel?.Invoke(this, new BeepMouseEventArgs("MouseWheel", null));

		// Debounce timer for resize
		_resizeTimer = new System.Windows.Forms.Timer { Interval = 50 };
		_resizeTimer.Tick += (s, e) =>
		{
			_resizeTimer.Stop();
			// Update DrawingRect first (viewport may have changed)
			UpdateDrawingRect();
			// Recalculate layout when size changes (viewport changed)
			if (_visibleNodes.Count > 0)
			{
				RecalculateLayoutCache();
			}
			// Also sync layout helper's cache
			if (_layoutHelper != null)
			{
				_layoutHelper.InvalidateCache();
				_layoutHelper.RecalculateLayout();
			}
			UpdateScrollBars();
			Invalidate();
		};			// Initialize scrollbars
			InitializeScrollbars();

			// Initialize painter and build initial layout
			InitializePainter();
			RebuildVisible();
			UpdateScrollBars();
			// Build initial hit areas for BaseControl hit-test infra
			try { _treeHitTestHelper?.RegisterHitAreas(); } catch { }
			
			// CRITICAL FIX: Handle visibility changes to ensure initial paint
			this.HandleCreated += (s, e) =>
			{
				System.Diagnostics.Debug.WriteLine("[BeepTree] HandleCreated - forcing refresh");
				RebuildVisible();
				UpdateScrollBars();
				Invalidate();
				Update(); // Force immediate paint
				// Ensure hit areas exist once handle is created
				try { _treeHitTestHelper?.RegisterHitAreas(); } catch { }
			};
			
			this.VisibleChanged += (s, e) =>
			{
				if (this.Visible)
				{
					System.Diagnostics.Debug.WriteLine("[BeepTree] VisibleChanged to true - forcing refresh");
					Invalidate();
					Update(); // Force immediate paint
				}
			};
		}
		#endregion

		#region Painter Management
		private void InitializePainter()
		{
			// Ensure theme exists (BaseControl sets _currentTheme in ctor)
			if (_currentTheme == null)
			{
				try
				{
					_currentTheme = ThemeManagement.BeepThemesManager.GetDefaultTheme();
				}
				catch { /* fallback to null-safe painters */ }
			}

			_currentPainter = BeepTreePainterFactory.CreatePainter(_treeStyle, this, _currentTheme);
		}

		private ITreePainter GetCurrentPainter()
		{
			return _currentPainter;
					try { _treeHitTestHelper?.RegisterHitAreas(); } catch { }
		}
		#endregion

		#region Scaling helpers
	internal int GetScaledBoxSize() => ScaleValue(14);
	internal int GetScaledImageSize() => ScaleValue(20);
	internal int GetScaledMinRowHeight() => ScaleValue(24);
	internal int GetScaledIndentWidth() => ScaleValue(16);
	internal int GetScaledVerticalPadding() => ScaleValue(4);
	#endregion

	#region Accessors used by helpers and painters
	internal int XOffset => _xOffset;
	internal int YOffset => _yOffset;
	internal BeepTreeLayoutHelper LayoutHelper => _layoutHelper;
	internal SimpleItem LastHoveredItem => _lastHoveredItem;
	internal List<NodeInfo> VisibleNodes => _visibleNodes;
	#endregion
}
}