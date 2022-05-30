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
        public static double InterpolateTwoPoints(double x, double x0, double x1, double y0, double y1)
        {
            if (x1 - x0 == 0)
            {
                return (y0 + y1) / 2;
            }
            return y0 + (x - x0) * (y1 - y0) / (x1 - x0);
        }

        public static bool IsBetweenTwoPoints<T>(T value, T value1, T value2)
        {
            return (dynamic)value >= value1 && (dynamic)value <= value2;
        }

        public static Interval LinearInterpolation(double xValue, double[] xPoints, Interval[] yPoints)
        {
            int indexStart, indexEnd;
            FindPointIndexes(xValue, xPoints, out indexStart, out indexEnd);
            double x0 = xPoints[indexStart];
            double x1 = xPoints[indexEnd];
            Interval y0 = yPoints[indexStart];
            Interval y1 = yPoints[indexEnd];
            return InterpolateTwoPoints(xValue, x0, x1, y0, y1);
        }
        
        public static double LinearInterpolation(double xValue, double[] xPoints, double[] yPoints)
        {
            int indexStart, indexEnd;
            FindPointIndexes(xValue, xPoints, out indexStart, out indexEnd);
            double x0 = xPoints[indexStart];
            double x1 = xPoints[indexEnd];
            double y0 = yPoints[indexStart];
            double y1 = yPoints[indexEnd];
            return InterpolateTwoPoints(xValue, x0, x1, y0, y1);
        }

        static void FindPointIndexes<T>(T xValue, T[] xPoints, out int indexStart, out int indexEnd)
        {
            indexStart = 0;
            indexEnd = 1;
            for (int i = 0; i < xPoints.Length - 1; i++)
            {
                if (i == xPoints.Length - 2 ||
                    IsBetweenTwoPoints(xValue, xPoints[i], xPoints[i+1]))
                {
                    indexStart = i;
                    indexEnd = i + 1;
                    break;
                }
            }
        }
    }
}