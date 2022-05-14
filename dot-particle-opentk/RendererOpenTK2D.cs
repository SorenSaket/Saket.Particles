using OpenTK.Graphics.OpenGL4;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Particles.Rendering
{
	class RendererOpenTK2D 
	{
		private int _vertexArrayObject;
		private int _quadBufferObject;
		private int _vertexPositionXBufferObject;
		private int _vertexPositionYBufferObject;

		private int colorBufferObject;

		private int rotationBufferObject;
		private int lifetimeBufferObject;

		private static Shader ShaderParticle;

		private int texObject;
		private SimulatorCPU system;


		private ModulePosition modulePosition;
		private ModuleRotation moduleRotation;

		private ModuleLifetime simulatorModuleLifetime;
		private ModuleColor simulatorModuleColor;


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

		public RendererOpenTK2D(SimulatorCPU system)
		{
			this.system = system;
			this.modulePosition = system.GetModule<ModulePosition>();
			

			if (ShaderParticle == null)
				ShaderParticle = new Shader("Assets/Shaders/Particle/vertex.glsl", "Assets/Shaders/Particle/fragment.glsl"); // , "Assets/Shaders/Particle/geometry.glsl"


			_vertexArrayObject = GL.GenVertexArray();
			GL.BindVertexArray(_vertexArrayObject);

			/*
			int width = system.settings.RenderSettings.Width;
			int height = system.settings.RenderSettings.Height;
			int depth = system.settings.RenderSettings.Frames.Length;

			uint[] textures = new uint[width* height * depth];
			for (int i = 0; i < depth; i++)
			{
				//tex.ge(0, system.settings.RenderSettings.Frames[i], textures, width * height * i, width * height);
			}
			*/
			/*
			texObject = GL.GenTexture();
			GL.BindTexture(TextureTarget.Texture2DArray, texObject);
			GL.TexImage3D(
				TextureTarget.Texture2DArray, 
				0, 
				PixelInternalFormat.Rgba8,
				width, 
				height, 
				depth, 
				0, 
				PixelFormat.Rgba,
				PixelType.UnsignedInt8888,
				textures);
			GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
			GL.ActiveTexture(TextureUnit.Texture0);
			*/
			int layoutLocation = 0;


			// QuadVert buffer
			_quadBufferObject = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ArrayBuffer, _quadBufferObject);
			GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * quadVertices.Length, quadVertices, BufferUsageHint.StaticDraw);
			GL.VertexAttribPointer(layoutLocation, 2, VertexAttribPointerType.Float, false, sizeof(float)*2, 0);
			GL.EnableVertexAttribArray(layoutLocation);

			// X pos buffer
			layoutLocation++;
			_vertexPositionXBufferObject = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexPositionXBufferObject);
			GL.BufferData(BufferTarget.ArrayBuffer, system.Count * sizeof(float), modulePosition.PositionX, BufferUsageHint.StreamDraw);
			GL.VertexAttribPointer(layoutLocation, 1, VertexAttribPointerType.Float, false, sizeof(float), 0);
			GL.EnableVertexAttribArray(layoutLocation);
			GL.VertexAttribDivisor(layoutLocation, 1);

			// Y pos buffer
			layoutLocation++;
			_vertexPositionYBufferObject = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexPositionYBufferObject);
			GL.BufferData(BufferTarget.ArrayBuffer, system.Count * sizeof(float), modulePosition.PositionY, BufferUsageHint.StreamDraw);
			GL.VertexAttribPointer(layoutLocation, 1, VertexAttribPointerType.Float, false, sizeof(float), 0);
			GL.EnableVertexAttribArray(layoutLocation);
			GL.VertexAttribDivisor(layoutLocation, 1);


			// Rotation Buffer
			layoutLocation++;
			moduleRotation = system.GetModule<ModuleRotation>();
			rotationBufferObject = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ArrayBuffer, rotationBufferObject);
			GL.BufferData(BufferTarget.ArrayBuffer, system.Count * sizeof(float), IntPtr.Zero, BufferUsageHint.StreamDraw);
			GL.VertexAttribPointer(layoutLocation, 1, VertexAttribPointerType.Float, false, sizeof(float), (IntPtr)0);
			GL.EnableVertexAttribArray(layoutLocation);
			GL.VertexAttribDivisor(layoutLocation, 1);
			
		
			// color buffer
			layoutLocation++;
			simulatorModuleColor = system.GetModule<ModuleColor>();
			if (simulatorModuleColor != null)
            {
				colorBufferObject = GL.GenBuffer();
				GL.BindBuffer(BufferTarget.ArrayBuffer, colorBufferObject);
				GL.BufferData(BufferTarget.ArrayBuffer, system.Count * sizeof(uint), IntPtr.Zero, BufferUsageHint.StreamDraw);
				GL.VertexAttribPointer(layoutLocation, 4, VertexAttribPointerType.UnsignedByte, true, sizeof(uint), (IntPtr)0);
				GL.EnableVertexAttribArray(layoutLocation);
				GL.VertexAttribDivisor(layoutLocation, 1);
			}

			// Lifetime
			layoutLocation++;
			this.simulatorModuleLifetime = system.GetModule<ModuleLifetime>();
			if (simulatorModuleLifetime != null)
            {
				lifetimeBufferObject = GL.GenBuffer();
				GL.BindBuffer(BufferTarget.ArrayBuffer, lifetimeBufferObject);
				GL.BufferData(BufferTarget.ArrayBuffer, system.Count * sizeof(float), simulatorModuleLifetime.LifeProgress, BufferUsageHint.StreamDraw);
				GL.VertexAttribPointer(layoutLocation, 1, VertexAttribPointerType.Float, false, sizeof(float), (IntPtr)0);
				GL.EnableVertexAttribArray(layoutLocation);
				GL.VertexAttribDivisor(layoutLocation, 1);
			}

		}


		public void Draw()
		{
			//
			GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexPositionXBufferObject);
			GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)0, system.Count * sizeof(float), modulePosition.PositionX);
			
			//
			GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexPositionYBufferObject);
			GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)0, system.Count * sizeof(float), modulePosition.PositionY);
			
			if (moduleRotation != null)
			{
				GL.BindBuffer(BufferTarget.ArrayBuffer, rotationBufferObject);
				GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)0, system.Count * sizeof(float), moduleRotation.Rotation);
			}

			if (simulatorModuleColor != null)
			{
				GL.BindBuffer(BufferTarget.ArrayBuffer, colorBufferObject);
				GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)0, system.Count * sizeof(uint), simulatorModuleColor.Color);
			}

			if(simulatorModuleLifetime != null)
            {
				GL.BindBuffer(BufferTarget.ArrayBuffer, lifetimeBufferObject);
				GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)0, system.Count * sizeof(float), simulatorModuleLifetime.LifeProgress);
			}
			
			
			ShaderParticle.Use();

			GL.BindVertexArray(_vertexArrayObject);
			GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
			GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, 6, system.Count);
		}
	}
}
