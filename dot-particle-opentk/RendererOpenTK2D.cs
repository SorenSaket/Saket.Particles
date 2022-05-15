using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
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


		private int buffer_lifetime;

		private int buffer_position_x;
		private int buffer_position_y;
		private int buffer_position_z;
		
		private int buffer_rotation;

		private int buffer_scale_x;
		private int buffer_scale_y;

		private int buffer_color;

		
	

		private static Shader ShaderParticle;

		private int texObject;
		private SimulatorCPU system;


		private ModulePosition modulePosition;
		private ModuleRotation moduleRotation;

		private ModuleLifetime simulatorModuleLifetime;
		private ModuleColor simulatorModuleColor;

		private ModuleScale moduleScale;

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

			int layoutLocation = 0;

			// QuadVert buffer
			_quadBufferObject = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ArrayBuffer, _quadBufferObject);
			GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * quadVertices.Length, quadVertices, BufferUsageHint.StaticDraw);
			GL.VertexAttribPointer(layoutLocation, 2, VertexAttribPointerType.Float, false, sizeof(float)*2, 0);
			GL.EnableVertexAttribArray(layoutLocation);



			// X pos buffer
			layoutLocation++;
			buffer_position_x = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ArrayBuffer, buffer_position_x);
			GL.BufferData(BufferTarget.ArrayBuffer, system.Count * sizeof(float), modulePosition.PositionX, BufferUsageHint.StreamDraw);
			GL.VertexAttribPointer(layoutLocation, 1, VertexAttribPointerType.Float, false, sizeof(float), 0);
			GL.EnableVertexAttribArray(layoutLocation);
			GL.VertexAttribDivisor(layoutLocation, 1);

			// Y pos buffer
			layoutLocation++;
			buffer_position_y = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ArrayBuffer, buffer_position_y);
			GL.BufferData(BufferTarget.ArrayBuffer, system.Count * sizeof(float), modulePosition.PositionY, BufferUsageHint.StreamDraw);
			GL.VertexAttribPointer(layoutLocation, 1, VertexAttribPointerType.Float, false, sizeof(float), 0);
			GL.EnableVertexAttribArray(layoutLocation);
			GL.VertexAttribDivisor(layoutLocation, 1);

			// Z pos buffer
			layoutLocation++;
			buffer_position_z = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ArrayBuffer, buffer_position_z);
			GL.BufferData(BufferTarget.ArrayBuffer, system.Count * sizeof(float), modulePosition.PositionZ, BufferUsageHint.StreamDraw);
			GL.VertexAttribPointer(layoutLocation, 1, VertexAttribPointerType.Float, false, sizeof(float), 0);
			GL.EnableVertexAttribArray(layoutLocation);
			GL.VertexAttribDivisor(layoutLocation, 1);

			// Rotation Buffer
			layoutLocation++;
			moduleRotation = system.GetModule<ModuleRotation>();
			buffer_rotation = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ArrayBuffer, buffer_rotation);
			GL.BufferData(BufferTarget.ArrayBuffer, system.Count * sizeof(float), IntPtr.Zero, BufferUsageHint.StreamDraw);
			GL.VertexAttribPointer(layoutLocation, 1, VertexAttribPointerType.Float, false, sizeof(float), (IntPtr)0);
			GL.EnableVertexAttribArray(layoutLocation);
			GL.VertexAttribDivisor(layoutLocation, 1);

			moduleScale = system.GetModule<ModuleScale>();
			// X Scale Buffer
			layoutLocation++;
			
			if(moduleScale!= null)
            {
				buffer_scale_x = GL.GenBuffer();
				GL.BindBuffer(BufferTarget.ArrayBuffer, buffer_scale_x);
				GL.BufferData(BufferTarget.ArrayBuffer, system.Count * sizeof(float), IntPtr.Zero, BufferUsageHint.StreamDraw);
				GL.VertexAttribPointer(layoutLocation, 1, VertexAttribPointerType.Float, false, sizeof(float), (IntPtr)0);
				GL.EnableVertexAttribArray(layoutLocation);
				GL.VertexAttribDivisor(layoutLocation, 1);
			}

			
			
			// Y Scale Buffer
			layoutLocation++;
			if (moduleScale != null)
			{
				buffer_scale_y = GL.GenBuffer();
				GL.BindBuffer(BufferTarget.ArrayBuffer, buffer_scale_y);
				GL.BufferData(BufferTarget.ArrayBuffer, system.Count * sizeof(float), IntPtr.Zero, BufferUsageHint.StreamDraw);
				GL.VertexAttribPointer(layoutLocation, 1, VertexAttribPointerType.Float, false, sizeof(float), (IntPtr)0);
				GL.EnableVertexAttribArray(layoutLocation);
				GL.VertexAttribDivisor(layoutLocation, 1);
			}


			


			// color buffer
			layoutLocation++;
			simulatorModuleColor = system.GetModule<ModuleColor>();
			if (simulatorModuleColor != null)
            {
				buffer_color = GL.GenBuffer();
				GL.BindBuffer(BufferTarget.ArrayBuffer, buffer_color);
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
				buffer_lifetime = GL.GenBuffer();
				GL.BindBuffer(BufferTarget.ArrayBuffer, buffer_lifetime);
				GL.BufferData(BufferTarget.ArrayBuffer, system.Count * sizeof(float), simulatorModuleLifetime.LifeProgress, BufferUsageHint.StreamDraw);
				GL.VertexAttribPointer(layoutLocation, 1, VertexAttribPointerType.Float, false, sizeof(float), (IntPtr)0);
				GL.EnableVertexAttribArray(layoutLocation);
				GL.VertexAttribDivisor(layoutLocation, 1);
			}
			/*
			void CreateBuffer(ref int buffer, int size ,int layoutLocation, * )

			{
				// Generate the buffer
				buffer = GL.GenBuffer();
				// Bind the buffer
				GL.BindBuffer(BufferTarget.ArrayBuffer, buffer);
				// Set the default data
				GL.BufferData(BufferTarget.ArrayBuffer, system.Count * sizeof(T), IntPtr.Zero, BufferUsageHint.StreamDraw);
			
				//
				GL.VertexAttribPointer(layoutLocation, 4, VertexAttribPointerType.UnsignedByte, true, sizeof(uint), (IntPtr)0);

				GL.EnableVertexAttribArray(layoutLocation);
				GL.VertexAttribDivisor(layoutLocation, 1);

			}*/



			// Unbind to avoid leeking state
			GL.BindVertexArray(0);
		}


		public void Draw(ref Matrix4 view, ref Matrix4 projection)
		{
			
			//
			GL.BindBuffer(BufferTarget.ArrayBuffer, buffer_position_x);
			GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)0, system.Count * sizeof(float), modulePosition.PositionX);
			
			//
			GL.BindBuffer(BufferTarget.ArrayBuffer, buffer_position_y);
			GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)0, system.Count * sizeof(float), modulePosition.PositionY);

			GL.BindBuffer(BufferTarget.ArrayBuffer, buffer_position_z);
			GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)0, system.Count * sizeof(float), modulePosition.PositionZ);


			if (moduleRotation != null)
			{
				GL.BindBuffer(BufferTarget.ArrayBuffer, buffer_rotation);
				GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)0, system.Count * sizeof(float), moduleRotation.Rotation);
			}

			if (simulatorModuleColor != null)
			{
				GL.BindBuffer(BufferTarget.ArrayBuffer, buffer_color);
				GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)0, system.Count * sizeof(uint), simulatorModuleColor.Color);
			}

			if (simulatorModuleLifetime != null)
            {
				GL.BindBuffer(BufferTarget.ArrayBuffer, buffer_lifetime);
				GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)0, system.Count * sizeof(float), simulatorModuleLifetime.LifeProgress);
			}

			if(moduleScale != null)
            {
				GL.BindBuffer(BufferTarget.ArrayBuffer, buffer_scale_x);
				GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)0, system.Count * sizeof(float), moduleScale.ScaleX);
				GL.BindBuffer(BufferTarget.ArrayBuffer, buffer_scale_y);
				GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)0, system.Count * sizeof(float), moduleScale.ScaleY);
			}

			ShaderParticle.Use();

			ShaderParticle.SetMatrix4("view", view);
			ShaderParticle.SetMatrix4("projection", projection);

			GL.DepthMask(true);
			GL.Enable(EnableCap.DepthTest);
			GL.Enable(EnableCap.Blend);

			GL.BindVertexArray(_vertexArrayObject);
			GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
			GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, 6, system.Count);

			// Unbind to avoid leeking state
			GL.BindVertexArray(0);
		}
	}
}
