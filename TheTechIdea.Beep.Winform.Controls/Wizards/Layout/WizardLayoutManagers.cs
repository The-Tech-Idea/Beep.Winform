using System;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Wizards.Layout
{
    /// <summary>
    /// Abstract base for wizard layout managers
    /// </summary>
    public abstract class WizardLayoutManager
    {
        protected Control Host { get; private set; }
        protected WizardInstance Instance { get; private set; }

        public virtual void Initialize(Control host, WizardInstance instance)
        {
            Host = host;
            Instance = instance;
        }

        public abstract Rectangle GetStepIndicatorBounds();
        public abstract Rectangle GetContentBounds();
        public abstract Rectangle GetButtonPanelBounds();

        /// <summary>
        /// Hit test for step indicators (for clickable steps)
        /// </summary>
        public virtual int HitTestStep(Point location)
        {
            return -1; // Default: no step hit
        }
    }

    /// <summary>
    /// Layout manager for horizontal stepper wizard
    /// </summary>
    public class HorizontalStepperLayout : WizardLayoutManager
    {
        private const int StepIndicatorHeight = 100;
        private const int ButtonPanelHeight = 70;
        private const int SidePadding = 30;

        public override Rectangle GetStepIndicatorBounds()
        {
            if (Host == null) return Rectangle.Empty;
            return new Rectangle(0, 0, Host.ClientSize.Width, StepIndicatorHeight);
        }

        public override Rectangle GetContentBounds()
        {
            if (Host == null) return Rectangle.Empty;
            return new Rectangle(
                SidePadding,
                StepIndicatorHeight,
                Host.ClientSize.Width - (SidePadding * 2),
                Host.ClientSize.Height - StepIndicatorHeight - ButtonPanelHeight
            );
        }

        public override Rectangle GetButtonPanelBounds()
        {
            if (Host == null) return Rectangle.Empty;
            return new Rectangle(
                0,
                Host.ClientSize.Height - ButtonPanelHeight,
                Host.ClientSize.Width,
                ButtonPanelHeight
            );
        }

        public override int HitTestStep(Point location)
        {
            if (Instance == null || !GetStepIndicatorBounds().Contains(location))
                return -1;

            int stepCount = Instance.Config.Steps.Count;
            if (stepCount == 0) return -1;

            int padding = 50;
            int availableWidth = Host.ClientSize.Width - (padding * 2);
            int stepSpacing = stepCount > 1 ? availableWidth / (stepCount - 1) : 0;
            int startX = padding;
            int circleSize = 36;
            int hitRadius = circleSize; // Hit area radius

            for (int i = 0; i < stepCount; i++)
            {
                int x = startX + (i * stepSpacing);
                int centerY = StepIndicatorHeight / 2 - 5;

                // Check if within hit radius of circle center
                double distance = Math.Sqrt(Math.Pow(location.X - x, 2) + Math.Pow(location.Y - centerY, 2));
                if (distance <= hitRadius)
                {
                    // Only allow clicking on completed steps (to go back)
                    if (i < Instance.CurrentStepIndex)
                        return i;
                }
            }

            return -1;
        }
    }

    /// <summary>
    /// Layout manager for vertical stepper wizard
    /// </summary>
    public class VerticalStepperLayout : WizardLayoutManager
    {
        private const int SidePanelWidth = 280;
        private const int ButtonPanelHeight = 70;
        private const int ContentPadding = 30;

        public override Rectangle GetStepIndicatorBounds()
        {
            if (Host == null) return Rectangle.Empty;
            return new Rectangle(0, 0, SidePanelWidth, Host.ClientSize.Height);
        }

        public override Rectangle GetContentBounds()
        {
            if (Host == null) return Rectangle.Empty;
            return new Rectangle(
                SidePanelWidth + ContentPadding,
                ContentPadding,
                Host.ClientSize.Width - SidePanelWidth - (ContentPadding * 2),
                Host.ClientSize.Height - ButtonPanelHeight - ContentPadding
            );
        }

        public override Rectangle GetButtonPanelBounds()
        {
            if (Host == null) return Rectangle.Empty;
            return new Rectangle(
                SidePanelWidth,
                Host.ClientSize.Height - ButtonPanelHeight,
                Host.ClientSize.Width - SidePanelWidth,
                ButtonPanelHeight
            );
        }

        public override int HitTestStep(Point location)
        {
            if (Instance == null || !GetStepIndicatorBounds().Contains(location))
                return -1;

            int stepCount = Instance.Config.Steps.Count;
            if (stepCount == 0) return -1;

            int leftMargin = 30;
            int topMargin = 40;
            int circleSize = 32;
            int itemHeight = Math.Min(80, (Host.ClientSize.Height - topMargin * 2) / stepCount);

            for (int i = 0; i < stepCount; i++)
            {
                int y = topMargin + (i * itemHeight);
                var circleRect = new Rectangle(leftMargin, y, circleSize, circleSize);
                circleRect.Inflate(5, 5); // Expand hit area

                if (circleRect.Contains(location))
                {
                    // Only allow clicking on completed steps
                    if (i < Instance.CurrentStepIndex)
                        return i;
                }
            }

            return -1;
        }
    }

    /// <summary>
    /// Layout manager for minimal wizard
    /// </summary>
    public class MinimalLayout : WizardLayoutManager
    {
        private const int HeaderHeight = 60;
        private const int ButtonPanelHeight = 60;
        private const int SidePadding = 40;

        public override Rectangle GetStepIndicatorBounds()
        {
            if (Host == null) return Rectangle.Empty;
            return new Rectangle(0, 0, Host.ClientSize.Width, HeaderHeight);
        }

        public override Rectangle GetContentBounds()
        {
            if (Host == null) return Rectangle.Empty;
            return new Rectangle(
                SidePadding,
                HeaderHeight,
                Host.ClientSize.Width - (SidePadding * 2),
                Host.ClientSize.Height - HeaderHeight - ButtonPanelHeight
            );
        }

        public override Rectangle GetButtonPanelBounds()
        {
            if (Host == null) return Rectangle.Empty;
            return new Rectangle(
                0,
                Host.ClientSize.Height - ButtonPanelHeight,
                Host.ClientSize.Width,
                ButtonPanelHeight
            );
        }
    }
}
