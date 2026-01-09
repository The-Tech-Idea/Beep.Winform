using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Editors
{
    /// <summary>
    /// Design-time editor for selecting and configuring painters
    /// Provides a dialog to browse available painters for a control type
    /// </summary>
    public class PainterSelectorEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object? EditValue(ITypeDescriptorContext? context, IServiceProvider provider, object? value)
        {
            if (provider == null || context == null)
                return value;

            // Get the current painter type name
            string currentPainter = value as string ?? string.Empty;

            // For now, return a simple input dialog
            // In a full implementation, this would show a dialog with:
            // - List of available painters for the control type
            // - Preview of each painter
            // - Configuration options for the selected painter
            using (var dialog = new Form
            {
                Text = "Select Painter",
                Size = new System.Drawing.Size(400, 200),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            })
            {
                var label = new Label
                {
                    Text = "Painter Type:",
                    Location = new System.Drawing.Point(12, 15),
                    AutoSize = true
                };

                var textBox = new TextBox
                {
                    Text = currentPainter,
                    Location = new System.Drawing.Point(12, 35),
                    Size = new System.Drawing.Size(360, 23)
                };

                var okButton = new Button
                {
                    Text = "OK",
                    DialogResult = DialogResult.OK,
                    Location = new System.Drawing.Point(216, 130),
                    Size = new System.Drawing.Size(75, 23)
                };

                var cancelButton = new Button
                {
                    Text = "Cancel",
                    DialogResult = DialogResult.Cancel,
                    Location = new System.Drawing.Point(297, 130),
                    Size = new System.Drawing.Size(75, 23)
                };

                dialog.Controls.AddRange(new Control[] { label, textBox, okButton, cancelButton });
                dialog.AcceptButton = okButton;
                dialog.CancelButton = cancelButton;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    return textBox.Text;
                }
            }

            return value;
        }

        public override bool GetPaintValueSupported(ITypeDescriptorContext? context)
        {
            return true;
        }

        public override void PaintValue(PaintValueEventArgs e)
        {
            if (e.Value is string painterName && !string.IsNullOrEmpty(painterName))
            {
                using (var brush = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(100, 150, 200)))
                {
                    e.Graphics.FillRectangle(brush, e.Bounds);
                }

                using (var font = new System.Drawing.Font("Segoe UI", 7, System.Drawing.FontStyle.Regular))
                using (var textBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White))
                {
                    var format = new System.Drawing.StringFormat
                    {
                        Alignment = System.Drawing.StringAlignment.Center,
                        LineAlignment = System.Drawing.StringAlignment.Center,
                        Trimming = System.Drawing.StringTrimming.EllipsisCharacter
                    };
                    e.Graphics.DrawString("P", font, textBrush, e.Bounds, format);
                }
            }
        }
    }
}
