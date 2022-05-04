using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Core.Particles
{
	/// <summary> Particle Datastucture </summary>

	/// <remarks> Size = 384 bits = 32 + 32 + 32 + (32+32) + 32 + (32+32)+ (32+32) + 32 + 32</remarks>
	// Lifeprogress has to be computed each iterration because the starting lifetime varies between particles
	[StructLayout(LayoutKind.Sequential)]
	public struct ParticleSOA
	{
		public readonly int Length { get; }
		public ParticleSOA(int size)
		{
			Length = size;
			Lifetime			= new float[size];
			CurrentLifetime		= new float[size];
			LifeProgress		= new float[size];
			PositionX			= new float[size];
			PositionY			= new float[size];
			Rotation			= new float[size];
			ScaleX				= new float[size];
			ScaleY				= new float[size];
			VelocityX			= new float[size];
			VelocityY			= new float[size];
			RotationalVelocity	= new float[size];

			Color				= new Color[size];
		}

		/// <summary> The absolute max lifetime in seconds </summary>
		public float[] Lifetime { get; set; }
		/// <summary> The absolute current lifetime in seconds </summary>
		public float[] CurrentLifetime { get; set; }
		/// <summary> Relative 0..1 lifetime progress </summary>
		public float[] LifeProgress { get; set; }
		/// <summary>  World Space coordinates </summary>
		public float[] PositionX { get; set; }
		/// <summary>  World Space coordinates </summary>
		public float[] PositionY { get; set; }
		/// <summary>  World space rotaiton in radians </summary>
		public float[] Rotation { get; set; }
		/// <summary>  Scale </summary>
		public float[] ScaleX { get; set; }
		/// <summary>  Scale </summary>
		public float[] ScaleY { get; set; }
		/// <summary> Velocity in Pixels per Second </summary>
		public float[] VelocityX { get; set; }
		/// <summary> Velocity in Pixels per Second </summary>
		public float[] VelocityY { get; set; }
		/// <summary> Rotational Velocity in Radias Per Second</summary>
		public float[] RotationalVelocity { get; set; }
		/// <summary> Color </summary>
		/// Color is a packed uint
		public Color[] Color { get; set; }
	}
}