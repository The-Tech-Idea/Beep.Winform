using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Winform.Controls.Trees.Models
{
    /// <summary>
    /// Configuration for a tree row, allowing per-row customization of height and appearance.
    /// </summary>
    public class BeepTreeRowConfig
    {
        #region Size & Layout

        /// <summary>
        /// Gets or sets the height of the row in pixels.
        /// Set to 0 or negative to use the default row height.
        /// </summary>
        public int Height { get; set; } = 0;

        /// <summary>
        /// Gets or sets the minimum height of the row.
        /// </summary>
        public int MinHeight { get; set; } = 0;

        /// <summary>
        /// Gets or sets the padding inside the row (left, top, right, bottom).
        /// </summary>
        public Padding Padding { get; set; } = new Padding(0);

        /// <summary>
        /// Gets or sets the margin outside the row.
        /// </summary>
        public Padding Margin { get; set; } = new Padding(0);

        /// <summary>
        /// Gets or sets the indent offset for this row's content.
        /// </summary>
        public int IndentOffset { get; set; } = 0;

        #endregion

        #region Background Colors

        /// <summary>
        /// Gets or sets the background color for this row.
        /// Set to Empty to use the default theme color.
        /// </summary>
        public Color BackColor { get; set; } = Color.Empty;

        /// <summary>
        /// Gets or sets the background color when the row is hovered.
        /// Set to Empty to use the default hover color.
        /// </summary>
        public Color HoverBackColor { get; set; } = Color.Empty;

        /// <summary>
        /// Gets or sets the background color when the row is selected.
        /// Set to Empty to use the default selection color.
        /// </summary>
        public Color SelectedBackColor { get; set; } = Color.Empty;

        /// <summary>
        /// Gets or sets the background color when the row is focused.
        /// Set to Empty to use the default focus color.
        /// </summary>
        public Color FocusedBackColor { get; set; } = Color.Empty;

        /// <summary>
        /// Gets or sets the gradient start color for the row background.
        /// Set to Empty to disable gradient.
        /// </summary>
        public Color GradientStartColor { get; set; } = Color.Empty;

        /// <summary>
        /// Gets or sets the gradient end color for the row background.
        /// </summary>
        public Color GradientEndColor { get; set; } = Color.Empty;

        /// <summary>
        /// Gets or sets the gradient direction.
        /// </summary>
        public LinearGradientMode GradientDirection { get; set; } = LinearGradientMode.Horizontal;

        #endregion

        #region Foreground & Text

        /// <summary>
        /// Gets or sets the foreground (text) color for this row.
        /// Set to Empty to use the default theme color.
        /// </summary>
        public Color ForeColor { get; set; } = Color.Empty;

        /// <summary>
        /// Gets or sets the foreground color when the row is hovered.
        /// </summary>
        public Color HoverForeColor { get; set; } = Color.Empty;

        /// <summary>
        /// Gets or sets the foreground color when the row is selected.
        /// </summary>
        public Color SelectedForeColor { get; set; } = Color.Empty;

        /// <summary>
        /// Gets or sets the font for this row.
        /// Set to null to use the default font.
        /// </summary>
        public Font Font { get; set; } = null;

        /// <summary>
        /// Gets or sets the font style for this row.
        /// Set to null to use the default style.
        /// </summary>
        public FontStyle? FontStyle { get; set; } = null;

        #endregion

        #region Border

        /// <summary>
        /// Gets or sets the border color for this row.
        /// Set to Empty to use the default border color.
        /// </summary>
        public Color BorderColor { get; set; } = Color.Empty;

        /// <summary>
        /// Gets or sets the border width.
        /// Set to 0 to disable border.
        /// </summary>
        public float BorderWidth { get; set; } = 0;

        /// <summary>
        /// Gets or sets the border style.
        /// </summary>
        public DashStyle BorderStyle { get; set; } = DashStyle.Solid;

        /// <summary>
        /// Gets or sets which sides have borders.
        /// </summary>
        public BorderSides BorderSides { get; set; } = BorderSides.All;

        /// <summary>
        /// Gets or sets the corner radius for rounded row backgrounds.
        /// Set to 0 for square corners.
        /// </summary>
        public int CornerRadius { get; set; } = 0;

        #endregion

        #region Selection & Focus

        /// <summary>
        /// Gets or sets the selection border color.
        /// </summary>
        public Color SelectionBorderColor { get; set; } = Color.Empty;

        /// <summary>
        /// Gets or sets the selection border width.
        /// </summary>
        public float SelectionBorderWidth { get; set; } = 0;

        /// <summary>
        /// Gets or sets the focus indicator color (dotted line).
        /// </summary>
        public Color FocusIndicatorColor { get; set; } = Color.Empty;

        #endregion

        #region Effects

        /// <summary>
        /// Gets or sets whether to draw a shadow under this row.
        /// </summary>
        public bool ShowShadow { get; set; } = false;

        /// <summary>
        /// Gets or sets the shadow color.
        /// </summary>
        public Color ShadowColor { get; set; } = Color.FromArgb(50, 0, 0, 0);

        /// <summary>
        /// Gets or sets the shadow offset.
        /// </summary>
        public Point ShadowOffset { get; set; } = new Point(2, 2);

        /// <summary>
        /// Gets or sets whether this row should have an underline.
        /// </summary>
        public bool Underline { get; set; } = false;

        /// <summary>
        /// Gets or sets the underline color.
        /// </summary>
        public Color UnderlineColor { get; set; } = Color.Empty;

        /// <summary>
        /// Gets or sets the underline width.
        /// </summary>
        public float UnderlineWidth { get; set; } = 1f;

        /// <summary>
        /// Gets or sets the opacity of the row (0-255).
        /// 255 = fully opaque.
        /// </summary>
        public byte Opacity { get; set; } = 255;

        #endregion

        #region State

        /// <summary>
        /// Gets or sets whether this row should be drawn with a different style.
        /// </summary>
        public bool UseCustomStyle { get; set; } = false;

        /// <summary>
        /// Gets or sets whether this row is read-only (cannot be edited).
        /// </summary>
        public bool ReadOnly { get; set; } = false;

        /// <summary>
        /// Gets or sets whether this row is selectable.
        /// </summary>
        public bool Selectable { get; set; } = true;

        /// <summary>
        /// Gets or sets whether this row can be expanded/collapsed.
        /// </summary>
        public bool Expandable { get; set; } = true;

        /// <summary>
        /// Gets or sets optional tag data for this row configuration.
        /// </summary>
        public object Tag { get; set; }

        #endregion
    }

    /// <summary>
    /// Configuration for a tree cell, allowing per-cell customization.
    /// </summary>
    public class BeepTreeCellConfig
    {
        #region Background Colors

        /// <summary>
        /// Gets or sets the background color for this cell.
        /// Set to Empty to use the default row/theme color.
        /// </summary>
        public Color BackColor { get; set; } = Color.Empty;

        /// <summary>
        /// Gets or sets the background color when the cell is hovered.
        /// </summary>
        public Color HoverBackColor { get; set; } = Color.Empty;

        /// <summary>
        /// Gets or sets the background color when the cell is selected.
        /// </summary>
        public Color SelectedBackColor { get; set; } = Color.Empty;

        /// <summary>
        /// Gets or sets the gradient start color for the cell background.
        /// </summary>
        public Color GradientStartColor { get; set; } = Color.Empty;

        /// <summary>
        /// Gets or sets the gradient end color for the cell background.
        /// </summary>
        public Color GradientEndColor { get; set; } = Color.Empty;

        /// <summary>
        /// Gets or sets the gradient direction.
        /// </summary>
        public LinearGradientMode GradientDirection { get; set; } = LinearGradientMode.Horizontal;

        #endregion

        #region Foreground & Text

        /// <summary>
        /// Gets or sets the foreground (text) color for this cell.
        /// Set to Empty to use the default row/theme color.
        /// </summary>
        public Color ForeColor { get; set; } = Color.Empty;

        /// <summary>
        /// Gets or sets the foreground color when the cell is hovered.
        /// </summary>
        public Color HoverForeColor { get; set; } = Color.Empty;

        /// <summary>
        /// Gets or sets the foreground color when the cell is selected.
        /// </summary>
        public Color SelectedForeColor { get; set; } = Color.Empty;

        /// <summary>
        /// Gets or sets the font for this cell.
        /// Set to null to use the default row/font.
        /// </summary>
        public Font Font { get; set; } = null;

        /// <summary>
        /// Gets or sets the font style for this cell.
        /// </summary>
        public FontStyle? FontStyle { get; set; } = null;

        /// <summary>
        /// Gets or sets the text alignment for this cell.
        /// Set to null to use the column default.
        /// </summary>
        public ContentAlignment? Alignment { get; set; } = null;

        /// <summary>
        /// Gets or sets the display text override for this cell.
        /// Set to null to use the default value from data binding.
        /// </summary>
        public string DisplayText { get; set; } = null;

        /// <summary>
        /// Gets or sets the text format flags for this cell.
        /// </summary>
        public TextFormatFlags TextFormatFlags { get; set; } = TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix | TextFormatFlags.EndEllipsis;

        #endregion

        #region Border

        /// <summary>
        /// Gets or sets the border color for this cell.
        /// </summary>
        public Color BorderColor { get; set; } = Color.Empty;

        /// <summary>
        /// Gets or sets the border width.
        /// </summary>
        public float BorderWidth { get; set; } = 0;

        /// <summary>
        /// Gets or sets which sides have borders.
        /// </summary>
        public BorderSides BorderSides { get; set; } = BorderSides.All;

        /// <summary>
        /// Gets or sets the corner radius for the cell.
        /// </summary>
        public int CornerRadius { get; set; } = 0;

        #endregion

        #region Image & Icon

        /// <summary>
        /// Gets or sets an image to display in the cell.
        /// </summary>
        public string ImagePath { get; set; } = null;

        /// <summary>
        /// Gets or sets the image size.
        /// </summary>
        public Size? ImageSize { get; set; } = null;

        /// <summary>
        /// Gets or sets the image alignment.
        /// </summary>
        public ContentAlignment? ImageAlignment { get; set; } = null;

        /// <summary>
        /// Gets or sets the icon color (for SVG or themed icons).
        /// </summary>
        public Color IconColor { get; set; } = Color.Empty;

        #endregion

        #region Effects

        /// <summary>
        /// Gets or sets whether to draw a shadow under this cell.
        /// </summary>
        public bool ShowShadow { get; set; } = false;

        /// <summary>
        /// Gets or sets the shadow color.
        /// </summary>
        public Color ShadowColor { get; set; } = Color.FromArgb(50, 0, 0, 0);

        /// <summary>
        /// Gets or sets the shadow offset.
        /// </summary>
        public Point ShadowOffset { get; set; } = new Point(1, 1);

        /// <summary>
        /// Gets or sets the opacity of the cell (0-255).
        /// </summary>
        public byte Opacity { get; set; } = 255;

        #endregion

        #region State

        /// <summary>
        /// Gets or sets whether this cell should be drawn with a different style.
        /// </summary>
        public bool UseCustomStyle { get; set; } = false;

        /// <summary>
        /// Gets or sets whether this cell is read-only.
        /// </summary>
        public bool ReadOnly { get; set; } = false;

        /// <summary>
        /// Gets or sets the tooltip text for this cell.
        /// </summary>
        public string ToolTipText { get; set; } = null;

        /// <summary>
        /// Gets or sets optional tag data for this cell configuration.
        /// </summary>
        public object Tag { get; set; }

        #endregion
    }

    /// <summary>
    /// Specifies which sides of a control have borders.
    /// </summary>
    [Flags]
    public enum BorderSides
    {
        None = 0,
        Left = 1,
        Top = 2,
        Right = 4,
        Bottom = 8,
        All = Left | Top | Right | Bottom
    }
}
