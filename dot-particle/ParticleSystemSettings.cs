using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Particles
{

	public class ParticleSystemSettings
	{
		// System Settings
		public int MaxParticles { get; set; } = 64;

		// Particle Settings
		public LookUpCurve<float> StartLifetime { get; set; } = 1f;
		public LookUpCurve<float> StartSpeed { get; set; } = 1f;
		public LookUpCurve<float> StartRotationalSpeed { get; set; } = 0;
		public LookUpCurve<float> StartSize { get; set; } = 1f;
		public LookUpCurve<float> StartRotation { get; set; } = 0f;
		public LookUpCurve<Color> StartColor { get; set; } = Color.White;

		// Velocity over lifetime


		// Limit velocity over lifetime
		public LookUpCurve<float> Drag { get; set; } = 1;

		public LookUpCurve<float> RotationalDrag { get; set; } = 1;
		// Size over Lifetime
		public LookUpCurve<float> SizeOverLifetime { get; set; } = 1;
		// Size by Speed

		// Rotation over Lifetime

		// Rotation by Speed




		// Color over Lifetime
		//public Gradient ColorOverLifetime { get; set; }
		// Color by Speed


		// Rendering
		// Texture Sheet Animation
		public SheetRenderSettings RenderSettings { get; set; }
	}
}
