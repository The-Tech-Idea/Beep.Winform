using System;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Helpers
{
    // Compatibility wrapper to ensure both Math.Max and Math.max usages compile within this namespace.
    // Forwards to System.Math.
    internal static class Math
    {
        // Uppercase variants
        public static int Max(int a, int b) => System.Math.Max(a, b);
        public static float Max(float a, float b) => System.Math.Max(a, b);
        public static double Max(double a, double b) => System.Math.Max(a, b);

        public static int Min(int a, int b) => System.Math.Min(a, b);
        public static float Min(float a, float b) => System.Math.Min(a, b);
        public static double Min(double a, double b) => System.Math.Min(a, b);

        public static int Abs(int a) => System.Math.Abs(a);
        public static float Abs(float a) => System.Math.Abs(a);
        public static double Abs(double a) => System.Math.Abs(a);

        // Lowercase compatibility variants
        public static int max(int a, int b) => System.Math.Max(a, b);
        public static float max(float a, float b) => System.Math.Max(a, b);
        public static double max(double a, double b) => System.Math.Max(a, b);

        public static int min(int a, int b) => System.Math.Min(a, b);
        public static float min(float a, float b) => System.Math.Min(a, b);
        public static double min(double a, double b) => System.Math.Min(a, b);

        public static int abs(int a) => System.Math.Abs(a);
        public static float abs(float a) => System.Math.Abs(a);
        public static double abs(double a) => System.Math.Abs(a);
    }
}
