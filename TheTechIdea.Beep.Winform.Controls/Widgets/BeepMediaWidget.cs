using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Widgets.Helpers;
using TheTechIdea.Beep.Winform.Controls.Widgets.Models;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Media;

namespace TheTechIdea.Beep.Winform.Controls.Widgets
{
    public enum MediaWidgetStyle
    {
        ImageCard,        // Card with background image and overlay text
        AvatarGroup,      // Clustered circular profile pictures  
        IconCard,         // Large icon with label/description
        MediaGallery,     // Image carousel/gallery display
        ProfileCard,      // User profile with photo and details
        ImageOverlay,     // Image with text overlay
        PhotoGrid,        // Grid of photos/thumbnails
        MediaViewer,      // Single media item display
        AvatarList,       // List of avatars with details
        IconGrid          // Grid of icons with labels
    }

    [ToolboxItem(true)]
    [DisplayName("Beep Media Widget")]
    [Category("Beep Widgets")]
    [Description("Media widget for images, avatars, icons, and galleries.")]
    public class BeepMediaWidget : BaseControl
    {
        #region Fields
        private MediaWidgetStyle _style = MediaWidgetStyle.ImageCard;
        private IWidgetPainter _painter;
        private string _title = "Media Title";
        private string _subtitle = "Subtitle";
        private string _imagePath = "";
        private Image _image = null;
        private Color _accentColor = Color.FromArgb(33, 150, 243);
        private Color _cardBackColor = Color.White;
        private Color _surfaceColor = Color.FromArgb(250, 250, 250);
        private Color _panelBackColor = Color.FromArgb(250, 250, 250);
        private Color _titleForeColor = Color.Black;
        private Color _textForeColor = Color.FromArgb(100, 100, 100);
        private Color _subTitleForeColor = Color.FromArgb(150, 150, 150);
        private Color _borderColor = Color.FromArgb(200, 200, 200);
        private Color _primaryColor = Color.FromArgb(33, 150, 243);
        private Color _hoverBackColor = Color.FromArgb(245, 245, 245);
        private Color _highlightBackColor = Color.FromArgb(240, 240, 240);
        private bool _showOverlay = true;
        private string _overlayText = "Overlay Text";
        private List<MediaItem> _mediaItems = new List<MediaItem>();

        // Events
        public event EventHandler<BeepEventDataArgs> MediaClicked;
        public event EventHandler<BeepEventDataArgs> ImageClicked;
        public event EventHandler<BeepEventDataArgs> AvatarClicked;
        public event EventHandler<BeepEventDataArgs> OverlayClicked;
        #endregion

        #region Constructor
        public BeepMediaWidget() : base()
        {
            IsChild = false;
            Padding = new Padding(5);
            this.Size = new Size(250, 200);
            ApplyThemeToChilds = false;
            InitializeSampleData();
            ApplyTheme();
            CanBeHovered = true;
            InitializePainter();
        }

        private void InitializeSampleData()
        {
            _mediaItems.AddRange(new[]
            {
                new MediaItem { Title = "User 1", ImagePath = "", IsAvatar = true },
                new MediaItem { Title = "User 2", ImagePath = "", IsAvatar = true },
                new MediaItem { Title = "User 3", ImagePath = "", IsAvatar = true },
                new MediaItem { Title = "Image 1", ImagePath = "", IsAvatar = false },
                new MediaItem { Title = "Image 2", ImagePath = "", IsAvatar = false }
            });
        }

        private void InitializePainter()
        {
            switch (_style)
            {
                case MediaWidgetStyle.ImageCard:
                    _painter = new ImageCardPainter();
                    break;
                case MediaWidgetStyle.AvatarGroup:
                    _painter = new AvatarGroupPainter();
                    break;
                case MediaWidgetStyle.IconCard:
                    _painter = new IconCardPainter();
                    break;
                case MediaWidgetStyle.MediaGallery:
                    _painter = new MediaGalleryPainter();
                    break;
                case MediaWidgetStyle.ProfileCard:
                    _painter = new ProfileCardPainter();
                    break;
                case MediaWidgetStyle.ImageOverlay:
                    _painter = new ImageOverlayPainter();
                    break;
                case MediaWidgetStyle.PhotoGrid:
                    _painter = new PhotoGridPainter();
                    break;
                case MediaWidgetStyle.MediaViewer:
                    _painter = new MediaViewerPainter();
                    break;
                case MediaWidgetStyle.AvatarList:
                    _painter = new AvatarListPainter();
                    break;
                case MediaWidgetStyle.IconGrid:
                    _painter = new IconGridPainter();
                    break;
                default:
                    _painter = new ImageCardPainter();
                    break;
            }
            _painter?.Initialize(this, _currentTheme);
        }
        #endregion

        #region Properties
        [Category("Media")]
        [Description("Visual Style of the media widget.")]
        public MediaWidgetStyle Style
        {
            get => _style;
            set { _style = value; InitializePainter(); Invalidate(); }
        }

        [Category("Media")]
        [Description("Title text for the media widget.")]
        public string Title
        {
            get => _title;
            set { _title = value; Invalidate(); }
        }

        [Category("Media")]
        [Description("Subtitle text for the media widget.")]
        public string Subtitle
        {
            get => _subtitle;
            set { _subtitle = value; Invalidate(); }
        }

        [Category("Media")]
        [Description("Path to the main image file.")]
        public string ImagePath
        {
            get => _imagePath;
            set { _imagePath = value; LoadImage(); Invalidate(); }
        }

        [Category("Media")]
        [Description("Main image for the widget.")]
        public Image Image
        {
            get => _image;
            set { _image = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Primary accent color for the media widget.")]
        public Color AccentColor
        {
            get => _accentColor;
            set { _accentColor = value; Invalidate(); }
        }

        [Category("Media")]
        [Description("Whether to show overlay on images.")]
        public bool ShowOverlay
        {
            get => _showOverlay;
            set { _showOverlay = value; Invalidate(); }
        }

        [Category("Media")]
        [Description("Text to display in overlay.")]
        public string OverlayText
        {
            get => _overlayText;
            set { _overlayText = value; Invalidate(); }
        }

        [Category("Media")]
        [Description("Collection of media items for galleries and grids.")]
        public List<MediaItem> MediaItems
        {
            get => _mediaItems;
            set { _mediaItems = value ?? new List<MediaItem>(); Invalidate(); }
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
                Value = _subtitle,
                AccentColor = _accentColor,
                ShowIcon = _showOverlay,
                IsInteractive = true,
                CornerRadius = BorderRadius,
                
                // Media-specific typed properties
                Image = _image,
                ShowOverlay = _showOverlay
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

            if (!ctx.ContentRect.IsEmpty)
            {
                AddHitArea("Media", ctx.ContentRect, null, () =>
                {
                    MediaClicked?.Invoke(this, new BeepEventDataArgs("MediaClicked", this));
                });
            }

            if (!ctx.IconRect.IsEmpty)
            {
                AddHitArea("Image", ctx.IconRect, null, () =>
                {
                    ImageClicked?.Invoke(this, new BeepEventDataArgs("ImageClicked", this));
                });
            }

            // Add hit areas for individual media items based on Style
            if (_style == MediaWidgetStyle.AvatarGroup || _style == MediaWidgetStyle.PhotoGrid)
            {
                for (int i = 0; i < _mediaItems.Count && i < 6; i++) // Limit to 6 visible items
                {
                    int itemIndex = i; // Capture for closure
                    AddHitArea($"Item{i}", new Rectangle(), null, () =>
                    {
                        AvatarClicked?.Invoke(this, new BeepEventDataArgs("AvatarClicked", this) { EventData = _mediaItems[itemIndex] });
                    });
                }
            }

            if (_showOverlay && !ctx.FooterRect.IsEmpty)
            {
                AddHitArea("Overlay", ctx.FooterRect, null, () =>
                {
                    OverlayClicked?.Invoke(this, new BeepEventDataArgs("OverlayClicked", this));
                });
            }
        }

        private void LoadImage()
        {
            try
            {
                if (!string.IsNullOrEmpty(_imagePath) && System.IO.File.Exists(_imagePath))
                {
                    _image?.Dispose();
                    _image = Image.FromFile(_imagePath);
                }
            }
            catch
            {
                _image = null; // Failed to load image
            }
        }
        #endregion

        public override void ApplyTheme()
        {
            base.ApplyTheme();
            if (_currentTheme == null) return;

            // Apply media-specific theme colors
            BackColor = _currentTheme.BackColor;
            ForeColor = _currentTheme.ForeColor;
            
            // Update card and surface colors
            _cardBackColor = _currentTheme.CardBackColor;
            _surfaceColor = _currentTheme.SurfaceColor;
            _panelBackColor = _currentTheme.PanelBackColor;
            
            // Update text colors
            _titleForeColor = _currentTheme.CardTitleForeColor;
            _textForeColor = _currentTheme.CardTextForeColor;
            _subTitleForeColor = _currentTheme.CardSubTitleForeColor;
            
            // Update border and accent colors
            _borderColor = _currentTheme.BorderColor;
            _accentColor = _currentTheme.AccentColor;
            _primaryColor = _currentTheme.PrimaryColor;
            
            // Update interactive colors
            _hoverBackColor = _currentTheme.ButtonHoverBackColor;
            _highlightBackColor = _currentTheme.HighlightBackColor;
            
            InitializePainter();
            Invalidate();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _image?.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    /// <summary>
    /// Media item data structure for galleries and grids
    /// </summary>
    public class MediaItem
    {
        public string Title { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public Image Image { get; set; }
        public bool IsAvatar { get; set; } = false;
        public Color AccentColor { get; set; } = Color.FromArgb(33, 150, 243);
        public string Tag { get; set; } = string.Empty;
        public WidgetMetadata Metadata { get; set; } = new WidgetMetadata();
    }
}
