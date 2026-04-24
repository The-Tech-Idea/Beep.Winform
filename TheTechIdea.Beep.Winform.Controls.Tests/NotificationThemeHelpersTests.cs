// NotificationThemeHelpersTests.cs
// Unit tests for notification theme color helpers.
// These tests cover color calculation, contrast, and theme integration.
// ─────────────────────────────────────────────────────────────────────────────
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Notifications.Helpers;
using Xunit;

namespace TheTechIdea.Beep.Winform.Controls.Tests
{
    public class NotificationThemeHelpersTests
    {
        // ── GetBackgroundColor ────────────────────────────────────────────────

        [Fact]
        public void GetBackgroundColor_Success_ReturnsExpectedDefault()
        {
            var result = NotificationThemeHelpers.GetBackgroundColor(TheTechIdea.Beep.Winform.Controls.Notifications.NotificationType.Success);
            Assert.Equal(240, result.R);
            Assert.Equal(255, result.G);
            Assert.Equal(240, result.B);
        }

        [Fact]
        public void GetBackgroundColor_Warning_ReturnsExpectedDefault()
        {
            var result = NotificationThemeHelpers.GetBackgroundColor(TheTechIdea.Beep.Winform.Controls.Notifications.NotificationType.Warning);
            Assert.Equal(255, result.R);
            Assert.Equal(252, result.G);
            Assert.Equal(232, result.B);
        }

        [Fact]
        public void GetBackgroundColor_Error_ReturnsExpectedDefault()
        {
            var result = NotificationThemeHelpers.GetBackgroundColor(TheTechIdea.Beep.Winform.Controls.Notifications.NotificationType.Error);
            Assert.Equal(254, result.R);
            Assert.Equal(242, result.G);
            Assert.Equal(242, result.B);
        }

        [Fact]
        public void GetBackgroundColor_Info_ReturnsExpectedDefault()
        {
            var result = NotificationThemeHelpers.GetBackgroundColor(TheTechIdea.Beep.Winform.Controls.Notifications.NotificationType.Info);
            Assert.Equal(239, result.R);
            Assert.Equal(246, result.G);
            Assert.Equal(255, result.B);
        }

        [Fact]
        public void GetBackgroundColor_CustomColor_OverridesTypeColor()
        {
            var custom = Color.FromArgb(100, 150, 200);
            var result = NotificationThemeHelpers.GetBackgroundColor(TheTechIdea.Beep.Winform.Controls.Notifications.NotificationType.Info, customColor: custom);
            Assert.Equal(custom, result);
        }

        // ── GetForegroundColor ────────────────────────────────────────────────

        [Fact]
        public void GetForegroundColor_Success_ReturnsExpectedDefault()
        {
            var result = NotificationThemeHelpers.GetForegroundColor(TheTechIdea.Beep.Winform.Controls.Notifications.NotificationType.Success);
            Assert.Equal(22, result.R);
            Assert.Equal(101, result.G);
            Assert.Equal(52, result.B);
        }

        [Fact]
        public void GetForegroundColor_Warning_ReturnsExpectedDefault()
        {
            var result = NotificationThemeHelpers.GetForegroundColor(TheTechIdea.Beep.Winform.Controls.Notifications.NotificationType.Warning);
            Assert.Equal(113, result.R);
            Assert.Equal(63, result.G);
            Assert.Equal(18, result.B);
        }

        [Fact]
        public void GetForegroundColor_Error_ReturnsExpectedDefault()
        {
            var result = NotificationThemeHelpers.GetForegroundColor(TheTechIdea.Beep.Winform.Controls.Notifications.NotificationType.Error);
            Assert.Equal(127, result.R);
            Assert.Equal(29, result.G);
            Assert.Equal(29, result.B);
        }

        [Fact]
        public void GetForegroundColor_Info_ReturnsExpectedDefault()
        {
            var result = NotificationThemeHelpers.GetForegroundColor(TheTechIdea.Beep.Winform.Controls.Notifications.NotificationType.Info);
            Assert.Equal(30, result.R);
            Assert.Equal(58, result.G);
            Assert.Equal(138, result.B);
        }

        [Fact]
        public void GetForegroundColor_CustomColor_OverridesTypeColor()
        {
            var custom = Color.FromArgb(50, 100, 150);
            var result = NotificationThemeHelpers.GetForegroundColor(TheTechIdea.Beep.Winform.Controls.Notifications.NotificationType.Warning, customColor: custom);
            Assert.Equal(custom, result);
        }

        // ── GetBorderColor ───────────────────────────────────────────────────

        [Fact]
        public void GetBorderColor_Success_ReturnsExpectedDefault()
        {
            var result = NotificationThemeHelpers.GetBorderColor(TheTechIdea.Beep.Winform.Controls.Notifications.NotificationType.Success);
            Assert.Equal(134, result.R);
            Assert.Equal(239, result.G);
            Assert.Equal(172, result.B);
        }

        [Fact]
        public void GetBorderColor_Warning_ReturnsExpectedDefault()
        {
            var result = NotificationThemeHelpers.GetBorderColor(TheTechIdea.Beep.Winform.Controls.Notifications.NotificationType.Warning);
            Assert.Equal(251, result.R);
            Assert.Equal(191, result.G);
            Assert.Equal(36, result.B);
        }

        [Fact]
        public void GetBorderColor_Error_ReturnsExpectedDefault()
        {
            var result = NotificationThemeHelpers.GetBorderColor(TheTechIdea.Beep.Winform.Controls.Notifications.NotificationType.Error);
            Assert.Equal(252, result.R);
            Assert.Equal(165, result.G);
            Assert.Equal(165, result.B);
        }

        [Fact]
        public void GetBorderColor_Info_ReturnsExpectedDefault()
        {
            var result = NotificationThemeHelpers.GetBorderColor(TheTechIdea.Beep.Winform.Controls.Notifications.NotificationType.Info);
            Assert.Equal(147, result.R);
            Assert.Equal(197, result.G);
            Assert.Equal(253, result.B);
        }

        // ── GetIconColor ─────────────────────────────────────────────────────

        [Fact]
        public void GetIconColor_Success_ReturnsExpectedDefault()
        {
            var result = NotificationThemeHelpers.GetIconColor(TheTechIdea.Beep.Winform.Controls.Notifications.NotificationType.Success);
            Assert.Equal(34, result.R);
            Assert.Equal(197, result.G);
            Assert.Equal(94, result.B);
        }

        [Fact]
        public void GetIconColor_Warning_ReturnsExpectedDefault()
        {
            var result = NotificationThemeHelpers.GetIconColor(TheTechIdea.Beep.Winform.Controls.Notifications.NotificationType.Warning);
            Assert.Equal(245, result.R);
            Assert.Equal(158, result.G);
            Assert.Equal(11, result.B);
        }

        [Fact]
        public void GetIconColor_Error_ReturnsExpectedDefault()
        {
            var result = NotificationThemeHelpers.GetIconColor(TheTechIdea.Beep.Winform.Controls.Notifications.NotificationType.Error);
            Assert.Equal(239, result.R);
            Assert.Equal(68, result.G);
            Assert.Equal(68, result.B);
        }

        [Fact]
        public void GetIconColor_Info_ReturnsExpectedDefault()
        {
            var result = NotificationThemeHelpers.GetIconColor(TheTechIdea.Beep.Winform.Controls.Notifications.NotificationType.Info);
            Assert.Equal(59, result.R);
            Assert.Equal(130, result.G);
            Assert.Equal(246, result.B);
        }

        // ── GetRelativeLuminance ──────────────────────────────────────────────

        [Fact]
        public void GetRelativeLuminance_White_IsNearOne()
        {
            var luminance = NotificationThemeHelpers.GetRelativeLuminance(Color.White);
            Assert.InRange(luminance, 0.99f, 1.01f);
        }

        [Fact]
        public void GetRelativeLuminance_Black_IsZero()
        {
            var luminance = NotificationThemeHelpers.GetRelativeLuminance(Color.Black);
            Assert.Equal(0f, luminance);
        }

        [Fact]
        public void GetRelativeLuminance_MidGray_IsAroundHalf()
        {
            var luminance = NotificationThemeHelpers.GetRelativeLuminance(Color.FromArgb(128, 128, 128));
            Assert.InRange(luminance, 0.18f, 0.25f);
        }

        // ── GetContrastColor ──────────────────────────────────────────────────

        [Fact]
        public void GetContrastColor_WhiteBackground_ReturnsDarkColor()
        {
            var result = NotificationThemeHelpers.GetContrastColor(Color.White);
            Assert.True(result.R < 128 && result.G < 128 && result.B < 128);
        }

        [Fact]
        public void GetContrastColor_BlackBackground_ReturnsLightColor()
        {
            var result = NotificationThemeHelpers.GetContrastColor(Color.Black);
            Assert.True(result.R > 128 && result.G > 128 && result.B > 128);
        }

        [Fact]
        public void GetContrastColor_WithExplicitColors_ReturnsCorrectContrast()
        {
            var darkOnLight = NotificationThemeHelpers.GetContrastColor(Color.White, Color.Black, Color.White);
            Assert.Equal(Color.Black, darkOnLight);

            var lightOnDark = NotificationThemeHelpers.GetContrastColor(Color.Black, Color.Black, Color.White);
            Assert.Equal(Color.White, lightOnDark);
        }

        // ── ShiftLuminance ────────────────────────────────────────────────────

        [Fact]
        public void ShiftLuminance_PositiveAmount_LightensColor()
        {
            var original = Color.FromArgb(100, 100, 100);
            var lightened = NotificationThemeHelpers.ShiftLuminance(original, 0.3f);
            Assert.True(lightened.R > original.R);
            Assert.True(lightened.G > original.G);
            Assert.True(lightened.B > original.B);
        }

        [Fact]
        public void ShiftLuminance_NegativeAmount_DarkensColor()
        {
            var original = Color.FromArgb(150, 150, 150);
            var darkened = NotificationThemeHelpers.ShiftLuminance(original, -0.3f);
            Assert.True(darkened.R < original.R);
            Assert.True(darkened.G < original.G);
            Assert.True(darkened.B < original.B);
        }

        [Fact]
        public void ShiftLuminance_ZeroAmount_ReturnsSameColor()
        {
            var original = Color.FromArgb(120, 140, 160);
            var result = NotificationThemeHelpers.ShiftLuminance(original, 0f);
            Assert.Equal(original.R, result.R);
            Assert.Equal(original.G, result.G);
            Assert.Equal(original.B, result.B);
        }

        // ── GetColorsForType ──────────────────────────────────────────────────

        [Fact]
        public void GetColorsForType_ReturnsAllFourColors()
        {
            var colors = NotificationThemeHelpers.GetColorsForType(TheTechIdea.Beep.Winform.Controls.Notifications.NotificationType.Error);
            Assert.NotEqual(Color.Empty, colors.BackColor);
            Assert.NotEqual(Color.Empty, colors.ForeColor);
            Assert.NotEqual(Color.Empty, colors.BorderColor);
            Assert.NotEqual(Color.Empty, colors.IconColor);
        }

        [Fact]
        public void GetColorsForType_Success_HasGreenishIconColor()
        {
            var colors = NotificationThemeHelpers.GetColorsForType(TheTechIdea.Beep.Winform.Controls.Notifications.NotificationType.Success);
            Assert.True(colors.IconColor.G > colors.IconColor.R);
            Assert.True(colors.IconColor.G > colors.IconColor.B);
        }

        [Fact]
        public void GetColorsForType_Error_HasReddishIconColor()
        {
            var colors = NotificationThemeHelpers.GetColorsForType(TheTechIdea.Beep.Winform.Controls.Notifications.NotificationType.Error);
            Assert.True(colors.IconColor.R > colors.IconColor.G);
            Assert.True(colors.IconColor.R > colors.IconColor.B);
        }

        [Fact]
        public void GetColorsForType_Warning_HasYellowishIconColor()
        {
            var colors = NotificationThemeHelpers.GetColorsForType(TheTechIdea.Beep.Winform.Controls.Notifications.NotificationType.Warning);
            Assert.True(colors.IconColor.R > 200);
            Assert.True(colors.IconColor.G > 100);
            Assert.True(colors.IconColor.B < 50);
        }

        [Fact]
        public void GetColorsForType_Info_HasBluishIconColor()
        {
            var colors = NotificationThemeHelpers.GetColorsForType(TheTechIdea.Beep.Winform.Controls.Notifications.NotificationType.Info);
            Assert.True(colors.IconColor.B > colors.IconColor.R);
            Assert.True(colors.IconColor.B > colors.IconColor.G);
        }
    }
}
