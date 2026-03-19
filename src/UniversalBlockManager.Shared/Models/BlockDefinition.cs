using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace UniversalBlockManager.Shared.Models
{
    [XmlRoot("BlockDefinition")]
    public class BlockDefinition
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string Units { get; set; }

        public Header Header { get; set; }

        [XmlArray("Layers")]
        [XmlArrayItem("Layer")]
        public List<Layer> Layers { get; set; } = new List<Layer>();

        [XmlArray("Attributes")]
        [XmlArrayItem("Attribute")]
        public List<BlockAttribute> Attributes { get; set; } = new List<BlockAttribute>();

        [XmlArray("Geometry")]
        [XmlArrayItem("Line", typeof(Line))]
        [XmlArrayItem("Circle", typeof(Circle))]
        [XmlArrayItem("Path", typeof(PathElement))]
        public List<GeometryElement> Geometry { get; set; } = new List<GeometryElement>();

        public void Save(string path)
        {
            var serializer = new XmlSerializer(typeof(BlockDefinition));
            using (var writer = new StreamWriter(path))
            {
                serializer.Serialize(writer, this);
            }
        }

        public static BlockDefinition Load(string path)
        {
            var serializer = new XmlSerializer(typeof(BlockDefinition));
            using (var reader = new StreamReader(path))
            {
                return (BlockDefinition)serializer.Deserialize(reader);
            }
        }
    }

    public class Header
    {
        public Point BasePoint { get; set; }
        public double ScaleFactor { get; set; }
    }

    public class Layer
    {
        [XmlAttribute]
        public string Name { get; set; }
    }

    public class BlockAttribute
    {
        [XmlAttribute]
        public string Key { get; set; }
        [XmlAttribute]
        public string Value { get; set; }
    }

    public class Point
    {
        [XmlAttribute]
        public double X { get; set; }
        [XmlAttribute]
        public double Y { get; set; }
    }

    public abstract class GeometryElement
    {
        [XmlAttribute]
        public string Layer { get; set; }
    }

    public class Line : GeometryElement
    {
        public Point Start { get; set; }
        public Point End { get; set; }
    }

    public class Circle : GeometryElement
    {
        public Point Center { get; set; }
        
        [XmlAttribute]
        public double Radius { get; set; }
    }

    public class PathElement : GeometryElement
    {
        [XmlAttribute]
        public string Data { get; set; }
    }
}
