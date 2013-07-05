using System;

namespace FeatureExtraction.Util
{
    public class Vector3D
    {
        // define commonly used vectors:
        public static Vector3D Zero = new Vector3D(0, 0, 0);
        public static Vector3D UnitX = new Vector3D(1, 0, 0);
        public static Vector3D UnitY = new Vector3D(0, 1, 0);
        public static Vector3D UnitZ = new Vector3D(0, 0, 1);
        public static Vector3D Right = UnitX;
        public static Vector3D Up = UnitY;
        public static Vector3D Forward = UnitZ;
        
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public Vector3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        // Scalar multiplication
        public static Vector3D operator *(double s, Vector3D v1)
        {
            return new Vector3D(v1.X * s, v1.Y * s, v1.Z * s);
        }

        // Vector addition
        public static Vector3D operator +(Vector3D v1, Vector3D v2)
        {
            return new Vector3D(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }

        // Vector subtraction
        public static Vector3D operator -(Vector3D v1, Vector3D v2)
        {
            return new Vector3D(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        }

        // Vector negation (i.e. additive inverse)
        public static Vector3D operator -(Vector3D v1)
        {
            return new Vector3D(-v1.X, -v1.Y, -v1.Z);
        }

        // * is vector dot product
        public static double operator *(Vector3D v1, Vector3D v2)
        {
            return v1.DotProduct(v2);
        }

        public double DotProduct(Vector3D other)
        {
            return (this.X * other.X) + (this.Y * other.Y) + (this.Z * other.Z);
        }

        /// <summary>
        /// Convert the vector to a unit directional vector.
        /// 
        /// The norm of the resulting vector is zero 
        ///     if and only if 
        /// the vector is the zero vector
        /// 
        /// Otherwise, the norm is 1.
        /// </summary>
        public void Normalise()
        {
            double norm = ComputeNorm();
            if (norm.Equals(0)) return;
            X /= norm;
            Y /= norm;
            Z /= norm;
        }

        private double ComputeNorm()
        {
            return Math.Sqrt((X * X) + (Y * Y) + (Z * Z));
        }

        /// <summary>
        /// Format the vector in a row-vector notation, (x,y,z).
        /// </summary>
        public override string ToString()
        {
            return String.Format("({0},{1},{2})", X, Y, Z);
        }
    }
}
