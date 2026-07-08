using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Wizards.Layout
{
    /// <summary>
    /// Abstract base for wizard layout managers. All pixel values are DPI-scaled.
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

        /// <summary>DPI-scale a logical pixel value.</summary>
        protected int S(int value) => Host != null ? DpiScalingHelper.ScaleValue(value, Host) : value;
        protected float SF(float value) => Host != null ? DpiScalingHelper.ScaleValue(value, Host) : value;

        public abstract Rectangle GetStepIndicatorBounds();
        public abstract Rectangle GetContentBounds();
        public abstract Rectangle GetButtonPanelBounds();
        public abstract Size GetRecommendedMinimumSize();

        public virtual int HitTestStep(Point location) => -1;
    }

    public class HorizontalStepperLayout : WizardLayoutManager
    {
        protected virtual int StepIndicatorHeight => S(100);
        protected virtual int ButtonPanelHeight => S(70);
        protected virtual int SidePadding => S(30);

        public override Rectangle GetStepIndicatorBounds()
        {
            if (Host == null) return Rectangle.Empty;
            return new Rectangle(0, 0, Host.ClientSize.Width, StepIndicatorHeight);
        }
        public override Rectangle GetContentBounds()
        {
            if (Host == null) return Rectangle.Empty;
            return new Rectangle(SidePadding, StepIndicatorHeight,
                Host.ClientSize.Width - SidePadding * 2,
                Host.ClientSize.Height - StepIndicatorHeight - ButtonPanelHeight);
        }
        public override Rectangle GetButtonPanelBounds()
        {
            if (Host == null) return Rectangle.Empty;
            return new Rectangle(0, Host.ClientSize.Height - ButtonPanelHeight,
                Host.ClientSize.Width, ButtonPanelHeight);
        }
        public override Size GetRecommendedMinimumSize() => new Size(S(600), S(400));

        public override int HitTestStep(Point location)
        {
            if (Instance == null || !GetStepIndicatorBounds().Contains(location)) return -1;
            int stepCount = Instance.Config.Steps.Count;
            if (stepCount == 0) return -1;

            int padding = S(50), circleSize = S(36);
            int availableWidth = Host.ClientSize.Width - padding * 2;
            int stepSpacing = stepCount > 1 ? availableWidth / (stepCount - 1) : 0;
            int centerY = StepIndicatorHeight / 2 - S(5);

            for (int i = 0; i < stepCount; i++)
            {
                int x = padding + i * stepSpacing;
                if (Math.Sqrt(Math.Pow(location.X - x, 2) + Math.Pow(location.Y - centerY, 2)) <= circleSize)
                    if (i < Instance.CurrentStepIndex) return i;
            }
            return -1;
        }
    }

    public class VerticalStepperLayout : WizardLayoutManager
    {
        protected virtual int SidePanelWidth => S(280);
        protected virtual int ButtonPanelHeight => S(70);
        protected virtual int ContentPadding => S(30);

        public override Rectangle GetStepIndicatorBounds()
        {
            if (Host == null) return Rectangle.Empty;
            return new Rectangle(0, 0, SidePanelWidth, Host.ClientSize.Height);
        }
        public override Rectangle GetContentBounds()
        {
            if (Host == null) return Rectangle.Empty;
            return new Rectangle(SidePanelWidth + ContentPadding, ContentPadding,
                Host.ClientSize.Width - SidePanelWidth - ContentPadding * 2,
                Host.ClientSize.Height - ButtonPanelHeight - ContentPadding);
        }
        public override Rectangle GetButtonPanelBounds()
        {
            if (Host == null) return Rectangle.Empty;
            return new Rectangle(SidePanelWidth, Host.ClientSize.Height - ButtonPanelHeight,
                Host.ClientSize.Width - SidePanelWidth, ButtonPanelHeight);
        }
        public override Size GetRecommendedMinimumSize() => new Size(S(500), S(400));

        public override int HitTestStep(Point location)
        {
            if (Instance == null || !GetStepIndicatorBounds().Contains(location)) return -1;
            int stepCount = Instance.Config.Steps.Count;
            if (stepCount == 0) return -1;

            int leftMargin = S(30), topMargin = S(40), circleSize = S(32);
            int maxItemHeight = S(80);
            int itemHeight = Math.Min(maxItemHeight, (Host.ClientSize.Height - topMargin * 2) / stepCount);

            for (int i = 0; i < stepCount; i++)
            {
                var circleRect = new Rectangle(leftMargin, topMargin + i * itemHeight, circleSize, circleSize);
                circleRect.Inflate(S(5), S(5));
                if (circleRect.Contains(location) && i < Instance.CurrentStepIndex) return i;
            }
            return -1;
        }
    }

    public class MinimalLayout : WizardLayoutManager
    {
        protected virtual int HeaderHeight => S(60);
        protected virtual int ButtonPanelHeight => S(60);
        protected virtual int SidePadding => S(40);

        public override Rectangle GetStepIndicatorBounds()
        {
            if (Host == null) return Rectangle.Empty;
            return new Rectangle(0, 0, Host.ClientSize.Width, HeaderHeight);
        }
        public override Rectangle GetContentBounds()
        {
            if (Host == null) return Rectangle.Empty;
            return new Rectangle(SidePadding, HeaderHeight,
                Host.ClientSize.Width - SidePadding * 2,
                Host.ClientSize.Height - HeaderHeight - ButtonPanelHeight);
        }
        public override Rectangle GetButtonPanelBounds()
        {
            if (Host == null) return Rectangle.Empty;
            return new Rectangle(0, Host.ClientSize.Height - ButtonPanelHeight,
                Host.ClientSize.Width, ButtonPanelHeight);
        }
        public override Size GetRecommendedMinimumSize() => new Size(S(450), S(350));
    }
}
