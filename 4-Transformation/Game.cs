using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;


namespace LearnOpenTK
{
    public class Game : GameWindow
    {
        float[] vertices =
        {
            //Position          Texture coordinates
             0.5f,  0.5f, 0.0f, 1.0f, 1.0f, // top right
             0.5f, -0.5f, 0.0f, 1.0f, 0.0f, // bottom right
            -0.5f, -0.5f, 0.0f, 0.0f, 0.0f, // bottom left
            -0.5f,  0.5f, 0.0f, 0.0f, 1.0f  // top left
        };
        private readonly uint[] indices =
        {
            // Note that indices start at 0!
            0, 1, 3, // The first triangle will be the top-right half of the triangle
            1, 2, 3  // Then the second will be the bottom-left half of the triangle
        };

        int VertexBufferObject;

        int VertexArrayObject;

        int ElementBufferObject;

        Shader shader;
        Texture texture;


        public Game(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings() { Size = (width, height), Title = title })
        {
        }
        protected override void OnLoad()
        {
            base.OnLoad();

            GL.ClearColor(0.5f, 1.0f, 0.7f, 1.0f); // Белый цвет фона

            VertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(VertexArrayObject);

            VertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            ElementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);


            shader = new Shader("shader.vert", "shader.frag");
            shader.Use();


            var vertexLocation = shader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            int texCoordLocation = shader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

            texture = Texture.LoadFromFile("Resources/container.png");
            texture.Use(TextureUnit.Texture0);
        }



        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit);

            // Activate the VAO
            GL.BindVertexArray(VertexArrayObject);

            texture.Use(TextureUnit.Texture0);
            // Activate the shader

            var transform = Matrix4.Identity;
            transform = transform * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(20f));

            // Next, we scale the matrix. This will make the rectangle slightly larger.
            transform = transform * Matrix4.CreateScale(1.1f);

            // Then, we translate the matrix, which will move it slightly towards the top-right.
            // Note that we aren't using a full coordinate system yet, so the translation is in normalized device coordinates.
            // The next tutorial will be about how to set one up so we can use more human-readable numbers.
            transform = transform * Matrix4.CreateTranslation(0.5f, 0.5f, 0.5f);

            texture.Use(TextureUnit.Texture0);
            shader.Use();

            // Now that the matrix is finished, pass it to the vertex shader.
            // Go over to shader.vert to see how we finally apply this to the vertices.
            shader.SetMatrix4("transform", transform);

            // Render the square using the EBO
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);

            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            if (KeyboardState.IsKeyDown(Keys.Escape))
            {
                Close();
            }
        }



        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, e.Width, e.Height);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            GL.DeleteBuffer(VertexBufferObject);
            GL.DeleteBuffer(ElementBufferObject);
            GL.DeleteVertexArray(VertexArrayObject);
           
        }
    }
}
