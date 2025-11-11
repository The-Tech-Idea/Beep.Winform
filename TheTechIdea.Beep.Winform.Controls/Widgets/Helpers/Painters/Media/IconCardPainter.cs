using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.BaseImage;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Media
{
	/// <summary>
	/// IconCard - Large icon with label/description (now with card/badge interactions)
	/// </summary>
	internal sealed class IconCardPainter : WidgetPainterBase, IDisposable
	{
		private ImagePainter _iconPainter;
		private Rectangle _cardRectCache;
		private Rectangle _badgeRectCache;

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

			// Cache card rect and badge rect
			_cardRectCache = ctx.DrawingRect;
			_badgeRectCache = new Rectangle(ctx.IconRect.Right - 12, ctx.IconRect.Top - 4, 16, 16);
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
				_iconPainter.CurrentTheme = Theme;
			}

			// Draw icon background circle
			using var iconBgBrush = new SolidBrush(Color.FromArgb(20, ctx.AccentColor));
			g.FillEllipse(iconBgBrush, ctx.IconRect);

			// Draw icon using ImagePainter
			DrawIconWithImagePainter(g, ctx.IconRect, ctx);

			// Title
			if (!string.IsNullOrEmpty(ctx.Title))
			{
				using var titleFont = new Font(Owner.Font.FontFamily, 10f, FontStyle.Bold);
				using var titleBrush = new SolidBrush(Color.FromArgb(180, Color.Black));
				var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
				g.DrawString(ctx.Title, titleFont, titleBrush, ctx.HeaderRect, format);
			}

			// Subtitle
			if (!string.IsNullOrEmpty(ctx.Value))
			{
				using var subtitleFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);
				using var subtitleBrush = new SolidBrush(Color.FromArgb(120, Color.Black));
				var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
				g.DrawString(ctx.Value, subtitleFont, subtitleBrush, ctx.ContentRect, format);
			}

			// Optional badge
			bool showBadge = ctx.ShowBadge;
			if (showBadge)
			{
				using var badgeBrush = new SolidBrush(Color.FromArgb(200, ctx.AccentColor));
				g.FillEllipse(badgeBrush, _badgeRectCache);
				using var starFont = new Font("Segoe UI Symbol", 8, FontStyle.Bold);
				using var starBrush = new SolidBrush(Color.White);
				var sz = TextUtils.MeasureText(g,"★", starFont);
				g.DrawString("★", starFont, starBrush, _badgeRectCache.X + (_badgeRectCache.Width - sz.Width) / 2, _badgeRectCache.Y + (_badgeRectCache.Height - sz.Height) / 2);
			}
		}

		private void DrawIconWithImagePainter(Graphics g, Rectangle rect, WidgetContext ctx)
		{
			if (!string.IsNullOrEmpty(ctx.IconPath))
			{
				try
				{
					_iconPainter.DrawImage(g, ctx.IconPath, rect);
					return;
				}
				catch { }
			}
			// Placeholder
			using var iconPen = new Pen(ctx.AccentColor, 3);
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
			// Card hover
			if (IsAreaHovered("IconCard_Card"))
			{
				using var pen = new Pen(Color.FromArgb(120, Theme?.PrimaryColor ?? Color.Blue), 2);
				using var path = CreateRoundedPath(_cardRectCache, ctx.CornerRadius);
				g.DrawPath(pen, path);
			}
			// Badge hover
			if (IsAreaHovered("IconCard_Badge"))
			{
				using var glow = new SolidBrush(Color.FromArgb(40, Theme?.AccentColor ?? Color.Blue));
				g.FillEllipse(glow, Rectangle.Inflate(_badgeRectCache, 3, 3));
			}
		}

		public override void UpdateHitAreas(BaseControl owner, WidgetContext ctx, Action<string, Rectangle>? notifyAreaHit)
		{
			if (owner == null) return;
			ClearOwnerHitAreas();
			owner.AddHitArea("IconCard_Card", _cardRectCache, null, () =>
			{
				ctx.IconCardClicked = true;
				notifyAreaHit?.Invoke("IconCard_Card", _cardRectCache);
				Owner?.Invalidate();
			});
			bool showBadge = ctx.ShowBadge;
			if (showBadge)
			{
				owner.AddHitArea("IconCard_Badge", _badgeRectCache, null, () =>
				{
					ctx.IconCardBadgeClicked = true;
					notifyAreaHit?.Invoke("IconCard_Badge", _badgeRectCache);
					Owner?.Invalidate();
				});
			}
		}

		public void Dispose()
		{
			_iconPainter?.Dispose();
		}
	}
}
