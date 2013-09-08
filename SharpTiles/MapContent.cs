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
	internal enum Orientation : byte
	{
		Orthogonal,
		Isometric
	}

	internal class MapContent : IDisposable
	{
		private PropertyCollection properties = new PropertyCollection();
		private List<TileSet> tileSets = new List<TileSet>();
		private List<Layer> layers = new List<Layer>();

		public string FileName { get; private set; }
		public string FileDirectory { get; private set; }
		public string Version { get; private set; }
		public Orientation Orientation { get; private set; }
		public int Width { get; private set; }
		public int Height { get; private set; }
		public int TileWidth { get; private set; }
		public int TileHeight { get; private set; }
		public PropertyCollection Properties { get { return properties; } }
		public IEnumerable<TileSet> TileSets { get { return tileSets; } }
		public IEnumerable<Layer> Layers { get { return layers; } }

		public MapContent(string filePath, Renderer renderer, string contentRoot = "")
		{
			XmlDocument document = new XmlDocument();
			document.Load(filePath);

			XmlNode mapNode = document[AttributeNames.MapAttributes.Map];

			Version = mapNode.Attributes[AttributeNames.MapAttributes.Version].Value;

			Orientation = (Orientation)Enum.Parse(typeof(Orientation),
				mapNode.Attributes[AttributeNames.MapAttributes.Orientation].Value, true);

			int width = 0;
			int.TryParse(mapNode.Attributes[AttributeNames.MapAttributes.Width].Value, NumberStyles.None, CultureInfo.InvariantCulture, out width);
			Width = width;

			int height = 0;
			int.TryParse(mapNode.Attributes[AttributeNames.MapAttributes.Height].Value, NumberStyles.None, CultureInfo.InvariantCulture, out height);
			Height = height;

			int tileWidth = 0;
			int.TryParse(mapNode.Attributes[AttributeNames.MapAttributes.TileWidth].Value, NumberStyles.None, CultureInfo.InvariantCulture, out tileWidth);
			TileWidth = tileWidth;

			int tileHeight = 0;
			int.TryParse(mapNode.Attributes[AttributeNames.MapAttributes.TileHeight].Value, NumberStyles.None, CultureInfo.InvariantCulture, out tileHeight);
			TileHeight = tileHeight;

			XmlNode propertiesNode = document.SelectSingleNode(AttributeNames.MapAttributes.MapProperties);
			if (propertiesNode != null)
				properties = new PropertyCollection(propertiesNode);

			foreach (XmlNode tileSetNode in document.SelectNodes(AttributeNames.MapAttributes.MapTileSet))
			{
				if (tileSetNode.Attributes[AttributeNames.MapAttributes.Source] != null)
					tileSets.Add(new ExternalTileSet(tileSetNode));
				else
					tileSets.Add(new TileSet(tileSetNode));
			}

			foreach(XmlNode layerNode in document.SelectNodes(AttributeNames.MapAttributes.MapTileLayer + "|" + AttributeNames.MapAttributes.MapObjectLayer))
			{
				Layer layer;
				if (layerNode.Name == AttributeNames.MapAttributes.TileLayer)
					layer = new TileLayerContent(layerNode);
				else if (layerNode.Name == AttributeNames.MapAttributes.ObjectLayer)
					layer = new MapObjectLayer(layerNode);
				else
					throw new Exception(String.Format("Unknown layer: {0}", layerNode.Name));

				string layerName = layer.Name;
				int duplicateCount = 2;

				while (layers.Any(l => l.Name == layerName))
				{
					layerName = String.Format("{0}{1}", layer.Name, duplicateCount);
					duplicateCount++;
				}

				layer.Name = layerName;

				layers.Add(layer);
			}

			BuildTileSetTextures(renderer, contentRoot);

			GenerateTileSourceRectangles(contentRoot);
		}

		private void BuildTileSetTextures(Renderer renderer, string contentRoot)
		{
			// build textures
			foreach (TileSet tileSet in tileSets)
			{
				string path = Path.Combine(contentRoot, tileSet.ImageSource);

				string assetPath = String.Empty;
				if (path.StartsWith(Directory.GetCurrentDirectory()))
					assetPath = path.Remove(tileSet.ImageSource.LastIndexOf('.')).Substring(Directory.GetCurrentDirectory().Length + 1);
				else
					assetPath = path;

				// need to use colorkey

				Surface surface = new Surface(assetPath, Surface.SurfaceType.PNG);
				tileSet.Texture = new Texture(renderer, surface);
			}
		}

		private void GenerateTileSourceRectangles(string contentRoot)
		{
			// process the tilesets, calculate tiles to fit in each set, calculate source rectangles
			foreach (TileSet tileSet in tileSets)
			{
				string path = Path.Combine(contentRoot, tileSet.ImageSource);

				int imageWidth = tileSet.Texture.Width;
				int imageHeight = tileSet.Texture.Height;

				imageWidth -= tileSet.Margin * 2;
				imageHeight -= tileSet.Margin * 2;

				int tileCountX = 0;
				while (tileCountX * tileSet.TileWidth < imageWidth)
				{
					tileCountX++;
					imageWidth -= tileSet.Spacing;
				}

				int tileCountY = 0;
				while (tileCountY * tileSet.TileHeight < imageHeight)
				{
					tileCountY++;
					imageHeight -= tileSet.Spacing;
				}

				for (int y = 0; y < tileCountY; y++)
				{
					for (int x = 0; x < tileCountX; x++)
					{
						int rx = tileSet.Margin + x * (tileSet.TileWidth + tileSet.Spacing);
						int ry = tileSet.Margin + y * (tileSet.TileHeight + tileSet.Spacing);
						Rectangle source = new Rectangle(rx, ry, tileSet.TileWidth, tileSet.TileHeight);

						int index = tileSet.FirstGID + (y * tileCountX + x);
						PropertyCollection tileProperties = new PropertyCollection();
						if (tileSet.TileProperties.ContainsKey(index))
							tileProperties = tileSet.TileProperties[index];

						TileContent tile = new TileContent(source, tileProperties);
						tileSet.Tiles.Add(tile);
					}
				}
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~MapContent()
		{
			Dispose(false);
		}

		private void Dispose(bool isDisposing)
		{
			foreach (TileSet tileSet in tileSets)
				tileSet.Dispose();
		}
	}
}
