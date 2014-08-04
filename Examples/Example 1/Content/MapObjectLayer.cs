using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpTiles_Example1.Content
{
    /// <summary>
    /// Represents a single object layer from a .tmx file. An object layer contains 0 to many objects.
    /// </summary>
    public class MapObjectLayer
    {
        private List<MapObject> mapObjects = new List<MapObject>();

        /// <summary>
        /// The name given to the layer by the author of the map
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// A read only collection of objects contained within the layer
        /// </summary>
        public IReadOnlyCollection<MapObject> MapObjects { get { return mapObjects; } }
        
        /// <summary>
        /// The default constructor requires a name
        /// </summary>
        /// <param name="name"></param>
        public MapObjectLayer(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Adds a single object to the layer's collection of objects
        /// </summary>
        /// <param name="mapObject"></param>
        public void AddMapObject(MapObject mapObject)
        {
            mapObjects.Add(mapObject);
        }
    }
}
