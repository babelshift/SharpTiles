using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpTiles
{
	public class Tile
	{
		private PropertyCollection properties = new PropertyCollection();

		public Rectangle Source { get; private set; }
		public PropertyCollection Properties { get { return properties; } }

		public Tile(Rectangle source, PropertyCollection properties)
		{
			Source = source;
			this.properties = properties;
		}
	}
}
