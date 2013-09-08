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
	internal class TileSetContent : IDisposable
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
		public Texture Texture { get; set; }
		public IList<TileContent> Tiles { get; private set; }
		public IReadOnlyDictionary<int, PropertyCollection> TileProperties { get { return tileProperties; } }

		public TileSetContent(XmlNode tileSetNode)
		{
			Tiles = new List<TileContent>();

			FirstGID = Utilities.TryToParseInt(tileSetNode.Attributes[AttributeNames.TileSetAttributes.FirstGID].Value);

			XmlNode preparedNode = PrepareXmlNode(tileSetNode);
			Initialize(tileSetNode);
		}

		private void Initialize(XmlNode tileSetNode)
		{
			Name = tileSetNode.Attributes[AttributeNames.TileSetAttributes.Name].Value;

			TileWidth = Utilities.TryToParseInt(tileSetNode.Attributes[AttributeNames.TileSetAttributes.TileWidth].Value);
			TileHeight = Utilities.TryToParseInt(tileSetNode.Attributes[AttributeNames.TileSetAttributes.TileHeight].Value);

			if (tileSetNode.Attributes[AttributeNames.TileSetAttributes.Spacing] != null)
				Spacing = Utilities.TryToParseInt(tileSetNode.Attributes[AttributeNames.TileSetAttributes.Spacing].Value);

			if (tileSetNode.Attributes[AttributeNames.TileSetAttributes.Margin] != null)
				Margin = Utilities.TryToParseInt(tileSetNode.Attributes[AttributeNames.TileSetAttributes.Margin].Value);

			XmlNode imageNode = tileSetNode[AttributeNames.TileSetAttributes.Image];
			ImageSource = imageNode.Attributes[AttributeNames.TileSetAttributes.Source].Value;

			if (ImageSource.StartsWith(".."))
				ImageSource = Path.GetFileName(ImageSource);

			if (imageNode.Attributes[AttributeNames.TileSetAttributes.Transparency] != null)
			{
				string colorCode = imageNode.Attributes[AttributeNames.TileSetAttributes.Transparency].Value;
				ColorKey = new Color(colorCode);
			}

			foreach(XmlNode tile in tileSetNode.SelectNodes(AttributeNames.TileSetAttributes.Tile))
			{
				int id = Utilities.TryToParseInt(tile.Attributes[AttributeNames.TileSetAttributes.ID].Value);
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

		~TileSetContent()
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
