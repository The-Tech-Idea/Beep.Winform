using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;


namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers
{
    /// <summary>
    /// Animation helper for DialogManager
    /// </summary>
    public static partial class DialogManager
    {
        private static class DialogAnimator
        {
            private const int AnimationDuration = 200; // milliseconds
            private const int AnimationSteps = 10;

            public static void ApplyAnimation(BeepPopupForm form, DialogShowAnimation animation)
            {
                if (form == null || animation == DialogShowAnimation.None)
                    return;

                switch (animation)
                {
                    case DialogShowAnimation.FadeIn:
                        AnimateFadeIn(form);
                        break;
                    case DialogShowAnimation.SlideInFromTop:
                        AnimateSlideIn(form, Direction.Top);
                        break;
                    case DialogShowAnimation.SlideInFromBottom:
                        AnimateSlideIn(form, Direction.Bottom);
                        break;
                    case DialogShowAnimation.SlideInFromLeft:
                        AnimateSlideIn(form, Direction.Left);
                        break;
                    case DialogShowAnimation.SlideInFromRight:
                        AnimateSlideIn(form, Direction.Right);
                        break;
                    case DialogShowAnimation.ZoomIn:
                        AnimateZoomIn(form);
                        break;
                }
            }

            private static void AnimateFadeIn(BeepPopupForm form)
            {
                form.Opacity = 0;
                var timer = new Timer { Interval = AnimationDuration / AnimationSteps };
                int step = 0;

                timer.Tick += (s, e) =>
                {
                    step++;
                    form.Opacity = (double)step / AnimationSteps;
                    
                    if (step >= AnimationSteps)
                    {
                        timer.Stop();
                        timer.Dispose();
                        form.Opacity = 1;
                    }
                };
                
                timer.Start();
            }

            private static void AnimateSlideIn(BeepPopupForm form, Direction direction)
            {
                var finalLocation = form.Location;
                int distance = 50; // pixels to slide
                
                switch (direction)
                {
                    case Direction.Top:
                        form.Location = new Point(finalLocation.X, finalLocation.Y - distance);
                        break;
                    case Direction.Bottom:
                        form.Location = new Point(finalLocation.X, finalLocation.Y + distance);
                        break;
                    case Direction.Left:
                        form.Location = new Point(finalLocation.X - distance, finalLocation.Y);
                        break;
                    case Direction.Right:
                        form.Location = new Point(finalLocation.X + distance, finalLocation.Y);
                        break;
                }

                form.Opacity = 0;
                var timer = new Timer { Interval = AnimationDuration / AnimationSteps };
                int step = 0;

                timer.Tick += (s, e) =>
                {
                    step++;
                    double progress = (double)step / AnimationSteps;
                    form.Opacity = progress;
                    
                    int currentX = (int)(form.Location.X + (finalLocation.X - form.Location.X) * progress);
                    int currentY = (int)(form.Location.Y + (finalLocation.Y - form.Location.Y) * progress);
                    form.Location = new Point(currentX, currentY);
                    
                    if (step >= AnimationSteps)
                    {
                        timer.Stop();
                        timer.Dispose();
                        form.Opacity = 1;
                        form.Location = finalLocation;
                    }
                };
                
                timer.Start();
            }

            private static void AnimateZoomIn(BeepPopupForm form)
            {
                var finalSize = form.Size;
                var finalLocation = form.Location;
                
                form.Size = new Size(finalSize.Width / 2, finalSize.Height / 2);
                form.Location = new Point(
                    finalLocation.X + finalSize.Width / 4,
                    finalLocation.Y + finalSize.Height / 4
                );
                form.Opacity = 0;

                var timer = new Timer { Interval = AnimationDuration / AnimationSteps };
                int step = 0;

                timer.Tick += (s, e) =>
                {
                    step++;
                    double progress = (double)step / AnimationSteps;
                    form.Opacity = progress;
                    
                    int currentWidth = (int)(form.Size.Width + (finalSize.Width - form.Size.Width) * progress);
                    int currentHeight = (int)(form.Size.Height + (finalSize.Height - form.Size.Height) * progress);
                    form.Size = new Size(currentWidth, currentHeight);
                    
                    int currentX = (int)(form.Location.X + (finalLocation.X - form.Location.X) * progress);
                    int currentY = (int)(form.Location.Y + (finalLocation.Y - form.Location.Y) * progress);
                    form.Location = new Point(currentX, currentY);
                    
                    if (step >= AnimationSteps)
                    {
                        timer.Stop();
                        timer.Dispose();
                        form.Opacity = 1;
                        form.Size = finalSize;
                        form.Location = finalLocation;
                    }
                };
                
                timer.Start();
            }

            private enum Direction
            {
                Top,
                Bottom,
                Left,
                Right
            }
        }
    }
}
