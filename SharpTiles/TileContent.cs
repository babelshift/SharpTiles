using SharpDL.Graphics;

namespace SharpTiles
{
    internal class TileContent
    {
        private PropertyCollection properties = new PropertyCollection();

        public Rectangle SourceTextureBounds { get; private set; }

        public PropertyCollection Properties { get { return properties; } }

        public TileContent(Rectangle source, PropertyCollection properties)
        {
            SourceTextureBounds = source;
            this.properties = properties;
        }
    }
}