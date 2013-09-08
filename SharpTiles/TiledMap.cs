using SharpDL.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpTiles
{
	public class Tile
	{
		public Texture Texture { get; private set; }
		public Rectangle Source { get; private set; }
		public bool IsEmpty { get; private set; }

		public Tile()
		{
			IsEmpty = true;
		}

		public Tile(Texture texture, Rectangle source)
		{
			IsEmpty = false;
			Texture = texture;
			Source = source;
		}
	}

	public class TileLayer
	{
		private List<Tile> tiles = new List<Tile>();

		public int Width { get; private set; }
		public int Height { get; private set; }
		public IReadOnlyList<Tile> Tiles { get { return tiles; } }

		public TileLayer(int width, int height)
		{
			Width = width;
			Height = height;
		}

		public void AddTile(Tile tile)
		{
			tiles.Add(tile);
		}
	}

	public class TiledMap
	{
		private List<TileLayer> tileLayers = new List<TileLayer>();

		public int TileWidth { get; private set; }
		public int TileHeight { get; private set; }
		public IEnumerable<TileLayer> TileLayers { get { return tileLayers; } }

		public TiledMap(string filePath, Renderer renderer)
		{
			MapContent mapContent = new MapContent(filePath, renderer);

			TileWidth = mapContent.TileWidth;
			TileHeight = mapContent.TileHeight;

			foreach (Layer layer in mapContent.Layers)
			{
				TileLayerContent tileLayerContent = layer as TileLayerContent;
				if (tileLayerContent != null)
				{
					TileLayer tileLayer = new TileLayer(tileLayerContent.Width, tileLayerContent.Height);
					for (int i = 0; i < tileLayerContent.Data.Length; i++)
					{
						uint tileID = tileLayerContent.Data[i];

						uint flippedHorizontallyFlag = 0x80000000;
						uint flippedVerticallyFlag = 0x40000000;
						int tileIndex = (int)(tileID & ~(flippedVerticallyFlag | flippedHorizontallyFlag));

						Tile tile = new Tile();
						
						if (tileIndex > 0)
						{
							Texture tileSetTexture = null;
							Rectangle source = new Rectangle();
							foreach (TileSet tileSet in mapContent.TileSets)
							{
								if (tileIndex - tileSet.FirstGID < tileSet.Tiles.Count)
								{
									tileSetTexture = tileSet.Texture;
									source = tileSet.Tiles[(int)(tileIndex - tileSet.FirstGID)].Source;

									break;
								}
							}

							tile = new Tile(tileSetTexture, source);
						}
						
						tileLayer.AddTile(tile);
					}

					tileLayers.Add(tileLayer);
				}
			}
		}

		public void AddTileLayer(TileLayer tileLayer)
		{
			tileLayers.Add(tileLayer);
		}
	}
}
