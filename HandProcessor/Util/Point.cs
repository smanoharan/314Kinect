using System;

namespace FeatureExtraction.Util
{
    public class Point
    {
        public int x;
        public int y;
        public int z;

        public Point(int _x, int _y, int _z)
        {
            //Create Point
            x = _x;
            y = _y;
            z = _z;
        }

        public override bool Equals(Object p1)
        {
            Point p = (Point) p1;
            if (p.x == x && p.y == y && p.z == z) return true; //Point is the same as p1
            return false; //Point is not equal to p1
        }
    }
}
