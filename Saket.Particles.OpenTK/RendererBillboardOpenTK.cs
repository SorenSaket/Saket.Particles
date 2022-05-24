using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;

namespace Saket.Particles.OpenTK
{
	public class RendererBillboardOpenTK 
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
		private ModuleLifetime moduleLifetime;
		private ModuleColor moduleColor;
		private ModuleScale moduleScale;
		private ModuleSheet moduleSheet;

		private SimulatorCPU system;

		// TL---TR
		// | \  |
		// |  \ |
		// BL---BR
		const float size = 0.5f;
		float[] quadVertices = new float[]{
			// positions  // UVS
			-size, -size, 0,0,
			 size, -size, 1,0, 
			 size,  size, 1,1,
			
			-size, -size, 0,0,
			 size,  size, 1,1,
			 -size,  size, 0,1,
		};

		public RendererBillboardOpenTK(SimulatorCPU system)
		{
			this.system = system;

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
				GL.BufferData(BufferTarget.ArrayBuffer, system.Count * sizeof(float), IntPtr.Zero, BufferUsageHint.DynamicDraw);
				GL.VertexAttribPointer(layoutLocation, 1, VertexAttribPointerType.Float, false, sizeof(float), 0);
				GL.EnableVertexAttribArray(layoutLocation);
				GL.VertexAttribDivisor(layoutLocation, 1);

				// Y pos buffer
				layoutLocation++;
				buffer_position_y = GL.GenBuffer();
				GL.BindBuffer(BufferTarget.ArrayBuffer, buffer_position_y);
				GL.BufferData(BufferTarget.ArrayBuffer, system.Count * sizeof(float), IntPtr.Zero, BufferUsageHint.DynamicDraw);
				GL.VertexAttribPointer(layoutLocation, 1, VertexAttribPointerType.Float, false, sizeof(float), 0);
				GL.EnableVertexAttribArray(layoutLocation);
				GL.VertexAttribDivisor(layoutLocation, 1);

				// Z pos buffer
				layoutLocation++;
				buffer_position_z = GL.GenBuffer();
				GL.BindBuffer(BufferTarget.ArrayBuffer, buffer_position_z);
				GL.BufferData(BufferTarget.ArrayBuffer, system.Count * sizeof(float), IntPtr.Zero, BufferUsageHint.DynamicDraw);
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
					GL.BufferData(BufferTarget.ArrayBuffer, system.Count * sizeof(float), IntPtr.Zero, BufferUsageHint.DynamicDraw);
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
					GL.BufferData(BufferTarget.ArrayBuffer, system.Count * sizeof(float), IntPtr.Zero, BufferUsageHint.DynamicDraw);
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
					GL.BufferData(BufferTarget.ArrayBuffer, system.Count * sizeof(float), IntPtr.Zero, BufferUsageHint.DynamicDraw);
					GL.VertexAttribPointer(layoutLocation, 1, VertexAttribPointerType.Float, false, sizeof(float), (IntPtr)0);
					GL.EnableVertexAttribArray(layoutLocation);
					GL.VertexAttribDivisor(layoutLocation, 1);
				}
			}

            // COLOR
            {
				layoutLocation++;
				moduleColor = system.GetModule<ModuleColor>();
				if (moduleColor != null)
				{
					buffer_color = GL.GenBuffer();
					GL.BindBuffer(BufferTarget.ArrayBuffer, buffer_color);
					GL.BufferData(BufferTarget.ArrayBuffer, system.Count * sizeof(uint), IntPtr.Zero, BufferUsageHint.DynamicDraw);
					GL.VertexAttribPointer(layoutLocation, 4, VertexAttribPointerType.UnsignedByte, true, sizeof(uint), (IntPtr)0);
					GL.EnableVertexAttribArray(layoutLocation);
					GL.VertexAttribDivisor(layoutLocation, 1);
				}
			}

            // Lifetime
            {
				layoutLocation++;
				this.moduleLifetime = system.GetModule<ModuleLifetime>();
				if (moduleLifetime != null)
				{
					buffer_lifetime = GL.GenBuffer();
					GL.BindBuffer(BufferTarget.ArrayBuffer, buffer_lifetime);
					GL.BufferData(BufferTarget.ArrayBuffer, system.Count * sizeof(float), IntPtr.Zero, BufferUsageHint.DynamicDraw);
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
					GL.BufferData(BufferTarget.ArrayBuffer, system.Count * sizeof(byte), IntPtr.Zero, BufferUsageHint.DynamicDraw);
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

		public virtual void Update()
        {
			// Position
			{
				// X
				GL.BindBuffer(BufferTarget.ArrayBuffer, buffer_position_x);
				GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)0, system.Count * sizeof(float), modulePosition.PositionX);
				// Y
				GL.BindBuffer(BufferTarget.ArrayBuffer, buffer_position_y);
				GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)0, system.Count * sizeof(float), modulePosition.PositionY);
				// Z
				GL.BindBuffer(BufferTarget.ArrayBuffer, buffer_position_z);
				GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)0, system.Count * sizeof(float), modulePosition.PositionZ);
			}

			if (moduleRotation != null)
			{
				GL.BindBuffer(BufferTarget.ArrayBuffer, buffer_rotation);
				GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)0, system.Count * sizeof(float), moduleRotation.Rotation);
			}

			if (moduleColor != null)
			{
				GL.BindBuffer(BufferTarget.ArrayBuffer, buffer_color);
				GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)0, system.Count * sizeof(uint), moduleColor.Color);
			}

			if (moduleLifetime != null)
			{
				GL.BindBuffer(BufferTarget.ArrayBuffer, buffer_lifetime);
				GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)0, system.Count * sizeof(float), moduleLifetime.LifeProgress);
			}

			if (moduleScale != null)
			{
				GL.BindBuffer(BufferTarget.ArrayBuffer, buffer_scale_x);
				GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)0, system.Count * sizeof(float), moduleScale.ScaleX);
				GL.BindBuffer(BufferTarget.ArrayBuffer, buffer_scale_y);
				GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)0, system.Count * sizeof(float), moduleScale.ScaleY);
			}

			if (moduleSheet != null)
			{
				GL.BindBuffer(BufferTarget.ArrayBuffer, buffer_sheet);
				GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)0, system.Count * sizeof(byte), moduleSheet.SpriteIndex);
			}

		}

		public virtual void Draw()
        {
		


			GL.DepthMask(true);
			//
			GL.Enable(EnableCap.DepthTest);
			
			//
			//GL.Enable(EnableCap.Blend);
			//GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
			//GL.BlendEquation(BlendEquationMode.FuncAdd);
			
			//
			GL.PolygonMode(MaterialFace.Front, PolygonMode.Fill);
			// Enable Backface culling
			GL.Enable(EnableCap.CullFace);

			// Bind VAO
			GL.BindVertexArray(_vertexArrayObject);

			// Draw All the Particles
			GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, 6, system.Count);

			// Unbind to avoid leeking state
			GL.BindVertexArray(0);
		}
	}
}
