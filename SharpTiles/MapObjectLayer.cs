using SharpDL.Graphics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SharpTiles
{
	internal class MapObjectLayer : Layer
	{
		private List<MapObject> mapObjects = new List<MapObject>();

		public Color Color { get; private set; }
		public IEnumerable<MapObject> MapObjects { get { return mapObjects; } }

		public MapObjectLayer(XmlNode node)
			: base(node)
		{
			if (node.Attributes[AttributeNames.MapObjectLayerAttributes.Color] != null)
			{
				string color = node.Attributes[AttributeNames.MapObjectLayerAttributes.Color].Value.Substring(1);

				string redValue = color.Substring(0, 2);
				string greenValue = color.Substring(2, 2);
				string blueValue = color.Substring(4, 2);

				byte red = (byte)int.Parse(redValue, NumberStyles.AllowHexSpecifier);
				byte green = (byte)int.Parse(greenValue, NumberStyles.AllowHexSpecifier);
				byte blue = (byte)int.Parse(blueValue, NumberStyles.AllowHexSpecifier);

				Color = new Color(red, green, blue);
			}

			foreach (XmlNode objectNode in node.SelectNodes(AttributeNames.MapObjectLayerAttributes.Object))
			{
				string originalObjectName = String.Empty;
				if (objectNode.Attributes[AttributeNames.MapObjectAttributes.Name] != null)
				{
					originalObjectName = objectNode.Attributes[AttributeNames.MapObjectAttributes.Name].Value;

					string finalObjectName = originalObjectName;
					int duplicateCount = 2;

					while (mapObjects.Find(mo => mo.Name == finalObjectName) != null)
					{
						finalObjectName = String.Format("{0}{1}", originalObjectName, duplicateCount);
						duplicateCount++;
					}

					objectNode.Attributes[AttributeNames.MapObjectAttributes.Name].Value = finalObjectName;
				}
				MapObject mapObject = new MapObject(objectNode);
				mapObjects.Add(mapObject);
			}
		}
	}
}
