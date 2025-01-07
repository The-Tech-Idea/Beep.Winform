using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Desktop.Common.Printer
{
    /// <summary>
    /// Class for the owner-drawn event. Provides the caller with the cell data, the current
    /// graphics context, and the location in which to draw the cell.
    /// </summary>
    public class DGVCellDrawingEventArgs : EventArgs
    {
        public Graphics GraphicsContext { get; }
        public RectangleF DrawingBounds { get; }
        public DataGridViewCellStyle CellStyle { get; }
        public int RowIndex { get; }
        public int ColumnIndex { get; }
        public bool Handled { get; set; }

        public DGVCellDrawingEventArgs(Graphics g, RectangleF bounds, DataGridViewCellStyle style,
            int row, int column)
        {
            GraphicsContext = g;
            DrawingBounds = bounds;
            CellStyle = style;
            RowIndex = row;
            ColumnIndex = column;
            Handled = false;
        }
    }

    /// <summary>
    /// Delegate for owner-drawn cells - allows the caller to provide custom drawing for the cell.
    /// </summary>
    public delegate void CellOwnerDrawEventHandler(object sender, DGVCellDrawingEventArgs e);

    /// <summary>
    /// Class to hold extension methods for BeepPrinter.
    /// </summary>
    public static class BeepPrinterExtensions
    {
        /// <summary>
        /// Extension method to draw all the "EmbeddedImages" in a provided list.
        /// </summary>
        /// <typeparam name="T">Type of the embedded image.</typeparam>
        /// <param name="list">List of embedded images.</param>
        /// <param name="g">Graphics context.</param>
        /// <param name="pageWidth">Width of the page.</param>
        /// <param name="pageHeight">Height of the page.</param>
        /// <param name="margins">Page margins.</param>
        public static void DrawEmbeddedImages<T>(this IEnumerable<T> list,
            Graphics g, int pageWidth, int pageHeight, Margins margins)
        {
            foreach (var item in list)
            {
                if (item is EmbeddedImage embeddedImage)
                {
                    var destRect = new Rectangle(embeddedImage.UpperLeft(pageWidth, pageHeight, margins),
                        new Size(embeddedImage.TheImage.Width, embeddedImage.TheImage.Height));
                    g.DrawImage(embeddedImage.TheImage, destRect);
                }
            }
        }
    }
    /// <summary>
    /// Represents an embedded image with positioning information.
    /// </summary>
    public class EmbeddedImage
    {
        public Image TheImage { get; set; }
        public Alignment ImageAlignment { get; set; }
        public Location ImageLocation { get; set; }
        public int ImageX { get; set; }
        public int ImageY { get; set; }

        /// <summary>
        /// Calculates the upper-left point for the image based on alignment and location.
        /// </summary>
        public Point UpperLeft(int pageWidth, int pageHeight, Margins margins)
        {
            int y = 0;
            int x = 0;

            // Absolute positioning
            if (ImageLocation == Location.Absolute)
                return new Point(ImageX, ImageY);

            // Header or Footer positioning
            switch (ImageLocation)
            {
                case Location.Header:
                    y = margins.Top;
                    break;
                case Location.Footer:
                    y = pageHeight - TheImage.Height - margins.Bottom;
                    break;
                default:
                    throw new ArgumentException($"Unknown value: {ImageLocation}");
            }

            // Horizontal alignment
            switch (ImageAlignment)
            {
                case Alignment.Left:
                    x = margins.Left;
                    break;
                case Alignment.Center:
                    x = (pageWidth / 2) - (TheImage.Width / 2) + margins.Left;
                    break;
                case Alignment.Right:
                    x = pageWidth - TheImage.Width - margins.Right;
                    break;
                case Alignment.NotSet:
                    x = ImageX;
                    break;
                default:
                    throw new ArgumentException($"Unknown value: {ImageAlignment}");
            }

            return new Point(x, y);
        }
    }
    /// <summary>
    /// Specifies the alignment options for embedded images.
    /// </summary>
    public enum Alignment
    {
        Left,
        Center,
        Right,
        Top,
        Middle,
        Bottom,
        NotSet
    }

    /// <summary>
    /// Specifies the location where an embedded image should appear.
    /// </summary>
    public enum Location
    {
        Header,
        Footer,
        Absolute
    }
    /// <summary>
    /// Represents a page definition for handling multi-page printing.
    /// </summary>
    public class PageDef
    {
        public Margins Margins { get; set; }
        public List<int> ColumnIndices { get; set; } = new List<int>();
        public List<DataGridViewColumn> ColumnsToPrint { get; set; } = new List<DataGridViewColumn>();
        public List<float> ColumnWidths { get; set; } = new List<float>();
        public List<float> ColumnWidthOverrides { get; set; } = new List<float>();
        public float TotalColumnWidth { get; set; } = 0;
        public float PrintWidth { get; set; } = 0;
    }

    /// <summary>
    /// Represents data associated with a row for printing purposes.
    /// </summary>
    internal class RowData
    {
        public DataGridViewRow Row { get; set; }
        public float Height { get; set; }
        public bool PageBreak { get; set; } = false;
        public bool SplitRow { get; set; } = false;
    }
}
