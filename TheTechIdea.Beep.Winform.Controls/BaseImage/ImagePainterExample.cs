using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Vis.Modules.Managers;
 

namespace TheTechIdea.Beep.Winform.Controls.BaseImage
{
    /// <summary>
    /// Example demonstrating how to use ImagePainter for custom drawing scenarios
    /// </summary>
    public class ImagePainterExample
    {
        /// <summary>
        /// Basic usage example - painting an image to a Graphics context
        /// </summary>
        public static void BasicUsage(Graphics graphics, Rectangle bounds)
        {
            // Create an image painter
            using var painter = new ImagePainter();
            
            // Load an image
            painter.ImagePath = "cancel.svg";
            
            // Configure basic properties
            painter.ScaleMode = ImageScaleMode.KeepAspectRatio;
            painter.ClipShape = ImageClipShape.Circle;
            
            // Draw the image
            painter.DrawImage(graphics, bounds);
        }

        /// <summary>
        /// Advanced usage with theme integration
        /// </summary>
        public static void ThemeIntegration(Graphics graphics, Rectangle bounds, IBeepTheme theme)
        {
            using var painter = new ImagePainter();
            
            // Load SVG and apply theme
            painter.ImagePath = "menu-icon.svg";
            painter.CurrentTheme = theme;
            painter.ImageEmbededin = ImageEmbededin.Menu;
            painter.ApplyThemeOnImage = true;
            
            // Configure appearance
            painter.ClipShape = ImageClipShape.RoundedRect;
            painter.CornerRadius = 8f;
            painter.Opacity = 0.8f;
            
            // Draw with theme colors
            painter.DrawImage(graphics, bounds);
        }

        /// <summary>
        /// Animation and effects example
        /// </summary>
        public static void AnimationEffects(Graphics graphics, Rectangle bounds, float animationProgress)
        {
            using var painter = new ImagePainter();
            
            painter.ImagePath = "loading.svg";
            
            // Apply animation effects
            painter.ManualRotationAngle = animationProgress * 360f; // Full rotation
            painter.PulseScale = 1.0f + (float)Math.Sin(animationProgress * Math.PI * 2) * 0.1f; // Pulse effect
            painter.FadeAlpha = 0.5f + (float)Math.Sin(animationProgress * Math.PI) * 0.5f; // Fade effect
            
            // Apply visual effects
            painter.Grayscale = false;
            painter.ClipShape = ImageClipShape.Circle;
            
            painter.DrawImage(graphics, bounds);
        }

        /// <summary>
        /// Custom control example using ImagePainter
        /// </summary>
        public class CustomImageControl : Control
        {
            private ImagePainter _painter;
            private Timer _animationTimer;
            private float _animationProgress = 0f;

            public string ImagePath
            {
                get => _painter?.ImagePath;
                set
                {
                    if (_painter != null)
                    {
                        _painter.ImagePath = value;
                        Invalidate();
                    }
                }
            }

            public IBeepTheme Theme
            {
                get => _painter?.CurrentTheme;
                set
                {
                    if (_painter != null)
                    {
                        _painter.CurrentTheme = value;
                        _painter.ApplyThemeOnImage = value != null;
                        Invalidate();
                    }
                }
            }

            public bool Animate { get; set; } = false;

            public CustomImageControl()
            {
                _painter = new ImagePainter();
                
                // Configure painter
                _painter.ScaleMode = ImageScaleMode.KeepAspectRatio;
                _painter.ClipShape = ImageClipShape.Circle;
                
                // Setup animation timer
                _animationTimer = new Timer { Interval = 16 }; // ~60 FPS
                _animationTimer.Tick += AnimationTimer_Tick;
                
                // Set control styles
                SetStyle(ControlStyles.AllPaintingInWmPaint | 
                        ControlStyles.UserPaint | 
                        ControlStyles.DoubleBuffer | 
                        ControlStyles.ResizeRedraw, true);
            }

            private void AnimationTimer_Tick(object sender, EventArgs e)
            {
                if (Animate)
                {
                    _animationProgress += 0.02f;
                    if (_animationProgress > 1f) _animationProgress = 0f;
                    
                    // Update animation properties
                    _painter.ManualRotationAngle = _animationProgress * 360f;
                    _painter.PulseScale = 1.0f + (float)Math.Sin(_animationProgress * Math.PI * 4) * 0.1f;
                    
                    Invalidate();
                }
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                
                if (_painter?.HasImage == true)
                {
                    var contentRect = new Rectangle(5, 5, Width - 10, Height - 10);
                    _painter.DrawImage(e.Graphics, contentRect);
                }
            }

            protected override void OnEnabledChanged(EventArgs e)
            {
                base.OnEnabledChanged(e);
                
                // Adjust painter properties based on enabled state
                if (_painter != null)
                {
                    _painter.Opacity = Enabled ? 1.0f : 0.5f;
                    _painter.Grayscale = !Enabled;
                    Invalidate();
                }
            }

            protected override void OnMouseEnter(EventArgs e)
            {
                base.OnMouseEnter(e);
                if (!Animate)
                {
                    _animationTimer.Start();
                }
            }

            protected override void OnMouseLeave(EventArgs e)
            {
                base.OnMouseLeave(e);
                if (!Animate)
                {
                    _animationTimer.Stop();
                    _animationProgress = 0f;
                    _painter.ManualRotationAngle = 0f;
                    _painter.PulseScale = 1.0f;
                    Invalidate();
                }
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    _animationTimer?.Dispose();
                    _painter?.Dispose();
                }
                base.Dispose(disposing);
            }
        }

        /// <summary>
        /// Example of using ImagePainter in a custom drawing scenario like OwnerDraw
        /// </summary>
        public static void OwnerDrawExample()
        {
            var listBox = new ListBox();
            listBox.DrawMode = DrawMode.OwnerDrawFixed;
            listBox.ItemHeight = 32;

            // Store painters for each item
            var painters = new System.Collections.Generic.Dictionary<int, ImagePainter>();

            listBox.DrawItem += (sender, e) =>
            {
                if (e.Index >= 0)
                {
                    // Create painter for this item if it doesn't exist
                    if (!painters.ContainsKey(e.Index))
                    {
                        painters[e.Index] = new ImagePainter();
                        painters[e.Index].ImagePath = $"icon{e.Index}.svg";
                        painters[e.Index].ScaleMode = ImageScaleMode.KeepAspectRatio;
                        painters[e.Index].ClipShape = ImageClipShape.Circle;
                        
                        // Apply theme if available
                        var theme = BeepThemesManager.GetTheme("DefaultType");
                        if (theme != null)
                        {
                            painters[e.Index].CurrentTheme = theme;
                            painters[e.Index].ImageEmbededin = ImageEmbededin.ListBox;
                            painters[e.Index].ApplyThemeOnImage = true;
                        }
                    }

                    // Draw background
                    e.DrawBackground();

                    // Draw icon
                    var iconRect = new Rectangle(e.Bounds.X + 4, e.Bounds.Y + 4, 24, 24);
                    painters[e.Index].DrawImage(e.Graphics, iconRect);

                    // Draw text
                    var textRect = new Rectangle(e.Bounds.X + 32, e.Bounds.Y, 
                                                e.Bounds.Width - 32, e.Bounds.Height);
                    TextRenderer.DrawText(e.Graphics, listBox.Items[e.Index].ToString(), 
                                        listBox.Font, textRect, e.ForeColor, TextFormatFlags.VerticalCenter);

                    e.DrawFocusRectangle();
                }
            };

            // Clean up painters when listbox is disposed
            listBox.Disposed += (sender, e) =>
            {
                foreach (var painter in painters.Values)
                {
                    painter.Dispose();
                }
                painters.Clear();
            };
        }

        /// <summary>
        /// Batch processing example - applying the same configuration to multiple painters
        /// </summary>
        public static void BatchConfiguration(ImagePainter[] painters, IBeepTheme theme)
        {
            foreach (var painter in painters)
            {
                // Apply common configuration
                painter.CurrentTheme = theme;
                painter.ApplyThemeOnImage = true;
                painter.ScaleMode = ImageScaleMode.KeepAspectRatio;
                painter.ClipShape = ImageClipShape.RoundedRect;
                painter.CornerRadius = 6f;
                painter.Opacity = 0.9f;
                
                // Apply theme-specific settings
                painter.ImageEmbededin = ImageEmbededin.Button;
            }
        }
    }
}