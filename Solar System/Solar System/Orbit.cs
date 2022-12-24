using APIs;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Solar_System
{
    public class Orbit
    {
        private readonly VAO vao;
        private readonly Planet parent;

        private const int sides = 120;
        public Vector3 Positon { get; private set; }
        public float Radius { get; private set; }
        public Matrix4 Transform { get; private set; }

        public Orbit(Planet parent, Vector3 center, double radius)
        {
            this.parent = parent;
            Positon = center;
            Radius = (float)radius;
            Transform = Matrix4.CreateTranslation(center);

            List<Vector3>? verts = new();

            float step = (float)(2 * Math.PI / sides);
            for (float alpha = 0; alpha < 2 * Math.PI; alpha += step)
            {
                float x = (float)(radius * Math.Cos(alpha));
                float y = 0;
                float z = (float)(radius * Math.Sin(alpha));
                verts.Add(new Vector3(x, y, z));

                alpha += step;
            }

            vao = new VAO(verts.ToArray());
        }

        public void Draw(Camera camera)
        {
            ShaderManger.Orbit_Axis(camera, Transform);
            vao.DrawArrays(PrimitiveType.LineLoop);
        }

        public void UpdatePosition(Vector3 pos)
        {
            Transform = Matrix4.CreateTranslation(pos);
        }
    }
}
