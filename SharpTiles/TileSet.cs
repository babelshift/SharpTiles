using SharpDL.Graphics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SharpTiles
{
	internal class TileSet : IDisposable
	{
		private Dictionary<int, PropertyCollection> tileProperties = new Dictionary<int, PropertyCollection>();

		public int FirstGID { get; private set; }
		public string Name { get; private set; }
		public int TileWidth { get; private set; }
		public int TileHeight { get; private set; }
		public string ImageSource { get; private set; }
		public int Spacing { get; private set; }
		public int Margin { get; private set; }
		public Color? ColorKey { get; private set; }
		public IList<TileContent> Tiles { get; private set; }
		public Texture Texture { get; set; }
		public IReadOnlyDictionary<int, PropertyCollection> TileProperties { get { return tileProperties; } }

		public TileSet(XmlNode node)
		{
			Tiles = new List<TileContent>();

			int firstGID = 0;
			int.TryParse(node.Attributes[AttributeNames.TileSetAttributes.FirstGID].Value, NumberStyles.None, CultureInfo.InvariantCulture, out firstGID);
			FirstGID = firstGID;

			XmlNode preparedNode = PrepareXmlNode(node);
			Initialize(node);
		}

		private void Initialize(XmlNode node)
		{
			Name = node.Attributes[AttributeNames.TileSetAttributes.Name].Value;

			int tileWidth = 0;
			int.TryParse(node.Attributes[AttributeNames.TileSetAttributes.TileWidth].Value, NumberStyles.None, CultureInfo.InvariantCulture, out tileWidth);
			TileWidth = tileWidth;

			int tileHeight = 0;
			int.TryParse(node.Attributes[AttributeNames.TileSetAttributes.TileHeight].Value, NumberStyles.None, CultureInfo.InvariantCulture, out tileHeight);
			TileHeight = tileHeight;

			if (node.Attributes[AttributeNames.TileSetAttributes.Spacing] != null)
			{
				int spacing = 0;
				int.TryParse(node.Attributes[AttributeNames.TileSetAttributes.Spacing].Value, NumberStyles.None, CultureInfo.InvariantCulture, out spacing);
				Spacing = spacing;
			}

			if (node.Attributes[AttributeNames.TileSetAttributes.Margin] != null)
			{
				int margin = 0;
				int.TryParse(node.Attributes[AttributeNames.TileSetAttributes.Margin].Value, NumberStyles.None, CultureInfo.InvariantCulture, out margin);
				Margin = margin;
			}

			XmlNode imageNode = node[AttributeNames.TileSetAttributes.Image];
			ImageSource = imageNode.Attributes["source"].Value;

			if (ImageSource.StartsWith(".."))
				ImageSource = Path.GetFileName(ImageSource);

			if (imageNode.Attributes[AttributeNames.TileSetAttributes.Transparency] != null)
			{
				string colorCode = imageNode.Attributes[AttributeNames.TileSetAttributes.Transparency].Value;
				ColorKey = new Color(colorCode);
			}

			foreach(XmlNode tile in node.SelectNodes(AttributeNames.TileSetAttributes.Tile))
			{
				int id = 0;
				int.TryParse(tile.Attributes[AttributeNames.TileSetAttributes.ID].Value, NumberStyles.None, CultureInfo.InvariantCulture, out id);
				int tileId = FirstGID + id;

				PropertyCollection properties = new PropertyCollection();

				if (tile[AttributeNames.TileSetAttributes.Properties] != null)
					properties = new PropertyCollection(tile[AttributeNames.TileSetAttributes.Properties]);

				tileProperties.Add(id, properties);
			}
		}

		protected virtual XmlNode PrepareXmlNode(XmlNode root)
		{
			return root;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~TileSet()
		{
			Dispose(false);
		}

		private void Dispose(bool isDisposing)
		{
			if(Texture != null)
				Texture.Dispose();
		}
	}
}
