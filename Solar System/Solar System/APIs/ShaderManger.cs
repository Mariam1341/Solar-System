using OpenTK.Mathematics;
using Solar_System;

namespace APIs
{
    public class ShaderManger
    {
        private const string TEXTURE_MAPPING = "texture mapping.vert";

        #region Folders
        private const string LIGHTING = "Lighting";
        private const string BASIC = "Basic";
        private const string TEXTURE = "Texture";
        #endregion

        public static void DirecLight(Camera camera, Matrix4 model, Vector3 lightPos)
        {
            Shader shader = GetShader(TEXTURE_MAPPING, LIGHTING, "direcional lights.frag", LIGHTING);
            shader.Use();

            WriteToProjAttr(camera, model, shader);

            shader.SetVector3("viewPos", camera.Position);

            shader.SetInt("material.diffuse", 0);
            shader.SetInt("material.specular", 0);
            shader.SetFloat("material.shininess", 32);

            Vector3 lightColor = new(1, 1, 1);

            shader.SetVector3("light.direction", lightPos);
            shader.SetVector3("light.ambient", lightColor * new Vector3(0.25f));
            shader.SetVector3("light.diffuse", lightColor * new Vector3(1f));
            shader.SetVector3("light.specular", lightColor * new Vector3(.25f));

        }

        public static void LightMapping(Camera camera, Matrix4 model, Vector3 lightPos)
        {
            Shader shader = GetShader(TEXTURE_MAPPING, LIGHTING, "texture mapping.frag", LIGHTING);
            shader.Use();

            WriteToProjAttr(camera, model, shader);

            shader.SetVector3("viewPos", camera.Position);

            shader.SetInt("material.diffuse", 0);
            shader.SetInt("material.specular", 1);
            shader.SetVector3("material.specular", new Vector3(0.5f, 0.5f, 0.5f));
            shader.SetFloat("material.shininess", 32.0f);

            shader.SetVector3("light.position", lightPos);
            shader.SetVector3("light.ambient", new Vector3(.9f));
            shader.SetVector3("light.diffuse", new Vector3(0.8f));
            shader.SetVector3("light.specular", new Vector3(2f));

        }
        public static void ColorLight(Camera camera, Matrix4 model, Vector3 objColor, Vector3 lightColor)
        {
            Shader shader = new(Server.GetShaderPath("without texture.vert", "Lighting"), Server.GetShaderPath("lightin.frag", "Lighting"));
            shader.Use();

            WriteToProjAttr(camera, model, shader);

            shader.SetVector3("objectColor", objColor);
            shader.SetVector3("lightColor", lightColor);

        }

        public static void Orbit_Axis(Camera camera, Matrix4 model)
        {
            Shader shader = GetShader("without texture.vert", LIGHTING, "shader.frag", BASIC);
            shader.Use();
            WriteToProjAttr(camera, model, shader);
        }
        public static void BasicLighting(Camera camera, Matrix4 model, Vector3 objColor, Vector3 lightColor, Vector3 lightPos)
        {
            Shader shader = GetShader("light calc.vert", "Lighting", "light calc.frag", "Lighting");
            shader.Use();

            WriteToProjAttr(camera, model, shader);

            shader.SetVector3("objectColor", objColor);
            shader.SetVector3("lightColor", lightColor);
            shader.SetVector3("lightPos", lightPos);
            shader.SetVector3("viewPos", camera.Position);

        }

        public static void PointLight(Camera camera, Matrix4 model, Vector3 lightPos)
        {
            Shader shader = GetShader(TEXTURE_MAPPING, LIGHTING, "point light.frag", LIGHTING);
            shader.Use();

            WriteToProjAttr(camera, model, shader);
            shader.SetVector3("viewPos", camera.Position);

            shader.SetInt("material.diffuse", 0);
            shader.SetInt("material.specular", 0);
            shader.SetFloat("material.shininess", 16.0f);

            shader.SetVector3("light.position", lightPos);
            shader.SetFloat("light.constant", .9f);
            shader.SetFloat("light.linear", 0.0014f);
            shader.SetFloat("light.quadratic", 0.000007f);
            shader.SetVector3("light.ambient", new Vector3(0.45f));
            shader.SetVector3("light.diffuse", new Vector3(1.5f));
            shader.SetVector3("light.specular", new Vector3(.25f));


        }

        public static void SpotLight(Camera camera, Matrix4 model)
        {
            Shader shader = GetShader(TEXTURE_MAPPING, LIGHTING, "spot light.frag", LIGHTING);
            shader.Use();

            WriteToProjAttr(camera, model, shader);
            shader.SetVector3("viewPos", camera.Position);

            shader.SetInt("material.diffuse", 0);
            shader.SetInt("material.specular", 1);
            shader.SetVector3("material.specular", new Vector3(0.5f, 0.5f, 0.5f));
            shader.SetFloat("material.shininess", 32.0f);

            shader.SetVector3("light.position", camera.Position);
            shader.SetVector3("light.direction", camera.Front);
            shader.SetFloat("light.cutOff", MathF.Cos(MathHelper.DegreesToRadians(42.5f)));
            shader.SetFloat("light.outerCutOff", MathF.Cos(MathHelper.DegreesToRadians(57.5f)));
            shader.SetFloat("light.constant", 1.0f);
            shader.SetFloat("light.linear", 0.09f);
            shader.SetFloat("light.quadratic", 0.032f);
            shader.SetVector3("light.ambient", new Vector3(0.2f));
            shader.SetVector3("light.diffuse", new Vector3(0.5f));
            shader.SetVector3("light.specular", new Vector3(1.0f));

        }
        private static void WriteToProjAttr(Camera camera, Matrix4 model, Shader shader)
        {
            shader.SetMatrix4("model", model);
            shader.SetMatrix4("view", camera.GetViewMatrix());
            shader.SetMatrix4("projection", camera.GetProjectionMatrix());
        }
        private static Shader GetShader(string vertName, string vertFolder, string fragName, string fragFolder)
        {
            return new(Server.GetShaderPath(vertName, vertFolder), Server.GetShaderPath(fragName, fragFolder));
        }
    }
}
