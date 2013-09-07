using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SharpTiles
{
	public class PropertyCollection : Dictionary<string, string>
	{
		public PropertyCollection() { }

		public PropertyCollection(XmlNode node)
		{
			foreach (XmlNode property in node.ChildNodes)
			{
				string name = property.Attributes[AttributeNames.PropertyAttributes.Name].Value;
				string value = property.Attributes[AttributeNames.PropertyAttributes.Value].Value;

				bool isDuplicateFound = false;

				foreach (var p in this)
				{
					if (p.Key == name)
					{
						if (p.Value == value)
							isDuplicateFound = true;
						else
							throw new Exception(String.Format("Duplicate properties of name {0} found with values {1} and {2}", name, value, p.Value));
					}
				}

				if (!isDuplicateFound)
					Add(name, value);
			}
		}
	}
}
