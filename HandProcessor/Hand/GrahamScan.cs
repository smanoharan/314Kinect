using System.Collections.Generic;
using System.Linq;
using FeatureExtraction.Util;

namespace FeatureExtraction.Hand
{
    public class GrahamScan
    {
        public static List<Point> Scan(List<Point> _points)
        {
            if (_points.Count() < 4) return _points;            //Only 3 or less points, so they all must be on the hull

            Point start = GetLowestPoint(_points);              //Get the lowest point in the list
            List<Point> points = RemovePoint(_points, start);   //remove the lowest point from the list
            List<Point> hull = new List<Point>();               
            PointComparator pc = new PointComparator(start);    //Create a new comparator for the list of points
            points.Sort(pc.Compare);                            //Sort points based on the angle from the start point

            //Add first 3 points
            hull.Add(start);
            for (int i = 0; i < 2; i++)
            {
                hull.Add(points[0]);
                points.RemoveAt(0);
            }

            //Find Points that are on the hull
            while(points.Count()>0)
            {
                Point p = points[0];
                Point a = hull[hull.Count() - 2];       //Get the second to last point on the hull
                Point b = hull[hull.Count() - 1];       //Get the last point on the hull
                if(PointComparator.isLeftIncl(a,b,p))
                {
                    hull.Add(p);                        //Point is on the hull, so add it to the hull
                    points.RemoveAt(0);                 //Remove the point of the list of points we are processing
                }
                else hull.RemoveAt(hull.Count() - 1);
            }
            return hull;
        }

        public static List<Point> RemovePoint(List<Point> _points, Point p)
        {
            List<Point> points = new List<Point>();
            for (int i = 0; i < _points.Count(); i++)
            {
                if (!_points[i].Equals(p)) points.Add(_points[i]);  //point is not the one we what to remove, so add it to the new list
            }
            return points;
        }

        public static Point GetLowestPoint(List<Point> points)
        {
            Point y = null;
            foreach(Point p in points)
            {
                if (y == null || p.y < y.y) y = p;   //Current point is lower than previous lowest point
                if (p.y == y.y && p.x > y.x) y = p;  //Current point is the same depth but further to the right
            }
            return y;
        }
    }
}
