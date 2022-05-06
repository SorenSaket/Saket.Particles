using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Particles
{
	/// <summary>
	/// 
	/// </summary>
	public class ParticleSystemSettings
	{
		// Velocity over lifetime


		// Limit velocity over lifetime
		public LookUpCurve<float> Drag { get; set; } = 1;

		public LookUpCurve<float> RotationalDrag { get; set; } = 1;

		public bool MultiplyBySize { get; set; } = true;
		public bool MultiplyByVelocity { get; set; } = true;



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
