using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Particles
{
	// Similar implementation as MonoGame.Framework.Color
	public struct Color : ILerpable<Color>
	{
		public uint PackedValue => packedValue;
		private uint packedValue;

        public Color(byte r, byte g, byte b, byte a)
        {
			packedValue = (uint)((a << 24) | (b << 16) | (g << 8) | (r));
		}
        /// <summary>
        /// Gets or sets the blue component.
        /// </summary>
        
        public byte B
        {
            get
            {
                unchecked
                {
                    return (byte)(this.packedValue >> 16);
                }
            }
            set
            {
                this.packedValue = (this.packedValue & 0xff00ffff) | ((uint)value << 16);
            }
        }

        /// <summary>
        /// Gets or sets the green component.
        /// </summary>
        
        public byte G
        {
            get
            {
                unchecked
                {
                    return (byte)(this.packedValue >> 8);
                }
            }
            set
            {
                this.packedValue = (this.packedValue & 0xffff00ff) | ((uint)value << 8);
            }
        }

        /// <summary>
        /// Gets or sets the red component.
        /// </summary>
        
        public byte R
        {
            get
            {
                unchecked
                {
                    return (byte)this.packedValue;
                }
            }
            set
            {
                this.packedValue = (this.packedValue & 0xffffff00) | value;
            }
        }

        /// <summary>
        /// Gets or sets the alpha component.
        /// </summary>
        
        public byte A
        {
            get
            {
                unchecked
                {
                    return (byte)(this.packedValue >> 24);
                }
            }
            set
            {
                this.packedValue = (this.packedValue & 0x00ffffff) | ((uint)value << 24);
            }
        }

        public Color Lerp(Color a, Color b, float t)
		{
			return new Color(
                (byte)(a.R + (b.R - a.R) * t),
                (byte)(a.G + (b.G - a.G) * t),
                (byte)(a.B + (b.B - a.B) * t),
                (byte)(a.A + (b.A - a.A) * t)
                );
		}
	}
}
