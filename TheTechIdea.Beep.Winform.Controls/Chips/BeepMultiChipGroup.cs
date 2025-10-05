﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Chips.Helpers;
using TheTechIdea.Beep.Winform.Controls.Chips.Painters;

namespace TheTechIdea.Beep.Winform.Controls.Chips
{
    #region Enums
    /// <summary>
    /// Chip visual variants following Material Design principles
    /// </summary>
    public enum ChipVariant
    {
        Filled,
        Text,
        Outlined
    }

    public enum ChipColor
    {
        Default,
        Primary,
        Secondary,
        Info,
        Success,
        Warning,
        Error,
        Dark
    }

    public enum ChipSize
    {
        Small,
        Medium,
        Large
    }

    public enum ChipSelectionMode
    {
        Single,
        Multiple,
        Toggle
    }

    public enum ChipStyle
    {
        Default,
        Modern,
        Classic,
        Minimalist,
        Colorful,
        Professional,
        Soft,
        HighContrast
    }
    #endregion

    [ToolboxItem(true)]
    [DisplayName("Beep MultiChip")]
    [Description("A MultiChip.")]
    public partial class BeepMultiChipGroup : BaseControl
    {
        #region Fields
        private BindingList<SimpleItem> _chipItems = new BindingList<SimpleItem>();
        private List<ChipItem> _chips = new List<ChipItem>();
        private readonly int _chipPadding = 5;
        private readonly int _chipCornerRadius = 15;
        private SimpleItem _selectedItem;
        private int _selectedIndex = -1;

        // Title
        private string _titleText = "Multi-Select Chip Group";
        private Font _titleFont = new Font("Segoe UI", 12, FontStyle.Bold);
        private Color _titleColor = Color.Black;
        private ContentAlignment _titleAlignment = ContentAlignment.TopLeft;
        private int _titleHeight = 30;

        // Chip styling
        private ChipVariant _chipVariant = ChipVariant.Filled;
        private ChipColor _chipColor = ChipColor.Default;
        private ChipSize _chipSize = ChipSize.Medium;
        private ChipSelectionMode _selectionMode = ChipSelectionMode.Single;
        private BindingList<SimpleItem> _selectedItems = new BindingList<SimpleItem>();

        private ChipStyle _chipStyle = ChipStyle.Default;
        private int _chipBorderWidth = 1;
        private bool _showChipBorders = true;

        // Painter infra
        private IChipGroupPainter _painter;
        private readonly Dictionary<int, Rectangle> _closeRects = new();
        private ChipRenderOptions _renderOptions = new ChipRenderOptions();
        #endregion

        #region Properties
        [Browsable(true)]
        [Localizable(true)]
        [MergableProperty(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category("Data")]
        [Description("The list of chip items to display in the group.")]
        public BindingList<SimpleItem> ListItems
        {
            get => _chipItems;
            set
            {
                _chipItems = value;
                UpdateChipsFromItems();
                ApplyThemeToChips();
                UpdateChipBounds();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Data")]
        [Description("The currently selected chip item.")]
        public SimpleItem SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                if (_selectedItem != null)
                {
                    _selectedIndex = _chipItems.IndexOf(_selectedItem);
                    UpdateSelectedChips();
                }
                else
                {
                    _selectedIndex = -1;
                    _chips.ForEach(c => c.IsSelected = false);
                }
                OnSelectedItemChanged(_selectedItem);
                Invalidate();
            }
        }

        [Browsable(false)]
        [Category("Data")]
        [Description("The index of the currently selected chip item.")]
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (value >= 0 && value < _chipItems.Count)
                {
                    _selectedIndex = value;
                    _selectedItem = _chipItems[_selectedIndex];
                    UpdateSelectedChips();
                    OnSelectedItemChanged(_selectedItem);
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("The title text displayed above the chip group.")]
        public string TitleText
        {
            get => _titleText;
            set { _titleText = value; UpdateChipBounds(); Invalidate(); }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("The font used for the title text.")]
        public Font TitleFont
        {
            get => _titleFont;
            set { _titleFont = value; UpdateChipBounds(); Invalidate(); }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("The color of the title text.")]
        public Color TitleColor { get => _titleColor; set { _titleColor = value; Invalidate(); } }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("The alignment of the title text.")]
        public ContentAlignment TitleAlignment { get => _titleAlignment; set { _titleAlignment = value; Invalidate(); } }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("The visual variant of the chips (Filled, Text, Outlined).")]
        [DefaultValue(ChipVariant.Filled)]
        public ChipVariant ChipVariant { get => _chipVariant; set { _chipVariant = value; Invalidate(); } }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("The color of the chip backgrounds.")]
        public ChipColor ChipColor { get => _chipColor; set { _chipColor = value; ApplyThemeToChips(); Invalidate(); } }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("The size of the chips.")]
        public ChipSize ChipSize { get => _chipSize; set { _chipSize = value; UpdateChipBounds(); Invalidate(); } }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("The selection mode of the chip group (Single, Multiple, Toggle).")]
        public ChipSelectionMode SelectionMode { get => _selectionMode; set { _selectionMode = value; Invalidate(); } }

        [Browsable(true)]
        [Category("Data")]
        [Description("The collection of currently selected chip items.")]
        public BindingList<SimpleItem> SelectedItems { get => _selectedItems; set { _selectedItems = value; UpdateSelectedChips(); Invalidate(); } }

        [Browsable(true)]
        [Category("Chip ProgressBarStyle")]
        [Description("Predefined styling preset for chips.")]
        [DefaultValue(ChipStyle.Default)]
        public ChipStyle ChipStyle { get => _chipStyle; set { _chipStyle = value; ApplyChipStyle(value); Invalidate(); } }

        [Browsable(true)]
        [Category("Chip ProgressBarStyle")]
        [Description("Width of chip borders.")]
        [DefaultValue(1)]
        public int ChipBorderWidth { get => _chipBorderWidth; set { _chipBorderWidth = Math.Max(0, value); Invalidate(); } }

        [Browsable(true)]
        [Category("Chip ProgressBarStyle")]
        [Description("Whether to show borders around chips.")]
        [DefaultValue(true)]
        public bool ShowChipBorders { get => _showChipBorders; set { _showChipBorders = value; Invalidate(); } }

        public event EventHandler<SelectedItemChangedEventArgs> SelectedItemChanged;
        protected virtual void OnSelectedItemChanged(SimpleItem selectedItem) => SelectedItemChanged?.Invoke(this, new SelectedItemChangedEventArgs(selectedItem));

        public new bool AutoScroll { get => false; set { } }
        #endregion

        #region Constructor
        public BeepMultiChipGroup() : base()
        {
            DoubleBuffered = true;
            AutoSize = true;
            IsRounded = true;
            BorderThickness = 1;
            ShowShadow = true;
            BackColor = Color.White;
            AnimationType = DisplayAnimationType.Fade;
            CanBeHovered = true;
            CanBePressed = false;
            CanBeFocused = false;

            // Painter setup
            _painter = new DefaultChipGroupPainter();
            _painter.Initialize(this, _currentTheme);
            _renderOptions = new ChipRenderOptions
            {
                Font = this.Font,
                CornerRadius = _chipCornerRadius,
                Gap = _chipPadding,
                ShowBorders = _showChipBorders,
                BorderWidth = _chipBorderWidth,
                Theme = _currentTheme,
                Size = _chipSize
            };

            ApplyChipStyle(_chipStyle);

            _chipItems.ListChanged += (s, e) =>
            {
                UpdateChipsFromItems();
                ApplyThemeToChips();
                UpdateChipBounds();
                Invalidate();
            };
        }
        #endregion

        #region Chip Item Class
        private class ChipItem
        {
            public SimpleItem Item { get; set; }
            public Rectangle Bounds { get; set; }
            public bool IsSelected { get; set; }
            public bool IsHovered { get; set; }
            public ChipVariant Variant { get; set; }
            public ChipColor Color { get; set; }
            public ChipSize Size { get; set; }
        }
        #endregion

        #region Chip Management
        private void UpdateChipsFromItems()
        {
            _chips.Clear();
            foreach (var item in _chipItems)
            {
                _chips.Add(new ChipItem
                {
                    Item = item,
                    IsSelected = _selectedItems.Contains(item),
                    IsHovered = false,
                    Variant = _chipVariant,
                    Color = _chipColor,
                    Size = _chipSize
                });
            }
            UpdateChipBounds();
        }

        private void UpdateSelectedChips()
        {
            foreach (var chip in _chips)
            {
                chip.IsSelected = _selectedItems.Contains(chip.Item);
            }
            _selectedItem = _selectedItems.FirstOrDefault();
            _selectedIndex = _selectedItem != null ? _chipItems.IndexOf(_selectedItem) : -1;
        }
        #endregion

        #region Data Binding
        public override void SetValue(object value)
        {
            if (value is SimpleItem selectedItem)
            {
                SelectedItem = selectedItem;
            }
            else if (value is string guidId)
            {
                SelectedItem = _chipItems.FirstOrDefault(item => item.GuidId == guidId);
            }
        }

        public override object GetValue() => SelectedItem;

        public override void ClearValue()
        {
            SelectedItem = null;
            _selectedIndex = -1;
            _chips.ForEach(c => c.IsSelected = false);
            Invalidate();
        }
        #endregion

        #region Theming
        public override void ApplyTheme()
        {
            _titleColor = _currentTheme.CardTitleForeColor;
            BackColor = _currentTheme.ButtonBackColor;
            _renderOptions.Theme = _currentTheme;
            _painter?.UpdateTheme(_currentTheme);
            ApplyThemeToChips();
            Invalidate();
        }

        private void ApplyThemeToChips()
        {
            Invalidate();
        }

        private void ApplyChipStyle(ChipStyle style)
        {
            // Keep borders config lightweight; actual visuals are painter-driven
            switch (style)
            {
                case ChipStyle.Minimalist:
                    _showChipBorders = false; _chipBorderWidth = 0; break;
                case ChipStyle.Classic:
                case ChipStyle.HighContrast:
                    _showChipBorders = true; _chipBorderWidth = 2; break;
                default:
                    _showChipBorders = true; _chipBorderWidth = 1; break;
            }

            SwitchPainter(style);
        }
        #endregion
    }
}