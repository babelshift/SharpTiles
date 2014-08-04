using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpTiles_Example1.Content
{
    /// <summary>
    /// Represents a single tile layer from a .tmx file. A tile layer contains 0 to many tiles.
    /// </summary>
    public class TileLayer : IDisposable
    {
        private List<Tile> tiles = new List<Tile>();

        /// <summary>
        /// The name given to the layer by the author of the map
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The width of the layer (usually the sum of all widths of tiles)
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// The height of the layer (usually the sum of all heights of tiles)
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// A read only indexable list of the tiles contained within the layer
        /// </summary>
        public IReadOnlyList<Tile> Tiles { get { return tiles; } }

        /// <summary>
        /// Default constructor requires a name, the width, and the height of the layer from the .tmx file.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public TileLayer(string name, int width, int height)
        {
            Name = name;
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Adds a single tile to the layer's collection of tiles.
        /// </summary>
        /// <param name="tile"></param>
        public void AddTile(Tile tile)
        {
            tiles.Add(tile);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Always dispose of the tile layers because they contain native textured tiles.
        /// </summary>
        /// <param name="isDisposing"></param>
        private void Dispose(bool isDisposing)
        {
            foreach (Tile tile in tiles)
            {
                tile.Dispose();
            }
        }
    }
}
