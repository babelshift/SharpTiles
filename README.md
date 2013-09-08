# SharpTiles

SharpTiles is a C# library used to load Tiled Map Editor's in a format used to render via SDL2. This library is heavily based on an XNA implementation created by Nick Gravelyn who has since dropped support for the project. You can find the XNA implementation on [GitHub](https://github.com/babelshift/TiledLib).

## Overview

SharpTiles allows you to deserialize, process, and render maps created with the [Tiled Map Editor](http://www.mapeditor.org/). You are able to load tile layers, object layers, tileset textures, and tiles. The library supports reading maps that have been saved with gzip or zlib compression from within the Tiled Map Editor.

**It is important to emphasize that SharpTiles utilizes SharpDL (which utilizes SDL2) there are native resources involved. Because SDL2 is a native C library, objects such as Textures, Surfaces, Renderers, and Windows will be maintained in native (non-managed) areas.**

## Prerequisites

The maps generated by this library are for use with SDL2 and require both the base [SDL2 library](http://www.libsdl.org/) and an [XNA-like framework named SharpDL](https://github.com/babelshift/SharpDL). More information about SharpDL is provided at its GitHub page.

## Examples

### Load a Map

* Window is a SharpDL object that represents a [SDL_Window](http://wiki.libsdl.org/SDL_CreateWindow?highlight=%28\bCategoryAPI\b%29|%28SDLFunctionTemplate%29)
* Renderer is a SharpDL object that represents a [SDL_Renderer](http://wiki.libsdl.org/SDL_CreateRenderer?highlight=%28\bCategoryAPI\b%29|%28SDLFunctionTemplate%29)
* TiledMap is the main object that you will use to load a map file

```C#
// create an SDL window at position 100,100 with size 640x480
Window window = new Window("Example 1", 100, 100, 640, 480, Window.WindowFlags.Shown)

// create an SDL rendering context based on the window above
Renderer renderer = new Renderer(Renderer.RendererFlags.RendererAccelerated);

// load and create map to be displayed in the passed Renderer
TiledMap map = new TiledMap("Maps/Map1.tmx", Renderer);
```

### Draw a Map

You can draw a map by looping through all TileLayers (layers which containd textured tiles) and all Tiles within the layers. Make sure to check if the tile is empty before drawing it. Tiled Map Editor will create empty tiles in spots that contain no textured tiles within the layer.

* Texture is the tileset that contains the tile's texture
* Position.X is the X position of the tile to draw
* Position.Y is the Y position of the tile to draw
* SourceTextureBounds is a rectangle that indicates where in the tileset the tile's actual texture is contained

```C#
foreach (TileLayer tileLayer in map.TileLayers)
    foreach (Tile tile in tileLayer.Tiles)
        if (!tile.IsEmpty)
            Renderer.RenderTexture(tile.Texture, (int)tile.Position.X, (int)tile.Position.Y, tile.SourceTextureBounds);
```
