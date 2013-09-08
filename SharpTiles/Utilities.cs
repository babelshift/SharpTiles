using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpTiles
{
	public static class Utilities
	{
		public static int TryToParseInt(string valueToParse)
		{
			int tempValue = 0;
			bool success = int.TryParse(valueToParse, NumberStyles.None, CultureInfo.InvariantCulture, out tempValue);
			if (success)
				return tempValue;
			else
				throw new Exception(String.Format("Failed to parse value {0} to int", valueToParse));
		}

		public static bool TryToParseBool(string valueToParse)
		{
			bool tempValue = false;
			bool success = bool.TryParse(valueToParse, out tempValue);
			if (success)
				return tempValue;
			else
				throw new Exception(String.Format("Failed to parse value {0} to bool", valueToParse));
		}

		public static float TryToParseFloat(string valueToParse)
		{
			float tempOpacity = 0f;
			bool success = float.TryParse(valueToParse, NumberStyles.None, CultureInfo.InvariantCulture, out tempOpacity);
			if (success)
				return tempOpacity;
			else
				throw new Exception(String.Format("Failed to parse value {0} to float", valueToParse));
		}
	}
}
