using System;

namespace Shared
{
    public static class Mathd
    {
        /// <summary>
        ///     PI / 4
        /// </summary>
        public const double QuarterPi = Math.PI / 4;

        /// <summary>
        ///     2 * PI
        /// </summary>
        public static double TwoPi = Math.PI * 2;

        /// <summary>
        ///     Clamps <paramref name="value" /> to inclusive 0..1 range
        /// </summary>
        public static double Clamp01(double value) => Clamp(value, 0, 1);

        /// <summary>
        ///     Clamps <paramref name="value" /> to inclusive min..max range
        /// </summary>
        public static double Clamp(double value, double min, double max) => Math.Max(min, Math.Min(max, value));

        /// <summary>
        ///     Returns true if <paramref name="value" /> is inclusive between <paramref name="min" /> and <paramref name="max" />,
        ///     false otherwise
        /// </summary>
        public static bool IsInRange(double value, double min, double max, double maxDelta = double.Epsilon) => AreClose(value, min, maxDelta)
                                                                                                                || value > min && value < max;
        /// <summary>
        ///     Returns true if <paramref name="value" /> is close to zero
        /// </summary>
        public static bool IsZero(double value, double maxDelta = double.Epsilon) => AreClose(value, 0, maxDelta);

        /// <summary>
        ///     Verifies whether <paramref name="value1" /> and <paramref name="value2" /> are approximately equal
        /// </summary>
        public static bool Approximately(double value1, double value2, double epsilon = double.Epsilon)
        {
            if (value1.Equals(value2))
                return true;

            double eps = (Math.Abs(value1) + Math.Abs(value2) + 10.0) * epsilon;
            double delta = value1 - value2;
            return -eps < delta && eps > delta;
        }

        /// <summary>
        ///     Verifies whether the difference between <paramref name="value1" /> and <paramref name="value2" /> is lesser than
        ///     <paramref name="maxDelta" />
        /// </summary>
        public static bool AreClose(double value1, double value2, double maxDelta = double.Epsilon)
        {
            double diff = value1 - value2;
            return Math.Abs(diff) <= maxDelta;
        }

        /// <summary>
        ///     Returns true if <paramref name="lesser" /> is lesser but not close to <paramref name="greater" />
        /// </summary>
        public static bool IsLesser(double lesser, double greater, double maxDelta = double.Epsilon) =>
            lesser < greater && !AreClose(lesser, greater, maxDelta);

        /// <summary>
        ///     Returns true if <paramref name="greater" /> is greater but not close to <paramref name="lesser" />
        /// </summary>
        public static bool IsGreater(double greater, double lesser, double maxDelta = double.Epsilon) =>
            greater > lesser && !AreClose(greater, lesser, maxDelta);

        /// <summary>
        ///     Returns true if <paramref name="greater" /> is greater or close to <paramref name="lesser" />
        /// </summary>
        public static bool IsGreaterOrClose(double greater, double lesser, double maxDelta) => AreClose(greater, lesser) || greater > lesser;

        /// <summary>
        ///     Returns true if <paramref name="lesser" /> is lesser or close to <paramref name="greater" />
        /// </summary>
        public static bool IsLesserOrClose(double lesser, double greater, double maxDelta) => AreClose(greater, lesser) || lesser < greater;

        /// <summary>
        ///     Returns a linear interpolation between <paramref name="from" /> and <paramref name="to" /> at
        ///     <paramref name="factor" />
        /// </summary>
        public static double Lerp(double from, double to, double factor) => from + (to - from) * factor;

        /// <summary>
        ///     Normalizes angle to 0-360 range
        /// </summary>
        public static double? NormalizeAngleDegrees(double? degrees)
        {
            if (!degrees.HasValue)
                return null;

            return NormalizeAngleDegrees(degrees.Value);
        }

        /// <summary>
        ///     Normalizes angle to 0-360 range
        /// </summary>
        public static double NormalizeAngleDegrees(double degrees)
        {
            degrees %= 360;
            if (degrees < 0)
                degrees += 360;

            return degrees;
        }
    }
}