using System;

namespace RayTracer
{
    /// <summary>
    /// Immutable structure to represent a ray (origin, direction).
    /// </summary>
    public readonly struct Ray
    {
        private readonly Vector3 origin;
        private readonly Vector3 direction;
        private readonly double etai;

        /// <summary>
        /// Construct a new ray.
        /// </summary>
        /// <param name="origin">The starting position of the ray</param>
        /// <param name="direction">The direction of the ray</param>
        public Ray(Vector3 origin, Vector3 direction)
        {
            this.origin = origin;
            this.direction = direction;
            this.etai = 1;
        }
        public Ray(Vector3 origin, Vector3 direction, Double etai)
        {
            this.origin = origin;
            this.direction = direction;
            this.etai = etai;
        }

        /// <summary>
        /// The starting position of the ray.
        /// </summary>
        public Vector3 Origin { get { return this.origin; } }

        /// <summary>
        /// The direction of the ray.
        /// </summary>
        public Vector3 Direction { get { return this.direction; } }
        public Double Etai { get { return this.etai; } }
    }
}
