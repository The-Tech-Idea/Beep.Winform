using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Cards.Helpers;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepCard
    {
        private void RefreshLayout()
        {
            // Only keep DrawingRect calculation; painters decide internal layout
            Padding = new Padding(3);
            UpdateDrawingRect();
            InvalidateLayoutCache();
        }

        /// <summary>
        /// Invalidates the layout cache, forcing recalculation on next paint
        /// </summary>
        private void InvalidateLayoutCache()
        {
            _layoutCacheValid = false;
        }

        private void RefreshHitAreas(LayoutContext ctx)
        {
            ClearHitList();

            if (_isLoading)
            {
                if (_showSelectionCheckbox && !_selectionRect.IsEmpty)
                {
                    AddHitArea("SelectionCheckbox", _selectionRect, null, () => IsSelected = !IsSelected);
                }

                if (!string.IsNullOrWhiteSpace(_contextMenuIcon) && !_contextMenuRect.IsEmpty)
                {
                    AddHitArea("ContextMenu", _contextMenuRect, null, () => ContextMenuRequested?.Invoke(this, EventArgs.Empty));
                }

                if (_isCollapsible && !_collapseRect.IsEmpty)
                {
                    AddHitArea("CollapseChevron", _collapseRect, null, () => ToggleExpandedState());
                }

                return;
            }

            // Image hit area
            if (ctx.ShowImage && ctx.ImageRect != Rectangle.Empty)
            {
                AddHitArea("Image", ctx.ImageRect, null, () =>
                {
                    ImageClicked?.Invoke(this, new BeepEventDataArgs("ImageClicked", this));
                });
            }

            // Header hit area
            if (!string.IsNullOrEmpty(headerText) && ctx.HeaderRect != Rectangle.Empty)
            {
                AddHitArea("Header", ctx.HeaderRect, null, () =>
                {
                    HeaderClicked?.Invoke(this, new BeepEventDataArgs("HeaderClicked", this));
                });
            }

            // Paragraph hit area
            if (!string.IsNullOrEmpty(paragraphText) && ctx.ParagraphRect != Rectangle.Empty)
            {
                AddHitArea("Paragraph", ctx.ParagraphRect, null, () =>
                {
                    ParagraphClicked?.Invoke(this, new BeepEventDataArgs("ParagraphClicked", this));
                });
            }

            // Primary button hit area
            if (ctx.ShowButton && ctx.ButtonRect != Rectangle.Empty)
            {
                AddHitArea("Button", ctx.ButtonRect, null, () =>
                {
                    ButtonClicked?.Invoke(this, new BeepEventDataArgs("ButtonClicked", this));
                });
            }

            // Secondary button hit area
            if (ctx.ShowSecondaryButton && ctx.SecondaryButtonRect != Rectangle.Empty)
            {
                AddHitArea("SecondaryButton", ctx.SecondaryButtonRect, null, () =>
                {
                    ButtonClicked?.Invoke(this, new BeepEventDataArgs("SecondaryButtonClicked", this));
                });
            }

            if (_showSelectionCheckbox && !_selectionRect.IsEmpty)
            {
                AddHitArea("SelectionCheckbox", _selectionRect, null, () =>
                {
                    IsSelected = !IsSelected;
                });
            }

            if (!string.IsNullOrWhiteSpace(_contextMenuIcon) && !_contextMenuRect.IsEmpty)
            {
                AddHitArea("ContextMenu", _contextMenuRect, null, () =>
                {
                    ContextMenuRequested?.Invoke(this, EventArgs.Empty);
                });
            }

            if (_isCollapsible && !_collapseRect.IsEmpty)
            {
                AddHitArea("CollapseChevron", _collapseRect, null, () =>
                {
                    ToggleExpandedState();
                });
            }
        }

        // Build layout context with all current card data
        private LayoutContext BuildLayoutContext()
        {
            // Determine if we should show image (check both property and design-time samples)
            bool hasImage = !string.IsNullOrEmpty(imagePath) || (DesignMode && !string.IsNullOrEmpty(GetDesignTimeSampleImage(_style)));
            
            return new LayoutContext
            {
                DrawingRect = DrawingRect,
                ImageRect = Rectangle.Empty, // Painter will calculate
                HeaderRect = Rectangle.Empty,
                ParagraphRect = Rectangle.Empty,
                ButtonRect = Rectangle.Empty,
                SecondaryButtonRect = Rectangle.Empty,
                ShowImage = hasImage,
                ShowButton = showButton,
                ShowSecondaryButton = showSecondaryButton,
                Radius = BorderRadius,
                AccentColor = _accentColor,
                Tags = _tags,
                BadgeText1 = _badgeText1,
                Badge1BackColor = _badge1BackColor,
                Badge1ForeColor = _badge1ForeColor,
                BadgeText2 = _badgeText2,
                Badge2BackColor = _badge2BackColor,
                Badge2ForeColor = _badge2ForeColor,
                SubtitleText = _subtitleText,
                StatusText = _statusText,
                StatusColor = _statusColor,
                ShowStatus = _showStatus,
                Rating = _rating,
                ShowRating = _showRating,
                IsHovered = _interactionManager?.HoverProgress > 0.001f,
                IsPressed = _interactionManager?.PressProgress > 0.001f,
                IsSelected = _isSelected,
                IsLoading = _isLoading,
                HoverProgress = _interactionManager?.HoverProgress ?? 0f,
                PressProgress = _interactionManager?.PressProgress ?? 0f,
                IsFocused = Focused,
                RipplePoint = _interactionManager?.RippleCenter ?? Point.Empty
            };
        }

        // Get image path with design-time fallback
        private string GetImagePath()
        {
            if (!string.IsNullOrEmpty(ImagePath))
                return ImagePath;

            if (DesignMode)
                return GetDesignTimeSampleImage(CardStyle);

            return null;
        }

    }
}
