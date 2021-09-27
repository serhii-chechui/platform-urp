using UnityEngine;

namespace Bini.ToolKit.Core.Unity.Utilities.Math
{
    public struct IntRect : System.IEquatable<IntRect>
    {
        /// <summary>
        /// IntRect initialized to (min = int.MaxValue, max = int.MinValue).
        /// Intended to be used alongside with Encapsulate().
        /// </summary>
        public static IntRect Empty => new IntRect
        {
            Min = new Int2D {X = int.MaxValue, Y = int.MaxValue},
            Max = new Int2D {X = int.MinValue, Y = int.MinValue}
        };

        public Int2D Min;
        public Int2D Max;

        public bool IsFlipped => (Min.X > Max.X) || (Min.Y > Max.Y);

        public int X
        {
            get => Min.X;
            set
            {
                Max.X += value - Min.X;
                Min.X = value;
            }
        }

        public int Y
        {
            get => Min.Y;
            set
            {
                Max.Y += value - Min.Y;
                Min.Y = value;
            }
        }

        public int Width
        {
            get => Max.X - Min.X;
            set => Max.X = Min.X + value;
        }

        public int Height
        {
            get => Max.Y - Min.Y;
            set => Max.Y = Min.Y + value;
        }

        public Int2D Position
        {
            get => Min;
            set
            {
                Max = value + Size;
                Min = value;
            }
        }

        public Int2D Size
        {
            get => Max - Min;
            set => Max = Min + value;
        }

        public Int2D AbsMin => new Int2D(Mathf.Min(Min.X, Max.X), Mathf.Min(Min.Y, Max.Y));

        public Int2D AbsMax => new Int2D(Mathf.Max(Min.X, Max.X), Mathf.Max(Min.Y, Max.Y));

        public Int2D AbsSize => Size.Abs();

        public IntRect Abs => new IntRect {Min = AbsMin, Max = AbsMax};

        public Vector2 Extents => new Vector2((Max.X - Min.X) * 0.5f, (Max.Y - Min.Y) * 0.5f);

        public Vector2 Center => new Vector2(Min.X + (Max.X - Min.X) * 0.5f, Min.Y + (Max.Y - Min.Y) * 0.5f);

        public int Area => (Max.X - Min.X) * (Max.Y - Min.Y);

        public IntRect(int x, int y, int width, int height)
        {
            Min.X = x;
            Min.Y = y;
            Max.X = x + width;
            Max.Y = y + height;
        }

        public IntRect(Int2D position, Int2D size)
        {
            Min = position;
            Max = position + size;
        }

        public static explicit operator IntRect(Rect rect) =>
            new IntRect((int) rect.x, (int) rect.y, (int) rect.width, (int) rect.height);

        public static implicit operator Rect(IntRect rect) => new Rect(rect.X, rect.Y, rect.Width, rect.Height);

        public static implicit operator IntRect(RectInt rect) => new IntRect(rect.x, rect.y, rect.width, rect.height);

        public static implicit operator RectInt(IntRect rect) => new RectInt(rect.X, rect.Y, rect.Width, rect.Height);

        public static bool operator ==(IntRect a, IntRect b) =>
            (a.Min.X == b.Min.X) && (a.Min.Y == b.Min.Y) &&
            (a.Max.X == b.Max.X) && (a.Max.Y == b.Max.Y);

        public static bool operator !=(IntRect a, IntRect b) =>
            (a.Min.X != b.Min.X) || (a.Min.Y != b.Min.Y) ||
            (a.Max.X != b.Max.X) || (a.Max.Y != b.Max.Y);

        public bool Equals(IntRect other) =>
            (other.Min.X == Min.X) && (other.Min.Y == Min.Y) &&
            (other.Max.X == Max.X) && (other.Max.Y == Max.Y);

        public override bool Equals(object other) => (other is IntRect casted) && Equals(casted);

        public override int GetHashCode()
        {
            var hash1 = Min.GetHashCode();
            var hash2 = Max.GetHashCode();
            return (hash1 << 5) + hash1 ^ hash2;
        }

        public override string ToString() => $"IntRect({X}, {Y}, {Width}, {Height})";

        /// <summary>
        /// Returns the IntRect that is fully within the given rect
        /// </summary>
        public static IntRect Floor(Rect rect) => new IntRect {Min = Int2D.Ceil(rect.min), Max = Int2D.Floor(rect.max)};

        /// <summary>
        /// Returns the IntRect which contains the given rect
        /// </summary>
        public static IntRect Ceil(Rect rect) => new IntRect {Min = Int2D.Floor(rect.min), Max = Int2D.Ceil(rect.max)};

        /// <summary>
        /// Rounds the position and size of the given rect 
        /// </summary>
        public static IntRect Round(Rect rect) => new IntRect(Int2D.Round(rect.min), Int2D.Round(rect.size));

        public void Set(int x, int y, int width, int height)
        {
            Min.X = x;
            Min.Y = y;
            Max.X = x + width;
            Max.Y = y + height;
        }

        public void Set(Int2D position, Int2D size)
        {
            Min = position;
            Max = position + size;
        }

        public void Encapsulate(Int2D point)
        {
            Encapsulate(point.X, point.Y);
        }

        public void Encapsulate(int x, int y)
        {
            if (x < Min.X)
                Min.X = x;

            if (x > Max.X)
                Max.X = x;

            if (y < Min.Y)
                Min.Y = y;

            if (y > Max.Y)
                Max.Y = y;
        }

        public void Encapsulate(IntRect rect)
        {
            if (rect.Min.X < Min.X)
                Min.X = rect.Min.X;

            if (rect.Max.X > Max.X)
                Max.X = rect.Max.X;

            if (rect.Min.Y < Min.Y)
                Min.Y = rect.Min.Y;

            if (rect.Max.Y > Max.Y)
                Max.Y = rect.Max.Y;
        }

        public void Intersect(IntRect rect)
        {
            if (rect.Min.X > Min.X)
                Min.X = rect.Min.X;

            if (rect.Max.X < Max.X)
                Max.X = rect.Max.X;

            if (rect.Min.Y > Min.Y)
                Min.Y = rect.Min.Y;

            if (rect.Max.Y < Max.Y)
                Max.Y = rect.Max.Y;
        }

        public IntRect Intersection(IntRect rect)
        {
            rect.Intersect(this);
            return rect;
        }

        public void Expand(Int2D amount)
        {
            Expand(amount.X, amount.Y);
        }

        public void Expand(int amount)
        {
            Expand(amount, amount);
        }

        public void Expand(int x, int y)
        {
            Min.X -= x;
            Min.Y -= y;
            Max.X += x;
            Max.Y += y;
        }

        // necessary due to inapplicability of the "center" concept
        public void Move(Int2D offset)
        {
            Move(offset.X, offset.Y);
        }

        public void Move(int x, int y)
        {
            Min.X += x;
            Max.X += x;
            Min.Y += y;
            Max.Y += y;
        }

        public bool Contains(Int2D point, bool maxEdge = true) =>
            maxEdge
                ? (point.X >= Min.X) && (point.Y >= Min.Y) && (point.X <= Max.X) && (point.Y <= Max.Y)
                : (point.X >= Min.X) && (point.Y >= Min.Y) && (point.X < Max.X) && (point.Y < Max.Y);

        public bool Contains(IntRect rect) =>
            (rect.Min.X >= Min.X) && (rect.Min.Y >= Min.Y) && (rect.Max.X <= Max.X) && (rect.Max.Y <= Max.Y);

        public bool Overlaps(IntRect rect)
        {
            rect.Intersect(this);
            return !rect.IsFlipped;
        }
    }
}