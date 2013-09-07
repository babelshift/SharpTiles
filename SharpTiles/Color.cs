﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpTiles
{
	public struct Color
	{
		public byte R { get; private set; }
		public byte G { get; private set; }
		public byte B { get; private set; }

		public Color(byte red, byte green, byte blue)
			: this()
		{
			R = red;
			G = green;
			B = blue;
		}

		public Color(string colorCode)
			: this()
		{
			string r = colorCode.Substring(0, 2);
			string g = colorCode.Substring(2, 2);
			string b = colorCode.Substring(4, 2);
			R = (byte)Convert.ToInt32(r, 16);
			G = (byte)Convert.ToInt32(g, 16);
			B = (byte)Convert.ToInt32(b, 16);
		}
	}
}
