using System;
using System.Collections.Generic;

namespace RayTracer
{
    /// <summary>
    /// Class to represent a ray traced scene, including the objects,
    /// light sources, and associated rendering logic.
    /// </summary>
    public class Scene
    {
        private SceneOptions options;
        private ISet<SceneEntity> entities;
        private ISet<PointLight> lights;

        /// <summary>
        /// Construct a new scene with provided options.
        /// </summary>
        /// <param name="options">Options data</param>
        public Scene(SceneOptions options = new SceneOptions())
        {
            this.options = options;
            this.entities = new HashSet<SceneEntity>();
            this.lights = new HashSet<PointLight>();
        }

        /// <summary>
        /// Add an entity to the scene that should be rendered.
        /// </summary>
        /// <param name="entity">Entity object</param>
        public void AddEntity(SceneEntity entity)
        {
            this.entities.Add(entity);
        }

        /// <summary>
        /// Add a point light to the scene that should be computed.
        /// </summary>
        /// <param name="light">Light structure</param>
        public void AddPointLight(PointLight light)
        {
            this.lights.Add(light);
        }

        /// <summary>
        /// Render the scene to an output image. This is where the bulk
        /// of your ray tracing logic should go... though you may wish to
        /// break it down into multiple functions as it gets more complex!
        /// </summary>
        /// <param name="outputImage">Image to store render output</param>
        public void Render(Image outputImage)
        {
            /*Console.WriteLine(outputImage.Height);*/


            // Initializing variables
            double fov = 60;
            Vector3 origin = new Vector3(0, 0, 0);
            double imageAspectRatio = outputImage.Width / outputImage.Height; // assuming width > height 
            double scale = Math.Tan(fov / 2 * Math.PI / 180);

            // Initializing a 2D array of rays, origin at 0,0,0 and direction of pixel
            List<List<Ray>> rays = new List<List<Ray>>();
            for (int j = 0; j < outputImage.Height; j++)
            {
                for (int i = 0; i < outputImage.Width; i++)
                {

                    double x = (2 * ((i + 0.5) / outputImage.Width) - 1) * scale * imageAspectRatio;
                    double y = (1 - 2 * ((j + 0.5) / outputImage.Height)) * scale;
                    Vector3 rayDirection = new Vector3(x, y, 1);
                    rayDirection = rayDirection - origin;
                    rayDirection = rayDirection.Normalized();
                    Ray ray = new Ray(origin, rayDirection);


                    //     Console.WriteLine(ray.Direction);
                    // if ((x == 0) && (y == 0))
                    // {
                    // }


                    outputImage.SetPixel(i, j, new Color(0.0, 0.0, 0.0));
                    double zdepth = -1;
                    foreach (SceneEntity entity in this.entities)
                    {
                        RayHit hit = entity.Intersect(ray);
                        if (hit != null)
                        {
                            // We got a hit with this entity!
                            double new_zdepth = hit.Position.LengthSq();
                            // The colour of the entity is entity.Material.Color
                            if ((zdepth == -1) || (zdepth > new_zdepth))
                            { outputImage.SetPixel(i, j, entity.Material.Color); }
                            // continue;
                        }

                    }


                }

                /*            double imageAspectRatio = options.width / (double)options.height;*/

                /*OutputImageWidth;
                    OutputImageHeight;*/
                // CHeck Hit?
                // Profit

            }

        }
    }
}
