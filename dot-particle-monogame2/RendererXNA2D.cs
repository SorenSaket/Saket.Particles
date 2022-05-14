
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
    public class RendererXNA2D 
    {
        private readonly GraphicsDevice graphicsDevice;
        private readonly SimulatorCPU particleSystem;

        private readonly RasterizerState rasterizerState;
        private readonly VertexBufferBinding[] bindings;

        private readonly VertexBuffer vertexBuffer;
        private readonly IndexBuffer indexBuffer;

        private readonly VertexBuffer buffer_position_x;
        private readonly VertexBuffer buffer_position_y;


        private Effect effect;


        private ModulePosition module2D;

        public RendererXNA2D(GraphicsDevice graphicsDevice, Effect shader, SimulatorCPU system)
        {
            this.graphicsDevice = graphicsDevice;
            this.particleSystem = system;
            this.effect = shader;
            this.module2D = system.GetModule<ModulePosition>();


            // ---- Verticies buffer ----
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

            // ---- Indices buffer ----
            int[] indices = new int[] {
                0,1,2,2,3,1
            };
            indexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.SixteenBits, indices.Length, BufferUsage.WriteOnly);
            indexBuffer.SetData(indices);

            // ---- Instance buffers ----
            buffer_position_x = new VertexBuffer(graphicsDevice,
                new VertexDeclaration(
                    new VertexElement(0, VertexElementFormat.Single, VertexElementUsage.Position, 1)), system.Count, BufferUsage.WriteOnly);
            
            buffer_position_y = new VertexBuffer(
                graphicsDevice,
                new VertexDeclaration(
                    new VertexElement(0, VertexElementFormat.Single, VertexElementUsage.Position, 2)
                ), 
                system.Count, 
                BufferUsage.WriteOnly
                );

            // ---- Bindings ----
            bindings = new VertexBufferBinding[]
            {
                new VertexBufferBinding(vertexBuffer),
                new VertexBufferBinding(buffer_position_x,0,1),
                new VertexBufferBinding(buffer_position_y,0,1)
            };

            // ---- Define raster state ----
            rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;

        }

        public void Update()
        {
            buffer_position_x.SetData(module2D.PositionX);
            buffer_position_y.SetData(module2D.PositionY);
        }

        public void Draw(ref Matrix view, ref Matrix projection)
        {
            // Set texture
            //graphicsDevice.Textures[0] = particleSystem.settings.RenderSettings.Texture;
            //
            //effect.Projection =





            // set other state
            graphicsDevice.RasterizerState = RasterizerState.CullNone;
            graphicsDevice.BlendState = BlendState.Opaque;
            graphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
            //graphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;


            effect.CurrentTechnique = effect.Techniques["Instancing"];
            effect.Parameters["WVP"].SetValue(view * projection);

       
            // Set Index buffer
            graphicsDevice.Indices = indexBuffer;
            
            // Render all passes
            effect.CurrentTechnique.Passes[0].Apply();

            // Set Vertex Buffer and Instance buffers
            graphicsDevice.SetVertexBuffers(bindings);
            //
            graphicsDevice.DrawInstancedPrimitives(
                PrimitiveType.TriangleList,
                0,
                0,
                2,
                (particleSystem.Count));
            
        }

    }
}
