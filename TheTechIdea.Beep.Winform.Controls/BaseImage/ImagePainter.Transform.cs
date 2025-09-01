using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.BaseImage
{
    public partial class ImagePainter
    {
        public void Rotate90Clockwise()
        {
            if (_isSvg && _svgDocument != null)
                ManualRotationAngle = (ManualRotationAngle + 90f) % 360f;
            else if (_regularImage != null)
                RotateImage(RotateFlipType.Rotate90FlipNone);
        }

        public void Rotate90CounterClockwise()
        {
            if (_isSvg && _svgDocument != null)
                ManualRotationAngle = (ManualRotationAngle - 90f + 360f) % 360f;
            else if (_regularImage != null)
                RotateImage(RotateFlipType.Rotate270FlipNone);
        }

        public void Rotate180()
        {
            if (_isSvg && _svgDocument != null)
                ManualRotationAngle = (ManualRotationAngle + 180f) % 360f;
            else if (_regularImage != null)
                RotateImage(RotateFlipType.Rotate180FlipNone);
        }

        public void FlipHorizontal()
        {
            if (_isSvg && _svgDocument != null)
            {
                FlipX = !FlipX;
            }
            else if (_regularImage != null)
            {
                RotateImage(RotateFlipType.RotateNoneFlipX);
            }
        }

        public void FlipVertical()
        {
            if (_isSvg && _svgDocument != null)
            {
                FlipY = !FlipY;
            }
            else if (_regularImage != null)
            {
                RotateImage(RotateFlipType.RotateNoneFlipY);
            }
        }

        public void RotateImage(RotateFlipType rotateFlipType)
        {
            if (_regularImage != null)
            {
                _regularImage.RotateFlip(rotateFlipType);
                _stateChanged = true;
                InvalidateCache();
            }
        }

        public void ResetTransformations()
        {
            ManualRotationAngle = 0;
            FlipX = false;
            FlipY = false;
            if (_regularImage != null && !string.IsNullOrEmpty(ImagePath))
            {
                LoadImage(ImagePath);
            }
        }
    }
}
