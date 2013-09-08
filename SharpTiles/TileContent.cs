using SharpDL.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpTiles
{
	internal class TileContent
	{
		private PropertyCollection properties = new PropertyCollection();

		public Rectangle SourceTextureBounds { get; private set; }
		public PropertyCollection Properties { get { return properties; } }

		public TileContent(Rectangle source, PropertyCollection properties)
		{
			SourceTextureBounds = source;
			this.properties = properties;
		}
	}
}
