using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SharpTiles
{
	internal abstract class Layer
	{
		private PropertyCollection properties = new PropertyCollection();

		public string Name { get; set; }
		public string Type { get; private set; }
		public int Width { get; private set; }
		public int Height { get; private set; }
		public float Opacity { get; private set; }
		public bool Visible { get; private set; }
		public PropertyCollection Properties { get { return properties; } }

		public Layer(XmlNode node)
		{
			Type = node.Name;
			Name = node.Attributes[AttributeNames.LayerAttributes.Name].Value;

			int tempWidth = 0;
			bool success = int.TryParse(node.Attributes[AttributeNames.LayerAttributes.Width].Value, NumberStyles.None, CultureInfo.InvariantCulture, out tempWidth);
			if (success)
				Width = tempWidth;

			int tempHeight = 0;
			success = int.TryParse(node.Attributes[AttributeNames.LayerAttributes.Height].Value, NumberStyles.None, CultureInfo.InvariantCulture, out tempHeight);
			if (success)
				Height = tempHeight;

			if (node.Attributes[AttributeNames.LayerAttributes.Opacity] != null)
			{
				float tempOpacity = 0f;
				success = float.TryParse(node.Attributes[AttributeNames.LayerAttributes.Opacity].Value, NumberStyles.None, CultureInfo.InvariantCulture, out tempOpacity);
				if (success)
					Opacity = tempOpacity;
			}

			if (node.Attributes[AttributeNames.LayerAttributes.Visible] != null)
			{
				bool tempVisible = false;
				success = bool.TryParse(node.Attributes[AttributeNames.LayerAttributes.Visible].Value, out tempVisible);
			}

			if (node.Attributes[AttributeNames.LayerAttributes.Properties] != null)
				properties = new PropertyCollection(node.Attributes[AttributeNames.LayerAttributes.Properties]);
		}
	}
}
