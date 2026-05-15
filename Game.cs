using VoxelTK.Graphics;
using VoxelTK.World;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace VoxelTK
{
    internal class Game : GameWindow
    {
        Chunk chunk;
        ShaderProgram program;

        // Camera
        Camera camera;

        public int Width, Height;

        public Game(int Width, int Height) : base(GameWindowSettings.Default, NativeWindowSettings.Default)
        {
            this.Width = Width;
            this.Height = Height;

            CenterWindow(new Vector2i(Width, Height));
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, e.Width, e.Height);
            this.Width = e.Width;
            this.Height = e.Height;
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            chunk = new Chunk(new Vector3(0, 0, 0));
            program = new ShaderProgram("Default.vert", "Default.frag");

            GL.Enable(EnableCap.DepthTest);
            GL.FrontFace(FrontFaceDirection.Cw);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);

            camera = new Camera((float)Width, (float)Height, new Vector3(0, 5, 10));
            CursorState = CursorState.Grabbed;
        }

        protected override void OnUnload()
        {
            base.OnUnload();

            chunk.Delete();
            program.Delete();
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.ClearColor(0.3f, 0.3f, 1f, 1f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Matrix4 model = Matrix4.Identity;
            Matrix4 view = camera.GetViewMatrix();
            Matrix4 projection = camera.GetProjectionMatrix();

            int modelLocation = GL.GetUniformLocation(program.ID, "model");
            int viewLocation = GL.GetUniformLocation(program.ID, "view");
            int projectionLocation = GL.GetUniformLocation(program.ID, "projection");

            GL.UniformMatrix4(modelLocation, true, ref model);
            GL.UniformMatrix4(viewLocation, true, ref view);
            GL.UniformMatrix4(projectionLocation, true, ref projection);

            chunk.Render(program);

            Context.SwapBuffers();
            base.OnRenderFrame(args);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            MouseState mouse = MouseState;
            KeyboardState input = KeyboardState;

            base.OnUpdateFrame(args);
            camera.Update(input, mouse, args);
        }
    }
}