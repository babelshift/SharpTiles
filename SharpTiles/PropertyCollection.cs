using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SharpTiles
{
	internal class PropertyCollection : Dictionary<string, string>
	{
		public PropertyCollection() { }

		public PropertyCollection(XmlNode node)
		{
			foreach (XmlNode propertyNode in node.ChildNodes)
			{
				string propertyName = propertyNode.Attributes[AttributeNames.PropertyAttributes.Name].Value;
				string propertyValue = propertyNode.Attributes[AttributeNames.PropertyAttributes.Value].Value;

				bool isDuplicateFound = false;

				foreach (var property in this)
				{
					if (property.Key == propertyName)
					{
						if (property.Value == propertyValue)
							isDuplicateFound = true;
						else
							throw new Exception(String.Format("Duplicate properties of name {0} found with values {1} and {2}", propertyName, propertyValue, property.Value));
					}
				}

				if (!isDuplicateFound)
					Add(propertyName, propertyValue);
			}
		}
	}
}
