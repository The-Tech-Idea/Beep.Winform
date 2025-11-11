using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Control
{
    /// <summary>
    /// ButtonGroup - Group of related action buttons using BaseControl's hit area system like BeepAppBar
    /// </summary>
    internal sealed class ButtonGroupPainter : WidgetPainterBase
    {
        // Button layout data - no hit area management, just UI state
        private List<Rectangle> _buttonRects = new List<Rectangle>();
        private List<string> _buttonTexts = new List<string>();
        private int _selectedButtonIndex = 0;

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int padding = 8;
            // Always base on Owner.DrawingRect to respect BaseControl layout pipeline
            var baseRect = Owner?.DrawingRect ?? drawingRect;
            ctx.DrawingRect = Rectangle.Inflate(baseRect, -4, -4);
            
            // Header area for title
            ctx.HeaderRect = new Rectangle(
                ctx.DrawingRect.Left + padding,
                ctx.DrawingRect.Top + padding,
                ctx.DrawingRect.Width - padding * 2,
                string.IsNullOrEmpty(ctx.Title) ? 0 : 24
            );
            
            // Button area
            int contentTop = ctx.HeaderRect.Bottom + (ctx.HeaderRect.Height > 0 ? 8 : 0);
            ctx.ContentRect = new Rectangle(
                ctx.DrawingRect.Left + padding,
                contentTop,
                ctx.DrawingRect.Width - padding * 2,
                ctx.DrawingRect.Bottom - contentTop - padding
            );

            // FIXED: Setup hit areas using BaseControl's system directly (like BeepAppBar)
            SetupHitAreas(ctx);
            
            return ctx;
        }

        private void SetupHitAreas(WidgetContext ctx)
        {
            _buttonTexts = GetSampleButtons();
            _buttonRects = CalculateButtonLayout(ctx.ContentRect, _buttonTexts.Count);
            
            // FIXED: Clear existing hit areas and add new ones using BaseControl's methods
            ClearOwnerHitAreas();
            
            // Add hit area for each button using BaseControl's AddHitArea (same as BeepAppBar)
            for (int i = 0; i < _buttonRects.Count; i++)
            {
                int buttonIndex = i; // Capture for closure
                string buttonName = $"ButtonGroup_Button_{i}";
                AddHitAreaToOwner(buttonName, _buttonRects[i], () => HandleButtonClick(buttonIndex, ctx));
            }

            // Add title area if present
            if (!string.IsNullOrEmpty(ctx.Title))
            {
                AddHitAreaToOwner("ButtonGroup_Title", ctx.HeaderRect, () => HandleTitleClick(ctx));
            }
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            // Draw subtle background with light border
            var bgColor = Theme?.BackColor ?? Color.FromArgb(250, 250, 250);
            var borderColor = Theme?.BorderColor ?? Color.FromArgb(220, 220, 220);
            
            using (var bgBrush = new SolidBrush(bgColor))
            using (var borderPen = new Pen(borderColor))
            {
                g.FillRoundedRectangle(bgBrush, ctx.DrawingRect, 6);
                g.DrawRoundedRectangle(borderPen, ctx.DrawingRect, 6);
            }
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            // FIXED: Check hover state using BaseControl's hit test system
            bool isTitleHovered = IsAreaHovered("ButtonGroup_Title");
            DrawTitle(g, ctx, isTitleHovered);
            DrawButtons(g, ctx);
        }

        private void DrawTitle(Graphics g, WidgetContext ctx, bool isTitleHovered)
        {
            // Draw title if present with hover effects
            if (!string.IsNullOrEmpty(ctx.Title))
            {
                var titleColor = isTitleHovered 
                    ? Theme?.PrimaryColor ?? Color.FromArgb(0, 120, 215)
                    : Theme?.TextBoxForeColor ?? Color.FromArgb(70, 70, 70);
                
                using (var titleFont = new Font("Segoe UI", 10f, 
                    isTitleHovered ? FontStyle.Bold | FontStyle.Underline : FontStyle.Bold))
                using (var titleBrush = new SolidBrush(titleColor))
                {
                    var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                    string titleText = isTitleHovered ? $"{ctx.Title} - Button Group Options" : ctx.Title;
                    g.DrawString(titleText, titleFont, titleBrush, ctx.HeaderRect, format);
                }
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Enhanced focus indicators and hover effects using BaseControl's hit test system
            if (_buttonRects.Count > 0)
            {
                // Draw focus on selected button
                var focusColor = Theme?.AccentColor ?? Color.FromArgb(0, 120, 215);
                using (var focusPen = new Pen(focusColor, 2))
                {
                    var focusRect = Rectangle.Inflate(_buttonRects[_selectedButtonIndex], 2, 2);
                    g.DrawRoundedRectangle(focusPen, focusRect, 4);
                }

                // Draw hover glow effect if a button is hovered
                int hoveredButtonIndex = GetHoveredButtonIndex();
                if (hoveredButtonIndex >= 0 && hoveredButtonIndex < _buttonRects.Count)
                {
                    var glowColor = Color.FromArgb(30, Theme?.PrimaryColor ?? Color.Blue);
                    using (var glowBrush = new SolidBrush(glowColor))
                    {
                        var glowRect = Rectangle.Inflate(_buttonRects[hoveredButtonIndex], 4, 4);
                        g.FillRoundedRectangle(glowBrush, glowRect, 8);
                    }
                }
            }

            // Draw title hover indicator
            if (IsAreaHovered("ButtonGroup_Title") && !string.IsNullOrEmpty(ctx.Title))
            {
                using (var underlinePen = new Pen(Color.FromArgb(150, Theme?.PrimaryColor ?? Color.Blue), 2))
                {
                    g.DrawLine(underlinePen, 
                        ctx.HeaderRect.Left, ctx.HeaderRect.Bottom - 2,
                        ctx.HeaderRect.Right, ctx.HeaderRect.Bottom - 2);
                }
            }
        }

        private void DrawButtons(Graphics g, WidgetContext ctx)
        {
            // Get hovered button index from BaseControl's hit test system
            int hoveredButtonIndex = GetHoveredButtonIndex();

            // Draw each button with enhanced visual feedback
            for (int i = 0; i < _buttonTexts.Count && i < _buttonRects.Count; i++)
            {
                bool isSelected = i == _selectedButtonIndex;
                bool isHovered = i == hoveredButtonIndex;
                DrawButton(g, _buttonRects[i], _buttonTexts[i], isSelected, isHovered);
            }
        }

        private int GetHoveredButtonIndex()
        {
            // FIXED: Get hovered button index from BaseControl's hit test system
            string hoveredAreaName = GetHoveredAreaName();
            if (hoveredAreaName?.StartsWith("ButtonGroup_Button_") == true)
            {
                if (int.TryParse(hoveredAreaName.Replace("ButtonGroup_Button_", ""), out int buttonIndex))
                {
                    return buttonIndex;
                }
            }
            return -1;
        }

        private List<Rectangle> CalculateButtonLayout(Rectangle area, int buttonCount)
        {
            if (buttonCount == 0) return new List<Rectangle>();
            
            var buttons = new List<Rectangle>();
            int spacing = 4;
            int buttonWidth = (area.Width - (buttonCount - 1) * spacing) / buttonCount;
            
            for (int i = 0; i < buttonCount; i++)
            {
                buttons.Add(new Rectangle(
                    area.Left + i * (buttonWidth + spacing),
                    area.Top,
                    buttonWidth,
                    Math.Min(32, area.Height)
                ));
            }
            
            return buttons;
        }

        private void DrawButton(Graphics g, Rectangle rect, string text, bool isSelected, bool isHovered)
        {
            // Enhanced button drawing with multiple states
            Color bgColor;
            Color textColor;
            Color borderColor;

            if (isSelected)
            {
                bgColor = Theme?.AccentColor ?? Color.FromArgb(0, 120, 215);
                textColor = Color.White;
                borderColor = Theme?.AccentColor ?? Color.FromArgb(0, 120, 215);
            }
            else if (isHovered)
            {
                bgColor = Theme?.ButtonHoverBackColor ?? Color.FromArgb(225, 225, 225);
                textColor = Theme?.ButtonHoverForeColor ?? Color.FromArgb(50, 50, 50);
                borderColor = Theme?.ButtonHoverBorderColor ?? Color.FromArgb(150, 150, 150);
            }
            else
            {
                bgColor = Theme?.ButtonBackColor ?? Color.FromArgb(240, 240, 240);
                textColor = Theme?.ButtonForeColor ?? Color.FromArgb(70, 70, 70);
                borderColor = Theme?.BorderColor ?? Color.FromArgb(200, 200, 200);
            }

            using (var bgBrush = new SolidBrush(bgColor))
            using (var textBrush = new SolidBrush(textColor))
            using (var borderPen = new Pen(borderColor))
            using (var font = new Font("Segoe UI", 8.5f, isSelected ? FontStyle.Bold : FontStyle.Regular))
            {
                // Scale button slightly when hovered (similar to BeepAppBar icons)
                var drawRect = isHovered ? Rectangle.Inflate(rect, 1, 1) : rect;
                
                g.FillRoundedRectangle(bgBrush, drawRect, 4);
                g.DrawRoundedRectangle(borderPen, drawRect, 4);
                
                var format = new StringFormat 
                { 
                    Alignment = StringAlignment.Center, 
                    LineAlignment = StringAlignment.Center 
                };

                // Add hover hint to text
                string displayText = isHovered && !isSelected ? $"{text} ✓" : text;
                g.DrawString(displayText, font, textBrush, drawRect, format);
            }
        }

        private List<string> GetSampleButtons()
        {
            return new List<string> { "Create", "Edit", "Delete", "Archive" };
        }

        // FIXED: Action handlers work with BaseControl's hit area system (same as BeepAppBar)
        private void HandleButtonClick(int buttonIndex, WidgetContext ctx)
        {
            // Update selected button
            _selectedButtonIndex = buttonIndex;
            
            // Store button action in context for parent control to handle
            string buttonText = buttonIndex < _buttonTexts.Count ? _buttonTexts[buttonIndex] : $"Button_{buttonIndex}";
            ctx.SelectedButton = buttonText;
            ctx.SelectedButtonIndex = buttonIndex;
            
            // Force repaint to show selection change (same as BeepAppBar)
            Owner?.Invalidate();
            
            // OPTIONAL: Raise event for external handling
            OnButtonGroupAction?.Invoke(this, new ButtonGroupActionEventArgs(buttonText, buttonIndex, "ButtonClick"));
        }

        private void HandleTitleClick(WidgetContext ctx)
        {
            // Show button group configuration or help
            ctx.ShowButtonGroupOptions = true;
            
            // Force repaint (same as BeepAppBar)
            Owner?.Invalidate();
            
            // OPTIONAL: Raise event for external handling
            OnButtonGroupAction?.Invoke(this, new ButtonGroupActionEventArgs(ctx.Title ?? "Title", -1, "TitleClick"));
        }

        // OPTIONAL: Event for external subscribers
        public event EventHandler<ButtonGroupActionEventArgs>? OnButtonGroupAction;
    }

    // OPTIONAL: Event args for button group actions
    public class ButtonGroupActionEventArgs : EventArgs
    {
        public string Text { get; }
        public int Index { get; }
        public string ActionType { get; }
        
        public ButtonGroupActionEventArgs(string text, int index, string actionType)
        {
            Text = text;
            Index = index;
            ActionType = actionType;
        }
    }
}
