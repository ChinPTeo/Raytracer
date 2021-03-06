using System;

namespace RayTracer
{
    /// <summary>
    /// Class to represent an (infinite) plane in a scene.
    /// </summary>
    public class Plane : SceneEntity
    {
        private Vector3 center;
        private Vector3 normal;
        private Material material;

        /// <summary>
        /// Construct an infinite plane object.
        /// </summary>
        /// <param name="center">Position of the center of the plane</param>
        /// <param name="normal">Direction that the plane faces</param>
        /// <param name="material">Material assigned to the plane</param>
        public Plane(Vector3 center, Vector3 normal, Material material)
        {
            this.center = center;
            this.normal = normal.Normalized();
            this.material = material;
        }

        /// <summary>
        /// Determine if a ray intersects with the plane, and if so, return hit data.
        /// </summary>
        /// <param name="ray">Ray to check</param>
        /// <returns>Hit data (or null if no intersection)</returns>
        public RayHit Intersect(Ray ray)
        {
            double eps = 0.001;
            // there is intersection when dot product  is 0 it is parallel
            if ((ray.Direction).Dot(this.normal) >= eps)
            {
                return null;
            }
            else
            {
                Vector3 position = (((this.center - ray.Origin).Dot(this.normal) / (ray.Direction).Dot(this.normal)) * ray.Direction) + ray.Origin;
                // Console.WriteLine(position);

                return new RayHit(position, (this.normal), ray.Direction, this.material);
            }
        }

        public RayHit Shadow(Ray ray)
        {
            double eps = 0.01;
            // there is intersection when dot product  is 0 it is parallel
            if ((ray.Direction).Dot(this.normal) >= eps)
            {
                return null;
            }
            else
            {
                Vector3 position = ((this.center - ray.Origin).Dot(this.normal) / (ray.Direction).Dot(this.normal)) * ray.Direction;
                // Console.WriteLine(position);
                return new RayHit(position, this.normal, ray.Direction, this.material);
            }
        }

        /// <summary>
        /// The material of the plane.
        /// </summary>
        public Material Material { get { return this.material; } }
    }

}
