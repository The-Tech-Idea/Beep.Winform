using System;
using System.Media;
using System.IO;

namespace TheTechIdea.Beep.Winform.Controls.Notifications
{
    /// <summary>
    /// Sound manager for notification system
    /// Supports system sounds and custom WAV files
    /// </summary>
    internal static class BeepNotificationSound
    {
        private static bool _soundEnabled = true;
        private static SoundPlayer _customPlayer;

        /// <summary>
        /// Enable or disable all notification sounds globally
        /// </summary>
        public static bool SoundEnabled
        {
            get => _soundEnabled;
            set => _soundEnabled = value;
        }

        /// <summary>
        /// Play appropriate sound for notification type
        /// </summary>
        public static void PlaySound(NotificationType type, string? customSoundPath = null)
        {
            if (!_soundEnabled)
                return;

            try
            {
                // Custom sound takes precedence
                if (!string.IsNullOrEmpty(customSoundPath) && File.Exists(customSoundPath))
                {
                    PlayCustomSound(customSoundPath);
                    return;
                }

                // Default system sounds based on type
                switch (type)
                {
                    case NotificationType.Success:
                        SystemSounds.Exclamation.Play();
                        break;

                    case NotificationType.Warning:
                        SystemSounds.Exclamation.Play();
                        break;

                    case NotificationType.Error:
                        SystemSounds.Hand.Play();
                        break;

                    case NotificationType.Info:
                        SystemSounds.Asterisk.Play();
                        break;

                    case NotificationType.System:
                        SystemSounds.Beep.Play();
                        break;

                    default:
                        SystemSounds.Asterisk.Play();
                        break;
                }
            }
            catch
            {
                // Silently fail if sound cannot be played
            }
        }

        /// <summary>
        /// Play custom WAV file
        /// </summary>
        public static void PlayCustomSound(string wavFilePath)
        {
            if (!_soundEnabled)
                return;

            if (string.IsNullOrEmpty(wavFilePath) || !File.Exists(wavFilePath))
                return;

            try
            {
                // Dispose previous player if exists
                _customPlayer?.Dispose();

                _customPlayer = new SoundPlayer(wavFilePath);
                _customPlayer.Play();
            }
            catch
            {
                // Silently fail if sound cannot be played
            }
        }

        /// <summary>
        /// Stop any currently playing custom sound
        /// </summary>
        public static void Stop()
        {
            try
            {
                _customPlayer?.Stop();
            }
            catch
            {
                // Silently fail
            }
        }

        /// <summary>
        /// Cleanup resources
        /// </summary>
        public static void Dispose()
        {
            try
            {
                _customPlayer?.Dispose();
                _customPlayer = null;
            }
            catch
            {
                // Silently fail
            }
        }
    }
}
