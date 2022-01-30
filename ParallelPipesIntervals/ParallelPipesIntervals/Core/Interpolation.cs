namespace ParallelPipesIntervals.Core
{
    public static class Interpolation
    {
        public static Interval InterpolateTwoPoints(double x, double x0, double x1, Interval y0, Interval y1)
        {
            var yi0 = InterpolateTwoPoints(x, x0, x1, y0.x1, y1.x1);
            var yi1 = InterpolateTwoPoints(x, x0, x1, y0.x2, y1.x2);
            return new Interval(yi0, yi1);    
        }
        public static T InterpolateTwoPoints<T>(double x, double x0, double x1, T y0, T y1)
        {
            if ((x1 - x0) == 0)
            {
                return ((dynamic)y0 + y1) / 2;
            }
            return y0 + (x - x0) * ((dynamic)y1 - y0) / (x1 - x0);
        }

        public static bool IsBetweenTwoPoints<T>(T value, T value1, T value2)
        {
            return (dynamic)value >= value1 && (dynamic)value <= value2;
        }

        public static Interval LinearInterpolation(double xValue, double[] xPoints, Interval[] yPoints)
        {
            double x0 = xPoints[0];
            double x1 = xPoints[1];
            Interval y0 = yPoints[0];
            Interval y1 = yPoints[1];
            for (int i = 0; i < xPoints.Length - 1; i++)
            {
                if (i == xPoints.Length - 2 || IsBetweenTwoPoints(xValue, xPoints[i], xPoints[i+1]))
                {
                    x0 = xPoints[i];
                    x1 = xPoints[i + 1];
                    y0 = yPoints[i];
                    y1 = yPoints[i + 1];
                    break;
                }
            }
            return InterpolateTwoPoints(xValue, x0, x1, y0, y1);
        }
    }
}