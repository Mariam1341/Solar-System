
using APIs;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Solar_System
{

    public class Planet
    {

        public Orbit? Orbit { get; private set; }
        public bool DrawOrbit { get; set; }
        public bool DrawAxis { get; set; }

        private VAO axisLine;

        public string PlanetName { get; set; }
        //parent which it roatates around
        private Planet? parent;

        public Vector3 Position { get { return modelMatrix.ExtractTranslation(); } }

        private float radius;
        private float disFromSun;
        private float rotationPeriod; // in hours
        private float orbitalPeriod; // in days
        private float axialTilt; // in degrees

        private float orbitalAngle;
        private float rotationAngle;

        private Sphere planet;
        private Matrix4 modelMatrix = Matrix4.Identity;

        public Planet(string name, Planet? p = null)
        {
            parent = p;
            PlanetName = name;
            LoadPlanetInfo(name);
            planet = new Sphere(radius);
            planet.ApplyTexture(Server.GetTexturePath(name));

            if (parent != null)
            {
                Orbit = new Orbit(this, parent.Position, disFromSun);
                DrawOrbit = true;
            }

            Vector3[] axisVerts = { new Vector3(0, -70, 0), new Vector3(0, 70, 0) };
            axisLine = new VAO(axisVerts);
            DrawAxis = true;
        }

        public void RenderPlanet(Camera camera, Vector3 lightPos)
        {

            planet.UseTexture();
            if (PlanetName.ToLower() == "sky" || PlanetName.ToLower() == "sun")
                ShaderManger.LightMapping(camera, modelMatrix, new Vector3(100, 100, 100));
            else
                ShaderManger.PointLight(camera, modelMatrix, lightPos);
            planet.Draw();

            if (DrawAxis && PlanetName.ToLower() != "sky")
            {
                ShaderManger.Orbit_Axis(camera, modelMatrix);
                axisLine.DrawArrays(PrimitiveType.Lines);
            }

            if (Orbit != null && DrawOrbit)
                Orbit.Draw(camera);

        }
        private void LoadPlanetInfo(string name)
        {
            Dictionary<string, float> info = Server.GetPlanetInfo(name);
            radius = info["radius"];
            disFromSun = info["disFromSun"];
            rotationPeriod = info["rotationPeriod"];
            orbitalPeriod = info["orbitalPeriod"];
            axialTilt = info["axialTilt"];

        }

        public virtual void UpdatePosition(float delta, float hoursPerSecond)
        {
            float deltaHours = hoursPerSecond * delta;
            float deltaDays = deltaHours / 24.0f;

            if (orbitalPeriod != 0)
                orbitalAngle += deltaDays / orbitalPeriod * float.Pi * 2;
            if (rotationPeriod != 0)
                rotationAngle += deltaHours / rotationPeriod * float.Pi * 2;

            //limit angle to (0 - 360) 0 - 2PI

            if (orbitalAngle > float.Pi * 2)
                orbitalAngle -= (float.Pi * 2);
            if (rotationAngle > float.Pi * 2)
                rotationAngle -= (float.Pi * 2);

            modelMatrix = modelMatrix.ClearRotation();
            modelMatrix *= Matrix4.CreateRotationY(rotationAngle);
            modelMatrix *= Matrix4.CreateRotationX(MathHelper.DegreesToRadians(axialTilt));

            if (parent != null)
            {
                float x = (float)(disFromSun * Math.Cos(orbitalAngle) + parent.Position.X);
                float y = 0;
                float z = (float)(disFromSun * Math.Sin(orbitalAngle) + parent.Position.Z);
                modelMatrix.Row3 = new Vector4(x, y, z, 1);
            }

            if (parent != null)
            {
                if (parent.PlanetName.ToLower().Equals("earth") && Orbit != null)
                    Orbit.UpdatePosition(parent.Position);
            }
        }

    }
}
