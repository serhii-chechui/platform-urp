using System.Collections.Generic;

namespace Bini.ToolKit.Core.Unity.Utilities.Spline
{
    public class CurveSegmentIterator<T>
    {
        public T Point0;
        public T Point1;
        public T Point2;
        public T Point3;

        private readonly IEnumerator<T> enumerator;
        private bool nextSegment;

        public CurveSegmentIterator(IEnumerable<T> enumerable)
        {
            enumerator = enumerable.GetEnumerator();
        }

        public CurveSegmentIterator(IEnumerator<T> enumerator)
        {
            this.enumerator = enumerator;
        }

        public bool MoveNext()
        {
            var moved = enumerator.MoveNext();

            if (moved)
            {
                var current = enumerator.Current;

                if (nextSegment)
                {
                    Point0 = Point1;
                    Point1 = Point2;
                    Point2 = Point3;
                    Point3 = current;
                }
                else
                {
                    Point0 = current;
                    Point1 = Point0;
                    moved = enumerator.MoveNext();

                    if (moved)
                    {
                        Point2 = enumerator.Current;
                        moved = enumerator.MoveNext();

                        if (moved)
                        {
                            Point3 = enumerator.Current;
                            nextSegment = true;
                        }
                        else
                            Point3 = Point2;
                    }
                    else
                    {
                        Point2 = Point0;
                        Point3 = Point0;
                        return false;
                    }
                }
            }
            else
            {
                if (!nextSegment)
                    return false;

                Point0 = Point1;
                Point1 = Point2;
                Point2 = Point3;
                nextSegment = false;
            }

            return true;
        }
    }
}