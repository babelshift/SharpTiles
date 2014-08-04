using SharpDL;
using SharpDL.Graphics;
using SharpTiles_Example1.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpTiles_Example1
{
    public class MainGame : Game
    {
        public const int SCREEN_WIDTH_LOGICAL = 1280;
        public const int SCREEN_HEIGHT_LOGICAL = 720;

        private TiledMap map;

        /// <summary>
        /// Initialize the window and the renderer.
        /// </summary>
        protected override void Initialize()
        {
            CreateWindow("SharpTiles, Example 1", 50, 50, SCREEN_WIDTH_LOGICAL, SCREEN_HEIGHT_LOGICAL, WindowFlags.Shown);
            CreateRenderer(RendererFlags.RendererAccelerated | RendererFlags.RendererPresentVSync);
            Renderer.SetRenderLogicalSize(SCREEN_WIDTH_LOGICAL, SCREEN_HEIGHT_LOGICAL);
        }

        /// <summary>
        /// Load the .tmx map when the game loads content
        /// </summary>
        protected override void LoadContent()
        {
            map = new TiledMap("Content/TileMap_Example1.tmx", Renderer, "Content");
        }

        /// <summary>
        /// Draw the map
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Draw(GameTime gameTime)
        {
            map.Draw(gameTime, Renderer);

            // draws the present context of the renderer to the window
            Renderer.RenderPresent();
        }
    }
}
