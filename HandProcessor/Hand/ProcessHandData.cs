using System.Collections.Generic;
using FeatureExtraction.Util;

namespace FeatureExtraction.Hand
{
    public class ProcessHandData
    {
        /*
        public Point GetPalm(List<Point> hull)
        {
            int countX = 0;
            int countY = 0;
            int countZ = 0;
            foreach (Point point in hull)
            {
                countX += point.x;
                countY += point.y;
                countZ += point.z;
            }
            return new Point(countX/hull.Count,countY/hull.Count,countZ/hull.Count);
        }

        public List<Point> getFingers(List<Point> hull, Point hand, Point palm, Point elbow)
        {
            //remove other points
            int cY = hand.y - elbow.y;
            int cX = hand.x - elbow.x;
            float grad = 1/(float) (cY)/(float) (cX);
            Point other = new Point(palm.x+100, (int)(palm.y+(100*grad)),palm.z);

            for(int i=0;i<hull.Count;i++)
            {
                if (PointComparator.isLeftExcl(palm, other, hull[i])) hull.RemoveAt(i--);
            }
            for (int i = 0; i < hull.Count;i++)
            {
                Point current = hull[i];
                for(int j=i+1;j<hull.Count;j++)
                {
                    Point compare = hull[j];
                    if (current.x - compare.x < 5 && current.x - compare.x > 5)
                    {
                         if (current.y - compare.y < 5 && current.y - compare.y > 5)
                         {
                             hull.RemoveAt(j--);
                         }
                    }
                }
            }
                return hull;
        }

        public List<List<Point>> GetHands(List<List<Point>> points, Point rightHand, Point leftHand)
        {
            List<List<Point>> hands = new List<List<Point>>();
            foreach (List<Point> lPoints in points)
            {
                if(ContainsPoint(lPoints,rightHand) || ContainsPoint(lPoints,leftHand))
                {
                      hands.Add(lPoints);
                }
            }
            return hands;
        }
        */

        public bool ContainsPoint(List<Point> points, Point point)
        {
            foreach (Point p in points)
            {
                if (p.x==point.x && p.y==point.y) return true;
            }
            return false;
        }

        public int[,] MirrorArray(int[,] data, int rX, int rY)
        {
            int [,] newData = new int[rX,rY]; 
            for(int y=0;y<rY;y++) for(int x=1;x<=rX;x++)
                newData[x-1, y] = data[rX-x,y];
            return newData;
        }

        public List<List<Point>> SplitData(int[,] points, int rX, int rY)
        {
            List<List<Point>> hands = new List<List<Point>>();
            for (int y = 0; y < rY; y++)
            {
                for(int x=0; x<rX;x++)
                {
                    if(points[x,y]>0)
                    {
                        List<Point> lPoints = new List<Point>();
                        GetPointsConnected(lPoints,points,x,y,rX,rY);
                        hands.Add(lPoints);
                    }
                }
            }
            return hands;
        }

        public void GetPointsConnected(List<Point> lPoints, int[,] points, int x, int y, int maxX, int maxY)
        {
            if (points[x,y] == 0) return;
            lPoints.Add(new Point(x,y,points[x,y]));
            points[x, y] = 0;
            if (x > 0) GetPointsConnected(lPoints,points,x-1,y,maxX,maxY);
            if (y > 0) GetPointsConnected(lPoints, points, x, y - 1, maxX, maxY);
            if (x < maxX - 1) GetPointsConnected(lPoints, points, x + 1, y, maxX, maxY);
            if (y < maxY - 1) GetPointsConnected(lPoints, points, x, y + 1, maxX, maxY);   
        }
    }
}
