using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.BaseImage;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using TheTechIdea.Beep.Winform.Controls.Widgets;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Media
{
	/// <summary>
	/// AvatarList - Vertical list of avatars with names and status (now with interactions)
	/// </summary>
	internal sealed class AvatarListPainter : WidgetPainterBase, IDisposable
	{
		private ImagePainter _avatarPainter;
		private readonly List<Rectangle> _itemRects = new();
		private readonly List<Rectangle> _avatarRects = new();
		private readonly List<Rectangle> _statusRects = new();

		public AvatarListPainter()
		{
			_avatarPainter = new ImagePainter();
			_avatarPainter.ScaleMode = Vis.Modules.ImageScaleMode.Fill;
			_avatarPainter.ClipShape = Vis.Modules.ImageClipShape.Circle;
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

			// Precompute item/avatar/status rects for hit-testing
			_itemRects.Clear();
			_avatarRects.Clear();
			_statusRects.Clear();

			int avatarSize = 40;
			int itemHeight = avatarSize + 8;
			int maxItems = Math.Max(0, ctx.ContentRect.Height / itemHeight);
			int dataCount = ctx.SocialItems != null && ctx.SocialItems.Count > 0 ? ctx.SocialItems.Count : 6;
			int itemsToShow = Math.Min(maxItems, dataCount);
			for (int i = 0; i < itemsToShow; i++)
			{
				var itemRect = new Rectangle(
					ctx.ContentRect.X,
					ctx.ContentRect.Y + i * itemHeight,
					ctx.ContentRect.Width,
					itemHeight - 4
				);
				_itemRects.Add(itemRect);
				var avatarRect = new Rectangle(
					itemRect.X,
					itemRect.Y + (itemRect.Height - avatarSize) / 2,
					avatarSize,
					avatarSize
				);
				_avatarRects.Add(avatarRect);
				_statusRects.Add(new Rectangle(avatarRect.Right - 10, avatarRect.Bottom - 10, 8, 8));
			}
			
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
				_avatarPainter.CurrentTheme = Theme;
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
			int maxItems = Math.Max(0, area.Height / itemHeight);
			
			var sampleUsers = new[] 
			{
				new { Name = "Alice Johnson", Status = "Online", Role = "Designer" },
				new { Name = "Bob Smith", Status = "Away", Role = "Developer" },
				new { Name = "Carol Davis", Status = "Online", Role = "Manager" },
				new { Name = "David Wilson", Status = "Offline", Role = "QA Tester" },
				new { Name = "Eve Brown", Status = "Online", Role = "DevOps" },
				new { Name = "Frank Miller", Status = "Away", Role = "Analyst" }
			};
            var users = (ctx.SocialItems != null && ctx.SocialItems.Count > 0)
                ? ctx.SocialItems.Select(x => new { Name = x.Name, Status = x.Status, Role = x.Role }).ToArray()
                : sampleUsers;
			
			int itemsToShow = Math.Min(maxItems, users.Length);
			
			for (int i = 0; i < itemsToShow && i < _itemRects.Count; i++)
			{
				var user = users[i];
				var itemRect = _itemRects[i];
				var avatarRect = _avatarRects[i];
				var statusRect = _statusRects[i];
				
				// Hover background
				if (IsAreaHovered($"AvatarList_Item_{i}"))
				{
					using var hover = new SolidBrush(Color.FromArgb(6, Theme?.PrimaryColor ?? Color.Blue));
					g.FillRectangle(hover, itemRect);
				}
				
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
				DrawStatusIndicator(g, statusRect, user.Status);

				// Hover accents for avatar/status
				if (IsAreaHovered($"AvatarList_Avatar_{i}"))
				{
					using var pen = new Pen(Theme?.AccentColor ?? Color.Blue, 1.2f);
					g.DrawEllipse(pen, avatarRect);
				}
				if (IsAreaHovered($"AvatarList_Status_{i}"))
				{
					using var glow = new SolidBrush(Color.FromArgb(80, Theme?.AccentColor ?? Color.Blue));
					g.FillEllipse(glow, Rectangle.Inflate(statusRect, 3, 3));
				}
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
					float radius = Math.Min(rect.Width, rect.Height) / 2f;
					var cx = rect.X + rect.Width / 2f;
					var cy = rect.Y + rect.Height / 2f;
					StyledImagePainter.PaintInCircle(g, cx, cy, radius, ctx.ImagePath);
					avatarDrawn = true;
				}
				catch
				{
					try { _avatarPainter.DrawImage(g, ctx.ImagePath, rect); avatarDrawn = true; } catch { }
				}
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

		private void DrawStatusIndicator(Graphics g, Rectangle statusRect, string status)
		{
			var statusColor = status switch
			{
				"Online" => Color.LimeGreen,
				"Away" => Color.Orange,
				"Offline" => Color.Gray,
				_ => Color.Gray
			};
			
			using var statusBrush = new SolidBrush(statusColor);
			using var statusBorder = new Pen(Theme?.BackColor ?? Color.White, 2);
			g.FillEllipse(statusBrush, statusRect);
			g.DrawEllipse(statusBorder, statusRect);
		}

		public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
		{
			// Optional: scroll indicators or selection highlights already handled per-item hover
		}

		public override void UpdateHitAreas(BaseControl owner, WidgetContext ctx, Action<string, Rectangle>? notifyAreaHit)
		{
			if (owner == null) return;
			ClearOwnerHitAreas();
			for (int i = 0; i < _itemRects.Count; i++)
			{
				int idx = i;
				var itemRect = _itemRects[i];
				var avatarRect = _avatarRects[i];
				var statusRect = _statusRects[i];
				owner.AddHitArea($"AvatarList_Item_{idx}", itemRect, null, () =>
				{
					ctx.SelectedAvatarItemIndex = idx;
					notifyAreaHit?.Invoke($"AvatarList_Item_{idx}", itemRect);
					Owner?.Invalidate();
				});
				owner.AddHitArea($"AvatarList_Avatar_{idx}", avatarRect, null, () =>
				{
					ctx.SelectedAvatarIndex = idx;
					notifyAreaHit?.Invoke($"AvatarList_Avatar_{idx}", avatarRect);
					Owner?.Invalidate();
				});
				owner.AddHitArea($"AvatarList_Status_{idx}", statusRect, null, () =>
				{
					ctx.SelectedStatusIndex = idx;
					notifyAreaHit?.Invoke($"AvatarList_Status_{idx}", statusRect);
					Owner?.Invalidate();
				});
			}
		}

		public void Dispose()
		{
			_avatarPainter?.Dispose();
		}
	}
}
