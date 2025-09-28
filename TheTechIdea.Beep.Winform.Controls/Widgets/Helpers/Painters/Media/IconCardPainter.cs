using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.BaseImage;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Media
{

	/// <summary>
	/// IconCard - Large icon with label/description
	/// </summary>
	internal sealed class IconCardPainter : WidgetPainterBase, IDisposable
	{
		private ImagePainter _iconPainter;

		public IconCardPainter()
		{
			_iconPainter = new ImagePainter();
			_iconPainter.ScaleMode = ImageScaleMode.KeepAspectRatio;
			_iconPainter.ClipShape = ImageClipShape.None; // Icons usually don't need clipping
		}
		public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
		{
			int pad = 16;
			ctx.DrawingRect = Rectangle.Inflate(drawingRect, -4, -4);

			// Icon area (top portion)
			int iconSize = Math.Min(64, ctx.DrawingRect.Width / 3);
			ctx.IconRect = new Rectangle(
				ctx.DrawingRect.Left + (ctx.DrawingRect.Width - iconSize) / 2,
				ctx.DrawingRect.Top + pad,
				iconSize,
				iconSize
			);

			// Title area
			ctx.HeaderRect = new Rectangle(
				ctx.DrawingRect.Left + pad,
				ctx.IconRect.Bottom + 8,
				ctx.DrawingRect.Width - pad * 2,
				20
			);

			// Subtitle area
			ctx.ContentRect = new Rectangle(
				ctx.DrawingRect.Left + pad,
				ctx.HeaderRect.Bottom + 4,
				ctx.DrawingRect.Width - pad * 2,
				ctx.DrawingRect.Bottom - ctx.HeaderRect.Bottom - pad
			);

			return ctx;
		}

		public override void DrawBackground(Graphics g, WidgetContext ctx)
		{
			DrawSoftShadow(g, ctx.DrawingRect, 8, layers: 3, offset: 2);
			using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.White);
			using var bgPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
			g.FillPath(bgBrush, bgPath);
		}

		public override void DrawContent(Graphics g, WidgetContext ctx)
		{
			// Update theme configuration
			if (Theme != null)
			{
				_iconPainter.Theme = Theme;
			}

			// Draw icon background circle
			using var iconBgBrush = new SolidBrush(Color.FromArgb(20, ctx.AccentColor));
			g.FillEllipse(iconBgBrush, ctx.IconRect);

			// Draw icon using ImagePainter
			DrawIconWithImagePainter(g, ctx.IconRect, ctx);

			// Draw title
			if (!string.IsNullOrEmpty(ctx.Title))
			{
				using var titleFont = new Font(Owner.Font.FontFamily, 10f, FontStyle.Bold);
				using var titleBrush = new SolidBrush(Color.FromArgb(180, Color.Black));
				var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
				g.DrawString(ctx.Title, titleFont, titleBrush, ctx.HeaderRect, format);
			}

			// Draw subtitle
			if (!string.IsNullOrEmpty(ctx.Value))
			{
				using var subtitleFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);
				using var subtitleBrush = new SolidBrush(Color.FromArgb(120, Color.Black));
				var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
				g.DrawString(ctx.Value, subtitleFont, subtitleBrush, ctx.ContentRect, format);
			}
		}

		private void DrawIconWithImagePainter(Graphics g, Rectangle rect, WidgetContext ctx)
		{
			// Try to draw custom icon if IconPath is provided
			if (!string.IsNullOrEmpty(ctx.IconPath))
			{
				try
				{
					_iconPainter.DrawImage(g, ctx.IconPath, rect);
					return;
				}
				catch
				{
					// Fall back to placeholder if icon fails to load
				}
			}

			// Draw placeholder icon using traditional method as fallback
			using var iconPen = new Pen(ctx.AccentColor, 3);

			// Draw a simple house icon as placeholder
			var points = new Point[]
			{
				new Point(rect.X + rect.Width / 2, rect.Y + 8),
				new Point(rect.X + 8, rect.Y + rect.Height / 2),
				new Point(rect.X + rect.Width / 4, rect.Y + rect.Height / 2),
				new Point(rect.X + rect.Width / 4, rect.Bottom - 8),
				new Point(rect.Right - rect.Width / 4, rect.Bottom - 8),
				new Point(rect.Right - rect.Width / 4, rect.Y + rect.Height / 2),
				new Point(rect.Right - 8, rect.Y + rect.Height / 2)
			};

			g.DrawLines(iconPen, points);
		}

		public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
		{
			// Optional: Draw badges or status indicators
		}

		public void Dispose()
		{
			_iconPainter?.Dispose();
		}
	}
}
