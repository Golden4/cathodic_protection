using System;

namespace ParallelPipesIntervals.Core
{
    public struct IntervalDouble : IComparable<IntervalDouble>
    {
        public double x1;
        public double x2;
        
        public IntervalDouble(double x1, double x2)
        {
            if (x1 < x2)
            {
                this.x1 = x1;
                this.x2 = x2;
            }
            else
            {
                this.x1 = x2;
                this.x2 = x1;
            }
        }
        
        public IntervalDouble Log()
        {
            return new IntervalDouble((double) Math.Log(x1), (double) Math.Log(x2));
        }

        public IntervalDouble Sin()
        {
            return new IntervalDouble((double) Math.Sin(x1), (double) Math.Sin(x2));
        }

        public IntervalDouble Abs()
        {
            return new IntervalDouble((double) Math.Abs(x1), (double) Math.Abs(x2));
        }

        public IntervalDouble Sqrt()
        {
            return new IntervalDouble((double) Math.Sqrt(x1), (double) Math.Sqrt(x2));
        }
        
        public double Mid()
        {
            return (x1 + x2) / 2d;
        }

        public double Rad()
        {
            return Math.Abs(x1 - x2) / 2d;
        }

        public static IntervalDouble operator +(IntervalDouble a, IntervalDouble b)
        {
            return new IntervalDouble(a.x1 + b.x1, a.x2 + b.x2);
        }

        public static IntervalDouble operator -(IntervalDouble a, IntervalDouble b)
        {
            return new IntervalDouble(a.x1 - b.x2, a.x2 - b.x1);
        }

        public static IntervalDouble operator -(IntervalDouble a)
        {
            return new IntervalDouble(-a.x1, -a.x2);
        }

        public static IntervalDouble operator *(IntervalDouble a, IntervalDouble b)
        {
            double min1 = Math.Min(a.x1 * b.x1, a.x1 * b.x2);
            double min2 = Math.Min(a.x2 * b.x1, a.x2 * b.x2);
            double min0 = Math.Min(min2, min1);
            double max1 = Math.Max(a.x1 * b.x1, a.x1 * b.x2);
            double max2 = Math.Max(a.x2 * b.x1, a.x2 * b.x2);
            double max0 = Math.Max(max2, max1);
            return new IntervalDouble(min0, max0);
        }

        public static IntervalDouble operator /(IntervalDouble a, IntervalDouble b)
        {
            return (1f / b) * a;
        }

        public static IntervalDouble operator *(double a, IntervalDouble b)
        {
            return new IntervalDouble(a * b.x1, a * b.x2);
        }

        public static IntervalDouble operator /(double a, IntervalDouble b)
        {
            return new IntervalDouble(a / b.x1, a / b.x2);
        }

        public static IntervalDouble operator +(double a, IntervalDouble b)
        {
            return new IntervalDouble(a + b.x1, a + b.x2);
        }

        public static bool operator >(IntervalDouble a, IntervalDouble b)
        {
            return a.CompareTo(b) > 0;
        }

        public static bool operator <(IntervalDouble a, IntervalDouble b)
        {
            return a.CompareTo(b) < 0;
        }

        public static bool operator >=(IntervalDouble a, IntervalDouble b)
        {
            return a.CompareTo(b) >= 0;
        }

        public static bool operator <=(IntervalDouble a, IntervalDouble b)
        {
            return a.CompareTo(b) <= 0;
        }

        public override string ToString()
        {
            return ToString("0.000", 3);
        }

        public string ToString(string format, int num)
        {
            return string.Format("[{0," + (num * 2 + 1) + "};{1," + (num * 2 + 1) + "}]",
                Math.Round(x1, num).ToString(format), Math.Round(x2, num).ToString(format));
        }

        public int CompareTo(IntervalDouble other)
        {
            return (x2).CompareTo(other.x1);
        }
    }
}