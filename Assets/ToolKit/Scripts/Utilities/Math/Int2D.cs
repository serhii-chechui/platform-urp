using UnityEngine;

namespace Bini.ToolKit.Core.Unity.Utilities.Math
{
    /// <summary>
    /// An int-based equivalent of Vector2. Unlike Vector2Int, has public fields.
    /// </summary>
    public struct Int2D : System.IEquatable<Int2D>
    {
        public int X;
        public int Y;

        public int this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return X;
                    case 1:
                        return Y;
                    default:
                        throw new System.IndexOutOfRangeException($"Invalid index: {index}");
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        X = value;
                        break;
                    case 1:
                        Y = value;
                        break;
                    default:
                        throw new System.IndexOutOfRangeException($"Invalid index: {index}");
                }
            }
        }

        public float Magnitude => Mathf.Sqrt(X * X + Y * Y);

        public int SquaredMagnitude => X * X + Y * Y;

        public Int2D(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static explicit operator Int2D(Vector2 value) => new Int2D((int) value.x, (int) value.y);

        public static implicit operator Vector2(Int2D value) => new Vector2(value.X, value.Y);

        public static implicit operator Int2D(Vector2Int value) => new Int2D(value.x, value.y);

        public static implicit operator Vector2Int(Int2D value) => new Vector2Int(value.X, value.Y);

        public static implicit operator Int2D((int X, int Y) value) => new Int2D(value.X, value.Y);

        public static implicit operator (int X, int Y)(Int2D value) => (value.X, value.Y);

        public static Int2D operator -(Int2D value) => new Int2D(-value.X, -value.Y);

        public static Int2D operator +(Int2D value1, Int2D value2) =>
            new Int2D(value1.X + value2.X, value1.Y + value2.Y);

        public static Int2D operator -(Int2D value1, Int2D value2) =>
            new Int2D(value1.X - value2.X, value1.Y - value2.Y);

        public static Int2D operator *(Int2D value1, Int2D value2) =>
            new Int2D(value1.X * value2.X, value1.Y * value2.Y);

        public static Int2D operator /(Int2D value1, Int2D value2) =>
            new Int2D(value1.X / value2.X, value1.Y / value2.Y);

        public static Int2D operator +(Int2D value1, int value2) => new Int2D(value1.X + value2, value1.Y + value2);

        public static Int2D operator -(Int2D value1, int value2) => new Int2D(value1.X - value2, value1.Y - value2);

        public static Int2D operator *(Int2D value1, int value2) => new Int2D(value1.X * value2, value1.Y * value2);

        public static Int2D operator /(Int2D value1, int value2) => new Int2D(value1.X / value2, value1.Y / value2);

        public static Int2D operator +(int value1, Int2D value2) => new Int2D(value1 + value2.X, value1 + value2.Y);

        public static Int2D operator -(int value1, Int2D value2) => new Int2D(value1 - value2.X, value1 - value2.Y);

        public static Int2D operator *(int value1, Int2D value2) => new Int2D(value1 * value2.X, value1 * value2.Y);

        public static Int2D operator /(int value1, Int2D value2) => new Int2D(value1 / value2.X, value1 / value2.Y);

        public static bool operator ==(Int2D value1, Int2D value2) => (value1.X == value2.X) && (value1.Y == value2.Y);

        public static bool operator !=(Int2D value1, Int2D value2) => (value1.X != value2.X) || (value1.Y != value2.Y);

        public bool Equals(Int2D other) => (other.X == X) && (other.Y == Y);

        public override bool Equals(object other) => (other is Int2D casted) && Equals(casted);

        public override int GetHashCode()
        {
            var hash1 = X.GetHashCode();
            var hash2 = Y.GetHashCode();
            return (hash1 << 5) + hash1 ^ hash2;
        }

        public void Deconstruct(out int x, out int y)
        {
            x = X;
            y = Y;
        }

        public override string ToString() => $"Int2D({X}, {Y})";

        public void Set(int x, int y)
        {
            X = x;
            Y = y;
        }

        public void Clamp(Int2D min, Int2D max)
        {
            if (X < min.X)
                X = min.X;
            else if (X > max.X)
                X = max.X;

            if (Y < min.Y)
                Y = min.Y;
            else if (Y > max.Y)
                Y = max.Y;
        }

        public int Min() => (X < Y ? X : Y);

        public int Max() => (X > Y ? X : Y);

        public Int2D Abs() => new Int2D((X < 0 ? -X : X), (Y < 0 ? -Y : Y));

        public static Int2D Floor(Vector2 value) => new Int2D(Mathf.FloorToInt(value.x), Mathf.FloorToInt(value.y));

        public static Int2D Ceil(Vector2 value) => new Int2D(Mathf.CeilToInt(value.x), Mathf.CeilToInt(value.y));

        public static Int2D Round(Vector2 value) => new Int2D(Mathf.RoundToInt(value.x), Mathf.RoundToInt(value.y));

        public static Int2D Min(Int2D value1, Int2D value2) =>
            new Int2D(Mathf.Min(value1.X, value2.X), Mathf.Min(value1.Y, value2.Y));

        public static Int2D Max(Int2D value1, Int2D value2) =>
            new Int2D(Mathf.Max(value1.X, value2.X), Mathf.Max(value1.Y, value2.Y));

        public static float Distance(Int2D vector1, Int2D vector2) => (vector2 - vector1).Magnitude;

        public static int Dot(Int2D vector1, Int2D vector2) => vector1.X * vector2.X + vector1.Y * vector2.Y;
    }
}