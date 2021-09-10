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
            const int max_bounce = 15;
            int samples_per_pixel = 1;
            // Initializing a 2D array of rays, origin at 0,0,0 and direction of pixel
            List<List<Ray>> rays = new List<List<Ray>>();
            for (int j = 0; j < outputImage.Height; j++)
            {
                for (int i = 0; i < outputImage.Width; i++)
                {
                    Color pixelColor = new Color(0, 0, 0);
                    for (int s = 0; s < samples_per_pixel; s++)
                    {
                        double x, y;
                        if (samples_per_pixel == 1)
                        {
                            x = (2 * ((i + 0.5) / outputImage.Width) - 1) * scale * imageAspectRatio;
                            y = (1 - 2 * ((j + 0.5) / outputImage.Height)) * scale;
                        }
                        else
                        {
                            x = (2 * ((i + 0.5 + AAhelper()) / outputImage.Width) - 1) * scale * imageAspectRatio;
                            y = (1 - 2 * ((j + 0.5 + AAhelper()) / outputImage.Height)) * scale;
                        }
                        Vector3 rayDirection = new Vector3(x, y, 1);
                        rayDirection = rayDirection - origin;
                        rayDirection = rayDirection.Normalized();
                        Ray ray = new Ray(origin, rayDirection);
                        pixelColor += Colorizer(ray, max_bounce, i, j);
                    }

                    //     Console.WriteLine(ray.Direction);


                    outputImage.SetPixel(i, j, pixelColor / samples_per_pixel);


                }
            }

        }

        public Color Colorizer(Ray ray, int bounce, int i, int j)
        {
            Color pixelColor = new Color(0, 0, 0);
            if (bounce == 0)
            {
                // Console.WriteLine("its 0 knob");
                return new Color(ray.Direction.X, ray.Direction.Y, ray.Direction.Z); ;
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
                    if ((zdepth == -1) || (zdepth > (hit.Position - ray.Origin).LengthSq()))
                    {
                        newEntity = entity;//Debug
                        zdepth = (hit.Position - ray.Origin).LengthSq();
                        storedHit = new RayHit(hit.Position, hit.Normal, hit.Incident, hit.Material);
                    }
                }
            }



            // Shading
            if (storedHit != null)
            {
                if (storedHit.Material.Type == Material.MaterialType.Reflective)
                {
                    pixelColor += Reflect(storedHit, bounce, i, j);
                }
                else if (storedHit.Material.Type == Material.MaterialType.Refractive)
                {
                    pixelColor += Refract(storedHit, bounce, i, j);
                }
                else if (storedHit.Material.Type == Material.MaterialType.Diffuse)
                {
                    // Diffuse lighting
                    foreach (PointLight light in this.lights)
                    {
                        // Shadows
                        double lum = 1;
                        // Decide on what side the shadow ray should be spawned
                        Vector3 origin = storedHit.Position + storedHit.Normal * 0.005;
                        Ray shadowRay = new Ray(origin, (light.Position - origin).Normalized());

                        foreach (SceneEntity entity in this.entities)
                        {
                            RayHit shwHit = entity.Intersect(shadowRay);

                            if ((shwHit != null && ((light.Position - origin).LengthSq() >= (shwHit.Position - origin).LengthSq())))
                            {
                                lum = 0;
                            }
                        }

                        lum = (storedHit.Normal.Dot((light.Position - storedHit.Position).Normalized())) * lum;
                        // Preventing underflow
                        if (lum < 0)
                        {
                            lum = 0;
                        }

                        pixelColor += light.Color * storedHit.Material.Color * lum;
                    }
                }
            }
            return pixelColor;
        }

        Color Reflect(RayHit storedHit, int bounce, int i, int j)
        {
            Vector3 origin = storedHit.Position + storedHit.Normal * 0.005;

            Ray reflectRay = new Ray(origin, (storedHit.Incident - (2 * storedHit.Incident.Dot(storedHit.Normal) * storedHit.Normal)));

            return Colorizer(reflectRay, bounce - 1, i, j);
        }

        Color Refract(RayHit storedHit, int bounce, int i, int j)
        {
            double cosi = Math.Clamp(storedHit.Incident.Dot(storedHit.Normal), -1, 1);
            double etai = 1, etat = storedHit.Material.RefractiveIndex;
            Vector3 n = storedHit.Normal;

            // Detect going in(<0) or out(>0)
            if (cosi < 0)
            {
                cosi = -cosi;
            }
            else
            {
                double temp = etai;
                etai = etat;
                etat = temp;
                n = -storedHit.Normal;
            }

            double eta = etai / etat;
            double k = 1 - eta * eta * (1 - cosi * cosi);
            Color refractColor = new Color(0, 0, 0);
            Color reflectColor = new Color(0, 0, 0);

            double kr = Fresnel(storedHit, bounce, i, j);

            if ((i == 200) && ((j == 246)))
            {
                // Console.Write(j);
                // Console.Write(": ");
                // Console.WriteLine(bounce);
                // Console.Write("Normal");
                // Console.Write(": ");
                // Console.WriteLine(n);
                // Console.Write("k");
                // Console.Write(": ");
                // Console.WriteLine(k);
                // // Console.WriteLine(newEntity.Material.Color);
                // Console.Write("Cosi: ");
                // Console.WriteLine(cosi);
                // Console.Write("Eta: ");
                // Console.WriteLine(eta);
                // Console.Write("Etai: ");
                // Console.WriteLine(etai);
                // Console.Write("Etat: ");
                // Console.WriteLine(etat);
                // Console.Write("Origin: ");
                // Console.WriteLine(storedHit.Position);
                // // Console.Write("Direction: ");
                // // Console.WriteLine(ReflectRay.Direction);
                // Console.Write("Incidence: ");
                // Console.WriteLine(storedHit.Incident);
                // // Console.WriteLine();

                Console.WriteLine("=============================================================================");

            }

            if (kr < 1)
            {
                Vector3 reflectDir = eta * storedHit.Incident + (eta * cosi - Math.Sqrt(k)) * n;
                Vector3 origin = storedHit.Position - n * 0.005;
                Ray RefractRay = new Ray(origin, reflectDir.Normalized());

                refractColor = Colorizer(RefractRay, bounce - 1, i, j);
            }

            reflectColor = Reflect(storedHit, bounce - 1, i, j);


            return (reflectColor * kr) + (refractColor * (1 - kr));
        }


        double Fresnel(RayHit storedHit, int bounce, int i, int j)
        {
            double cosi = Math.Clamp(storedHit.Incident.Dot(storedHit.Normal), -1, 1);
            double etai = 1, etat = storedHit.Material.RefractiveIndex;

            // Detect going in(<0) or out(>0)
            if (cosi > 0)
            {
                double temp = etai;
                etai = etat;
                etat = temp;
            }
            double sint = (etai / etat) * Math.Sqrt(Math.Max(0.0, 1 - cosi * cosi));

            double kr = -99;

            // Total internal reflection
            if (sint >= 1)
            {
                kr = 1;
            }
            else
            {
                double cost = Math.Sqrt(Math.Max(0.0, 1 - sint * sint));
                cosi = Math.Abs(cosi);
                double Rs = ((etat * cosi) - (etai * cost)) / ((etat * cosi) + (etai * cost));
                double Rp = ((etai * cosi) - (etat * cost)) / ((etai * cosi) + (etat * cost));
                kr = (Rs * Rs + Rp * Rp) / 2;
            }

            return kr;
        }
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

        double AAhelper()
        {
            var rand = new Random();

            while (true)
            {
                double p = rand.NextDouble() * 2 - 1;
                if (p == 1) { continue; }
                return p / 3;
            }
        }
    }
}
