using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Particles.Rendering;
using System.Diagnostics;

using OpenTK.Mathematics;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Runtime.InteropServices;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Core.Particles
{
    internal unsafe class OpenTKGame : GameWindow
    {

        private uint scencilIndex = 0;
        private RendererOpenTK renderer;


        private Emitter emitter_spawner;

        private bool _firstMove = true;
        private Vector2 _lastPos;
        private Camera camera;
        public OpenTKGame(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
            camera = new Camera(new Vector3(0, 0, 5), 60, Size.X / Size.Y);
            // Generate texture
            {
                //LoadSpriteSheet("./Assets/Textures/matrix.png", 1,8,8);

                //Load the image
                Image<Rgba32> image = SixLabors.ImageSharp.Image.Load<Rgba32>("./Assets/Textures/matrix.png");


                int width = image.Width;
                int height = image.Height;
                int count = width * height;
                int columns = 8;
                int rows = 8;
                int depth = columns * rows;
                int elementWidth = width / rows;
                int elementHeight = height / columns;


                //ImageSharp loads from the top-left pixel, whereas OpenGL loads from the bottom-left, causing the texture to be flipped vertically.
                //This will correct that, making the texture display properly.
                image.Mutate(x => x.Flip(FlipMode.Vertical));

                Rgba32[] pixelArray = new Rgba32[count];

                image.CopyPixelDataTo(pixelArray);
                pixelArray = ReorderToSpriteSheet(pixelArray, width, height, columns, rows);

                GL.Enable(EnableCap.Texture3DExt);
                int texObject = GL.GenTexture();
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2DArray, texObject);
                GL.TextureStorage3D(texObject, 0, SizedInternalFormat.Rgba8, elementWidth, elementHeight, depth);
                GL.TexImage3D(TextureTarget.Texture2DArray,
                0,
                PixelInternalFormat.Rgba,
                image.Width / 8,
                image.Height / 8,
                8 * 8,
                0,
                OpenTK.Graphics.OpenGL4.PixelFormat.Bgra,
                PixelType.UnsignedByte,
                pixelArray);

                GL.GenerateMipmap(GenerateMipmapTarget.Texture2DArray);
            }



            // Save these for access in spawner
            var letter_lifetime = new ModuleLifetime();
            var letter_position = new ModulePosition();
            var letter_sheet = new ModuleSheet(0);

            var gradient = new Gradient<Color>(new GradientPoint<Color>[] {
                new GradientPoint<Color>(0,     new Color(255,  255,    255,    255)),
                new GradientPoint<Color>(0.1f,  new Color(0,    255,    0,      255)),
                new GradientPoint<Color>(0.8f,  new Color(0,    255,    0,      255)),
                new GradientPoint<Color>(1f,    new Color(0,    255,    0,      0))
                }).Quantize(128).Select(x=>x.PackedValue).ToArray();

            // Letter system
            var system_letter = new SimulatorCPU(500000, new IModule[]
            {
                letter_lifetime,
                letter_position,
                new ModuleScale(),
                new ModuleColor(),

                letter_sheet,
                new ModuleColorOverLifetime(new TableUint(gradient))
            });



            var spawner_position = new ModulePosition();
            var spawner_velocity = new ModuleVelocity();
            var spawner_emitter = new ModuleEmitterTimer((int a) =>
            {
                int particle = system_letter.GetNextParticle();


                // copy position
                letter_position.PositionX[particle] = spawner_position.PositionX[a];
                letter_position.PositionY[particle] = spawner_position.PositionY[a];
                letter_position.PositionZ[particle] = spawner_position.PositionZ[a];
                letter_sheet.SpriteIndex[particle] = (byte)Randoms.Range(0, 64);
                // set lifetime
                letter_lifetime.Lifetime[particle] = 3f;
                letter_lifetime.LifeProgress[particle] = 0f;

            }, 0.1f);

            // Spawner System
            var system_spawner = new SimulatorCPU(5000, new IModule[]
            {
                spawner_position,
                spawner_velocity,
                spawner_emitter
            });


            var emitterSettings = new EmitterSettings()
            {
                RateOverTime = 500,
            };

            emitter_spawner = new Emitter(emitterSettings, () =>
            {
                int particle = system_spawner.GetNextParticle();


                float z = Randoms.Range01() * -100f -5;

                float width = MathF.Tan(MathHelper.DegreesToRadians(60)) * z + 5;


                spawner_position.PositionX[particle] = (Randoms.Range01() * width) - width/2f;
                spawner_position.PositionY[particle] = Randoms.Range01() - width/2f;
                spawner_position.PositionZ[particle] = z;

                spawner_velocity.VelocityX[particle] = 0;
                spawner_velocity.VelocityY[particle] = -0.05f;
                spawner_velocity.VelocityZ[particle] = 0;

                spawner_emitter.Timer[particle] = 0;
            });

            renderer = new RendererOpenTK(system_letter);

            system_letter.Play();
            system_spawner.Play();

            emitter_spawner.Start();
        }

        protected override void OnLoad()
        {
            base.OnLoad();
        }

        T[] ReorderToSpriteSheet<T>(T[] data, int width, int height, int columns, int rows)
        {
            int elementWidth = width / columns;
            int elementHeight = height / rows;
            int elementCount = elementWidth * elementHeight;

            T[] r = new T[data.Length];

            for (int i = 0; i < r.Length; i++)
            {
                // Curret
                int column = (i / elementCount) % (columns);
                int row = i / (elementCount * columns);

                int x = i % elementWidth;
                int y = (i%elementCount) / elementWidth;

                int index = column * elementWidth + row * (elementCount*columns) + x + y * width;
                r[i] = data[index];
            }
            return r;
        }

        // Does not currently work
        // https://gamedev.stackexchange.com/questions/147854/unpacking-sprite-sheet-into-2d-texture-array
        void LoadSpriteSheet(string path, int levels, int columns, int rows)
        {/*
            //Load the image
            Image<Rgba32> image = Image.Load<Rgba32>(path);

            // Define Constants
            int width = image.Width;
            int height = image.Height;
            int count = width * height;
            int depth = columns * rows;
            int elementWidth = width / columns;
            int elementHeight = height / rows;

            PixelFormat format = OpenTK.Graphics.OpenGL4.PixelFormat.Bgra;

            PixelType type = PixelType.UnsignedByte;

            SizedInternalFormat internalFormat = SizedInternalFormat.Rgba8;


            // Flip the image since OpenGL loads in revserse
            image.Mutate(x => x.Flip(FlipMode.Vertical));
            // Store pixels in array
            Rgba32[] pixelArray = new Rgba32[count];
            image.CopyPixelDataTo(pixelArray);


            // Make sure 3D Textures are enabled
            GL.Enable(EnableCap.Texture3DExt);


            // Generate texture
            int texObject = GL.GenTexture();
            //
            GL.TextureStorage3D(texObject, levels, internalFormat, elementWidth, elementHeight, depth);
            GL.ActiveTexture(TextureUnit.Texture0);
            // Bind texture to  Texture2DArray
            GL.BindTexture(TextureTarget.Texture2DArray, texObject);

            // Tell WebGL the size of one row
            GL.PixelStore(PixelStoreParameter.UnpackRowLength, width);

            // Store Elements
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    int xoff = x * elementWidth;
                    int yoff = y * elementHeight;
                    int cdepth = y * columns + x;

                    // Tell WebGL where to start copying from
                    GL.PixelStore(PixelStoreParameter.UnpackSkipPixels, xoff);
                    GL.PixelStore(PixelStoreParameter.UnpackSkipRows, yoff);

                    GL.TexSubImage3D(TextureTarget.Texture2DArray, levels, 0, 0, cdepth, elementWidth, elementHeight, 1, format, type, pixelArray);
                }
            }
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2DArray);
            GL.PixelStore(PixelStoreParameter.UnpackRowLength, 0);
            GL.PixelStore(PixelStoreParameter.UnpackSkipPixels, 0);
            GL.PixelStore(PixelStoreParameter.UnpackSkipRows, 0);*/
        }




        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.ClearColor(0f, 0, 0, 1f);
            GL.ClearStencil(0);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.StencilBufferBit | ClearBufferMask.DepthBufferBit);


            renderer.Draw(camera.ViewMatrix, camera.ProjectionMatrix);

            SwapBuffers();
        }
        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Size.X, Size.Y);
        }

        private float totalTime = 0;
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            emitter_spawner.Update((float)e.Time);

            totalTime += (float)e.Time;

            float power = 0.5f;

            KeyboardState input = KeyboardState;
            MouseState mouse = MouseState;

            float cameraSpeed = 10f;

            if (input.IsKeyDown(Keys.LeftShift))
                cameraSpeed = 50f;

            const float sensitivity = 0.2f;

            if (input.IsKeyDown(Keys.W))
            {
                camera.Translate(camera.Front * cameraSpeed * (float)e.Time); // Forward
            }
            if (input.IsKeyDown(Keys.S))
            {
                camera.Translate(-camera.Front * cameraSpeed * (float)e.Time); // Backwards
            }
            if (input.IsKeyDown(Keys.A))
            {
                camera.Translate(-camera.Right * cameraSpeed * (float)e.Time); // Left
            }
            if (input.IsKeyDown(Keys.D))
            {
                camera.Translate(camera.Right * cameraSpeed * (float)e.Time); // Right
            }
            if (input.IsKeyDown(Keys.E))
            {
                camera.Translate(camera.Up * cameraSpeed * (float)e.Time); // Up
            }
            if (input.IsKeyDown(Keys.Q))
            {
                camera.Translate(-camera.Up * cameraSpeed * (float)e.Time); // Down
            }

            if (!mouse.WasButtonDown(MouseButton.Right) && mouse.IsButtonDown(MouseButton.Right))
                _lastPos = new Vector2(mouse.X, mouse.Y);

            if (mouse.IsButtonDown(MouseButton.Right))
            {
                // Calculate the offset of the mouse position
                var deltaX = mouse.X - _lastPos.X;
                var deltaY = mouse.Y - _lastPos.Y;
                _lastPos = new Vector2(mouse.X, mouse.Y);

                // Apply the camera pitch and yaw (we clamp the pitch in the camera class)
                camera.Yaw += deltaX * sensitivity;
                camera.Pitch -= deltaY * sensitivity; // Reversed since y-coordinates range from bottom to top

            }

            if (input.IsKeyPressed(Keys.Escape))
            {
                if (CursorGrabbed)
                {
                    CursorGrabbed = false;
                    CursorVisible = true;
                }
                else
                    Close();
            }

        }
    }
}
