using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Core.Particles
{
	public struct Particle
	{
		public float Lifetime;
		public float PositionX;
		public float PositionY;
		public float Rotation;

		public float VelocityX;
		public float VelocityY;
		public float RotationalVelocity;

		public float ScaleX;
		public float ScaleY;
		public uint  Color;
	}

	/// <summary> Particle Datastucture </summary>
	/// <remarks> </remarks>
	// Lifeprogress has to be computed each iterration because the starting lifetime varies between particles
	[StructLayout(LayoutKind.Sequential)]
	public struct ParticlePublic
	{
		public readonly int Length { get; }
		public ParticlePublic(int size)
		{
			Length				= size;

			Lifetime			= new float[size];
			CurrentLifetime		= new float[size];
			LifeProgress		= new float[size];

			PositionX			= new float[size];
			VelocityX			= new float[size];

			PositionY			= new float[size];
			VelocityY			= new float[size];
			
			Rotation			= new float[size];
			RotationalVelocity	= new float[size];

			ScaleX				= new float[size];
			ScaleY				= new float[size];

			Color				= new uint[size];
		}
		// --------- Lifetime ---------

		/// <summary> The absolute max lifetime in seconds </summary>
		public float[] Lifetime { get; set; }
		/// <summary> The absolute current lifetime in seconds </summary>
		public float[] CurrentLifetime { get; set; }
		/// <summary> Relative 0..1 lifetime progress. Saved to prevent recalculation </summary>
		public float[] LifeProgress { get; set; }
		// --------- Positional State ---------

		/// <summary> X World Space coordinates </summary>
		public float[] PositionX { get; set; }
		/// <summary> Velocity in Pixels per Second </summary>
		public float[] VelocityX { get; set; }
		/// <summary> Y World Space coordinates </summary>
		public float[] PositionY { get; set; }
		/// <summary> Velocity in Pixels per Second </summary>
		public float[] VelocityY { get; set; }
		/// <summary>  World space rotaiton in radians </summary>
		public float[] Rotation { get; set; }
		/// <summary> Rotational Velocity in Radias Per Second</summary>
		public float[] RotationalVelocity { get; set; }

		// --------- Scale ---------
	
		/// <summary> Scale </summary>
		public float[] ScaleX { get; set; }

		/// <summary>  Scale </summary>
		public float[] ScaleY { get; set; }

		// --------- Color ---------
		// Color is a packed uint

		/// <summary> Color </summary>
		public uint[] Color { get; set; }
	}
}