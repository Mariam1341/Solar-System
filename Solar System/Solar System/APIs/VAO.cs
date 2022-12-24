
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Runtime.InteropServices;
namespace APIs
{
    public class VAO
    {
        public int Id { get; private set; }
        public int ElementsId { get; private set; }
        public int Length { get; private set; }

        private List<int> Buffers { get; set; }

        public VAO(Vector3[] positions, Vector2[] textures, Vector3[] normals)
        {
            Buffers = new List<int>();
            Length = positions.Length;
            Id = GL.GenVertexArray();

            AddAttributeArray(positions, 3, 0);
            AddAttributeArray(normals, 3, 1);
            AddAttributeArray(textures, 2, 2);
        }
        public VAO(Vector3[] positions, Vector2[] textures)
        {
            Buffers = new List<int>();
            Length = positions.Length;
            Id = GL.GenVertexArray();

            AddAttributeArray(positions, 3, 0);
            AddAttributeArray(textures, 2, 1);
        }

        public VAO(Vector3[] positions)
        {
            Buffers = new List<int>();
            Length = positions.Length;
            Id = GL.GenVertexArray();

            AddAttributeArray(positions, 3, 0);
        }

        public VAO(Vector2[] positions)
        {
            Buffers = new List<int>();
            Length = positions.Length;
            Id = GL.GenVertexArray();

            AddAttributeArray(positions, 2, 0);
        }

        public VAO(float[] positions)
        {
            Buffers = new List<int>();
            Length = positions.Length;
            Id = GL.GenVertexArray();

            AddAttributeArray(positions, 1, 0);
        }

        public int AddAttributeArray<E>(E[] bufferData, int stride, int location) where E : struct
        {
            if (bufferData == null)
                return -1;

            GL.BindVertexArray(Id);

            int typeSize = Marshal.SizeOf(bufferData.GetType().GetElementType());

            int bufferID = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, bufferID);
            GL.BufferData(BufferTarget.ArrayBuffer, bufferData.Length * typeSize, bufferData, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(location);
            GL.VertexAttribPointer(location, stride, VertexAttribPointerType.Float, false, 0, 0);
            Buffers.Add(bufferID);

            //unbind our buffers
            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            return Buffers.Count;
        }

        public void AddElementArray(uint[] elements)
        {
            GL.BindVertexArray(Id);

            Length = elements.Length;

            ElementsId = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementsId);
            GL.BufferData(BufferTarget.ElementArrayBuffer, elements.Length * sizeof(uint), elements, BufferUsageHint.StaticDraw);

            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

        public void Bind()
        {
            GL.BindVertexArray(Id);
        }

        public void DrawArrays(PrimitiveType type = PrimitiveType.Triangles)
        {
            Bind();
            GL.DrawArrays(type, 0, Length);
            Unbind();
        }

        public void DrawElements(PrimitiveType type = PrimitiveType.Triangles)
        {
            Bind();
            GL.DrawElements(type, Length, DrawElementsType.UnsignedInt, 0);
            Unbind();
        }

        public void Unbind()
        {
            GL.BindVertexArray(0);
        }

        public void Dispose()
        {
            foreach (var bufferID in Buffers)
                GL.DeleteBuffer(bufferID);

            GL.DeleteVertexArray(Id);
        }
    }
}

