using APIs;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Solar_System
{
    public class SolarScene : GameWindow
    {
        private Camera? camera;

        private readonly Vector3 lightPos = new(36.956654f, 3.0306416f, 22.81607f);
        //private readonly Vector3 lightPos = new(45f, 45f, 0);

        private bool firstMove = true;

        private Vector2 lastPos;
        public bool UseMouse { get; set; }

        private float hoursPerSecond = 1;

        private Planet? background;

        private List<Planet> planets = new();
        public List<Planet> Planets { set { planets = value; } }

        private readonly Dictionary<string, Planet> focusing = new();
        public SolarScene() :
            base(GameWindowSettings.Default, new NativeWindowSettings()
            {
                StartFocused = true,
                StartVisible = false,
                Size = new Vector2i(1400, 850),
                Title = "Solar System",
                WindowState = WindowState.Maximized
            })
        {
            CenterWindow();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, e.Width, e.Height);
            if (camera != null)
                camera.AspectRatio = Size.X / (float)Size.Y;
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            IsVisible = true;
            GL.ClearColor(Color4.Black);
            GL.Enable(EnableCap.DepthTest);
            camera = new Camera(new Vector3(-32.406296f, 97.20163f, 265.43283f), Size.X / (float)Size.Y)
            {
                Yaw = -59.195007f,
                Pitch = -19.698872f
            };
            background = new("sky");

            foreach (var planet in planets)
            {
                focusing.Add(planet.PlanetName, planet);
            }
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            background.RenderPlanet(camera, lightPos);
            // Render plantes here
            foreach (Planet planet in planets)
            {
                planet.RenderPlanet(camera, lightPos);
            }


            SwapBuffers();
            base.OnRenderFrame(args);
        }
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            //Console.WriteLine(camera.Position);
            //Console.WriteLine(camera.Yaw);
            //Console.WriteLine(camera.Pitch);
            //Console.WriteLine(e.Time);
            foreach (Planet planet in planets)
            {
                planet.UpdatePosition((float)e.Time, hoursPerSecond);
            }


            if (!IsFocused)
            {
                return;
            }

            var input = KeyboardState;

            if (input.IsKeyDown(Keys.Escape))
            {
                Close();
            }

            const float cameraSpeed = 60f;
            const float sensitivity = 0.1f;

            KeyboardControl(e, input, cameraSpeed);

            if (UseMouse)
            {
                MouseControl(sensitivity);
            }


        }

        private void MouseControl(float sensitivity)
        {
            var mouse = MouseState;

            if (firstMove)
            {
                lastPos = new Vector2(mouse.X, mouse.Y);
                firstMove = false;
            }
            else
            {
                var deltaX = mouse.X - lastPos.X;
                var deltaY = mouse.Y - lastPos.Y;
                lastPos = new Vector2(mouse.X, mouse.Y);

                camera.Yaw += deltaX * sensitivity;
                camera.Pitch -= deltaY * sensitivity;
            }
        }

        private void KeyboardControl(FrameEventArgs e, KeyboardState input, float cameraSpeed)
        {


            SetFocusOnSpecificPlanet(input, cameraSpeed);



            if (input.IsKeyDown(Keys.W))
            {
                camera.Position += camera.Front * cameraSpeed * (float)e.Time; // Forward
            }
            if (input.IsKeyDown(Keys.S))
            {
                camera.Position -= camera.Front * cameraSpeed * (float)e.Time; // Backwards
            }
            if (input.IsKeyDown(Keys.A))
            {
                camera.Position -= camera.Right * cameraSpeed * (float)e.Time; // Left
            }
            if (input.IsKeyDown(Keys.D))
            {
                camera.Position += camera.Right * cameraSpeed * (float)e.Time; // Right
            }
            if (input.IsKeyDown(Keys.Space))
            {
                camera.Position += camera.Up * cameraSpeed * (float)e.Time; // Up
            }
            if (input.IsKeyDown(Keys.LeftShift))
            {
                camera.Position -= camera.Up * cameraSpeed * (float)e.Time; // Down
            }
            if (input.IsKeyDown(Keys.R))
            {
                hoursPerSecond = 1;
            }
            if (input.IsKeyDown(Keys.F1))
            {
                hoursPerSecond += .1f;
            }
            if (input.IsKeyDown(Keys.F2) && hoursPerSecond > 1)
            {
                hoursPerSecond -= .1f;
            }
            if (input.IsKeyPressed(Keys.O))
            {
                foreach (var planet in planets)
                {
                    planet.DrawOrbit = !planet.DrawOrbit;
                }
            }
            if (input.IsKeyPressed(Keys.P))
            {
                foreach (var planet in planets)
                {
                    planet.DrawAxis = !planet.DrawAxis;
                }
            }

            if (input.IsKeyPressed(Keys.M))
            {
                UseMouse = !UseMouse;
            }

        }
        private void SetFocusOnSpecificPlanet(KeyboardState input, float cameraSpeed)
        {
            if (input.IsKeyDown(Keys.D1))
            {
                camera = new Camera(focusing["sun"].Position - camera.Front * cameraSpeed * 1.5f, Size.X / (float)Size.Y);
            }
            if (input.IsKeyDown(Keys.D2))
            {
                camera = new Camera(focusing["mercury"].Position - camera.Front * cameraSpeed * 0.5f, Size.X / (float)Size.Y);
            }
            if (input.IsKeyDown(Keys.D3))
            {
                camera = new Camera(focusing["venus"].Position - camera.Front * cameraSpeed * 0.5f, Size.X / (float)Size.Y);
            }
            if (input.IsKeyDown(Keys.D4))
            {
                camera = new Camera(focusing["earth"].Position - camera.Front * cameraSpeed * 0.5f, Size.X / (float)Size.Y);
            }
            if (input.IsKeyDown(Keys.D5))
            {
                camera = new Camera(focusing["moon"].Position - camera.Front * cameraSpeed * 0.3f, Size.X / (float)Size.Y);
            }
            if (input.IsKeyDown(Keys.D6))
            {
                camera = new Camera(focusing["mars"].Position - camera.Front * cameraSpeed * 0.5f, Size.X / (float)Size.Y);
            }
            if (input.IsKeyDown(Keys.D7))
            {
                camera = new Camera(focusing["jupiter"].Position - camera.Front * cameraSpeed * 0.5f, Size.X / (float)Size.Y);
            }
            if (input.IsKeyDown(Keys.D8))
            {
                camera = new Camera(focusing["saturn"].Position - camera.Front * cameraSpeed * 0.5f, Size.X / (float)Size.Y);
            }
            if (input.IsKeyDown(Keys.D9))
            {
                camera = new Camera(focusing["uranus"].Position - camera.Front * cameraSpeed * 0.5f, Size.X / (float)Size.Y);
            }
            if (input.IsKeyDown(Keys.D9))
            {
                camera = new Camera(focusing["neptune"].Position - camera.Front * cameraSpeed * 0.5f, Size.X / (float)Size.Y);
            }
        }
        public void AddPlanet(Planet planet)
        {
            planets.Add(planet);
        }
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            camera.Fov -= e.OffsetY;
        }
        private Planet sun = new Planet("sun");
        public void planetMoon(string planet, int num)
        {
            Planet p = new Planet(planet, sun);
            planets.Add(p)
            for (int i = 1; i <= num; i++)
            {
                Planet moon = new Planet("moon" + i, p);
                planets.Add(moon)
            }
        }
    }
}
