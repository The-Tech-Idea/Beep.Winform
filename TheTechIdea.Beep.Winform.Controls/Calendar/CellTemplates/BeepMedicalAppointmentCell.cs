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
            if (_currentTheme == null) return;

            var appointmentType = Metadatum("AppointmentType", "Appointment");
            var accentColor = appointmentType switch
            {
                "Emergency" => BeepCellTemplateHelpers.PriorityColor("critical"),
                "Surgery" => BeepCellTemplateHelpers.PriorityColor("high"),
                "New" => BeepCellTemplateHelpers.PriorityColor("medium"),
                "Follow-up" => BeepCellTemplateHelpers.StatusColor(CalendarEventStatus.Confirmed),
                _ => BeepCellTemplateHelpers.StatusColor(Event.Status)
            };

            var content = CellContentRect(rect);
            BeepCellTemplateHelpers.DrawAccentBadge(g, rect, accentColor, CellAccentWidth);

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
                var doctorRect = new Rectangle(content.X, drawY, content.Width, CellLineHeight);
                BeepCellTemplateHelpers.DrawIconLabel(g, doctorRect, DetailFont,
                    SecondaryTextColor, SvgsUIcons.Common.Calendar, doctorLine, 10);
                drawY += CellLineHeight;
            }

            var patientName = Metadatum("PatientName", "Patient");
            var patientRect = new Rectangle(content.X, drawY, content.Width, CellLineHeight);
            TextRenderer.DrawText(g, patientName, TitleFont, patientRect, TextColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter |
                TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
            drawY += CellLineHeight;

            var procedureCode = Metadatum("ProcedureCode", "");
            var procLine = string.IsNullOrEmpty(procedureCode)
                ? appointmentType
                : $"{procedureCode} · {appointmentType}";
            var procRect = new Rectangle(content.X, drawY, content.Width, CellLineHeight);
            BeepCellTemplateHelpers.DrawIconLabel(g, procRect, DetailFont,
                SecondaryTextColor, SvgsUIcons.Common.Edit, procLine, 10);
            drawY += CellLineHeight;

            var hasSpaceForTime = content.Y + content.Height - drawY >= CellLineHeight;
            if (!Event.IsAllDay && hasSpaceForTime)
            {
                var timeText = $"{Event.StartTime:t} - {Event.EndTime:t}";
                var timeRect = new Rectangle(content.X, drawY, content.Width, CellLineHeight);
                BeepCellTemplateHelpers.DrawIconLabel(g, timeRect, DetailFont,
                    SecondaryTextColor, SvgsUIcons.Common.Clock, timeText, 10);
            }

            var patientStatus = Metadatum("PatientStatus", "Scheduled");
            var statusColor = patientStatus switch
            {
                "Complete" => BeepCellTemplateHelpers.StatusColor(CalendarEventStatus.Confirmed),
                "InProgress" => BeepCellTemplateHelpers.PriorityColor("high"),
                "CheckedIn" => BeepCellTemplateHelpers.PriorityColor("medium"),
                "NoShow" => BeepCellTemplateHelpers.PriorityColor("critical"),
                _ => BeepCellTemplateHelpers.StatusColor(CalendarEventStatus.Confirmed)
            };
            BeepCellTemplateHelpers.DrawStatusDot(g, rect.Right - 16,
                rect.Y + rect.Height / 2, statusColor, 5);
        }
    }
}
