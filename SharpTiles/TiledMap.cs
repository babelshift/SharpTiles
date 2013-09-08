using SharpDL.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpTiles
{
	/// <summary>A Tile is a representation of a Tile in a .tmx file. These tiles contain textures and positions in order to render the map properly.
	/// </summary>
	public class Tile
	{
		public Point Position { get; internal set; }
		public Texture Texture { get; private set; }
		public Rectangle SourceTextureBounds { get; private set; }
		public bool IsEmpty { get; private set; }

		public Tile()
		{
			IsEmpty = true;
		}

		public Tile(Texture texture, Rectangle source)
		{
			IsEmpty = false;
			Texture = texture;
			SourceTextureBounds = source;
		}
	}

	/// <summary>A TileLayer is a representation of a tile layer in a .tmx file. These layers contain Tile objects which can be accessed
	/// in order to render the textures associated with the tiles.
	/// </summary>
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

	/// <summary>A TiledMap is a representation of a .tmx map created with the Tiled Map Editor. You can access layers and tiles within
	/// layers by accessing the properties of this class after instantiation.
	/// </summary>
	public class TiledMap
	{
		private List<TileLayer> tileLayers = new List<TileLayer>();

		public int TileWidth { get; private set; }
		public int TileHeight { get; private set; }
		public IEnumerable<TileLayer> TileLayers { get { return tileLayers; } }

		/// <summary>Default constructor creates a map from a .tmx file and creates any associated tileset textures by using the passed renderer.
		/// </summary>
		/// <param name="filePath">Path to the .tmx file to load</param>
		/// <param name="renderer">Renderer object used to load tileset textures</param>
		public TiledMap(string filePath, Renderer renderer)
		{
			MapContent mapContent = new MapContent(filePath, renderer);

			TileWidth = mapContent.TileWidth;
			TileHeight = mapContent.TileHeight;

			foreach (LayerContent layer in mapContent.Layers)
			{
				TileLayerContent tileLayerContent = layer as TileLayerContent;
				if (tileLayerContent != null)
				{
					TileLayer tileLayer = new TileLayer(tileLayerContent.Width, tileLayerContent.Height);
					for (int i = 0; i < tileLayerContent.Data.Length; i++)
					{
						// strip out the flipped flags from the map editor to get the real tile index
						uint tileID = tileLayerContent.Data[i];
						uint flippedHorizontallyFlag = 0x80000000;
						uint flippedVerticallyFlag = 0x40000000;
						int tileIndex = (int)(tileID & ~(flippedVerticallyFlag | flippedHorizontallyFlag));

						Tile tile = CreateTile(tileIndex, mapContent.TileSets);
						
						tileLayer.AddTile(tile);
					}

					tileLayers.Add(tileLayer);
				}
			}

			CalculateTilePositions();
		}

		/// <summary>Based on a passed tile index, create a Tile by looking up which TileSet it belongs to, assign the proper TilSet texture,
		/// and find the bounds of the rectangle that encompasses the correct tile texture within the total tileset texture.
		/// </summary>
		/// <param name="tileIndex">Index of the tile (GID) within the map file</param>
		/// <param name="tileSets">Enumerable list of tilesets used to find out which tileset a tile belongs to</param>
		/// <returns></returns>
		private Tile CreateTile(int tileIndex, IEnumerable<TileSetContent> tileSets)
		{
			Tile tile = new Tile();

			// we don't want to look up tiles with ID 0 in tile sets because Tiled Map Editor treats ID 0 as an empty tile
			if (tileIndex > 0)
			{
				Texture tileSetTexture = null;
				Rectangle source = new Rectangle();
				foreach (TileSetContent tileSet in tileSets)
				{
					if (tileIndex - tileSet.FirstGID < tileSet.Tiles.Count)
					{
						tileSetTexture = tileSet.Texture;
						source = tileSet.Tiles[(int)(tileIndex - tileSet.FirstGID)].SourceTextureBounds;
						break;
					}
				}

				tile = new Tile(tileSetTexture, source);
			}

			return tile;
		}

		/// <summary>Loop through all tiles in all tile layers and calculate their X,Y coordinates. This will be used
		/// by renderers to paint the textures in the correct position of the rendering target.
		/// </summary>
		private void CalculateTilePositions()
		{
			foreach (TileLayer tileLayer in tileLayers)
			{
				for (int y = 0; y < tileLayer.Height; y++)
				{
					for (int x = 0; x < tileLayer.Width; x++)
					{
						Tile tile = tileLayer.Tiles[y * tileLayer.Width + x];
						tile.Position = new Point((float)(x * TileWidth), (float)(y * TileHeight));
					}
				}
			}
		}
	}
}
