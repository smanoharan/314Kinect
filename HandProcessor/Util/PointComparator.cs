namespace FeatureExtraction.Util
{
    public class PointComparator
    {
        private Point start;
        public PointComparator(Point _start)
        {
            start = _start;  //Set start point
        }

        public int Compare(object p1, object p2)
        {
            Point b = (Point) p1;
            Point c = (Point) p2;
            if (b.Equals(c)) return 0;                      //b and c are the same point
            if (isLeftExcl(start, b, c)) return -1;         //c is left of line though start and b
            if( isLeftIncl(start,b,c))                      //c is on the line through start and b
            {
                if (b.x >= start.x && b.y < c.y) return -1; //is b right of the start point and c is higher than b
                if (b.x < start.x && b.y > c.y) return -1;  //is b left of the start point and c is less than b
            }
            return 1;
        }

        public static bool isLeftIncl(Point a, Point b, Point c)
        {
            //Is point c left or on the line through a and b
            return ((b.x - a.x)*(c.y - a.y) - (b.y - a.y)*(c.x - a.x)) >= 0;
        }

        public static bool isLeftExcl(Point a, Point b, Point c)
        {
            //Is point c left of the line through a and b
            return ((b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x)) > 0;
        }
    }
}
