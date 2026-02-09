using System;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// Partial class containing core rendering infrastructure for BeepSimpleGrid
    /// Handles main DrawContent override, layout recalculation, and rendering resource initialization
    /// </summary>
    public partial class BeepSimpleGrid
    {
        #region Core Rendering

        /// <summary>
        /// Main drawing method that orchestrates all grid rendering
        /// </summary>
        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);

            if (_layoutDirty)
            {
                RecalculateGridRect();
                _layoutDirty = false;
            }

            var drawingBounds = DrawingRect;

            // Calculate layout positions
            bottomPanelY = drawingBounds.Bottom;
            botomspacetaken = 0;
            topPanelY = drawingBounds.Top;

            // Reserve space for horizontal scrollbar if visible
            if (_horizontalScrollBar.Visible)
            {
                bottomPanelY -= _horizontalScrollBar.Height;
                botomspacetaken += _horizontalScrollBar.Height;
            }

            // Draw bottom panels (from bottom to top)
            if (_showNavigator)
            {
                bottomPanelY -= navigatorPanelHeight;
                botomspacetaken += navigatorPanelHeight;
                navigatorPanelRect = new Rectangle(drawingBounds.Left, drawingBounds.Bottom - navigatorPanelHeight - (_horizontalScrollBar.Visible ? _horizontalScrollBar.Height : 0), drawingBounds.Width, navigatorPanelHeight);
                DrawNavigationRow(g, navigatorPanelRect);
            }

            if (_showFooter)
            {
                bottomPanelY -= footerPanelHeight;
                botomspacetaken += footerPanelHeight;
                footerPanelRect = new Rectangle(drawingBounds.Left, bottomPanelY, drawingBounds.Width, footerPanelHeight);
                DrawFooterRow(g, footerPanelRect);
            }

            if (_showaggregationRow)
            {
                bottomPanelY -= bottomagregationPanelHeight;
                botomspacetaken += bottomagregationPanelHeight;
                bottomagregationPanelRect = new Rectangle(drawingBounds.Left, bottomPanelY, drawingBounds.Width, bottomagregationPanelHeight);
                DrawBottomAggregationRow(g, bottomagregationPanelRect);
            }

            // Draw top panels (from top to bottom)
            if (_showFilterpanel)
            {
                filterPanelRect = new Rectangle(drawingBounds.Left, topPanelY, drawingBounds.Width, filterPanelHeight);
                DrawFilterPanel(g, filterPanelRect);
                topPanelY += filterPanelHeight + 10;
            }

            if (_showHeaderPanel)
            {
                int ttitleLabelHeight = headerPanelHeight;
                ttitleLabelHeight = titleLabel.GetPreferredSize(Size.Empty).Height;
                if (ttitleLabelHeight > headerPanelHeight)
                {
                    headerPanelHeight = ttitleLabelHeight;
                }
                headerPanelRect = new Rectangle(drawingBounds.Left, topPanelY, drawingBounds.Width, headerPanelHeight);
                DrawHeaderPanel(g, headerPanelRect);
                topPanelY += headerPanelHeight;
            }

            if (_showColumnHeaders)
            {
                columnsheaderPanelRect = new Rectangle(drawingBounds.Left, topPanelY, drawingBounds.Width, ColumnHeaderHeight);
                PaintColumnHeaders(g, columnsheaderPanelRect);
                topPanelY += ColumnHeaderHeight;
            }

            // Calculate grid area (remaining space after all panels)
            int availableHeight = drawingBounds.Height - topPanelY - botomspacetaken;
            int availableWidth = drawingBounds.Width - (_verticalScrollBar.Visible ? _verticalScrollBar.Width : 0);
            gridRect = new Rectangle(drawingBounds.Left, topPanelY, availableWidth, availableHeight);

            // Draw grid content
            PaintRows(g, gridRect);

            // Draw grid lines if enabled
            if (_showverticalgridlines)
            {
                DrawColumnBorders(g, gridRect);
            }
            if (_showhorizontalgridlines)
            {
                DrawRowsBorders(g, gridRect);
            }

            PositionScrollBars();
        }

        /// <summary>
        /// Recalculates all panel rectangles based on current visibility settings
        /// Called when layout changes (resize, property changes, etc.)
        /// </summary>
        private void RecalculateGridRect()
        {
            UpdateDrawingRect();

            var drawingBounds = DrawingRect;

            // Update scrollbar visibility first
            UpdateScrollBars();

            // Calculate bottom panel positions
            bottomPanelY = drawingBounds.Bottom;
            botomspacetaken = 0;
            topPanelY = drawingBounds.Top;

            // Reserve space for horizontal scrollbar if visible
            if (_horizontalScrollBar.Visible)
            {
                bottomPanelY -= _horizontalScrollBar.Height;
                botomspacetaken += _horizontalScrollBar.Height;
            }

            if (_showNavigator)
            {
                bottomPanelY -= navigatorPanelHeight;
                botomspacetaken += navigatorPanelHeight;
            }

            _navigatorDrawn = true;
            if (_showNavigator)
            {
                navigatorPanelRect = new Rectangle(drawingBounds.Left, drawingBounds.Bottom - navigatorPanelHeight, drawingBounds.Width, navigatorPanelHeight);
            }
            else
            {
                navigatorPanelRect = new Rectangle(-100, -100, drawingBounds.Width, navigatorPanelHeight);
            }

            if (_showFooter)
            {
                bottomPanelY -= footerPanelHeight;
                botomspacetaken += footerPanelHeight;
                footerPanelRect = new Rectangle(drawingBounds.Left, bottomPanelY, drawingBounds.Width, footerPanelHeight);
            }

            if (_showaggregationRow)
            {
                bottomPanelY -= bottomagregationPanelHeight;
                botomspacetaken += bottomagregationPanelHeight;
                bottomagregationPanelRect = new Rectangle(drawingBounds.Left, bottomPanelY, drawingBounds.Width, bottomagregationPanelHeight);
            }

            // Calculate top panel positions
            if (!_filterpaneldrawn)
            {
                _filterpaneldrawn = true;
                if (_showFilterpanel)
                {
                    filterPanelRect = new Rectangle(drawingBounds.Left, topPanelY, drawingBounds.Width, filterPanelHeight);
                }
                else
                {
                    filterPanelRect = new Rectangle(-100, -100, drawingBounds.Width, filterPanelHeight);
                }
            }
            if (_showFilterpanel)
            {
                topPanelY += filterPanelHeight + 10;
            }
            if (_showHeaderPanel)
            {
                int ttitleLabelHeight = headerPanelHeight;

                ttitleLabelHeight = titleLabel.GetPreferredSize(Size.Empty).Height;

                if (ttitleLabelHeight > headerPanelHeight)
                {
                    headerPanelHeight = ttitleLabelHeight;
                }

                headerPanelRect = new Rectangle(drawingBounds.Left, topPanelY, drawingBounds.Width, headerPanelHeight);
                topPanelY += headerPanelHeight;
            }
            else
            {
                headerPanelHeight = 0;
                headerPanelRect = new Rectangle(-100, -100, drawingBounds.Width, headerPanelHeight);
            }

            if (_showColumnHeaders)
            {
                columnsheaderPanelRect = new Rectangle(drawingBounds.Left, topPanelY, drawingBounds.Width, ColumnHeaderHeight);
                topPanelY += ColumnHeaderHeight;
            }

            // Grid occupies the remaining space
            int availableHeight = drawingBounds.Height - topPanelY - botomspacetaken;
            int availableWidth = drawingBounds.Width - (_verticalScrollBar.Visible ? _verticalScrollBar.Width : 0);
            gridRect = new Rectangle(drawingBounds.Left, topPanelY, availableWidth, availableHeight);
        }

        /// <summary>
        /// Initializes cached brushes and pens for painting performance
        /// </summary>
        private void InitializePaintingResources()
        {
            _cellBrush = new SolidBrush(_currentTheme.GridBackColor);
            _selectedCellBrush = new SolidBrush(_currentTheme.GridRowHoverBackColor);
            _borderPen = new Pen(_currentTheme.GridLineColor);
            _selectedBorderPen = new Pen(_currentTheme.PrimaryTextColor, 2);
        }

        /// <summary>
        /// Gets effective color considering black and white mode
        /// </summary>
        private Color GetEffectiveColor(Color originalColor)
        {
            if (!DrawInBlackAndWhite)
                return originalColor;

            int gray = (int)(0.3 * originalColor.R + 0.59 * originalColor.G + 0.11 * originalColor.B);
            return Color.FromArgb(originalColor.A, gray, gray, gray);
        }

        #endregion
    }
}
