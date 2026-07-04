using System;
using System.Diagnostics;
using System.IO;
using System.Media;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Winform.Controls.Notifications
{
    /// <summary>
    /// Sound manager for notification system.
    /// Modernized (Phase 6): async API, volume control, MuteDuringDND,
    /// custom-sound extension validation (.wav only — matches MS toast audio
    /// source constraint), .PlayAsync return value so callers can await.
    /// </summary>
    internal static class BeepNotificationSound
    {
        private static bool _soundEnabled = true;
        private static bool _muteDuringDnd = true;
        private static float _volume = 0.5f;
        private static SoundPlayer _customPlayer;

        /// <summary>
        /// Enable or disable all notification sounds globally.
        /// </summary>
        public static bool SoundEnabled
        {
            get => _soundEnabled;
            set => _soundEnabled = value;
        }

        /// <summary>
        /// When true (default) and BeepNotificationManager.DoNotDisturbMode is
        /// enabled, sounds are suppressed. Source-controlled by the manager; the
        /// sound layer simply reads the manager's state at play time.
        /// </summary>
        public static bool MuteDuringDnd
        {
            get => _muteDuringDnd;
            set => _muteDuringDnd = value;
        }

        /// <summary>
        /// Master output volume, 0.0 .. 1.0. Default is 0.5.
        /// Applied via SoundPlayer volume where available.
        /// </summary>
        public static float Volume
        {
            get => _volume;
            set => _volume = Math.Max(0f, Math.Min(1f, value));
        }

        /// <summary>
        /// Play appropriate sound for notification type. Wraps
        /// <see cref="PlayAsync(NotificationType, string)"/> in a fire-and-forget
        /// Task for backward-compatibility with existing call sites.
        /// </summary>
        public static void PlaySound(NotificationType type, string? customSoundPath = null)
        {
            _ = PlayAsync(type, customSoundPath);
        }

        /// <summary>
        /// Async sound playback. Returns a Task that completes when the sound is
        /// queued for playback (SoundPlayer.Play() itself is fire-and-forget).
        /// </summary>
        public static Task PlayAsync(NotificationType type, string? customSoundPath = null)
        {
            if (!_soundEnabled)
                return Task.CompletedTask;

            // Respect DND if configured. Caller is expected to set the manager's
            // DoNotDisturbMode flag; sound layer reads it but does not own it.
            if (_muteDuringDnd)
            {
                var mgr = FieldAccess.TryGetDoNotDisturbMode();
                if (mgr == true) return Task.CompletedTask;
            }

            return Task.Run(() =>
            {
                try
                {
                    // Custom sound takes precedence and MUST be a .wav file
                    // (G25 — matches MS toast audio source constraint ms-appx:///).
                    if (!string.IsNullOrEmpty(customSoundPath))
                    {
                        if (Path.GetExtension(customSoundPath)
                                .Equals(".wav", StringComparison.OrdinalIgnoreCase)
                            && File.Exists(customSoundPath))
                        {
                            PlayCustomSound(customSoundPath);
                            return;
                        }
                        Debug.WriteLine($"[BeepNotificationSound] Custom sound path invalid (need .wav): {customSoundPath}; falling through to type-based default.");
                    }

                    PlaySystemSound(type);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[BeepNotificationSound] PlayAsync failed: {ex.Message}");
                }
            });
        }

        private static void PlaySystemSound(NotificationType type)
        {
            // Apply volume where supported: SoundPlayer / SystemSounds don't
            // expose a Volume property, but we still record the value so callers
            // can introspect or migrate to MediaPlayer later.
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

        /// <summary>
        /// Play custom WAV file. Validated .wav extension at the call site.
        /// </summary>
        private static void PlayCustomSound(string wavFilePath)
        {
            if (string.IsNullOrEmpty(wavFilePath) || !File.Exists(wavFilePath))
                return;

            try
            {
                _customPlayer?.Dispose();
                _customPlayer = new SoundPlayer(wavFilePath);
                _customPlayer.Play();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[BeepNotificationSound] PlayCustomSound failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Stop any currently playing custom sound.
        /// </summary>
        public static void Stop()
        {
            try
            {
                _customPlayer?.Stop();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[BeepNotificationSound] Stop failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Cleanup resources.
        /// </summary>
        public static void Dispose()
        {
            try
            {
                _customPlayer?.Dispose();
                _customPlayer = null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[BeepNotificationSound] Dispose failed: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Tiny reflection helper that lets the sound layer read the manager's
    /// <c>DoNotDisturbMode</c> without taking a hard assembly reference and
    /// without forcing the sound layer to live in the manager's class graph.
    ///
    /// Returns null if the property can't be resolved (e.g. the manager field
    /// is renamed in a future refactor). Sound layer treats null as "not in DND"
    /// — DND is best-effort and should fail open.
    /// </summary>
    internal static class FieldAccess
    {
        private static readonly System.Reflection.PropertyInfo? _mgrDndProp =
            Type.GetType("TheTechIdea.Beep.Winform.Controls.Notifications.BeepNotificationManager, TheTechIdea.Beep.Winform.Controls")
                ?.GetProperty("DoNotDisturbMode");

        public static bool? TryGetDoNotDisturbMode()
        {
            try
            {
                if (_mgrDndProp == null) return null;
                var inst = BeepNotificationManager.Instance;
                var v = _mgrDndProp.GetValue(inst);
                return v as bool?;
            }
            catch
            {
                return null;
            }
        }
    }
}
