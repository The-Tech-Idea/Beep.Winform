using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules.Managers;
using TheTechIdea.Beep.Desktop.Common.Util;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// A comprehensive project task card designed for kanban-style project management.
    /// Features team avatars, project status, priority indicators, progress tracking, and action buttons.
    /// </summary>
    [ToolboxItem(true)]
    [DisplayName("Beep Project Task Card")]
    [Description("A project card for kanban-style project management with avatars, status, priority, progress, and actions.")]
    public class BeepProjectTaskCard : BeepControl
    {
        #region Private Fields
        private BeepButton _actionButton;
        private BeepLabel _label;
        private BeepImage _image;
        private List<string> _teamAvatarPaths = new List<string>();
        private List<Image> _teamAvatars = new List<Image>();
        
        // Project Information
        private string _projectTitle = "New Project";
        private string _projectDescription = "Project description goes here";
        private string _projectStatus = "In Progress";
        private string _projectPriority = "Medium";
        private string _projectCategory = "Development";
        
        // Team and Timeline
        private string _projectManager = "Unassigned";
        private string _teamCount = "3";
        private DateTime? _dueDate = DateTime.Now.AddDays(7);
        private DateTime _createdDate = DateTime.Now;
        
        // Progress and Metrics
        private float _progressValue = 45f; // Progress percentage (0-100)
        private int _completedTasks = 5;
        private int _totalTasks = 12;
        private int _hoursSpent = 85;
        private int _estimatedHours = 120;
        
        // Visual Elements - using string paths like BeepTaskCard
        private string _statusIconPath = BeepSvgPaths.CheckCircle;
        private string _priorityIconPath = BeepSvgPaths.ExclamationTriangle;
        private Color _statusColor = Color.FromArgb(59, 130, 246); // Blue for In Progress
        private Color _priorityColor = Color.FromArgb(245, 158, 11); // Orange for Medium
        
        // Card Layout
        private bool _compactMode = false;
        private bool _showTeamAvatars = true;
        private bool _showProgressBar = true;
        private bool _showActionButtons = true;
        private bool _showDueDate = true;
        private bool _showTaskCount = true;
        #endregion

        #region Public Properties

        [Category("Project Info")]
        [Description("Main project title.")]
        public string ProjectTitle
        {
            get => _projectTitle;
            set { _projectTitle = value; Invalidate(); }
        }

        [Category("Project Info")]
        [Description("Project description or summary.")]
        public string ProjectDescription
        {
            get => _projectDescription;
            set { _projectDescription = value; Invalidate(); }
        }

        [Category("Project Info")]
        [Description("Current project status (e.g., 'Not Started', 'In Progress', 'Completed', 'On Hold').")]
        public string ProjectStatus
        {
            get => _projectStatus;
            set { 
                _projectStatus = value; 
                UpdateStatusColor();
                Invalidate(); 
            }
        }

        [Category("Project Info")]
        [Description("Project priority level (e.g., 'Low', 'Medium', 'High', 'Critical').")]
        public string ProjectPriority
        {
            get => _projectPriority;
            set { 
                _projectPriority = value; 
                UpdatePriorityColor();
                Invalidate(); 
            }
        }

        [Category("Project Info")]
        [Description("Project category or type (e.g., 'Development', 'Design', 'Marketing').")]
        public string ProjectCategory
        {
            get => _projectCategory;
            set { _projectCategory = value; Invalidate(); }
        }

        [Category("Team")]
        [Description("List of team member avatar image paths.")]
        public List<string> TeamAvatarPaths
        {
            get => _teamAvatarPaths;
            set
            {
                _teamAvatarPaths = value;
                // Convert each path to an Image using the helper, same as BeepTaskCard
                _teamAvatars = _teamAvatarPaths.Select(path => ImageListHelper.GetImageFromName(path) as Image).ToList();
                Invalidate();
            }
        }

        [Category("Team")]
        [Description("Project manager or lead person.")]
        public string ProjectManager
        {
            get => _projectManager;
            set { _projectManager = value; Invalidate(); }
        }

        [Category("Team")]
        [Description("Total number of team members.")]
        public string TeamCount
        {
            get => _teamCount;
            set { _teamCount = value; Invalidate(); }
        }

        [Category("Timeline")]
        [Description("Project due date.")]
        public DateTime? DueDate
        {
            get => _dueDate;
            set { _dueDate = value; Invalidate(); }
        }

        [Category("Timeline")]
        [Description("Project creation date.")]
        public DateTime CreatedDate
        {
            get => _createdDate;
            set { _createdDate = value; Invalidate(); }
        }

        [Category("Progress")]
        [Description("Overall project progress percentage (0-100).")]
        public float ProgressValue
        {
            get => _progressValue;
            set
            {
                _progressValue = Math.Max(0f, Math.Min(100f, value));
                Invalidate();
            }
        }

        [Category("Progress")]
        [Description("Number of completed tasks.")]
        public int CompletedTasks
        {
            get => _completedTasks;
            set { _completedTasks = value; Invalidate(); }
        }

        [Category("Progress")]
        [Description("Total number of tasks in the project.")]
        public int TotalTasks
        {
            get => _totalTasks;
            set { _totalTasks = value; Invalidate(); }
        }

        [Category("Progress")]
        [Description("Hours spent on the project.")]
        public int HoursSpent
        {
            get => _hoursSpent;
            set { _hoursSpent = value; Invalidate(); }
        }

        [Category("Progress")]
        [Description("Estimated total hours for the project.")]
        public int EstimatedHours
        {
            get => _estimatedHours;
            set { _estimatedHours = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Status icon path or name (e.g., 'CheckCircle', 'Clock', etc.).")]
        public string StatusIconPath
        {
            get => _statusIconPath;
            set
            {
                _statusIconPath = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        [Description("Priority icon path or name (e.g., 'ExclamationTriangle', 'Star', etc.).")]
        public string PriorityIconPath
        {
            get => _priorityIconPath;
            set
            {
                _priorityIconPath = value;
                Invalidate();
            }
        }

        [Category("Layout")]
        [Description("Use compact layout for smaller cards.")]
        public bool CompactMode
        {
            get => _compactMode;
            set { _compactMode = value; Invalidate(); }
        }

        [Category("Layout")]
        [Description("Show team member avatars.")]
        public bool ShowTeamAvatars
        {
            get => _showTeamAvatars;
            set { _showTeamAvatars = value; Invalidate(); }
        }

        [Category("Layout")]
        [Description("Show progress bar.")]
        public bool ShowProgressBar
        {
            get => _showProgressBar;
            set { _showProgressBar = value; Invalidate(); }
        }

        [Category("Layout")]
        [Description("Show action buttons.")]
        public bool ShowActionButtons
        {
            get => _showActionButtons;
            set { _showActionButtons = value; Invalidate(); }
        }

        [Category("Layout")]
        [Description("Show due date information.")]
        public bool ShowDueDate
        {
            get => _showDueDate;
            set { _showDueDate = value; Invalidate(); }
        }

        [Category("Layout")]
        [Description("Show task count information.")]
        public bool ShowTaskCount
        {
            get => _showTaskCount;
            set { _showTaskCount = value; Invalidate(); }
        }

        #endregion

        #region Events

        public event EventHandler<ProjectCardEventArgs> ProjectClicked;
        public event EventHandler<ProjectCardEventArgs> StatusChanged;
        public event EventHandler<ProjectCardEventArgs> PriorityChanged;
        public event EventHandler<ProjectCardEventArgs> ProgressUpdated;
        public event EventHandler<ProjectCardActionEventArgs> ActionButtonClicked;

        #endregion

        #region Constructor

        public BeepProjectTaskCard()
        {
            InitializeCard();
            UpdateStatusColor();
            UpdatePriorityColor();
        }

        private void InitializeCard()
        {
            // Set default size and style based on compact mode
            this.Size = _compactMode ? new Size(200, 180) : new Size(280, 320);
            this.BorderRadius = 12;
            this.ShowShadow = true;
            this.UseGradientBackground = true;
            this.GradientDirection = LinearGradientMode.Vertical;
            this.GradientStartColor = Color.FromArgb(255, 248, 249, 250);
            this.GradientEndColor = Color.FromArgb(255, 241, 245, 249);
            this.ForeColor = Color.FromArgb(51, 51, 51);
            this.BorderColor = Color.FromArgb(226, 232, 240);
            this.Cursor = Cursors.Hand;

            // Enable mouse events
            this.SetStyle(ControlStyles.Selectable, true);
            this.TabStop = true;
        }

        #endregion

        #region Drawing Methods

        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);
            UpdateDrawingRect();
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var clientRect = this.DrawingRect;
            int padding = _compactMode ? 8 : 12;
            int currentY = clientRect.Top + padding;

            // Draw in sections
            currentY = DrawHeader(g, clientRect, padding, currentY);
            currentY = DrawContent(g, clientRect, padding, currentY);
            currentY = DrawMetrics(g, clientRect, padding, currentY);
            if (_showProgressBar)
                currentY = DrawProgressBar(g, clientRect, padding, currentY);
            if (_showActionButtons && !_compactMode)
                DrawActionButtons(g, clientRect, padding, currentY);
        }

        private int DrawHeader(Graphics g, Rectangle clientRect, int padding, int currentY)
        {
            int headerHeight = _compactMode ? 40 : 50;
            
            // Draw status indicator with icon using BeepImage methodology
            var statusRect = new Rectangle(clientRect.Left + padding, currentY, 80, 24);
            DrawStatusBadge(g, statusRect);

            // Draw priority indicator on the right with icon using BeepImage methodology
            var priorityRect = new Rectangle(clientRect.Right - padding - 80, currentY, 80, 24);
            DrawPriorityBadge(g, priorityRect);

            return currentY + headerHeight;
        }

        private void DrawStatusBadge(Graphics g, Rectangle rect)
        {
            using (var brush = new SolidBrush(_statusColor))
            using (var borderBrush = new SolidBrush(Color.FromArgb(100, _statusColor)))
            {
                // Draw background with rounded corners
                var bgRect = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
                using (var path = CreateRoundedPath(bgRect, 12))
                {
                    g.FillPath(borderBrush, path);
                }

                // Draw status icon using BeepImage methodology
                if (!string.IsNullOrEmpty(_statusIconPath))
                {
                    var iconSize = _compactMode ? 12 : 14;
                    var iconRect = new Rectangle(rect.X + 4, rect.Y + (rect.Height - iconSize) / 2, iconSize, iconSize);
                    
                    if (_image == null) _image = new BeepImage();
                    _image.ImagePath = _statusIconPath;
                    _image.IsChild = true;
                    _image.ApplyThemeOnImage = true;
                    _image.Draw(g, iconRect);
                }

                // Draw status text
                var statusFont = FontListHelper.CreateFontFromTypography(_currentTheme.TaskCardTitleStyle) ?? 
                                new Font("Segoe UI", _compactMode ? 7f : 8f, FontStyle.Bold);
                var textColor = GetContrastColor(_statusColor);
                using (var textBrush = new SolidBrush(textColor))
                {
                    var textRect = new RectangleF(rect.X + 18, rect.Y + 2, rect.Width - 22, rect.Height - 4);
                    var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString(_projectStatus, statusFont, textBrush, textRect, format);
                }
            }
        }

        private void DrawPriorityBadge(Graphics g, Rectangle rect)
        {
            using (var brush = new SolidBrush(_priorityColor))
            using (var borderBrush = new SolidBrush(Color.FromArgb(100, _priorityColor)))
            {
                // Draw background with rounded corners
                var bgRect = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
                using (var path = CreateRoundedPath(bgRect, 12))
                {
                    g.FillPath(borderBrush, path);
                }

                // Draw priority icon using BeepImage methodology
                if (!string.IsNullOrEmpty(_priorityIconPath))
                {
                    var iconSize = _compactMode ? 12 : 14;
                    var iconRect = new Rectangle(rect.X + 4, rect.Y + (rect.Height - iconSize) / 2, iconSize, iconSize);
                    
                    if (_image == null) _image = new BeepImage();
                    _image.ImagePath = _priorityIconPath;
                    _image.IsChild = true;
                    _image.ApplyThemeOnImage = true;
                    _image.Draw(g, iconRect);
                }

                // Draw priority text
                var priorityFont = FontListHelper.CreateFontFromTypography(_currentTheme.TaskCardTitleStyle) ?? 
                                  new Font("Segoe UI", _compactMode ? 7f : 8f, FontStyle.Bold);
                var textColor = GetContrastColor(_priorityColor);
                using (var textBrush = new SolidBrush(textColor))
                {
                    var textRect = new RectangleF(rect.X + 18, rect.Y + 2, rect.Width - 22, rect.Height - 4);
                    var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString(_projectPriority, priorityFont, textBrush, textRect, format);
                }
            }
        }

        private int DrawContent(Graphics g, Rectangle clientRect, int padding, int currentY)
        {
            int contentHeight = _compactMode ? 80 : 120;
            
            // Draw project title using BeepLabel methodology
            if (_label == null) _label = new BeepLabel();
            _label.Text = _projectTitle;
            _label.ForeColor = _currentTheme.TaskCardTitleForeColor;
            _label.Font = FontListHelper.CreateFontFromTypography(_currentTheme.TaskCardTitleFont) ?? 
                         new Font("Segoe UI", _compactMode ? 12f : 16f, FontStyle.Bold);
            _label.BackColor = _currentTheme.BackColor;
            _label.IsChild = true;
            var titleRect = new Rectangle(clientRect.Left + padding, currentY, 
                                        clientRect.Width - 2 * padding, _compactMode ? 20 : 30);
            _label.Draw(g, titleRect);

            currentY += _compactMode ? 25 : 35;

            // Draw project description (if not compact) using BeepLabel methodology
            if (!_compactMode)
            {
                var descLabel = new BeepLabel
                {
                    Text = _projectDescription,
                    ForeColor = _currentTheme.TaskCardSubTitleForeColor,
                    Font = FontListHelper.CreateFontFromTypography(_currentTheme.TaskCardSubStyleStyle) ?? 
                          new Font("Segoe UI", 10f, FontStyle.Regular),
                    BackColor = _currentTheme.BackColor,
                    IsChild = true
                };
                var descRect = new Rectangle(clientRect.Left + padding, currentY, 
                                            clientRect.Width - 2 * padding, 40);
                descLabel.Draw(g, descRect);
                currentY += 45;
            }

            // Draw team avatars using BeepImage methodology
            if (_showTeamAvatars)
            {
                currentY = DrawTeamAvatars(g, clientRect, padding, currentY);
            }

            return currentY;
        }

        private int DrawTeamAvatars(Graphics g, Rectangle clientRect, int padding, int currentY)
        {
            int avatarSize = _compactMode ? 24 : 32;
            int overlap = _compactMode ? 6 : 8;
            int maxVisibleAvatars = _compactMode ? 3 : 4;
            
            int avatarX = clientRect.Left + padding;
            int displayedCount = Math.Min(_teamAvatars.Count, maxVisibleAvatars);

            // Draw avatars using BeepImage methodology (same as BeepTaskCard)
            for (int i = 0; i < displayedCount; i++)
            {
                int offsetX = avatarX + i * (avatarSize - overlap);
                var avatarRect = new Rectangle(offsetX, currentY, avatarSize, avatarSize);

                if (_image == null) _image = new BeepImage();
                _image.Image = _teamAvatars[i];
                _image.IsChild = true;
                _image.BorderColor = Color.White;
                _image.BorderThickness = 2;
                _image.IsRounded = true;
                _image.Draw(g, avatarRect);
            }

            // Draw +X indicator if there are more avatars using BeepLabel methodology
            if (_teamAvatars.Count > maxVisibleAvatars)
            {
                int offsetX = avatarX + displayedCount * (avatarSize - overlap);
                var plusRect = new Rectangle(offsetX, currentY, avatarSize, avatarSize);

                var plusLabel = new BeepLabel
                {
                    Text = $"+{_teamAvatars.Count - maxVisibleAvatars}",
                    ForeColor = Color.White,
                    BackColor = Color.FromArgb(150, 107, 114, 128),
                    Font = FontListHelper.CreateFontFromTypography(_currentTheme.TaskCardSubStyleStyle) ??
                          new Font("Segoe UI", _compactMode ? 8f : 10f, FontStyle.Bold),
                    IsChild = true,
                    IsRounded = true,
                    BorderColor = Color.White,
                    BorderThickness = 2
                };
                plusLabel.Draw(g, plusRect);
            }

            return currentY + avatarSize + (_compactMode ? 8 : 12);
        }

        private int DrawMetrics(Graphics g, Rectangle clientRect, int padding, int currentY)
        {
            int metricsHeight = _compactMode ? 30 : 40;
            
            // Draw metrics using BeepLabel methodology
            var metricsLabel = new BeepLabel
            {
                ForeColor = _currentTheme.TaskCardMetricTextForeColor,
                Font = FontListHelper.CreateFontFromTypography(_currentTheme.TaskCardMetricTextStyle) ??
                      new Font("Segoe UI", _compactMode ? 8f : 9f, FontStyle.Regular),
                BackColor = _currentTheme.BackColor,
                IsChild = true
            };

            int leftX = clientRect.Left + padding;
            int rightX = clientRect.Right - padding - 80;

            // Left side metrics
            if (_showTaskCount)
            {
                metricsLabel.Text = $"Tasks: {_completedTasks}/{_totalTasks}";
                var tasksRect = new Rectangle(leftX, currentY, 120, 20);
                metricsLabel.Draw(g, tasksRect);
            }

            // Right side metrics
            if (_showDueDate && _dueDate.HasValue)
            {
                var dueText = _dueDate.Value.ToString("MMM dd");
                var isOverdue = _dueDate.Value < DateTime.Now;
                metricsLabel.ForeColor = isOverdue ? Color.FromArgb(239, 68, 68) : _currentTheme.TaskCardMetricTextForeColor;
                metricsLabel.Text = dueText;
                var dueRect = new Rectangle(rightX, currentY, 80, 20);
                metricsLabel.Draw(g, dueRect);
            }

            currentY += _compactMode ? 15 : 20;

            // Hours metric
            metricsLabel.ForeColor = _currentTheme.TaskCardMetricTextForeColor;
            metricsLabel.Text = $"{_hoursSpent}h / {_estimatedHours}h";
            var hoursRect = new Rectangle(leftX, currentY, 120, 20);
            metricsLabel.Draw(g, hoursRect);

            return currentY + metricsHeight;
        }

        private int DrawProgressBar(Graphics g, Rectangle clientRect, int padding, int currentY)
        {
            int barHeight = _compactMode ? 4 : 6;
            int barX = clientRect.Left + padding;
            int barWidth = clientRect.Width - 2 * padding;
            
            // Draw background
            using (var backBrush = new SolidBrush(Color.FromArgb(226, 232, 240)))
            {
                var bgRect = new Rectangle(barX, currentY, barWidth, barHeight);
                using (var path = CreateRoundedPath(bgRect, barHeight / 2))
                {
                    g.FillPath(backBrush, path);
                }
            }

            // Draw progress
            var progressWidth = (int)((ProgressValue / 100f) * barWidth);
            if (progressWidth > 0)
            {
                using (var progressBrush = new SolidBrush(_statusColor))
                {
                    var progressRect = new Rectangle(barX, currentY, progressWidth, barHeight);
                    using (var path = CreateRoundedPath(progressRect, barHeight / 2))
                    {
                        g.FillPath(progressBrush, path);
                    }
                }
            }

            // Draw progress percentage text using BeepLabel methodology
            if (!_compactMode)
            {
                var progressLabel = new BeepLabel
                {
                    Text = $"{ProgressValue:F0}%",
                    ForeColor = _currentTheme.TaskCardMetricTextForeColor,
                    Font = new Font("Segoe UI", 8f, FontStyle.Bold),
                    BackColor = _currentTheme.BackColor,
                    IsChild = true
                };
                var textX = clientRect.Right - padding - 30;
                var textY = currentY + barHeight + 4;
                var progressTextRect = new Rectangle(textX, textY, 30, 15);
                progressLabel.Draw(g, progressTextRect);
            }

            return currentY + barHeight + (_compactMode ? 8 : 20);
        }

        private void DrawActionButtons(Graphics g, Rectangle clientRect, int padding, int currentY)
        {
            if (_compactMode) return;

            var buttonFont = new Font("Segoe UI", 9f, FontStyle.Regular);
            int buttonHeight = 28;
            int buttonSpacing = 8;
            int buttonWidth = (clientRect.Width - 2 * padding - buttonSpacing) / 2;

            // View Details button
            var viewRect = new Rectangle(clientRect.Left + padding, currentY, buttonWidth, buttonHeight);
            DrawActionButton(g, viewRect, "View Details", Color.FromArgb(59, 130, 246), buttonFont);

            // Update Status button
            var updateRect = new Rectangle(clientRect.Left + padding + buttonWidth + buttonSpacing, currentY, buttonWidth, buttonHeight);
            DrawActionButton(g, updateRect, "Update", Color.FromArgb(34, 197, 94), buttonFont);
        }

        private void DrawActionButton(Graphics g, Rectangle rect, string text, Color backgroundColor, Font font)
        {
            // Draw button background
            using (var brush = new SolidBrush(backgroundColor))
            {
                using (var path = CreateRoundedPath(rect, 6))
                {
                    g.FillPath(brush, path);
                }
            }

            // Draw button text using BeepLabel methodology
            var buttonLabel = new BeepLabel
            {
                Text = text,
                ForeColor = Color.White,
                Font = font,
                BackColor = backgroundColor,
                IsChild = true
            };
            buttonLabel.Draw(g, rect);
        }

        #endregion

        #region Helper Methods

        private void UpdateStatusColor()
        {
            _statusColor = _projectStatus.ToLower() switch
            {
                "not started" => Color.FromArgb(107, 114, 128), // Gray
                "in progress" => Color.FromArgb(59, 130, 246),  // Blue
                "completed" => Color.FromArgb(34, 197, 94),     // Green
                "on hold" => Color.FromArgb(245, 158, 11),      // Orange
                "cancelled" => Color.FromArgb(239, 68, 68),     // Red
                _ => Color.FromArgb(107, 114, 128)               // Default Gray
            };
        }

        private void UpdatePriorityColor()
        {
            _priorityColor = _projectPriority.ToLower() switch
            {
                "low" => Color.FromArgb(34, 197, 94),       // Green
                "medium" => Color.FromArgb(245, 158, 11),   // Orange
                "high" => Color.FromArgb(239, 68, 68),      // Red
                "critical" => Color.FromArgb(147, 51, 234), // Purple
                _ => Color.FromArgb(107, 114, 128)           // Default Gray
            };
        }

        private GraphicsPath CreateRoundedPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            path.AddArc(rect.X, rect.Y, radius * 2, radius * 2, 180, 90);
            path.AddArc(rect.Right - radius * 2, rect.Y, radius * 2, radius * 2, 270, 90);
            path.AddArc(rect.Right - radius * 2, rect.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
            path.AddArc(rect.X, rect.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
            path.CloseFigure();
            return path;
        }

        private Color GetContrastColor(Color color)
        {
            // Calculate luminance and return black or white for best contrast
            double luminance = (0.299 * color.R + 0.587 * color.G + 0.114 * color.B) / 255;
            return luminance > 0.5 ? Color.Black : Color.White;
        }

        #endregion

        #region Event Handling

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            ProjectClicked?.Invoke(this, new ProjectCardEventArgs
            {
                ProjectTitle = _projectTitle,
                ProjectStatus = _projectStatus,
                ProjectPriority = _projectPriority,
                ProgressValue = _progressValue
            });
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            
            // Check if action buttons were clicked
            if (_showActionButtons && !_compactMode)
            {
                var clientRect = this.DrawingRect;
                int padding = 12;
                int buttonHeight = 28;
                int buttonSpacing = 8;
                int buttonWidth = (clientRect.Width - 2 * padding - buttonSpacing) / 2;
                int buttonY = clientRect.Bottom - padding - buttonHeight;

                var viewRect = new Rectangle(clientRect.Left + padding, buttonY, buttonWidth, buttonHeight);
                var updateRect = new Rectangle(clientRect.Left + padding + buttonWidth + buttonSpacing, buttonY, buttonWidth, buttonHeight);

                if (viewRect.Contains(e.Location))
                {
                    ActionButtonClicked?.Invoke(this, new ProjectCardActionEventArgs
                    {
                        Action = "ViewDetails",
                        ProjectTitle = _projectTitle
                    });
                }
                else if (updateRect.Contains(e.Location))
                {
                    ActionButtonClicked?.Invoke(this, new ProjectCardActionEventArgs
                    {
                        Action = "Update",
                        ProjectTitle = _projectTitle
                    });
                }
            }
        }

        #endregion

        #region Theme Application

        public override void ApplyTheme()
        {
            base.ApplyTheme();
            
            // Apply theme colors to the control
            this.BackColor = _currentTheme.TaskCardBackColor;
            this.GradientStartColor = _currentTheme.GradientStartColor;
            this.GradientEndColor = _currentTheme.GradientEndColor;
            this.BorderColor = _currentTheme.TaskCardBorderColor;
            this.ForeColor = _currentTheme.TaskCardForeColor;
            
            Invalidate();
        }

        #endregion
    }

    #region Event Args Classes

    public class ProjectCardEventArgs : EventArgs
    {
        public string ProjectTitle { get; set; }
        public string ProjectStatus { get; set; }
        public string ProjectPriority { get; set; }
        public float ProgressValue { get; set; }
    }

    public class ProjectCardActionEventArgs : EventArgs
    {
        public string Action { get; set; }
        public string ProjectTitle { get; set; }
    }

    #endregion
}
