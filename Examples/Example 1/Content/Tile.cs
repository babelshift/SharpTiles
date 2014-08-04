using SharpDL;
using SharpDL.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpTiles_Example1.Content
{
    /// <summary>
    /// Represents a single tile from a .tmx file in Tiled Map Editor. A tile can contain a texture and properties.
    /// </summary>
    public class Tile : IDisposable
    {
        // Tiled Editor assigns the id of '0' to tiles with no textures
        public const int EmptyTileID = 0;

        /// <summary>
        /// Unique auto-generated GUID to identify a tile (not assigned by Tiled Editor)
        /// </summary>
        public Guid ID { get; private set; }

        /// <summary>
        /// Width of the tile in pixels
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Height of the tile in pixels
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// The position on the [x,y] coordinate tile map where this tile is located.
        /// </summary>
        public Point GridPosition { get; set; }

        /// <summary>
        /// Texture from which to select a rectangle source texture (similar to selecting from a sprite sheet)
        /// </summary>
        public Texture Texture { get; private set; }

        /// <summary>
        /// Rectangle determining where in the Texture to select the specific texture for our sprite or frame
        /// </summary>
        public Rectangle SourceTextureBounds { get; private set; }

        /// <summary>
        /// Tiles are empty if they have no texture assigned within Tiled Map Editor
        /// </summary>
        public bool IsEmpty { get; private set; }

        /// <summary>
        /// Default empty constructor creates an empty tile
        /// </summary>
        public Tile()
        {
            ID = Guid.NewGuid();
            IsEmpty = true;
        }

        /// <summary>
        /// Main constructor used to instantiate a tile when data is known at import
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="source"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Tile(Texture texture, Rectangle source, int width, int height)
        {
            ID = Guid.NewGuid();
            IsEmpty = false;
            Texture = texture;
            SourceTextureBounds = source;
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Draws the tile to the passed renderer if the tile is not empty. The draw will occur at the center of the tile's texture.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="renderer"></param>
        public void Draw(GameTime gameTime, Renderer renderer)
        {
            if (IsEmpty) return;

            Texture.Draw(
                GridPosition.X * Width,
                GridPosition.Y * Height,
                SourceTextureBounds);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Always dispose of the texture because it's instantiated in native code.
        /// </summary>
        /// <param name="isDisposing"></param>
        private void Dispose(bool isDisposing)
        {
            if (Texture != null)
                Texture.Dispose();
        }
    }
}
