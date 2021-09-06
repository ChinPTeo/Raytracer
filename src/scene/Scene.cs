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
            const int max_bounce = 10;
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
                    

                    Color pixel_color = Colorizer(ray, max_bounce, i, j);
                    outputImage.SetPixel(i, j, pixel_color);


                }

                /*            double imageAspectRatio = options.width / (double)options.height;*/

                /*OutputImageWidth;
                    OutputImageHeight;*/
                // CHeck Hit?
                // Profit

            }

        }

        public Color Colorizer(Ray ray, int bounce, int i, int j)
        {
            Color pixelColor = new Color(0, 0, 0);
            if (bounce == 0)
            {
                return pixelColor;
            }
            SceneEntity newEntity = null;
            double zdepth = -1;
            RayHit storedHit = null;
            // Looping through objects
            foreach (SceneEntity entity in this.entities)
            {
                RayHit hit = entity.Intersect(ray);
                if (hit != null)
                {
                    // Selecting closest obj
                    if ((zdepth == -1) || (zdepth > hit.Position.LengthSq()))
                    {
                        newEntity = entity;
                        zdepth = hit.Position.LengthSq();
                        storedHit = hit;
                    }
                    if ((i == 300) && (j == 300))
                    {
                        Console.WriteLine(entity);
                    }
                    
                }
            }
            // ONly null or triangles
            if (storedHit != null)
            {//Make random ray
            double lum=1;
            // return newEntity.Material.Color;
            foreach (PointLight light in this.lights){

                // Decide on what side the shadow ray should be spawned
                // if (newEntity.)
                    Vector3 origin = storedHit.Position + storedHit.Normal*storedHit.Normal.Dot(storedHit.Incident) * 0.005;
                    Ray shadowRay = new Ray(origin, (light.Position - origin).Normalized());
                    foreach (SceneEntity entity in this.entities)
                    {
                        //
                        RayHit shwHit = entity.Intersect(shadowRay);
                        if ((shwHit != null &&((light.Position-origin).LengthSq() > (shwHit.Position-origin).LengthSq())) )
                        {
                            if ((i == 300) && (j == 300))
                    {
                        Console.Write("shadow: ");
                        Console.WriteLine(entity.Material.Type);
                        Console.WriteLine(shwHit.Position);

                    }
                            lum=0.3;
//                     Color fixing = new Color(storedHit.Normal.X + 1, storedHit.Normal.Y + 1, storedHit.Normal.Z + 1);
// pixelColor +=  fixing*.1;

//                     return pixelColor;
                        }
                    }
                }
             // Vector3 new_p = hit.Normal + hit.Position + random_in_unit_sphere();
             // Ray new_ray = new Ray(hit.Position, (new_p - hit.Position).Normalized());
             // Console.WriteLine(newEntity);
                Material.MaterialType diffuse =  Material.MaterialType.Diffuse;
                Material.MaterialType reflective =  Material.MaterialType.Reflective;
                // if(newEntity.Material.Type == (Material.MaterialType.Diffuse)){
                foreach (PointLight light in this.lights)
                {
                    

                    // pixelColor = entity.Material.Color * (new Color(0.5, 0.5, 0.5) * Colorizer(new_ray, bounce - 1));
                    lum = (storedHit.Normal.Dot((light.Position - storedHit.Position).Normalized())) * lum;
                    // Console.WriteLine(a_color);
                    if (lum < 0)
                    {
                        lum = 0;
                    }
                    // Color fixing = new Color(storedHit.Normal.X + 1, storedHit.Normal.Y + 1, storedHit.Normal.Z + 1);

                    pixelColor += light.Color * newEntity.Material.Color * lum;
                    // pixelColor +=  fixing*.5;

                    return pixelColor;
                }
                
                // } 
                
                // else if (newEntity.Material.Type == (Material.MaterialType.Reflective)){
                //     Ray reflectRay = new Ray(storedHit.Position, storedHit.Incident - 2*storedHit.Incident.Dot(storedHit.Normal)*storedHit.Normal);
                //     return Colorizer(reflectRay, bounce-1);
                // }
            }
            return pixelColor;
        }
        // F
        Vector3 random_in_unit_sphere()
        {
            var rand = new Random();
            while (true)
            {
                Vector3 p = new Vector3(rand.NextDouble() * 2 - 1, rand.NextDouble() * 2 - 1, rand.NextDouble() * 2 - 1);
                if (p.LengthSq() >= 1) { continue; }
                return p;
            }
        }
    }
}
