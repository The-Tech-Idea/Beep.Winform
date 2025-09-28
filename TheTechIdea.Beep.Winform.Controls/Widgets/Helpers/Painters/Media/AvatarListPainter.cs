using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.BaseImage;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Media
{
	/// <summary>
	/// AvatarList - Vertical list of avatars with names and status
	/// </summary>
	internal sealed class AvatarListPainter : WidgetPainterBase, IDisposable
	{
		private ImagePainter _avatarPainter;

		public AvatarListPainter()
		{
			_avatarPainter = new ImagePainter();
			_avatarPainter.ScaleMode = ImageScaleMode.Fill;
			_avatarPainter.ClipShape = ImageClipShape.Circle;
		}

		public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
		{
			int pad = 12;
			ctx.DrawingRect = Rectangle.Inflate(drawingRect, -4, -4);
			
			// Title area
			if (!string.IsNullOrEmpty(ctx.Title))
			{
				ctx.HeaderRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, ctx.DrawingRect.Width - pad * 2, 24);
			}
			
			// List area
			int contentTop = ctx.HeaderRect.IsEmpty ? ctx.DrawingRect.Top + pad : ctx.HeaderRect.Bottom + 8;
			ctx.ContentRect = new Rectangle(
				ctx.DrawingRect.Left + pad,
				contentTop,
				ctx.DrawingRect.Width - pad * 2,
				ctx.DrawingRect.Bottom - contentTop - pad
			);
			
			return ctx;
		}

		public override void DrawBackground(Graphics g, WidgetContext ctx)
		{
			using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.White);
			using var bgPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
			g.FillPath(bgBrush, bgPath);
			
			using var borderPen = new Pen(Theme?.BorderColor ?? Color.LightGray, 1);
			g.DrawPath(borderPen, bgPath);
		}

		public override void DrawContent(Graphics g, WidgetContext ctx)
		{
			// Update theme configuration
			if (Theme != null)
			{
				_avatarPainter.Theme = Theme;
			}

			// Draw title
			if (!string.IsNullOrEmpty(ctx.Title))
			{
				using var titleFont = new Font(Owner.Font.FontFamily, 10f, FontStyle.Bold);
				using var titleBrush = new SolidBrush(Theme?.ForeColor ?? Color.Black);
				var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
				g.DrawString(ctx.Title, titleFont, titleBrush, ctx.HeaderRect, format);
			}

			// Draw avatar list
			DrawAvatarList(g, ctx.ContentRect, ctx);
		}

		private void DrawAvatarList(Graphics g, Rectangle area, WidgetContext ctx)
		{
			int avatarSize = 40;
			int itemHeight = avatarSize + 8;
			int maxItems = area.Height / itemHeight;
			
			var users = new[] 
			{
				new { Name = "Alice Johnson", Status = "Online", Role = "Designer" },
				new { Name = "Bob Smith", Status = "Away", Role = "Developer" },
				new { Name = "Carol Davis", Status = "Online", Role = "Manager" },
				new { Name = "David Wilson", Status = "Offline", Role = "QA Tester" },
				new { Name = "Eve Brown", Status = "Online", Role = "DevOps" },
				new { Name = "Frank Miller", Status = "Away", Role = "Analyst" }
			};
			
			int itemsToShow = Math.Min(maxItems, users.Length);
			
			for (int i = 0; i < itemsToShow; i++)
			{
				var user = users[i];
				var itemRect = new Rectangle(
					area.X,
					area.Y + i * itemHeight,
					area.Width,
					itemHeight - 4
				);
				
				// Avatar rectangle
				var avatarRect = new Rectangle(
					itemRect.X,
					itemRect.Y + (itemRect.Height - avatarSize) / 2,
					avatarSize,
					avatarSize
				);
				
				// Draw avatar
				DrawAvatarWithImagePainter(g, avatarRect, ctx, user.Status);
				
				// Text areas
				var nameRect = new Rectangle(
					avatarRect.Right + 12,
					itemRect.Y,
					itemRect.Width - avatarRect.Width - 16,
					itemRect.Height / 2
				);
				
				var roleRect = new Rectangle(
					avatarRect.Right + 12,
					itemRect.Y + itemRect.Height / 2,
					itemRect.Width - avatarRect.Width - 16,
					itemRect.Height / 2
				);
				
				// Draw name
				using var nameFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Bold);
				using var nameBrush = new SolidBrush(Theme?.ForeColor ?? Color.Black);
				var nameFormat = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
				g.DrawString(user.Name, nameFont, nameBrush, nameRect, nameFormat);
				
				// Draw role
				using var roleFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);
				using var roleBrush = new SolidBrush(Color.FromArgb(120, Theme?.ForeColor ?? Color.Black));
				var roleFormat = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
				g.DrawString(user.Role, roleFont, roleBrush, roleRect, roleFormat);
				
				// Draw status indicator
				DrawStatusIndicator(g, avatarRect, user.Status);
			}
		}

		private void DrawAvatarWithImagePainter(Graphics g, Rectangle rect, WidgetContext ctx, string status)
		{
			// Try to draw custom avatar using ImagePainter
			bool avatarDrawn = false;
			if (!string.IsNullOrEmpty(ctx.ImagePath))
			{
				try
				{
					_avatarPainter.DrawImage(g, ctx.ImagePath, rect);
					avatarDrawn = true;
				}
				catch { }
			}
			
			// Draw placeholder avatar if no custom image
			if (!avatarDrawn)
			{
				DrawPlaceholderAvatar(g, rect, ctx.AccentColor);
			}
		}

		private void DrawPlaceholderAvatar(Graphics g, Rectangle rect, Color color)
		{
			// Draw gradient background
			using var gradientBrush = new LinearGradientBrush(
				rect, 
				Color.FromArgb(120, color), 
				Color.FromArgb(60, color), 
				LinearGradientMode.Vertical);
			
			g.FillEllipse(gradientBrush, rect);
			
			// Draw simple person icon
			using var personPen = new Pen(Color.FromArgb(180, Color.White), 2);
			
			// Head circle
			var headSize = rect.Width / 3;
			var headRect = new Rectangle(
				rect.X + (rect.Width - headSize) / 2,
				rect.Y + rect.Height / 4,
				headSize,
				headSize
			);
			g.DrawEllipse(personPen, headRect);
			
			// Body arc
			var bodyRect = new Rectangle(
				rect.X + rect.Width / 6,
				rect.Y + rect.Height * 2 / 3,
				rect.Width * 2 / 3,
				rect.Height / 3
			);
			g.DrawArc(personPen, bodyRect, 0, 180);
		}

		private void DrawStatusIndicator(Graphics g, Rectangle avatarRect, string status)
		{
			var statusColor = status switch
			{
				"Online" => Color.LimeGreen,
				"Away" => Color.Orange,
				"Offline" => Color.Gray,
				_ => Color.Gray
			};
			
			var statusRect = new Rectangle(
				avatarRect.Right - 10, 
				avatarRect.Bottom - 10, 
				8, 8
			);
			
			using var statusBrush = new SolidBrush(statusColor);
			using var statusBorder = new Pen(Theme?.BackColor ?? Color.White, 2);
			g.FillEllipse(statusBrush, statusRect);
			g.DrawEllipse(statusBorder, statusRect);
		}

		public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
		{
			// Optional: Draw scroll indicators if list is too long
			// or selection highlights
		}

		public void Dispose()
		{
			_avatarPainter?.Dispose();
		}
	}
}
