using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Toggle;
using TheTechIdea.Beep.Icons;

namespace TheTechIdea.Beep.Winform.Controls.Toggle.Examples
{
    /// <summary>
    /// Demo form showing various custom icon toggle configurations
    /// </summary>
    public class CustomIconToggleDemo : Form
    {
        public CustomIconToggleDemo()
        {
            Text = "BeepToggle - Custom Icon Examples";
            Size = new Size(800, 600);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(250, 250, 250);
            Padding = new Padding(20);

            var panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                AutoScroll = true,
                WrapContents = false
            };

            // Example 1: Play/Pause Media Control
            panel.Controls.Add(CreateExampleGroup(
                "Play/Pause Media Control",
                CreateMediaToggle(),
                "Uses Play/Pause icons (if available) with red/green colors"
            ));

            // Example 2: Notification Toggle
            panel.Controls.Add(CreateExampleGroup(
                "Notifications On/Off",
                CreateNotificationToggle(),
                "Uses Bell/BellOff icons"
            ));

            // Example 3: Visibility Toggle
            panel.Controls.Add(CreateExampleGroup(
                "Show/Hide Content",
                CreateVisibilityToggle(),
                "Uses Eye/EyeOff icons"
            ));

            // Example 4: Star Rating Toggle
            panel.Controls.Add(CreateExampleGroup(
                "Favorite/Unfavorite",
                CreateStarToggle(),
                "Uses Star icon with different colors"
            ));

            // Example 5: Shield/Security Toggle
            panel.Controls.Add(CreateExampleGroup(
                "Security On/Off",
                CreateSecurityToggle(),
                "Uses Shield/ShieldOff icons"
            ));

            // Example 6: Download Toggle
            panel.Controls.Add(CreateExampleGroup(
                "Download/Upload",
                CreateDownloadToggle(),
                "Uses Download/Upload arrow icons"
            ));

            // Example 7: Calendar Toggle
            panel.Controls.Add(CreateExampleGroup(
                "Calendar View",
                CreateCalendarToggle(),
                "Uses Calendar icon with different colors"
            ));

            // Example 8: Grid/List View Toggle
            panel.Controls.Add(CreateExampleGroup(
                "Grid/List View",
                CreateViewToggle(),
                "Uses Grid/List icons"
            ));

            Controls.Add(panel);
        }

        private GroupBox CreateExampleGroup(string title, BeepToggle toggle, string description)
        {
            var group = new GroupBox
            {
                Text = title,
                Width = 740,
                Height = 100,
                Margin = new Padding(0, 0, 0, 15),
                Padding = new Padding(10)
            };

            var descLabel = new Label
            {
                Text = description,
                Location = new Point(10, 25),
                AutoSize = true,
                ForeColor = Color.FromArgb(100, 100, 100)
            };

            toggle.Location = new Point(10, 50);
            toggle.IsOnChanged += (s, e) =>
            {
                var t = (BeepToggle)s;
                descLabel.Text = $"{description} - Current: {(t.IsOn ? "ON" : "OFF")}";
            };

            group.Controls.Add(descLabel);
            group.Controls.Add(toggle);

            return group;
        }

        private BeepToggle CreateMediaToggle()
        {
            return new BeepToggle
            {
                ToggleStyle = ToggleStyle.IconCustom,
                OnIconPath = SvgsUI.Pause,
                OffIconPath = SvgsUI.Play,
                OnColor = Color.FromArgb(244, 67, 54),  // Red when playing/paused
                OffColor = Color.FromArgb(76, 175, 80), // Green when stopped
                Size = new Size(60, 30),
                IsOn = false
            };
        }

        private BeepToggle CreateNotificationToggle()
        {
            return new BeepToggle
            {
                ToggleStyle = ToggleStyle.IconCustom,
                OnIconPath = SvgsUI.Bell,
                OffIconPath = SvgsUI.BellOff,
                OnColor = Color.FromArgb(33, 150, 243), // Blue
                OffColor = Color.FromArgb(158, 158, 158),
                Size = new Size(60, 30),
                IsOn = true
            };
        }

        private BeepToggle CreateVisibilityToggle()
        {
            return new BeepToggle
            {
                ToggleStyle = ToggleStyle.IconCustom,
                OnIconPath = SvgsUI.Eye,
                OffIconPath = SvgsUI.EyeOff,
                OnColor = Color.FromArgb(103, 58, 183), // Purple
                OffColor = Color.FromArgb(158, 158, 158),
                Size = new Size(60, 30),
                IsOn = true
            };
        }

        private BeepToggle CreateStarToggle()
        {
            return new BeepToggle
            {
                ToggleStyle = ToggleStyle.IconCustom,
                OnIconPath = SvgsUI.Star,
                OffIconPath = SvgsUI.Star,
                OnColor = Color.FromArgb(255, 193, 7),  // Gold
                OffColor = Color.FromArgb(189, 189, 189),
                Size = new Size(60, 30),
                IsOn = false
            };
        }

        private BeepToggle CreateSecurityToggle()
        {
            return new BeepToggle
            {
                ToggleStyle = ToggleStyle.IconCustom,
                OnIconPath = SvgsUI.Shield,
                OffIconPath = SvgsUI.ShieldOff,
                OnColor = Color.FromArgb(76, 175, 80),  // Green
                OffColor = Color.FromArgb(244, 67, 54), // Red
                Size = new Size(60, 30),
                IsOn = true
            };
        }

        private BeepToggle CreateDownloadToggle()
        {
            return new BeepToggle
            {
                ToggleStyle = ToggleStyle.IconCustom,
                OnIconPath = SvgsUI.ArrowDown,
                OffIconPath = SvgsUI.ArrowUp,
                OnColor = Color.FromArgb(0, 150, 136),  // Teal
                OffColor = Color.FromArgb(255, 87, 34), // Deep Orange
                Size = new Size(60, 30),
                IsOn = false
            };
        }

        private BeepToggle CreateCalendarToggle()
        {
            return new BeepToggle
            {
                ToggleStyle = ToggleStyle.IconCustom,
                OnIconPath = SvgsUI.Calendar,
                OffIconPath = SvgsUI.Calendar,
                OnColor = Color.FromArgb(33, 150, 243), // Blue
                OffColor = Color.FromArgb(158, 158, 158),
                Size = new Size(60, 30),
                IsOn = true
            };
        }

        private BeepToggle CreateViewToggle()
        {
            return new BeepToggle
            {
                ToggleStyle = ToggleStyle.IconCustom,
                OnIconPath = SvgsUI.Grid,
                OffIconPath = SvgsUI.List,
                OnColor = Color.FromArgb(156, 39, 176), // Purple
                OffColor = Color.FromArgb(96, 125, 139), // Blue Gray
                Size = new Size(60, 30),
                IsOn = true
            };
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new CustomIconToggleDemo());
        }
    }
}
