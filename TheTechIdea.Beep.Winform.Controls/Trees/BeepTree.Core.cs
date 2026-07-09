using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Trees.Helpers;
using TheTechIdea.Beep.Winform.Controls.Trees.Models;
using TheTechIdea.Beep.Winform.Controls.Trees.Painters;
using TheTechIdea.Beep.Winform.Controls.Trees.Editors;
using TheTechIdea.Beep.Winform.Controls.Editors;
using TheTechIdea.Beep.Winform.Controls.CheckBoxes;
using TheTechIdea.Beep.Winform.Controls.Images;
using System.Drawing.Design;
using TheTechIdea.Beep.Winform.Controls.Scolling;

namespace TheTechIdea.Beep.Winform.Controls
{
	/// <summary>
	/// BeepTree - Core partial: fields, events, helpers, constructor, and common utilities.
	/// </summary>
	public partial class BeepTree
	{
		#region Fields
		// Style and painter
		private TreeStyle _treeStyle = TreeStyle.AntDesign;
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
		// Note: _textFont is inherited from BaseControl (protected)
		private TextAlignment _textAlignment = TextAlignment.Left;
		private bool _showCheckBox = false;
		private bool _enableThreeStateCheckboxes = false;

		// Behavior/performance
		private bool _allowMultiSelect = false;
		private bool _virtualizeLayout = true;
		private int _virtualizationBufferRows = 100;
		private bool _enableBackgroundLayout = true;

		// Render helpers (basic placeholders for theme application)
		private BeepButton _toggleRenderer = new BeepButton();
		private BeepCheckBoxBool _checkRenderer = new BeepCheckBoxBool();
		private BeepImage _iconRenderer = new BeepImage();
		private BeepButton _buttonRenderer = new BeepButton();

		// Context menu
		private BindingList<SimpleItem> _currentMenuItems;

		// Timers
		private System.Windows.Forms.Timer _resizeTimer;
		private System.Windows.Forms.Timer _typeAheadTimer;

		// Phase 3: empty state and filter
		private string _emptyStateText = "No items to display";
		private string _filterText = string.Empty;

		// Type-ahead search
		private string _typeAheadBuffer = string.Empty;

		// Column resizing
		private bool _isResizingColumn = false;
		private int _resizingColumnIndex = -1;
		private int _resizeStartX = 0;
		private int _resizeStartWidth = 0;
		private const int RESIZE_HIT_TEST_MARGIN = 4;

		// Drag and drop
		private BeepTreeDragDropManager _dragDropManager;
		private bool _allowDragDrop = false;

		// Inline editing
		private BeepTreeCellEditor _cellEditor;
		private DateTime _lastClickTime = DateTime.MinValue;
		private SimpleItem _lastDoubleClickNode = null;
		private const int SLOW_DOUBLE_CLICK_MIN_MS = 300;
		private const int SLOW_DOUBLE_CLICK_MAX_MS = 800;

		// Kinetic scrolling
		private bool _enableKineticScrolling = false;
		private bool _isKineticScrolling = false;
		private Point _kineticScrollStartPoint;
		private int _kineticScrollStartYOffset;
		private Timer _kineticTimer;
		private float _kineticVelocityY = 0;
		private const float KINETIC_FRICTION = 0.9f;
		private const float KINETIC_MIN_VELOCITY = 1.0f;
		private const int KINETIC_TIMER_INTERVAL = 16; // ~60 FPS

		// Find panel
		private BeepTreeFindPanel _findPanel;
		private List<SimpleItem> _findMatches = new();
		private int _currentFindIndex = -1;

		// Breadcrumb navigation
		private bool _showBreadcrumb = false;
		private int _breadcrumbHeight = 24;
		private List<Rectangle> _breadcrumbItemRects = new List<Rectangle>();
		private List<SimpleItem> _breadcrumbItems = new List<SimpleItem>();

		// Animation
		private BeepTreeAnimationHelper _animationHelper;
		private bool _enableAnimations = false;

		// Async image loading
		private BeepTreeAsyncImageLoader _asyncImageLoader;
		private bool _enableAsyncImageLoading = true;
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
		/// <summary>Fired before a node expands; set Cancel = true to prevent the expansion.</summary>
		public event EventHandler<BeepTreeNodeCancelEventArgs> NodeBeforeExpand;
		/// <summary>Fired before a node collapses; set Cancel = true to prevent the collapse.</summary>
		public event EventHandler<BeepTreeNodeCancelEventArgs> NodeBeforeCollapse;
		public event EventHandler<BeepMouseEventArgs> NodeChecked;
		public event EventHandler<BeepMouseEventArgs> NodeUnchecked;
	public event EventHandler<BeepMouseEventArgs> NodeAdded;
	public event EventHandler<BeepMouseEventArgs> NodeDeleted;
		public event EventHandler<NodesNeededEventArgs> NodesNeeded;
		public event EventHandler<BeepTreeItemDragEventArgs> ItemDrag;
		public event EventHandler<BeepTreeDragOverEventArgs> DragOverNode;
		public event EventHandler<BeepTreeDragDropEventArgs> NodeDragDrop;
		public event EventHandler<BeepTreeQueryAllowedPositionEventArgs> QueryAllowedPosition;
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
		public BeepTree():base()
		{
			// Control configuration
			// SetStyle( ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.Opaque, true);
			// UpdateStyles();
			DoubleBuffered = true;

            // Accessibility (design-skill compliance)
            AccessibleRole = AccessibleRole.Outline;
            AccessibleName = "Tree";
            AccessibleDescription = "Hierarchical tree view with expandable nodes";
			//Padding= new Padding(2);
            // Initialize helpers
            _treeHelper = new BeepTreeHelper(this);
			_layoutHelper = new BeepTreeLayoutHelper(this, _treeHelper);
			_treeHitTestHelper = new BeepTreeHitTestHelper(this, _layoutHelper);
			_dragDropManager = new BeepTreeDragDropManager(this);
			_cellEditor = new BeepTreeCellEditor(this);

			// Wire up drag drop manager events
			_dragDropManager.ItemDrag += (s, e) => ItemDrag?.Invoke(this, e);
			_dragDropManager.DragOver += (s, e) => DragOverNode?.Invoke(this, e);
			_dragDropManager.DragDrop += (s, e) => NodeDragDrop?.Invoke(this, e);
			_dragDropManager.QueryAllowedPosition += (s, e) => QueryAllowedPosition?.Invoke(this, e);
		
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
			MouseWheel += (s, e) =>
			{
				// Perform scrolling
				int scrollAmount = e.Delta / SystemInformation.MouseWheelScrollDelta * SystemInformation.MouseWheelScrollLines * GetScaledMinRowHeight();
				_yOffset = Math.Max(0, Math.Min(_yOffset - scrollAmount, _totalContentHeight - GetClientArea().Height));
				
				// Update viewport layout for virtualization
				if (VirtualizeLayout && _layoutHelper != null)
				{
					_layoutHelper.UpdateViewportLayout();
				}
				
				try { _treeHitTestHelper?.RegisterHitAreas(); } catch { }
				Invalidate();
				
				// Fire event
				NodeMouseWheel?.Invoke(this, new BeepMouseEventArgs("MouseWheel", null));
			};

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
			// Sync helper cache from already-computed visible nodes (no second traversal)
			_layoutHelper?.SyncFromVisibleNodes(_visibleNodes);
			UpdateScrollBars();
			Invalidate();
		};

		// Type-ahead search timer (resets buffer after 1 second of inactivity)
		_typeAheadTimer = new System.Windows.Forms.Timer { Interval = 1000 };
		_typeAheadTimer.Tick += (s, e) =>
		{
			_typeAheadTimer.Stop();
			_typeAheadBuffer = string.Empty;
		};

		// Kinetic scrolling timer
		_kineticTimer = new System.Windows.Forms.Timer { Interval = KINETIC_TIMER_INTERVAL };
		_kineticTimer.Tick += (s, e) =>
		{
			if (Math.Abs(_kineticVelocityY) < KINETIC_MIN_VELOCITY)
			{
				_kineticTimer.Stop();
				_isKineticScrolling = false;
				return;
			}

			ScrollBy(0, -(int)_kineticVelocityY);
			_kineticVelocityY *= KINETIC_FRICTION;
		};

		// Find panel
		_findPanel = new BeepTreeFindPanel(this);
		_findPanel.Visible = false;
		_findPanel.FindNext += (s, e) => FindNext(e.SearchText, e.MatchCase, e.WholeWord);
		_findPanel.FindPrevious += (s, e) => FindPrevious(e.SearchText, e.MatchCase, e.WholeWord);
		_findPanel.Closed += (s, e) =>
		{
			_findMatches.Clear();
			_currentFindIndex = -1;
			Invalidate();
		};
		this.Controls.Add(_findPanel);

		// Animation helper
		_animationHelper = new BeepTreeAnimationHelper(this);

		// Async image loader
		_asyncImageLoader = new BeepTreeAsyncImageLoader(this);

			// Initialize scrollbars
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
#if DEBUG
				System.Diagnostics.Debug.WriteLine("[BeepTree] HandleCreated - forcing refresh");
#endif
				RebuildVisible();
				UpdateScrollBars();
				Invalidate(); // Let normal paint cycle handle rendering
				// Ensure hit areas exist once handle is created
				try { _treeHitTestHelper?.RegisterHitAreas(); } catch { }
			};
			
			this.VisibleChanged += (s, e) =>
			{
				if (this.Visible)
				{
#if DEBUG
					System.Diagnostics.Debug.WriteLine("[BeepTree] VisibleChanged to true - forcing refresh");
#endif
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
		}
		#endregion

		#region Scaling helpers - DPI-aware scaling via DpiScalingHelper
	internal int GetScaledBoxSize() => DpiScalingHelper.ScaleValue(14, this);
	internal int GetScaledImageSize() => DpiScalingHelper.ScaleValue(20, this);
	internal int GetScaledMinRowHeight() => DpiScalingHelper.ScaleValue(24, this);
	internal int GetScaledIndentWidth() => DpiScalingHelper.ScaleValue(16, this);
	internal int GetScaledVerticalPadding() => DpiScalingHelper.ScaleValue(4, this);
	#endregion

		#region Accessors used by helpers and painters
	internal int XOffset => _xOffset;
	internal int YOffset => _yOffset;
	internal BeepTreeLayoutHelper LayoutHelper => _layoutHelper;
	internal BeepTreeHitTestHelper HitTestHelper => _treeHitTestHelper;
	internal SimpleItem LastHoveredItem => _lastHoveredItem;
	internal List<NodeInfo> VisibleNodes => _visibleNodes;

	/// <summary>
	/// Notifies accessibility clients of an event.
	/// </summary>
	public new void AccessibilityNotifyClients(AccessibleEvents accEvent, int childID)
	{
		base.AccessibilityNotifyClients(accEvent, childID);
	}
	#endregion

	#region Disposal

	/// <summary>
	/// Creates the accessibility object for this tree control.
	/// </summary>
	protected override AccessibleObject CreateAccessibilityInstance()
	{
		return new BeepTreeAccessibleObject(this);
	}

	/// <summary>
	/// Disposes resources used by the tree, including the async image loader.
	/// </summary>
	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			// Dispose async image loader
			_asyncImageLoader?.Dispose();
			_asyncImageLoader = null;

			// Dispose timers
			_resizeTimer?.Stop();
			_resizeTimer?.Dispose();
			_resizeTimer = null;

			_typeAheadTimer?.Stop();
			_typeAheadTimer?.Dispose();
			_typeAheadTimer = null;

			_kineticTimer?.Stop();
			_kineticTimer?.Dispose();
			_kineticTimer = null;

			// Dispose render helpers
			_toggleRenderer?.Dispose();
			_toggleRenderer = null;
			_checkRenderer?.Dispose();
			_checkRenderer = null;
			_iconRenderer?.Dispose();
			_iconRenderer = null;
			_buttonRenderer?.Dispose();
			_buttonRenderer = null;

			// Dispose drag drop manager
			_dragDropManager?.Dispose();
			_dragDropManager = null;

			// Dispose cell editor
			_cellEditor?.Dispose();
			_cellEditor = null;

			// Dispose animation helper
			_animationHelper?.Dispose();
			_animationHelper = null;

			// Remove and dispose find panel
			if (_findPanel != null)
			{
				Controls.Remove(_findPanel);
				_findPanel.Dispose();
				_findPanel = null;
			}

			// Dispose smooth scroll timer
			_smoothScrollTimer?.Stop();
			_smoothScrollTimer?.Dispose();
			_smoothScrollTimer = null;
		}
		base.Dispose(disposing);
	}

	#endregion
	}

	/// <summary>
	/// Event arguments for cancellable node expand/collapse operations.
	/// Set <see cref="CancelEventArgs.Cancel"/> = true to prevent the state change.
	/// </summary>
	public sealed class BeepTreeNodeCancelEventArgs : CancelEventArgs
	{
		/// <summary>The node that is about to be expanded or collapsed.</summary>
		public SimpleItem Node { get; }

		public BeepTreeNodeCancelEventArgs(SimpleItem node)
		{
			Node = node;
		}
	}

	/// <summary>
	/// Event arguments for the NodesNeeded event, used for on-demand loading.
	/// </summary>
	public sealed class NodesNeededEventArgs : EventArgs
	{
		/// <summary>The node that needs children loaded.</summary>
		public SimpleItem Node { get; }

		/// <summary>Set to true if children were loaded.</summary>
		public bool ChildrenLoaded { get; set; }

		public NodesNeededEventArgs(SimpleItem node)
		{
			Node = node;
		}
	}
}