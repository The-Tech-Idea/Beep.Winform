using System;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Layouts
{
    /// <summary>
    /// Configuration for grid animations
    /// Controls which animations are enabled and their behavior
    /// </summary>
    public class AnimationConfig
    {
        /// <summary>
        /// Whether to animate when rows are inserted
        /// </summary>
        public bool AnimateRowInsert { get; set; } = true;
        
        /// <summary>
        /// Whether to animate when rows are deleted
        /// </summary>
        public bool AnimateRowDelete { get; set; } = true;
        
        /// <summary>
        /// Whether to animate when cell values are updated
        /// </summary>
        public bool AnimateCellUpdate { get; set; } = true;
        
        /// <summary>
        /// Whether to animate when sorting changes
        /// </summary>
        public bool AnimateSortChange { get; set; } = false;
        
        /// <summary>
        /// Whether to animate when filtering changes
        /// </summary>
        public bool AnimateFilterChange { get; set; } = false;
        
        /// <summary>
        /// Whether to animate row selection
        /// </summary>
        public bool AnimateSelection { get; set; } = true;
        
        /// <summary>
        /// Duration of animations in milliseconds
        /// </summary>
        public int AnimationDuration { get; set; } = 200;
        
        /// <summary>
        /// Easing function for animations
        /// </summary>
        public AnimationEasing EasingFunction { get; set; } = AnimationEasing.EaseOutCubic;
        
        /// <summary>
        /// Whether animations are enabled globally
        /// </summary>
        public bool Enabled { get; set; } = true;
        
        /// <summary>
        /// Delay before starting animation (useful for insert/delete)
        /// </summary>
        public int AnimationDelay { get; set; } = 0;
        
        /// <summary>
        /// Whether to use hardware acceleration for animations (when available)
        /// </summary>
        public bool UseHardwareAcceleration { get; set; } = true;
        
        /// <summary>
        /// Maximum number of concurrent animations
        /// </summary>
        public int MaxConcurrentAnimations { get; set; } = 50;
        
        /// <summary>
        /// Cell update highlight duration
        /// </summary>
        public int CellHighlightDuration { get; set; } = 1000;
        
        /// <summary>
        /// Cell update highlight color (null = use theme accent)
        /// </summary>
        public System.Drawing.Color? CellHighlightColor { get; set; } = null;
    }
    
    /// <summary>
    /// Easing functions for animations
    /// </summary>
    public enum AnimationEasing
    {
        /// <summary>
        /// Linear interpolation (no easing)
        /// </summary>
        Linear,
        
        /// <summary>
        /// Quadratic ease in
        /// </summary>
        EaseInQuad,
        
        /// <summary>
        /// Quadratic ease out
        /// </summary>
        EaseOutQuad,
        
        /// <summary>
        /// Quadratic ease in and out
        /// </summary>
        EaseInOutQuad,
        
        /// <summary>
        /// Cubic ease in
        /// </summary>
        EaseInCubic,
        
        /// <summary>
        /// Cubic ease out (smooth deceleration)
        /// </summary>
        EaseOutCubic,
        
        /// <summary>
        /// Cubic ease in and out
        /// </summary>
        EaseInOutCubic,
        
        /// <summary>
        /// Material Design standard easing
        /// </summary>
        MaterialStandard,
        
        /// <summary>
        /// Material Design decelerate easing
        /// </summary>
        MaterialDecelerate,
        
        /// <summary>
        /// Material Design accelerate easing
        /// </summary>
        MaterialAccelerate,
        
        /// <summary>
        /// Fluent Design ease-out easing
        /// </summary>
        FluentEaseOut,
        
        /// <summary>
        /// iOS standard spring animation
        /// </summary>
        iOSSpring,
        
        /// <summary>
        /// Elastic bounce effect
        /// </summary>
        Elastic
    }
    
    /// <summary>
    /// Helper class for easing function calculations
    /// </summary>
    public static class EasingFunctions
    {
        public static float Apply(float progress, AnimationEasing easing)
        {
            progress = Math.Max(0, Math.Min(1, progress));
            
            return easing switch
            {
                AnimationEasing.Linear => progress,
                AnimationEasing.EaseInQuad => progress * progress,
                AnimationEasing.EaseOutQuad => 1 - (1 - progress) * (1 - progress),
                AnimationEasing.EaseInOutQuad => progress < 0.5f 
                    ? 2 * progress * progress 
                    : 1 - (float)Math.Pow(-2 * progress + 2, 2) / 2,
                AnimationEasing.EaseInCubic => progress * progress * progress,
                AnimationEasing.EaseOutCubic => 1 - (float)Math.Pow(1 - progress, 3),
                AnimationEasing.EaseInOutCubic => progress < 0.5f 
                    ? 4 * progress * progress * progress 
                    : 1 - (float)Math.Pow(-2 * progress + 2, 3) / 2,
                AnimationEasing.MaterialStandard => CubicBezier(progress, 0.4f, 0.0f, 0.2f, 1.0f),
                AnimationEasing.MaterialDecelerate => CubicBezier(progress, 0.0f, 0.0f, 0.2f, 1.0f),
                AnimationEasing.MaterialAccelerate => CubicBezier(progress, 0.4f, 0.0f, 1.0f, 1.0f),
                AnimationEasing.FluentEaseOut => CubicBezier(progress, 0.0f, 0.0f, 0.0f, 1.0f),
                AnimationEasing.iOSSpring => Spring(progress, 300f, 30f),
                AnimationEasing.Elastic => EaseOutElastic(progress),
                _ => progress
            };
        }
        
        private static float CubicBezier(float t, float p1x, float p1y, float p2x, float p2y)
        {
            // Simplified cubic bezier (approximation)
            float u = 1 - t;
            return 3 * u * u * t * p1y + 3 * u * t * t * p2y + t * t * t;
        }
        
        private static float Spring(float t, float stiffness, float damping)
        {
            // Simplified spring physics
            float w = (float)Math.Sqrt(stiffness);
            float dampingRatio = damping / (2 * (float)Math.Sqrt(stiffness));
            
            if (dampingRatio < 1)
            {
                float wd = w * (float)Math.Sqrt(1 - dampingRatio * dampingRatio);
                return 1 - (float)Math.Exp(-dampingRatio * w * t) * (float)Math.Cos(wd * t);
            }
            
            return 1 - (float)Math.Exp(-w * t) * (1 + w * t);
        }
        
        private static float EaseOutElastic(float t)
        {
            const float c4 = (2 * (float)Math.PI) / 3;
            
            return t == 0 ? 0 : t == 1 ? 1
                : (float)Math.Pow(2, -10 * t) * (float)Math.Sin((t * 10 - 0.75f) * c4) + 1;
        }
    }
}

