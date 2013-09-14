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
	public class Tile : IDisposable
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

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~Tile()
		{
			Dispose(false);
		}

		private void Dispose(bool isDisposing)
		{
			if (Texture != null)
				Texture.Dispose();
		}
	}

	/// <summary>A TileLayer is a representation of a tile layer in a .tmx file. These layers contain Tile objects which can be accessed
	/// in order to render the textures associated with the tiles.
	/// </summary>
	public class TileLayer : IDisposable
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

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~TileLayer()
		{
			Dispose(false);
		}

		private void Dispose(bool isDisposing)
		{
			foreach (Tile tile in tiles)
				tile.Dispose();
		}
	}

	public class MapObject
	{
		public string Name { get; private set; }
		public Rectangle Bounds { get; private set; }
		public bool IsCollidable { get; private set; }

		public MapObject(string name, Rectangle bounds, bool isCollidable)
		{
			Name = name;
			Bounds = bounds;
			IsCollidable = isCollidable;
		}
	}

	public class MapObjectLayer
	{
		private List<MapObject> mapObjects = new List<MapObject>();

		public string Name { get; private set; }
		public IEnumerable<MapObject> MapObjects { get { return mapObjects; } }

		public MapObjectLayer(string name)
		{
			Name = name;
		}

		public void AddMapObject(MapObject mapObject)
		{
			mapObjects.Add(mapObject);
		}
	}

	/// <summary>A TiledMap is a representation of a .tmx map created with the Tiled Map Editor. You can access layers and tiles within
	/// layers by accessing the properties of this class after instantiation.
	/// </summary>
	public class TiledMap : IDisposable
	{
		private List<TileLayer> tileLayers = new List<TileLayer>();
		private List<MapObjectLayer> mapObjectLayers = new List<MapObjectLayer>();

		public int TileWidth { get; private set; }
		public int TileHeight { get; private set; }
		public IEnumerable<TileLayer> TileLayers { get { return tileLayers; } }
		public IEnumerable<MapObjectLayer> MapObjectLayers { get { return mapObjectLayers; } }

		/// <summary>Default constructor creates a map from a .tmx file and creates any associated tileset textures by using the passed renderer.
		/// </summary>
		/// <param name="filePath">Path to the .tmx file to load</param>
		/// <param name="renderer">Renderer object used to load tileset textures</param>
		public TiledMap(string filePath, Renderer renderer, string contentRoot = "")
		{
			MapContent mapContent = new MapContent(filePath, renderer, contentRoot);

			TileWidth = mapContent.TileWidth;
			TileHeight = mapContent.TileHeight;

			foreach (LayerContent layer in mapContent.Layers)
			{
				if(layer is TileLayerContent)
				{
					TileLayerContent tileLayerContent = layer as TileLayerContent;
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
				else if (layer is ObjectLayerContent)
				{
					ObjectLayerContent objectLayerContent = layer as ObjectLayerContent;

					bool isCollidable = false;
					if(objectLayerContent.Name == "Collidables")
						isCollidable = true;

					MapObjectLayer mapObjectLayer = new MapObjectLayer(objectLayerContent.Name);

					foreach (ObjectContent objectContent in objectLayerContent.MapObjects)
					{
						MapObject mapObject = new MapObject(objectContent.Name, objectContent.Bounds, isCollidable);
						mapObjectLayer.AddMapObject(mapObject);
					}

					mapObjectLayers.Add(mapObjectLayer);
				}
			}

			CalculateTilePositions(mapContent.Orientation);
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
		private void CalculateTilePositions(Orientation mapOrientation)
		{
			foreach (TileLayer tileLayer in tileLayers)
			{
				for (int x = 0; x < tileLayer.Width; x++)
				{
					for (int y = 0; y < tileLayer.Height; y++)
					{
						Tile tile = tileLayer.Tiles[x * tileLayer.Height + y];

						Point position = new Point(0, 0);
						if (mapOrientation == Orientation.Isometric)
						{
							int positionX = (y * TileWidth / 2) - (x * TileWidth / 2);
							int positionY = (x * TileHeight / 2) + (y * TileHeight / 2);

							//TODO FIX THIS
							positionX += 300;

							position = new Point(positionX, positionY);
						}
						else if (mapOrientation == Orientation.Orthogonal)
						{
							position = new Point(x * TileWidth, y * TileHeight);
						}

						tile.Position = position;
					}
				}
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~TiledMap()
		{
			Dispose(false);
		}

		private void Dispose(bool isDisposing)
		{
			foreach (TileLayer tileLayer in tileLayers)
				tileLayer.Dispose();
		}
	}
}
