
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
        private SimulatorCPU particleSystem;

      

        private BasicEffect effect;
        private RasterizerState rasterizerState;


        private VertexBuffer vertexBuffer;
        private IndexBuffer indexBuffer;

        public RendererXNA(GraphicsDevice graphicsDevice, SimulatorCPU system)
        {
            this.graphicsDevice = graphicsDevice;
            this.particleSystem = system;

            this.effect = new BasicEffect(graphicsDevice);
     

            // ---- Verticies ----
            var verticies = new VertexPositionTexture[]
            {
                // TL
                new VertexPositionTexture(new Vector3(-0.5f,0.5f,0), new Vector2(0,0f)),
                // TR
                new VertexPositionTexture(new Vector3(0.5f,0.5f,0), new Vector2(1f,0f)),
                // BL
                new VertexPositionTexture(new Vector3(-0.5f,-0.5f,0), new Vector2(0f,1f)),
                // BR
                new VertexPositionTexture(new Vector3(0.5f,-0.5f,0), new Vector2(1f,1f)),
            };
            vertexBuffer = new VertexBuffer(graphicsDevice, VertexPositionTexture.VertexDeclaration, verticies.Length, BufferUsage.WriteOnly);
            vertexBuffer.SetData(verticies);

            // ---- Indices ----
            int[] indices = new int[] {
                0,1,2,2,3,1
            };
            indexBuffer = new IndexBuffer(graphicsDevice, typeof(int), indices.Length, BufferUsage.WriteOnly);
            indexBuffer.SetData(indices);

            rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
        }

        public void Render()
        {
            // Set texture
            //graphicsDevice.Textures[0] = particleSystem.settings.RenderSettings.Texture;

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

    }
}
