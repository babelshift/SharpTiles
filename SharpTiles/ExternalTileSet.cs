using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SharpTiles
{
	internal class ExternalTileSet : TileSet
	{
		public ExternalTileSet(XmlNode node)
			: base(node)
		{
		}

		protected override XmlNode PrepareXmlNode(XmlNode root)
		{
			XmlDocument externalTileSet = new XmlDocument();
			externalTileSet.Load(root.Attributes[AttributeNames.TileSetAttributes.Source].Value);
			return externalTileSet[AttributeNames.TileSetAttributes.TileSet];
		}
	}
}
