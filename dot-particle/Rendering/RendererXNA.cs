using Core.Particles.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace Core.Particles
{
    public class RendererXNA 
    {
        private GraphicsDevice graphicsDevice;
        private ParticleSystem particleSystem;

        private IndexBuffer indexBuffer;
        private VertexBuffer vertexBuffer;

        private BasicEffect effect;
        VertexPositionColorTexture[] vertices;
        private RasterizerState rasterizerState;
        public RendererXNA(GraphicsDevice graphicsDevice, ParticleSystem system)
        {
            this.graphicsDevice = graphicsDevice;
            this.particleSystem = system;

            this.effect = new BasicEffect(graphicsDevice);
            // Create graphics buffers
            // 4 verticies per particle
            this.vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionColorTexture), system.Count * 4, BufferUsage.WriteOnly);
            vertices = new VertexPositionColorTexture[system.particles.Length * 4];



            this.indexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.ThirtyTwoBits, system.Count * 6, BufferUsage.WriteOnly);
            SetIndexBuffer();

            rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
        }

        public void Render()
        {
            // Set texture
            graphicsDevice.Textures[0] = particleSystem.settings.RenderSettings.Texture;

            UpdateVertexBuffer(particleSystem);
            //
            graphicsDevice.SetVertexBuffer(vertexBuffer);
            graphicsDevice.Indices = indexBuffer;

			graphicsDevice.RasterizerState = rasterizerState;
            graphicsDevice.BlendState = BlendState.Opaque;
            graphicsDevice.DepthStencilState = DepthStencilState.Default;

            graphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            effect.VertexColorEnabled = true;
            effect.TextureEnabled = true;

            effect.Projection = Matrix.CreateOrthographic((float)graphicsDevice.Viewport.Width , (float)graphicsDevice.Viewport.Height, 0, 100.0f);


            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                // This is the all-important line that sets the effect, and all of its settings, on the graphics device
                pass.Apply();
                /*
                VertexPositionColor[] vertices = new VertexPositionColor[3];
                vertices[0].Position = new Vector3(100, 200, 0f);
                vertices[0].Color = Color.Red;
                vertices[1].Position = new Vector3(0, 0, 0f);
                vertices[1].Color = Color.Green;
                vertices[2].Position = new Vector3(200, 200, 0f);
                vertices[2].Color = Color.Yellow;
                graphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, vertices, 0, 1);
                */
                
                graphicsDevice.DrawIndexedPrimitives(
                  PrimitiveType.TriangleList,
                  0,
                  0,
                  (particleSystem.Count * 2));

                
            }
        }

        private void UpdateVertexBuffer(ParticleSystem system)
		{
            for (int i = 0; i < system.particles.Length; i++)
            {   /*
                     *  TL    TR
                     *   0----1 0,1,2,3 = index offsets for vertex indices
                     *   |   /| TL,TR,BL,BR are vertex references in SpriteBatchItem.
                     *   |  / |
                     *   | /  |
                     *   |/   |
                     *   2----3
                     *  BL    BR
                     */
                
                int frameIndex = (int)(system.particles.LifeProgress[i] * (system.settings.RenderSettings.Frames.Length - 1));
                Rectangle frame = system.settings.RenderSettings.Frames[frameIndex];

                int vertIndex = i * 4;

                // Top Left
                vertices[vertIndex + 0].Position.X = system.particles.PositionX[i];
                vertices[vertIndex + 0].Position.Y = system.particles.PositionY[i];
                vertices[vertIndex + 0].Position.Z = 0;
                vertices[vertIndex + 0].Color = system.particles.Color[i];

                vertices[vertIndex + 0].TextureCoordinate.X = frame.Left;
                vertices[vertIndex + 0].TextureCoordinate.Y = frame.Top;

                // Top Right
                vertices[vertIndex + 1].Position.X = system.particles.PositionX[i] + system.particles.ScaleX[i];
                vertices[vertIndex + 1].Position.Y = system.particles.PositionY[i];
                vertices[vertIndex + 1].Position.Z = 0;
                vertices[vertIndex + 1].Color = system.particles.Color[i];
                vertices[vertIndex + 1].TextureCoordinate.X = frame.Right;
                vertices[vertIndex + 1].TextureCoordinate.Y = frame.Top;

                // Bottom Left
                vertices[vertIndex + 2].Position.X = system.particles.PositionX[i];
                vertices[vertIndex + 2].Position.Y = system.particles.PositionY[i] + system.particles.ScaleY[i];
                vertices[vertIndex + 2].Position.Z = 0;
                vertices[vertIndex + 2].Color = system.particles.Color[i];
                vertices[vertIndex + 2].TextureCoordinate.X = frame.Left;
                vertices[vertIndex + 2].TextureCoordinate.Y = frame.Bottom;

                // Bottom Right
                vertices[vertIndex + 3].Position.X = system.particles.PositionX[i] + system.particles.ScaleX[i];
                vertices[vertIndex + 3].Position.Y = system.particles.PositionY[i] + system.particles.ScaleY[i];
                vertices[vertIndex + 3].Position.Z = 0;
                vertices[vertIndex + 3].Color = system.particles.Color[i];
                vertices[vertIndex + 3].TextureCoordinate.X = frame.Right;
                vertices[vertIndex + 3].TextureCoordinate.Y = frame.Bottom;
            }
            vertexBuffer.SetData(vertices);
        }


        private unsafe void SetIndexBuffer()
		{
            // Copied from EnsureArrayCapacity in Microsoft.Xna.Framework.Graphics.SpriteBatcher
            int[] newIndex = new int[indexBuffer.IndexCount];

            fixed (int* indexFixedPtr = newIndex)
            {
                var indexPtr = indexFixedPtr;
                for (var i = 0; i < particleSystem.Count; i++, indexPtr += 6)
                {
                    /*
                     *  TL    TR
                     *   0----1 0,1,2,3 = index offsets for vertex indices
                     *   |   /| TL,TR,BL,BR are vertex references in SpriteBatchItem.
                     *   |  / |
                     *   | /  |
                     *   |/   |
                     *   2----3
                     *  BL    BR
                     */
                    // Triangle 1
                    *(indexPtr + 0) = (int)(i * 4);
                    *(indexPtr + 1) = (int)(i * 4 + 1);
                    *(indexPtr + 2) = (int)(i * 4 + 2);
                    // Triangle 2
                    *(indexPtr + 3) = (int)(i * 4 + 1);
                    *(indexPtr + 4) = (int)(i * 4 + 3);
                    *(indexPtr + 5) = (int)(i * 4 + 2);
                }
            }
            indexBuffer.SetData(newIndex);
        }
    }
}
