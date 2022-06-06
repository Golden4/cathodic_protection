using System;

namespace CP_ParallelPipesForm.Core
{
    [Serializable]
    public struct Interval : IComparable<Interval>
    {
        public double x1;
        public double x2;

        public Interval(double x1, double x2)
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

        public Interval Log()
        {
            return new Interval(Math.Log(x1), Math.Log(x2));
        }

        public Interval Sin()
        {
            return new Interval(Math.Sin(x1), Math.Sin(x2));
        }

        public Interval Abs()
        {
            return new Interval(Math.Abs(x1), Math.Abs(x2));
        }

        public Interval Sqrt()
        {
            return new Interval(Math.Sqrt(x1), Math.Sqrt(x2));
        }

        public double Mid()
        {
            return (x1 + x2) / 2d;
        }

        public double Rad()
        {
            return Math.Abs(x1 - x2) / 2d;
        }

        public static Interval operator +(Interval a, Interval b)
        {
            return new Interval(a.x1 + b.x1, a.x2 + b.x2);
        }

        public static Interval operator -(Interval a, Interval b)
        {
            return new Interval(a.x1 - b.x2, a.x2 - b.x1);
        }

        public static Interval operator -(Interval a)
        {
            return new Interval(-a.x1, -a.x2);
        }

        public static Interval operator *(Interval a, Interval b)
        {
            double min1 = Math.Min(a.x1 * b.x1, a.x1 * b.x2);
            double min2 = Math.Min(a.x2 * b.x1, a.x2 * b.x2);
            double min0 = Math.Min(min2, min1);
            double max1 = Math.Max(a.x1 * b.x1, a.x1 * b.x2);
            double max2 = Math.Max(a.x2 * b.x1, a.x2 * b.x2);
            double max0 = Math.Max(max2, max1);
            return new Interval(min0, max0);
        }

        public static Interval operator /(Interval a, Interval b)
        {
            return (1d / b) * a;
        }

        public static Interval operator +(double a, Interval b)
        {
            return new Interval(a + b.x1, a + b.x2);
        }

        public static Interval operator -(double a, Interval b)
        {
            return new Interval(a, a) - b;
        }

        public static Interval operator *(double a, Interval b)
        {
            return new Interval(a * b.x1, a * b.x2);
        }

        public static Interval operator /(double a, Interval b)
        {
            return new Interval(a / b.x1, a / b.x2);
        }

        public static Interval operator +(Interval b, double a)
        {
            return new Interval(b.x1 + a, b.x2 + a);
        }
        public static Interval operator -(Interval a, double b)
        {
            return a - new Interval(b, b);
        }

        public static Interval operator *(Interval b, double a)
        {
            return new Interval(b.x1 * a, b.x2 * a);
        }

        public static Interval operator /(Interval b, double a)
        {
            return new Interval(b.x1 / a, b.x2 / a);
        }

        public static explicit operator Interval(double b) => new Interval(b, b);

        public static bool operator >(Interval a, Interval b)
        {
            return a.x1 > b.x2;
            // return a.CompareTo(b) > 0;
        }
        
        public static bool operator >=(Interval a, Interval b)
        {
            return a.x1 >= b.x2;
        }

        public static bool operator <(Interval a, Interval b)
        {
            return a.x2 < b.x1;
        }

        public static bool operator <=(Interval a, Interval b)
        {
            return a.x2 <= b.x1;
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

        public int CompareTo(Interval other)
        {
            return (x2).CompareTo(other.x1);
        }
    }
}