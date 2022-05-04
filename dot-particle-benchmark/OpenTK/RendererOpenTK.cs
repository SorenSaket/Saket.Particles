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
		private int _vertexPositionXBufferObject;
		private int _vertexPositionYBufferObject;


		private static Shader ShaderParticle;


		private ParticleSystem system;

		float[] quadVertices = new float[]{
			// positions
			-0.05f,  0.05f,
			 0.05f, -0.05f,
			-0.05f, -0.05f,

			-0.05f,  0.05f,
			 0.05f, -0.05f,
			 0.05f,  0.05f,
		};

		public RendererOpenTK(ParticleSystem system)
		{
			this.system = system;
			if (ShaderParticle == null)
				ShaderParticle = new Shader("Assets/Shaders/Particle/vertex.glsl", "Assets/Shaders/Particle/fragment.glsl", "Assets/Shaders/Particle/geometry.glsl"); // 
																															 
			_vertexArrayObject = GL.GenVertexArray();
			GL.BindVertexArray(_vertexArrayObject);

			// X pos buffer
			_vertexPositionXBufferObject = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexPositionXBufferObject);
			GL.BufferData(BufferTarget.ArrayBuffer, system.Count * sizeof(float), system.particles.PositionX, BufferUsageHint.StreamDraw);
			GL.VertexAttribPointer(0, 1, VertexAttribPointerType.Float, false, 1, 0);
			GL.EnableVertexAttribArray(0);

			// X pos buffer
			_vertexPositionYBufferObject = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexPositionYBufferObject);
			GL.BufferData(BufferTarget.ArrayBuffer, system.Count * sizeof(float), system.particles.PositionY, BufferUsageHint.StreamDraw);
			GL.VertexAttribPointer(1, 1, VertexAttribPointerType.Float, false, 1, 0);
			GL.EnableVertexAttribArray(1);
		}


		public void Draw()
		{
			//
			GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexPositionXBufferObject);
			GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)0, system.Count * sizeof(float), system.particles.PositionX);
			GL.VertexAttribDivisor(0, 1);

			//
			GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexPositionYBufferObject);
			GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)0, system.Count * sizeof(float), system.particles.PositionY);
			GL.VertexAttribDivisor(1, 1);


			ShaderParticle.Use();
			/*
			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture2D, texture);*/

			GL.BindVertexArray(_vertexArrayObject);
			GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
			GL.DrawArraysInstanced(PrimitiveType.Points, 0, 1, system.Count);

		}
	}
}
