using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
namespace APIs
{
    public class Sphere
    {
        private readonly float Radius;

        private const float PI = float.Pi;

        private const int sectorCount = 100, stackCount = 30;

        private readonly List<Vector3> vertices;

        private readonly List<Vector3> normals;

        private readonly List<Vector2> texels;

        private readonly List<uint> indices;

        private readonly VAO vertexArrayBuffer;

        private Texture? texture;
        public Sphere(float radius)
        {
            Radius = radius;
            vertices = new List<Vector3>();
            normals = new List<Vector3>();
            texels = new List<Vector2>();
            indices = new List<uint>();
            CalculateVertices();
            vertexArrayBuffer = new VAO(vertices.ToArray(), texels.ToArray(), normals.ToArray());
            vertexArrayBuffer.AddElementArray(indices.ToArray());
        }

        private void CalculateVertices()
        {
            #region License

            // Algorithm reference: http://www.songho.ca/opengl/gl_sphere.html
            #endregion


            float radius = Radius;

            float x, y, z, xy;                              // vertex position
            float nx, ny, nz, lengthInv = 1.0f / radius;    // vertex normal

            float sectorStep = 2 * PI / sectorCount;
            float stackStep = PI / stackCount;
            float sectorAngle, stackAngle;

            for (int i = 0; i <= stackCount; ++i)
            {
                stackAngle = PI / 2 - i * stackStep;        // starting from pi/2 to -pi/2
                xy = radius * MathF.Cos(stackAngle);             // r * cos(u)
                z = radius * MathF.Sin(stackAngle);              // r * sin(u)

                // add (sectorCount+1) vertices per stack
                // the first and last vertices have same position and normal, but different tex coords
                for (int j = 0; j <= sectorCount; ++j)
                {
                    sectorAngle = j * sectorStep;           // starting from 0 to 2pi

                    // vertex position (x, y, z)
                    x = xy * MathF.Cos(sectorAngle);             // r * cos(u) * cos(v)
                    y = xy * MathF.Sin(sectorAngle);             // r * cos(u) * sin(v)

                    vertices.Add(new Vector3(x, y, z));

                    // normalized vertex normal (nx, ny, nz)
                    nx = x * lengthInv;
                    ny = y * lengthInv;
                    nz = z * lengthInv;

                    normals.Add(new Vector3(nx, ny, nz));

                    // Texture coodrdinates
                    texels.Add(new Vector2((float)j / sectorCount, (float)i / stackCount));

                }
            }

            CalculateIndices();
        }

        private void CalculateIndices()
        {
            uint k1, k2;
            for (uint i = 0; i < stackCount; ++i)
            {
                k1 = i * (sectorCount + 1);     // beginning of current stack
                k2 = k1 + sectorCount + 1;      // beginning of next stack

                for (int j = 0; j < sectorCount; ++j, ++k1, ++k2)
                {
                    // 2 triangles per sector excluding first and last stacks
                    // k1 => k2 => k1+1
                    if (i != 0)
                    {
                        indices.Add(k1);
                        indices.Add(k2);
                        indices.Add(k1 + 1);
                    }

                    // k1+1 => k2 => k2+1
                    if (i != (stackCount - 1))
                    {
                        indices.Add(k1 + 1);
                        indices.Add(k2);
                        indices.Add(k2 + 1);
                    }

                }
            }
        }

        public void ApplyTexture(string path)
        {
            texture = Texture.LoadFromFile(path);
        }

        public void Draw(PrimitiveType type = PrimitiveType.Triangles)
        {
            vertexArrayBuffer.DrawElements(type);
        }

        public void UseTexture(TextureUnit unit = TextureUnit.Texture0)
        {
            texture?.Use(unit);
        }
    }
}
