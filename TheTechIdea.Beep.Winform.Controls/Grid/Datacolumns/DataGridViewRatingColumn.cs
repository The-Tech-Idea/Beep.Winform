using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Grid.Datacolumns;

[ToolboxItem(false)]
public class BeepDataGridViewRatingColumn : DataGridViewColumn
{
    public BeepDataGridViewRatingColumn()
        : base(new DataGridViewRatingCell()) // Set the cell template to our custom rating cell
    {
    }

    // Clone method for copying the column settings
    public override object Clone()
    {
        BeepDataGridViewRatingColumn clone = (BeepDataGridViewRatingColumn)base.Clone();
        return clone;
    }

    // Property for setting the filled star color at the column level
    public Color FilledStarColor
    {
        get
        {
            if (this.CellTemplate is DataGridViewRatingCell cell)
            {
                return cell.FilledStarColor;
            }
            return Color.Gold;
        }
        set
        {
            if (this.CellTemplate is DataGridViewRatingCell cell)
            {
                cell.FilledStarColor = value;
            }

            // Apply the color to existing cells
            if (this.DataGridView != null)
            {
                foreach (DataGridViewRow row in this.DataGridView.Rows)
                {
                    if (row.Cells[this.Index] is DataGridViewRatingCell ratingCell)
                    {
                        ratingCell.FilledStarColor = value;
                    }
                }
            }
        }
    }

    // Property for setting the empty star color at the column level
    public Color EmptyStarColor
    {
        get
        {
            if (this.CellTemplate is DataGridViewRatingCell cell)
            {
                return cell.EmptyStarColor;
            }
            return Color.LightGray;
        }
        set
        {
            if (this.CellTemplate is DataGridViewRatingCell cell)
            {
                cell.EmptyStarColor = value;
            }

            // Apply the color to existing cells
            if (this.DataGridView != null)
            {
                foreach (DataGridViewRow row in this.DataGridView.Rows)
                {
                    if (row.Cells[this.Index] is DataGridViewRatingCell ratingCell)
                    {
                        ratingCell.EmptyStarColor = value;
                    }
                }
            }
        }
    }

    // Property to allow setting the maximum stars in the column
    public int MaxStars
    {
        get
        {
            if (this.CellTemplate is DataGridViewRatingCell cell)
            {
                return cell.MaxStars;
            }
            return 5; // Default
        }
        set
        {
            if (this.CellTemplate is DataGridViewRatingCell cell)
            {
                cell.MaxStars = value;
            }

            // Apply the max stars to existing cells
            if (this.DataGridView != null)
            {
                foreach (DataGridViewRow row in this.DataGridView.Rows)
                {
                    if (row.Cells[this.Index] is DataGridViewRatingCell ratingCell)
                    {
                        ratingCell.MaxStars = value;
                    }
                }
            }
        }
    }
}
public class DataGridViewRatingCell : DataGridViewTextBoxCell
{
    private int maxStars = 5;      // Maximum stars in the rating, now adjustable
    private const int StarSize = 16;      // Size of each star in pixels
    private Color filledStarColor = Color.Gold;  // Default color for filled stars
    private Color emptyStarColor = Color.LightGray;  // Default color for empty stars

    public DataGridViewRatingCell()
    {
        this.ValueType = typeof(int);  // The cell value is expected to be an integer rating between 0 and maxStars.
    }

    public override Type EditType => null;  // No editing control

    public override object DefaultNewRowValue => 0;  // Default rating is 0

    // Set custom colors for the filled and empty stars
    public Color FilledStarColor
    {
        get => filledStarColor;
        set => filledStarColor = value;
    }

    public Color EmptyStarColor
    {
        get => emptyStarColor;
        set => emptyStarColor = value;
    }

    // Property to allow dynamic setting of MaxStars
    public int MaxStars
    {
        get => maxStars;
        set
        {
            if (value > 0)
            {
                maxStars = value;
            }
        }
    }

    // Draw stars programmatically using Graphics
    protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex,
                                  DataGridViewElementStates cellState, object value, object formattedValue,
                                  string errorText, DataGridViewCellStyle cellStyle,
                                  DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
    {
        base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, null, null, errorText, cellStyle, advancedBorderStyle, (paintParts & ~DataGridViewPaintParts.ContentForeground));

        // Convert cell value to an integer rating
        int rating = (value != null) ? Convert.ToInt32(value) : 0;

        // Calculate the position to start drawing stars
        int starX = cellBounds.X + (cellBounds.Width - (MaxStars * StarSize)) / 2;
        int starY = cellBounds.Y + (cellBounds.Height - StarSize) / 2;

        using (Brush filledBrush = new SolidBrush(filledStarColor))
        using (Brush emptyBrush = new SolidBrush(emptyStarColor))
        {
            for (int i = 0; i < MaxStars; i++)
            {
                // Draw a filled or empty star depending on the rating
                DrawStar(graphics, (i < rating) ? filledBrush : emptyBrush, starX + (i * StarSize), starY);
            }
        }
    }

    // Helper method to draw a star shape
    private void DrawStar(Graphics graphics, Brush brush, int x, int y)
    {
        PointF[] starPoints = new PointF[10];
        double angle = Math.PI / 5;  // 36 degrees in radians

        for (int i = 0; i < 10; i++)
        {
            double radius = (i % 2 == 0) ? StarSize / 2.0 : StarSize / 4.0;  // Alternate between outer and inner points
            double theta = i * angle;
            starPoints[i] = new PointF(
                x + StarSize / 2 + (float)(radius * Math.Sin(theta)),
                y + StarSize / 2 - (float)(radius * Math.Cos(theta))
            );
        }

        graphics.FillPolygon(brush, starPoints);
    }

    // Handle mouse click to change the rating
    protected override void OnMouseClick(DataGridViewCellMouseEventArgs e)
    {
        base.OnMouseClick(e);

        if (e.Button == MouseButtons.Left)
        {
            int clickedStar = (e.X - this.DataGridView.GetCellDisplayRectangle(this.ColumnIndex, e.RowIndex, false).X) / StarSize;
            clickedStar = Math.Max(0, Math.Min(clickedStar, MaxStars - 1));  // Ensure rating stays within bounds

            // Set the new rating based on the clicked position
            this.Value = clickedStar + 1;
            this.DataGridView.NotifyCurrentCellDirty(true);
        }
    }
}

