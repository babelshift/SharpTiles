using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SharpTiles
{
	internal class TileLayerContent : Layer
	{
		private uint[] data;

		public uint[] Data { get { return data; } }

		public TileLayerContent(XmlNode node)
			: base(node)
		{
			XmlNode dataNode = node[AttributeNames.TileLayerAttributes.Data];
			data = new uint[Width * Height];

			if (dataNode.Attributes[AttributeNames.TileLayerAttributes.Encoding] != null)
			{
				string encoding = dataNode.Attributes[AttributeNames.TileLayerAttributes.Encoding].Value;

				if (encoding == "base64")
					ReadAsBase64(node, dataNode);
				else if (encoding == "csv")
					ReadAsCSV(node);
				else
					throw new Exception(String.Format("Unsupported encoding type: {0}", encoding));
			}
		}

		private void ReadAsBase64(XmlNode node, XmlNode dataNode)
		{
			Stream uncompressedData = new MemoryStream(Convert.FromBase64String(node.InnerText), false);

			if (dataNode.Attributes[AttributeNames.TileLayerAttributes.Compression] != null)
			{
				string compression = dataNode.Attributes[AttributeNames.TileLayerAttributes.Compression].Value;

				if (compression == "gzip")
					uncompressedData = new GZipStream(uncompressedData, CompressionMode.Decompress, false);
				else if (compression == "zlib")
					uncompressedData = new Ionic.Zlib.ZlibStream(uncompressedData, Ionic.Zlib.CompressionMode.Decompress, false);
				else
					throw new InvalidOperationException(String.Format("Unsupported compression type: {0}", compression));
			}

			using (uncompressedData)
			{
				using (BinaryReader reader = new BinaryReader(uncompressedData))
				{
					for (int i = 0; i < data.Length; i++)
						data[i] = reader.ReadUInt32();
				}
			}
		}

		private void ReadAsCSV(XmlNode node)
		{
			string[] lines = node.InnerText.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

			for(int i = 0; i < lines.Length; i++)
			{
				string[] indices = lines[i].Split(new [] { ',' }, StringSplitOptions.RemoveEmptyEntries);

				for(int j = 0; j < indices.Length; j++)
				{
					uint index = 0;
					uint.TryParse(indices[j], NumberStyles.None, CultureInfo.InvariantCulture, out index);
					Data[i * Width + j] = index;
				}
			}
		}
	}
}
