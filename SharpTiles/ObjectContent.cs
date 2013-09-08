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
	internal enum MapObjectType : byte
	{
		Plain,
		Tile,
		Polygon,
		Polyline
	}

	internal class ObjectContent
	{
		private PropertyCollection properties = new PropertyCollection();
		private List<Point> points = new List<Point>();
		private MapObjectType objectType = MapObjectType.Plain;

		public MapObjectType ObjectType { get { return objectType; } }
		public string Name { get; private set; }
		public string Type { get; private set; }
		public int X { get; private set; }
		public int Y { get; private set; }
		public int Width { get; private set; }
		public int Height { get; private set; }
		public int GID { get; private set; }
		public IEnumerable<Point> Points { get { return points; } }
		public PropertyCollection Properties { get { return properties; } }

		public ObjectContent(XmlNode node)
		{
			if (node.Attributes[AttributeNames.MapObjectAttributes.Name] != null)
				Name = node.Attributes[AttributeNames.MapObjectAttributes.Name].Value;
			
			if (node.Attributes[AttributeNames.MapObjectAttributes.Type] != null)
				Type = node.Attributes[AttributeNames.MapObjectAttributes.Type].Value;

			if (node[AttributeNames.MapObjectAttributes.Properties] != null)
				properties = new PropertyCollection(node[AttributeNames.MapObjectAttributes.Properties]);

			if (node.Attributes[AttributeNames.MapObjectAttributes.X] != null)
				X = Utilities.TryToParseInt(node.Attributes[AttributeNames.MapObjectAttributes.X].Value);

			if (node.Attributes[AttributeNames.MapObjectAttributes.Y] != null)
				Y = Utilities.TryToParseInt(node.Attributes[AttributeNames.MapObjectAttributes.Y].Value);

			if (node.Attributes[AttributeNames.MapObjectAttributes.Width] != null)
				Width = Utilities.TryToParseInt(node.Attributes[AttributeNames.MapObjectAttributes.Width].Value);

			if (node.Attributes[AttributeNames.MapObjectAttributes.Height] != null)
				Height = Utilities.TryToParseInt(node.Attributes[AttributeNames.MapObjectAttributes.Height].Value);

			string objectPoints = String.Empty;

			if (node.Attributes[AttributeNames.MapObjectAttributes.GID] != null)
			{
				objectType = MapObjectType.Tile;
				GID = Utilities.TryToParseInt(node.Attributes[AttributeNames.MapObjectAttributes.GID].Value);
			}
			else if (node.Attributes[AttributeNames.MapObjectAttributes.Polygon] != null)
			{
				objectType = MapObjectType.Polygon;
				objectPoints = node.Attributes[AttributeNames.MapObjectAttributes.Polygon].Value;
			}
			else if (node.Attributes[AttributeNames.MapObjectAttributes.Polyline] != null)
			{
				objectType = MapObjectType.Polyline;
				objectPoints = node.Attributes[AttributeNames.MapObjectAttributes.Polyline].Value;
			}

			if(!String.IsNullOrEmpty(objectPoints))
			{
				string[] splitPoints = objectPoints.Split(' ');
				foreach(string splitPoint in splitPoints)
				{
					string[] coordinates = splitPoint.Split(',');

					float x, y = 0f;
					float.TryParse(coordinates[0], NumberStyles.None, CultureInfo.InvariantCulture, out x);
					float.TryParse(coordinates[1], NumberStyles.None, CultureInfo.InvariantCulture, out y);

					points.Add(new Point(x, y));
				}
			}
		}
	}
}
