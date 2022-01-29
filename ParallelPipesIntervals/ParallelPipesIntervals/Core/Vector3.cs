using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Text;

namespace ParallelPipesIntervals.Core
{
    [Serializable]
    public struct Vector3<T> : IEquatable<Vector3<T>>, IFormattable
    {
        private T _x;
        private T _y;
        private T _z;

        public T X => _x;
        public T Y => _y;
        public T Z => _z;

        public Vector3(T value)
            : this(value, value, value)
        {
        }

        public Vector3(T x, T y, T z)
        {
            this._x = x;
            this._y = y;
            this._z = z;
        }
        static bool IsPropertyOrMethodExist(dynamic settings, string name)
        {
            if (settings is ExpandoObject)
                return ((IDictionary<string, object>)settings).ContainsKey(name);
            return settings.GetType().GetMethod(name) != null || settings.GetType().GetProperty(name) != null;
        }

        public static float Dot(Vector3<T> vector1, Vector3<T> vector2) =>
            ((dynamic) vector1.X * vector2.X + (dynamic) vector1.Y * vector2.Y + (dynamic) vector1.Z * vector2.Z);

        public static Vector3<T> Min(Vector3<T> value1, Vector3<T> value2) => new Vector3<T>(
            (dynamic) value1.X < value2.X ? value1.X : value2.X, (dynamic) value1.Y < value2.Y ? value1.Y : value2.Y,
            (dynamic) value1.Z < value2.Z ? value1.Z : value2.Z);

        public static Vector3<T> Max(Vector3<T> value1, Vector3<T> value2) => new Vector3<T>(
            (dynamic) value1.X > value2.X ? value1.X : value2.X, (dynamic) value1.Y > value2.Y ? value1.Y : value2.Y,
            (dynamic) value1.Z > value2.Z ? value1.Z : value2.Z);

        public static Vector3<T> Abs(Vector3<T> value)
        {
            dynamic x = value.X;
            dynamic y = value.Y;
            dynamic z = value.Z;
            return new Vector3<T>(IsPropertyOrMethodExist(x, "Abs") ? x.Abs() : Math.Abs(x), 
                IsPropertyOrMethodExist(y, "Abs") ? y.Abs() : Math.Abs(y),
                IsPropertyOrMethodExist(z, "Abs") ? z.Abs() : Math.Abs(z));
        }

        public static Vector3<T> Sqrt(Vector3<T> value)
        {
            dynamic x = value.X;
            dynamic y = value.Y;
            dynamic z = value.Z;
            return new Vector3<T>(IsPropertyOrMethodExist(x, "Sqrt") ? x.Sqrt() : Math.Sqrt(x), 
                IsPropertyOrMethodExist(y, "Sqrt") ? y.Sqrt() : Math.Sqrt(y),
                IsPropertyOrMethodExist(z, "Sqrt") ? z.Sqrt() : Math.Sqrt(z));
        }

        public static Vector3<T> operator +(Vector3<T> left, Vector3<T> right) =>
            new Vector3<T>((dynamic) left.X + right.X, (dynamic) left.Y + right.Y, (dynamic) left.Z + right.Z);

        public static Vector3<T> operator -(Vector3<T> left, Vector3<T> right) =>
            new Vector3<T>((dynamic) left.X - right.X, (dynamic) left.Y - right.Y, (dynamic) left.Z - right.Z);

        public static Vector3<T> operator *(Vector3<T> left, Vector3<T> right) =>
            new Vector3<T>((dynamic) left.X * right.X, (dynamic) left.Y * right.Y, (dynamic) left.Z * right.Z);

        public static Vector3<T> operator *(Vector3<T> left, T right) => left * new Vector3<T>(right);
        public static Vector3<T> operator *(T left, Vector3<T> right) => new Vector3<T>(left) * right;

        public static Vector3<T> operator /(Vector3<T> left, Vector3<T> right) =>
            new Vector3<T>((dynamic) left.X / right.X, (dynamic) left.Y / right.Y, (dynamic) left.Z / right.Z);

        public static Vector3<T> operator /(Vector3<T> value1, T value2)
        {
            return new Vector3<T>((dynamic) value1.X / value2, (dynamic) value1.Y / value2,
                (dynamic) value1.Z / value2);
        }

        public static Vector3<T> operator -(Vector3<T> value) =>
            new Vector3<T>(-(dynamic) value.X, -(dynamic) value.Y, -(dynamic) value.Z);

        public static bool operator ==(Vector3<T> left, Vector3<T> right) =>
            (dynamic) left.X == right.X && (dynamic) left.Y == right.Y && (dynamic) left.Z == right.Z;

        public static bool operator !=(Vector3<T> left, Vector3<T> right) =>
            (dynamic) left.X != right.X || (dynamic) left.Y != right.Y || (dynamic) left.Z != right.Z;

        public static T DistanceSquared(Vector3<T> value1, Vector3<T> value2)
        {
            dynamic num1 = (dynamic)value1.X - value2.X;
            dynamic num2 = (dynamic)value1.Y - value2.Y;
            dynamic num3 = (dynamic)value1.Z - value2.Z;
            return (num1 * num1 + num2 * num2 + num3 * num3);
        }

        public static T Distance(Vector3<T> v1, Vector3<T> v2)
        {
            dynamic sum = DistanceSquared(v1, v2);
            return IsPropertyOrMethodExist(sum, "Sqrt") ? sum.Sqrt() : Math.Sqrt(sum);
        }

        public override string ToString() => this.ToString("G", (IFormatProvider) CultureInfo.CurrentCulture);
        public string ToString(string format) => this.ToString(format, (IFormatProvider) CultureInfo.CurrentCulture);

        public string ToString(string format, IFormatProvider formatProvider)
        {
            StringBuilder stringBuilder = new StringBuilder();
            string numberGroupSeparator = NumberFormatInfo.GetInstance(formatProvider).NumberGroupSeparator;
            stringBuilder.Append('<');
            stringBuilder.Append(((IFormattable) this.X).ToString(format, formatProvider));
            stringBuilder.Append(numberGroupSeparator);
            stringBuilder.Append(' ');
            stringBuilder.Append(((IFormattable) this.Y).ToString(format, formatProvider));
            stringBuilder.Append(numberGroupSeparator);
            stringBuilder.Append(' ');
            stringBuilder.Append(((IFormattable) this.Z).ToString(format, formatProvider));
            stringBuilder.Append('>');
            return stringBuilder.ToString();
        }

        public override bool Equals(object obj)
        {
            return obj is Vector3<T> other && Equals(other);
        }

        public bool Equals(Vector3<T> other) =>
            (dynamic) this.X == other.X && (dynamic) this.Y == other.Y && (dynamic) this.Z == other.Z;

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = EqualityComparer<T>.Default.GetHashCode(X);
                hashCode = (hashCode * 397) ^ EqualityComparer<T>.Default.GetHashCode(Y);
                hashCode = (hashCode * 397) ^ EqualityComparer<T>.Default.GetHashCode(Z);
                return hashCode;
            }
        }
    }
}