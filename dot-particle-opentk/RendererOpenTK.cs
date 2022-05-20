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
	class RendererOpenTK 
	{
		// VAO
		private int _vertexArrayObject;

		// BUFFERS
		// STATIC
		private int _quadBufferObject;
		// DYNAMIC
		private int buffer_lifetime;
		private int buffer_position_x;
		private int buffer_position_y;
		private int buffer_position_z;
		private int buffer_rotation;
		private int buffer_scale_x;
		private int buffer_scale_y;
		private int buffer_color;
		private int buffer_sheet;

		// MODULES
		private ModulePosition modulePosition;
		private ModuleRotation moduleRotation;

		private ModuleLifetime simulatorModuleLifetime;
		private ModuleColor simulatorModuleColor;

		private ModuleScale moduleScale;

		private ModuleSheet moduleSheet;

		// OTHER
		private static Shader ShaderParticle;
		private int texObject;
		private SimulatorCPU system;
		
		const float size = 0.01f;


		// TL---TR
		// | \  |
		// |  \ |
		// BL---BR

		float[] quadVertices = new float[]{
			// positions  // UVS
			-size, -size, 0,0,
			 size, -size, 1,0, 
			 size,  size, 1,1,
			
			-size, -size, 0,0,
			 size,  size, 1,1,
			 -size,  size, 0,1,
		};


		public RendererOpenTK(SimulatorCPU system)
		{
			this.system = system;
			
			if (ShaderParticle == null)
				ShaderParticle = new Shader("Assets/Shaders/Particle/vertex.glsl", "Assets/Shaders/Particle/fragment.glsl"); // , "Assets/Shaders/Particle/geometry.glsl"


			_vertexArrayObject = GL.GenVertexArray();
			GL.BindVertexArray(_vertexArrayObject);

			int layoutLocation = 0;

            // QuadVert buffer
            {
				_quadBufferObject = GL.GenBuffer();
				GL.BindBuffer(BufferTarget.ArrayBuffer, _quadBufferObject);
				GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * quadVertices.Length, quadVertices, BufferUsageHint.StaticDraw);

				// POS
				GL.VertexAttribPointer(layoutLocation, 2, VertexAttribPointerType.Float, false, sizeof(float) * 4, 0);
				GL.EnableVertexAttribArray(layoutLocation);

				// UV
				layoutLocation++;
				GL.VertexAttribPointer(layoutLocation, 2, VertexAttribPointerType.Float, false, sizeof(float) * 4, sizeof(float) * 2);
				GL.EnableVertexAttribArray(layoutLocation);
			}
			
			// POSITIONS
			{
				this.modulePosition = system.GetModule<ModulePosition>();
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
			}

            // ROTATION
            {
				layoutLocation++;
				moduleRotation = system.GetModule<ModuleRotation>();
				if (moduleRotation != null)
				{
					buffer_rotation = GL.GenBuffer();
					GL.BindBuffer(BufferTarget.ArrayBuffer, buffer_rotation);
					GL.BufferData(BufferTarget.ArrayBuffer, system.Count * sizeof(float), IntPtr.Zero, BufferUsageHint.StreamDraw);
					GL.VertexAttribPointer(layoutLocation, 1, VertexAttribPointerType.Float, false, sizeof(float), (IntPtr)0);
					GL.EnableVertexAttribArray(layoutLocation);
					GL.VertexAttribDivisor(layoutLocation, 1);
				}
			}

			// SCALE
			{ 
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
			}

            // COLOR
            {
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
			}

            // Lifetime
            {
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

			}

            // Sheet
            {
				layoutLocation++;
				this.moduleSheet = system.GetModule<ModuleSheet>();
				if (moduleSheet != null)
				{
					buffer_sheet = GL.GenBuffer();
					GL.BindBuffer(BufferTarget.ArrayBuffer, buffer_sheet);
					GL.BufferData(BufferTarget.ArrayBuffer, system.Count * sizeof(byte), IntPtr.Zero, BufferUsageHint.StreamDraw);
					GL.VertexAttribIPointer(layoutLocation, 1, VertexAttribIntegerType.Byte, sizeof(byte), IntPtr.Zero);
					GL.EnableVertexAttribArray(layoutLocation);
					GL.VertexAttribDivisor(layoutLocation, 1);
				}
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

			if(moduleSheet != null)
            {
				GL.BindBuffer(BufferTarget.ArrayBuffer, buffer_sheet);
				GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)0, system.Count * sizeof(byte), moduleSheet.SpriteIndex);
			}


			ShaderParticle.Use();

			ShaderParticle.SetMatrix4("view", view);
			ShaderParticle.SetMatrix4("projection", projection);

			GL.DepthMask(true);
			GL.Enable(EnableCap.DepthTest);
			GL.Enable(EnableCap.Blend);
			GL.Enable(EnableCap.CullFace);
			GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
			GL.BlendEquation(BlendEquationMode.FuncAdd);
			//GL.BlendFuncSeparate(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha, BlendingFactorSrc.One, BlendingFactorDest.One);

			GL.BindVertexArray(_vertexArrayObject);
			GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
			GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, 6, system.Count);

			// Unbind to avoid leeking state
			GL.BindVertexArray(0);
		}
	}
}
