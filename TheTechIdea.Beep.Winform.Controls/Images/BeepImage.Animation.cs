using System;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Images
{
    public partial class BeepImage
    {
        #region Spin and Animations

        private void StartSpin()
        {
            if (_spinTimer != null && _spinTimer.Enabled)
                return;

            _manualRotationAngle = 0;

            if (_spinTimer == null)
            {
                _spinTimer = new Timer { Interval = 30 };
                _spinTimer.Tick += (s, e) =>
                {
                    bool needsRedraw = false;

                    if (IsSpinning)
                    {
                        _rotationAngle = (_rotationAngle + SpinSpeed) % 360;
                        needsRedraw = true;
                    }

                    if (_isPulsing)
                    {
                        _pulseScale += 0.01f * _pulseDirection;
                        if (_pulseScale > 1.1f || _pulseScale < 0.9f)
                            _pulseDirection *= -1;
                        needsRedraw = true;
                    }

                    if (_isBouncing)
                    {
                        _pulseScale += 0.04f * _pulseDirection;
                        if (_pulseScale > 1.2f || _pulseScale < 0.8f)
                            _pulseDirection *= -1;
                        needsRedraw = true;
                    }

                    if (_isFading)
                    {
                        _fadeAlpha += 0.05f * _fadeDirection;
                        if (_fadeAlpha <= 0.4f || _fadeAlpha >= 1.0f)
                            _fadeDirection *= -1;
                        needsRedraw = true;
                    }

                    if (_isShaking)
                    {
                        _shakeOffset += 1 * _shakeDirection;
                        if (Math.Abs(_shakeOffset) > 3)
                            _shakeDirection *= -1;
                        needsRedraw = true;
                    }

                    // Only invalidate if something actually changed
                    if (needsRedraw)
                    {
                        Invalidate();
                    }
                };
            }

            _spinTimer.Start();
        }

        private void StopSpin()
        {
            if (!IsSpinning) return; // If not spinning, do nothing

            _isSpinning = false;
            _spinTimer?.Stop();

            // Reset the rotation angle
            _rotationAngle = 0;
            _manualRotationAngle = 0; // Optionally reset manual rotation too
            _pulseScale = 1.0f;
            _fadeAlpha = 1.0f;
            _shakeOffset = 0;

            Invalidate(new Rectangle(0, 0, Width, Height)); // Redraw the entire control
        }

        #endregion
    }
}
