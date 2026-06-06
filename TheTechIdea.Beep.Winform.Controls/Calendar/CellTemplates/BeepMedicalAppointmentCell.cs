using System;
using System.Drawing;
using TheTechIdea.Beep.Icons;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.CellTemplates
{
    public sealed class BeepMedicalAppointmentCell : BeepCellTemplateBase
    {
        protected override void DrawCellContent(Graphics g, Rectangle rect)
        {
            if (Event == null) return;
            var theme = _currentTheme;
            if (theme == null) return;

            var appointmentType = Metadatum("AppointmentType", "Appointment");
            var accentColor = appointmentType switch
            {
                "Emergency" => Color.FromArgb(207, 34, 46),
                "Surgery" => Color.FromArgb(210, 153, 34),
                "New" => Color.FromArgb(69, 133, 244),
                "Follow-up" => Color.FromArgb(46, 160, 67),
                _ => BeepCellTemplateHelpers.StatusColor(Event.Status)
            };

            int pad = 6;
            int accentWidth = 4;
            var content = new Rectangle(rect.X + pad + accentWidth, rect.Y + pad,
                rect.Width - pad * 2 - accentWidth, rect.Height - pad * 2);

            BeepCellTemplateHelpers.DrawAccentBadge(g, rect, accentColor, accentWidth);

            var titleFont = new Font(Font?.FontFamily ?? FontFamily.GenericSansSerif, 9f, FontStyle.Regular);
            var detailFont = new Font(Font?.FontFamily ?? FontFamily.GenericSansSerif, 7.5f, FontStyle.Regular);
            var textColor = theme.PrimaryTextColor;
            var secondaryColor = Color.FromArgb(180, textColor);

            int lineH = 16;
            var drawY = content.Y;

            var doctorName = Metadatum("DoctorName", "");
            var roomNumber = Metadatum("RoomNumber", "");
            var doctorLine = string.IsNullOrEmpty(doctorName)
                ? $"Room {roomNumber}"
                : string.IsNullOrEmpty(roomNumber)
                    ? doctorName
                    : $"{doctorName} · Room {roomNumber}";

            if (!string.IsNullOrEmpty(doctorLine))
            {
                var doctorRect = new Rectangle(content.X, drawY, content.Width, lineH);
                BeepCellTemplateHelpers.DrawIconLabel(g, doctorRect, detailFont,
                    secondaryColor, SvgsUIcons.Common.Calendar, doctorLine, 10);
                drawY += lineH;
            }

            var patientName = Metadatum("PatientName", "Patient");
            var patientRect = new Rectangle(content.X, drawY, content.Width, lineH);
            TextRenderer.DrawText(g, patientName, titleFont, patientRect, textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter |
                TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
            drawY += lineH;

            var procedureCode = Metadatum("ProcedureCode", "");
            var procLine = string.IsNullOrEmpty(procedureCode)
                ? appointmentType
                : $"{procedureCode} · {appointmentType}";
            var procRect = new Rectangle(content.X, drawY, content.Width, lineH);
            BeepCellTemplateHelpers.DrawIconLabel(g, procRect, detailFont,
                secondaryColor, SvgsUIcons.Common.Edit, procLine, 10);
            drawY += lineH;

            var hasSpaceForTime = content.Y + content.Height - drawY >= lineH;
            if (!Event.IsAllDay && hasSpaceForTime)
            {
                var timeText = $"{Event.StartTime:t} - {Event.EndTime:t}";
                var timeRect = new Rectangle(content.X, drawY, content.Width, lineH);
                BeepCellTemplateHelpers.DrawIconLabel(g, timeRect, detailFont,
                    secondaryColor, SvgsUIcons.Common.Clock, timeText, 10);
            }

            var patientStatus = Metadatum("PatientStatus", "Scheduled");
            var statusColor = patientStatus switch
            {
                "Complete" => Color.FromArgb(46, 160, 67),
                "InProgress" => Color.FromArgb(210, 153, 34),
                "CheckedIn" => Color.FromArgb(69, 133, 244),
                "NoShow" => Color.FromArgb(207, 34, 46),
                _ => Color.FromArgb(46, 160, 67)
            };
            BeepCellTemplateHelpers.DrawStatusDot(g, rect.Right - 16,
                rect.Y + rect.Height / 2, statusColor, 5);

            titleFont.Dispose();
            detailFont.Dispose();
        }
    }
}
