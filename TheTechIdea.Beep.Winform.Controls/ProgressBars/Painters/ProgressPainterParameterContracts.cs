using System;
using System.Collections.Generic;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.ProgressBars.Models;

namespace TheTechIdea.Beep.Winform.Controls.ProgressBars.Painters
{
    internal static class ProgressPainterParameterContracts
    {
        public static ProgressPainterState GetState(IReadOnlyDictionary<string, object> parameters)
            => parameters != null && parameters.TryGetValue("State", out var value) && value is ProgressPainterState state
                ? state
                : null;

        public static ProgressPainterContext GetContext(IReadOnlyDictionary<string, object> parameters)
            => parameters != null && parameters.TryGetValue("Context", out var value) && value is ProgressPainterContext context
                ? context
                : null;

        public static int GetInt(IReadOnlyDictionary<string, object> parameters, string key, int fallback)
            => parameters != null && parameters.TryGetValue(key, out var value) && value is IConvertible
                ? Convert.ToInt32(value)
                : fallback;

        public static float GetFloat(IReadOnlyDictionary<string, object> parameters, string key, float fallback)
            => parameters != null && parameters.TryGetValue(key, out var value) && value is IConvertible
                ? Convert.ToSingle(value)
                : fallback;

        public static bool GetBool(IReadOnlyDictionary<string, object> parameters, string key, bool fallback)
            => parameters != null && parameters.TryGetValue(key, out var value) && value is bool boolean
                ? boolean
                : fallback;

        public static string GetString(IReadOnlyDictionary<string, object> parameters, string key, string fallback)
            => parameters != null && parameters.TryGetValue(key, out var value) && value is string text
                ? text
                : fallback;

        public static Color GetColor(IReadOnlyDictionary<string, object> parameters, string key, Color fallback)
            => parameters != null && parameters.TryGetValue(key, out var value) && value is Color color
                ? color
                : fallback;
    }
}
