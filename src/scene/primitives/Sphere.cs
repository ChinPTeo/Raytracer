using System;

namespace RayTracer
{
    /// <summary>
    /// Class to represent an (infinite) plane in a scene.
    /// </summary>
    public class Sphere : SceneEntity
    {
        private Vector3 center;
        private double radius;
        private Material material;

        /// <summary>
        /// Construct a sphere given its center point and a radius.
        /// </summary>
        /// <param name="center">Center of the sphere</param>
        /// <param name="radius">Radius of the spher</param>
        /// <param name="material">Material assigned to the sphere</param>
        public Sphere(Vector3 center, double radius, Material material)
        {
            this.center = center;
            this.radius = radius;
            this.material = material;
        }

        /// <summary>
        /// Determine if a ray intersects with the sphere, and if so, return hit data.
        /// </summary>
        /// <param name="ray">Ray to check</param>
        /// <returns>Hit data (or null if no intersection)</returns>
        public RayHit Intersect(Ray ray)
        {
            //solve for tc
            Vector3 L = this.center - ray.Origin;
            double tc = L.Dot(ray.Direction);
            if (tc < 0.0) { return null; }

            Vector3 d = (tc * ray.Direction) - L;
            if (d.Length() > this.radius) { return null; }

            Vector3 t1 = new Vector3(0, 0, 0);
            if (L.LengthSq() < (this.radius * this.radius))
            {
                t1 = (tc + tc) * ray.Direction + ray.Origin;
                return new RayHit(t1, ((t1 - this.center).Normalized()), ray.Direction, this.material);
            }



            //solve for t1c
            double t1c = Math.Sqrt((this.radius * this.radius) - (d.LengthSq()));

            double t = tc - t1c;

            t1 = (t * ray.Direction) + ray.Origin;

            // 
            // solve for intersection points
            // if ((t1 - this.center).Length() > radius)
            // {
            //     Console.Write("tc ");
            //     Console.WriteLine(tc);

            //     Console.Write("t1c ");
            //     Console.WriteLine(t1c);

            //     Console.Write("t1 ");
            //     Console.WriteLine(t1.Length());
            // }

            return new RayHit(t1, (t1 - this.center).Normalized(), ray.Direction, this.material);
        }

        /// <summary>
        /// The material of the sphere.
        /// </summary>
        public Material Material { get { return this.material; } }
    }

}
