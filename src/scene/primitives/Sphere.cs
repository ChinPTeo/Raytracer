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
            double L = this.center - ray.Origin;
            double tc = dot(L, ray.Direction);
            
            if ( tc < 0.0 ) {return null};
            double d2 = (tc*tc) - (L*L);
            
            double radius2 = this.radius * this.radius;
            if ( d2 > radius2){ return null};

            //solve for t1c
            double t1c = sqrt( radius2 - d2 );

            //solve for intersection points
            *t1 = tc - t1c;
            *t2 = tc + t1c;
            
            return new RayHit();
        }

        /// <summary>
        /// The material of the sphere.
        /// </summary>
        public Material Material { get { return this.material; } }
    }

}
