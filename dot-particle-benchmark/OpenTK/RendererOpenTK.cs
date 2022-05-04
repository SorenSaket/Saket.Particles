using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Particles.Rendering
{
	class RendererOpenTK 
	{
		private int _vertexArrayObject;
		private int _quadBufferObject;
		private int _vertexPositionXBufferObject;
		private int _vertexPositionYBufferObject;


		private static Shader ShaderParticle;


		private ParticleSystem system;


		const float size = 0.01f;
		float[] quadVertices = new float[]{
			// positions
			-size,  size,
			 size, -size,
			-size, -size,

			-size,  size,
			 size, -size,
			 size,  size,
		};

		public RendererOpenTK(ParticleSystem system)
		{
			this.system = system;
			if (ShaderParticle == null)
				ShaderParticle = new Shader("Assets/Shaders/Particle/vertex.glsl", "Assets/Shaders/Particle/fragment.glsl"); // , "Assets/Shaders/Particle/geometry.glsl"

			_vertexArrayObject = GL.GenVertexArray();
			GL.BindVertexArray(_vertexArrayObject);

			// QuadVert buffer
			_quadBufferObject = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ArrayBuffer, _quadBufferObject);
			GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * quadVertices.Length, quadVertices, BufferUsageHint.StaticDraw);
			GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, sizeof(float)*2, 0);
			GL.EnableVertexAttribArray(0);

			// X pos buffer
			_vertexPositionXBufferObject = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexPositionXBufferObject);
			GL.BufferData(BufferTarget.ArrayBuffer, system.Count * sizeof(float), system.particles.PositionX, BufferUsageHint.StreamDraw);
			GL.VertexAttribPointer(1, 1, VertexAttribPointerType.Float, false, sizeof(float), 0);
			GL.EnableVertexAttribArray(1);
			GL.VertexAttribDivisor(1, 1);
			
			// X pos buffer
			_vertexPositionYBufferObject = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexPositionYBufferObject);
			GL.BufferData(BufferTarget.ArrayBuffer, system.Count * sizeof(float), system.particles.PositionY, BufferUsageHint.StreamDraw);
			GL.VertexAttribPointer(2, 1, VertexAttribPointerType.Float, false, sizeof(float), 0);
			GL.EnableVertexAttribArray(2);
			GL.VertexAttribDivisor(2, 1);
		}


		public void Draw()
		{
			//
			GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexPositionXBufferObject);
			GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)0, system.Count * sizeof(float), system.particles.PositionX);
			
			//
			GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexPositionYBufferObject);
			GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)0, system.Count * sizeof(float), system.particles.PositionY);

			ShaderParticle.Use();
			/*
			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture2D, texture);*/

			GL.BindVertexArray(_vertexArrayObject);
			GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
			GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, 6, system.Count);

		}
	}
}
