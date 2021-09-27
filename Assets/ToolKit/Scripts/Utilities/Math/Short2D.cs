using UnityEngine;

namespace Bini.ToolKit.Core.Unity.Utilities.Math
{
    public struct Short2D : System.IEquatable<Short2D>
    {
        public short X;
        public short Y;

        public short this[int index]
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

        public Short2D(int x, int y)
        {
            X = (short) x;
            Y = (short) y;
        }

        public Short2D(short x, short y)
        {
            X = x;
            Y = y;
        }

        public static implicit operator Short2D(Int2D value) => new Short2D(value.X, value.Y);

        public static implicit operator Int2D(Short2D value) => new Int2D(value.X, value.Y);

        public static explicit operator Short2D(Vector2 value) => new Short2D((short) value.x, (short) value.y);

        public static implicit operator Vector2(Short2D value) => new Vector2(value.X, value.Y);

        public static implicit operator Short2D(Vector2Int value) => new Short2D(value.x, value.y);

        public static implicit operator Vector2Int(Short2D value) => new Vector2Int(value.X, value.Y);

        public static implicit operator Short2D((int X, int Y) value) => new Short2D(value.X, value.Y);

        public static implicit operator (int X, int Y)(Short2D value) => (value.X, value.Y);

        public static implicit operator Short2D((short X, short Y) value) => new Short2D(value.X, value.Y);

        public static implicit operator (short X, short Y)(Short2D value) => (value.X, value.Y);

        public static Short2D operator -(Short2D value) => new Short2D(-value.X, -value.Y);

        public static Short2D operator +(Short2D value1, Short2D value2) =>
            new Short2D(value1.X + value2.X, value1.Y + value2.Y);

        public static Short2D operator -(Short2D value1, Short2D value2) =>
            new Short2D(value1.X - value2.X, value1.Y - value2.Y);

        public static Short2D operator *(Short2D value1, Short2D value2) =>
            new Short2D(value1.X * value2.X, value1.Y * value2.Y);

        public static Short2D operator /(Short2D value1, Short2D value2) =>
            new Short2D(value1.X / value2.X, value1.Y / value2.Y);

        public static Short2D operator +(Short2D value1, int value2) =>
            new Short2D(value1.X + value2, value1.Y + value2);

        public static Short2D operator -(Short2D value1, int value2) =>
            new Short2D(value1.X - value2, value1.Y - value2);

        public static Short2D operator *(Short2D value1, int value2) =>
            new Short2D(value1.X * value2, value1.Y * value2);

        public static Short2D operator /(Short2D value1, int value2) =>
            new Short2D(value1.X / value2, value1.Y / value2);

        public static Short2D operator +(int value1, Short2D value2) =>
            new Short2D(value1 + value2.X, value1 + value2.Y);

        public static Short2D operator -(int value1, Short2D value2) =>
            new Short2D(value1 - value2.X, value1 - value2.Y);

        public static Short2D operator *(int value1, Short2D value2) =>
            new Short2D(value1 * value2.X, value1 * value2.Y);

        public static Short2D operator /(int value1, Short2D value2) =>
            new Short2D(value1 / value2.X, value1 / value2.Y);

        public static bool operator ==(Short2D value1, Short2D value2) =>
            (value1.X == value2.X) && (value1.Y == value2.Y);

        public static bool operator !=(Short2D value1, Short2D value2) =>
            (value1.X != value2.X) || (value1.Y != value2.Y);


        public bool Equals(Short2D other) => (other.X == X) && (other.Y == Y);

        public override bool Equals(object other) => (other is Short2D casted) && Equals(casted);

        public override int GetHashCode()
        {
            var hash1 = X.GetHashCode();
            var hash2 = Y.GetHashCode();
            return (hash1 << 5) + hash1 ^ hash2;
        }

        public void Deconstruct(out short x, out short y)
        {
            x = X;
            y = Y;
        }

        public override string ToString() => $"Int2D({X}, {Y})";

        public void Set(int x, int y)
        {
            X = (short) x;
            Y = (short) y;
        }

        public void Clamp(Short2D min, Short2D max)
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

        public short Min() => (X < Y ? X : Y);

        public short Max() => (X > Y ? X : Y);

        public Short2D Abs() => new Short2D((X < 0 ? -X : X), (Y < 0 ? -Y : Y));

        public static Short2D Floor(Vector2 value) => new Short2D(Mathf.FloorToInt(value.x), Mathf.FloorToInt(value.y));

        public static Short2D Ceil(Vector2 value) => new Short2D(Mathf.CeilToInt(value.x), Mathf.CeilToInt(value.y));

        public static Short2D Round(Vector2 value) => new Short2D(Mathf.RoundToInt(value.x), Mathf.RoundToInt(value.y));

        public static Short2D Min(Short2D value1, Short2D value2) =>
            new Short2D(Mathf.Min(value1.X, value2.X), Mathf.Min(value1.Y, value2.Y));

        public static Short2D Max(Short2D value1, Short2D value2) =>
            new Short2D(Mathf.Max(value1.X, value2.X), Mathf.Max(value1.Y, value2.Y));

        public static float Distance(Short2D vector1, Short2D vector2) => (vector2 - vector1).Magnitude;

        public static int Dot(Short2D vector1, Short2D vector2) => vector1.X * vector2.X + vector1.Y * vector2.Y;
    }
}